SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;

CREATE TABLE [vs].[Configuration](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[configuration_group] [varchar](150) NOT NULL,
	[configuration_subgroup] [varchar](150) NOT NULL,
	[name] [varchar](150) NOT NULL,
	[value] [varchar](max) NULL,
	[visible] [bit] NOT NULL
)
go

ALTER TABLE [vs].[configuration] ADD  DEFAULT ((0)) FOR [visible]
go

CREATE TABLE [vs].[Iteration](
	[IterationId] [int] IDENTITY(1,1) NOT NULL,
	[NativeId] [varchar](255) NULL,
	[Name] [varchar](255) NULL,
	[Path] [varchar](255) NULL,
	[StartDate] [varchar](255) NULL,
	[FinishDate] [varchar](255) NULL,
	[url] [varchar](500) NULL,
	[ETLImportDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[IterationId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)
go

CREATE TABLE [vs].[Person](
	[PersonId] [int] IDENTITY(1,1) NOT NULL,
	[NativeId] [int] NOT NULL,
	[UserName] [varchar](255) NOT NULL,
	[UserEmail] [varchar](255) NOT NULL,
	[Name_Email] [varchar](500) NOT NULL,
	[ETLImportDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[PersonId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)
go

CREATE TABLE [vs].[Project](
	[ProjectId] [int] IDENTITY(1,1) NOT NULL,
	[NativeId] [varchar](255) NULL,
	[Name] [varchar](255) NULL,
	[Description] [varchar](4000) NULL,
	[url] [varchar](255) NULL,
	[State] [varchar](255) NULL,
	[Revision] [varchar](255) NULL,
	[Visibility] [varchar](255) NULL,
	[ETLImportDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[ProjectId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)
go

CREATE TABLE [vs].[stg_Iteration](
	[StageId] [int] IDENTITY(1,1) NOT NULL,
	[id] [varchar](255) NULL,
	[name] [varchar](255) NULL,
	[path] [varchar](255) NULL,
	[startDate] [varchar](255) NULL,
	[finishDate] [varchar](255) NULL,
	[url] [varchar](255) NULL,
	[ETLImportDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[StageId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)
go

CREATE TABLE [vs].[stg_Project](
	[StageId] [int] IDENTITY(1,1) NOT NULL,
	[id] [varchar](255) NULL,
	[name] [varchar](255) NULL,
	[description] [varchar](4000) NULL,
	[url] [varchar](255) NULL,
	[state] [varchar](255) NULL,
	[revision] [varchar](255) NULL,
	[visibility] [varchar](255) NULL,
	[ETLImportDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[StageId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)
go

CREATE TABLE [vs].[stg_WorkItem](
	[StageId] [int] IDENTITY(1,1) NOT NULL,
	[Id] [int] NOT NULL,
	[AreaId] [int] NOT NULL,
	[AreaPath] [varchar](255) NULL,
	[TeamProject] [varchar](255) NULL,
	[NodeName] [varchar](255) NULL,
	[AreaLevel1] [varchar](255) NULL,
	[Rev] [int] NULL,
	[AuthorizedDate] [datetime] NULL,
	[RevisedDate] [datetime] NULL,
	[IterationId] [int] NULL,
	[IterationPath] [varchar](255) NULL,
	[IterationLevel1] [varchar](255) NULL,
	[IterationLevel2] [varchar](255) NULL,
	[WorkItemType] [varchar](255) NULL,
	[State] [varchar](255) NULL,
	[Reason] [varchar](255) NULL,
	[AssignedTo] [varchar](255) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [varchar](255) NULL,
	[ChangedDate] [datetime] NULL,
	[ChangedBy] [varchar](255) NULL,
	[AuthorizedAs] [varchar](255) NULL,
	[PersonId] [int] NULL,
	[Watermark] [int] NULL,
	[AttachedFileCount] [int] NULL,
	[HyperLinkCount] [int] NULL,
	[ExternalLinkCount] [int] NULL,
	[RelatedLinkCount] [int] NULL,
	[Title] [varchar](1000) NULL,
	[BoardColumnDone] [bit] NULL,
	[ActivatedDate] [datetime] NULL,
	[ActivatedBy] [varchar](255) NULL,
	[ResolvedDate] [datetime] NULL,
	[ResolvedBy] [varchar](255) NULL,
	[Priority] [int] NULL,
	[StackRank] [float] NULL,
	[ValueArea] [varchar](255) NULL,
	[BoardColumn] [varchar](255) NULL,
	[StateChangeDate] [datetime] NULL,
	[StoryPoints] [float] NULL,
	[ClosedDate] [datetime] NULL,
	[ClosedBy] [varchar](255) NULL,
	[Risk] [varchar](255) NULL,
	[StartDate] [datetime] NULL,
	[DueDate] [datetime] NULL,
	[Effort] [float] NULL,
	[Activity] [varchar](255) NULL,
	[RemainingWork] [float] NULL,
	[OriginalEstimate] [float] NULL,
	[CompletedWork] [float] NULL,
	[Severity] [varchar](255) NULL,
	[FinishDate] [datetime] NULL,
	[BacklogPriority] [float] NULL,
	[ETLImportDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[StageId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)
go

CREATE TABLE [vs].[WorkItemRevision](
	[WorkOrderRevisionId] [int] IDENTITY(1,1) NOT NULL,
	[NativeId] [int] NOT NULL,
	[AreaId] [int] NOT NULL,
	[AreaPath] [varchar](255) NULL,
	[TeamProject] [varchar](255) NULL,
	[NodeName] [varchar](255) NULL,
	[AreaLevel1] [varchar](255) NULL,
	[RevisionNumber] [int] NULL,
	[AuthorizedDate] [datetime] NULL,
	[RevisedDate] [datetime] NULL,
	[IterationId] [int] NULL,
	[IterationPath] [varchar](255) NULL,
	[IterationLevel1] [varchar](255) NULL,
	[IterationLevel2] [varchar](255) NULL,
	[WorkItemType] [varchar](255) NULL,
	[State] [varchar](255) NULL,
	[Reason] [varchar](255) NULL,
	[AssignedTo] [varchar](255) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [varchar](255) NULL,
	[ChangedDate] [datetime] NULL,
	[ChangedBy] [varchar](255) NULL,
	[AuthorizedAs] [varchar](255) NULL,
	[NativeAuthorizedAsPersonId] [int] NULL,
	[Watermark] [int] NULL,
	[AttachedFileCount] [int] NULL,
	[HyperLinkCount] [int] NULL,
	[ExternalLinkCount] [int] NULL,
	[RelatedLinkCount] [int] NULL,
	[Title] [varchar](1000) NULL,
	[BoardColumnDone] [bit] NULL,
	[ActivatedDate] [datetime] NULL,
	[ActivatedBy] [varchar](255) NULL,
	[ResolvedDate] [datetime] NULL,
	[ResolvedBy] [varchar](255) NULL,
	[Priority] [int] NULL,
	[StackRank] [float] NULL,
	[ValueArea] [varchar](255) NULL,
	[BoardColumn] [varchar](255) NULL,
	[StateChangeDate] [varchar](255) NULL,
	[StoryPoints] [float] NULL,
	[ClosedDate] [datetime] NULL,
	[ClosedBy] [varchar](255) NULL,
	[Risk] [varchar](255) NULL,
	[StartDate] [datetime] NULL,
	[DueDate] [datetime] NULL,
	[Effort] [float] NULL,
	[Activity] [varchar](255) NULL,
	[RemainingWork] [float] NULL,
	[OriginalEstimate] [float] NULL,
	[CompletedWork] [float] NULL,
	[Severity] [varchar](255) NULL,
	[FinishDate] [datetime] NULL,
	[BacklogPriority] [float] NULL,
	[ETLImportDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[WorkOrderRevisionId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)
go

CREATE TABLE [vs].[Date](
	[date_key] [int] NOT NULL,
	[full_date] [date] NOT NULL,
	[day_of_week] [tinyint] NOT NULL,
	[day_num_in_month] [tinyint] NOT NULL,
	[day_name] [nvarchar](50) NOT NULL,
	[day_abbrev] [nvarchar](10) NOT NULL,
	[weekday_flag] [char](1) NOT NULL,
	[week_num_in_year] [tinyint] NOT NULL,
	[week_begin_date] [date] NOT NULL,
	[month] [tinyint] NOT NULL,
	[month_name] [nvarchar](50) NOT NULL,
	[month_abbrev] [nvarchar](10) NOT NULL,
	[quarter] [tinyint] NOT NULL,
	[year] [smallint] NOT NULL,
	[yearmo] [int] NOT NULL,
	[same_day_year_ago_date] [date] NOT NULL,
 CONSTRAINT [pk_dim_date] PRIMARY KEY CLUSTERED 
(
	[date_key] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)
go

ALTER TABLE [vs].[Iteration] ADD  DEFAULT (getdate()) FOR [ETLImportDate]
go

ALTER TABLE [vs].[Person] ADD  DEFAULT (getdate()) FOR [ETLImportDate]
go

ALTER TABLE [vs].[Project] ADD  DEFAULT (getdate()) FOR [ETLImportDate]
go

ALTER TABLE [vs].[stg_Iteration] ADD  DEFAULT (getdate()) FOR [ETLImportDate]
go

ALTER TABLE [vs].[stg_Project] ADD  DEFAULT (getdate()) FOR [ETLImportDate]
go

ALTER TABLE [vs].[stg_WorkItem] ADD  DEFAULT (getdate()) FOR [ETLImportDate]
go

CREATE NONCLUSTERED INDEX [IDX_NC_Iteration_StartDate_i_FinishDate] ON [vs].[WorkItemRevision]
(
	[StartDate] DESC
) INCLUDE ([FinishDate])
go

CREATE NONCLUSTERED INDEX [IDX_NC_WIR_CreateDate] ON [vs].[WorkItemRevision]
(
	[CreatedDate] DESC
)
go

CREATE NONCLUSTERED INDEX [IDX_NC_WIR_NativeId] ON [vs].[WorkItemRevision]
(
	[NativeId] DESC
)
go

CREATE NONCLUSTERED INDEX [IDX_NC_WIR_AssignedTo] ON [vs].[WorkItemRevision]
(
	[AssignedTo] DESC
)
go

CREATE NONCLUSTERED INDEX [IDX_NC_WIR_ChangedDate] ON [vs].[WorkItemRevision]
(
	[ChangedDate] DESC
)
go

CREATE NONCLUSTERED INDEX [IDX_NC_WIR_ImportDate] ON [vs].[WorkItemRevision]
(
	[ETLImportDate] DESC
)
go

CREATE NONCLUSTERED INDEX [IDX_NC_Iteration_NativeId] ON [vs].[Iteration]
(
	[NativeId] DESC
)
go

CREATE NONCLUSTERED INDEX [IDX_NC_Person_NativeId] ON [vs].[Person]
(
	[NativeId] DESC
)
go

CREATE NONCLUSTERED INDEX [IDX_NC_Project_NativeId] ON [vs].[Project]
(
	[NativeId] DESC
)
go

CREATE NONCLUSTERED INDEX [IDX_NC_stg_WI_Id] ON [vs].[stg_WorkItem]
(
	[Id] DESC
)
go

CREATE NONCLUSTERED INDEX [IDX_NC_stg_Proj_Id] ON [vs].[stg_Project]
(
	[Id] DESC
)
go

CREATE NONCLUSTERED INDEX [IDX_NC_stg_Iteration_Id] ON [vs].[stg_Iteration]
(
	[Id] DESC
)
go
