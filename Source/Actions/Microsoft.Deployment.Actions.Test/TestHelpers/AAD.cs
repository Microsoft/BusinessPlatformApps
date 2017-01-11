using Microsoft.Deployment.Common;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.IdentityModel.Clients.ActiveDirectory.Internal;

namespace Microsoft.Deployment.Actions.Test.TestHelpers
{
    public class AAD
    {
        public string ClientID { get; set; }

        public string ClientSecret { get; set; }

        public string TenantId { get; set; }

        public static async Task<DataStore> GetTokenWithDataStore()
        {
            if (DataStoreWithToken == null)
            {
                ClientCredential cred = new ClientCredential(Credential.Instance.AAD.ClientID, Credential.Instance.AAD.ClientSecret);

                AuthenticationContext context = new AuthenticationContext("https://login.windows.net/" + Credential.Instance.AAD.TenantId);
                var token = await context.AcquireTokenAsync(Constants.AzureManagementCoreApi, cred);
                dynamic tokenObj = new ExpandoObject();
                tokenObj.access_token = token.AccessToken;
                DataStore datastore = new DataStore();
                datastore.AddToDataStore("AzureToken", JObject.FromObject(tokenObj), DataStoreType.Private);

                DataStoreWithToken = datastore;

            }

            return DataStoreWithToken;
        }

        private static DataStore DataStoreWithToken { get; set; }

//        public static async Task<DataStore> GetUserTokenFromPopup()
//        {
//            dynamic tokenObj = new ExpandoObject();
//#if DEBUG
//            AuthenticationContext context = new AuthenticationContext("https://login.windows.net/" + Credential.Instance.AAD.TenantId);
//            var token = await context.AcquireTokenAsync(Constants.AzureManagementCoreApi, Constants.MicrosoftClientId, new Uri("https://unittest/redirect.html"), new PlatformParameters(PromptBehavior.Auto
//                ));
//            tokenObj.access_token = token.AccessToken;
//#endif

//            DataStore datastore = new DataStore();
//            datastore.AddToDataStore("AzureToken", JObject.FromObject(tokenObj), DataStoreType.Private);
//            return datastore;
//        }

        private static string code = string.Empty;
        public static async Task<DataStore> GetUserTokenFromPopup()
        {
#if DEBUG
            AuthenticationContext context = new AuthenticationContext("https://login.windows.net/" + Credential.Instance.AAD.TenantId);
            var url = context.GetAuthorizationRequestUrlAsync(Constants.AzureManagementCoreApi, Constants.MicrosoftClientId, new Uri("https://unittest/redirect.html"), UserIdentifier.AnyUser, "").Result;
            WindowsFormsWebAuthenticationDialog form = new WindowsFormsWebAuthenticationDialog(null);
            form.WebBrowser.Navigated += delegate (object sender, WebBrowserNavigatedEventArgs args)
            {
                if (args.Url.ToString().StartsWith("https://unittest/redirect.html"))
                {
                    string tempcode = args.Url.ToString();
                    tempcode = tempcode.Substring(tempcode.IndexOf("code=") + 5);
                    code = tempcode.Substring(0, tempcode.IndexOf("&"));
                    form.Close();
                };
            };
            form.WebBrowser.Navigate(url);
            form.ShowBrowser();

            while (string.IsNullOrEmpty(code))
            {

                await Task.Delay(5000);
            }
#endif

            DataStore datastore = new DataStore();
            datastore.AddToDataStore("code", code, DataStoreType.Private);
            datastore.AddToDataStore("AADTenant", "common", DataStoreType.Private);
            datastore.AddToDataStore("AADRedirect", "https://unittest/redirect.html");
            var result = await TestHarness.ExecuteActionAsync("Microsoft-GetAzureToken", datastore);

            return result.DataStore;
        }
    }
}
