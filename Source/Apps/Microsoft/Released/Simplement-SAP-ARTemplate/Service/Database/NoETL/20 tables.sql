SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
go

CREATE TABLE [dbo].[ActivityLog]
(
    [ActivityLogId] INT IDENTITY(1,1) NOT NULL,
    [StartTime]     DATETIME NOT NULL,
    [ActivityTime]  DATETIME NOT NULL,
    [Message]       NVARCHAR(500) NOT NULL,
    [MessageType]   TINYINT NOT NULL,
    CONSTRAINT [PK_ActivityLog] PRIMARY KEY CLUSTERED ([ActivityLogId] ASC)
);

CREATE TABLE sap.[configuration]
(
    id                     INT IDENTITY(1, 1) NOT NULL,
    configuration_group    VARCHAR(150) NOT NULL,
    configuration_subgroup VARCHAR(150) NOT NULL,
    name                   VARCHAR(150) NOT NULL,
    [value]                VARCHAR(max) NULL,
    visible                BIT NOT NULL DEFAULT 0
);

CREATE TABLE sap.sap_replicationstatus
(
    id             INT IDENTITY(1, 1) NOT NULL,
    EntityName     NVARCHAR(1024) NULL,
    StartDate      DATETIME2 NULL,
    EndDate        DATETIME2 NULL,
    [Status]       NCHAR(1) NULL,
    StatusDetails  NVARCHAR(500) NULL,
    [TimeStamp]    DATETIME2 DEFAULT SYSDATETIME(),
    CONSTRAINT pk_sap_replicationstatus PRIMARY KEY CLUSTERED (id)
);
go
