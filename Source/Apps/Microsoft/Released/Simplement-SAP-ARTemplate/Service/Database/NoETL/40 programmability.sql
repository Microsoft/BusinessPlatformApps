SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
go

CREATE PROCEDURE sap.sp_get_replication_counts AS
BEGIN
    SET NOCOUNT ON;

	IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='sap' AND TABLE_NAME='sap_replicationstatus' AND TABLE_TYPE='BASE TABLE')
	   SELECT TOP 0 '' AS EntityName, 0 AS [Count], '' AS [Status];
	ELSE
		WITH TableCounts AS
		(
            SELECT UPPER(LEFT(ta.name, 1)) + LOWER(SUBSTRING(ta.name, 2, 100)) AS EntityName, SUM(pa.rows) AS [Count]
            FROM sys.tables ta INNER JOIN sys.partitions pa ON pa.[OBJECT_ID] = ta.[OBJECT_ID]
                                INNER JOIN sys.schemas sc ON ta.[schema_id] = sc.[schema_id]
            WHERE
                sc.name='sap' AND ta.is_ms_shipped = 0 AND pa.index_id IN (0,1) AND
                ta.name IN ('AccountGroup', 'ARClearedLineItem', 'ARLineItem', 'Client', 'Company', 'CurrencyCode', 'CurrencyCodeName', 'Customer', 'CustomerAccountGroupName',
                            'DocumentType', 'DocumentTypeName', 'ExchangeRate', 'GLAccount', 'GLAccountName', 'PaymentTerm', 'PaymentTermName', 'ProfitCenter', 'ProfitCenterName', 'SpecialGLIndicator',
                            'SpecialGLIndicatorName')
            GROUP BY ta.name		
        ),
		LastStats(EntityName, [TimeStamp]) AS
		(
			SELECT EntityName, Max([TimeStamp]) AS [TimeStamp]
		    FROM sap.sap_replicationstatus
			GROUP BY EntityName
		)
		SELECT TableCounts.*,
				CASE
					WHEN s.EntityName IS NULL THEN 'Not started'
					WHEN s.EndDate IS NULL AND s.EntityName IS NOT NULL THEN 'In progress'
					ELSE 'Finished'
				END As [Status]
		FROM TableCounts LEFT OUTER JOIN sap.sap_replicationstatus s ON TableCounts.EntityName COLLATE Latin1_General_100_CI_AS = s.EntityName COLLATE Latin1_General_100_CI_AS
		                 INNER JOIN LastStats ON s.EntityName=LastStats.EntityName AND s.[TimeStamp]=LastStats.[TimeStamp]
		ORDER BY TableCounts.EntityName;


END;
go

