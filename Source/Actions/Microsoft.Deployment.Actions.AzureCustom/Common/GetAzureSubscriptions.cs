using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Dynamic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Hyak.Common;
using Microsoft.Azure;
using Microsoft.Azure.Subscriptions;
using Microsoft.Azure.Subscriptions.Models;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.AzureCustom.Common
{
    [Export(typeof(IAction))]
    public class GetAzureSubscriptions : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureToken", "access_token");

            // Show location selection on the page
            var showLocationsString = request.DataStore.GetValue("showLocations");
            
            // For certain service, it only exists in few regions so add the allowed location list
            var allowedLocations = request.DataStore.GetValue("allowedLocations");

            bool.TryParse(showLocationsString, out bool showLocations);

            string[] allowedLocationsList = null;
            if (showLocations && !string.IsNullOrWhiteSpace(allowedLocations))
            {
                allowedLocationsList = allowedLocations.Split(new char[] { ',', ';' }, System.StringSplitOptions.RemoveEmptyEntries);
            }

            CloudCredentials creds = new TokenCloudCredentials(azureToken);
            dynamic subscriptionWrapper = new ExpandoObject();

            List<Subscription> validSubscriptions = new List<Subscription>();

            using (SubscriptionClient client = new SubscriptionClient(creds))
            {
                SubscriptionListResult subscriptionList = await client.Subscriptions.ListAsync();

                foreach (Subscription s in subscriptionList.Subscriptions)
                {
                    if (s.State.EqualsIgnoreCase("Disabled") || s.State.EqualsIgnoreCase("Deleted"))
                        continue;

                    if (!showLocations)
                    {
                        validSubscriptions.Add(s);
                    }
                    else
                    {
                        var locationsList = (await client.Subscriptions.ListLocationsAsync(s.SubscriptionId, new CancellationToken())).Locations.ToList();
                        if (allowedLocationsList != null && allowedLocationsList.Length > 0)
                        {
                            locationsList = locationsList.Where(l => allowedLocationsList.Contains(l.Name, StringComparer.OrdinalIgnoreCase)).ToList();
                        }

                        var subscription = new SubscriptionWithLocations()
                        {
                            SubscriptionId = s.SubscriptionId,
                            DisplayName = s.DisplayName,
                            State = s.State,
                            SubscriptionPolicies = s.SubscriptionPolicies,
                            Locations = locationsList
                        };

                        validSubscriptions.Add(subscription);
                    }
                }

                subscriptionWrapper.value = validSubscriptions;
            }

            request.Logger.LogEvent("GetAzureSubscriptions-result", new Dictionary<string, string>() { { "Subscriptions", string.Join(",", validSubscriptions.Select(p => p.SubscriptionId)) } });
            return new ActionResponse(ActionStatus.Success, subscriptionWrapper);
        }
    }

    // Same subscription but added locations
    public class SubscriptionWithLocations : Subscription
    {
        public IList<Location> Locations { get; set; }
    }
}