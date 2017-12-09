
CREATE PROCEDURE pbist_twitter.sp_finish_process
    @status_flag nvarchar(MAX) = 'Success'
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    UPDATE [pbist_twitter].[configuration] 
    SET [value]=GETDATE()
    WHERE [configuration_group] = 'SolutionTemplate' AND [configuration_subgroup]='SSAS' AND [name]='LastProcessedDateTime';
    
     UPDATE [pbist_twitter].[configuration] 
    SET [value]=@status_flag
    WHERE [configuration_group] = 'SolutionTemplate' AND [configuration_subgroup]='SSAS' AND [name]='LastProcessedStatus';
    
    UPDATE [pbist_twitter].[configuration] 
    SET [value]='Not Running'
    WHERE [configuration_group] = 'SolutionTemplate' AND [configuration_subgroup]='SSAS' AND [name]='CurrentStatus';
    
    UPDATE [pbist_twitter].[configuration] 
    SET [value]='1'
    WHERE [configuration_group] = 'SolutionTemplate' AND [configuration_subgroup]='SSAS' AND [name]='ProcessOnNextSchedule';
END
GO

CREATE PROCEDURE [pbist_twitter].[sp_get_pull_status]
AS
BEGIN
		
	--InitialPullComplete statuses
	-- -1 -> Initial State
	-- 1 -> Data is present but not complete (Not applicable for twitter - declare success when we see one tweet)
	-- 2 -> Data pull is complete
	-- 3 -> No data is present

	DECLARE @StatusCode int;
	SET @StatusCode = -1;

	SET NOCOUNT ON;

	DECLARE @DeploymentTimestamp datetime2;
	SET @DeploymentTimestamp = CAST((SELECT [value] from [pbist_twitter].[configuration] config
								WHERE config.configuration_group = 'SolutionTemplate' AND config.configuration_subgroup = 'Notifier' AND config.[name] = 'DeploymentTimestamp') AS datetime2)


	DECLARE @NumberOfTweets int;
	SET @NumberOfTweets = (SELECT COUNT(*) AS [Count]
				   FROM [pbist_twitter].[tweets_processed])
			
	IF (@NumberOfTweets > 0 )
		SET @StatusCode = 2 --Data pull is complete
	
	
	IF (@NumberOfTweets = 0  AND DATEDIFF(HOUR, @DeploymentTimestamp, CURRENT_TIMESTAMP) > 24)
	SET @StatusCode = 3 --No data is present
	
	DECLARE @ASDeployment bit;	 
	SET @ASDeployment = 0;

	IF EXISTS (SELECT * FROM [pbist_twitter].[configuration] 
			   WHERE [configuration].configuration_group = 'SolutionTemplate' AND 
					 [configuration].configuration_subgroup = 'Notifier' AND 
					 [configuration].[name] = 'ASDeployment' AND
					 [configuration].[value] ='true')
	SET @ASDeployment = 1;

	-- AS Flow
	IF NOT EXISTS (SELECT * FROM [pbist_twitter].ssas_jobs WHERE [statusMessage] = 'Success') AND @ASDeployment = 1 AND DATEDIFF(HOUR, @DeploymentTimestamp, CURRENT_TIMESTAMP) < 24
	SET @StatusCode = -1;

	UPDATE [pbist_twitter].[configuration] 
	SET [configuration].[value] = @StatusCode
	WHERE [configuration].configuration_group = 'SolutionTemplate' AND [configuration].configuration_subgroup = 'Notifier' AND [configuration].[name] = 'DataPullStatus'

END;
GO

CREATE PROCEDURE pbist_twitter.sp_get_prior_content AS
BEGIN
    SET NOCOUNT ON;

    SELECT Count(*) AS ExistingObjectCount
    FROM   INFORMATION_SCHEMA.TABLES
    WHERE  table_schema = 'pbist_twitter' AND
           table_name IN ('configuration', 'date', 'tweets_processed', 'tweets_normalized', 'hashtag_slicer', 'mention_slicer', 'entity_graph',
                          'authorhashtag_graph', 'authormention_graph', 'entities', 'entities2');
END;
GO

CREATE PROCEDURE pbist_twitter.sp_get_process_flag
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    SELECT [value]
    FROM pbist_twitter.[configuration] 
    WHERE configuration_group = 'SolutionTemplate' AND configuration_subgroup='SSAS' AND [name]='ProcessOnNextSchedule';
END
GO

CREATE PROCEDURE pbist_twitter.sp_get_process_status_flag
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    SELECT [value]
    FROM pbist_twitter.[configuration] 
    WHERE configuration_group = 'SolutionTemplate' AND configuration_subgroup='SSAS' AND [name]='CurrentStatus';
END
GO

CREATE PROCEDURE pbist_twitter.sp_get_replication_counts AS
BEGIN
    SET NOCOUNT ON;

    SELECT UPPER(LEFT(ta.name, 1)) + LOWER(SUBSTRING(ta.name, 2, 100)) AS EntityName, SUM(pa.[rows]) AS [Count]
    FROM sys.tables ta INNER JOIN sys.partitions pa ON pa.[OBJECT_ID] = ta.[OBJECT_ID]
                        INNER JOIN sys.schemas sc ON ta.[schema_id] = sc.[schema_id]
    WHERE
        sc.name='pbist_twitter' AND ta.is_ms_shipped = 0 AND pa.index_id IN (0,1) AND
        ta.name IN ('tweets_processed', 'tweets_normalized', 'hashtag_slicer', 'mention_slicer','authorhashtag_graph', 'authormention_graph')
    GROUP BY ta.name
END;
GO

CREATE PROCEDURE pbist_twitter.sp_set_process_flag
    @status_flag NCHAR(1) = '1'
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    UPDATE pbist_twitter.[configuration] 
    SET [value]=@status_flag
    WHERE configuration_group='SolutionTemplate' AND configuration_subgroup='SSAS' AND [name]='ProcessOnNextSchedule';
END;
GO


CREATE PROCEDURE pbist_twitter.sp_set_process_status_flag
    @status_flag NVARCHAR(20) = 'Running'
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    UPDATE pbist_twitter.[configuration] 
    SET [value]=@status_flag
    WHERE [configuration_group] = 'SolutionTemplate' AND [configuration_subgroup]='SSAS' AND [name]='CurrentStatus';
END;
GO

CREATE PROCEDURE pbist_twitter.sp_start_process
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    UPDATE pbist_twitter.[configuration] 
    SET [value]='Running'
    WHERE configuration_group='SolutionTemplate' AND configuration_subgroup='SSAS' AND [name]='CurrentStatus';
END;
GO
