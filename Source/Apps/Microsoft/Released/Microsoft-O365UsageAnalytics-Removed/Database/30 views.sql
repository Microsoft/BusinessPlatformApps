SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
go

CREATE VIEW bpst_aal.AdministrativeView
AS
    SELECT 
        eventId				AS [Event ID],
        [caller]			AS [Caller],
        correlationId		AS [Correlation ID],
        [description]		AS [Description],
        eventCategory		AS [Event Category], 
        [level]				AS [Level],
        operationCategory	AS [Operation Category],
        operationId			AS [Operation ID],
        operationName		AS [Operation Name],
        resourceGroup		AS [Resource Group],
        resourceId			AS [Resource ID],
        resourceProvider    AS [Resource Provider],
        [status]			AS [Status],
        [timestamp]			AS [Timestamp]
    FROM bpst_aal.AdministrativeData;
GO

CREATE VIEW bpst_aal.DateView
AS
    SELECT * 
    FROM bpst_aal.[date];
GO

CREATE VIEW bpst_aal.ServiceHealthView
AS
    SELECT * FROM (
        SELECT         
            shd.correlationId           AS [Correlation ID],
            shd.operationId	            AS [Operation ID],
            shd.[timestamp]				AS [Timestamp],
            shd.serviceHealthId         AS [Service Health ID],
            shd.[description]			AS [Description], 
            shd.impact					AS [Impact], 
            shd.impactedRegions			AS [Impacted Regions], 
            shd.impactedServices		AS [Impacted Services], 
            shd.incidentType            AS [Incident Type],
            shd.[level]					AS [Level], 
            shd.[status]			    AS [Status], 
            shd.[title]                 AS [Title],
        RANK() OVER (PARTITION BY operationId ORDER BY timestamp DESC) AS rnk FROM bpst_aal.ServiceHealthData shd
    ) T
    WHERE T.rnk=1

