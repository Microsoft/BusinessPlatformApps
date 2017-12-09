using System;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.IdentityModel.Clients.ActiveDirectory.Internal;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Actions.AzureCustom.AzureToken;

namespace Microsoft.Deployment.Tests.Actions.TestHelpers
{
    public class AAD
    {
        public string TenantId { get; set; }

        private static string code = string.Empty;
        public static async Task<DataStore> GetUserTokenFromPopup(string openAuthorizationType = "")
        {
#if DEBUG
            AuthenticationContext context = new AuthenticationContext("https://login.windows.net/" + "common");
            AzureTokenRequestMeta meta = AzureTokenUtility.GetMetaFromOAuthType(openAuthorizationType);

            var url = context.GetAuthorizationRequestUrlAsync(meta.Resource, meta.ClientId, new Uri("https://unittest/redirect.html"), UserIdentifier.AnyUser, "prompt=consent").Result;
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
            datastore.AddToDataStore("oauthType", openAuthorizationType);
            var result = await TestManager.ExecuteActionAsync("Microsoft-GetAzureToken", datastore);

            return result.DataStore;
        }
    }
}