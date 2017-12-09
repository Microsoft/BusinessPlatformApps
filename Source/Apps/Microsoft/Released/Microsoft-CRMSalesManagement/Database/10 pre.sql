SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
go

-- Must be executed inside the target database
DECLARE @stmt AS VARCHAR(500), @p1 AS VARCHAR(100), @p2 AS VARCHAR(100);
DECLARE @cr CURSOR;

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='smgt' AND TABLE_NAME='configuration' AND TABLE_TYPE='BASE TABLE')
BEGIN
    DECLARE @additionalTables NVARCHAR(MAX);
    SELECT @additionalTables = [value]
    FROM smgt.[configuration] WHERE configuration_group = 'SolutionTemplate' AND configuration_subgroup = 'SalesManagement' AND [name] = 'AdditionalTables';
SET @cr = CURSOR FAST_FORWARD FOR
              SELECT [value] FROM STRING_SPLIT(@additionalTables,',')

IF(@additionalTables <> '')
BEGIN
OPEN @cr;
FETCH NEXT FROM @cr INTO @p1;
WHILE @@FETCH_STATUS = 0  
BEGIN 
    SET @stmt = 'IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA=''dbo'' AND TABLE_NAME='''+REPLACE(REPLACE(QuoteName(@p1),'[',''),']','')+''' AND TABLE_TYPE=''BASE TABLE'') DROP TABLE dbo.' + QuoteName(@p1);
    EXEC (@stmt);
    SET @stmt = 'IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_SCHEMA=''dbo'' AND ROUTINE_NAME=''spMerge'+REPLACE(REPLACE(QuoteName(@p1),'[',''),']','')+''' AND ROUTINE_TYPE=''PROCEDURE'')   DROP PROCEDURE dbo.spMerge'+ REPLACE(REPLACE(QuoteName(@p1),'[',''),']','');
    EXEC (@stmt);
    SET @stmt = 'IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.DOMAINS WHERE DOMAIN_SCHEMA=''dbo'' AND DOMAIN_NAME='''+REPLACE(REPLACE(QuoteName(@p1),'[',''),']','')+'Idtype'' ) DROP TYPE dbo.'+ REPLACE(REPLACE(QuoteName(@p1),'[',''),']','')+'Idtype';
    EXEC (@stmt);
    SET @stmt = 'IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.DOMAINS WHERE DOMAIN_SCHEMA=''dbo'' AND DOMAIN_NAME='''+REPLACE(REPLACE(QuoteName(@p1),'[',''),']','')+'type'' ) DROP TYPE dbo.'+ REPLACE(REPLACE(QuoteName(@p1),'[',''),']','')+'type';
    EXEC (@stmt);
    FETCH NEXT FROM @cr INTO @p1;
END;
CLOSE @cr;
DEALLOCATE @cr;
END;
END;

-- Regular views
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='smgt' AND TABLE_NAME='AccountView' AND TABLE_TYPE='VIEW')
    DROP VIEW smgt.AccountView;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='smgt' AND TABLE_NAME='BusinessUnitView' AND TABLE_TYPE='VIEW')
    DROP VIEW smgt.BusinessUnitView;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='smgt' AND TABLE_NAME='DateView' AND TABLE_TYPE='VIEW')
    DROP VIEW smgt.DateView;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='smgt' AND TABLE_NAME='LeadView' AND TABLE_TYPE='VIEW')
    DROP VIEW smgt.LeadView;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='smgt' AND TABLE_NAME='TeamView' AND TABLE_TYPE='VIEW')
    DROP VIEW smgt.TeamView;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='smgt' AND TABLE_NAME='MeasuresView' AND TABLE_TYPE='VIEW')
    DROP VIEW smgt.MeasuresView;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='smgt' AND TABLE_NAME='OpportunityProductView' AND TABLE_TYPE='VIEW')
    DROP VIEW smgt.OpportunityProductView;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='smgt' AND TABLE_NAME='OpportunityView' AND TABLE_TYPE='VIEW')
    DROP VIEW smgt.OpportunityView;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='smgt' AND TABLE_NAME='ProductView' AND TABLE_TYPE='VIEW')
    DROP VIEW smgt.ProductView;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='smgt' AND TABLE_NAME='TempUserView' AND TABLE_TYPE='VIEW')
    DROP VIEW smgt.TempUserView;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='smgt' AND TABLE_NAME='TerritoryView' AND TABLE_TYPE='VIEW')
    DROP VIEW smgt.TerritoryView;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='smgt' AND TABLE_NAME='UserView' AND TABLE_TYPE='VIEW')
    DROP VIEW smgt.UserView;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='smgt' AND TABLE_NAME='UserAscendantsView' AND TABLE_TYPE='VIEW')
    DROP VIEW smgt.UserAscendantsView;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='smgt' AND TABLE_NAME='ConfigurationView' AND TABLE_TYPE='VIEW')
    DROP VIEW smgt.ConfigurationView;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='smgt' AND TABLE_NAME='ownerview' AND TABLE_TYPE='VIEW')
    DROP VIEW smgt.ownerview;

-- Tables
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='smgt' AND TABLE_NAME='configuration' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE smgt.[configuration];
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='smgt' AND TABLE_NAME='date' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE smgt.[date];
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='smgt' AND TABLE_NAME='userMapping' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE smgt.userMapping;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='smgt' AND TABLE_NAME='entityinitialcount' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE smgt.entityinitialcount;

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='account' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.account;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='businessunit' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.businessunit;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='lead' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.lead;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='Team' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.Team;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='opportunity' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.opportunity;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='opportunityproduct' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.opportunityproduct;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='product' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.product;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='systemuser' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.systemuser;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='systemusermanagermap' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.systemusermanagermap;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='territory' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.territory;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='AttributeMetadata' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.AttributeMetadata;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='GlobalOptionSetMetadata' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.GlobalOptionSetMetadata;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='OptionSetMetadata' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.OptionSetMetadata;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='StateMetadata' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.StateMetadata;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='StatusMetadata' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.StatusMetadata;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='TargetMetadata' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.TargetMetadata;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='DeleteLog' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.DeleteLog;


IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_SCHEMA='dbo' AND ROUTINE_NAME='sp_get_replication_counts' AND ROUTINE_TYPE='PROCEDURE')
    DROP PROCEDURE dbo.sp_get_replication_counts;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_SCHEMA='dbo' AND ROUTINE_NAME='sp_get_prior_content' AND ROUTINE_TYPE='PROCEDURE')
    DROP PROCEDURE dbo.sp_get_prior_content;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_SCHEMA='dbo' AND ROUTINE_NAME='UpsertAttributeMetadata' AND ROUTINE_TYPE='PROCEDURE')
    DROP PROCEDURE dbo.UpsertAttributeMetadata;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_SCHEMA='dbo' AND ROUTINE_NAME='UpsertGlobalOptionSetMetadata' AND ROUTINE_TYPE='PROCEDURE')
    DROP PROCEDURE dbo.UpsertGlobalOptionSetMetadata;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_SCHEMA='dbo' AND ROUTINE_NAME='UpsertOptionSetMetadata' AND ROUTINE_TYPE='PROCEDURE')
    DROP PROCEDURE dbo.UpsertOptionSetMetadata;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_SCHEMA='dbo' AND ROUTINE_NAME='UpsertStateMetadata' AND ROUTINE_TYPE='PROCEDURE')
    DROP PROCEDURE dbo.UpsertStateMetadata;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_SCHEMA='dbo' AND ROUTINE_NAME='UpsertStatusMetadata' AND ROUTINE_TYPE='PROCEDURE')
    DROP PROCEDURE dbo.UpsertStatusMetadata;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_SCHEMA='dbo' AND ROUTINE_NAME='UpsertTargetMetadata' AND ROUTINE_TYPE='PROCEDURE')
    DROP PROCEDURE dbo.UpsertTargetMetadata;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_SCHEMA='dbo' AND ROUTINE_NAME='sp_get_pull_status' AND ROUTINE_TYPE='PROCEDURE')
    DROP PROCEDURE dbo.sp_get_pull_status;


IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='StateMetadataList')
    DROP TYPE dbo.StateMetadataList;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='StatusMetadataList')
    DROP TYPE dbo.StatusMetadataList;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='OptionSetMetadataList')
    DROP TYPE dbo.OptionSetMetadataList;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='TargetMetadataList')
    DROP TYPE dbo.TargetMetadataList;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='AttributeMetadataList')
    DROP TYPE dbo.AttributeMetadataList;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='systemusermanagermapType')
    DROP TYPE dbo.systemusermanagermapType;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='systemusermanagermapIdType')
    DROP TYPE dbo.systemusermanagermapIdType;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='territoryType')
    DROP TYPE dbo.territoryType;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='territoryIdType')
    DROP TYPE dbo.territoryIdType;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='productType')
    DROP TYPE dbo.productType;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='productIdType')
    DROP TYPE dbo.productIdType;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='opportunityproductType')
    DROP TYPE dbo.opportunityproductType;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='opportunityproductIdType')
    DROP TYPE dbo.opportunityproductIdType;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='systemuserType')
    DROP TYPE dbo.systemuserType;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='systemuserIdType')
    DROP TYPE dbo.systemuserIdType;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='opportunityType')
    DROP TYPE dbo.opportunityType;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='opportunityIdType')
    DROP TYPE dbo.opportunityIdType;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='leadType')
    DROP TYPE dbo.leadType;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='leadIdType')
    DROP TYPE dbo.leadIdType;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='accountType')
    DROP TYPE dbo.accountType;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='accountIdType')
    DROP TYPE dbo.accountIdType;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='teamType')
    DROP TYPE dbo.teamType;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='teamIdType')
    DROP TYPE dbo.teamIdType;
    
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name='smgt')
BEGIN
    EXEC ('CREATE SCHEMA smgt AUTHORIZATION dbo'); -- Avoid batch error
END;


