SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;

CREATE TABLE [adtlog].[20_PowerBI](
	[dbInsertTimeUTC] [datetime] NOT NULL DEFAULT (getutcdate()),
	[BatchId] [bigint] NULL,
	[Id] [varchar](900) NOT NULL,
	[CreationTime] [datetime] NOT NULL,
	[RecordType] [varchar](8000) NULL,
	[Workload] [varchar](8000) NULL,
	[Activity] [varchar](8000) NULL,
	[Operation] [varchar](8000) NULL,
	[OrganizationId] [varchar](8000) NULL,
	[UserKey] [varchar](8000) NULL,
	[UserId] [varchar](8000) NULL,
	[ClientIP] [varchar](8000) NULL,
	[UserAgent] [varchar](8000) NULL,
	[UserType] [varchar](8000) NULL,
	[ObjectId] [varchar](8000) NULL,
	[ItemName] [varchar](8000) NULL,
	[WorkspaceId] [varchar](8000) NULL,
	[WorkSpaceName] [varchar](8000) NULL,
	[DashboardId] [varchar](8000) NULL,
	[DashboardName] [varchar](8000) NULL,
	[ReportId] [varchar](8000) NULL,
	[ReportName] [varchar](8000) NULL,
	[DatasetId] [varchar](8000) NULL,
	[DatasetName] [varchar](8000) NULL
)

CREATE TABLE [adtlog].[staging_20_PowerBI](
	[dbInsertTimeUTC] [datetime] NOT NULL DEFAULT (getutcdate()),
	[BatchId] [bigint] NULL,
	[Id] [varchar](900) NOT NULL,
	[CreationTime] [datetime] NOT NULL,
	[RecordType] [varchar](8000) NULL,
	[Workload] [varchar](8000) NULL,
	[Activity] [varchar](8000) NULL,
	[Operation] [varchar](8000) NULL,
	[OrganizationId] [varchar](8000) NULL,
	[UserKey] [varchar](8000) NULL,
	[UserId] [varchar](8000) NULL,
	[ClientIP] [varchar](8000) NULL,
	[UserAgent] [varchar](8000) NULL,
	[UserType] [varchar](8000) NULL,
	[ObjectId] [varchar](8000) NULL,
	[ItemName] [varchar](8000) NULL,
	[WorkspaceId] [varchar](8000) NULL,
	[WorkSpaceName] [varchar](8000) NULL,
	[DashboardId] [varchar](8000) NULL,
	[DashboardName] [varchar](8000) NULL,
	[ReportId] [varchar](8000) NULL,
	[ReportName] [varchar](8000) NULL,
	[DatasetId] [varchar](8000) NULL,
	[DatasetName] [varchar](8000) NULL
)

CREATE TABLE [adtlog].[batch_log](
	[dbInsertTimeUTC] [datetime] NOT NULL DEFAULT (getutcdate()),
	[BatchID] [bigint] NOT NULL,
	[RecordCount] [int] NULL,
	[Status] [varchar](50) NULL,
	[ExecutedBy] [nvarchar](50) NULL,
	[EventStart] [datetime] NULL,
	[EventEnd] [datetime] NULL,
	[APIStartDate] [datetime] NULL,
	[APIEndDate] [datetime] NULL,
	[LogText] [varchar](8000) NULL,
	[BatchResponse] [varchar](8000) NULL,
	[IndividualResponse] [varchar](8000) NULL,
	[APIContentUri] [varchar](8000) NULL
)

CREATE TABLE [adtlog].[user](
    [Email] [varchar](8000) NULL,
    [Type] [varchar](100) NULL,
    [EntryTime] [datetime] NOT NULL DEFAULT (getutcdate())
)

CREATE TABLE [adtlog].[trace_log](
	[TraceDate] [datetime] NOT NULL DEFAULT (getutcdate()),
	[BatchID] [bigint] NULL,
	[LogText] [varchar](8000) NULL,
	[ExecutedBy] [varchar](8000) NULL
)
GO
