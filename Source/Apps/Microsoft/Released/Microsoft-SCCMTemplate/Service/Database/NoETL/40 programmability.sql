SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
go

CREATE PROCEDURE pbist_sccm.sp_get_replication_counts AS
BEGIN
    SET NOCOUNT ON;

    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; -- it's ok for these to be somewhat inaccurate
     
    WITH TableCounts(EntityName, [Count]) AS
    (
        SELECT LEFT(ta.name, CASE WHEN CHARINDEX('_', ta.name)=0 THEN 100 ELSE CHARINDEX('_', ta.name)-1 END) AS EntityName, SUM(pa.rows) AS [Count]
        FROM sys.tables ta INNER JOIN sys.partitions pa ON pa.object_id = ta.object_id
                           INNER JOIN sys.schemas sc ON ta.schema_id = sc.schema_id
        WHERE
            sc.name='pbist_sccm' AND ta.is_ms_shipped = 0 AND pa.index_id IN (0,1) AND
            ta.name IN ('computer', 'site', 'user', 'usercomputer', 'computerprogram', 'program', 'collection', 'computercollection', 'malware', 'computermalware', 'update', 'computerupdate', 'scanhistory',
                        'computer_staging', 'site_staging', 'user_staging', 'usercomputer_staging', 'computerprogram_staging', 'program_staging', 'collection_staging', 'computercollection_staging', 'malware_staging', 'computermalware_staging', 'update_staging', 'computerupdate_staging', 'scanhistory_staging')
        GROUP BY LEFT(ta.name, CASE WHEN CHARINDEX('_', ta.name)=0 THEN 100 ELSE CHARINDEX('_', ta.name)-1 END)
    )
    SELECT UPPER(LEFT(EntityName, 1)) + LOWER(SUBSTRING(EntityName, 2, 100)) AS EntityName, [Count] FROM TableCounts;
END;
go

CREATE PROCEDURE pbist_sccm.sp_get_pull_status
AS
BEGIN
    SET NOCOUNT ON;
		
	--InitialPullComplete statuses
	-- -1 -> Initial State
	-- 1 -> Data is present but not complete
	-- 2 -> Data pull is complete
	-- 3 -> No data is present

   DECLARE @StatusCode INT = -1;

    SELECT ta.[name] AS EntityName, SUM(pa.[rows]) AS [Count] INTO #counts
    FROM sys.tables ta INNER JOIN sys.partitions pa ON pa.object_id = ta.object_id
                       INNER JOIN sys.schemas sc ON ta.schema_id = sc.schema_id
    WHERE        
		    sc.name='pbist_sccm' AND ta.is_ms_shipped = 0 AND pa.index_id IN (0,1) AND
		    ta.name IN ('computer', 'user', 'usercomputer', 'computerprogram', 'program', 'collection', 'computercollection', 'malware', 'computermalware', 'update', 'computerupdate', 'scanhistory' )
	 GROUP BY ta.[name];

		SELECT CASE
                WHEN c.[Count] = 0 AND i.initialcount = 0 THEN 100 
                ELSE (
                        CASE
                            WHEN 100.0*c.[Count] / nullif(i.initialCount,0) IS NULL THEN 0.0
                            ELSE 100.0 * c.[Count]/i.initialCount 
                        END
                     ) 
            END AS [Percentage], 
            c.EntityName as EntityName INTO #percentages 
	    FROM #counts c INNER JOIN pbist_sccm.entityinitialcount i ON i.entityname = c.entityname COLLATE Latin1_General_100_CI_AS


	DECLARE @DeploymentTimestamp datetime2;
	SELECT @DeploymentTimestamp = Convert(DATETIME2, [value], 126)
	FROM pbist_sccm.[configuration] WHERE configuration_group = 'SolutionTemplate' AND configuration_subgroup = 'Notifier' AND [name] = 'DeploymentTimestamp';

	IF EXISTS (SELECT *
			   FROM #counts
			   WHERE [Count] > 0 AND DATEDIFF(HOUR, @DeploymentTimestamp, SYSUTCDATETIME()) > 24)
	SET @StatusCode = 1 --Data pull is partially complete
	
	
	DECLARE @CompletePercentage FLOAT;
    SELECT @CompletePercentage = Convert(float, [value])
    FROM pbist_sccm.[configuration] WHERE configuration_group = 'SolutionTemplate' AND configuration_subgroup = 'Notifier' AND [name] = 'DataPullCompleteThreshold';
			
	
    DECLARE @CountsRows INT, @CountRowsComplete INT;
	SELECT @CountsRows = COUNT(*) FROM #counts;
	
	SELECT p.[Percentage], p.[EntityName], i.lasttimestamp,  DATEDIFF(MINUTE, i.lasttimestamp, SYSUTCDATETIME()) AS [TimeDifference] INTO #entitiesComplete
    FROM #percentages p
              INNER JOIN pbist_sccm.entityinitialcount i ON i.entityName = p.EntityName COLLATE Latin1_General_100_CI_AS
              WHERE 
			  ((p.[Percentage] >= @CompletePercentage) AND DATEDIFF(MINUTE, i.lasttimestamp, SYSUTCDATETIME()) > 5) OR
			  (p.[Percentage] >= 100) OR
			  ((p.[Percentage] >= 100) AND DATEDIFF(MINUTE, i.lasttimestamp, SYSUTCDATETIME()) > 5)

	SELECT @CountRowsComplete = COUNT(*) FROM #entitiesComplete;
			  
    IF (@CountRowsComplete = @CountsRows)
        SET @StatusCode = 2 --Data pull complete

    DECLARE @EntitiesWithNoData INT;
    SELECT @EntitiesWithNoData = COUNT(*) FROM #counts WHERE [Count] = 0;
    IF @EntitiesWithNoData = @CountsRows AND DATEDIFF(HOUR, @DeploymentTimestamp, SYSUTCDATETIME()) > 24
        SET @StatusCode = 3; --No data is present
	
	DECLARE @ASDeployment bit = 0;
	
	
	IF EXISTS (SELECT * FROM pbist_sccm.[configuration] WHERE configuration_group = 'SolutionTemplate' AND configuration_subgroup = 'Notifier' AND [name] = 'ASDeployment' AND [value] ='true')
		SET @ASDeployment = 1;
		

	-- AS Flow
	IF @ASDeployment=1 AND DATEDIFF(HOUR, @DeploymentTimestamp, SYSUTCDATETIME()) < 24 AND NOT EXISTS (SELECT * FROM pbist_sccm.ssas_jobs WHERE [statusMessage] = 'Success')
	SET @StatusCode = -1;

	-- Delayed Processing Flow
	DECLARE @c1 INT, @c2 INT;
    	SELECT @c1 = COUNT(*) FROM #counts;
    	SELECT @c2 = COUNT(*) from pbist_sccm.entityinitialcount;
    	IF @c1<>@c2 
    	SET @StatusCode = -1;


	UPDATE pbist_sccm.[configuration] 
	SET [configuration].[value] = @StatusCode
	WHERE [configuration].configuration_group = 'SolutionTemplate' AND [configuration].configuration_subgroup = 'Notifier' AND [configuration].[name] = 'DataPullStatus'

	MERGE pbist_sccm.entityinitialcount AS TARGET
	USING #counts AS SOURCE
	ON (TARGET.entityname = SOURCE.entityname COLLATE Latin1_General_100_CI_AS)
	WHEN MATCHED AND SOURCE.[Count] > TARGET.lastcount
	THEN 
	UPDATE SET target.lastcount = source.[Count], target.lasttimestamp = SYSUTCDATETIME();
END;
GO

CREATE PROCEDURE pbist_sccm.sp_get_prior_content AS
BEGIN
    SET NOCOUNT ON;

    SELECT Count(*) AS ExistingObjectCount
    FROM   INFORMATION_SCHEMA.TABLES
    WHERE  TABLE_SCHEMA = 'pbist_sccm' AND
           TABLE_NAME IN ('computer', 'site', 'user', 'usercomputer', 'computerprogram', 'program', 'collection', 'computercollection', 'malware', 'computermalware', 'update', 'computerupdate', 'scanhistory',
                          'computer_staging', 'site_staging', 'user_staging', 'usercomputer_staging', 'computerprogram_staging', 'program_staging', 'collection_staging', 'computercollection_staging', 'malware_staging', 'computermalware_staging', 'update_staging', 'computerupdate_staging', 'scanhistory_staging');
END;
go

CREATE PROCEDURE pbist_sccm.sp_get_last_updatetime AS
BEGIN
    SET NOCOUNT ON;

    SELECT CONVERT(datetime, [value], 126) AS lastLoadTimestamp FROM pbist_sccm.[configuration] WHERE name='lastLoadTimestamp' AND configuration_group='SolutionTemplate' AND configuration_subgroup='System Center';
END;
go


CREATE PROCEDURE pbist_sccm.sp_populatecomputer AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;

    -- Mark rows as deleted not found in the staging table
    UPDATE pbist_sccm.computer
    SET    [deleted date] = Getdate()
    FROM   pbist_sccm.computer
    WHERE  pbist_sccm.computer.[deleted date] IS NULL AND
            pbist_sccm.computer.machineid NOT IN (SELECT DISTINCT machineid FROM pbist_sccm.computer_staging)

    -- Merge the rest.
    MERGE pbist_sccm.computer AS Target
    using (SELECT machineid,
                sitecode,
                name,
                [operating system],
                [client type],
                manufacturer,
                model,
                [platform],
                [physical memory]
            FROM   pbist_sccm.computer_staging) AS Source
    ON ( Target.machineid = Source.machineid )
    WHEN matched AND ( Target.sitecode <> Source.sitecode OR Target.name <> Source.name OR Target.[operating system] <>
    Source.[operating system]
    OR Target.[client type] <> Source.[client type] OR Target.manufacturer <> Source.manufacturer OR Target.model <> Source.model OR
    Target.[platform] <> Source.[platform] OR Target.[physical memory] <> Source.[physical memory] ) THEN
    UPDATE SET Target.sitecode = Source.sitecode,
                Target.name = Source.name,
                Target.[operating system] = Source.[operating system],
                Target.[client type] = Source.[client type],
                Target.manufacturer = Source.manufacturer,
                Target.model = Source.model,
                Target.[platform] = Source.[platform],
                Target.[physical memory] = Source.[physical memory]
    WHEN NOT matched BY target THEN
    INSERT ( machineid,
                sitecode,
                name,
                [operating system],
                [client type],
                manufacturer,
                model,
                [platform],
                [physical memory])
    VALUES ( Source.machineid,
                Source.sitecode,
                Source.name,
                Source.[operating system],
                Source.[client type],
                Source.manufacturer,
                Source.model,
                Source.[platform],
                Source.[physical memory] );

    TRUNCATE TABLE pbist_sccm.computer_staging;

    COMMIT;
END;
go

-- =============================================
-- Description: Merges rows from ComputerMalware_staging into ComputerMalware. This is a simple truncate and insert.
CREATE PROCEDURE pbist_sccm.sp_populatecomputermalware AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @retention int;

    BEGIN TRANSACTION;

    -- Get the configured value for data retention from configuration table
    SELECT @retention = [value]
    FROM [pbist_sccm].[vw_configuration]
    WHERE [name] = 'dataretentiondays';

    -- IF null, use the default day (60 days)
    SET @retention = Coalesce(@retention, 60);

    DECLARE @mindate DATE = DATEADD(d,-@retention,GETDATE());
    
    -- Empty the original table
    TRUNCATE TABLE pbist_sccm.computermalware;

    -- Copy the contents from the staging table
    INSERT INTO pbist_sccm.computermalware
                (threatid,
                 machineid,
                 [detection date],
                 [observer product name],
                 [observer product version],
                 [observer detection],
                 [remediation type],
                 [remediation result],
                 [remediation error code],
                 [remediation pending action],
                 [is active malware])
    SELECT threatid,
           machineid,
           [detection date],
           [observer product name],
           [observer product version],
           [observer detection],
           [remediation type],
           [remediation result],
           [remediation error code],
           [remediation pending action],
           [is active malware]
    FROM   pbist_sccm.computermalware_staging
    WHERE  [detection date] >= @mindate;

    TRUNCATE TABLE pbist_sccm.computermalware_staging;

    COMMIT;
END;
go

-- ============================================= 
-- Description: Merges rows from ComputerProgram_staging into ComputerProgram.
CREATE PROCEDURE pbist_sccm.sp_populatecomputerprogram AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @result_code INT;

    -- Logically, this is a simple insert from staging. But it's a lot of data. Drop + rename is much faster
    DROP TABLE pbist_sccm.computerprogram;
    EXEC @result_code = sp_rename 'pbist_sccm.computerprogram_staging', 'computerprogram', 'OBJECT';
    
    -- The * should obey the column order used when the table was created
    SELECT TOP 0 * INTO pbist_sccm.computerprogram_staging FROM pbist_sccm.computerprogram;
END
go


-- =============================================
-- Description: Merges rows from ComputerMalware_staging into ComputerMalware. This is a simple truncate and insert.
CREATE PROCEDURE pbist_sccm.sp_populatecomputerupdate AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @retention int;

    BEGIN TRANSACTION;

    -- Get the configured value for data retention from configuration table
    SELECT @retention = [value] FROM pbist_sccm.[vw_configuration] WHERE [name]='dataretentiondays' AND [configuration group]='SolutionTemplate' AND [configuration subgroup]='System Center';
    -- IF null, use the default day (60 days)
    SET @retention = Coalesce(@retention, 60);

    DECLARE @mindate DATE = DATEADD(d,-@retention,GETDATE());
    
    -- Empty the original table
    TRUNCATE TABLE pbist_sccm.computerupdate;
    
    -- Copy the contents from the staging table
    INSERT INTO pbist_sccm.computerupdate
                (machineid,
                ci_id,
                laststatuschangetime,
                laststatuschecktime,
                [status])
    SELECT machineid,
            ci_id,
            laststatuschangetime,
            laststatuschecktime,
            [status]
    FROM   pbist_sccm.computerupdate_staging
    WHERE  laststatuschangetime >= @mindate;

    TRUNCATE TABLE pbist_sccm.computerupdate_staging;

    COMMIT;
END;
go


-- =============================================
-- Description: Merges rows from Malware_staging into Malware. This is a simple truncate and insert.
CREATE PROCEDURE pbist_sccm.sp_populatemalware AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;

    -- Empty the original table
    TRUNCATE TABLE pbist_sccm.malware;

    -- Copy the contents from the staging table
    INSERT INTO pbist_sccm.malware
                (threatid,
                [malware name],
                [malware severity],
                [malware category])
    SELECT threatid,
            [malware name],
            [malware severity],
            [malware category]
    FROM   pbist_sccm.malware_staging;

    TRUNCATE TABLE pbist_sccm.malware_staging;

    COMMIT;
END ;
go


-- =============================================
-- Description: Merges rows from Program_staging into Malware. This is a simple truncate and insert.
CREATE PROCEDURE pbist_sccm.sp_populateprogram AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;
    -- Empty the original table
    TRUNCATE TABLE pbist_sccm.program;

INSERT INTO pbist_sccm.program
           ([program name]
           ,publisher
           ,[version])
    SELECT          
            [program name],
            publisher,
            [version]
    FROM   pbist_sccm.program_staging;
    
    TRUNCATE TABLE pbist_sccm.program_staging;

    COMMIT;
END;
go

-- =============================================
-- Description: Append the rows from ScanHistory_staging to ScanHistory. This is a simply appending rows with the recent date.
CREATE PROCEDURE pbist_sccm.sp_populatescanhistory
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @retention INT;

    BEGIN TRANSACTION;
    -- Get the configured value for data retention from configuration table
    SELECT @retention = [value] FROM pbist_sccm.vw_configuration WHERE [name]='dataretentiondays';

    -- IF null, use the default day (60 days)
    SET @retention = COALESCE(@retention, 60);

    DECLARE @mindate DATE = Dateadd(d, -@retention, Getdate());
    DECLARE @mindatekey INT = 10000 * Year(@mindate) + 100 * Month(@mindate) + Day(@mindate);

    MERGE INTO pbist_sccm.scanhistory dest
    USING (SELECT machineid,
                date_key,
                sitecode,
                [client type],
                [enabled],
                [client version],
                [real time protection enabled],
                [on access protection enabled],
                [input/output protection enabled],
                [behavior monitor enabled],
                [antivirus enabled],
                [antispyware enabled],
                [nis enabled],
                [quick scan age (days)],
                [full scan age (days)],
                [signature age (days)],
                [engine version],
                [antivirus signature version],
                [missing important update count],
                [missing critical update count],
                [client active status],
                [health evaluation result],
                [health evaluation],
                [last online],
                [health status message],
                [client state],
                [last ddr],
                [last hw],
                [last sw],
                [last status message],
                [last policy request],
                [last scan time]
            FROM   pbist_sccm.scanhistory_staging) AS src(machineid, date_key, sitecode, [client type], [enabled], [client version],
                                                        [real time protection enabled], [on access protection enabled], [input/output protection enabled], [behavior monitor enabled],
                                                        [antivirus enabled], [antispyware enabled], [nis enabled], [quick scan age (days)], [full scan age (days)], [signature age (days)],
                                                        [engine version], [antivirus signature version], [missing important update count], [missing critical update count], [client active status],
                                                        [health evaluation result], [health evaluation], [last online], [health status message], [client state], [last ddr], [last hw], [last sw],
                                                        [last status message], [last policy request], [last scan time])
    ON dest.machineid = src.machineid AND dest.date_key = src.date_key
    WHEN matched THEN
    UPDATE SET machineid = src.machineid,
                date_key = src.date_key,
                sitecode = src.sitecode,
                [client type] = src.[client type],
                [enabled] = src.[enabled],
                [client version] = src.[client version],
                [real time protection enabled] = src.[real time protection enabled],
                [on access protection enabled] = src.[on access protection enabled],
                [input/output protection enabled] = src.[input/output protection enabled],
                [behavior monitor enabled] = src.[behavior monitor enabled],
                [antivirus enabled] = src.[antivirus enabled],
                [antispyware enabled] = src.[antispyware enabled],
                [nis enabled] = src.[nis enabled],
                [quick scan age (days)] = src.[quick scan age (days)],
                [full scan age (days)] = src.[full scan age (days)],
                [signature age (days)] = src.[signature age (days)],
                [engine version] = src.[engine version],
                [antivirus signature version] = src.[antivirus signature version],
                [missing important update count] = src.[missing important update count],
                [missing critical update count] = src.[missing critical update count],
                [client active status] = src.[client active status],
                [health evaluation result] = src.[health evaluation result],
                [health evaluation] = src.[health evaluation],
                [last online] = src.[last online],
                [health status message] = src.[health status message],
                [client state] = src.[client state],
                [last ddr] = src.[last ddr],
                [last hw] = src.[last hw],
                [last sw] = src.[last sw],
                [last status message] = src.[last status message],
                [last policy request] = src.[last policy request],
                [last scan time] = src.[last scan time]
    WHEN NOT matched THEN
    INSERT (machineid,
            date_key,
            sitecode,
            [client type],
            [enabled],
            [client version],
            [real time protection enabled],
            [on access protection enabled],
            [input/output protection enabled],
            [behavior monitor enabled],
            [antivirus enabled],
            [antispyware enabled],
            [nis enabled],
            [quick scan age (days)],
            [full scan age (days)],
            [signature age (days)],
            [engine version],
            [antivirus signature version],
            [missing important update count],
            [missing critical update count],
            [client active status],
            [health evaluation result],
            [health evaluation],
            [last online],
            [health status message],
            [client state],
            [last ddr],
            [last hw],
            [last sw],
            [last status message],
            [last policy request],
            [last scan time])
    VALUES (src.machineid,
            src.date_key,
            src.sitecode,
            src.[client type],
            src.[enabled],
            src.[client version],
            src.[real time protection enabled],
            src.[on access protection enabled],
            src.[input/output protection enabled],
            src.[behavior monitor enabled],
            src.[antivirus enabled],
            src.[antispyware enabled],
            src.[nis enabled],
            src.[quick scan age (days)],
            src.[full scan age (days)],
            src.[signature age (days)],
            src.[engine version],
            src.[antivirus signature version],
            src.[missing important update count],
            src.[missing critical update count],
            src.[client active status],
            src.[health evaluation result],
            src.[health evaluation],
            src.[last online],
            src.[health status message],
            src.[client state],
            src.[last ddr],
            src.[last hw],
            src.[last sw],
            src.[last status message],
            src.[last policy request],
            src.[last scan time]);

    TRUNCATE TABLE pbist_sccm.scanhistory_staging;

    -- Purge records if the data is older than x days
    DELETE FROM pbist_sccm.scanhistory WHERE date_key < @mindatekey;

    COMMIT;
END;
go


-- =============================================
-- Description: Append the rows from  Site_staging to Site.
-- =============================================
CREATE PROCEDURE pbist_sccm.sp_populatesite AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;

    TRUNCATE TABLE pbist_sccm.[site];
    INSERT INTO [pbist_sccm].[site]
           ([sitecode]
           ,[site name]
           ,[version]
           ,[server name]
           ,[availability]
           ,[location]
           ,[overalllinkstatus]
           ,[client successfully communicating with mp]
           ,[client failing to communicate with mp]
           ,[health check count]
           ,[ok health check count]
           ,[av definition compliance count]
           ,[scep count]
           ,[av reach count])
    SELECT sitecode,
            [site name],
            [version],
            [server name],
            [availability],
            [location],
            overalllinkstatus,
            [client successfully communicating with mp],
            [client failing to communicate with mp],
            [health check count],
            [ok health check count],
            [av definition compliance count],
            [scep count],
            [av reach count]
    FROM   pbist_sccm.site_staging;

    TRUNCATE TABLE pbist_sccm.site_staging;

    COMMIT;
END;
go

-- =============================================
-- Description: Append the rows from  Site_staging to Site.
-- =============================================
CREATE PROCEDURE pbist_sccm.sp_populateupdate AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;

    TRUNCATE TABLE pbist_sccm.[update];
    INSERT INTO [pbist_sccm].[update]
           (ci_id
           ,articleid
           ,bulletinid
           ,title
           ,severity
           ,severityname
           ,infoURL)
    SELECT ci_id,
            articleid,
            bulletinid,
            title,
            severity,
            [severity name],
            infourl
    FROM   pbist_sccm.update_staging;

    TRUNCATE TABLE pbist_sccm.update_staging;

    COMMIT;
END;
go

-- =============================================
-- Description: Move the rows from  User_staging to User.
-- =============================================
CREATE PROCEDURE pbist_sccm.sp_populateuser AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;
    TRUNCATE TABLE pbist_sccm.[user];

    INSERT INTO pbist_sccm.[user](username,[full name])
    SELECT username, [full name]
    FROM pbist_sccm.user_staging;

    TRUNCATE TABLE pbist_sccm.user_staging;
    
    COMMIT;
END;
go

-- =============================================
-- Description: Move the rows from  UserComputer_staging to UserComputer.
-- =============================================
CREATE PROCEDURE pbist_sccm.sp_populateusercomputer AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;

    TRUNCATE TABLE pbist_sccm.usercomputer;

    INSERT INTO pbist_sccm.usercomputer(machineid, username,[full name])
    SELECT machineid, username, [full name]
    FROM pbist_sccm.usercomputer_staging;

    TRUNCATE TABLE pbist_sccm.usercomputer_staging;
    COMMIT;
END;
go

-- =============================================
-- Description: Move the rows from  Collection_staging to Collection.
-- =============================================
CREATE PROCEDURE pbist_sccm.sp_populatecollection AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;
    TRUNCATE TABLE pbist_sccm.[collection];

    INSERT INTO pbist_sccm.[collection](collectionid, [collection name])
    SELECT collectionid, [collection name]
    FROM pbist_sccm.collection_staging;

    TRUNCATE TABLE pbist_sccm.collection_staging;
    COMMIT;

END;
go

-- =============================================
-- Description: Move the rows from  ComputerCollection_staging to ComputerCollection.
-- =============================================
CREATE PROCEDURE pbist_sccm.sp_populatecomputercollection AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @result_code INT;

    -- Logically, this is a simple insert from staging. But it's a lot of data. Drop + rename is much faster
    DROP TABLE pbist_sccm.computercollection;
    EXEC @result_code = sp_rename 'pbist_sccm.computercollection_staging', 'computercollection', 'OBJECT';
    
    -- The * should obey the column order used when the table was created
    SELECT TOP 0 * INTO pbist_sccm.computercollection_staging FROM pbist_sccm.computercollection;
END;
go
