SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
go

CREATE TABLE pbist_sccm.computer
(
    machineid          INT NOT NULL,
    sitecode           NVARCHAR(3) NULL,
    name               NVARCHAR(256) NULL,
    [operating system] NVARCHAR(256) NULL,
    [client type]      TINYINT NULL,
    manufacturer       NVARCHAR(255) NULL,
    model              NVARCHAR(255) NULL,
    [platform]         NVARCHAR(255) NULL,
    [physical memory]  BIGINT NULL,
    [deleted date]     DATETIME NULL
);

CREATE TABLE pbist_sccm.computermalware
(
     threatid                     BIGINT NULL,
     machineid                    INT NOT NULL,
     [detection date]             DATETIME NULL,
     [observer product name]      NVARCHAR(32) NOT NULL,
     [observer product version]   NVARCHAR(255) NULL,
     [observer detection]         NVARCHAR(8) NULL,
     [remediation type]           NVARCHAR(11) NULL,
     [remediation result]         NVARCHAR(5) NOT NULL,
     [remediation error code]     INT NULL,
     [remediation pending action] NVARCHAR(16) NOT NULL,
     [is active malware]          NVARCHAR(5) NOT NULL
);

CREATE TABLE pbist_sccm.malware
(
    threatid           BIGINT NOT NULL,
    [malware name]     NVARCHAR(128) NOT NULL,
    [malware severity] NVARCHAR(128) NOT NULL,
    [malware category] NVARCHAR(128) NOT NULL
);

CREATE  TABLE  pbist_sccm.computerprogram
(
    machineid       INT NOT NULL,
    [program name]  NVARCHAR(max)  NOT NULL,
    publisher       NVARCHAR(max)  NULL,
    [version]       NVARCHAR(max)  NULL,
    [timestamp]     DATETIME NULL
);

CREATE  TABLE  pbist_sccm.computerupdate
(
    machineid             INT  NOT  NULL,
    ci_id                 INT  NOT  NULL,
    laststatuschangetime  DATETIME  NULL,
    laststatuschecktime   DATETIME  NULL,
    [status]              TINYINT  NOT  NULL
);


CREATE  TABLE  pbist_sccm.operatingsystem
( 
    osid                 INT IDENTITY(1,  1)  NOT  NULL, 
    [operating  system]  NVARCHAR(256)  NULL 
);

CREATE  TABLE  pbist_sccm.program
(
      [program name]  NVARCHAR(max)  NOT NULL,
      publisher       NVARCHAR(max)  NULL,
      [version]       NVARCHAR(max)  NULL,
);

CREATE  TABLE  pbist_sccm.scanhistory 
( 
        machineid                         INT  NOT  NULL, 
        date_key                          INT  NOT  NULL, 
        sitecode                          NVARCHAR(3)  NULL, 
        [client type]                     SMALLINT  NULL, 
        [enabled]                         INT  NULL, 
        [client version]                  NVARCHAR(255)  NULL, 
        [real time protection enabled]    BIT  NULL, 
        [on access protection enabled]    BIT  NULL, 
        [input/output protection enabled] BIT  NULL, 
        [behavior monitor enabled]        BIT  NULL, 
        [antivirus enabled]               BIT  NULL, 
        [antispyware enabled]             BIT  NULL, 
        [nis enabled]                     BIT  NULL, 
        [quick scan age (days)]           INT  NULL, 
        [full scan age (days)]            INT  NULL, 
        [signature age (days)]            INT  NULL, 
        [engine version]                  NVARCHAR(255)  NULL, 
        [antivirus signature version]     NVARCHAR(255)  NULL, 
        [missing important update count]  INT  NULL, 
        [missing critical update count]   INT  NULL, 
        [client active status]            BIT  NOT  NULL, 
        [health evaluation result]        SMALLINT  NULL, 
        [health evaluation]               DATETIME  NULL, 
        [last online]                     DATETIME  NULL, 
        [health status message]           DATETIME  NULL, 
        [client state]                    SMALLINT  NULL,
        [last ddr]                        DATETIME  NULL,
        [last hw]                         DATETIME  NULL,
        [last sw]                         DATETIME  NULL,
        [last status message]             DATETIME  NULL,
        [last policy request]             DATETIME  NULL,
        [last scan time]                  DATETIME  NULL,
);

CREATE TABLE pbist_sccm.[site]
( 
      sitecode                                      NVARCHAR(3)  NOT  NULL, 
      [site name]                                   NVARCHAR(200)  NULL, 
      [version]                                     NVARCHAR(32)  NULL, 
      [server name]                                 NVARCHAR(256)  NULL, 
      [availability]                                VARCHAR(9)  NOT  NULL, 
      [location]                                    NVARCHAR(256)  NULL, 
      overalllinkstatus                             NVARCHAR(12)  NULL, 
      [client successfully communicating with mp]   INT  NULL, 
      [client failing to communicate with mp]       INT  NULL, 
      [health check count]                          INT  NULL, 
      [ok health check count]                       INT  NULL, 
      [av definition compliance count]              INT  NULL, 
      [scep count]                                  INT  NULL, 
      [av reach count]                              INT  NULL 
);

CREATE TABLE pbist_sccm.[update]
( 
    ci_id          INT  NOT  NULL, 
    articleid      NVARCHAR(64)  NULL, 
    bulletinid     NVARCHAR(64)  NULL, 
    title          NVARCHAR(513)  NULL, 
    severity       INT  NULL, 
    severityname   NVARCHAR(64)  NULL, 
    infoURL        NVARCHAR(512)  NULL 
);

CREATE  TABLE  pbist_sccm.[user]
( 
    username     NVARCHAR(256)  NULL, 
    [full name]  NVARCHAR(255)  NULL 
);

CREATE  TABLE  pbist_sccm.usercomputer 
( 
    machineid    INT NOT NULL,
    username     NVARCHAR(256)  NULL, 
    [full name]  NVARCHAR(255)  NULL 
);

CREATE  TABLE  pbist_sccm.[collection]
( 
    collectionid        NVARCHAR(8) NOT NULL, 
    [collection name]   NVARCHAR(255) NOT NULL 
);

CREATE  TABLE  pbist_sccm.computercollection
( 
    collectionid  NVARCHAR(8) NOT NULL,
    resourceid    INT NOT NULL
);

CREATE TABLE pbist_sccm.[configuration]
(
  id                     INT IDENTITY(1, 1) NOT NULL,
  configuration_group    VARCHAR(150) NOT NULL,
  configuration_subgroup VARCHAR(150) NOT NULL,
  [name]                 VARCHAR(150) NOT NULL,
  [value]                VARCHAR(max) NULL,
  visible                BIT NOT NULL DEFAULT 0
);


CREATE TABLE pbist_sccm.[date]
(
   date_key               INT NOT NULL,
   full_date              DATE NOT NULL,
   day_of_week            TINYINT NOT NULL,
   day_num_in_month       TINYINT NOT NULL,
   day_name               NVARCHAR(50) NOT NULL,
   day_abbrev             NVARCHAR(10) NOT NULL,
   weekday_flag           CHAR(1) NOT NULL,
   week_num_in_year       TINYINT NOT NULL,
   week_begin_date        DATE NOT NULL,
   [month]                TINYINT NOT NULL,
   month_name             NVARCHAR(50) NOT NULL,
   month_abbrev           NVARCHAR(10) NOT NULL,
   [quarter]              TINYINT NOT NULL,
   [year]                 SMALLINT NOT NULL,
   yearmo                 INT NOT NULL,
   same_day_year_ago_date DATE NOT NULL
   CONSTRAINT pk_dim_date PRIMARY KEY CLUSTERED (date_key)
);
GO

CREATE TABLE pbist_sccm.computermalware_staging
(
    threatid                     BIGINT NULL,
    machineid                    INT NOT NULL,
    [detection date]             DATETIME NULL,
    [observer product name]      NVARCHAR(32) NOT NULL,
    [observer product version]   NVARCHAR(255) NULL,
    [observer detection]         NVARCHAR(8) NULL,
    [remediation type]           NVARCHAR(11) NULL,
    [remediation result]         NVARCHAR(5) NOT NULL,
    [remediation error code]     INT NULL,
    [remediation pending action] NVARCHAR(16) NOT NULL,
    [is active malware]          NVARCHAR(5) NOT NULL
);

CREATE TABLE pbist_sccm.computer_staging
(
    machineid          INT NOT NULL,
    sitecode           NVARCHAR(3) NULL,
    name               NVARCHAR(256) NULL,
    [operating system] NVARCHAR(256) NULL,
    [client type]      TINYINT NULL,
    manufacturer       NVARCHAR(255) NULL,
    model              NVARCHAR(255) NULL,
    [platform]         NVARCHAR(255) NULL,
    [physical memory]  BIGINT NULL
);

CREATE TABLE pbist_sccm.computerprogram_staging
(
    machineid       INT NOT NULL,
    [program name]  NVARCHAR(max) NOT NULL,
    publisher       NVARCHAR(max)  NULL,
    [version]       NVARCHAR(max)  NULL,
    [timestamp]     DATETIME NULL
);

CREATE TABLE pbist_sccm.computerupdate_staging
(
    machineid            INT NOT NULL,
    ci_id                INT NOT NULL,
    laststatuschangetime DATETIME NULL,
    laststatuschecktime  DATETIME NULL,
    [status]             TINYINT NOT NULL
);

CREATE TABLE pbist_sccm.malware_staging
(
    threatid           BIGINT NOT NULL,
    [malware name]     NVARCHAR(128) NOT NULL,
    [malware severity] NVARCHAR(128) NOT NULL,
    [malware category] NVARCHAR(128) NOT NULL
);

CREATE TABLE pbist_sccm.program_staging
(
      [program name]  NVARCHAR(max)  NOT NULL,
      publisher       NVARCHAR(max)  NULL,
      [version]       NVARCHAR(max)  NULL
);


CREATE TABLE pbist_sccm.scanhistory_staging
(
    machineid                         INT NOT NULL,
    date_key                          INT NOT NULL,
    sitecode                          NVARCHAR(3) NULL,
    [client type]                     SMALLINT NULL,
    [enabled]                         INT NULL,
    [client version]                  NVARCHAR(255) NULL,
    [real time protection enabled]    BIT NULL,
    [on access protection enabled]    BIT NULL,
    [input/output protection enabled] BIT NULL,
    [behavior monitor enabled]        BIT NULL,
    [antivirus enabled]               BIT NULL,
    [antispyware enabled]             BIT NULL,
    [nis enabled]                     BIT NULL,
    [quick scan age (days)]           INT NULL,
    [full scan age (days)]            INT NULL,
    [signature age (days)]            INT NULL,
    [engine version]                  NVARCHAR(255) NULL,
    [antivirus signature version]     NVARCHAR(255) NULL,
    [missing important update count]  INT NULL,
    [missing critical update count]   INT NULL,
    [client active status]            BIT NOT NULL,
    [health evaluation result]        SMALLINT NULL,
    [health evaluation]               DATETIME NULL,
    [last online]                     DATETIME NULL,
    [health status message]           DATETIME NULL,
    [client state]                    SMALLINT NULL,
    [last ddr]                        DATETIME NULL,
    [last hw]                         DATETIME NULL,
    [last sw]                         DATETIME NULL,
    [last status message]             DATETIME NULL,
    [last policy request]             DATETIME NULL,
    [last scan time]                  DATETIME NULL
);

CREATE TABLE pbist_sccm.site_staging
(
    sitecode                                    NVARCHAR(3) NOT NULL,
    [site name]                                 NVARCHAR(200) NULL,
    [version]                                   NVARCHAR(32) NULL,
    [server name]                               NVARCHAR(256) NULL,
    [availability]                              NVARCHAR(20) NOT NULL,
    [location]                                  NVARCHAR(256) NULL,
    overalllinkstatus                           NVARCHAR(12) NULL,
    [client successfully communicating with mp] INT NULL,
    [client failing to communicate with mp]     INT NULL,
    [health check count]                        INT NULL,
    [ok health check count]                     INT NULL,
    [av definition compliance count]            INT NULL,
    [scep count]                                INT NULL,
    [av reach count]                            INT NULL
);

CREATE TABLE pbist_sccm.update_staging
(
    ci_id           INT NOT NULL,
    articleid       NVARCHAR(64) NULL,
    bulletinid      NVARCHAR(64) NULL,
    title           NVARCHAR(513) NULL,
    severity        INT NULL,
    [severity name] NVARCHAR(64) NULL,
    infourl         NVARCHAR(512) NULL
);

CREATE TABLE pbist_sccm.user_staging
(
    username     NVARCHAR(256)  NULL, 
    [full name]  NVARCHAR(255)  NULL 
);

CREATE TABLE pbist_sccm.usercomputer_staging
(
    machineid    INT NOT NULL,
    username     NVARCHAR(256)  NULL, 
    [full name]  NVARCHAR(255)  NULL 
);

CREATE  TABLE  pbist_sccm.collection_staging
( 
    collectionid        NVARCHAR(8) NOT NULL, 
    [collection name]   NVARCHAR(255) NOT NULL 
);

CREATE  TABLE  pbist_sccm.computercollection_staging
( 
    collectionid   NVARCHAR(8) NOT NULL,
    resourceid     INT NOT NULL
);


CREATE TABLE pbist_sccm.[entityinitialcount](
    [entityname] [nvarchar](40) NULL,
    [initialcount] INT NULL,
    [lastcount] INT NULL,
    [lasttimestamp] DATETIME2 NULL
);