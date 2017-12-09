SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
go


-- ConfigurationView
CREATE VIEW pbist_twitter.vw_configuration
AS
    SELECT [id],
            configuration_group    AS [configuration group],
            configuration_subgroup AS [configuration subgroup],
            [name]                 AS [name],
            [value]                AS [value]
    FROM   pbist_twitter.[configuration]
    WHERE  visible = 1;
go


CREATE VIEW pbist_twitter.vw_authorhashtag_graph
AS
    SELECT tweetid					AS [Tweet Id],
           author					AS [Author],
           authorcolor				AS [Author Color],		
           hashtag					AS [Hashtag],
           hashtagcolor				AS [Hashtag Color]
    FROM   pbist_twitter.authorhashtag_graph;
go


CREATE VIEW pbist_twitter.vw_authormention_graph
AS
    SELECT tweetid					AS [Tweet Id],
           author					AS [Author],
           authorcolor				AS [Author Color],
           mention					AS [Mention],
           mentioncolor				AS [Mention Color]
    FROM   pbist_twitter.authormention_graph;
go



CREATE VIEW pbist_twitter.vw_hashtag_slicer
AS
    SELECT tweetid					AS [Tweet Id],
           facet					AS [Facet]
    FROM   pbist_twitter.hashtag_slicer;
go


CREATE VIEW pbist_twitter.vw_mention_slicer
AS
    SELECT tweetid					AS [Tweet Id],
           facet					AS [Facet]
    FROM   pbist_twitter.mention_slicer;
go


CREATE VIEW pbist_twitter.vw_tweets_normalized
AS
    SELECT masterid					AS [Master Id],
           mentions					AS [Mentions],
           hashtags					AS [Hashtags],
           tweet					AS [Tweet],
           twitterhandle			AS [Twitter Handle],
		   userlocation				AS [User Location],
           usernumber				AS [User Number],
           sentiment				AS [Sentiment],
           sentimentbin				AS [Sentiment Bin],
           sentimentposneg			AS [Sentiment Positive/Negative],
           lang						AS [Language],
           accounttag				AS [Account Tag]
    FROM   pbist_twitter.tweets_normalized;
go


CREATE VIEW pbist_twitter.vw_tweets_processed
AS
    SELECT tweetid					AS [Tweet Id],
           dateorig					AS [Original Date],
           Convert(date,[dateorig]) AS [Date],
           hourofdate				AS [Hours],
           minuteofdate				AS [Minutes],
           latitude					AS [Latitude],
           longitude				AS [Longitude],
           masterid					AS [Master Id],
           retweet					AS [Retweet],
           username					AS [Username],
		   userlocation				AS [User Location],
           usernumber				AS [User Number],
           image_url				AS [Image URL],
           authorimage_url			AS [Author Image URL],
           direction				AS [Direction],
           favorited				AS [Favorited],
           user_followers			AS [User Followers],
           user_friends				AS [User Friends],
           user_favorites			As [User Favourites],
           user_totaltweets			AS [User Total Tweets]
    FROM   pbist_twitter.tweets_processed;
GO

--CREATE VIEW pbist_twitter.vw_search_terms
--AS
--    SELECT tweetid					AS [Tweet Id],
--           searchterm				AS [Search Term],
--		   accountid				AS [Account Id],
--		   direction				AS [Tweet Direction]
--    FROM   pbist_twitter.search_terms;
--go

CREATE VIEW pbist_twitter.vw_minimum_tweets
AS
    SELECT MinimumTweets AS [Minimum Tweets] FROM pbist_twitter.minimum_tweets
go
