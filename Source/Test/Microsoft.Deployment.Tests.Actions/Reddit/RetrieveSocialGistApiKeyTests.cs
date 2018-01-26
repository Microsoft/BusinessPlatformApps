using Microsoft.Deployment.Actions.AzureCustom.Reddit;
using Microsoft.Deployment.Common;
using Microsoft.Deployment.Tests.Actions.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Deployment.Common.ActionModel;

namespace Microsoft.Deployment.Tests.Actions.Reddit
{
    [TestClass]
    public class RetrieveSocialGistApiKeyTests
    {

        // key from json using jsonpath
        [TestMethod]
        public void TestKeyFromJson_works()
        {
            var sample = "{\"response\":{\"Request\":{\"Actual\":{\"rt\":\"json\"}},\"Keys\":{\"Key\":[{\"Key\":\"THISISNOTAKEYTHISISATRIBUTE\",\"Description\":\"Microsoft PowerBI - DwayneTest1\",\"Enabled\":1,\"RequestsPerDay\":\"1000\",\"DataCounters\":{\"MaxMatches\":\"10000\",\"MaxDataSet\":\"100\",\"MaxResultsDays\":\"No Limit\"},\"DataSources\":{\"SelectedSources\":\"\",\"DedicatedContentProviderId\":245}}]},\"Timer\":[]}}";
            var retriever = new RetrieveSocialGistApiKey();
            var key = retriever.KeyFromJson(sample);
            Assert.AreEqual("THISISNOTAKEYTHISISATRIBUTE", key);
        }

        [TestMethod]
        public void TestKeyFromJson_malformed_json()
        {
            var sample = "I am not json";
            var retriever = new RetrieveSocialGistApiKey();
            var key = retriever.KeyFromJson(sample);
            Assert.IsNull(key);
        }

        [TestMethod]
        public void TestKeyFromJson_unexpected_json()
        {
            var sample = "{\"response\":{\"Request\":{\"Actual\":{\"rt\":\"json\"}},\"Keys\":{\"Key\":[{\"Frog\":\"THISISNOTAKEYTHISISATRIBUTE\",\"Description\":\"Microsoft PowerBI - DwayneTest1\",\"Enabled\":1,\"RequestsPerDay\":\"1000\",\"DataCounters\":{\"MaxMatches\":\"10000\",\"MaxDataSet\":\"100\",\"MaxResultsDays\":\"No Limit\"},\"DataSources\":{\"SelectedSources\":\"\",\"DedicatedContentProviderId\":245}}]},\"Timer\":[]}}";
            var retriever = new RetrieveSocialGistApiKey();
            var key = retriever.KeyFromJson(sample);
            Assert.IsNull(key);
        }

        // retrieve key tests (live against system, disabled)
        [Ignore]
        [TestMethod]
        public async Task TestRetrieveKey_works()
        {
            var uri = Constants.SocialGistProvisionKeyUrl;
            var username = Credential.Instance.SocialGist.SocialGistRedditUserName;
            var passphrase = Credential.Instance.SocialGist.SocialGistRedditPassphrase;

            var retriever = new RetrieveSocialGistApiKey();

            var description = retriever.DescriptionFromFields(
                "Frodo",
                "Baggins",
                "example@example.org",
                "Burglar",
                "Bag End Holdings, Inc.",
                false,
                "Rings and such"
            );

            var key = await retriever.RetrieveKey(
                uri, 
                username,
                passphrase,
                description
            );
            // now delete it by key
            if (key != null)
            {
                await CleanupKey(uri, username, passphrase, key);
            }

            Assert.IsNotNull(key);
        }

        private async Task CleanupKey(
            string uri,
            string username,
            string passphrase,
            string key
        )
        {
            try
            {
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);
                httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue(
                        "Basic",
                        Convert.ToBase64String(
                            Encoding.ASCII.GetBytes($"{username}:{passphrase}")
                        )
                    );
                var parameters = new Dictionary<string, string>
            {
                { "rt", "json" },
                { "filter_key", key }
            };
                var deletePayload = new FormUrlEncodedContent(parameters);
                httpRequestMessage.Content = deletePayload;
                using (var client = new HttpClient())
                {
                    var response = await client.SendAsync(httpRequestMessage);
                    response.EnsureSuccessStatusCode();
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Something went wrong when trying to delete the API key {key} from {uri}.  You should delete this manually using postman.", e);
            }
        }

    }
}
