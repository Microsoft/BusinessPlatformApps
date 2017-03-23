//-----------------------------------------------------------------------
// <copyright file="GetCMDEntities.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Threading.Tasks;

namespace Microsoft.Deployment.Actions.AzureCustom.CDM
{
    using System.ComponentModel.Composition;
    using System.Threading;
    using Microsoft.Azure;
    using Microsoft.Azure.Management.Resources;
    using Microsoft.Azure.Management.Resources.Models;
    using Microsoft.Deployment.Common.ActionModel;
    using Microsoft.Deployment.Common.Actions;
    using Microsoft.Deployment.Common.ErrorCode;
    using Microsoft.Deployment.Common.Helpers;
    using System.Net.Http;

    [Export(typeof(IAction))]
    public class GetObjID : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            AzureHttpClient client = new AzureHttpClient(azureToken);

            var response = await client.ExecuteGenericRequestWithHeaderAsync(HttpMethod.Post, "https://management.azure.com/providers/Microsoft.PowerApps/enroll?api-version=2016-11-01&id=@id", "{}");
            var responseString = await response.Content.ReadAsStringAsync();
            var responseParsed = JsonUtility.GetJsonObjectFromJsonString(responseString);
            var objectId = responseParsed["featuresEnabled"][0]["id"].ToString();

            int indexFrom = objectId.IndexOf("objectIds/") + "objectIds/".Length;
            int indexTo = objectId.LastIndexOf("/features");

            objectId = objectId.Substring(indexFrom, indexTo - indexFrom);
            request.DataStore.AddToDataStore("ObjectID", objectId, DataStoreType.Public);

            return new ActionResponse(ActionStatus.Success);
        }
    }
}
