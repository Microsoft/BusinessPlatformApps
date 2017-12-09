using System.ComponentModel.Composition;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.Deployment.Common.Model.Bpst;

namespace Microsoft.Deployment.Actions.AzureCustom.ActivityLogs
{
    [Export(typeof(IAction))]
    public class ToggleStreamAnalytics : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            BpstAzure ba = new BpstAzure(request.DataStore);

            AzureHttpClient ahc = new AzureHttpClient(ba.TokenAzure);

            string nameStreamAnalyticsJob = request.DataStore.GetValue("nameStreamAnalyticsJob");

            string url = $"https://management.azure.com/subscriptions/{ba.IdSubscription}/resourceGroups/{ba.NameResourceGroup}/providers/Microsoft.StreamAnalytics/streamingjobs/{nameStreamAnalyticsJob}/start?api-version=2015-10-01";

            string error = await ahc.Test(HttpMethod.Post, url);

            return error == null
                ? new ActionResponse(ActionStatus.Success)
                : new ActionResponse(ActionStatus.Failure, new ActionResponseExceptionDetail("StreamAnalyticsToggleFailure", error));
        }
    }
}