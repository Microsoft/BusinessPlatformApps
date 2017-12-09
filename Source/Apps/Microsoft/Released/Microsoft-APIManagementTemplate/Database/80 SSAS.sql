SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
go


-- =============================================
-- Pre Setup - Remove table from previous installation
-- =============================================
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='pbist_apimgmt' AND TABLE_NAME='ssas_jobs' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE pbist_apimgmt.ssas_jobs;
	
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_SCHEMA='pbist_apimgmt' AND ROUTINE_NAME='sp_set_process_flag' AND ROUTINE_TYPE='PROCEDURE')
    DROP PROCEDURE [pbist_apimgmt].sp_set_process_flag;
	
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_SCHEMA='pbist_apimgmt' AND ROUTINE_NAME='sp_validate_schema' AND ROUTINE_TYPE='PROCEDURE')
    DROP PROCEDURE [pbist_apimgmt].sp_validate_schema;
	
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_SCHEMA='pbist_apimgmt' AND ROUTINE_NAME='sp_get_current_record_counts' AND ROUTINE_TYPE='PROCEDURE')
    DROP PROCEDURE [pbist_apimgmt].sp_get_current_record_counts;
	
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_SCHEMA='pbist_apimgmt' AND ROUTINE_NAME='sp_check_record_counts_changed' AND ROUTINE_TYPE='PROCEDURE')
    DROP PROCEDURE [pbist_apimgmt].sp_check_record_counts_changed;
	
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_SCHEMA='pbist_apimgmt' AND ROUTINE_NAME='sp_finish_job' AND ROUTINE_TYPE='PROCEDURE')
    DROP PROCEDURE [pbist_apimgmt].sp_finish_job;
	
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_SCHEMA='pbist_apimgmt' AND ROUTINE_NAME='sp_start_job' AND ROUTINE_TYPE='PROCEDURE')
    DROP PROCEDURE [pbist_apimgmt].sp_start_job;
	
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_SCHEMA='pbist_apimgmt' AND ROUTINE_NAME='sp_reset_job' AND ROUTINE_TYPE='PROCEDURE')
    DROP PROCEDURE [pbist_apimgmt].sp_reset_job;

GO
	
-- =============================================
-- Pre Setup - Insert Configuration Values
-- =============================================
DELETE 
FROM pbist_apimgmt.[configuration]
WHERE [configuration_group] = 'SolutionTemplate' AND [configuration_subgroup]='SSAS'
GO

INSERT pbist_apimgmt.[configuration] (configuration_group, configuration_subgroup, [name], [value], [visible])
    VALUES ( N'SolutionTemplate', N'SSAS', N'ProcessOnNextSchedule', N'0', 0),
		   ( N'SolutionTemplate', N'SSAS', N'Timeout', N'4', 0),
		   ( N'SolutionTemplate', N'SSAS', N'ValidateSchema', N'1', 0),
		   ( N'SolutionTemplate', N'SSAS', N'CheckRowCounts', N'1', 0),
		   ( N'SolutionTemplate', N'SSAS', N'CheckProcessFlag', N'0', 0);
GO


-- =============================================
-- SSAS tables
-- =============================================
CREATE  TABLE  pbist_apimgmt.ssas_jobs
( 
	id                  INT IDENTITY(1, 1) NOT NULL,
    startTime           DateTime NOT NULL, 
    endTime             DateTime NULL,
    statusMessage       nvarchar(MAX),
	lastCount			INT,
	CONSTRAINT id_pk PRIMARY KEY (id)
);
go


-- =============================================
-- SSAS stored procs
-- =============================================
-- set process flag
CREATE PROCEDURE pbist_apimgmt.sp_set_process_flag
	@process_flag nvarchar = '1'
AS
BEGIN

	SET NOCOUNT ON;
    UPDATE [pbist_apimgmt].[configuration] 
	SET [value]=@process_flag
	WHERE [configuration_group] = 'SolutionTemplate' AND [configuration_subgroup]='SSAS' AND [name]='ProcessOnNextSchedule';
END
GO


-- SSAS validate schema
CREATE PROCEDURE pbist_apimgmt.sp_validate_schema AS
BEGIN
    SET NOCOUNT ON;
	DECLARE @returnValue INT;
	SELECT @returnValue = Count(*)
	FROM   information_schema.tables
	WHERE  ( table_schema = 'pbist_apimgmt' AND
				 table_name IN (
				 'request'));
    if(@returnValue = 1)
    BEGIN
    RETURN 1;
    END;
    RETURN 0;
END
go

-- Get Record Counts
CREATE PROCEDURE pbist_apimgmt.sp_get_current_record_counts AS
BEGIN
    SET NOCOUNT ON;
	DECLARE @returnValue INT;
       SELECT @returnValue = SUM(tableCount) FROM
        (
			SELECT Count(*) AS tableCount FROM pbist_apimgmt.request
        ) AS temp;
		RETURN @returnValue;
END;
go

-- get record counts
CREATE PROCEDURE pbist_apimgmt.sp_check_record_counts_changed AS
BEGIN
    SET NOCOUNT ON;
	DECLARE @newRowCount INT;
	DECLARE @oldRowCount INT;
	EXECUTE @newRowCount = pbist_apimgmt.sp_get_current_record_counts;

	SELECT TOP 1 @oldRowCount  = [lastCount]
	FROM pbist_apimgmt.[ssas_jobs]
	WHERE [statusMessage] = 'Success'
	ORDER BY startTime DESC;

	IF @newRowCount = @oldRowCount
		RETURN 0;
	ELSE
		RETURN 1;
END;
go

-- finish job
CREATE PROCEDURE [pbist_apimgmt].[sp_finish_job]
	@jobid INT,
	@jobMessage NVARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @newRowCount INT;
	EXECUTE @newRowCount = pbist_apimgmt.sp_get_current_record_counts;

	IF @jobMessage = 'Success' 
		UPDATE [pbist_apimgmt].[ssas_jobs] 
		SET [endTime]=GETDATE(), [statusMessage]=@jobMessage, [lastCount]=@newRowCount
		WHERE [id] = @jobid;
	ELSE
		UPDATE [pbist_apimgmt].[ssas_jobs] 
		SET [endTime]=GETDATE(), [statusMessage]=@jobMessage
		WHERE [id] = @jobid;
END;
GO

-- timeout jobs
CREATE PROCEDURE pbist_apimgmt.sp_reset_job
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE pbist_apimgmt.[ssas_jobs] 
	
	SET [statusMessage]='Timed Out',
	[endTime]=GetDate()
	WHERE endTime is NULL AND
	DATEPART(HOUR, getdate() - startTime) >= 
	(SELECT [value] FROM pbist_apimgmt.[configuration] WHERE [configuration_group] = 'SolutionTemplate' AND [configuration_subgroup]='SSAS' AND [name]='Timeout')
	
	DELETE 
	FROM pbist_apimgmt.[ssas_jobs] 
	WHERE DATEPART(DAY, getdate() - startTime) >= 30
END;
GO

-- start job
CREATE PROCEDURE pbist_apimgmt.sp_start_job
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @validateSchema INT;
	DECLARE @checkRowCounts INT;
	DECLARE @checkProcessFlag INT;
	DECLARE @id INT;
	DECLARE @checksPassed INT;
	DECLARE @errorMessage NVARCHAR(MAX);
	SET @errorMessage = '';
	SET @checksPassed = 1;

	SELECT @validateSchema  = [value]
	FROM pbist_apimgmt.[configuration]
	WHERE [configuration_group] = 'SolutionTemplate' AND [configuration_subgroup]='SSAS' AND [name]='ValidateSchema'; 
	
	SELECT @checkRowCounts  = [value]
	FROM pbist_apimgmt.[configuration]
	WHERE [configuration_group] = 'SolutionTemplate' AND [configuration_subgroup]='SSAS' AND [name]='CheckRowCounts'; 
	
	SELECT @checkProcessFlag  = [value]
	FROM pbist_apimgmt.[configuration]
	WHERE [configuration_group] = 'SolutionTemplate' AND [configuration_subgroup]='SSAS' AND [name]='CheckProcessFlag'; 
	
	EXEC pbist_apimgmt.sp_reset_job
	
	INSERT pbist_apimgmt.[ssas_jobs] (startTime, statusMessage)
    VALUES ( GETDATE(), 'Running');
	
	SELECT @id = SCOPE_IDENTITY();
	
    DECLARE @validateSchemaResult INT = 0;
	if(@validateSchema = 1)
	BEGIN		
		EXECUTE @validateSchemaResult = pbist_apimgmt.sp_validate_schema;
		if(@validateSchemaResult = 0)
		BEGIN
			SET @errorMessage = @errorMessage + 'Validate Schema unsuccessful. ';
			SET @checksPassed = 0;
		END
	END;
	
	if(@validateSchema = 1 AND @validateSchemaResult = 1 AND @checkRowCounts = 1)
	BEGIN
		DECLARE @checkRowCountsResult INT;
		EXECUTE @checkRowCountsResult = pbist_apimgmt.sp_check_record_counts_changed;
		if(@checkRowCountsResult = 0)
		BEGIN
			SET @errorMessage = @errorMessage + 'Record Counts unchanged. ';
			SET @checksPassed = 0;
		END
	END;
	
	if(@checkProcessFlag = 1)
	BEGIN
		DECLARE @checkProcessFlagResult INT;
		SELECT @checkProcessFlagResult = [value]
		FROM pbist_apimgmt.[configuration]
		WHERE [configuration_group] = 'SolutionTemplate' AND [configuration_subgroup]='SSAS' AND [name]='ProcessOnNextSchedule'
		if(@checkProcessFlagResult = 0)
		BEGIN
			SET @errorMessage = @errorMessage + 'Process flag not set to 1. ';
			SET @checksPassed = 0;
		END
	END;
	
	DECLARE @getLastJobs INT;
	SELECT @checkProcessFlagResult = count(*)
		FROM pbist_apimgmt.[ssas_jobs]
		WHERE [endTime] is NULL
		if(@checkProcessFlagResult > 1)
		BEGIN
			SET @errorMessage = @errorMessage + 'Job currently Running. ';
			SET @checksPassed = 0;
		END;	
	

	IF(@checksPassed = 0)
	BEGIN
		EXEC [pbist_apimgmt].[sp_finish_job] @jobid= @id, @jobMessage= @errorMessage
        return 0;
	END

    EXEC [pbist_apimgmt].[sp_set_process_flag] @process_flag = '0'

	RETURN @id;
	END;
GO
