SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
go

CREATE PROCEDURE dbo.sp_get_replication_counts
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @tables NVARCHAR(MAX);
    SELECT @tables = REPLACE([value],' ','')
    FROM [csrv].[configuration]
    WHERE configuration_group = 'SolutionTemplate'
    AND	configuration_subgroup = 'StandardConfiguration' 
    AND	name = 'Tables';

    SELECT UPPER(LEFT(ta.name, 1)) + LOWER(SUBSTRING(ta.name, 2, 100)) AS EntityName, SUM(pa.rows) AS [Count]
    FROM sys.tables ta INNER JOIN sys.partitions pa ON pa.object_id = ta.object_id
                       INNER JOIN sys.schemas sc ON ta.schema_id = sc.schema_id
    WHERE
        sc.name='dbo' AND ta.is_ms_shipped = 0 AND pa.index_id IN (0,1) AND
        ta.name COLLATE SQL_Latin1_General_CP1_CI_AS IN (SELECT [value] COLLATE SQL_Latin1_General_CP1_CI_AS FROM STRING_SPLIT(@tables,',') WHERE RTRIM([value])<>'')
    GROUP BY ta.name
    ORDER BY ta.name;
END;
go


CREATE PROCEDURE dbo.sp_get_pull_status
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @tables NVARCHAR(MAX);
    SELECT @tables = REPLACE([value],' ','')
    FROM [csrv].[configuration]
    WHERE configuration_group = 'SolutionTemplate'
    AND	configuration_subgroup = 'StandardConfiguration' 
    AND	name = 'Tables';

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
        sc.name='dbo' AND ta.is_ms_shipped = 0 AND pa.index_id IN (0,1) AND
        ta.name COLLATE SQL_Latin1_General_CP1_CI_AS IN (SELECT [value] COLLATE SQL_Latin1_General_CP1_CI_AS FROM STRING_SPLIT(@tables,',') WHERE RTRIM([value])<>'')
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
        FROM #counts c INNER JOIN csrv.entityinitialcount i ON i.entityname = c.entityname



    DECLARE @DeploymentTimestamp datetime2;
    SELECT @DeploymentTimestamp = Convert(DATETIME2, [value], 126)
    FROM csrv.[configuration] WHERE configuration_group = 'SolutionTemplate' AND configuration_subgroup = 'Notifier' AND [name] = 'DeploymentTimestamp';

    IF EXISTS (SELECT *
               FROM #counts
               WHERE [Count] > 0 AND DATEDIFF(HOUR, @DeploymentTimestamp, Sysdatetime()) > 24)
           SET @StatusCode = 1 --Data pull is partially complete

        
    
    DECLARE @CompletePercentage FLOAT;
    SELECT @CompletePercentage = Convert(float, [value])
    FROM csrv.[configuration] WHERE configuration_group = 'SolutionTemplate' AND configuration_subgroup = 'Notifier' AND [name] = 'DataPullCompleteThreshold';

    DECLARE @CountsRows INT, @CountRowsComplete INT;
    SELECT @CountsRows = COUNT(*) FROM #counts;
    
    SELECT p.[Percentage], p.[EntityName], i.lasttimestamp,  DATEDIFF(MINUTE, i.lasttimestamp, Sysdatetime()) AS [TimeDifference] INTO #entitiesComplete
    FROM #percentages p
              INNER JOIN csrv.entityinitialcount i ON i.entityName = p.EntityName
              WHERE 
              ((p.[Percentage] >= @CompletePercentage) AND DATEDIFF(MINUTE, i.lasttimestamp, Sysdatetime()) > 5) OR
              (p.[Percentage] >= 100) OR
              ((p.[Percentage] >= 100) AND DATEDIFF(MINUTE, i.lasttimestamp, Sysdatetime()) > 5)

    SELECT @CountRowsComplete = COUNT(*) FROM #entitiesComplete;
              
    IF (@CountRowsComplete = @CountsRows)
        SET @StatusCode = 2 --Data pull complete

    DECLARE @EntitiesWithNoData INT;
    SELECT @EntitiesWithNoData = COUNT(*) FROM #counts WHERE [Count] = 0;
    IF @EntitiesWithNoData = @CountsRows AND DATEDIFF(HOUR, @DeploymentTimestamp, Sysdatetime()) > 24
        SET @StatusCode = 3; --No data is present
    
    DECLARE @ASDeployment bit = 0;

    IF EXISTS (SELECT * FROM csrv.[configuration] WHERE configuration_group = 'SolutionTemplate' AND configuration_subgroup = 'Notifier' AND [name] = 'ASDeployment' AND [value] ='true')
    SET @ASDeployment = 1;

    -- AS Flow
    IF @ASDeployment=1 AND DATEDIFF(HOUR, @DeploymentTimestamp, Sysdatetime()) < 24 AND NOT EXISTS (SELECT * FROM csrv.ssas_jobs WHERE [statusMessage] = 'Success')
    SET @StatusCode = -1;

    -- Delayed Processing Flow
    DECLARE @c1 INT, @c2 INT;
    SELECT @c1 = COUNT(*) FROM #counts;
    SELECT @c2 = COUNT(*) from csrv.entityinitialcount;
    IF @c1<>@c2 
    SET @StatusCode = -1;


    UPDATE csrv.[configuration] 
    SET [configuration].[value] = @StatusCode
    WHERE [configuration].configuration_group = 'SolutionTemplate' AND [configuration].configuration_subgroup = 'Notifier' AND [configuration].[name] = 'DataPullStatus'

    MERGE csrv.entityinitialcount AS target
    USING #counts AS source
    ON (target.entityname = source.entityname)
    WHEN MATCHED AND source.[Count] > target.lastcount 
    THEN
        UPDATE SET target.lastcount = source.[Count], target.lasttimestamp = Sysdatetime();

END;
GO



CREATE PROCEDURE dbo.sp_get_prior_content
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Count(*) AS ExistingObjectCount
    FROM   INFORMATION_SCHEMA.TABLES
    WHERE  ( table_schema = 'dbo' AND
             table_name IN ('account', 'appointment', 'contact', 'email', 'fax', 'incident', 'letter', 'msdyn_survey', 'msdyn_surveyresponse', 'phonecall', 'slakpiinstance', 'systemuser', 'task', 'team')
           ) OR
           ( table_schema = 'csrv' AND
             table_name IN ('configuration', 'configurationview', 'date', 'dateview',
                            'activityiew', 'caseview', 'customerview', 'measuresview', 'ownerview', 'surveyresponseview', 'surveyview', 'teamview', 
                            'userview'
                           )
           );
END;
go
