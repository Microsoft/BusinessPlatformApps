SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
go

/************************************
* Tables to drop                    *
*************************************/

-- MsCrm needs to recreate these tables, however the fields we needed in the views will still be present


DROP TABLE dbo.account;
DROP TABLE dbo.appointment;
DROP TABLE dbo.contact;
DROP TABLE dbo.email;
DROP TABLE dbo.fax;
DROP TABLE dbo.incident;
DROP TABLE dbo.letter;
DROP TABLE dbo.msdyn_survey;
DROP TABLE dbo.msdyn_surveyresponse;
DROP TABLE dbo.phonecall;
DROP TABLE dbo.slakpiinstance;
DROP TABLE dbo.systemuser;
DROP TABLE dbo.task;
DROP TABLE dbo.team;



-- Looks like these need to be remove, too
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

/************************************
* Tables to truncate                *
*************************************/
