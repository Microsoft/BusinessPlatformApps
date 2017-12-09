SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
go


-- Computer malware
CREATE VIEW pbist_sccm.vw_computermalware
AS
    SELECT threatid,
           machineid,
           10000*Year([detection date]) + 100*Month([detection date]) + Day([detection date]) date_key,
           [observer product name],
           [observer product version],
           [observer detection],
           [remediation type],
           [remediation result],
           [remediation error code],
           [remediation pending action],
           [is active malware]
    FROM   pbist_sccm.computermalware
    WHERE DateDiff(day, [detection date], GetDate()) <=365;
go


-- Computer program
CREATE VIEW pbist_sccm.vw_computerprogram
AS 
  SELECT machineid, 
         [program name],
         publisher,
         [version]
  FROM   pbist_sccm.computerprogram;
go

-- Computer update
CREATE VIEW pbist_sccm.vw_computerupdate
AS 
  SELECT machineid, 
         ci_id, 
         laststatuschangetime [last status change time], 
         laststatuschecktime  [last status check time], 
         [status]
  FROM   pbist_sccm.computerupdate;
go

CREATE VIEW pbist_sccm.vw_computer
AS 
SELECT machineid,
       sitecode,
       [name],
       CASE [client type]
           WHEN 1 THEN 'Computer'
           WHEN 3 THEN 'Mobile'
           ELSE 'Undetermined'
       END AS [client type], 
       CASE
           WHEN [operating system] LIKE 'Windows Phone%' THEN 'Windows Phone'
           WHEN [operating system] LIKE 'iOS%' THEN 'iOS'
           WHEN [operating system] LIKE 'Android%' THEN 'Android'
           WHEN [operating system] LIKE 'OS X%' THEN 'OS X'


            --- Windows Workstation
           WHEN [operating system] = 'Microsoft Windows NT Workstation 5.0' THEN 'Windows 2000 Professional'
           WHEN [operating system] = 'Microsoft Windows NT Workstation 5.1' OR [operating system] = 'Microsoft Windows NT Workstation 5.1 (Embedded)' OR 
                [operating system] = 'Microsoft Windows NT Workstation 5.1 (Tablet Edition)' OR
                [operating system] = 'Microsoft Windows NT Workstation 5.2' THEN 'Windows XP'
           WHEN [operating system] = 'Microsoft Windows NT Workstation 6.0' OR [operating system] = 'Microsoft Windows NT Workstation 6.0 (Tablet Edition)' THEN 'Windows Vista'
           WHEN [operating system] LIKE '%Windows%Workstation 6.1%' OR [operating system] LIKE 'Windows 7%6.1' OR [operating system] LIKE 'Windows Embedded%6.1' THEN 'Windows 7'
           WHEN [operating system] LIKE '%Windows%Workstation 6.2%' THEN 'Windows 8'
           WHEN [operating system] LIKE '%Windows%Workstation 6.3%' OR [operating system] LIKE 'Windows 8.1%' THEN 'Windows 8.1'
           WHEN [operating system] LIKE 'Windows 10%' OR
                [operating system] = 'Microsoft Windows NT Workstation 10.0' OR
                [operating system] LIKE '%Windows%Workstation 6.4%' OR
                [operating system] = 'Microsoft Windows NT Workstation 10.0 (Tablet Edition)' THEN 'Windows 10'
            --- Windows Server
           WHEN [operating system] = 'Microsoft Windows NT Server 5.0' OR [operating system] = 'Microsoft Windows NT Advanced Server 5.0' THEN 'Windows Server 2000' 
           WHEN [operating system] = 'Microsoft Windows NT Advanced Server 5.2' OR [operating system] = 'Microsoft Windows NT Server 5.2' THEN 'Windows Server 2003'
           WHEN [operating system] = 'Microsoft Windows NT Advanced Server 6.0' OR [operating system] = 'Microsoft Windows NT Server 6.0' THEN 'Windows Server 2008'
           WHEN [operating system] = 'Microsoft Windows NT Advanced Server 6.1' OR [operating system] = 'Microsoft Windows NT Server 6.1' THEN 'Windows Server 2008 R2'
           WHEN [operating system] = 'Microsoft Windows NT Advanced Server 6.2' OR [operating system] = 'Microsoft Windows NT Server 6.2' THEN 'Windows Server 2012'
           WHEN [operating system] = 'Microsoft Windows NT Advanced Server 6.3' OR [operating system] = 'Microsoft Windows NT Server 6.3' THEN 'Window Server 2012 R2'
           WHEN [operating system] = 'Microsoft Windows NT Advanced Server 10.0' OR [operating system] = 'Microsoft Windows NT Server 10.0' OR
                [operating system] LIKE 'Windows Server 2016%' OR
                [operating system] LIKE '%Windows%Server 6.4%' THEN 'Windows Server 2016'
           ELSE
                [operating system]
       END AS [operating system name],
       [operating system] AS [operating system long name],
       manufacturer,
       model,
       [platform],
       [physical memory]
FROM pbist_sccm.computer AS c
WHERE [deleted date] IS NULL;
go


CREATE VIEW pbist_sccm.vw_scanhistory
AS
  SELECT machineid,
         date_key,
         sitecode,
         [enabled],
         [client version],
         CONVERT(CHAR(1), [real time protection enabled])    [real time protection enabled],
         CONVERT(CHAR(1), [on access protection enabled])    [on access protection enabled],
         CONVERT(CHAR(1), [input/output protection enabled]) [input/output protection enabled],
         CONVERT(CHAR(1), [behavior monitor enabled])        [behavior monitor enabled],
         CONVERT(CHAR(1), [antivirus enabled])               [antivirus enabled],
         CONVERT(CHAR(1), [antispyware enabled])             [antispyware enabled],
         CONVERT(CHAR(1), [nis enabled])                     [nis enabled],
         [quick scan age (days)],
         [full scan age (days)],
         [signature age (days)],
         [engine version],
         [antivirus signature version],
         [missing critical update count],
         [client active status],
         [health evaluation result],
         [client state]              -- 1: Active/Pass, 2: Active/Fail, 3; Active/Unknown, 4: Inactive/Pass, 5: Inactive/Fail, 6; Inactive/Unknown,

  FROM   pbist_sccm.scanhistory;
go

CREATE VIEW pbist_sccm.vw_update
AS
  SELECT ci_id,
         articleid,
         bulletinid,
         severity,
         CASE
           WHEN severityname IS NULL OR Len(severityname) = 0 THEN 'None'
           ELSE severityname
         END [severity name],
         title,
         infoURL
  FROM   pbist_sccm.[update];
go

CREATE VIEW pbist_sccm.vw_usercomputer
AS
  SELECT machineid,
         username, 
         [full name] 
  FROM   pbist_sccm.usercomputer;
go


CREATE VIEW pbist_sccm.vw_user
AS 
  SELECT username, 
         [full name] 
  FROM   pbist_sccm.[user];
go

-- ConfigurationView
CREATE VIEW pbist_sccm.vw_configuration
AS
    SELECT [id],
            configuration_group    AS [configuration group],
            configuration_subgroup AS [configuration subgroup],
            [name],
            [value],
			CASE
			   WHEN [name]='lastLoadTimestamp' AND configuration_subgroup='System Center' THEN CONVERT(datetime, [value], 126) -- We need this because Power BI / DAX cannot convert from ISO8601 date
			   ELSE NULL
			END AS value_as_datetime
    FROM   pbist_sccm.[configuration]
    WHERE  visible = 1;
go

-- DateView
CREATE VIEW pbist_sccm.vw_date
AS
    SELECT date_key,
           full_date        AS [date],
           day_of_week      AS [day of the week],
           day_num_in_month AS [day number of the month],
           day_name         AS [day name],
           day_abbrev       AS [day abbreviated],
           weekday_flag     AS [weekday flag],
           [month],
           month_name       AS [month name],
           month_abbrev,
           [quarter],
           [year],
           same_day_year_ago_date,
           week_begin_date  AS [week begin date]
    FROM   pbist_sccm.[date];
go

-- Program View
CREATE VIEW pbist_sccm.vw_program
AS 
  SELECT [program name],
         publisher,
         [version] 
  FROM   pbist_sccm.[program]
  WHERE  [program name] IS NOT NULL AND
		 publisher IS NOT NULL AND
		 [version] IS NOT NULL AND	  
		 UNICODE([program name]) <> 127 AND
		 UNICODE(RIGHT([version],1)) > 31;
go

CREATE VIEW pbist_sccm.vw_collection
AS 
  SELECT collectionid, 
         [collection name] 
  FROM   pbist_sccm.[collection];
go

CREATE VIEW pbist_sccm.vw_computercollection
AS
  SELECT collectionid,
         resourceid
  FROM   pbist_sccm.computercollection;
go
