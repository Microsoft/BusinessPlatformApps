CREATE VIEW reddit.SubredditDocumentCountView AS
	SELECT
		Subreddit,
		COUNT(Subreddit) AS DocumentCount
	FROM reddit.Documents
	GROUP BY Subreddit
GO

-- All SubReddit Post fields
CREATE VIEW reddit.AllPostsView AS
	SELECT 
		reddit.Documents.*, 
		reddit.Sentiment.sentiment, 
		reddit.Posts.title,
		reddit.Posts.mediaPreviewUrl,
		CASE WHEN t0.documentContainsUserDefinedEntities
			IS NOT NULL THEN t0.documentContainsUserDefinedEntities
			ELSE CAST(0 AS BIT)
		END AS documentContainsUserDefinedEntities
	FROM
		reddit.Documents
		INNER JOIN reddit.Posts 
			ON reddit.Documents.id = reddit.Posts.documentId
		LEFT JOIN reddit.Sentiment 
			ON reddit.Documents.id = reddit.Sentiment.documentId
		LEFT JOIN (SELECT DISTINCT documentId, CAST(1 AS BIT) AS documentContainsUserDefinedEntities FROM reddit.UserDefinedEntities) t0 
			ON t0.documentId = reddit.Documents.id
GO

-- All documents (comments and posts)
CREATE VIEW [reddit].[AllDocumentsView] AS
	SELECT 
		reddit.Documents.*, 
		reddit.Sentiment.sentiment,
		CASE WHEN t0.documentContainsUserDefinedEntities
			IS NOT NULL THEN t0.documentContainsUserDefinedEntities
			ELSE CAST(0 AS BIT)
		END AS documentContainsUserDefinedEntities
	FROM
		reddit.Documents
		LEFT JOIN reddit.Sentiment 
			ON reddit.Documents.id = reddit.Sentiment.documentId
		LEFT JOIN (SELECT DISTINCT documentId, CAST(1 AS BIT) AS documentContainsUserDefinedEntities FROM reddit.UserDefinedEntities) t0 
			ON t0.documentId = reddit.Documents.id
GO

-- All Entities
CREATE VIEW reddit.AllEntitiesView AS
	SELECT 
		reddit.Entities.documentId,
	    entity,
		entityType,
	    entityOffset,
		entityLength,
		'Automatic' AS EntityOrigin,
		CASE WHEN t0.documentContainsUserDefinedEntities
			IS NOT NULL THEN t0.documentContainsUserDefinedEntities
			ELSE CAST(0 AS BIT)
		END AS documentContainsUserDefinedEntities
	FROM
		reddit.Entities
		LEFT JOIN (SELECT DISTINCT documentId, CAST(1 AS BIT) AS documentContainsUserDefinedEntities FROM reddit.UserDefinedEntities) t0 
			ON t0.documentId = reddit.Entities.documentId
UNION ALL
	SELECT 
		documentId,
	    entity,
		entityType,
	    entityOffset,
		entityLength,
		'UserDefined' AS EntityOrigin,
		CAST(1 AS BIT) AS documentContainsUserDefinedEntities
	FROM
		reddit.UserDefinedEntities
GO

-- All Comment fields
CREATE VIEW reddit.AllCommentsView AS
	SELECT 
		reddit.Documents.*, 
		reddit.Sentiment.sentiment, 
		reddit.Comments.parentId,
		reddit.Comments.postId,
		reddit.Comments.parentUrl,
		reddit.Comments.postUrl,
		CASE WHEN t0.documentContainsUserDefinedEntities
			IS NOT NULL THEN t0.documentContainsUserDefinedEntities
			ELSE CAST(0 AS BIT)
		END AS documentContainsUserDefinedEntities
	FROM
		reddit.Documents
		INNER JOIN reddit.Comments 
			ON reddit.Documents.id = reddit.Comments.documentId
		LEFT JOIN reddit.Sentiment 
			ON reddit.Documents.id = reddit.Sentiment.documentId
		LEFT JOIN (SELECT DISTINCT documentId, CAST(1 AS BIT) AS documentContainsUserDefinedEntities FROM reddit.UserDefinedEntities) t0 
			ON t0.documentId = reddit.Documents.id
GO

-- Comment Authors
CREATE VIEW reddit.CommentAuthorCountsView AS
	SELECT 
		author,
		COUNT(id) AS [count]
	FROM reddit.AllCommentsView
	GROUP BY author
GO

-- Post Authors
CREATE VIEW reddit.PostAuthorCountsView AS
	SELECT 
		author, 
		COUNT(id) AS [count]
	FROM reddit.AllPostsView
	GROUP BY author
GO

-- Subreddit co-authors
CREATE VIEW reddit.PostCoauthorsView AS
	WITH AuthorSubredditCounts AS 
	(
		SELECT subreddit, author, count(*) AS CNT 
		FROM reddit.Documents 
		WHERE isComment = 0
		GROUP by subreddit, author
	)
	SELECT A.subreddit AS subreddit1, B.subreddit AS subreddit2,
		SUM(A.CNT + B.CNT) AS CoAuthorScore
	FROM AuthorSubredditCounts A
	INNER JOIN AuthorSubredditCounts B
	ON A.author = B.author AND A.subreddit <> B.subreddit
	GROUP BY A.subreddit, B.subreddit
GO

-- Comment co-authors
CREATE VIEW reddit.CommentCoauthorsView AS
	WITH AuthorSubredditCounts AS 
	(
		SELECT subreddit, author, count(*) AS CNT 
		FROM reddit.Documents 
		WHERE isComment = 1
		GROUP by subreddit, author
	)
	SELECT A.subreddit AS subreddit1, B.subreddit AS subreddit2,
		SUM(A.CNT + B.CNT) AS CoAuthorScore
	FROM AuthorSubredditCounts A
	INNER JOIN AuthorSubredditCounts B
	ON A.author = B.author AND A.subreddit <> B.subreddit
	GROUP BY A.subreddit, B.subreddit
GO

-- Post and comment subreddit co-authors
CREATE VIEW reddit.PostAndCommentCoauthorsView AS
	WITH AuthorSubredditCounts AS 
	(
		SELECT subreddit, author, count(*) AS CNT 
		FROM reddit.Documents group by subreddit, author
	)
	SELECT A.subreddit AS subreddit1, B.subreddit AS subreddit2,
		SUM(A.CNT + B.CNT) AS CoAuthorScore
	FROM AuthorSubredditCounts A
	INNER JOIN AuthorSubredditCounts B
	ON A.author = B.author AND A.subreddit <> B.subreddit
	GROUP BY A.subreddit, B.subreddit
GO

-- Subreddit count
CREATE VIEW reddit.SubRedditPostCountView AS
	SELECT 
		subreddit, 
		COUNT(id) [PostCount] 
	FROM reddit.AllPostsView
	GROUP BY subreddit
GO

-- comment count by subreddit
CREATE VIEW reddit.CommentCountView AS
	SELECT 
		subreddit, 
		count(id) [count]
	FROM reddit.AllCommentsView
	GROUP BY subreddit
GO

-- Comment Co-Authors where authors commented on the same parent post
CREATE VIEW reddit.CoAuthorView AS
	SELECT tbl1.author AS author1, tbl2.author AS author2, COUNT(1) AS CNT 
	FROM reddit.AllCommentsView tbl1 
	INNER JOIN reddit.AllCommentsView tbl2 ON tbl1.parentid = tbl2.parentid AND tbl1.author != tbl2.author 
	GROUP by tbl1.author, tbl2.author HAVING count(1) > 5;
GO

-- Count of first-level comments on each document (post or comment) that has at least one comment.  This
-- does not count every comment in the comment tree, just the top-level comments based on parentId
CREATE VIEW reddit.CommentCountPerDocumentView AS
	WITH CommentCount AS
	(
		SELECT parentId AS documentId, COUNT(parentId) AS FirstLevelCommentCount
		FROM reddit.Comments
		GROUP BY parentId
	) 
	SELECT CommentCount.documentId, CommentCount.FirstLevelCommentCount, reddit.Documents.isComment
	FROM CommentCount
	INNER JOIN reddit.Documents ON CommentCount.documentId = reddit.Documents.id
GO

-- KeyPhrase Counts of total use of keyPhrase
CREATE VIEW reddit.KeyPhraseCountView AS
	SELECT LOWER(keyPhrase) AS keyPhraseLower, COUNT(1) AS DocumentFrequency 
	FROM reddit.KeyPhrases 
	GROUP BY LOWER(keyPhrase);
GO

-- subreddit /r name to display name mapping view
CREATE VIEW reddit.SubredditMappingView AS
	SELECT DISTINCT subreddit AS fancySubreddit, 
		SUBSTRING(
			url,
			CHARINDEX('reddit.com/r/',url)+13,
			CHARINDEX('/',
				url,
				CHARINDEX('reddit.com/r/',url)+13
			)-13-CHARINDEX('reddit.com/r/',url)
		) AS simpleSubreddit
	FROM reddit.AllDocumentsView;
GO

-- Count of embedded URL domains
CREATE VIEW reddit.EmbeddedUrlDomainCountView AS
SELECT [embeddedUrlDomain], count(embeddedUrlDomain) usageCount
  FROM [reddit].[EmbeddedUrls]
  GROUP BY embeddedUrlDomain
GO

CREATE VIEW reddit.EmbeddedUrlSubredditView AS
	SELECT documentId, 
		embeddedUrl, 
		Docs.subreddit AS sourceSubreddit,
		CASE 
			WHEN 
				CHARINDEX('/',
					embeddedUrl,
					CHARINDEX('reddit.com/r/',embeddedUrl)+13
				) > 0 
				THEN 
					SUBSTRING(embeddedUrl,
						CHARINDEX('reddit.com/r/',embeddedUrl)+13,
						CHARINDEX('/',
							embeddedUrl,
							CHARINDEX('reddit.com/r/',embeddedUrl)+13
						)-13-CHARINDEX('reddit.com/r/',embeddedUrl)
					)
			WHEN 
				CHARINDEX(')',
					embeddedUrl,
					CHARINDEX('reddit.com/r/',embeddedUrl)+13
				) > 0 
				THEN 
					SUBSTRING(embeddedUrl,
						CHARINDEX('reddit.com/r/',embeddedUrl)+13,
						CHARINDEX(')',
							embeddedUrl,
							CHARINDEX('reddit.com/r/',embeddedUrl)+13
						)-13-CHARINDEX('reddit.com/r/',embeddedUrl)
					)
			ELSE 
				SUBSTRING(embeddedUrl,
					CHARINDEX('reddit.com/r/',embeddedUrl)+13,
					LEN(embeddedUrl)
				)
		END AS destinationSubreddit
	FROM reddit.EmbeddedUrls AS URLS 
	INNER JOIN reddit.Documents AS Docs ON URLS.documentId=Docs.id 
	WHERE CHARINDEX('reddit.com/r/',embeddedUrl)>0;
GO

CREATE VIEW reddit.TopDocumentsByDayView AS
	SELECT id, 
			entity,
			RowNum
		FROM (
			SELECT id, 
			score, 
			entity, 
			publishedDayPrecision, 
			RowNum = ROW_NUMBER() 
				OVER (PARTITION BY publishedDayPrecision, 
					entity 
					order by score desc
				)
				FROM ( 
					SELECT DISTINCT entity,
							Docs.id, 
							score, 
							publishedDayPrecision
						FROM reddit.Documents Docs INNER JOIN reddit.UserDefinedEntities Ents ON Docs.id=Ents.documentId
					) tbl1
		) tbl2 WHERE RowNum <= 5
GO

/*
Gather the current and previous week's counts, average score, and average sentitment for all weeks in the dataset per entity. Make sure that each entity has a record for each week.
tmpAggregates = initial counts per week
tmpAggregates2 = set of entities and weeks
*/
CREATE VIEW reddit.WeekOverWeekView AS
	WITH tmpAggregates AS (
		SELECT COUNT(1) AS cnt, 
			AVG(tbl1.score) AS avg_score, 
			AVG(tbl1.sentiment) AS avg_sentiment, 
			entity, 
			tbl1.publishedWeekPrecision 
		FROM reddit.AllDocumentsView tbl1 
		JOIN reddit.UserDefinedEntities tbl2 ON tbl1.id = tbl2.documentId 
		GROUP BY entity, tbl1.publishedWeekPrecision
	)
	, tmpAggregates2 AS (
		SELECT publishedWeekPrecision, 
			entity 
		FROM (SELECT DISTINCT publishedWeekPrecision FROM reddit.AllDocumentsView) tbl1 
		CROSS JOIN (SELECT DISTINCT entity FROM reddit.UserDefinedEntities) tbl2
	)
	SELECT entity, 
		publishedWeekPrecision, 
		LAG(publishedWeekPrecision, 1) OVER(PARTITION BY entity ORDER BY publishedweekprecision) AS priorweek, 
		LAG(cnt, 1) OVER(PARTITION BY entity ORDER BY publishedweekprecision) AS priorweekcnt, 
		cnt, 
		LAG(avg_score, 1) OVER(PARTITION BY entity ORDER BY publishedweekprecision) AS priorweekavg_score, 
		avg_score, 
		LAG(avg_sentiment, 1) OVER(PARTITION BY entity ORDER BY publishedweekprecision) AS priorweekavg_sentiment, 
		avg_sentiment  
	FROM (
		SELECT tbl2.publishedWeekPrecision, 
			tbl2.entity, 
			COALESCE(tbl1.cnt, 0) AS cnt, 
			COALESCE(tbl1.avg_score, 0) AS avg_score, 
			COALESCE(tbl1.avg_sentiment, 0) AS avg_sentiment 
		FROM tmpAggregates tbl1 
		RIGHT OUTER JOIN tmpAggregates2 tbl2 ON tbl1.publishedWeekPrecision = tbl2.publishedWeekPrecision AND tbl1.entity = tbl2.entity
	) tbl3
GO

/* Duplicate data so the chord chart can be filtered on a single column instead of needing both a source and dest filter
*/
CREATE VIEW reddit.ChordChartSubredditAuthorConnections AS
	SELECT
		subreddit1 AS subreddit, 
		subreddit1 AS sourceSubreddit, 
		subreddit2 AS destSubreddit, 
		CoAuthorScore 
	FROM reddit.PostAndCommentCoauthorsView 
	UNION 
	SELECT subreddit2 AS subreddit, 
		subreddit1 AS sourceSubreddit, 
		subreddit2 AS destSubreddit, 
		CoAuthorScore 
	FROM reddit.PostAndCommentCoauthorsView 
GO

CREATE VIEW reddit.ChordChartEmbeddedUrlsView AS 
	SELECT
		subreddit,
		embeddedUrlDomain,
		COUNT(1) AS ConnectionScore
	FROM reddit.EmbeddedUrls EU INNER JOIN reddit.AllDocumentsView ADV ON EU.documentId=ADV.id 
	GROUP BY subreddit, embeddedUrlDomain
GO

CREATE VIEW reddit.CoauthorRelationshipsView AS
	SELECT a.id AS id,
		a.author AS author1, 
		b.author AS author2
	FROM reddit.AllPostsView a 
	INNER JOIN reddit.AllCommentsView b ON a.id=b.postId 
	WHERE NOT(a.author=b.author)	--commenting on your own posts doesn't count
		
	UNION ALL
		
	--select where b is post author and a comments on post, pulling in subreddit and entity (author/commenter relationship appears in both directions)
	SELECT a.id AS id,
		a.author AS author1, 
		b.author AS author2 
	FROM reddit.AllCommentsView a 
	INNER JOIN reddit.AllPostsView b ON a.postId=b.id 
	WHERE NOT(a.author=b.author)

	UNION ALL

	--coauthors on the same post
	SELECT a1.id,
		a1.author AS author1,
		b1.author AS author2
	FROM reddit.AllCommentsView a1
	INNER JOIN reddit.AllCommentsView b1 ON a1.postId=b1.postId
	WHERE NOT(a1.author=b1.author)
GO

/*
All documents to run through Machine Learning.  This is done in a view
so that the AML runner can check for available data before running
the experiment.
*/
CREATE VIEW reddit.AzureMachineLearningInputView AS
	SELECT TOP 50000 
		content, 
		id AS messageId, 
		(ROW_NUMBER() OVER (ORDER BY id) - 1) AS ExperimentDocId 
	FROM reddit.AllDocumentsView 
	WHERE
		content IS NOT NULL AND LEN(content) > 0 AND sentiment IS NULL
GO
