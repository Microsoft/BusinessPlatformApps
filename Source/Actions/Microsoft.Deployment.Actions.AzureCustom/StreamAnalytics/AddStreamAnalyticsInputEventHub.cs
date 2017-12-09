using System.ComponentModel.Composition;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.Deployment.Common.Model.Bpst;
using Microsoft.Deployment.Common.Model.StreamAnalytics;

namespace Microsoft.Deployment.Actions.AzureCustom.APIManagement
{
    [Export(typeof(IAction))]
    public class AddStreamAnalyticsInputEventHub : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            BpstAzure ba = new BpstAzure(request.DataStore);

            AzureHttpClient ahc = new AzureHttpClient(ba.TokenAzure);

            string aliasInput = request.DataStore.GetValue("nameStreamAnalyticsInputEventHub");
            string inputSerialization = request.DataStore.GetValue("inputSerialization") ?? "CSV";
            string keyPrimary = request.DataStore.GetValue("EventHubPrimaryKey");
            string nameEventHub = request.DataStore.GetValue("nameEventHub");
            string nameNamespace = request.DataStore.GetValue("nameNamespace");
            string nameStreamAnalyticsJob = request.DataStore.GetValue("nameStreamAnalyticsJob");

            string url = $"https://management.azure.com/subscriptions/{ba.IdSubscription}/resourceGroups/{ba.NameResourceGroup}/providers/Microsoft.StreamAnalytics/streamingjobs/{nameStreamAnalyticsJob}/inputs/{aliasInput}?api-version=2015-10-01";

            StreamAnalyticsInputPropertiesWrapper body = new StreamAnalyticsInputPropertiesWrapper(nameEventHub, nameNamespace, keyPrimary, inputSerialization);

            string error = await ahc.Test(HttpMethod.Put, url, JsonUtility.Serialize(body));

            return error == null
                ? new ActionResponse(ActionStatus.Success)
                : new ActionResponse(ActionStatus.Failure, new ActionResponseExceptionDetail("StreamAnalyticsSetInputFailure", error));
        }
    }
}