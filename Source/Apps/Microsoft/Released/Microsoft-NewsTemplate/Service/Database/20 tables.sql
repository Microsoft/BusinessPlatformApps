SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
go


-- Main tables
CREATE TABLE bpst_news.[configuration]
(
  id                     INT IDENTITY(1, 1) NOT NULL,
  configuration_group    NVARCHAR(150) NOT NULL,
  configuration_subgroup NVARCHAR(150) NOT NULL,
  name                   NVARCHAR(150) NOT NULL,
  [value]                NVARCHAR(max) NULL,
  visible                BIT NOT NULL DEFAULT 0
);


CREATE TABLE bpst_news.documents
(
    id				  NCHAR(64) NOT NULL,
    [text]			  NVARCHAR(max),
    textLength        INT,
    cleanedText       NVARCHAR(max),
    cleanedTextLength INT,
	abstract		  NVARCHAR(4000),
    title			  NVARCHAR(2000),
    sourceUrl		  NVARCHAR(2000),
    sourceDomain	  NVARCHAR(1000),
    category		  NVARCHAR(150),
    imageUrl		  NVARCHAR(max),
    imageWidth		  INT,
    imageHeight		  INT,
    CONSTRAINT pk_documents PRIMARY KEY CLUSTERED (id)
);

CREATE TABLE bpst_news.documentsearchterms
(
	[documentId]		NCHAR(64),
    [searchterms]		NVARCHAR(130)
);

CREATE TABLE bpst_news.documentpublishedtimes
(
    id				NCHAR(64) NOT NULL,
    [timestamp]		DATETIME NOT NULL,
    monthPrecision  DATETIME NOT NULL,
    weekPrecision	DATETIME NOT NULL,
    dayPrecision	DATETIME NOT NULL,
    hourPrecision	DATETIME NOT NULL,
    minutePrecision DATETIME NOT NULL,
    CONSTRAINT pk_documentpublishedtimes PRIMARY KEY CLUSTERED (id)
);


CREATE TABLE bpst_news.documentingestedtimes
(
    id				NCHAR(64) NOT NULL,
    [timestamp]		DATETIME NOT NULL,
    monthPrecision  DATETIME NOT NULL,
    weekPrecision	DATETIME NOT NULL,
    dayPrecision	DATETIME NOT NULL,
    hourPrecision	DATETIME NOT NULL,
    minutePrecision DATETIME NOT NULL,
    CONSTRAINT pk_documentingestedtimes PRIMARY KEY CLUSTERED (id)
);


CREATE TABLE bpst_news.documentsentimentscores
(
    id				NCHAR(64) NOT NULL,
    score			FLOAT NOT NULL,
    CONSTRAINT pk_documentsentimentscores PRIMARY KEY CLUSTERED (id)
);



CREATE TABLE bpst_news.documentkeyphrases
(
    documentid	NCHAR(64) NOT NULL,
    phrase		NVARCHAR(max) NOT NULL
);
CREATE NONCLUSTERED INDEX idx_documentkeyphrases_documentid ON [bpst_news].[documentkeyphrases] ([documentid]) INCLUDE ([phrase]) WITH (ONLINE = ON);

CREATE TABLE bpst_news.documenttopics
(
    documentId		 NCHAR(64) NOT NULL,
    topicId			 NCHAR(36) NOT NULL,
    batchId			 NVARCHAR(40) NULL,
    documentDistance FLOAT NOT NULL,
    topicScore		 INT NOT NULL,
    topicKeyPhrase   NVARCHAR(2000) NOT NULL,
    CONSTRAINT pk_documenttopics PRIMARY KEY CLUSTERED (documentId, topicId)
);


CREATE TABLE bpst_news.topickeyphrases
(
    topicId			INT NOT NULL,
    KeyPhrase		NVARCHAR(2000) NOT NULL
);

CREATE TABLE bpst_news.documenttopicimages
(
    topicId		NCHAR(36) NOT NULL,
    imageUrl1	NVARCHAR(MAX),
    imageUrl2	NVARCHAR(MAX),
    imageUrl3	NVARCHAR(MAX),
    imageUrl4	NVARCHAR(MAX),
    CONSTRAINT pk_documenttopicimages PRIMARY KEY CLUSTERED (topicId)
);


CREATE TABLE bpst_news.entities
(
	id							BIGINT NOT NULL IDENTITY (1, 1),
    documentId					NCHAR(64) NOT NULL,
    entityType					NVARCHAR(30) NOT NULL,
    entityValue					NVARCHAR(MAX) NULL,
    offset						INT NOT NULL,
    offsetDocumentPercentage	FLOAT NOT NULL,
    [length]					INT NOT NULL
);

-- Bring Your Own Entity Tables
CREATE TABLE bpst_news.userdefinedentities
(
	id							BIGINT NOT NULL IDENTITY (1, 1),
    documentId					NCHAR(64) NOT NULL,
    entityType					NVARCHAR(30) NOT NULL,
    entityValue					NVARCHAR(MAX) NULL,
    offset						INT NOT NULL,
    offsetDocumentPercentage	FLOAT NOT NULL,
    [length]					INT NOT NULL
);

CREATE TABLE bpst_news.userdefinedentitydefinitions
(
    regex			NVARCHAR(200) NOT NULL,
    entityType		NVARCHAR(30) NOT NULL,
    entityValue		NVARCHAR(MAX) NULL
);

CREATE TABLE bpst_news.typedisplayinformation
(
   entityType		NVARCHAR(30) NOT NULL,
   icon				NVARCHAR(30) NOT NULL,
   color			NVARCHAR(7) NOT NULL
);
CREATE INDEX idx_typedisplayinformation_entityType ON bpst_news.typedisplayinformation (entityType);

-- Staging tables

CREATE TABLE bpst_news.stg_documenttopics
(
    documentId		 NCHAR(64) NOT NULL,
    topicId			 NCHAR(36) NOT NULL,
    batchId			 NVARCHAR(40) NULL,
    documentDistance FLOAT NOT NULL,
    topicScore		 INT NOT NULL,
    topicKeyPhrase   NVARCHAR(2000) NOT NULL
);

CREATE TABLE bpst_news.stg_documentcompressedentities
(
    documentId				NCHAR(64) NOT NULL,
    compressedEntitiesJson	NVARCHAR(max),
    CONSTRAINT pk_documentcompressedentities PRIMARY KEY CLUSTERED (documentId)
);

CREATE TABLE bpst_news.stg_documenttopicimages
(
    topicId		NCHAR(36) NOT NULL,
    imageUrl1	NVARCHAR(MAX),
    imageUrl2	NVARCHAR(MAX),
    imageUrl3	NVARCHAR(MAX),
    imageUrl4	NVARCHAR(MAX)
);


CREATE TABLE bpst_news.stg_entities
(
    documentId					NCHAR(64) NOT NULL,
    entityType					NVARCHAR(30) NOT NULL,
    entityValue					NVARCHAR(MAX) NULL,
    offset						INT NOT NULL,
    offsetDocumentPercentage	FLOAT NOT NULL,
    [length]					INT NOT NULL
);


