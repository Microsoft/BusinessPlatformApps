DROP TABLE HistoricalData;

CREATE TABLE HistoricalData (
	authorizationAction VARCHAR(250),
	authorizationRole VARCHAR(250), 
	authorizationScope VARCHAR(1000),
	caller VARCHAR(50),
	channels VARCHAR(50),
	claimsName VARCHAR(50),
	correlationId VARCHAR(250),
	description VARCHAR(MAX),
	eventDataId VARCHAR(250), 
	eventName VARCHAR(150),
	eventSource VARCHAR(150), 
	httpRequestClientId	VARCHAR(150),
	httpRequestClientIpAddr VARCHAR(50),
	httpRequestMethod VARCHAR(20),
	id VARCHAR(MAX),
	level VARCHAR(100),
	resourceGroupName VARCHAR(100),
	resourceProviderName VARCHAR(100),
	resourceUri VARCHAR(350),
	operationId VARCHAR(250),
	operationName VARCHAR(250),
	statusCode VARCHAR(50),
	status VARCHAR(50),
	subStatus VARCHAR(150),
	eventTimestamp VARCHAR(50),
	submissionTimestamp VARCHAR(50),
	subscriptionId VARCHAR(100));


