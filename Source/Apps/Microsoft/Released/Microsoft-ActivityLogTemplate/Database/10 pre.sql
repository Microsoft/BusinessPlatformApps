﻿SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
go

-- Must be executed inside the target database

-- Regular views
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='aal' AND TABLE_NAME='FailuresView' AND TABLE_TYPE='VIEW')
    DROP VIEW aal.FailuresView;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='aal' AND TABLE_NAME='ServiceHealthView' AND TABLE_TYPE='VIEW')
    DROP VIEW aal.ServiceHealthView;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='aal' AND TABLE_NAME='EventsByResourceGroupView' AND TABLE_TYPE='VIEW')
    DROP VIEW aal.EventsByResourceGroupView;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='aal' AND TABLE_NAME='EventsByLevelView' AND TABLE_TYPE='VIEW')
    DROP VIEW aal.EventsByLevelView;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='aal' AND TABLE_NAME='EventsByStatusView' AND TABLE_TYPE='VIEW')
    DROP VIEW aal.EventsByStatusView;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='aal' AND TABLE_NAME='ApplicationGeneratedEventsView' AND TABLE_TYPE='VIEW')
    DROP VIEW aal.ApplicationGeneratedEventsView;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='aal' AND TABLE_NAME='UserGeneratedEventsView' AND TABLE_TYPE='VIEW')
    DROP VIEW aal.UserGeneratedEventsView;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='aal' AND TABLE_NAME='VerboseView' AND TABLE_TYPE='VIEW')
    DROP VIEW aal.VerboseView;
	
-- Tables
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='aal' AND TABLE_NAME='ActivityLogData' AND TABLE_TYPE='BASE TABLE')
    DROP TABLE aal.ActivityLogData;

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name='aal')
BEGIN
    EXEC ('CREATE SCHEMA aal AUTHORIZATION dbo'); -- Avoid batch error
END;
