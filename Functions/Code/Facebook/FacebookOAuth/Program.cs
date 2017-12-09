using Microsoft.IdentityModel.Clients.ActiveDirectory.Internal;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FacebookOAuth
{
    class Program
    {
        public const string clientId = "";
        public const string clientSecret = "";
        public const string redirectUri = "https://localhost/";
        public static string code = string.Empty;
        public string devCode = "";

        [STAThread]
        static void Main(string[] args)
        {
            code = GetToken(clientId, redirectUri).Result;
            var shortLivedAccessToken = GetAccessToken($"https://graph.facebook.com/oauth/access_token?client_id={clientId}&client_secret={clientSecret}&redirect_uri={redirectUri}&code={code}").Result;
            var longLivedAccessToken = GetAccessToken($"https://graph.facebook.com/oauth/access_token?grant_type=fb_exchange_token&client_id={clientId}&client_secret={clientSecret}&fb_exchange_token={shortLivedAccessToken}").Result;
            var id = GetUserId($"https://graph.facebook.com/v2.10/me?access_token={longLivedAccessToken}").Result;
            var pageToken = GetAccessToken($"https://graph.facebook.com/v2.10/{id}/accounts?access_token={longLivedAccessToken}").Result;
        }

        public static async Task<string> GetUserId(string uri)
        {
            string requestUri = uri;
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(requestUri);
            string responseObj = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception();
            }

            string id = JObject.Parse(responseObj)["id"].ToString();
            return id;
        }

        public static async Task<string> GetToken(string clientId, string redirectUri)
        {
            var url = $"https://www.facebook.com/v2.10/dialog/oauth?client_id={clientId}&redirect_uri={redirectUri}&response_type=code&granted_scopes=manage_pages,publish_pages";
            var code = string.Empty;
            WindowsFormsWebAuthenticationDialog form = new WindowsFormsWebAuthenticationDialog(null);
            form.WebBrowser.Navigated += delegate (object sender, WebBrowserNavigatedEventArgs args)
            {
                if (args.Url.ToString().StartsWith("https://localhost"))
                {
                    string tempcode = args.Url.ToString();
                    tempcode = tempcode.Substring(tempcode.IndexOf("code=") + 5);
                    code = tempcode;
                    form.Close();
                };
            };
            form.WebBrowser.Navigate(url);
            form.ShowBrowser();

            while (string.IsNullOrEmpty(code))
            {
                await Task.Delay(5000);
            }

            return code;
        }

        public static async Task<string> GetAccessToken(string uri)
        {
            string requestUri = uri;
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(requestUri);
            string responseObj = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception();
            }

            string accessToken = JObject.Parse(responseObj)["access_token"].ToString();
            return accessToken;
        }
    }
}
