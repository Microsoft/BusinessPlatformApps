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
    public class CreateCDMEntity : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureToken")["access_token"].ToString();
            var environId = request.DataStore.GetValue("EnvironmentID").ToString();
            var entityName = request.DataStore.GetValue("EntityName").ToString();
            var namespaceID = environId;

            var cdmEntity = System.IO.File.ReadAllText(System.IO.Path.Combine(request.ControllerModel.SiteCommonFilePath, "Service/Arm/sampleCDMEntity.json"));
            //var cdmEntity = JsonUtility.GetJObjectFromJsonString(System.IO.File.ReadAllText(System.IO.Path.Combine(request.ControllerModel.SiteCommonFilePath, "Service/Arm/sampleCDMEntity.json")));
            if (environId.Contains("Legacy"))
            {
                int indexFrom = environId.IndexOf("Legacy-") + "Legacy-".Length;
                int indexTo = environId.Length;

                namespaceID = environId.Substring(indexFrom, indexTo - indexFrom);
            }

            AzureHttpClient client = new AzureHttpClient(azureToken);

            var checkEntities = await RequestUtility.CallAction(request, "Microsoft-CheckCDMEntities");
            if (!checkEntities.IsSuccess)
            {
                return new ActionResponse(ActionStatus.FailureExpected);
            }

            var response = await client.ExecuteGenericRequestWithHeaderAsync(HttpMethod.Put, $"https://management.azure.com/providers/Microsoft.CommonDataModel/environments/{environId}/namespaces/{namespaceID}/entities/{entityName}_?api-version=2016-11-01", cdmEntity);
            var responseString = await response.Content.ReadAsStringAsync();


            return new ActionResponse(ActionStatus.Success);
        }
    }
}
