SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;

CREATE TABLE [fb].[HashTags](
	[Id] [nvarchar](50) NULL,
	[HashTags] [nvarchar](100) NULL
)

CREATE TABLE [fb].[KeyPhrase](
	[Id] [nvarchar](50) NULL,
	[KeyPhrase] [nvarchar](100) NULL
)

CREATE TABLE [fb].[Sentiment](
	[Id] [nvarchar](50) NULL,
	[Sentiment] [float] NULL
)


CREATE TABLE [fb].[Reactions](
	[Id] [nvarchar](50) NULL,
	[Reaction Type] [nvarchar](30) NULL,
	[Count] [bigint] NULL,
)

CREATE TABLE [fb].[Comments](
	[Id] [nvarchar](50) NULL,
	[Created Date] [datetime] NULL,
	[Message] [nvarchar](max) NULL,
	[From Id] [nvarchar](50) NULL,
	[From Name] [nvarchar](100) NULL,
	[Post Id] [nvarchar](50) NULL,
	[Page] [nvarchar](100) NULL,
    [PageDisplayName] [nvarchar](200) NULL,
    [PageId] [nvarchar](50) NULL
)

CREATE TABLE [fb].[Posts](
	[Id] [nvarchar](50) NULL,
	[Created Date] [datetime] NULL,
	[Message] [nvarchar](max) NULL,
	[From Id] [nvarchar](50) NULL,
	[From Name] [nvarchar](200) NULL,
	[Media] [nvarchar](1000) NULL,
    [Page] [nvarchar](100) NULL,
    [PageDisplayName] [nvarchar](200) NULL,
    [PageId] [nvarchar](50) NULL,
	[Total Comments] [int] NULL
)

CREATE TABLE [fb].[Configuration](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[configuration_group] [varchar](150) NOT NULL,
	[configuration_subgroup] [varchar](150) NOT NULL,
	[name] [varchar](150) NOT NULL,
	[value] [varchar](max) NULL,
	[visible] [bit] NOT NULL
)

CREATE TABLE [fb].[Date](
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

CREATE TABLE [fb].[StagingComments](
	[Id] [nvarchar](50) NULL,
	[Created Date] [datetime] NULL,
	[Message] [nvarchar](max) NULL,
	[From Id] [nvarchar](50) NULL,
	[From Name] [nvarchar](100) NULL,
	[Post Id] [nvarchar](50) NULL,
    [Page] [nvarchar](100) NULL,
    [PageDisplayName] [nvarchar](200) NULL,
    [PageId] [nvarchar](50) NULL
)

CREATE TABLE [fb].[StagingError](
	[Date] [datetime] NULL,
	[Error] [nvarchar](max) NULL,
	[Posts] [nvarchar](max) NULL
)

CREATE TABLE [fb].[StagingHashTags](
	[Id] [nvarchar](50) NULL,
	[HashTags] [nvarchar](100) NULL
)

CREATE TABLE [fb].[StagingKeyPhrase](
	[Id] [nvarchar](50) NULL,
	[KeyPhrase] [nvarchar](100) NULL
)

CREATE TABLE [fb].[StagingPosts](
	[Id] [nvarchar](50) NULL,
	[Created Date] [datetime] NULL,
	[Message] [nvarchar](max) NULL,
	[From Id] [nvarchar](50) NULL,
	[From Name] [nvarchar](200) NULL,
	[Media] [nvarchar](1000) NULL,
	[Page] [nvarchar](100) NULL,
    [PageDisplayName] [nvarchar](200) NULL,
    [PageId] [nvarchar](50) NULL,
	[Total Comments] [int] NULL,
)

CREATE TABLE [fb].[StagingReactions](
	[Id] [nvarchar](50) NULL,
	[Reaction Type] [nvarchar](30) NULL,
	[Count] [bigint] NULL,
)


CREATE TABLE [fb].[StagingSentiment](
	[Id] [nvarchar](50) NULL,
	[Sentiment] [float] NULL
)

GO

CREATE TABLE [fb].[Users](
	[Id] [bigint] NULL,
	[Name] [nvarchar](100) NULL,
)

CREATE TABLE [fb].[Edges](
	[SourceVertex] [bigint] NULL,
    [TargetVertex] [bigint] NULL,
    [EdgeWeight] [int] NULL,
	[PageId] [nvarchar](50) NULL,
)
GO

CREATE NONCLUSTERED INDEX [SourceVertedId] ON [fb].[Edges]
(
	[SourceVertex] ASC
)

CREATE NONCLUSTERED INDEX [UserId] ON [fb].[Users]
(
	[Id] ASC
)

CREATE NONCLUSTERED INDEX [StagingCommentsId1] ON [fb].[Comments]
(
	[Id] ASC
)
GO

CREATE NONCLUSTERED INDEX [StagingCommentsPid1] ON [fb].[Comments]
(
	[Post Id] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_HashTags] ON [fb].[HashTags]
(
	[Id] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_KeyPhrase] ON [fb].[KeyPhrase]
(
	[Id] ASC
)
GO


CREATE NONCLUSTERED INDEX [PostsDate] ON [fb].[Posts]
(
	[Created Date] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


CREATE NONCLUSTERED INDEX [PostsId1] ON [fb].[Posts]
(
	[Id] ASC
)
GO


CREATE NONCLUSTERED INDEX [IX_Reactions] ON [fb].[Reactions]
(
	[Id] ASC
)
GO


CREATE NONCLUSTERED INDEX [IX_Sentiment] ON [fb].[Sentiment]
(
	[Id] ASC
)
GO

CREATE NONCLUSTERED INDEX [StagingCommentsId1] ON [fb].[StagingComments]
(
	[Id] ASC
)
GO


CREATE NONCLUSTERED INDEX [StagingCommentsPid1] ON [fb].[StagingComments]
(
	[Post Id] ASC
)
GO


CREATE NONCLUSTERED INDEX [IX_StagingHashTags] ON [fb].[StagingHashTags]
(
	[Id] ASC
)
GO


CREATE NONCLUSTERED INDEX [IX_StagingKeyPhrase] ON [fb].[StagingKeyPhrase]
(
	[Id] ASC
)
GO

CREATE NONCLUSTERED INDEX [StagingPostsDate] ON [fb].[StagingPosts]
(
	[Created Date] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


CREATE NONCLUSTERED INDEX [StagingPostsId1] ON [fb].[StagingPosts]
(
	[Id] ASC
)
GO


CREATE NONCLUSTERED INDEX [IX_StagingReactions] ON [fb].[StagingReactions]
(
	[Id] ASC
)
GO


CREATE NONCLUSTERED INDEX [IX_StagingSentiment] ON [fb].[StagingSentiment]
(
	[Id] ASC
)
GO


CREATE NONCLUSTERED INDEX [UserId1] ON [fb].[StagingComments]
(
	[From Id] ASC
)
GO

CREATE NONCLUSTERED INDEX [UserId1] ON [fb].[StagingPosts]
(
	[From Id] ASC
)

CREATE NONCLUSTERED INDEX [UserId1] ON [fb].[Comments]
(
	[From Id] ASC
)
GO

CREATE NONCLUSTERED INDEX [UserId1] ON [fb].[Posts]
(
	[From Id] ASC
)


ALTER TABLE [fb].[configuration] ADD  DEFAULT ((0)) FOR [visible]

