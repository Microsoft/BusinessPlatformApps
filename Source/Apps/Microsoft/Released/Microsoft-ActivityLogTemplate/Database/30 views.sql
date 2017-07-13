SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
go

CREATE VIEW aal.VerboseView
AS
	SELECT *
	FROM aal.ActivityLogData;
GO

CREATE VIEW aal.FailuresView
AS
    SELECT eventId, [caller], [description], jobFailedMessage, operationCategory, operationName, resourceGroup, resourceId, statusCode, [timestamp]
    FROM   aal.ActivityLogData
    WHERE  [status] = 'Failed' OR [status] = 'Failure';
GO

CREATE VIEW aal.ServiceHealthView
AS
	SELECT eventId, [description], impact, impactedRegions, [level], operationName, [status], subscriptionId, [timestamp] 
	FROM aal.ActivityLogData
	WHERE eventCategory = 'ServiceHealth';
GO

CREATE VIEW aal.EventsByResourceGroupView
AS
	SELECT eventId, resourceGroup
	FROM aal.ActivityLogData;
GO

CREATE VIEW aal.EventsByLevelView
AS
	SELECT eventId, [level]
	FROM aal.ActivityLogData;
GO

CREATE VIEW aal.EventsByStatusView
AS
	SELECT eventId, [status]
	FROM aal.ActivityLogData;
GO

CREATE VIEW aal.UserGeneratedEventsView
AS
	SELECT eventId
	FROM aal.ActivityLogData
	WHERE [caller] IS NOT NULL;
GO

CREATE VIEW aal.ApplicationGeneratedEventsView
AS
	SELECT eventId
	FROM aal.ActivityLogData
	WHERE [caller] = NULL;
GO