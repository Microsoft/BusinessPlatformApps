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
    WHERE DateDiff(day, [Detection Date], GetDate()) <=365;
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
SELECT [machineid]
      ,[sitecode]
      ,[name]
      ,case [Client Type] when 1 then 'Computer' when 3 then 'Mobile' else 'tbd' end [client type]
      ,CASE
            when [operating system] like 'iOS%' then 'iOS'
            when [operating system] like 'Android%' then 'Android'
            when [operating system] like 'OS X%' then 'OS X'
            when [operating system]='Microsoft Windows NT Advanced Server 6.3' OR [operating system]='Microsoft Windows NT Advanced Server 6.4' OR [operating system]='Windows Technical Preview for Enterprise 6.4'
                then 'Window Server 2012 R2'
            when [operating System]='Microsoft Windows NT Workstation 5.0' then 'Windows 2000 Professional'
            when [operating system] like '%Windows%Workstation 6.1%' then 'Windows 7'
            when [operating system] like '%Windows%Workstation 6.2%' then 'Windows 8'
            when [operating system] like '%Windows%Workstation 6.3%' OR [operating system] like '%Windows%Workstation 6.4%' OR [operating system] like 'Windows 8.1%' then 'Windows 8.1'
            when [operating system] ='Microsoft Windows NT Advanced Server 5.2' then 'Windows Server 2003'
            when [operating system] ='Microsoft Windows NT Advanced Server 6.0' then 'Windows Server 2008'
            when [operating system] ='Microsoft Windows NT Advanced Server 6.1' then 'Windows Server 2008 R2'
            when [operating system] ='Microsoft Windows NT Advanced Server 6.2' then 'Windows Server 2012'
            when [operating system] ='Microsoft Windows NT Advanced Server 10.0'
                then 'Windows Server 2016'
            when [operating system] like 'Windows Phone%' then 'Windows Phone'
            when [operating system] like 'Windows 10%'
                OR [operating system]='Microsoft Windows NT Workstation 10.0'
                OR [operating system]='Microsoft Windows NT Workstation 10.0 (Tablet Edition)'
                OR [operating system]='Microsoft Windows NT Server 10.0'
                then 'Windows 10'
            when  [operating system]='Microsoft Windows NT Advanced Server 10.0'
                OR [operating system] like 'Windows Server 2016%'
                then 'Windows Server 2016'
            when [operating system]='Microsoft Windows NT Server 6.0'
                OR [operating system]='Microsoft Windows NT Workstation 6.0'
                OR [operating system]='Microsoft Windows NT Workstation 6.0 (Tablet Edition)'
                then 'Windows Vista'
            when [operating system]='Microsoft Windows NT Workstation 5.1'
                OR [operating system]='Microsoft Windows NT Server 5.2'
                then 'Windows XP'
            else [operating system]
      end [operating system name]
      ,[operating system] [operating system long name]
      ,manufacturer
      ,model
      ,[platform]
      ,[physical memory]
  FROM pbist_sccm.computer c WHERE c.[deleted date] IS NULL
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
            [name]                 AS [name],
            [value]                AS [value]
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
