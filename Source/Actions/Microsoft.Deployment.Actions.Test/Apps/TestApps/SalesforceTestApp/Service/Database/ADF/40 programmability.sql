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

    SELECT UPPER(LEFT(ta.name, 1)) + LOWER(SUBSTRING(ta.name, 2, 100)) AS EntityName, SUM(pa.rows) AS [Count], '' As [Status]
    FROM sys.tables ta INNER JOIN sys.partitions pa ON pa.OBJECT_ID = ta.OBJECT_ID
				       INNER JOIN sys.schemas sc ON ta.schema_id = sc.schema_id
    WHERE
	    sc.name='dbo' AND ta.is_ms_shipped = 0 AND pa.index_id IN (0,1) AND
	    ta.name IN ('Account', 'Lead', 'Opportunity', 'OpportunityLineItem', 'OpportunityStage', 'UserRole', 'User', 'Product2')
    GROUP BY ta.name
    ORDER BY ta.name
END;
go


CREATE PROCEDURE dbo.sp_get_prior_content
AS
BEGIN
	SET NOCOUNT ON;

    SELECT Count(*) AS ExistingObjectCount
    FROM   information_schema.tables
    WHERE  ( table_schema = 'dbo' AND
             table_name IN ('account', 'lead', 'opportunity', 'opportunitylineitem', 'opportunitystage', 'product2', 'user', 'userrole')
           ) OR
           ( table_schema = 'smgt' AND
             table_name IN ('AccountView', 'ActualSales', 'ActualSalesView', 'BusinessUnitView', 'configuration', 'ConfigurationView', 'date', 'DateView',
                            'LeadView', 'MeasuresView', 'OpportunityProductView', 'OpportunityView', 'ProductView', 'Quotas', 'QuotaView', 'Targets',
                            'TargetView', 'TempUserView', 'TerritoryView', 'UserAscendantsView', 'userMapping', 'UserView'
                           )
           );
END;