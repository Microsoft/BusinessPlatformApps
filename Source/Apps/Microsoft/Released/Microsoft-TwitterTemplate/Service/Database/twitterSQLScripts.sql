  
CREATE TABLE [dbo].[tweets_processed](
	tweetid [nchar](20) NOT NULL PRIMARY KEY,
	dateorig [datetime],
	hourofdate [datetime],
	minuteofdate [datetime],
	latitude [float],
	longitude [float],
	masterid [nchar](25),-- UNIQUE NOT NULL,
	retweet [nchar](6),
	username [nchar](100),
	usernumber [nchar](100),
	image_url [nchar](200),
	authorimage_url [nchar](200),
	direction [nchar](20),
	favorited [int],
	user_followers [int],
	user_friends [int],
	user_favorites [int],
	user_totaltweets [int]
)

CREATE TABLE [dbo].[tweets_normalized](
	masterid [nchar](25) NOT NULL PRIMARY KEY,-- foreign key references tweets_processed(masterid),
	mentions [int],
	hashtags [int],
	tweet [nchar](200),
	twitterhandle [nchar](100),
	usernumber [nchar](100),
	sentiment [float],
	sentimentBin [float],
	sentimentPosNeg [nchar](10),
	lang [nchar](4),
	accounttag [nchar](25)
)

ALTER TABLE tweets_processed ADD CONSTRAINT masteridconst FOREIGN KEY (masterid)  REFERENCES tweets_normalized(masterid);

CREATE TABLE [dbo].[hashtag_slicer](
	tweetid [nchar](20),
	facet [nchar](200)
);
ALTER TABLE [dbo].[hashtag_slicer]  ADD CONSTRAINT tweethashtag FOREIGN KEY(tweetid) REFERENCES tweets_processed(tweetid);

CREATE TABLE [dbo].[mention_slicer](
	tweetid [nchar](20),
	facet [nchar](200)
);
ALTER TABLE [dbo].[mention_slicer]  ADD CONSTRAINT tweetmention FOREIGN KEY(tweetid) REFERENCES tweets_processed(tweetid);

CREATE TABLE [dbo].[entity_graph](
	tweetid [nchar](20),
	source [nchar](200),
	target [nchar](200)
);
ALTER TABLE [dbo].[entity_graph]  ADD CONSTRAINT tweetentgraph FOREIGN KEY(tweetid) REFERENCES tweets_processed(tweetid);

CREATE TABLE [dbo].[authorhashtag_graph](
	tweetid [nchar](20),
	author [nchar](200),
	authorColor [nchar](10),
	hashtag [nchar](200),
	hashtagColor [nchar](10)
);
ALTER TABLE [dbo].[authorhashtag_graph]  ADD CONSTRAINT tweetauthor FOREIGN KEY(tweetid) REFERENCES tweets_processed(tweetid);

CREATE TABLE [dbo].[authormention_graph](
	tweetid [nchar](20),
	author [nchar](200),
	authorColor [nchar](10),
	mention [nchar](200),
	mentionColor [nchar](10)
);
ALTER TABLE [dbo].[authormention_graph]  ADD CONSTRAINT tweetmentiongraph FOREIGN KEY(tweetid) REFERENCES tweets_processed(tweetid);

CREATE TABLE [dbo].[entities](
	masterid [nchar](25),
	entity [nchar](200),
	entitytype [nchar](10),
	location [bigint],
	entitylength [int]
);
ALTER TABLE [dbo].[entities]  ADD CONSTRAINT tweetentity FOREIGN KEY(masterid) REFERENCES tweets_normalized(masterid);

CREATE TABLE [dbo].[entities2](
	masterid [nchar](25),
	entity [nchar](200),
	entitytype [nchar](10),
	location [bigint],
	entitylength [int]
);
ALTER TABLE [dbo].[entities2]  ADD CONSTRAINT tweetentity2 FOREIGN KEY(masterid) REFERENCES tweets_normalized(masterid);

CREATE TABLE [dbo].[TwitterQuery]
(
	[Id] INT NOT NULL PRIMARY KEY,
	[IsAdvanced] TINYINT NOT NULL,
	[QueryString] NVARCHAR(MAX) NOT NULL
)

CREATE TABLE [dbo].[TwitterQueryReadable] (
	[Id] INT NOT NULL PRIMARY KEY,
	[QueryId] INT NOT NULL,
	[QueryReadable] NVARCHAR (MAX) NOT NULL,
	[Query] NVARCHAR (MAX) NOT NULL
)

CREATE TABLE [dbo].[TwitterQueryDetails] (
	[Id] INT NOT NULL PRIMARY KEY,
	[ReadableId] INT NOT NULL,
	[Operator] NVARCHAR(MAX) NOT NULL,
	[Operand] NVARCHAR(MAX) NOT NULL
)