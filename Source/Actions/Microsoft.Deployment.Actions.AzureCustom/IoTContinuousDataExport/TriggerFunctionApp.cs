using System;
using System.ComponentModel.Composition;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.AzureCustom.IoTContinuousDataExport
{
    [Export(typeof(IAction))]
    public class TriggerFunctionApp : BaseAction
    {
        // Make a web request to trigger the Azure functions to start building
        // because azure functions is not built and triggered sometime after deployment
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var functionAppName = request.DataStore.GetValue("functionName");

            var client = new AzureHttpClient(string.Empty);

            HttpResponseMessage result = default(HttpResponseMessage);
            var url = $"https://{functionAppName}.azurewebsites.net/";
            try
            {
                result = await client.ExecuteGenericRequestNoTokenAsync(HttpMethod.Get, url, string.Empty);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return new ActionResponse(ActionStatus.Success);
        }
    }
}