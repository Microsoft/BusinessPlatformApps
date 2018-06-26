SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
GO

CREATE PROCEDURE bpst_aal.sp_get_replication_counts
AS
BEGIN
    SET NOCOUNT ON;

    SELECT UPPER(LEFT(ta.name, 1)) + LOWER(SUBSTRING(ta.name, 2, 100)) AS EntityName, SUM(pa.rows) AS [Count]
    FROM sys.tables ta INNER JOIN sys.partitions pa ON pa.object_id = ta.object_id
                       INNER JOIN sys.schemas sc ON ta.schema_id = sc.schema_id
    WHERE
        sc.name='bpst_aal' AND ta.is_ms_shipped = 0 AND pa.index_id IN (0,1) AND
        ta.name IN ('administrativedata', 'servicehealthdata')
    GROUP BY ta.name
    ORDER BY ta.name;
END;
go

