SET NOCOUNT ON;

DECLARE @curdate DATE = GETDATE();

WITH machines AS
(
    SELECT computer.ItemKey                    AS MachineID,
           client.SiteCode,
           computer.Name0                      AS [NAME],
           computer.Operating_System_Name_and0 AS [Operating System],
           computer.Client_Type0               AS [Client Type]
    FROM   dbo.vsms_r_system computer LEFT OUTER JOIN dbo.v_clientmachines AS client ON computer.ItemKey=client.ResourceID
    WHERE  computer.Decommissioned0=0 AND computer.Obsolete0=0
),
missingupdatecount AS
(
    SELECT  [status].machineid,
            SUM(CASE u.severity WHEN 10 THEN 1 ELSE 0 END) AS [Missing Critical Update Count],
            SUM(CASE u.severity WHEN 8 THEN 1 ELSE 0 END)  AS [Missing Important Update Count]
    FROM
        dbo.vsms_update_compliancestatus [status] INNER JOIN dbo.vsms_softwareupdate u ON [status].ci_id=u.ci_id
    WHERE
        [status].[status]<>3 AND u.isenabled=1 AND u.issuperseded=0 AND u.severity>=8
    GROUP BY
        [status].machineid
),
epcompliance AS
(
    SELECT  resourceid             AS MachineID,
            CASE
                WHEN enabled = 1 THEN 1
                ELSE 0
            END                       AS [Enabled],
            [version]                 AS [Client Version],
            CASE
                WHEN rtpenabled = 1 THEN 1
                ELSE 0
            END                       AS [Real Time Protection Enabled],
            CASE
                WHEN onaccessprotectionenabled = 1 THEN 1
                ELSE 0
            END                       AS [On Access Protection Enabled],
            CASE
                WHEN ioavprotectionenabled = 1 THEN 1
                ELSE 0
            END                       AS [Input/Output Protection Enabled],
            CASE
                WHEN behaviormonitorenabled = 1 THEN 1
                ELSE 0
            END                       AS [Behavior Monitor Enabled],
            CASE
                WHEN antivirusenabled = 1 THEN 1
                ELSE 0
            END                       AS [Antivirus Enabled],
            CASE
                WHEN antispywareenabled = 1 THEN 1
                ELSE 0
            END                       AS [Antispyware Enabled],
            CASE
                WHEN nisenabled = 1 THEN 1
                ELSE 0
            END                       AS [NIS Enabled],
            CASE
                WHEN lastquickscandatetimestart > Getdate() THEN 0
                ELSE Datediff(day, lastquickscandatetimestart, Getdate())
            END                       AS [Quick Scan Age (days)],
            CASE
                WHEN lastfullscandatetimestart > Getdate() THEN 0
                ELSE Datediff(day, lastfullscandatetimestart, Getdate())
            END                       AS [Full Scan Age (days)],
            CASE
                WHEN antivirussignatureupdatedatetime > Getdate() THEN 0
                ELSE Datediff(day, antivirussignatureupdatedatetime, Getdate())
            END                       AS [Signature Age (days)],
            engineversion             [Engine Version],
            antivirussignatureversion AS [Antivirus Signature Version]
    FROM   dbo.vsms_g_system_antimalwarehealthstatus
),
healthsummary AS
(
    SELECT cs.resourceid                 AS MachineID,
           cs.lastonline,
           cs.laststatusmessage          [Last Health Status Message],
           cs.lasthealthevaluation       [Last Health Evaluation],
           cs.lasthealthevaluationresult [Last Health Evaluation Result],
           cs.clientactivestatus         [Client Active Status],
           cs.clientstate                [Client State],
           cs.lastddr                    [Last DDR],
           cs.lasthw                     [Last HW],
           cs.lastsw                     [Last SW],
           cs.laststatusmessage          [Last Status Message],
           cs.lastpolicyrequest          [Last Policy Request]
    FROM  dbo.[v_ch_clientsummary] cs
),
scansummary AS
(
    SELECT resourceid   AS MachineID,
           lastscantime AS [Last Scan Time]
    FROM   dbo.v_updatescanstatus
)
SELECT
    c.machineid   AS MachineID,
    10000*Year(@curdate) + 100*Month(@curdate) + Day(@curdate) AS date_key,
    sitecode,
    [client type],
    ep.[enabled],
    ep.[client version],
    ep.[real time protection enabled],
    ep.[on access protection enabled],
    ep.[input/output protection enabled],
    ep.[behavior monitor enabled],
    ep.[antivirus enabled],
    ep.[antispyware enabled],
    ep.[nis enabled],
    ep.[quick scan age (days)],
    ep.[full scan age (days)],
    ep.[signature age (days)],
    ep.[engine version],
    ep.[antivirus signature version],
    updates.[missing important update count],
    updates.[missing critical update count],
    hs.[client active status],
    hs.[last health evaluation result],
    hs.[last health evaluation],
    hs.[last health status message],
    hs.lastonline [Last Online],
    hs.[client state],
    hs.[last ddr],
    hs.[last hw],
    hs.[last sw],
    hs.[last status message],
    hs.[last policy request],
    ss.[last scan time]
FROM   machines c LEFT OUTER JOIN epcompliance ep ON c.machineid = ep.machineid
                  LEFT OUTER JOIN missingupdatecount updates ON updates.machineid = c.machineid
                  INNER JOIN healthsummary hs ON hs.machineid = c.machineid
                  LEFT OUTER JOIN scansummary ss ON ss.machineid = c.machineid;