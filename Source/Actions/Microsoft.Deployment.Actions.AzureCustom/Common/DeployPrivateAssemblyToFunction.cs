using System.ComponentModel.Composition;
using System.Dynamic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.ErrorCode;
using Microsoft.Deployment.Common.Helpers;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Microsoft.Deployment.Actions.AzureCustom.Common
{
    [Export(typeof(IAction))]
    public class DeployPrivateAssemblyToFunction : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
 
            var functionName = request.DataStore.GetValue("FunctionName");
           

            AzureHttpClient client = new AzureHttpClient(azureToken, subscription, resourceGroup);
           
            HttpResponseMessage publishxml = await client.ExecuteWithSubscriptionAndResourceGroupAsync(HttpMethod.Post, "/providers/Microsoft.Web/sites/" +
                functionName + "/publishxml", "2015-02-01", string.Empty);
            var publishxmlfile = publishxml.Content.ReadAsStringAsync().Result;

            XDocument doc = XDocument.Parse(publishxmlfile);
            XElement xElement = doc.Element("publishData");

            if (xElement == null)
            {
                return new ActionResponse(ActionStatus.Failure, JsonUtility.GetJObjectFromStringValue(publishxmlfile), null, DefaultErrorCodes.DefaultErrorCode,
                        "Unable to get publisher profile");
            }

            var publishProfiles = xElement.Elements("publishProfile");
            var profile = publishProfiles.SingleOrDefault(p => p.Attribute("publishMethod").Value == "FTP");

            if (profile == null)
            {
                return new ActionResponse(ActionStatus.Failure, JsonUtility.GetJObjectFromStringValue(publishxmlfile), null, DefaultErrorCodes.DefaultErrorCode,
                    "Unable to find FTP profile");
            }

            var ftpServer = profile.Attribute("publishUrl").Value;
            var username = profile.Attribute("userName").Value;
            var password = profile.Attribute("userPWD").Value;
            FtpUtilityTest.UploadFileToServer(ftpServer, username, password, "/ArticleExtractor/bin/Microsoft.KnowledgeMining.MainArticleExtractor.dll", 
            request.ControllerModel.SiteCommonFilePath + "/Assemblies/Microsoft.KnowledgeMining.MainArticleExtractor.dll");

            return new ActionResponse(ActionStatus.Success, JsonUtility.GetEmptyJObject());
        }
    }
}
