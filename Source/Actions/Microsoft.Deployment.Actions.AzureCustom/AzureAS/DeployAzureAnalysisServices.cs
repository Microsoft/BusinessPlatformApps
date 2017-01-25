//using System.Collections.Generic;
//using System.ComponentModel.Composition;
//using System.IO;
//using System.Net.Http;
//using System.Threading.Tasks;
//using Microsoft.Azure;
//using Microsoft.Deployment.Common.ActionModel;
//using Microsoft.Deployment.Common.Actions;
//using Microsoft.Deployment.Common.Helpers;
//using Newtonsoft.Json.Linq;

//namespace Microsoft.Deployment.Actions.AzureCustom.AzureAS
//{
//    [Export(typeof(IAction))]
//    public class DeployAzureAnalysisServices : BaseAction
//    {
//        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
//        {
//            string azureToken = request.DataStore.GetJson("AzureToken")["access_token"].ToString();
//            string subscription = request.DataStore.GetJson("SelectedSubscription")["SubscriptionId"].ToString();
//            string resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");

//            string serverName = request.DataStore.GetValue("ASServerName") ?? "analysisserver-" + RandomGenerator.GetRandomLowerCaseCharacters(5);
//            string location = request.DataStore.GetValue("ASLocation") ?? "westcentralus";
//            string sku = request.DataStore.GetValue("ASSku") ?? "D1";
//            string admin = request.DataStore.GetValue("ASAdmin") ?? 
//                AzureUtility.GetEmailFromToken(request.DataStore.GetJson("AzureToken"));

//            SubscriptionCloudCredentials creds = new TokenCloudCredentials(subscription, azureToken);
//            AzureArmParameterGenerator param = new AzureArmParameterGenerator();
//            param.AddStringParam("name", serverName);
//            param.AddStringParam("location", location);
//            param.AddStringParam("sku", sku);
//            param.AddStringParam("admin", admin);

//            string armTemplatefilePath = File.ReadAllText(request.ControllerModel.SiteCommonFilePath + "/service/arm/NewAzureAS.json");
//            string template = AzureUtility.GetAzureArmParameters(armTemplatefilePath, param);
//            await AzureUtility.ValidateAndDeployArm(creds, resourceGroup, "ASDeployment", template);
//            await AzureUtility.WaitForArmDeployment(creds, resourceGroup, "ASDeployment");

//            AzureHttpClient client = new AzureHttpClient(azureToken, subscription, resourceGroup);
//            var response = await client.ExecuteWithSubscriptionAndResourceGroupAsync(HttpMethod.Get
//                , $"/providers/Microsoft.AnalysisServices/servers/{serverName}/"
//                , "2016-05-16"
//                , string.Empty
//                , new Dictionary<string, string>());

//            string responseBody = await response.Content.ReadAsStringAsync();
//            if (!response.IsSuccessStatusCode)
//            {
//                return new ActionResponse(ActionStatus.Failure);
//            }

//            JObject responseObj = JsonUtility.GetJObjectFromJsonString(responseBody);
//            request.DataStore.AddToDataStore("ASServerUrl", responseObj["properties"]["serverFullName"]);

//            return new ActionResponse(ActionStatus.Success, responseObj);
//        }
//    }
//}
