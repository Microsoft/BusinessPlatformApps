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
    public class CheckCDMEntities : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureToken")["access_token"].ToString();
            var environId = request.DataStore.GetValue("EnvironmentID").ToString();
            var entityName = request.DataStore.GetValue("EntityName").ToString();
            var namespaceID = environId;

            if (environId.Contains("Legacy"))
            {
                int indexFrom = environId.IndexOf("Legacy-") + "Legacy-".Length;
                int indexTo = environId.Length;

                namespaceID = environId.Substring(indexFrom, indexTo - indexFrom);
            }

            AzureHttpClient client = new AzureHttpClient(azureToken);

            var response = await client.ExecuteGenericRequestWithHeaderAsync(HttpMethod.Get, $"https://management.azure.com/providers/Microsoft.CommonDataModel/environments/{environId}/namespaces/{namespaceID}/entities?api-version=2016-11-01&$expand=namespace", "{}");
            var responseString = await response.Content.ReadAsStringAsync();
            var responseParsed = JsonUtility.GetJsonObjectFromJsonString(responseString);

            foreach (var obj in responseParsed["value"])
            {
                if (entityName == obj["name"].ToString())
                {
                    return new ActionResponse(ActionStatus.FailureExpected);
                }
            }
            return new ActionResponse(ActionStatus.Success);
        }
    }
}
