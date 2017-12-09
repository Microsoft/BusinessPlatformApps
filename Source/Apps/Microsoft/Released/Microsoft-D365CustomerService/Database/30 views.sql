SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
go


create view [csrv].[activityview] as
select id                                   AS [Activity Id],
regardingobjectid                           AS [Case Id],
directioncode                               AS [Direction Code],
'email'                                     AS [Activity Type]
from email
WHERE regardingobjectid_entitytype = 'Incident'
UNION ALL
select id                                   AS [Activity Id],
regardingobjectid                           AS [Case Id],
directioncode                               AS [Direction Code],
'fax'                                       AS [Activity Type]
from fax
WHERE regardingobjectid_entitytype = 'Incident'
UNION ALL
select id                                   AS [Activity Id],
regardingobjectid                           AS [Case Id],
directioncode                               AS [Direction Code],
'phonecall'                                 AS [Activity Type]
from phonecall
WHERE regardingobjectid_entitytype = 'Incident'

GO

create view [csrv].[caseview] as
select incidentid                          AS [Case Id],
       accountid                           AS [Account Id],
	   customerid                          AS [Customer Id],
	   i.ownerid                           AS [Owner Id],
	   title                               AS Title,
       Cast(i.createdon AS DATE)           AS [Created On Date],
       i.createdon                         AS [Created On],
	   Cast(i.resolveby AS DATE)           AS [Resolve By Date],
	   sentimentvalue                      AS [Sentiment Score],
	   resolvekpistat.[Value]              AS [Resolve SLA Status],
	   resolvekpistat.LocalizedLabel       AS [Resolve SLA],
	   responsekpistat.[Value]             AS [First Response SLA Status],
	   responsekpistat.LocalizedLabel      AS [First Response SLA],
	   i.modifiedon                        AS [Modified On],
	   case when osm2.LocalizedLabel='Active' THEN null ELSE Cast(i.modifiedon AS DATE) END AS [Resolved On Date],
       osm1.LocalizedLabel                 AS [Status],
       osm2.LocalizedLabel                 AS [State],
	   coc.LocalizedLabel                  AS [Origin],
	   prioritycode                        AS [Priority Code],
	   prio.LocalizedLabel                 AS [Priority],
	   case when escalatedOn IS NULL then 0 else 1 end AS [Escalated],
	   Cast(responseby AS DATE)            AS [Response By Date],
	   Cast(followupby AS DATE)            AS [Follow Up By Date],
	   Cast(escalatedon AS DATE)           AS [Escalated Date],
	   customersatisfactioncode            AS [Customer Satisfaction Code],
	   csat.LocalizedLabel                 AS [Customer Satisfaction]
    FROM   dbo.incident i
	   LEFT OUTER JOIN dbo.slakpiinstance slakpiresolve on i.resolvebykpiid=slakpiresolve.Id
	   LEFT OUTER JOIN dbo.slakpiinstance slakpiresponse on i.firstresponsebykpiid=slakpiresponse.Id
	   LEFT OUTER JOIN dbo.StatusMetadata osm1 ON i.statuscode=osm1.[Status] AND osm1.IsUserLocalizedLabel=0 AND osm1.LocalizedLabelLanguageCode=1033 AND osm1.EntityName='incident' COLLATE Latin1_General_100_CI_AS
       LEFT OUTER JOIN dbo.StateMetadata osm2 ON i.statecode=osm2.[State] AND osm2.IsUserLocalizedLabel=0 AND osm2.LocalizedLabelLanguageCode=1033 AND osm2.EntityName='incident' COLLATE Latin1_General_100_CI_AS
       LEFT OUTER JOIN 
		(select 
				[Option] as [Value], 
				[LocalizedLabel] 
				from [dbo].[GlobalOptionSetMetadata] 
				where  [OptionsetName] = 'caseorigincode' and [LocalizedLabelLanguageCode] = 1033
			) coc on i.caseorigincode= coc.[value]
		LEFT OUTER JOIN
		(select 
				[Option] as [Value], 
				[LocalizedLabel] 
				from [dbo].optionsetmetadata 
				where [OptionsetName] = 'prioritycode' and EntityName='incident' and [LocalizedLabelLanguageCode] = 1033
		) prio on i.prioritycode = prio.[value]
		LEFT OUTER JOIN
		(select 
				[Option] as [Value], 
				[LocalizedLabel] 
				from [dbo].optionsetmetadata 
				where [OptionsetName] = 'customersatisfactioncode' and EntityName='incident'and [LocalizedLabelLanguageCode] = 1033
		) csat on i.customersatisfactioncode = csat.[value]
		LEFT OUTER JOIN 
		(select 
				[Option] as [Value], 
				[LocalizedLabel] 
				from [dbo].optionsetmetadata 
				where [OptionsetName] = 'status' and EntityName='slakpiinstance' and [LocalizedLabelLanguageCode] = 1033
		) resolvekpistat on slakpiresolve.[status] = resolvekpistat.[value]
		LEFT OUTER JOIN 
		(select 
				[Option] as [Value], 
				[LocalizedLabel] 
				from [dbo].optionsetmetadata 
				where [OptionsetName] = 'status' and EntityName='slakpiinstance' and [LocalizedLabelLanguageCode] = 1033
		) responsekpistat on slakpiresponse.[status] = responsekpistat.[value]



GO


create view [csrv].[customerview] AS
SELECT   id                                 AS [Customer Id],
         [NAME]                             AS [Customer Name],
		 address1_city                      AS [City],
         address1_stateorprovince           AS [State],
         address1_country                   AS [Country]
FROM account
WHERE id in (SELECT customerid FROM incident WHERE customerid_entitytype='account')
UNION ALL
SELECT   id                                 AS [Customer Id],
         [yomifullname]                     AS [Customer Name],
		 address1_city                      AS [City],
         address1_stateorprovince           AS [State],
         address1_country                   AS [Country]
FROM contact
WHERE id in (SELECT customerid FROM incident WHERE customerid_entitytype='contact')

GO


CREATE VIEW [csrv].[dateview]
AS
    SELECT date_key,
           full_date        AS [Date],
           day_of_week      AS [Day of the Week],
           day_num_in_month AS [Day Number of the Month],
           day_name         AS [Day Name],
           day_abbrev       AS [Day Abbreviated],
           weekday_flag     AS [Weekday Flag],
           [month],
           month_name       AS [Month Name],
           month_abbrev,
           [quarter],
           [year],
           same_day_year_ago_date,
           week_begin_date  AS [Week Begin Date]
    FROM   csrv.[date];

GO


create view [csrv].[ownerview] as
SELECT 
systemuserid             AS [Owner ID],
fullname                 AS [Owner Name],
'User'                   AS [Owner Type]
FROM systemuser
WHERE
   systemuserid IN (select ownerid from incident)
UNION ALL
SELECT
   id                AS [Owner Id],
   [NAME]            AS [Owner Name],
   'Team'            AS [Owner Type]
FROM   dbo.team 
WHERE
   id IN (select owningteam from incident)

GO


create view [csrv].[surveyresponseview] AS
SELECT
Id                                          AS [Survey Response Id],
msdyn_surveyid                              AS [Survey Id],
owninguser                                  AS [Owner Id],
nps.localizedlabel                          AS [NPS Type],
msdyn_case                                  AS [Case Id],
CAST(msdyn_completedon AS DATE)             AS [Completed On Date],
msdyn_scoreaspercentage                     AS [Score]
FROM [dbo].[msdyn_surveyresponse] sr
LEFT OUTER JOIN (
   SELECT 
     [Option] as [Value], 
     [LocalizedLabel] 
   FROM [dbo].[GlobalOptionSetMetadata] 
   WHERE  [OptionsetName] = 'msdyn_npstype' and [LocalizedLabelLanguageCode] = 1033
) nps on sr.msdyn_npstype= nps.[value]

GO

create view [csrv].[surveyview] as
select id                                   AS [Survey Id],
       msdyn_name                           AS [Survey Name]
FROM [dbo].[msdyn_survey]

GO

-- ConfigurationView
CREATE VIEW csrv.[configurationview]
AS
    SELECT [id],
            configuration_group    AS [Configuration Group],
            configuration_subgroup AS [Configuration Subgroup],
            [name]                 AS [Name],
            [value]                AS [Value]
    FROM   csrv.[configuration]
    WHERE  visible = 1;

GO


-- MeasuresView
CREATE VIEW [csrv].[measuresview]
AS
    SELECT TOP 0 1 AS MeasureValues;

GO

-- teamview
CREATE VIEW [csrv].[teamview]
AS
  SELECT id                          AS [Team Id],
         [NAME]                      AS [Team Name]
  FROM   dbo.team 
  WHERE
   id IN (select owningteam from incident)

GO

-- userview
CREATE VIEW csrv.[userview]
AS
    WITH OrderedUSers(fullname, systemuserid, parentsystemuserid, hierarchylevel, managername) AS (
        SELECT fullname, systemuserid, parentsystemuserid, 0 AS hierarchylevel, CAST(NULL AS NVARCHAR(250)) AS managername
        FROM dbo.systemuser su
	    WHERE parentsystemuserid IS NULL AND isdisabled=0

	    UNION ALL
	    SELECT su.fullname, su.systemuserid, su.parentsystemuserid, OrderedUSers.hierarchylevel+1 AS hierarchylevel, CAST(OrderedUSers.fullname AS NVARCHAR(250)) AS managername
	    FROM dbo.systemuser su INNER JOIN OrderedUSers ON su.parentsystemuserid=OrderedUSers.systemuserid
	    WHERE su.isdisabled = 0
    )

SELECT  systemuserid        [User Id],         
	fullname              [Full Name],
    parentsystemuserid     [Parent User Id],
	hierarchylevel         [Employee Level]
	FROM OrderedUSers;

GO
