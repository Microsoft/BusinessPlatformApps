using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.WebServiceClient;
using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Actions.MsCrm
{
    [Export(typeof(IAction))]
    public class CrmGetEntityInitialCounts : BaseAction
    {
        public async override Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {

            var dataMovement = request.DataStore.GetValue("DataMovement");
            if(dataMovement == "Scribe")
            {
                return new ActionResponse(ActionStatus.Success);
            }

            string refreshToken = request.DataStore.GetJson("MsCrmToken")["refresh_token"].ToString();
            string organizationUrl = request.DataStore.GetValue("OrganizationUrl");
            Dictionary<string, string> entities = JsonConvert.DeserializeObject<Dictionary<string, string>>(request.DataStore.GetValue("Entities"));

            var crmToken = CrmTokenUtility.RetrieveCrmOnlineToken(refreshToken, request.Info.WebsiteRootUrl, request.DataStore, organizationUrl);

            Dictionary<string, int> initialCounts = new Dictionary<string, int>();

            var proxy = new OrganizationWebProxyClient(new Uri($"{organizationUrl}XRMServices/2011/Organization.svc/web"), true)
            {
                HeaderToken = crmToken["access_token"].ToString()
            };

            string count = string.Empty;
            int max = 0;

            foreach (var entry in entities)
            {
                try
                {
                             var xml = $@"
                        <fetch distinct='false' mapping='logical' aggregate='true'> 
                            <entity name='{entry.Key}'> 
                                 <attribute name = '{entry.Value}' alias = '{entry.Key}_count' aggregate = 'count'/>
                            </entity> 
                        </fetch>";

                    var fetchRequest = new ExecuteFetchRequest() { FetchXml = xml };

                    var result = (ExecuteFetchResponse)proxy.Execute(fetchRequest);

                    var xdoc = XDocument.Parse(result.FetchXmlResult);

                    count = xdoc.Descendants().First(e => e.Name == $"{entry.Key}_count").Value;
                }
                catch (Exception e)
                {
                    if(e.Message == $"The entity with a name = '{entry.Key}' was not found in the MetadataCache.")
                    {
                        return new ActionResponse(ActionStatus.Failure, null, e, "NotPSAInstance");
                    }

                    if (e.Message == "AggregateQueryRecordLimit exceeded. Cannot perform this operation.")
                    {
                        count = "-1";
                    }
                    else throw;
                }

                if(Convert.ToInt32(count) > max)
                {
                    max = Convert.ToInt32(count);
                }

                initialCounts.Add(entry.Key.ToLowerInvariant(), Convert.ToInt32(count));
            }
            var missingCounts = new List<string>();

            foreach (var pair in initialCounts)
            {
                if(pair.Value == -1)
                {
                    missingCounts.Add(pair.Key);
                }
            }

            foreach(var entry in missingCounts)
            {
                initialCounts[entry] = max;
            }

            request.DataStore.AddToDataStore("InitialCounts", JsonUtility.GetJObjectFromObject(initialCounts));
            return new ActionResponse(ActionStatus.Success);
        }
    }
}
