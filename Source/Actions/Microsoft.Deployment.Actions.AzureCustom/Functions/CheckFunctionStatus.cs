using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Dynamic;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.Deployment.Common.Model.Azure;

namespace Microsoft.Deployment.Actions.AzureCustom.Common
{
    [Export(typeof(IAction))]
    public class CheckFunctionStatus : BaseAction
    {
        public const int ATTEMPTS = 92;
        public const int STATUS = 4;
        public const int WAIT = 5000;

        public const string SUCCESS = "Deployment successful";

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            var location = request.DataStore.GetJson("SelectedLocation", "Name");
            var sitename = request.DataStore.GetValue("FunctionName");

            AzureHttpClient client = new AzureHttpClient(azureToken, subscription, resourceGroup);

            // Force sync
            dynamic obj = new ExpandoObject();
            obj.subscriptionId = subscription;
            obj.SiteId = new ExpandoObject();
            obj.SiteId.Name = sitename;
            obj.SiteId.ResourceGroup = resourceGroup;
            HttpResponseMessage result = await client.ExecuteGenericRequestWithHeaderAsync(HttpMethod.Post, "https://web1.appsvcux.ext.azure.com/websites/api/Websites/KuduSync", JsonUtility.GetJsonStringFromObject(obj));
            var resultBody = await result.Content.ReadAsStringAsync();

            return await IsReady(client, sitename) ? new ActionResponse(ActionStatus.Success) : new ActionResponse(ActionStatus.Failure);
        }

        private async Task<bool> IsReady(AzureHttpClient client, string sitename)
        {
            bool isReady = false;

            for (int i = 0; i < ATTEMPTS && !isReady; i++)
            {
                FunctionStatusWrapper statusWrapper = await client.RequestAzure<FunctionStatusWrapper>(HttpMethod.Get, $"/providers/Microsoft.Web/sites/{sitename}/deployments", "2016-08-01");

                if (statusWrapper != null && !statusWrapper.Value.IsNullOrEmpty())
                {
                    bool hasFinishedDeployment = true;

                    for (int j = 0; j < statusWrapper.Value.Count && hasFinishedDeployment; j++)
                    {
                        FunctionStatus status = statusWrapper.Value[j];

                        hasFinishedDeployment = status != null && status.Properties != null && !string.IsNullOrEmpty(status.Properties.LogUrl);

                        if (hasFinishedDeployment)
                        {
                            List<FunctionStatusLog> logs = await client.Request<List<FunctionStatusLog>>(HttpMethod.Get, status.Properties.LogUrl);

                            hasFinishedDeployment = !logs.IsNullOrEmpty();

                            if (hasFinishedDeployment)
                            {
                                bool isDeployed = false;

                                for (int k = 0; k < logs.Count && !isDeployed; k++)
                                {
                                    isDeployed = logs[k].Message.Contains(SUCCESS);
                                }

                                hasFinishedDeployment = isDeployed && status.Properties.Active && status.Properties.Complete &&
                                    status.Properties.EndTime != null && status.Properties.Status == STATUS;
                            }
                        }
                    }

                    if (hasFinishedDeployment)
                    {
                        FunctionWrapper functionWrapper = await client.RequestAzure<FunctionWrapper>(HttpMethod.Get, $"/providers/Microsoft.Web/sites/{sitename}/functions", "2016-08-01");

                        if (functionWrapper != null && !functionWrapper.Value.IsNullOrEmpty())
                        {
                            bool areFunctionsReady = true;

                            for (int j = 0; j < functionWrapper.Value.Count && areFunctionsReady; j++)
                            {
                                Function function = functionWrapper.Value[j];

                                areFunctionsReady = function != null && function.Properties != null && function.Properties.Config != null && !function.Properties.Config.Disabled;
                            }

                            isReady = areFunctionsReady;
                        }
                    }
                }

                if (!isReady)
                {
                    await Task.Delay(WAIT);
                }
            }

            return isReady;
        }
    }
}