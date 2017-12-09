SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;

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
