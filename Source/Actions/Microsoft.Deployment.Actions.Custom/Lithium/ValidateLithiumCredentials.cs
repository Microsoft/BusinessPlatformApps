using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.ErrorResources;
using Newtonsoft.Json.Linq;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Microsoft.Deployment.Actions.Custom.Lithium
{
    [Export(typeof(IAction))]
    public class ValidateLithiumCredentials : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string tenantId = request.DataStore.GetValue("LithiumTenantId");
            string clientId = request.DataStore.GetValue("LithiumClientId");
            string clientSecret = request.DataStore.GetValue("LithiumClientSecret");
            string refreshToken = request.DataStore.GetValue("LithiumRefreshToken");

            string strTokenUrl = "https://api.lithium.com/auth/v1/refreshToken";
            string strAccessToken = string.Empty;

            HttpWebRequest webRequest = WebRequest.Create(strTokenUrl) as HttpWebRequest;
            webRequest.Method = "POST";
            webRequest.ContentType = "application/json";
            webRequest.Headers.Add("client-id", clientId);
            webRequest.Headers.Add("client_secret", clientSecret);
            webRequest.Headers.Add("cache-control", "no-cache");

            using (var streamWriter = new StreamWriter(webRequest.GetRequestStream()))
            {
                string json = "{ \"client_id\" : \"" + clientId + "\", \"client_secret\" : \"" + clientSecret + "\", \"grant_type\" : \"refresh_token\",\"refresh_token\":\"" + refreshToken + "\" }";

                streamWriter.Write(json);
                streamWriter.Flush();
            }

            try
            {
                using (var httpResponse = webRequest.GetResponse() as HttpWebResponse)
                {
                    using (var reader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var responseObj = JObject.Parse(reader.ReadToEnd());
                        if (responseObj.HasValues)
                        {
                            if (responseObj.Children().FirstOrDefault().FirstOrDefault()["data"] != null)
                            {
                                strAccessToken = responseObj.Children().FirstOrDefault().FirstOrDefault()["data"].SelectToken("access_token").ToString();

                                var strTitle = GetLithiumCommunityName(tenantId, clientId, strAccessToken);

                               return strTitle == string.Empty ? new ActionResponse(ActionStatus.Failure, null, EnglishErrorCodes.LithiumCredentialsInvalid) : new ActionResponse(ActionStatus.Success, strTitle);
                            }
                        }
                    }
                }
            }
            catch
            {
                return new ActionResponse(ActionStatus.Failure, null, EnglishErrorCodes.LithiumCredentialsInvalid);
            }

            return new ActionResponse(ActionStatus.Success, strAccessToken);
        }

        //Returns the community title 
        private string GetLithiumCommunityName(string strTenantID, string strClientID, string strAccessToken)
        {
            string strQueryURL = $"https://api.lithium.com/community/2.0/{strTenantID}/search?q=select * from communities";
            string strTitle = string.Empty;

            HttpWebRequest webRequest3 = WebRequest.Create(strQueryURL) as HttpWebRequest;
            webRequest3.Method = "GET";
            webRequest3.ContentType = "application/json";
            webRequest3.Headers.Add("client-id", strClientID);
            webRequest3.Headers.Add("Authorization", " Bearer " + strAccessToken);
            string responseString = string.Empty;

            using (var httpResponse = webRequest3.GetResponse() as HttpWebResponse)
            {
                using (var reader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    responseString = reader.ReadToEnd();
                }

                var responseObj = JObject.Parse(responseString);

                if (responseObj.HasValues)
                {
                    var jData = responseObj["data"];

                    if (jData.HasValues)
                    {
                        var jItems = jData["items"];

                        if (jItems.FirstOrDefault().SelectToken("title") != null)
                        {
                            strTitle = jItems.FirstOrDefault().SelectToken("title").ToString();
                        }
                    }
                }
            }           

            return strTitle;
        }


    }
}