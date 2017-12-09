using FacebookETL;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookUtillity
{
    public class MainETL
    {
        public static async Task<bool> PopulateAll(string sqlConn, string schema, string cognitiveKey, string client, string secret, string date)
        {
            string token = await FacebookUtility.GetAccessTokenAsync(client, secret);
            string[] pages = SqlUtility.GetPages(sqlConn, schema);

            List<JObject> posts = new List<JObject>();

            foreach (var pageToSearch in pages)
            {
                string page = pageToSearch.Replace(" ", "");
                try
                {
                    var pageObj = await FacebookUtility.GetPage(page, token);

                    // Get Facebook Posts
                    posts = await FacebookUtility.GetPostsAsync(page, date, token);
                    // Get All Data Tables
                    var commentsDataTable = DataTableUtility.GetCommentsDataTable();
                    var hashTagDataTable = DataTableUtility.GetHashTagDataTable();
                    var keyPhraseDataTable = DataTableUtility.GetKeyPhraseDataTable();
                    var postDataTable = DataTableUtility.GetPostsDataTable();
                    var reactionsDataTable = DataTableUtility.GetReactionsDataTable();
                    var sentimentDataTable = DataTableUtility.GetSentimentDataTable();

                    PopulatePostCommentsAndReactions(postDataTable, commentsDataTable, reactionsDataTable, posts, page, pageObj);

                    // Populate Sentiment
                    Dictionary<string, string> items = new Dictionary<string, string>();
                    CognitiveUtility.PopulateDictionary(items, postDataTable);
                    CognitiveUtility.PopulateDictionary(items, commentsDataTable);
                    var payloads = CognitiveUtility.GetPayloads(items);

                    await CognitiveUtility.GetSentimentAsync(payloads, sentimentDataTable, cognitiveKey);
                    await CognitiveUtility.GetKeyPhraseAsync(payloads, keyPhraseDataTable, cognitiveKey);
                    CognitiveUtility.GetHashTags(postDataTable, hashTagDataTable);
                    CognitiveUtility.GetHashTags(commentsDataTable, hashTagDataTable);

                    // Bulk Insert
                    SqlUtility.BulkInsert(sqlConn, postDataTable, schema + "." + "StagingPosts");
                    SqlUtility.BulkInsert(sqlConn, sentimentDataTable, schema + "." + "StagingSentiment");
                    SqlUtility.BulkInsert(sqlConn, commentsDataTable, schema + "." + "StagingComments");
                    SqlUtility.BulkInsert(sqlConn, keyPhraseDataTable, schema + "." + "StagingKeyPhrase");
                    SqlUtility.BulkInsert(sqlConn, reactionsDataTable, schema + "." + "StagingReactions");
                    SqlUtility.BulkInsert(sqlConn, hashTagDataTable, schema + "." + "StagingHashTags");

                    // Debugging
                    var errorDataTable = DataTableUtility.GetErrorDataTable();
                    DataRow errorRow = errorDataTable.NewRow();
                    errorRow["Date"] = date;
                    errorRow["Error"] = "";
                    errorRow["Posts"] = page + ":" + JToken.FromObject(posts).ToString();
                    errorDataTable.Rows.Add(errorRow);
                    SqlUtility.BulkInsert(sqlConn, errorDataTable, schema + "." + "StagingError");
                }
                catch (Exception e)
                {
                    var errorDataTable = DataTableUtility.GetErrorDataTable();
                    DataRow errorRow = errorDataTable.NewRow();
                    errorRow["Date"] = date;
                    errorRow["Error"] = e.ToString();
                    errorRow["Posts"] = page + ":" + JToken.FromObject(posts).ToString();
                    errorDataTable.Rows.Add(errorRow);
                    SqlUtility.BulkInsert(sqlConn, errorDataTable, schema + "." + "StagingError");
                    throw;
                }
            }
            return true;
        }

        public static void PopulatePostCommentsAndReactions(DataTable postsDataTable, DataTable commentsDataTable,
            DataTable reactionsDataTable, List<JObject> posts, string page, JObject pageObj)
        {
            foreach (var postPayload in posts)
            {
                foreach (var post in postPayload["data"])
                {
                    DataRow postRow = postsDataTable.NewRow();
                    postRow["Id"] = post["id"];
                    postRow["Created Date"] = post["created_time"];
                    postRow["Message"] = post["message"];
                    postRow["From Id"] = post["from"]["id"];
                    int maxLength = Math.Min(post["from"]["name"].ToString().Length, 100);
                    postRow["From Name"] = post["from"]["name"].ToString().Substring(0, maxLength);
                    postRow["Media"] = post["picture"];
                    postRow["Page"] = page;
                    postRow["PageId"] = pageObj["id"].ToString();
                    postRow["PageDisplayName"] = pageObj["name"];

                    if(post["comments"]["data"].Count() == 100 )
                    {
                        postRow["Total Comments"] = Utility.ConvertToLong(post["comments"]["summary"]["total_count"]); ;
                    }
                    else
                    {
                        postRow["Total Comments"] = post["comments"]["data"].Count();
                    }

                    postsDataTable.Rows.Add(postRow);


                    foreach (var comment in post["comments"]["data"])
                    {
                       
                        DataRow commentRow = commentsDataTable.NewRow();
                        commentRow["Id"] = comment["id"];
                        commentRow["Created Date"] = comment["created_time"];
                        commentRow["Message"] = comment["message"];
                        commentRow["From Id"] = comment["from"]["id"];

                        maxLength = Math.Min(comment["from"]["name"].ToString().Length, 100);
                        commentRow["From Name"] = comment["from"]["name"].ToString().Substring(0, maxLength);
                        commentRow["Post Id"] = post["id"];
                        commentRow["Page"] = page;
                        commentRow["PageId"] = pageObj["id"].ToString();
                        commentRow["PageDisplayName"] = pageObj["name"];
                        commentsDataTable.Rows.Add(commentRow);
                    }


                    DataRow likeRow = reactionsDataTable.NewRow();
                    likeRow["Id"] = post["id"];
                    likeRow["Reaction Type"] = "Like";
                    likeRow["Count"] = Utility.ConvertToLong(post["reactions_like"]["summary"]["total_count"]);

                    DataRow hahaRow = reactionsDataTable.NewRow();
                    hahaRow["Id"] = post["id"];
                    hahaRow["Reaction Type"] = "Haha";
                    hahaRow["Count"] = Utility.ConvertToLong(post["reactions_haha"]["summary"]["total_count"]);

                    DataRow sadRow = reactionsDataTable.NewRow();
                    sadRow["Id"] = post["id"];
                    sadRow["Reaction Type"] = "Sad";
                    sadRow["Count"] = Utility.ConvertToLong(post["reactions_sad"]["summary"]["total_count"]);

                    DataRow loveRow = reactionsDataTable.NewRow();
                    loveRow["Id"] = post["id"];
                    loveRow["Reaction Type"] = "Love";
                    loveRow["Count"] = Utility.ConvertToLong(post["reactions_love"]["summary"]["total_count"]);

                    DataRow angryRow = reactionsDataTable.NewRow();
                    angryRow["Id"] = post["id"];
                    angryRow["Reaction Type"] = "Angry";
                    angryRow["Count"] = Utility.ConvertToLong(post["reactions_angry"]["summary"]["total_count"]);

                    DataRow wowRow = reactionsDataTable.NewRow();
                    wowRow["Id"] = post["id"];
                    wowRow["Reaction Type"] = "Wow";
                    wowRow["Count"] = Utility.ConvertToLong(post["reactions_wow"]["summary"]["total_count"]);

                    reactionsDataTable.Rows.Add(likeRow);
                    reactionsDataTable.Rows.Add(hahaRow);
                    reactionsDataTable.Rows.Add(sadRow);
                    reactionsDataTable.Rows.Add(loveRow);
                    reactionsDataTable.Rows.Add(angryRow);
                    reactionsDataTable.Rows.Add(wowRow);

                }
            }
        }
    }
}
