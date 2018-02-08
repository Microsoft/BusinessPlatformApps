CREATE TABLE dbo.tweets_processed
(
	tweetid          NCHAR(20) NOT NULL PRIMARY KEY,
	dateorig         DATETIME,
	hourofdate       DATETIME,
	minuteofdate     DATETIME,
	latitude         FLOAT,
	longitude        FLOAT,
	masterid         NCHAR(25),-- UNIQUE NOT NULL,
	retweet          NCHAR(6),
	username         NCHAR(100),
	usernumber       NCHAR(100),
	image_url        NCHAR(200),
	authorimage_url  NCHAR(200),
	direction        NCHAR(20),
	favorited        INT,
	user_followers   INT,
	user_friends     INT,
	user_favorites   INT,
	user_totaltweets INT
);


CREATE TABLE dbo.tweets_normalized
(
	masterid        NCHAR(25) NOT NULL PRIMARY KEY,-- foreign key references tweets_processed(masterid),
	mentions        INT,
	hashtags        INT,
	tweet           NCHAR(500),
	twitterhandle   NCHAR(100),
	usernumber      NCHAR(100),
	sentiment       FLOAT,
	sentimentbin    FLOAT,
	sentimentposneg NCHAR(10),
	lang            NCHAR(4),
	accounttag      NCHAR(25)
);
ALTER TABLE tweets_processed ADD CONSTRAINT masteridconst FOREIGN KEY (masterid) REFERENCES tweets_normalized(masterid);

CREATE TABLE dbo.hashtag_slicer
(
	 tweetid NCHAR(20),
	 facet   NCHAR(200)
);
ALTER TABLE dbo.hashtag_slicer ADD CONSTRAINT tweethashtag FOREIGN KEY(tweetid) REFERENCES tweets_processed(tweetid);

CREATE TABLE dbo.mention_slicer
(
    tweetid NCHAR(20),
    facet   NCHAR(200)
);
ALTER TABLE dbo.mention_slicer ADD CONSTRAINT tweetmention FOREIGN KEY(tweetid) REFERENCES tweets_processed(tweetid);


CREATE TABLE dbo.entity_graph
(
    tweetid  NCHAR(20),
    [source] NCHAR(200),
    [target] NCHAR(200)
);
ALTER TABLE dbo.entity_graph ADD CONSTRAINT tweetentgraph FOREIGN KEY(tweetid) REFERENCES tweets_processed(tweetid);


CREATE TABLE dbo.authorhashtag_graph
(
    tweetid      NCHAR(20),
    author       NCHAR(200),
    authorcolor  NCHAR(10),
    hashtag      NCHAR(200),
    hashtagcolor NCHAR(10)
);
ALTER TABLE dbo.authorhashtag_graph ADD CONSTRAINT tweetauthor FOREIGN KEY(tweetid) REFERENCES tweets_processed(tweetid);


CREATE TABLE dbo.authormention_graph
(
    tweetid      NCHAR(20),
    author       NCHAR(200),
    authorcolor  NCHAR(10),
    mention      NCHAR(200),
    mentioncolor NCHAR(10)
);
ALTER TABLE dbo.authormention_graph ADD CONSTRAINT tweetmentiongraph FOREIGN KEY(tweetid) REFERENCES tweets_processed(tweetid);


CREATE TABLE dbo.entities
(
    masterid     NCHAR(25),
    entity       NCHAR(200),
    entitytype   NCHAR(10),
    [location]   BIGINT,
    entitylength INT
);
ALTER TABLE dbo.entities ADD CONSTRAINT tweetentity FOREIGN KEY(masterid) REFERENCES tweets_normalized(masterid);


CREATE TABLE dbo.entities2
(
    masterid     NCHAR(25),
    entity       NCHAR(200),
    entitytype   NCHAR(10),
    [location]   BIGINT,
    entitylength INT
);
ALTER TABLE dbo.entities2 ADD CONSTRAINT tweetentity2 FOREIGN KEY(masterid) REFERENCES tweets_normalized(masterid);
go
