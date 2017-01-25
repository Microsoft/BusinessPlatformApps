SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
go


-- ConfigurationView
CREATE VIEW bpst_news.vw_configuration
AS
    SELECT [id],
            configuration_group    AS [configuration group],
            configuration_subgroup AS [configuration subgroup],
            [name]                 AS [name],
            [value]                AS [value]
    FROM	bpst_news.[configuration]
    WHERE  visible = 1;
go


CREATE VIEW bpst_news.vw_FullDocument
AS
    SELECT documents.id								AS [Id],
           documents.abstract						AS [Abstract],
           documents.title							AS [Title],
           documents.sourceUrl						AS [Source URL],
           documents.sourceDomain					AS [Source Domain],
           documents.category						AS [Category],
           documents.imageUrl						AS [Image URL],
           documents.imageWidth						AS [Image Width],
           documents.imageHeight					AS [Image Height],
           documentsentimentscores.score			AS [Sentiment Score],
           documentpublishedtimes.[timestamp]		AS [PublishedTimestamp],
           documentpublishedtimes.monthPrecision	AS [Published Month Precision],
           documentpublishedtimes.weekPrecision		AS [Published Week Precision],
           documentpublishedtimes.dayPrecision		AS [Published Day Precision],
           documentpublishedtimes.hourPrecision		AS [Published Hour Precision],
           documentpublishedtimes.minutePrecision	AS [Minute Precision],
           documentingestedtimes.[timestamp]		AS [Ingested Timestamp],
           documentingestedtimes.monthPrecision		AS [Ingested Month Precision],
           documentingestedtimes.weekPrecision		AS [Ingested Week Precision],
           documentingestedtimes.dayPrecision		AS [Ingested Day Precision],
           documentingestedtimes.hourPrecision		AS [Ingested Hour Precision],
           documentingestedtimes.minutePrecision	AS [Ingested Minute Precision]
    FROM   bpst_news.documents documents LEFT OUTER JOIN documentpublishedtimes 	ON documents.id = documentpublishedtimes.id
                                         LEFT OUTER JOIN documentingestedtimes		ON documents.id = documentingestedtimes.id
                                         LEFT OUTER JOIN documentsentimentscores 	ON documents.id=documentsentimentscores.id;
go

CREATE VIEW bpst_news.vw_FullDocumentTopics
AS
    SELECT documenttopics.documentId					AS [Document Id],
           documenttopics.topicId						AS [Topic Id],
           documenttopics.batchId						AS [Batch Id],
           documenttopics.documentDistance				AS [Document Distance],
           documenttopics.topicScore					AS [Topic Score],
           documenttopics.topicKeyPhrase				AS [Topic Key Phrase],
           documenttopicimages.imageUrl1				AS [Image URL 1],
           documenttopicimages.imageUrl2				AS [Image URL 2],
           documenttopicimages.imageUrl3				AS [Image URL 3],
           documenttopicimages.imageUrl4				AS [Image URL 4],
           ((1-DocumentTopics.documentDistance)*100)	AS [Weight]
    FROM   bpst_news.documenttopics	LEFT OUTER JOIN documenttopicimages	ON documenttopics.topicid = documenttopicimages.topicid;
go


CREATE VIEW bpst_news.vw_FullEntities
AS
    SELECT 	documentId					AS [Document Id],
            entityType					AS [Entity Type],
            entityValue					AS [Entity Value],
            offset						AS [Offset],
            offsetDocumentPercentage	AS [Offset Document Percentage],
            [length]					AS [Lenth],
            entityType + entityValue	AS [Entity Id],
        CASE
			WHEN entityType = 'TIL' THEN 'fa fa-certificate'
			WHEN entityType = 'Title' THEN 'fa fa-certificate'
			WHEN entityType = 'PER' THEN 'fa fa-male'
			WHEN entityType = 'Person' THEN 'fa fa-male'
			WHEN entityType = 'ORG' THEN 'fa fa-sitemap'
			WHEN entityType = 'Organization' THEN 'fa fa-sitemap'
			WHEN entityType = 'LOC' THEN 'fa fa-globe'
			WHEN entityType = 'Location' THEN 'fa fa-globe'
			ELSE null
            END [Entity Class],
		CASE
			WHEN entityType = 'TIL' THEN '#FFFFFF'
			WHEN entityType = 'Title' THEN '#FFFFFF'
			WHEN entityType = 'PER' THEN '#1BBB6A'
			WHEN entityType = 'Person' THEN '#1BBB6A'
			WHEN entityType = 'ORG' THEN '#FF001F'
			WHEN entityType = 'Organization' THEN '#FF001F'
			WHEN entityType = 'LOC' THEN '#FF8000'
			WHEN entityType = 'Location' THEN '#FF8000'
			ELSE null
            END [Entity Color]
     FROM bpst_news.entities;
go

CREATE VIEW bpst_news.vw_EntityRankings AS
    WITH DocCounts AS
    (
        SELECT  count(DISTINCT documentId)	AS [Document Count],
                entityType					AS [Entity Type],
                entityValue					AS [Entity Value]
        FROM bpst_news.Entities
        GROUP BY entityType, entityValue
    )
    SELECT ROW_NUMBER() OVER
        (PARTITION BY [Entity Type] ORDER BY [Document Count] DESC) AS [Entity Value Rank],
        [Entity Type] + [Entity Value]								AS [Entity Id],
        [Entity Type],
        [Entity Value],
        [Document Count]
    FROM DocCounts;
go

CREATE VIEW bpst_news.vw_DocumentKeyPhrases
AS
    SELECT documentid			AS [Document Id],
           phrase				AS [Phrase]
    FROM bpst_news.documentkeyphrases;
go


CREATE VIEW bpst_news.vw_DocumentSentimentScores
AS
    SELECT id		AS [Id],
           score	AS [Score]
    FROM bpst_news.documentsentimentscores;
go


CREATE VIEW bpst_news.vw_DocumentCompressedEntities
AS
    SELECT documentid				AS [Document Id],
           compressedEntitiesJson	AS [Compressed Entities Json]
    FROM bpst_news.documentcompressedentities;
go


CREATE VIEW bpst_news.vw_DocumentTopicImages
AS
    SELECT topicId			AS [Topic Id],
           imageUrl1		AS [Image URL 1],
           imageUrl2		AS [Image URL 2],
           imageUrl3		AS [Image URL 3],
           imageUrl4		AS [Image URL 4]
    FROM   bpst_news.documenttopicimages;
go

CREATE VIEW bpst_news.vw_TopicKeyPhrases
AS
	SELECT topicId			AS [Topic Id],
           KeyPhrase		AS [Key Phrase]
	FROM   bpst_news.topickeyphrases;
go
