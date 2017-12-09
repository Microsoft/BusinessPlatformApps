SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
go

-- Must be executed inside the target database

-- Regular views
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='fso' AND TABLE_NAME='bookingstatusview' AND TABLE_TYPE='VIEW')
    DROP VIEW fso.bookingstatusview;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='fso' AND TABLE_NAME='customerequipmentview' AND TABLE_TYPE='VIEW')
    DROP VIEW fso.customerequipmentview;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='fso' AND TABLE_NAME='configurationview' AND TABLE_TYPE='VIEW')
    DROP VIEW fso.configurationview;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='fso' AND TABLE_NAME='customerview' AND TABLE_TYPE='VIEW')
    DROP VIEW fso.customerview;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='fso' AND TABLE_NAME='dateview' AND TABLE_TYPE='VIEW')
    DROP VIEW fso.dateview;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='fso' AND TABLE_NAME='measuresview' AND TABLE_TYPE='VIEW')
    DROP VIEW fso.measuresview;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='fso' AND TABLE_NAME='incidenttypeview' AND TABLE_TYPE='VIEW')
    DROP VIEW fso.incidenttypeview;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='fso' AND TABLE_NAME='productview' AND TABLE_TYPE='VIEW')
    DROP VIEW fso.productview;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='fso' AND TABLE_NAME='resourcebookingview' AND TABLE_TYPE='VIEW')
    DROP VIEW fso.resourcebookingview;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='fso' AND TABLE_NAME='resourceview' AND TABLE_TYPE='VIEW')
    DROP VIEW fso.resourceview;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='fso' AND TABLE_NAME='serviceview' AND TABLE_TYPE='VIEW')
    DROP VIEW fso.serviceview;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='fso' AND TABLE_NAME='territoryview' AND TABLE_TYPE='VIEW')
    DROP VIEW fso.territoryview;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='fso' AND TABLE_NAME='workorderproductsview' AND TABLE_TYPE='VIEW')
    DROP VIEW fso.workorderproductsview;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='fso' AND TABLE_NAME='workorderserviceview' AND TABLE_TYPE='VIEW')
    DROP VIEW fso.workorderserviceview;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='fso' AND TABLE_NAME='workOrdertypeview' AND TABLE_TYPE='VIEW')
    DROP VIEW fso.workOrdertypeview;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='fso' AND TABLE_NAME='workorderview' AND TABLE_TYPE='VIEW')
    DROP VIEW fso.workorderview;



-- Tables
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='fso' AND TABLE_NAME='configuration' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE fso.[configuration];
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='fso' AND TABLE_NAME='date' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE fso.[date];
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='DeleteLog' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.DeleteLog;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='fso' AND TABLE_NAME='entityinitialcount' AND TABLE_TYPE='BASE TABLE')
	DROP TABLE fso.entityinitialcount;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='account' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.account;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='AttributeMetadata' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.AttributeMetadata;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='bookableresource' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.bookableresource;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='bookableresourcebooking' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.bookableresourcebooking;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='bookingstatus' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.bookingstatus;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='GlobalOptionSetMetadata' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.GlobalOptionSetMetadata;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_customerasset' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.msdyn_customerasset;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_incidenttype' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.msdyn_incidenttype;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_workorder' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.msdyn_workorder;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_workorderservice' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.msdyn_workorderservice;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_workorder_status' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.msdyn_workorder_status;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_workorderproduct' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.msdyn_workorderproduct;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_workordertype' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.msdyn_workordertype;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='optionsetmetadata' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.optionsetmetadata;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='product' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.product;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='resource' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.resource;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='service' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.[service];
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='serviceappointment' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.serviceappointment;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='StateMetadata' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.StateMetadata;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='StatusMetadata' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.StatusMetadata;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='TargetMetadata' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.TargetMetadata;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='task' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.task;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='territory' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.territory;



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


IF EXISTS (SELECT * FROM sys.Types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='StateMetadataList')
    DROP TYPE dbo.StateMetadataList;
IF EXISTS (SELECT * FROM sys.Types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='StatusMetadataList')
    DROP TYPE dbo.StatusMetadataList;
IF EXISTS (SELECT * FROM sys.Types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='OptionSetMetadataList')
    DROP TYPE dbo.OptionSetMetadataList;
IF EXISTS (SELECT * FROM sys.Types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='TargetMetadataList')
    DROP TYPE dbo.TargetMetadataList;
IF EXISTS (SELECT * FROM sys.Types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='AttributeMetadataList')
    DROP TYPE dbo.AttributeMetadataList;
IF EXISTS (SELECT * FROM sys.Types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='AccountType')
    DROP TYPE dbo.AccountType;
IF EXISTS (SELECT * FROM sys.Types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='AccountIdType')
    DROP TYPE dbo.AccountIdType;
IF EXISTS (SELECT * FROM sys.Types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='bookableresourcebookingType')
    DROP TYPE dbo.bookableresourcebookingType;
IF EXISTS (SELECT * FROM sys.Types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='bookableresourcebookingIdType')
    DROP TYPE dbo.bookableresourcebookingIdType;
IF EXISTS (SELECT * FROM sys.Types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='bookableresourceType')
    DROP TYPE dbo.bookableresourceType;
IF EXISTS (SELECT * FROM sys.Types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='bookableresourceIdType')
    DROP TYPE dbo.bookableresourceIdType;
IF EXISTS (SELECT * FROM sys.Types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='bookableresourceType')
    DROP TYPE dbo.bookableresourceType;
IF EXISTS (SELECT * FROM sys.Types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='bookableresourceIdType')
    DROP TYPE dbo.bookableresourceIdType;
IF EXISTS (SELECT * FROM sys.Types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='bookingstatusType')
    DROP TYPE dbo.bookingstatusType;
IF EXISTS (SELECT * FROM sys.Types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='bookingstatusIdType')
    DROP TYPE dbo.bookingstatusIdType;
IF EXISTS (SELECT * FROM sys.Types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='msdyn_customerassetType')
    DROP TYPE dbo.msdyn_customerassetType;
IF EXISTS (SELECT * FROM sys.Types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='msdyn_customerassetIdType')
    DROP TYPE dbo.msdyn_customerassetIdType;
IF EXISTS (SELECT * FROM sys.Types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='msdyn_customerassetType')
    DROP TYPE dbo.msdyn_customerassetType;
IF EXISTS (SELECT * FROM sys.Types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='msdyn_customerassetIdType')
    DROP TYPE dbo.msdyn_customerassetIdType;
IF EXISTS (SELECT * FROM sys.Types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='msdyn_incidenttypeType')
    DROP TYPE dbo.msdyn_incidenttypeType;
IF EXISTS (SELECT * FROM sys.Types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='msdyn_incidenttypeIdType')
    DROP TYPE dbo.msdyn_incidenttypeIdType;
IF EXISTS (SELECT * FROM sys.Types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='msdyn_workorderType')
    DROP TYPE dbo.msdyn_workorderType;
IF EXISTS (SELECT * FROM sys.Types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='msdyn_workorderIdType')
    DROP TYPE dbo.msdyn_workorderIdType;
IF EXISTS (SELECT * FROM sys.Types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='msdyn_workorderproductType')
    DROP TYPE dbo.msdyn_workorderproductType;
IF EXISTS (SELECT * FROM sys.Types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='msdyn_workorderproductIdType')
    DROP TYPE dbo.msdyn_workorderproductIdType;
IF EXISTS (SELECT * FROM sys.Types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='msdyn_workorderserviceType')
    DROP TYPE dbo.msdyn_workorderserviceType;
IF EXISTS (SELECT * FROM sys.Types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='msdyn_workorderserviceIdType')
    DROP TYPE dbo.msdyn_workorderserviceIdType;
IF EXISTS (SELECT * FROM sys.Types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='msdyn_workordertypeType')
    DROP TYPE dbo.msdyn_workordertypeType;
IF EXISTS (SELECT * FROM sys.Types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='msdyn_workordertypeIdType')
    DROP TYPE dbo.msdyn_workordertypeIdType;
IF EXISTS (SELECT * FROM sys.Types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='productType')
    DROP TYPE dbo.productType;
IF EXISTS (SELECT * FROM sys.Types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='productIdType')
    DROP TYPE dbo.productIdType;
IF EXISTS (SELECT * FROM sys.Types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='resourceType')
    DROP TYPE dbo.resourceType;
IF EXISTS (SELECT * FROM sys.Types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='resourceIdType')
    DROP TYPE dbo.resourceIdType;
IF EXISTS (SELECT * FROM sys.Types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='serviceappointmentType')
    DROP TYPE dbo.serviceappointmentType;
IF EXISTS (SELECT * FROM sys.Types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='serviceappointmentIdType')
    DROP TYPE dbo.serviceappointmentIdType;
IF EXISTS (SELECT * FROM sys.Types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='serviceType')
    DROP TYPE dbo.serviceType;
IF EXISTS (SELECT * FROM sys.Types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='serviceIdType')
    DROP TYPE dbo.serviceIdType;
IF EXISTS (SELECT * FROM sys.Types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='taskType')
    DROP TYPE dbo.taskType;
IF EXISTS (SELECT * FROM sys.Types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='taskIdType')
    DROP TYPE dbo.taskIdType;
IF EXISTS (SELECT * FROM sys.Types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='territoryType')
    DROP TYPE dbo.territoryType;
IF EXISTS (SELECT * FROM sys.Types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='territoryIdType')
    DROP TYPE dbo.territoryIdType;


	
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name='fso')
BEGIN
    EXEC ('CREATE SCHEMA fso AUTHORIZATION dbo'); -- Avoid batch error
END;


