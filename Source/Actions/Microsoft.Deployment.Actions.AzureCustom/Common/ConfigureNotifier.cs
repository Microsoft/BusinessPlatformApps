using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Deployment.Common;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using System.Data.SqlClient;

namespace Microsoft.Deployment.Actions.AzureCustom.Common
{
    [Export(typeof(IAction))]
    class ConfigureNotifier : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            string refreshToken = request.DataStore.GetJson("AzureToken", "refresh_token");
            string subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            string resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            string connectionString = request.DataStore.GetValue("SqlConnectionString");
            string sendNotification = request.DataStore.GetValue("SendCompletionNotification");
            Dictionary<string, string> configValues = new Dictionary<string, string>();

            if (!bool.Parse(sendNotification))
            {
                configValues = new Dictionary<string, string>()
                {
                    {"SendCompletionNotification", "0" },
                };

                CreatePayload(request, configValues);

                var resp = await RequestUtility.CallAction(request, "Microsoft-SetConfigValueInSql");

                if (resp.IsSuccess)
                {
                    return resp;
                }
            }


            string triggerUrl = request.DataStore.GetValue("NotifierTriggerUrl");
            string deploymentIdsConnection = Constants.BpstDeploymentIdDatabase;

            string deploymentId = Guid.NewGuid().ToString();
            string dataPullCompleteThreshold = "80";
            var asDisabled = request.DataStore.GetValue("ssasDisabled");

            configValues = new Dictionary<string, string>()
            {
                {"SendCompletionNotification", "1" },
                {"NotifierUrl", Constants.BpstNotifierUrl },
                {"NotificationEmails", request.DataStore.GetValue("EmailAddress") },
                {"DeploymentId", deploymentId },
                {"TemplateName", request.Info.AppName },
                {"DeploymentTimestamp", DateTime.UtcNow.ToString("o") },
                {"ASDeployment", string.IsNullOrEmpty(asDisabled) ? "False" : (!Convert.ToBoolean(asDisabled)).ToString()},
                {"DataPullCompleteThreshold", dataPullCompleteThreshold },
                {"DataPullStatus", "-1" }
            };

            CreatePayload(request, configValues);

            var configResponse = await RequestUtility.CallAction(request, "Microsoft-SetConfigValueInSql");

            if (!configResponse.IsSuccess)
            {
                return configResponse;
            }

            //OnPrem scenario
            if (request.Info.WebsiteRootUrl.Contains("https://msi"))
            {
                var post = PostDeploymentId(deploymentId, azureToken, refreshToken);
                if (!post)
                {
                    request.Logger.LogEvent("ConfigureNotifier failed for on prem scenario - couldn't reach service.", new Dictionary<string, string>());
                }
            }
            else
            {
                //Website scenario
                SqlParameter[] parameters = SqlUtility.MapValuesToSqlParameters(deploymentId, DateTime.UtcNow);
                SqlUtility.ExecuteQueryWithParameters(deploymentIdsConnection, "INSERT INTO deploymentids VALUES(@p1, @p2)", parameters);
            }

            AzureHttpClient azureClient = new AzureHttpClient(azureToken, subscription, resourceGroup);
            var response = await azureClient.ExecuteGenericRequestNoHeaderAsync(HttpMethod.Post, triggerUrl, string.Empty);

            return new ActionResponse(ActionStatus.Success);
        }

        private void CreatePayload(ActionRequest request, Dictionary<string, string> configValues)
        {
            for (int i = 0; i < configValues.Count; i++)
            {

                dynamic payload = new ExpandoObject();

                payload.SqlGroup = "SolutionTemplate";
                payload.SqlSubGroup = "Notifier";
                payload.SqlEntryName = configValues.ElementAt(i).Key;
                payload.SqlEntryValue = configValues.ElementAt(i).Value;

                request.DataStore.AddToDataStore("NotifierValues" + i, "SqlGroup", "SolutionTemplate");
                request.DataStore.AddToDataStore("NotifierValues" + i, "SqlSubGroup", "Notifier");
                request.DataStore.AddToDataStore("NotifierValues" + i, "SqlEntryName", configValues.ElementAt(i).Key);
                request.DataStore.AddToDataStore("NotifierValues" + i, "SqlEntryValue", configValues.ElementAt(i).Value);
            }
        }

        private bool PostDeploymentId(string deploymentId, string accessToken, string refreshToken)
        {
            HttpResponseMessage response;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(Constants.ServiceUrl);

                dynamic payload = new ExpandoObject();
                payload.tokens = new ExpandoObject();
                payload.tokens.access = accessToken;
                payload.tokens.refresh = refreshToken;
                payload.deploymentId = deploymentId;

                response = client.PostAsync("/api/notifier", new StringContent(JsonUtility.GetJObjectFromObject(payload).ToString(), Encoding.UTF8, "application/json")).Result;
            }

            return response.IsSuccessStatusCode;
        }
    }
}