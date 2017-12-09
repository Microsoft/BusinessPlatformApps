using Microsoft.Azure;
using Microsoft.Azure.Management.Resources;
using Microsoft.Azure.Management.Resources.Models;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Enums;

using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Deployment.Actions.AzureCustom.Common
{
    [Export(typeof(IAction))]
    public class CreateResourceGroup : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            var location = request.DataStore.GetJson("SelectedLocation", "Name");

            SubscriptionCloudCredentials creds = new TokenCloudCredentials(subscription, azureToken);
            Microsoft.Azure.Management.Resources.ResourceManagementClient client = new ResourceManagementClient(creds);
            /*var existsResourceGroup = await client.ResourceGroups.CheckExistenceAsync(resourceGroup, new CancellationToken());
            if (existsResourceGroup.Exists)
            {
                //show error on page saying please pick another RG name
                return new ActionResponse(ActionStatus.Failure, new ArgumentException("Resource Group " + resourceGroup + " already exists."), "ResourceGroupAlreadyExists");
            }*/

            var locationParam = new ResourceGroup { Location = location };
            var createdResourceGroup = await client.ResourceGroups.CreateOrUpdateAsync(resourceGroup, locationParam, new CancellationToken());

            request.Logger.LogResource(request.DataStore, resourceGroup, DeployedResourceType.ResourceGroup, CreatedBy.BPST, DateTime.UtcNow.ToString("o"), createdResourceGroup.ResourceGroup.Id);

            return new ActionResponse(ActionStatus.Success, createdResourceGroup);
        }
    }
}