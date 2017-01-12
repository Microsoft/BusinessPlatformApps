#r "Newtonsoft.Json"
#r "System.Data"

using System;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Web;


public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
{
    string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
    TweetHandler tweetHandler = new TweetHandler(connectionString);
    string jsonContent = await req.Content.ReadAsStringAsync();
    var tweets = JsonConvert.DeserializeObject(jsonContent);
    if (tweets is JArray)
    {
        foreach (var item in (JArray)tweets)
        {
            var individualtweet = item.ToString();
            tweetHandler.ParseTweet(individualtweet);
        }
    }
    else
    {
        tweetHandler.ParseTweet(jsonContent);
    }

    // log.Info($"{data}");
    return req.CreateResponse(HttpStatusCode.OK, "");
}

public class TweetHandler
{
    public TweetHandler(string connection)
    {
        if (string.IsNullOrEmpty(connection))
        {
            throw new ArgumentNullException("connection", "Connection string is null or empty.");
        }
        this.connectionString = connection;
    }

    private dynamic tweet = string.Empty;
    private JObject tweetObj = new JObject();
    private string connectionString;

    //Create dictionaries for SQL tables
    private Dictionary<string, string> originalTweets = new Dictionary<string, string>()
        {
            {"masterid", null},
            {"tweet", null},
            {"twitterhandle", null},
            {"sentiment", null},
            {"lang", null},
            {"sentimentBin", null},
            {"sentimentPosNeg", null},
            {"accounttag", "Unknown"}
        };

    private Dictionary<string, string> processedTweets = new Dictionary<string, string>()
        {
            {"tweetid", null},
            {"masterid", null},
            {"image_url", null},
            {"dateorig", null},
            {"authorimage_url", null},
            {"username", null},
            {"hourofdate", null},
            {"minuteofdate", null},
            {"direction", "Text"},
            {"favorited", "1"},
            {"retweet", "False"},
            {"user_followers", null},
            {"user_friends", null},
            {"user_favorites", null},
            {"user_totaltweets", null}
        };

    private Dictionary<string, string> hashtagSlicer = new Dictionary<string, string>()
        {
            {"tweetid", null},
            {"facet", null}
        };

    private Dictionary<string, string> mentionSlicer = new Dictionary<string, string>()
        {
            {"tweetid", null},
            {"facet", null}
        };

    private Dictionary<string, string> authorHashtagGraph = new Dictionary<string, string>()
        {
            {"tweetid", null},
            {"author", null},
            {"authorColor", "#01B8AA"},
            {"hashtag", null},
            {"hashtagColor", "#374649"},
        };

    private Dictionary<string, string> authorMentionGraph = new Dictionary<string, string>()
        {
            {"tweetid", null},
            {"author", null},
            {"authorColor", "#01B8AA"},
            {"mention", null},
            {"mentionColor", "#374649"},
        };

    public async Task<bool> ParseTweet(string entireTweet)
    {
        // Convert JSON to dynamic C# object
        tweetObj = JObject.Parse(entireTweet);
        tweet = tweetObj;

        //Connect to Azure SQL Database & bring in Twitter Handles & IDs
        string twitterHandles =
            ExecuteSqlQuery("select value FROM pbist_twitter.configuration where name = \'twitterHandle\'", "value");
        string twitterHandleId =
            ExecuteSqlQuery("select value FROM pbist_twitter.configuration where name = \'twitterHandleId\'",
                "value");

        // Split out all the handles & create dictionary
        String[] handle = null;
        String[] handleId = null;
        var dictionary = new Dictionary<string, string>();

        if (twitterHandles != String.Empty)
        {
            handle = SplitHandles(twitterHandles, ',');
            handleId = SplitHandles(twitterHandleId, ',');
            for (int index = 0; index < handle.Length; index++)
            {
                dictionary.Add(handle[index], handleId[index]);
            }
        }

        // Check if language of tweet is supported for sentiment analysis
        originalTweets["lang"] = tweet.TweetLanguageCode.ToString();
        if (originalTweets["lang"] == "en" || originalTweets["lang"] == "fr" || originalTweets["lang"] == "es" || originalTweets["lang"] == "pt")
        {
            //Sentiment analysis - Cognitive APIs 
            string sentiment = await MakeSentimentRequest(tweet);
            sentiment = (double.Parse(sentiment) * 2 - 1).ToString(CultureInfo.InvariantCulture);
            string sentimentBin = (Math.Floor(double.Parse(sentiment) * 10) / 10).ToString(CultureInfo.InvariantCulture);
            string sentimentPosNeg = String.Empty;
            if (double.Parse(sentimentBin) > 0)
            {
                sentimentPosNeg = "Positive";
            }
            else if (double.Parse(sentimentBin) < 0)
            {
                sentimentPosNeg = "Negative";
            }
            else
            {
                sentimentPosNeg = "Neutral";
            }

            //Save sentiment and language metadata into dictionary
            originalTweets["sentiment"] = sentiment;
            originalTweets["sentimentBin"] = sentimentBin;
            originalTweets["sentimentPosNeg"] = sentimentPosNeg;
        }
        else
        {
            originalTweets["sentimentPosNeg"] = "Undefined";
        }

        // Work out account and tweet direction for retweets
        if (tweet.OriginalTweet != null)
        {
            processedTweets["direction"] = "Text Retweet";
            originalTweets["twitterhandle"] = tweet.OriginalTweet.UserDetails.UserName;
            if (dictionary.Count > 0)
            {
                foreach (var entry in dictionary)
                {
                    HashtagDirectionCheck(entry, " Retweet");
                    MessageDirectionCheck(entry, tweetObj.SelectToken("OriginalTweet.UserMentions"), " Retweet");
                }
            }
            // Save retweets into SQL table
            saveTweets(tweet.OriginalTweet);
        }
        // Works out the tweet direction for original tweets (not retweets)
        else
        {
            originalTweets["twitterhandle"] = tweet.UserDetails.UserName;
            if (dictionary.Count > 0)
            {
                foreach (var entry in dictionary)
                {
                    HashtagDirectionCheck(entry);
                    MessageDirectionCheck(entry, tweetObj.SelectToken("UserMentions"));
                }
            }

            // Save original tweets into SQL Table
            saveTweets(tweet);
        }

        processedTweets["tweetid"] = tweet.TweetId;

        //Save time metadata about processed tweets
        string createdat = tweet.CreatedAt.ToString();
        DateTime ts = DateTime.ParseExact(createdat, "ddd MMM dd HH:mm:ss +ffff yyyy", CultureInfo.CurrentCulture);
        processedTweets["dateorig"] = DateTime.Parse(ts.Year.ToString() + " " + ts.Month.ToString() + " " + ts.Day.ToString() + " " + ts.Hour.ToString() + ":" + ts.Minute.ToString() + ":" + ts.Second.ToString()).ToString(CultureInfo.InvariantCulture);
        processedTweets["minuteofdate"] = DateTime.Parse(ts.Year.ToString() + " " + ts.Month.ToString() + " " + ts.Day.ToString() + " " + ts.Hour.ToString() + ":" + ts.Minute.ToString() + ":00").ToString(CultureInfo.InvariantCulture);
        processedTweets["hourofdate"] = DateTime.Parse(ts.Year.ToString() + " " + ts.Month.ToString() + " " + ts.Day.ToString() + " " + ts.Hour.ToString() + ":00:00").ToString(CultureInfo.InvariantCulture);
        

        //Save media and follower metadata about processed tweets
        processedTweets["authorimage_url"] = tweet.UserDetails.ProfileImageUrl;
        processedTweets["username"] = tweet.UserDetails.UserName;
        processedTweets["user_followers"] = tweet.UserDetails.FollowersCount;
        processedTweets["user_friends"] = tweet.UserDetails.FavouritesCount;
        processedTweets["user_favorites"] = tweet.UserDetails.FriendsCount;
        processedTweets["user_totaltweets"] = tweet.UserDetails.StatusesCount;

        string firstUrl = String.Empty;

        if (tweetObj.SelectToken("MediaUrls") != null && tweetObj.SelectToken("MediaUrls").HasValues)
        {
            firstUrl = tweet.MediaUrls[0];
            if (firstUrl != String.Empty)
            {
                processedTweets["image_url"] = firstUrl;
            }
        }

        if (tweet.favorited != "true")
        {
            processedTweets["favorited"] = "1";
        }

        if (tweet.OriginalTweet != null)
        {
            processedTweets["retweet"] = "True";
        }

        //Save processed tweets into SQL
        int response = 0;
        response = ExecuteSqlScalar(
            $"Select count(1) FROM pbist_twitter.tweets_processed WHERE tweetid = '{processedTweets["tweetid"]}'");
        if (response == 0)
        {
            try
            {
                ExecuteSqlNonQuery(generateSQLQuery("pbist_twitter.tweets_processed", processedTweets));
            }
            catch (Exception e) { }
        }

        string text = tweet.TweetText.ToString();
        //Populate hashtag slicer table
        if (text.Contains("#"))
        {
            hashtagSlicer["tweetid"] = tweet.TweetId;
            hashtagmentions(text, '#', "facet", "pbist_twitter.hashtag_slicer", hashtagSlicer);
        }

        //Populate author hashtag network table
        if (text.Contains("#"))
        {
            authorHashtagGraph["tweetid"] = tweet.TweetId;
            authorHashtagGraph["author"] = tweet.UserDetails.UserName;
            hashtagmentions(text, '#', "hashtag", "pbist_twitter.authorhashtag_graph", authorHashtagGraph);
        }

        //Populate mention slicer table
        if (text.Contains("@"))
        {
            mentionSlicer["tweetid"] = tweet.TweetId;
            hashtagmentions(text, '@', "facet", "pbist_twitter.mention_slicer", mentionSlicer);
        }

        //Populate author mention network table
        if (text.Contains("@"))
        {
            authorMentionGraph["tweetid"] = tweet.TweetId;
            authorMentionGraph["author"] = tweet.UserDetails.UserName;
            hashtagmentions(text, '@', "mention", "pbist_twitter.authormention_graph", authorMentionGraph);
        }

        return true;
    }

    //Execute SQL query without returning anything
    private void ExecuteSqlNonQuery(string sqlQuery)
    {
        using (SqlConnection connection = new SqlConnection(connectionString.ToString()))
        {
            connection.Open();
            var command = new SqlCommand(sqlQuery, connection);
            command.ExecuteNonQuery();
        }
    }

    //Execute SQL query returning something 
    private int ExecuteSqlScalar(string sqlQuery)
    {
        using (SqlConnection connection = new SqlConnection(connectionString.ToString()))
        {
            connection.Open();
            var command = new SqlCommand(sqlQuery, connection);
            return (int)command.ExecuteScalar();
        }
    }

    //Execute SQL query using a reader
    private string ExecuteSqlQuery(string sqlQuery, string value)
    {
        using (SqlConnection connection = new SqlConnection(connectionString.ToString()))
        {
            connection.Open();
            var command = new SqlCommand(sqlQuery, connection);
            SqlDataReader reader = command.ExecuteReader();
            string returnObject = string.Empty;
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    returnObject = reader[value].ToString();
                }
            }
            return returnObject;
        }
    }

    //Write tweets into SQL
    private void saveTweets(dynamic tweetType)
    {
        originalTweets["masterid"] = tweetType.TweetId.ToString();
        processedTweets["masterid"] = tweetType.TweetId.ToString();
        originalTweets["tweet"] = tweetType.TweetText.ToString();
        int response = 0;
        response = ExecuteSqlScalar(
            $"Select count(1) FROM pbist_twitter.tweets_normalized WHERE masterid = '{originalTweets["masterid"]}'");
        if (response == 0)
        {
            try
            {
                ExecuteSqlNonQuery(generateSQLQuery("pbist_twitter.tweets_normalized", originalTweets));
            }
            catch (Exception e) { }
        }
    }

    //Generate SQL Statement
    private string generateSQLQuery(string tableName, Dictionary<string, string> dictionary)
    {
        string sqlQueryGenerator = $"insert into " + tableName + "(" +
                                   String.Join(", ", dictionary.Select(x => x.Key)) + ")" + " VALUES " + "('" +
                                   String.Join("','", dictionary.Select(x => {
                                       if (string.IsNullOrEmpty(x.Value))
                                       {
                                           return x.Value;
                                       }
                                       return x.Value.Replace("'", "''");
                                   })) + "')";
        return sqlQueryGenerator;
    }


    //Split out terms by delimiter
    private String[] SplitHandles(string handle, char delimiter)
    {
        String[] handleId = new string[] { };
        handleId = handle.Split(delimiter);
        return handleId;
    }

    //Figure out if hashtag is in tweet
    private void HashtagDirectionCheck(KeyValuePair<string, string> currentEntry, string retweet = "")
    {
        string hashtag = "#" + currentEntry.Key.ToLower();
        string tweetText = tweet.TweetText.ToString();
        if (tweetText.ToLower().Contains(hashtag))
        {
            processedTweets["direction"] = "Hashtag" + retweet;
            originalTweets["accounttag"] = currentEntry.Key;
        }
    }

    //Figure out direction of tweet
    private void MessageDirectionCheck(KeyValuePair<string, string> currentEntry, JToken userMentions,
        string retweet = "")
    {
        if (currentEntry.Value.Contains(tweet.UserDetails.Id.ToString()))
        {
            processedTweets["direction"] = "Outbound" + retweet;
            originalTweets["accounttag"] = currentEntry.Key;
            if (tweet.UserDetails.TweetInReplyToUserId != null)
            {
                processedTweets["direction"] = "Outbound Reply" + retweet;
                originalTweets["accounttag"] = currentEntry.Key;
            }
        }
        else if (tweet.UserDetails.TweetInReplyToUserId != null)
        {
            if (currentEntry.Value.Contains(tweet.UserDetails.TweetInReplyToUserId.ToString()))
            {
                processedTweets["direction"] = "Inbound Reply" + retweet;
                originalTweets["accounttag"] = currentEntry.Key;
            }
        }
        else if (retweet != "")
        {
            if (currentEntry.Value.Contains(tweet.OriginalTweet.UserDetails.Id.ToString()))
            {
                processedTweets["direction"] = "Retweet of Outbound";
                originalTweets["accounttag"] = currentEntry.Key;
            }
        }
        if (userMentions != null && userMentions.HasValues)
        {
            foreach (var usermentionItem in userMentions)
            {
                string uid = usermentionItem.SelectToken("Id").ToString();
                if (currentEntry.Value.Contains(uid.ToString()))
                {
                    processedTweets["direction"] = "Inbound" + retweet;
                    originalTweets["accounttag"] = currentEntry.Key;
                }
            }
        }
    }

    private void hashtagmentions(string text, char delimiter, string field, string sqlTable, Dictionary<string, string> dictionary)
    {
        var regex = new Regex(@"(?<=" + delimiter + @")\w+");
        var matches = regex.Matches(text);
        foreach (Match match in matches)
        {
            dictionary[field] = match.ToString();
            ExecuteSqlNonQuery(generateSQLQuery(sqlTable, dictionary));
        }
    }

    static async Task<string> MakeSentimentRequest(dynamic tweet)
    {
        var client = new HttpClient();
        Document response = new Document()
        {
            Text = tweet.TweetText.ToString(),
            Language = tweet.TweetLanguageCode.ToString(),
            Id = tweet.TweetId.ToString()
        };

        //Request headers
        string subscriptionKey = System.Configuration.ConfigurationManager.ConnectionStrings["subscriptionKey"].ConnectionString;
        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
        var uri = "https://westus.api.cognitive.microsoft.com/text/analytics/v2.0/sentiment";
        //Request body
        Sentiment sentiment = new Sentiment();
        sentiment.documents = new List<Document>();
        sentiment.documents.Add(response);
        string serializedSentiment = JsonConvert.SerializeObject(sentiment);
        StringContent content = new StringContent(serializedSentiment, Encoding.UTF8, "application/json");

        //new request


        var sentimentResponse = await client.PostAsync(uri, content);
        if (!sentimentResponse.IsSuccessStatusCode)
        {
            throw new Exception();
        }

        var sentimentResponseBody = await sentimentResponse.Content.ReadAsStringAsync();
        dynamic sentimentDeserialized = JsonConvert.DeserializeObject(sentimentResponseBody);
        string sentimentScore = sentimentDeserialized.documents[0].score.ToString();

        return sentimentScore;


    }
}

public class Document
{
    public string Language { get; set; }
    public string Id { get; set; }
    public string Text { get; set; }
}

public class Sentiment
{
    [JsonProperty(PropertyName = "documents")]
    public List<Document> documents { get; set; }
}

