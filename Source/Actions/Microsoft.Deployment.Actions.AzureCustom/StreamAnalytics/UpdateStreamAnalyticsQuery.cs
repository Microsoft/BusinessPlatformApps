using System.ComponentModel.Composition;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.Deployment.Common.Model.Bpst;
using Microsoft.Deployment.Common.Model.StreamAnalytics;

namespace Microsoft.Deployment.Actions.AzureCustom.StreamAnalytics
{
    [Export(typeof(IAction))]
    public class UpdateStreamAnalyticsQuery : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            BpstAzure ba = new BpstAzure(request.DataStore);

            AzureHttpClient ahc = new AzureHttpClient(ba.TokenAzure);

            string nameStreamAnalyticsJob = request.DataStore.GetValue("nameStreamAnalyticsJob");

            string query = "SELECT CreatedDate,ServiceName,RequestId,IPAddress,Operation,OperationId,Api,ApiId,Product,ProductId,SubscriptionName,SubscriptionId,Length\r\nINTO [TemplatesSQLRequest]\r\nFROM [APIMEventHub] TIMESTAMP BY CreatedDate\r\nWHERE Type = 'Request'\r\n\r\nSELECT CreatedDate,ServiceName,RequestId,StatusCode,StatusReason,Length\r\nINTO [TemplatesSQLResponse]\r\nFROM [APIMEventHub] TIMESTAMP BY CreatedDate\r\nWHERE Type = 'Response'\r\n\r\nSELECT CreatedDate,ServiceName,RequestId,Source,Reason,Message \r\nINTO [TemplatesSQLError]\r\nFROM [APIMEventHub] TIMESTAMP BY CreatedDate\r\nWHERE Type = 'Error'";

            string url = $"https://management.azure.com/subscriptions/{ba.IdSubscription}/resourceGroups/{ba.NameResourceGroup}/providers/Microsoft.StreamAnalytics/streamingjobs/{nameStreamAnalyticsJob}/transformations/Transformation?api-version=2015-10-01";

            StreamAnalyticsQueryPropertiesWrapper body = new StreamAnalyticsQueryPropertiesWrapper(query);

            string error = await ahc.Test(HttpMethod.Put, url, JsonUtility.Serialize(body));

            return error == null
                ? new ActionResponse(ActionStatus.Success)
                : new ActionResponse(ActionStatus.Failure, new ActionResponseExceptionDetail("StreamAnalyticsUpdateQueryFailure", error));
        }
    }
}