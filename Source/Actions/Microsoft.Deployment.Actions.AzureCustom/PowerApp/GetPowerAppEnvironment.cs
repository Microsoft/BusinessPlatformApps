using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.Deployment.Common.Model.PowerApp;

namespace Microsoft.Deployment.Actions.AzureCustom.PowerApp
{
    [Export(typeof(IAction))]
    public class GetPowerAppEnvironment : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            AzureHttpClient ahc = new AzureHttpClient(request.DataStore.GetJson("AzureToken", "access_token"));

            List<PowerAppEnvironment> environments = await ahc.RequestValue<List<PowerAppEnvironment>>(HttpMethod.Get, PowerAppUtility.URL_POWERAPPS_ENVIRONMENTS);

            bool foundEnvironment = false;

            if (environments.IsNullOrEmpty())
            {
                PowerAppUtility.SkipPowerApp(request.DataStore);
                foundEnvironment = true;
            }
            else
            {
                for (int i = 0; i < environments.Count && !foundEnvironment; i++)
                {
                    PowerAppEnvironment environment = environments[i];
                    if (environment.Properties != null && environment.Properties.IsDefault && environment.Properties.Permissions != null && environment.Properties.Permissions.CreatePowerApp != null)
                    {
                        request.DataStore.AddToDataStore("PowerAppEnvironment", environment.Name, DataStoreType.Private);
                        foundEnvironment = true;
                    }
                }
            }

            return foundEnvironment
                ? new ActionResponse(ActionStatus.Success)
                : new ActionResponse(ActionStatus.Failure, JsonUtility.GetEmptyJObject(), "PowerAppNoEnvironment");
        }
    }
}