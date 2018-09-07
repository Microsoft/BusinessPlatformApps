using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Controller;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.WebServiceClient;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Net;
using System.Threading.Tasks;

namespace Microsoft.Deployment.Common.Actions.MsCrm
{
    [Export(typeof(IAction))]
    class CrmValidateEntities : BaseAction
    {
        private int maxRetries = 3;

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.CheckCertificateRevocationList = true;

            string refreshToken = request.DataStore.GetJson("MsCrmToken")["refresh_token"].ToString();
            string organizationUrl = request.DataStore.GetValue("OrganizationUrl");
            Dictionary<string, string> entities = JsonConvert.DeserializeObject<Dictionary<string, string>>(request.DataStore.GetValue("Entities"));

            var crmToken = CrmTokenUtility.RetrieveCrmOnlineToken(refreshToken, request.Info.WebsiteRootUrl, request.DataStore, organizationUrl);

            var proxy = new OrganizationWebProxyClient(new Uri($"{organizationUrl}XRMServices/2011/Organization.svc/web"), true)
            {
                HeaderToken = crmToken["access_token"].ToString()
            };

            Dictionary<string, bool> entitiesToReprocess = new Dictionary<string, bool>();

            bool retryNeeded = true;
            for (int i = 0; i < maxRetries && retryNeeded; i++)
            {
                try
                {
                    Parallel.ForEach(entities, (e) => { this.CheckAndUpdateEntity(e.Key, proxy, request.Logger); });

                    retryNeeded = false;
                }
                catch (AggregateException aex)
                {
                    string output = string.Empty;
                    foreach (var ex in aex.InnerExceptions)
                    {
                        output += ex.Message + $"[{ex.Data["entity"]}]. ";
                    }

                    return new ActionResponse(ActionStatus.Failure, JsonUtility.GetJObjectFromObject(aex), aex, "DefaultErrorCode", output);
                }
                catch (Exception e)
                {
                    return new ActionResponse(ActionStatus.Failure, JsonUtility.GetJObjectFromObject(e), e, "DefaultErrorCode", e.Message);
                }
            }

            return new ActionResponse(ActionStatus.Success);
        }

        public void CheckAndUpdateEntity(string entity, OrganizationWebProxyClient proxy, Logger logger)
        {
            var checkRequest = new RetrieveEntityRequest()
            {
                LogicalName = entity,
                EntityFilters = EntityFilters.Entity
            };


            RetrieveEntityResponse checkResponse = (RetrieveEntityResponse)proxy.Execute(checkRequest);

            // Check if entity exists
            if (checkResponse == null || checkResponse.EntityMetadata == null)
            {
                logger.LogCustomProperty("PSAEntity", $"The {entity} entity cannot be retrieved from the PSA instance.");
                throw new Exception($"The {entity} entity cannot be retrieved from the PSA instance.");
            }

            // Check if change tracking enabled
            if (!(bool)checkResponse.EntityMetadata.ChangeTrackingEnabled)
            {
                // Check if change tracking can be enabled
                if (!checkResponse.EntityMetadata.CanChangeTrackingBeEnabled.Value)
                {
                    logger.LogCustomProperty("PSAEntity", $"The {entity} entity cannot be enabled for change tracking.");
                    throw new Exception($"The {entity} entity cannot be enabled for change tracking.");
                }
                else
                {
                    UpdateEntityRequest updateRequest = new UpdateEntityRequest() { Entity = checkResponse.EntityMetadata };
                    updateRequest.Entity.ChangeTrackingEnabled = true;

                    UpdateEntityResponse updateResponse = (UpdateEntityResponse)proxy.Execute(updateRequest);

                    // Check the entity has actually been change tracking enabled
                    RetrieveEntityResponse verifyChange = (RetrieveEntityResponse)proxy.Execute(checkRequest);
                    if (!(bool)verifyChange.EntityMetadata.ChangeTrackingEnabled)
                    {
                        logger.LogCustomProperty("PSAEntity", $"Warning: Change tracking for {entity} has been enabled, but is not yet active.");
                    }
                }
            }
        }
    }
}
