-- =============================================
-- Author:      MSR Project Essex
-- Description: Deletes all existing data for a document.
-- =============================================
CREATE OR ALTER PROCEDURE reddit.DeleteDocuments
(
    @DocIds reddit.DocumentIdTable ReadOnly
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

	SET XACT_ABORT ON;
	  
	BEGIN TRANSACTION;

	DELETE FROM reddit.Comments WHERE documentId IN (SELECT id FROM @DocIds);
	DELETE FROM reddit.Posts WHERE documentId IN (SELECT id FROM @DocIds)

	DELETE FROM reddit.Entities WHERE documentId IN (SELECT id FROM @DocIds)
	DELETE FROM reddit.UserDefinedEntities WHERE documentId IN (SELECT id FROM @DocIds)
	DELETE FROM reddit.Sentiment WHERE documentId IN (SELECT id FROM @DocIds)
	DELETE FROM reddit.KeyPhrases WHERE documentId IN (SELECT id FROM @DocIds)

	DELETE FROM reddit.EmbeddedUrls WHERE documentId IN (SELECT id FROM @DocIds)

	DELETE FROM reddit.Documents WHERE id IN (SELECT id FROM @DocIds)

	COMMIT TRANSACTION;
END
GO

-- =============================================
-- Author:      MSR Project Essex
-- Description: Inserts a new reddit comment
-- =============================================
CREATE OR ALTER PROCEDURE reddit.InsertComment
(
	-- General
    @NewDocId reddit.DOC_ID,
	@content NVARCHAR(MAX),
    @score INT,
    @controversiality FLOAT,
    @gilded INT,
    @author NVARCHAR(100) = NULL,
	@subreddit NVARCHAR(200),

	-- Timestamps
	@publishedTimestamp			DATETIME,
    @publishedMonthPrecision	DATETIME,
    @publishedWeekPrecision		DATETIME,
    @publishedDayPrecision		DATETIME,
    @publishedHourPrecision		DATETIME,
    @publishedMinutePrecision	DATETIME,

	@ingestedTimestamp			DATETIME,
	@url NVARCHAR(2048),

	-- Comment-specific
    @parentId reddit.DOC_ID,
	@postId reddit.DOC_ID,
	@parentUrl NVARCHAR(2048),
	@postUrl NVARCHAR(2048)
)
AS
BEGIN
	INSERT INTO reddit.Documents
	(
		id,
		content,
		score,
		controversiality,
		gilded,
		author,
		subreddit,
		isComment,
		publishedTimestamp,
		publishedMonthPrecision,
		publishedWeekPrecision,
		publishedDayPrecision,
		publishedHourPrecision,
		publishedMinutePrecision,
		ingestedTimestamp,
		[url]
	)
	VALUES
	(
		@NewDocId, 
		@content, 
		@score, 
		@controversiality, 
		@gilded, 
		@author, 
		@subreddit, 
		1,	
		@publishedTimestamp,
		@publishedMonthPrecision,
		@publishedWeekPrecision,
		@publishedDayPrecision,
		@publishedHourPrecision,
		@publishedMinutePrecision,
		@ingestedTimestamp,
		@url
	);

	INSERT INTO reddit.Comments
	(
		documentId, 
		parentId,
		postId,
		parentUrl,
		postUrl
	) 
	VALUES 
	(
		@NewDocId,
		@parentId,
		@postId,
		@parentUrl,
		@postUrl
	);
END
GO

-- =============================================
-- Author:      MSR Project Essex
-- Description: Inserts a new subreddit post
-- =============================================
CREATE OR ALTER PROCEDURE reddit.InsertPost
(
	-- General
    @NewDocId reddit.DOC_ID,
	@content NVARCHAR(MAX),
    @score INT,
    @controversiality FLOAT,
    @gilded INT,
    @author NVARCHAR(100) = NULL,
	@subreddit NVARCHAR(200),

	-- Timestamps
	@publishedTimestamp			DATETIME,
    @publishedMonthPrecision	DATETIME,
    @publishedWeekPrecision		DATETIME,
    @publishedDayPrecision		DATETIME,
    @publishedHourPrecision		DATETIME,
    @publishedMinutePrecision	DATETIME,

	@ingestedTimestamp			DATETIME,
	@url NVARCHAR(2048),

	-- Subreddit-specific
    @title NVARCHAR(200),
	@mediaPreviewUrl NVARCHAR(2048)
)
AS
BEGIN
	INSERT INTO reddit.Documents
	(
		id,
		content,
		score,
		controversiality,
		gilded,
		author,
		subreddit,
		isComment,
		publishedTimestamp,
		publishedMonthPrecision,
		publishedWeekPrecision,
		publishedDayPrecision,
		publishedHourPrecision,
		publishedMinutePrecision,
		ingestedTimestamp,
		[url]
	)
	VALUES
	(
		@NewDocId, 
		@content, 
		@score, 
		@controversiality, 
		@gilded, 
		@author, 
		@subreddit, 
		0,	
		@publishedTimestamp,
		@publishedMonthPrecision,
		@publishedWeekPrecision,
		@publishedDayPrecision,
		@publishedHourPrecision,
		@publishedMinutePrecision,
		@ingestedTimestamp,
		@url
	);

	INSERT INTO reddit.Posts 
	(
		documentId, 
		title,
		mediaPreviewUrl
	) 
	VALUES 
	(
		@NewDocId, 
		@title,
		@mediaPreviewUrl
	);
END
GO

-- =============================================
-- Author:      MSR Project Essex
-- Description: Post Processes AzureML output
-- =============================================
CREATE OR ALTER PROCEDURE reddit.PostAzureML
AS
BEGIN
	INSERT INTO reddit.Sentiment (documentId, sentiment)
	SELECT DISTINCT documentId, sentiment FROM reddit.Staging_Sentiment
	WHERE documentId NOT IN (SELECT documentId FROM reddit.Sentiment);

	TRUNCATE TABLE reddit.Staging_Sentiment;

	INSERT INTO reddit.Entities (documentId, entity, entityType, entityOffset, entityLength)
	SELECT documentId, entity, entityType, entityOffset, entityLength
	FROM reddit.Staging_Entities WHERE documentId NOT IN (SELECT documentId FROM reddit.Entities);

	TRUNCATE TABLE reddit.Staging_Entities;

	INSERT INTO reddit.KeyPhrases (documentId, keyPhrase)
	SELECT DISTINCT documentId, keyPhrase
	FROM reddit.Staging_KeyPhrases WHERE documentId NOT IN (select documentId FROM reddit.KeyPhrases);

	TRUNCATE TABLE reddit.Staging_KeyPhrases;
END
GO

