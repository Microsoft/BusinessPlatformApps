SET NOCOUNT ON;

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS
          WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='v_GS_AntimalwareHealthStatus' AND COLUMN_NAME='AntivirusSignatureAge')
BEGIN
    EXEC ('
    WITH siteavcompliance AS
    (
        SELECT SMS_Assigned_Sites0 AS SiteCode,
               Sum(CASE
                       WHEN ep.DeploymentState=3 OR ep.DeploymentState=5 THEN 1
                       ELSE 0
                   END)            AS [SCEP Count],
               Sum(CASE
                       WHEN ep.DeploymentState=3 AND ahs.AntivirusSignatureAge BETWEEN 0 AND 7 THEN 1
                       ELSE 0
                   END)            AS [AV Definition Compliance Count],
               Sum(CASE
                       WHEN ep.DeploymentState=3 THEN 1
                       ELSE 0
                   END)            AS [AV Reach Count]
           FROM   dbo.EP_DeploymentState ep
                   INNER JOIN dbo.v_RA_System_SMSAssignedSites ass ON ass.resourceid = ep.machineid
                   INNER JOIN (SELECT DISTINCT resourceid
                               FROM   dbo.v_RA_System_SystemRoles) col ON col.resourceid = ep.machineid
                   INNER JOIN dbo.v_GS_AntimalwareHealthStatus ahs ON ass.resourceid = ahs.resourceid
           GROUP  BY ass.SMS_Assigned_Sites0
    ),
    sitecomputerhealth AS
    (
        SELECT s.sitecode,
               Sum(CASE
                       WHEN ev.healthcheckid = 1 OR ev.result = 6 THEN 1
                       ELSE 0
                   END)                             [OK Health Check Count],
               Count(ev.healthcheckid)              [Health Check Count]
        FROM ch_evalresults ev INNER JOIN dbo.v_R_System [sys] ON [sys].resourceid = ev.machineid
                               INNER JOIN dbo.v_RA_System_SMSInstalledSites SMSInst ON SMSInst.resourceid = [sys].resourceid
                               INNER JOIN dbo.v_Site s ON SMSInst.sms_installed_sites0 = s.sitecode
        GROUP BY s.sitecode
    ),
    sitehealth AS
    (
        SELECT sub.[site]   SiteCode,
               Sum(CASE sub.healthstate
                       WHEN 1 THEN sub.cnt
                   END)     AS [Client successfully communicating with MP],
               Sum(CASE sub.healthstate
                       WHEN 2 THEN sub.cnt
                   END)     AS [Client failing to communicate with MP],
               Sum(sub.cnt) AS [Total]
               FROM   ( SELECT
                            sit.sitecode        AS [Site],
                            chs.healthstate,
                            Count(chs.healthstate) AS [Cnt]
                        FROM
                            dbo.v_Site sit INNER JOIN dbo.v_ClientHealthState chs ON sit.sitecode=chs.assignedsitecode AND
                                                                              chs.healthtype = ''1000'' AND
                                                                              chs.lasthealthreportdate > (SELECT Dateadd(day, -1, Getdate())) AND
                                                                              sit.[type]=2
                        GROUP BY sit.sitecode, chs.healthstate
                       ) sub
                GROUP  BY sub.[site]
    )
    SELECT s.sitecode,
           sitename                                              [Site Name],
           [version],
           servername                                            [Server Name],
           CASE
             WHEN sss.availabilitystate = 1 THEN ''Warning''
             WHEN sss.availabilitystate = 2 THEN ''Error''
             ELSE ''Available''
           END                                                   [Availability],
           Isnull((SELECT TOP 1 CAST( Isnull(CONVERT(XML, value1).query(''/ServerLocation/Address/text()''), ''No Data'') AS NVARCHAR(100)) AS [Location]
                   FROM   vsms_sc_property_sdk
                   WHERE  propertyname = ''SiteLocation'' AND sitecode = s.sitecode), ''No Data'') [Location],
           CASE lnk.overalllinkstatus
             WHEN 0 THEN ''Deleted''
             WHEN 1 THEN ''Tombstoned''
             WHEN 2 THEN ''Active''
             WHEN 3 THEN ''Initializing''
             WHEN 4 THEN ''NotStarted''
             WHEN 5 THEN ''Error''
             WHEN 6 THEN ''Unknown''
             WHEN 7 THEN ''Degraded''
             WHEN 8 THEN ''Failed''
           END                                                   AS OverallLinkStatus,
           sh.[client successfully communicating with mp],
           sh.[client failing to communicate with mp],
           sch.[health check count],
           sch.[ok health check count],
           sav.[av definition compliance count],
           sav.[scep count],
           sav.[av reach count]
    FROM   dbo.v_site s
           LEFT OUTER JOIN dbo.vsummarizers_sitestatus sss ON s.sitecode = sss.sitecode
           LEFT OUTER JOIN dbo.rcm_replicationlinksummary_child lnk ON lnk.childsitecode = s.sitecode
           LEFT OUTER JOIN sitehealth sh ON sh.sitecode = s.sitecode
           LEFT OUTER JOIN sitecomputerhealth sch ON sch.sitecode = s.sitecode
           LEFT OUTER JOIN siteavcompliance sav ON sav.sitecode = s.sitecode');
END
ELSE
BEGIN
    EXEC ('
    WITH siteavcompliance AS
    (
        SELECT SMS_Assigned_Sites0 AS SiteCode,
               Sum(CASE
                       WHEN ep.DeploymentState=3 OR ep.DeploymentState=5 THEN 1
                       ELSE 0
                   END)            AS [SCEP Count],
               Sum(CASE
                       WHEN ep.DeploymentState=3 AND ahs.AntivirusSignatureAge0 BETWEEN 0 AND 7 THEN 1
                       ELSE 0
                   END)            AS [AV Definition Compliance Count],
               Sum(CASE
                       WHEN ep.DeploymentState=3 THEN 1
                       ELSE 0
                   END)            AS [AV Reach Count]
           FROM   dbo.EP_DeploymentState ep
                   INNER JOIN dbo.v_RA_System_SMSAssignedSites ass ON ass.resourceid = ep.machineid
                   INNER JOIN (SELECT DISTINCT resourceid
                               FROM   dbo.v_RA_System_SystemRoles) col ON col.resourceid = ep.machineid
                   INNER JOIN dbo.v_GS_AntimalwareHealthStatus ahs ON ass.resourceid = ahs.resourceid
           GROUP  BY ass.SMS_Assigned_Sites0
    ),
    sitecomputerhealth AS
    (
        SELECT s.sitecode,
               Sum(CASE
                       WHEN ev.healthcheckid = 1 OR ev.result = 6 THEN 1
                       ELSE 0
                   END)                             [OK Health Check Count],
               Count(ev.healthcheckid)              [Health Check Count]
        FROM ch_evalresults ev INNER JOIN dbo.v_R_System [sys] ON [sys].resourceid = ev.machineid
                               INNER JOIN dbo.v_RA_System_SMSInstalledSites SMSInst ON SMSInst.resourceid = [sys].resourceid
                               INNER JOIN dbo.v_Site s ON SMSInst.sms_installed_sites0 = s.sitecode
        GROUP BY s.sitecode
    ),
    sitehealth AS
    (
        SELECT sub.[site]   SiteCode,
               Sum(CASE sub.healthstate
                       WHEN 1 THEN sub.cnt
                   END)     AS [Client successfully communicating with MP],
               Sum(CASE sub.healthstate
                       WHEN 2 THEN sub.cnt
                   END)     AS [Client failing to communicate with MP],
               Sum(sub.cnt) AS [Total]
               FROM   ( SELECT
                            sit.sitecode        AS [Site],
                            chs.healthstate,
                            Count(chs.healthstate) AS [Cnt]
                        FROM
                            dbo.v_Site sit INNER JOIN dbo.v_ClientHealthState chs ON sit.sitecode=chs.assignedsitecode AND
                                                                              chs.healthtype = ''1000'' AND
                                                                              chs.lasthealthreportdate > (SELECT Dateadd(day, -1, Getdate())) AND
                                                                              sit.[type]=2
                        GROUP BY sit.sitecode, chs.healthstate
                       ) sub
                GROUP  BY sub.[site]
    )
    SELECT s.sitecode,
           sitename                                              [Site Name],
           [version],
           servername                                            [Server Name],
           CASE
             WHEN sss.availabilitystate = 1 THEN ''Warning''
             WHEN sss.availabilitystate = 2 THEN ''Error''
             ELSE ''Available''
           END                                                   [Availability],
           Isnull((SELECT TOP 1 CAST( Isnull(CONVERT(XML, value1).query(''/ServerLocation/Address/text()''), ''No Data'') AS NVARCHAR(100)) AS [Location]
                   FROM   vsms_sc_property_sdk
                   WHERE  propertyname = ''SiteLocation'' AND sitecode = s.sitecode), ''No Data'') [Location],
           CASE lnk.overalllinkstatus
             WHEN 0 THEN ''Deleted''
             WHEN 1 THEN ''Tombstoned''
             WHEN 2 THEN ''Active''
             WHEN 3 THEN ''Initializing''
             WHEN 4 THEN ''NotStarted''
             WHEN 5 THEN ''Error''
             WHEN 6 THEN ''Unknown''
             WHEN 7 THEN ''Degraded''
             WHEN 8 THEN ''Failed''
           END                                                   AS OverallLinkStatus,
           sh.[client successfully communicating with mp],
           sh.[client failing to communicate with mp],
           sch.[health check count],
           sch.[ok health check count],
           sav.[av definition compliance count],
           sav.[scep count],
           sav.[av reach count]
    FROM   dbo.v_site s
           LEFT OUTER JOIN dbo.vsummarizers_sitestatus sss ON s.sitecode = sss.sitecode
           LEFT OUTER JOIN dbo.rcm_replicationlinksummary_child lnk ON lnk.childsitecode = s.sitecode
           LEFT OUTER JOIN sitehealth sh ON sh.sitecode = s.sitecode
           LEFT OUTER JOIN sitecomputerhealth sch ON sch.sitecode = s.sitecode
           LEFT OUTER JOIN siteavcompliance sav ON sav.sitecode = s.sitecode');
END;