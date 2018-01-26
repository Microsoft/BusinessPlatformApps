SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;

CREATE TABLE [adtlog].[20_PowerBI](
	[BatchId] [varchar](8000) NULL,
	[Id] [varchar](900) NOT NULL,
	[RecordType] [varchar](8000) NULL,
	[CreationTime] [varchar](8000) NULL,
	[Operation] [varchar](8000) NULL,
	[OrganizationId] [varchar](8000) NULL,
	[UserType] [varchar](8000) NULL,
	[UserKey] [varchar](8000) NULL,
	[Workload] [varchar](8000) NULL,
	[UserId] [varchar](8000) NULL,
	[ClientIP] [varchar](8000) NULL,
	[UserAgent] [varchar](8000) NULL,
	[Activity] [varchar](8000) NULL,
	[ItemName] [varchar](8000) NULL,
	[WorkSpaceName] [varchar](8000) NULL,
	[DashboardName] [varchar](8000) NULL,
	[WorkspaceId] [varchar](8000) NULL,
	[ObjectId] [varchar](8000) NULL,
	[DashboardId] [varchar](8000) NULL,
	[DatasetName] [varchar](8000) NULL,
	[ReportName] [varchar](8000) NULL,
	[DatasetId] [varchar](8000) NULL,
	[ReportId] [varchar](8000) NULL
)

CREATE TABLE [adtlog].[batch_log](
	[BatchID] [varchar](14) NOT NULL,
	[LogText] [nvarchar](max) NULL,
	[ExecutedBy] [nvarchar](50) NULL,
	[RecordCount] [int] NULL,
	[EventStart] [datetime] NULL,
	[EventEnd] [datetime] NULL,
	[Status] [varchar](50) NULL,
	[BatchResponse] [nvarchar](max) NULL,
	[IndividualResponse] [nvarchar](max) NULL,
	[APIContentUri] [nvarchar](max) NULL,
	[APIStartDate] [datetime] NULL,
	[APIEndDate] [datetime] NULL
)

CREATE TABLE [adtlog].[user](
    [Email] [varchar](max) NULL,
    [Type] [varchar](10) NULL,
    [EntryTime] [datetime] NULL
)
GO

ALTER TABLE [adtlog].[user] ADD  DEFAULT (getdate()) FOR [EntryTime]
GO
