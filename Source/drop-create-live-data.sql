DROP TABLE eventhubsql;

CREATE TABLE dbo.eventHubSQL (
	authorizationAction VARCHAR(250),
	authorizationScope VARCHAR(250),
	callerIpAddress VARCHAR(50),
	category VARCHAR(50),
	correlationId VARCHAR(50),
	eventCategory VARCHAR(25),
	eventInitiatedBy VARCHAR(50),
	eventName VARCHAR(250),
	eventPropertyStatusCode VARCHAR(25),
	durationMs VARCHAR(50),
	jobFailedMessage VARCHAR(MAX),
	level VARCHAR(25),
	operationName VARCHAR(250),
	resourceId VARCHAR(250),
	resultSignature VARCHAR(50),
	resultType VARCHAR(50),
	statusCode VARCHAR(25),
	subStatusCode VARCHAR(25),
	time VARCHAR(50)
	);
GO
