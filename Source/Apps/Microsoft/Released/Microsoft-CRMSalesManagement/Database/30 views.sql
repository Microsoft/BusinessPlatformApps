SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
go

-- AccountView
CREATE VIEW smgt.accountview
AS
  SELECT accountid                          AS [Account Id],
         [NAME]                             AS [Account Name],
         ownerid                            AS [Owner Id],
         territoryid                        AS [Territory Id],
         osm.LocalizedLabel                 AS [Industry],
         owningbusinessunit                 AS [Business Unit Id],
         address1_city                      AS [City],
         address1_stateorprovince           AS [State],
         address1_country                   AS [Country],
         revenue                            AS [Annual Revenue]
  FROM   dbo.account LEFT OUTER JOIN dbo.OptionSetMetadata osm ON account.industrycode=osm.[Option] AND
                                                                  osm.IsUserLocalizedLabel=0 AND osm.LocalizedLabelLanguageCode=1033 AND
                                                                  osm.OptionSetName = 'industrycode'  COLLATE Latin1_General_100_CI_AS AND
                                                                  osm.EntityName = 'account' COLLATE Latin1_General_100_CI_AS
  UNION ALL
  SELECT opportunityid      AS [Account Id],
         NULL               AS [Account Name],
         ownerid            AS [Owner Id],
         NULL               AS [Territory Id],
         NULL               AS [Industry],
         owningbusinessunit AS [Business Unit Id],
         NULL               AS [City],
         NULL               AS [State],
         NULL               AS [Country],
         NULL               AS [Annual Revenue]
  FROM   dbo.opportunity
  WHERE  ( parentaccountid IS NULL );
go  


-- ActualSalesView
CREATE VIEW smgt.actualsalesview
AS
  SELECT invoiceid                              AS [Invoice Id],
         actualsales                            AS [Actual Sales],
         CONVERT(DATE, invoicedate)             AS [Invoice Date],
         CONVERT(UNIQUEIDENTIFIER, [accountid]) AS [Account Id],
         CONVERT(UNIQUEIDENTIFIER, [productid]) AS [Product Id]
  FROM   smgt.actualsales
  WHERE  EXISTS (SELECT *
                 FROM   smgt.[configuration]
                 WHERE  configuration_group = 'DATA' AND configuration_subgroup = 'actual_sales' AND [name] = 'enabled' AND value = '1')
  UNION ALL
  -- This gets the Opportunity's OpportunityProducts that can be prorated.
  SELECT CONVERT(VARCHAR(50), op.opportunityproductid)                        AS [Invoice Id],
         CASE
             WHEN o.totallineitemamount_base = 0 THEN NULL
             ELSE op.baseamount_base * o.actualvalue_base / o.totallineitemamount_base
         END                                                                  AS [Actual Sales],   -- Allocate line items based on ratio to actual total
         CONVERT(DATE, o.actualclosedate)                                     AS [Invoice Date],
         CONVERT(UNIQUEIDENTIFIER, o.parentaccountid)                         AS [Account ID],
         CONVERT(UNIQUEIDENTIFIER, op.productid)                              AS [Product ID]
  FROM   dbo.opportunity AS o INNER JOIN dbo.opportunityproduct AS op ON o.opportunityid = op.opportunityid
                              LEFT OUTER JOIN dbo.StatusMetadata sm ON o.statecode=sm.[State] AND
                                              sm.IsUserLocalizedLabel=0 AND sm.LocalizedLabelLanguageCode=1033 AND
                                              sm.EntityName='opportunity' COLLATE Latin1_General_100_CI_AS 
  WHERE  sm.LocalizedLabel = 'Won' AND
         op.baseamount_base > 0 AND
         op.baseamount_base IS NOT NULL AND
         NOT EXISTS (SELECT *
                     FROM   smgt.[configuration]
                     WHERE  configuration_group = 'DATA' AND configuration_subgroup = 'actual_sales' AND [name] = 'enabled' AND [value] = '1')
  UNION ALL
  -- This gets the Opportunities for which there are no OpportunityProducts that can be used
  SELECT CONVERT(VARCHAR(50), o.opportunityid)        AS [Invoice Id],
         o.actualvalue_base                           AS [Actual Sales],
         CONVERT(DATE, o.actualclosedate)             AS [Invoice Date],
         CONVERT(UNIQUEIDENTIFIER, o.parentaccountid) AS [Account ID],
         NULL                                         AS [Product ID]
  FROM   dbo.opportunity AS o LEFT OUTER JOIN dbo.StatusMetadata sm ON o.statecode=sm.[State] AND
                                              sm.IsUserLocalizedLabel=0 AND sm.LocalizedLabelLanguageCode=1033 AND
                                              sm.EntityName='opportunity' COLLATE Latin1_General_100_CI_AS
  WHERE  sm.LocalizedLabel = 'Won' AND
         NOT EXISTS (SELECT *
                     FROM   opportunityproduct op
                     WHERE  op.opportunityid = o.opportunityid AND op.baseamount_base > 0) AND
         NOT EXISTS (SELECT *
                     FROM   smgt.[configuration]
                     WHERE  configuration_group = 'DATA' AND configuration_subgroup = 'actual_sales' AND [name] = 'enabled' AND [value] = '1');
go


-- BusinessUnitView
CREATE VIEW smgt.businessunitview
AS
    WITH tree AS
    (
        SELECT parentbusinessunitid,
                parentbusinessunitidname,
                businessunitid,
                NAME,
                0                                    AS [Level],
                Cast(businessunitid AS VARCHAR(max)) AS pth
        FROM   dbo.businessunit
        WHERE  parentbusinessunitid IS NULL 
        UNION ALL
        SELECT a.parentbusinessunitid,
                a.parentbusinessunitidname,
                a.businessunitid,
                a.NAME,
                t.[level] + 1                                  AS [Level],
                t.pth + Cast(a.businessunitid AS VARCHAR(max)) AS pth
        FROM   tree AS t INNER JOIN dbo.businessunit AS a ON a.parentbusinessunitid = t.businessunitid
    )
    SELECT hierarchy.businessunitid AS [Business Unit Id],
            hierarchy.NAME           AS [Business Unit Name],
            [level],
            CONVERT(VARCHAR, b.NAME) AS Level1,
            CONVERT(VARCHAR, c.NAME) AS Level2,
            CONVERT(VARCHAR, d.NAME) AS Level3
    FROM   (SELECT businessunitid,
                    NAME,
                    [level],
                    CONVERT(UNIQUEIDENTIFIER, NULLIF(Substring(pth, 1, 36), ''))  AS Level1,
                    CONVERT(UNIQUEIDENTIFIER, NULLIF(Substring(pth, 37, 36), '')) AS Level2,
                    CONVERT(UNIQUEIDENTIFIER, NULLIF(Substring(pth, 73, 36), '')) AS Level3
            FROM tree) AS hierarchy LEFT JOIN businessunit AS b ON b.businessunitid = level1
                                    LEFT JOIN businessunit AS c ON c.businessunitid = level2
                                    LEFT JOIN businessunit AS d ON d.businessunitid = level3;
go


-- ConfigurationView
CREATE VIEW smgt.configurationview
AS
    SELECT [id],
            configuration_group    AS [Configuration Group],
            configuration_subgroup AS [Configuration Subgroup],
            [name]                 AS [Name],
            [value]                AS [Value]
    FROM   smgt.[configuration]
    WHERE  visible = 1;
go

-- DateView
CREATE VIEW smgt.dateview
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
    FROM   smgt.[date];
go

-- LeadView
CREATE VIEW smgt.leadview
AS
    SELECT estimatedamount               AS [Estimated Amount],
           osm1.LocalizedLabel           AS [Status],
           osm2.LocalizedLabel           AS [Lead Quality],
           [subject]                     AS [Subject],
           jobtitle                      AS [Job Title],
           leadid                        AS [Lead Id],
           estimatedamount_base          AS [Estimated Amount Base],
           ownerid                       AS [Owner Id],
           osm3.LocalizedLabel           AS [State Code],
           campaignid                    AS [Campaign Id],
           estimatedclosedate            AS [Estimated Close Date],
           osm4.LocalizedLabel           AS [Lead Source Name],
           osm5.LocalizedLabel           AS [Industry Name],
           osm6.LocalizedLabel           AS [Purchase Time Frame],
           createdon                     AS [Created On],
           companyname                   AS [Company Name]
  FROM     dbo.lead LEFT OUTER JOIN dbo.StatusMetadata osm1 ON lead.statuscode=osm1.[Status] AND osm1.IsUserLocalizedLabel=0 AND osm1.LocalizedLabelLanguageCode=1033 AND osm1.EntityName='lead' COLLATE Latin1_General_100_CI_AS
                    LEFT OUTER JOIN dbo.OptionSetMetadata osm2 ON lead.leadqualitycode=osm2.[Option] AND osm2.OptionSetName='leadqualitycode' COLLATE Latin1_General_100_CI_AS AND osm2.IsUserLocalizedLabel=0 AND osm2.LocalizedLabelLanguageCode=1033 AND osm2.EntityName='lead' COLLATE Latin1_General_100_CI_AS
                    LEFT OUTER JOIN dbo.StateMetadata osm3 ON lead.statecode=osm3.[State] AND osm3.IsUserLocalizedLabel=0 AND osm3.LocalizedLabelLanguageCode=1033 AND osm3.EntityName='lead' COLLATE Latin1_General_100_CI_AS
                    LEFT OUTER JOIN dbo.OptionSetMetadata osm4 ON lead.leadsourcecode=osm4.[Option] AND osm4.OptionSetName='leadsourcecode' COLLATE Latin1_General_100_CI_AS AND osm4.IsUserLocalizedLabel=0 AND osm4.LocalizedLabelLanguageCode=1033 AND osm4.EntityName='lead' COLLATE Latin1_General_100_CI_AS
                    LEFT OUTER JOIN dbo.OptionSetMetadata osm5 ON lead.industrycode=osm5.[Option] AND osm5.OptionSetName='industrycode'  COLLATE Latin1_General_100_CI_AS AND osm5.IsUserLocalizedLabel=0 AND osm5.LocalizedLabelLanguageCode=1033 AND osm5.EntityName='lead' COLLATE Latin1_General_100_CI_AS
                    LEFT OUTER JOIN dbo.GlobalOptionSetMetadata osm6 ON lead.purchasetimeframe=osm6.[Option] AND osm6.OptionSetName='purchasetimeframe'  COLLATE Latin1_General_100_CI_AS AND osm6.IsUserLocalizedLabel=0 AND osm6.LocalizedLabelLanguageCode=1033;
go


-- MeasuresView
CREATE VIEW smgt.measuresview
AS
    SELECT TOP 0 1 AS MeasureValues;
go


-- OpportunityProductView
CREATE VIEW smgt.opportunityproductview
AS
    SELECT CONVERT(UNIQUEIDENTIFIER, productid) AS [Product Id],
            opportunityid                        AS [Opportunity Id],
            baseamount_base                      AS [Revenue]
    FROM   dbo.opportunityproduct;
go


-- OpportunityView
CREATE VIEW smgt.opportunityview
AS
    SELECT  o.opportunityid                     AS [Opportunity Id],
            o.NAME                              AS [Opportunity Name],
            o.ownerid                           AS [Owner Id],
            CONVERT(DATE, o.actualclosedate)    AS [Actual Close Date],
            CONVERT(DATE, o.estimatedclosedate) AS [Estimated Close Date],
            o.closeprobability                  AS [Close Probability],
            CASE
                WHEN o.parentaccountid IS NULL THEN o.opportunityid
                ELSE o.parentaccountid
            END                                 AS [Account Id],
            o.actualvalue                       AS [Actual Value],
            o.estimatedvalue                    AS [Estimated Value],
            osm1.LocalizedLabel                 AS [Status],
            CASE
                WHEN stepname IS NULL OR Charindex('-', o.stepname) = 0 THEN NULL
                ELSE LEFT(o.stepname, Charindex('-', o.stepname) - 1)
            END                                 AS [Sales Stage Code],
            o.stepname                          AS [Sales Stage],
            osm2.LocalizedLabel                 AS [State],
            o.originatingleadid                 AS [Lead Id],
            osm3.LocalizedLabel                 AS [Opportunity Rating Name],
            NULL                                AS [Source]
    FROM   dbo.opportunity o LEFT OUTER JOIN dbo.StatusMetadata osm1 ON o.statuscode=osm1.[Status] AND osm1.IsUserLocalizedLabel=0 AND osm1.LocalizedLabelLanguageCode=1033 AND osm1.EntityName='opportunity' COLLATE Latin1_General_100_CI_AS
                             LEFT OUTER JOIN dbo.StateMetadata osm2 ON o.statecode=osm2.[State] AND osm2.IsUserLocalizedLabel=0 AND osm2.LocalizedLabelLanguageCode=1033 AND osm2.EntityName='opportunity' COLLATE Latin1_General_100_CI_AS
                             LEFT OUTER JOIN dbo.OptionSetMetadata osm3 ON o.opportunityratingcode=osm3.[Option] AND osm3.OptionSetName='opportunityratingcode'  COLLATE Latin1_General_100_CI_AS AND osm3.IsUserLocalizedLabel=0 AND osm3.LocalizedLabelLanguageCode=1033 AND osm3.EntityName='opportunity' COLLATE Latin1_General_100_CI_AS;
go

-- ProductView
CREATE VIEW smgt.productview
AS
    WITH tree AS
    (
        SELECT parentproductid,
               parentproductidname,
               productid,
               NAME,
               0                               AS [Level],
               Cast(productid AS VARCHAR(max)) AS pth
        FROM   dbo.product
        WHERE  parentproductid IS NULL
        UNION ALL
        SELECT a.parentproductid,
                a.parentproductidname,
                a.productid,
                a.NAME,
                t.[level] + 1                             AS [Level],
                t.pth + Cast(a.productid AS VARCHAR(max)) AS pth
        FROM   tree AS t INNER JOIN dbo.product AS a ON a.parentproductid = t.productid
    )
    SELECT hierarchy.productid     AS [Product Id],
           hierarchy.NAME          AS [Product Name],
           [level],
           CONVERT(VARCHAR, b.NAME) AS [Product Level 1],
           CONVERT(VARCHAR, c.NAME) AS [Product Level 2],
           CONVERT(VARCHAR, d.NAME) AS [Product Level 3]
    FROM   (SELECT productid,
                   NAME,
                   [level],
                   CONVERT(UNIQUEIDENTIFIER, NULLIF(Substring(pth, 1, 36), ''))  AS Level1,
                   CONVERT(UNIQUEIDENTIFIER, NULLIF(Substring(pth, 37, 36), '')) AS Level2,
                   CONVERT(UNIQUEIDENTIFIER, NULLIF(Substring(pth, 73, 36), '')) AS Level3
            FROM   tree) AS hierarchy LEFT OUTER JOIN dbo.product AS b ON b.productid = level1
                                      LEFT OUTER JOIN dbo.product AS c ON c.productid = level2
                                      LEFT OUTER JOIN dbo.product AS d ON d.productid = level3;
go


-- QuotaView
CREATE VIEW smgt.quotaview
AS
    SELECT [amount]                               AS [Amount],
           CONVERT(DATE, [date], 101)             AS [Date],
           CONVERT(UNIQUEIDENTIFIER, [ownerid])   AS [Owner Id],
           CONVERT(UNIQUEIDENTIFIER, [productid]) AS [Product Id]
    FROM   smgt.quotas;
go

-- TargetView
CREATE VIEW smgt.targetview
AS
    SELECT CONVERT(UNIQUEIDENTIFIER, productid)      AS [Product Id],
           CONVERT(UNIQUEIDENTIFIER, businessunitid) AS [Business Unit Id],
           CONVERT(UNIQUEIDENTIFIER, territoryid)    AS [Territory Id],
           [target]                                  AS [Target],
           CONVERT(DATE, [date], 101)                AS [Date]
    FROM   smgt.targets;
go

-- TempUserView
CREATE VIEW smgt.tempuserview
AS
    SELECT a.fullname,
            CONVERT(UNIQUEIDENTIFIER, a.systemuserid)       AS systemuserid,
            CONVERT(UNIQUEIDENTIFIER, a.parentsystemuserid) AS parentsystemuserid,
            a.hierarchylevel,
            systemuser_1.fullname                           AS managername
    FROM   (
                SELECT dbo.systemuser.fullname,
                    dbo.systemuser.systemuserid,
                    dbo.systemusermanagermap.parentsystemuserid,
                    dbo.systemusermanagermap.hierarchylevel
                FROM   dbo.systemusermanagermap LEFT OUTER JOIN dbo.systemuser ON dbo.systemusermanagermap.systemuserid = dbo.systemuser.systemuserid
            ) AS a
            LEFT OUTER JOIN dbo.systemuser AS systemuser_1 ON a.parentsystemuserid = systemuser_1.systemuserid
    WHERE  a.hierarchylevel = 1 AND
            systemuser_1.isdisabled = 0;
go

-- TerritoryView
CREATE VIEW smgt.territoryview
AS
    SELECT [NAME]      AS [Territory Name],
           territoryid AS [Territory Id]
    FROM   dbo.territory;
go

-- UserAscendantsView
CREATE VIEW smgt.userascendantsview
AS
    WITH mycte(systemuserid, emailaddress, ascendantsystemuserid, ascendantemailaddress, employeelevel) AS
    (
            -- Anchor
            SELECT  u.systemuserid,
                    u.internalemailaddress AS emailaddress,
                    u.systemuserid         AS ascendantsystemuserid,
                    u.internalemailaddress AS ascendantemailaddress,
                    0                      AS employeelevel
            FROM   dbo.systemuser u
            WHERE  u.internalemailaddress IS NOT NULL AND u.isdisabled = 0
            UNION ALL
            -- ...and the recursive part
            SELECT  c.systemuserid,
                    c.emailaddress,
                    u.parentsystemuserid                           AS ascendantsystemuserid,
                    (SELECT u1.internalemailaddress
                     FROM   systemuser u1
                     WHERE  u.parentsystemuserid = u1.systemuserid) AS ascendantemailaddress,
                    c.employeelevel + 1                             AS employeelevel
            FROM   mycte c INNER JOIN dbo.systemuser u ON c.ascendantsystemuserid = u.systemuserid
            WHERE  u.parentsystemuserid IS NOT NULL AND u.isdisabled=0
    )
    SELECT  mycte.systemuserid          [User Id],
            mycte.emailaddress          [Email],
            mycte.ascendantsystemuserid [Ascendant User Id],
            mycte.ascendantemailaddress [Ascendant Email],
            mycte.employeelevel         [Employee Level],
            um.domainuser               [Ascendant Domain User]
    FROM   mycte LEFT OUTER JOIN smgt.usermapping um ON mycte.ascendantsystemuserid = um.userid;
go


-- UserView
CREATE VIEW smgt.userview
AS
    SELECT  fullname           AS [Full Name],
            systemuserid       AS [User Id],
            parentsystemuserid AS [Parent User Id],
            hierarchylevel     AS [Hierarchy Level],
            managername        AS [Manager Name]
    FROM   smgt.tempuserview
    UNION ALL
    SELECT  b.fullname                             AS [Full Name],
            b.systemuserid                         AS [User Id],
            '00000000-0000-0000-0000-000000000000' AS [Parent User Id],
            1                                      AS [Hierarchy Level],
            'Root'                                 AS [Manager Name]
    FROM   (
               SELECT DISTINCT fullname, systemuserid
               FROM   dbo.systemuser
               WHERE  isdisabled = 0 AND
                      systemuserid NOT IN (SELECT DISTINCT systemuserid FROM smgt.tempuserview)
           ) AS b
    UNION ALL
    SELECT  'Root'                                 AS [Full Name],
            '00000000-0000-0000-0000-000000000000' AS [User Id],
            '00000000-0000-0000-0000-000000000000' AS [Parent User Id],
            1                                      AS [Hierarchy Level],
            'Root'                                 AS [Manager Name];
go
