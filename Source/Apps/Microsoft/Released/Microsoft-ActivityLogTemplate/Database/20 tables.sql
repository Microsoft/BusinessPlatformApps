SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;

CREATE TABLE aal.ActivityLogData (
	eventId INT IDENTITY(1, 1) PRIMARY KEY,
	[caller] VARCHAR(MAX),
	correlationId VARCHAR(MAX),
	[description] VARCHAR(MAX),
	eventCategory VARCHAR(MAX), 
	impact VARCHAR(MAX),
	impactedRegions VARCHAR(MAX),
	jobFailedMessage VARCHAR(MAX),
	[level] VARCHAR(MAX),
	operationCategory VARCHAR(MAX),
	operationId VARCHAR(MAX),
	operationName VARCHAR(MAX),
	resourceGroup VARCHAR(MAX),
	resourceId VARCHAR(MAX),
	[status] VARCHAR(MAX),
	statusCode VARCHAR(MAX),
	subscriptionId VARCHAR(MAX),
	[timestamp] VARCHAR(MAX));