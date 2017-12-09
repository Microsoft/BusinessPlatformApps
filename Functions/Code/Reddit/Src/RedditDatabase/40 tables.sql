-- Documents
CREATE TABLE reddit.Documents 
(
    id reddit.DOC_ID,
    content NVARCHAR(MAX),
	score INT,
	controversiality FLOAT NULL,
    gilded INT,
    author NVARCHAR(100),
    subreddit NVARCHAR(200),
    isComment BIT NULL,

	publishedTimestamp			DATETIME NOT NULL,
    publishedMonthPrecision		DATETIME NOT NULL,
    publishedWeekPrecision		DATETIME NOT NULL,
    publishedDayPrecision		DATETIME NOT NULL,
    publishedHourPrecision		DATETIME NOT NULL,
    publishedMinutePrecision	DATETIME NOT NULL,

	ingestedTimestamp			DATETIME NOT NULL,
	[url] NVARCHAR(2048) NULL,

    CONSTRAINT PK_Documents_PrimaryKey PRIMARY KEY CLUSTERED (id)
);

CREATE INDEX IDX_Documents_isComment ON reddit.Documents (isComment);

-- Sentiment
CREATE TABLE reddit.Sentiment 
(
    documentId reddit.DOC_ID,
    sentiment DECIMAL(18,17),

    CONSTRAINT PK_SentimentPrimaryKey PRIMARY KEY CLUSTERED (documentId)
);

CREATE INDEX IDX_Sentiment_documentId ON reddit.Sentiment (documentId);

-- Sub-Reddits
CREATE TABLE reddit.Posts 
(
    documentId reddit.DOC_ID,
    title NVARCHAR(200),
	mediaPreviewUrl NVARCHAR(2048) NULL,

    CONSTRAINT PK_PostsPrimaryKey PRIMARY KEY CLUSTERED (documentId)
);

-- Comments
CREATE TABLE reddit.Comments 
(
    documentId reddit.DOC_ID,
    parentId reddit.DOC_ID NOT NULL,
	postId reddit.DOC_ID NOT NULL,
	parentUrl NVARCHAR(2048) NOT NULL,
	postUrl NVARCHAR(2048) NOT NULL,

    CONSTRAINT PK_CommentPrimaryKey PRIMARY KEY CLUSTERED (documentId)
);

CREATE INDEX IDX_Comments_parentId ON reddit.Comments (parentId);

-- Key Phrases
CREATE TABLE reddit.KeyPhrases 
(
    documentId reddit.DOC_ID,
    keyPhrase NVARCHAR(400),

    CONSTRAINT PK_KeyPhrasesPrimaryKey PRIMARY KEY CLUSTERED (documentId, keyPhrase),
);

CREATE INDEX IDX_KeyPhrases_documentId ON reddit.KeyPhrases (documentId);

-- Embedded Links
CREATE TABLE reddit.EmbeddedUrls 
(
	[id] [bigint] IDENTITY(1,1) NOT NULL,
    documentId reddit.DOC_ID,
    embeddedUrl NVARCHAR(500),
    embeddedUrlDomain NVARCHAR(100)

	CONSTRAINT [PK_EmbeddedUrls] PRIMARY KEY CLUSTERED ([id] ASC) WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
);

CREATE INDEX IDX_EmbeddedUrls_documentId ON reddit.EmbeddedUrls (documentId);

-- Entities
CREATE TABLE reddit.Entities 
(
	[id] [bigint] IDENTITY(1,1) NOT NULL,
    documentId reddit.DOC_ID,
    entity NVARCHAR(500),
    entityType NVARCHAR(20),
    entityOffset INT,
    entityLength INT,

	CONSTRAINT [PK_Entities] PRIMARY KEY CLUSTERED ([id] ASC) WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
);

CREATE INDEX IDX_Entities_documentId ON reddit.Entities (documentId);

CREATE TABLE reddit.UserDefinedEntities 
(
	[id] [bigint] IDENTITY(1,1) NOT NULL,
    documentId reddit.DOC_ID,
    entity NVARCHAR(500),
    entityType NVARCHAR(20),
    entityOffset INT,
    entityLength INT,

	CONSTRAINT [PK_UserDefinedEntities] PRIMARY KEY CLUSTERED ([id] ASC) WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
);

CREATE INDEX IDX_UserDefinedEntities_documentId ON reddit.UserDefinedEntities (documentId);

-- User Defined Entities
CREATE TABLE reddit.UserDefinedEntityDefinitions
(
    regex			NVARCHAR(200) NOT NULL,
    entityType		NVARCHAR(30) NOT NULL,
    entityValue		NVARCHAR(MAX) NULL,
	color           NVARCHAR(7) NOT NULL
);

-- Number of comments per reddit post
CREATE TABLE reddit.PostCommentCount (
	postId reddit.DOC_ID,
	commentCount INT,

	CONSTRAINT [PK_PostCommentCount] PRIMARY KEY CLUSTERED ([postId] ASC) WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
);

-- Staging (Sentiment)
CREATE TABLE reddit.Staging_Sentiment 
(
    documentId reddit.DOC_ID,
    sentiment DECIMAL(18,17)
);

-- Staging (Entities)
CREATE TABLE reddit.Staging_Entities 
(
    documentId reddit.DOC_ID,
    entity NVARCHAR(500),
    entityType NVARCHAR(20),
    entityOffset INT,
    entityLength INT
);

-- Staging (Key Phrases)
CREATE TABLE reddit.Staging_KeyPhrases 
(
    documentId reddit.DOC_ID,
    keyPhrase NVARCHAR(400)
);
