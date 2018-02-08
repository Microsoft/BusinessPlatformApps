SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
GO


CREATE TABLE [it].[Badges](
	[BadgeID] [int] NOT NULL,
	[BadgeTitle] [nvarchar](120) NOT NULL,
	[BadgeIconUrl] [nvarchar](600) NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_Badges] PRIMARY KEY CLUSTERED 
(
	[BadgeID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [it].[Boards](
	[BoardID] [nvarchar](200) NOT NULL,
	[BoardID_New] [nvarchar](200) NULL,
	[BoardTitle] [nvarchar](1500) NOT NULL,
	[ConversationStyle] [varchar](50) NULL,
	[ParentCategoryID] [nvarchar](200) NULL,
	[IsHidden] [bit] NULL,
	[BoardMessages] [bigint] NOT NULL,
	[BoardTopics] [bigint] NOT NULL,
	[BoardViews] [bigint] NOT NULL,
	[BoardDepth] [smallint] NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_Boards] PRIMARY KEY CLUSTERED 
(
	[BoardID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [it].[Categories](
	[CategoryID] [nvarchar](200) NOT NULL,
	[CategoryID_New] [nvarchar](200) NULL,
	[CategoryTitle] [nvarchar](1500) NOT NULL,
	[IsHidden] [bit] NULL,
	[CategoryMessages] [bigint] NOT NULL,
	[CategoryTopics] [bigint] NOT NULL,
	[CategoryViews] [bigint] NOT NULL,
	[CategoryDepth] [smallint] NOT NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED 
(
	[CategoryID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]



CREATE TABLE [it].[Date](
	[DateID] [int] NOT NULL,
	[Date] [date] NOT NULL,
	[DayNumberOfWeek] [tinyint] NOT NULL,
	[NameOfWeek] [nvarchar](10) NOT NULL,
	[DayNumberOfMonth] [tinyint] NOT NULL,
	[DayNumberOfYear] [smallint] NOT NULL,
	[WeekNumberOfYear] [tinyint] NOT NULL,
	[MonthName] [nvarchar](10) NOT NULL,
	[MonthNumberOfYear] [tinyint] NOT NULL,
	[Quarter] [tinyint] NOT NULL,
	[Year] [smallint] NOT NULL,
 CONSTRAINT [PK_DateID] PRIMARY KEY CLUSTERED 
(
	[DateID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]



CREATE TABLE [it].[ETLAudit](
	[ETLAuditID] [int] IDENTITY(1,1) NOT NULL,
	[ETLJobName] [varchar](100) NULL,
	[TableName] [varchar](60) NULL,
	[RowCount] [int] NULL,
	[TableLastUpdated] [datetime] NULL,
	[ETLAuditDesc] [varchar](800) NULL,
	[ModifiedBy] [varchar](250) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_ETLAuditID] PRIMARY KEY CLUSTERED 
(
	[ETLAuditID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]



CREATE TABLE [it].[Kudos](
	[KudoID] [int] NOT NULL,
	[MessageID] [int] NOT NULL,
	[KudoDate] [date] NOT NULL,
	[KudoUserID] [int] NOT NULL,
	[KudoWeight] [smallint] NOT NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_Kudos] PRIMARY KEY CLUSTERED 
(
	[KudoID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]



CREATE TABLE [it].[Messages](
	[MessageKey] [int] IDENTITY(1,1) NOT NULL,
	[MessageID] [int] NOT NULL,
	[UserID] [int] NOT NULL,
	[MessageSubject] [nvarchar](1200) NOT NULL,
	[BoardID] [nvarchar](1000) NULL,
	[Topic] [nvarchar](1000) NULL,
	[ParentMessageID] [int] NULL,
	[PostedDate] [date] NOT NULL,
	[MessageDepth] [smallint] NULL,
	[IsSolution] [bit] NULL,
	[SolutionDate] [datetime] NULL,
	[MessageViews] [bigint] NULL,
	[MessageKudos] [int] NULL,
	[DeviceType] [varchar](40) NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_MessageKey] PRIMARY KEY CLUSTERED 
(
	[MessageKey] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]



CREATE TABLE [it].[Parameters](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ParamName] [varchar](250) NOT NULL,
	[ParamValue] [nvarchar](1500) NOT NULL,
	[ModifiedBy] [varchar](250) NULL,
	[ModifiedDate] [datetime] NULL
) ON [PRIMARY]
GO



CREATE TABLE [it].[References](
	[RefID] [smallint] IDENTITY(1,1) NOT NULL,
	[Page] [varchar](120) NULL,
	[Measure/Visual] [varchar](250) NULL,
	[Description] [varchar](600) NULL,
	[IsActive] [bit] NULL
) ON [PRIMARY]
GO



CREATE TABLE [it].[STG_Boards](
	[id] [nvarchar](200) NULL,
	[conversation_style] [varchar](50) NULL,
	[title] [nvarchar](1500) NULL,
	[parent_category] [nvarchar](100) NULL,
	[hidden] [varchar](10) NULL,
	[messages] [bigint] NULL,
	[topics] [bigint] NULL,
	[views] [bigint] NULL,
	[depth] [int] NULL
) ON [PRIMARY]



CREATE TABLE [it].[STG_Categories](
	[id] [nvarchar](200) NULL,
	[title] [nvarchar](1500) NULL,
	[hidden] [varchar](10) NULL,
	[messages] [bigint] NULL,
	[topics] [bigint] NULL,
	[views] [bigint] NULL,
	[depth] [int] NULL
) ON [PRIMARY]



CREATE TABLE [it].[STG_Kudos](
	[KudoID] [int] NULL,
	[MessageID] [int] NULL,
	[KudoTime] [datetime] NULL,
	[KudoUserID] [int] NULL,
	[KudoWeight] [smallint] NULL
) ON [PRIMARY]



CREATE TABLE [it].[STG_Messages](
	[id] [int] NULL,
	[author] [int] NULL,
	[subject] [nvarchar](1200) NULL,
	[board] [nvarchar](1000) NULL,
	[topic] [nvarchar](1000) NULL,
	[parent] [int] NULL,
	[post_time] [datetime] NULL,
	[depth] [int] NULL,
	[is_solution] [varchar](10) NULL,
	[solution_data] [datetime] NULL,
	[metrics] [bigint] NULL,
	[kudos] [int] NULL,
	[device_id] [varchar](200) NULL
) ON [PRIMARY]



CREATE TABLE [it].[STG_UserBadges](
	[user_id] [int] NULL,
	[badge_id] [int] NULL,
	[title] [nvarchar](120) NULL,
	[icon_url] [nvarchar](600) NULL,
	[awarded] [int] NULL,
	[activation_date] [datetime] NULL,
	[earned_date] [datetime] NULL
) ON [PRIMARY]



CREATE TABLE [it].[STG_Users](
	[id] [int] NULL,
	[login] [nvarchar](120) NULL,
	[deleted] [nvarchar](20) NULL,
	[registration_time] [datetime] NULL,
	[last_visit_time] [datetime] NULL
) ON [PRIMARY]



CREATE TABLE [it].[UserBadges](
	[UserID] [int] NOT NULL,
	[BadgeID] [int] NOT NULL,
	[BadgeActivationDate] [date] NULL,
	[BadgeEarnedDate] [date] NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_UserBadges] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[BadgeID] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]



CREATE TABLE [it].[Users](
	[UserId] [int] NOT NULL,
	[UserName] [nvarchar](100) NOT NULL,
	[Deleted] [bit] NOT NULL,
	[RegistrationDate] [date] NOT NULL,
	[LastVisited] [date] NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
	[Banned] [bit] NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]



-- Defaults on the tables
ALTER TABLE [it].[ETLAudit] ADD  CONSTRAINT [DF_ETLAudit_ModifiedBy]  DEFAULT (SUSER_SNAME()) FOR [ModifiedBy]
GO

ALTER TABLE [it].[ETLAudit] ADD  CONSTRAINT [DF_ETLAudit_ModifiedDate]  DEFAULT (GETDATE()) FOR [ModifiedDate]
GO

ALTER TABLE [it].[Parameters] ADD  CONSTRAINT [DF_Parameters_ModifiedBy]  DEFAULT (SUSER_SNAME()) FOR [ModifiedBy]
GO

ALTER TABLE [it].[Parameters] ADD  CONSTRAINT [DF_Parameters_ModifiedDate]  DEFAULT (GETDATE()) FOR [ModifiedDate]
GO

