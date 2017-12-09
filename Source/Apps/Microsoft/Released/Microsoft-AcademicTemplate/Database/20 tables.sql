SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;



CREATE TABLE [ak].[Configuration](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[configuration_group] [varchar](150) NOT NULL,
	[configuration_subgroup] [varchar](150) NOT NULL,
	[name] [varchar](150) NOT NULL,
	[value] [varchar](max) NULL,
	[visible] [bit] NOT NULL
)

CREATE TABLE ak.[Date](
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

ALTER TABLE [ak].[configuration] ADD  DEFAULT ((0)) FOR [visible]


-- Stage Tables
Create Table ak.PaperStaging
(
	PId bigint,
	Title nvarchar(511),
	CId bigint,
	JId bigint,
	StaticRank float,
	CitationCount bigint,
	Year int,
	Date datetime
);

CREATE NONCLUSTERED INDEX [key_PaperStaging] ON ak.PaperStaging
(
	PId ASC
)
GO

Create Table ak.AuthorStaging
(
	AuId bigint,
	AuthorName nvarchar(255),
	AuthorDisplayName nvarchar(max)
);


CREATE NONCLUSTERED INDEX [key_AuthorStaging] ON ak.AuthorStaging
(
	AuId ASC
)
GO


Create Table ak.JournalStaging
(
	JId bigint,
	JournalName nvarchar(255),
	JournalDisplayName nvarchar(255)
);

CREATE NONCLUSTERED INDEX [key_JournalStaging] ON ak.JournalStaging
(
	JId ASC
)
GO


Create Table ak.FieldsOfStudyStaging
(
	FId bigint,
	FieldLevel int,
	FieldName nvarchar(255),
	FieldDisplayName nvarchar(255)
);

CREATE NONCLUSTERED INDEX [key_FieldsOfStudyStaging] ON ak.FieldsOfStudyStaging
(
	FId ASC
)
GO

Create Table ak.ConferenceStaging
(
	CId bigint,
	ConferenceName nvarchar(255),
	ConferenceDisplayName nvarchar(255)
);

CREATE NONCLUSTERED INDEX [key_ConferenceStaging] ON ak.ConferenceStaging
(
	CId ASC
)
GO

Create Table ak.AffiliationStaging
(
	AfId bigint,
	AffiliationName nvarchar(255),
	AffiliationDisplayName nvarchar(255)
);

CREATE NONCLUSTERED INDEX [key_AffiliationStaging] ON ak.AffiliationStaging
(
	AfId ASC
)
GO

Create Table ak.PaperAuthorAffiliationRelationshipStaging
(
	PId bigint,
	AuId bigint,
	AfId bigint,
	AuS int
);

CREATE NONCLUSTERED INDEX [key_PaperAuthorAffiliationRelationshipStaging] ON ak.PaperAuthorAffiliationRelationshipStaging
(
	PId ASC
)
GO

CREATE NONCLUSTERED INDEX [keyi_PaperAuthorAffiliationRelationshipStaging] ON ak.PaperAuthorAffiliationRelationshipStaging
(
	AuId ASC
)
GO

CREATE NONCLUSTERED INDEX [keyii_PaperAuthorAffiliationRelationshipStaging] ON ak.PaperAuthorAffiliationRelationshipStaging
(
	AfId ASC
)
GO

Create Table ak.PaperFieldsOfStudyRelationshipStaging
(
	PId bigint,
	FId bigint
);

CREATE NONCLUSTERED INDEX [key_PaperFieldsOfStudyRelationshipStaging] ON ak.PaperFieldsOfStudyRelationshipStaging
(
	PId ASC
)
GO

CREATE NONCLUSTERED INDEX [keyi_PaperFieldsOfStudyRelationshipStaging] ON ak.PaperFieldsOfStudyRelationshipStaging
(
	FId ASC
)
GO

Create Table ak.PaperCitationRelationshipStaging
(
	PId bigint,
	RPId bigint
)

CREATE NONCLUSTERED INDEX [key_PaperCitationRelationshipStaging] ON ak.PaperCitationRelationshipStaging
(
	PId ASC
)
GO

CREATE NONCLUSTERED INDEX [keyi_PaperCitationRelationshipStaging] ON ak.PaperCitationRelationshipStaging
(
	RPId ASC
)
GO

-- Prod Tables
Create Table ak.Paper
(
	PId bigint,
	Title nvarchar(511),
	CId bigint,
	JId bigint,
	StaticRank float,
	CitationCount bigint,
	Year int,
	Date datetime
);

CREATE NONCLUSTERED INDEX [key_Paper] ON ak.Paper
(
	PId ASC
)
GO

Create Table ak.Author
(
	AuId bigint,
	AuthorName nvarchar(255),
	AuthorDisplayName nvarchar(max)
);


CREATE NONCLUSTERED INDEX [key_Author] ON ak.Author
(
	AuId ASC
)
GO


Create Table ak.Journal
(
	JId bigint,
	JournalName nvarchar(255),
	JournalDisplayName nvarchar(255)
);

CREATE NONCLUSTERED INDEX [key_Journal] ON ak.Journal
(
	JId ASC
)
GO


Create Table ak.FieldsOfStudy
(
	FId bigint,
	FieldLevel int,
	FieldName nvarchar(255),
	FieldDisplayName nvarchar(255)
);

CREATE NONCLUSTERED INDEX [key_FieldsOfStudy] ON ak.FieldsOfStudy
(
	FId ASC
)
GO

Create Table ak.Conference
(
	CId bigint,
	ConferenceName nvarchar(255),
	ConferenceDisplayName nvarchar(255)
);

CREATE NONCLUSTERED INDEX [key_Conference] ON ak.Conference
(
	CId ASC
)
GO

Create Table ak.Affiliation
(
	AfId bigint,
	AffiliationName nvarchar(255),
	AffiliationDisplayName nvarchar(255)
);

CREATE NONCLUSTERED INDEX [key_Affiliation] ON ak.Affiliation
(
	AfId ASC
)
GO

Create Table ak.PaperAuthorAffiliationRelationship
(
	PId bigint,
	AuId bigint,
	AfId bigint,
	AuS int
);

CREATE NONCLUSTERED INDEX [key_PaperAuthorAffiliationRelationship] ON ak.PaperAuthorAffiliationRelationship
(
	PId ASC
)
GO

CREATE NONCLUSTERED INDEX [keyi_PaperAuthorAffiliationRelationship] ON ak.PaperAuthorAffiliationRelationship
(
	AuId ASC
)
GO

CREATE NONCLUSTERED INDEX [keyii_PaperAuthorAffiliationRelationship] ON ak.PaperAuthorAffiliationRelationship
(
	AfId ASC
)
GO

Create Table ak.PaperFieldsOfStudyRelationship
(
	PId bigint,
	FId bigint
);

CREATE NONCLUSTERED INDEX [key_PaperFieldsOfStudyRelationship] ON ak.PaperFieldsOfStudyRelationship
(
	PId ASC
)
GO

CREATE NONCLUSTERED INDEX [keyi_PaperFieldsOfStudyRelationship] ON ak.PaperFieldsOfStudyRelationship
(
	FId ASC
)
GO

Create Table ak.PaperCitationRelationship
(
	PId bigint,
	RPId bigint
)

CREATE NONCLUSTERED INDEX [key_PaperCitationRelationship] ON ak.PaperCitationRelationship
(
	PId ASC
)
GO

CREATE NONCLUSTERED INDEX [keyi_PaperCitationRelationship] ON ak.PaperCitationRelationship
(
	RPId ASC
)
GO
