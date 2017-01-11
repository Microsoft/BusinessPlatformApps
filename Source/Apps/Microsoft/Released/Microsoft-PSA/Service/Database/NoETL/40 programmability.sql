SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
go

CREATE PROCEDURE psa.sp_get_replication_counts AS
BEGIN
    SET NOCOUNT ON;

    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; -- it's ok for these to be somewhat inaccurate
     
    WITH TableCounts(EntityName, [Count]) AS
    (
        SELECT LEFT(ta.name, CASE WHEN CHARINDEX('_', ta.name)=0 THEN 100 ELSE CHARINDEX('_', ta.name)-1 END) AS EntityName, SUM(pa.rows) AS [Count]
        FROM sys.tables ta INNER JOIN sys.partitions pa ON pa.OBJECT_ID = ta.OBJECT_ID
                           INNER JOIN sys.schemas sc ON ta.schema_id = sc.schema_id
        WHERE
            sc.name='dbo' AND ta.is_ms_shipped = 0 AND pa.index_id IN (0,1) AND
            ta.name IN ('account', 'bookableresource', 'bookableresourcebooking', 'bookableresourcecategory', 'bookableresourcecategoryassn')
        GROUP BY LEFT(ta.name, CASE WHEN CHARINDEX('_', ta.name)=0 THEN 100 ELSE CHARINDEX('_', ta.name)-1 END)
    )
    SELECT UPPER(LEFT(EntityName, 1)) + LOWER(SUBSTRING(EntityName, 2, 100)) AS EntityName, [Count] FROM TableCounts;
END;
go


CREATE PROCEDURE psa.sp_get_prior_content AS
BEGIN
    SET NOCOUNT ON;

    SELECT Count(*) AS ExistingObjectCount
    FROM   information_schema.tables
    WHERE  table_schema = 'psa' AND
           table_name IN ('account', 'bookableresource', 'bookableresourcebooking', 'bookableresourcecategory', 'bookableresourcecategoryassn');
END;
go

CREATE PROCEDURE psa.sp_get_last_updatetime AS
BEGIN
    SET NOCOUNT ON;

    SELECT [value] AS lastLoadTimestamp FROM pbist_sccm.[configuration] WHERE name='lastLoadTimestamp' AND configuration_group='SolutionTemplate' AND configuration_subgroup='PSA';
END;
go

