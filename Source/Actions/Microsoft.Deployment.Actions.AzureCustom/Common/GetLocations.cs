using System.ComponentModel.Composition;
using System.Dynamic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hyak.Common;
using Microsoft.Azure;
using Microsoft.Azure.Subscriptions;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;

namespace Microsoft.Deployment.Actions.AzureCustom.Common
{
    [Export(typeof(IAction))]
    public class GetLocations : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var token = request.DataStore.GetJson("AzureToken", "access_token");
            var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            CloudCredentials creds = new TokenCloudCredentials(token);
            SubscriptionClient client = new SubscriptionClient(creds);
            var locationsList = (await client.Subscriptions.ListLocationsAsync(subscription, new CancellationToken())).Locations.ToList();
            dynamic wrapper = new ExpandoObject();
            wrapper.value = locationsList;
            return new ActionResponse(ActionStatus.Success, wrapper);
        }
    }
}