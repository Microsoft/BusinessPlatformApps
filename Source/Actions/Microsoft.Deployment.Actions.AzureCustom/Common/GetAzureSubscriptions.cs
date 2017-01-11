using Hyak.Common;
using Microsoft.Azure;
using Microsoft.Azure.Subscriptions;
using Microsoft.Azure.Subscriptions.Models;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Deployment.Actions.AzureCustom.Common
{
    [Export(typeof(IAction))]
    public class GetAzureSubscriptions : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureToken")["access_token"].ToString();

            CloudCredentials creds = new TokenCloudCredentials(azureToken);
            dynamic subscriptionWrapper = new ExpandoObject();

            List<Subscription> validSubscriptions = new List<Subscription>();

            using (SubscriptionClient client = new SubscriptionClient(creds))
            {
                SubscriptionListResult subscriptionList = await client.Subscriptions.ListAsync();

                foreach (Subscription s in subscriptionList.Subscriptions)
                {
                    if (s.State.Equals("Disabled", System.StringComparison.OrdinalIgnoreCase) || s.State.Equals("Deleted", System.StringComparison.OrdinalIgnoreCase))
                        continue;
                    else
                        validSubscriptions.Add(s);
                }
                
                subscriptionWrapper.value = validSubscriptions;
            }

            request.Logger.LogEvent("GetAzureSubscriptions-result", new Dictionary<string, string>() {  { "Subscriptions", string.Join(",", validSubscriptions.Select(p => p.SubscriptionId)) }  });
            return new ActionResponse(ActionStatus.Success, subscriptionWrapper);
        }
    }
}