using System.ComponentModel.Composition;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.Deployment.Common.Model.Bpst;
using Microsoft.Deployment.Common.Model.EventHub;

namespace Microsoft.Deployment.Actions.AzureCustom.APIManagement
{
    [Export(typeof(IAction))]
    public class GetEventHubKeys : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            BpstAzure ba = new BpstAzure(request.DataStore);

            AzureHttpClient ahc = new AzureHttpClient(ba.TokenAzure);

            string nameNamespace = request.DataStore.GetValue("nameNamespace");

            string url = $"https://management.azure.com/subscriptions/{ba.IdSubscription}/resourceGroups/{ba.NameResourceGroup}/providers/Microsoft.EventHub/namespaces/{nameNamespace}/AuthorizationRules/RootManageSharedAccessKey/listkeys?api-version=2014-09-01";

            EventHubKeys eventHubKeys = await ahc.Request<EventHubKeys>(HttpMethod.Post, url);

            if (eventHubKeys != null)
            {
                request.DataStore.AddToDataStore("EventHubPrimaryConnectionString", eventHubKeys.PrimaryConnectionString, DataStoreType.Private);
                request.DataStore.AddToDataStore("EventHubPrimaryKey", eventHubKeys.PrimaryKey, DataStoreType.Private);
            }

            return eventHubKeys == null
                ? new ActionResponse(ActionStatus.Failure, new ActionResponseExceptionDetail("EventHubFailedToQueryKeys"))
                : new ActionResponse(ActionStatus.Success);
        }
    }
}