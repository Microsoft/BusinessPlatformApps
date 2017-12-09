SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
go

-- Must be executed inside the target database

-- Regular views
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='csrv' AND TABLE_NAME='activityview' AND TABLE_TYPE='VIEW')
    DROP VIEW csrv.activityview;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='csrv' AND TABLE_NAME='caseview' AND TABLE_TYPE='VIEW')
    DROP VIEW csrv.caseview;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='csrv' AND TABLE_NAME='configurationview' AND TABLE_TYPE='VIEW')
    DROP VIEW csrv.configurationview;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='csrv' AND TABLE_NAME='customerview' AND TABLE_TYPE='VIEW')
    DROP VIEW csrv.customerview;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='csrv' AND TABLE_NAME='dateview' AND TABLE_TYPE='VIEW')
    DROP VIEW csrv.dateview;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='csrv' AND TABLE_NAME='measuresview' AND TABLE_TYPE='VIEW')
    DROP VIEW csrv.measuresview;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='csrv' AND TABLE_NAME='ownerview' AND TABLE_TYPE='VIEW')
    DROP VIEW csrv.ownerview;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='csrv' AND TABLE_NAME='surveyview' AND TABLE_TYPE='VIEW')
    DROP VIEW csrv.surveyview;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='csrv' AND TABLE_NAME='teamview' AND TABLE_TYPE='VIEW')
    DROP VIEW csrv.teamview;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='csrv' AND TABLE_NAME='surveyresponseview' AND TABLE_TYPE='VIEW')
    DROP VIEW csrv.surveyresponseview;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='csrv' AND TABLE_NAME='userview' AND TABLE_TYPE='VIEW')
    DROP VIEW csrv.userview;


-- Tables
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='csrv' AND TABLE_NAME='configuration' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE csrv.[configuration];
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='csrv' AND TABLE_NAME='date' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE csrv.[date];
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='csrv' AND TABLE_NAME='entityinitialcount' AND TABLE_TYPE='BASE TABLE')
	DROP TABLE csrv.entityinitialcount;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='account' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.account;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='appointment' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.appointment;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='contact' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.contact;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='email' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.email;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='fax' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.fax;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='incident' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.incident;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='letter' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.letter;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_survey' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.msdyn_survey;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='msdyn_surveyresponse' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.msdyn_surveyresponse;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='phonecall' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.phonecall;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='slakpiinstance' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.slakpiinstance;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='systemuser' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.systemuser;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='task' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.task;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='team' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE dbo.team;
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
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='AccountType')
    DROP TYPE dbo.AccountType;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='AccountIdType')
    DROP TYPE dbo.AccountIdType;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='AppointmentType')
    DROP TYPE dbo.AppointmentType;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='AppointmentIdType')
    DROP TYPE dbo.AppointmentIdType;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='contactType')
    DROP TYPE dbo.contactType;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='contactIdType')
    DROP TYPE dbo.contactIdType;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='emailType')
    DROP TYPE dbo.emailType;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='emailIdType')
    DROP TYPE dbo.emailIdType;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='faxType')
    DROP TYPE dbo.faxType;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='faxIdType')
    DROP TYPE dbo.faxIdType;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='incidentType')
    DROP TYPE dbo.incidentType;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='incidentIdType')
    DROP TYPE dbo.incidentIdType;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='letterType')
    DROP TYPE dbo.letterType;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='letterIdType')
    DROP TYPE dbo.letterIdType;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='msdyn_surveyType')
    DROP TYPE dbo.msdyn_surveyType;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='msdyn_surveyIdType')
    DROP TYPE dbo.msdyn_surveyIdType;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='msdyn_surveyresponseType')
    DROP TYPE dbo.msdyn_surveyresponseType;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='msdyn_surveyresponseIdType')
    DROP TYPE dbo.msdyn_surveyresponseIdType;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='phonecallType')
    DROP TYPE dbo.phonecallType;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='phonecallIdType')
    DROP TYPE dbo.phonecallIdType;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='slakpiinstanceType')
    DROP TYPE dbo.slakpiinstanceType;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='slakpiinstanceIdType')
    DROP TYPE dbo.slakpiinstanceIdType;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='systemuserType')
    DROP TYPE dbo.systemuserType;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='systemuserIdType')
    DROP TYPE dbo.systemuserIdType;
	IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='taskType')
    DROP TYPE dbo.taskType;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='taskIdType')
    DROP TYPE dbo.taskIdType;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='teamType')
    DROP TYPE dbo.teamType;
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined=1 AND is_table_type=1 AND [name]='teamIdType')
    DROP TYPE dbo.teamIdType;

	
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name='csrv')
BEGIN
    EXEC ('CREATE SCHEMA csrv AUTHORIZATION dbo'); -- Avoid batch error
END;


