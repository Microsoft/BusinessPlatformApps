using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Deployment.Actions.Salesforce.Helpers;
using Microsoft.Deployment.Actions.Salesforce.SalesforceSOAP;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using Newtonsoft.Json;

namespace Microsoft.Deployment.Actions.Salesforce
{
    [Export(typeof(IAction))]
    public class SalesforceGetEntityInitialCounts : BaseAction
    {
        public async override Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string objects = request.DataStore.GetValue("ObjectTables");
            string sfUsername = request.DataStore.GetValue("SalesforceUser");
            string sfPassword = request.DataStore.GetValue("SalesforcePassword");
            string sfToken = request.DataStore.GetValue("SalesforceToken");
            string sfTestUrl = request.DataStore.GetValue("SalesforceUrl");

            List<string> sfObjects = objects.Split(',').ToList();
            Dictionary<string, int> initialCounts = new Dictionary<string, int>();

            SoapClient binding = new SoapClient("Soap");

            if (!string.IsNullOrEmpty(sfTestUrl) && sfTestUrl.Contains("test"))
            {
                binding.Endpoint.Address = new System.ServiceModel.EndpointAddress(binding.Endpoint.Address.ToString().Replace("login", "test"));
            }

            LoginResult lr;

            SecurityHelper.SetTls12();

            binding.ClientCredentials.UserName.UserName = sfUsername;
            binding.ClientCredentials.UserName.Password = sfPassword;

            lr =
               binding.login(null, null,
               sfUsername,
               string.Concat(sfPassword, sfToken));

            binding = new SoapClient("Soap");

            SessionHeader sheader = new SessionHeader();
            BasicHttpBinding bind = new BasicHttpBinding();
            bind = (BasicHttpBinding)binding.Endpoint.Binding;
            bind.MaxReceivedMessageSize = int.MaxValue;
            bind.MaxBufferPoolSize = int.MaxValue;
            bind.MaxBufferSize = int.MaxValue;
            bind.CloseTimeout = new TimeSpan(0, 0, 5, 0);
            bind.OpenTimeout = new TimeSpan(0, 0, 5, 0);
            bind.SendTimeout = new TimeSpan(0, 0, 5, 0);
            bind.ReaderQuotas.MaxArrayLength = int.MaxValue;
            bind.ReaderQuotas.MaxDepth = int.MaxValue;
            bind.ReaderQuotas.MaxNameTableCharCount = int.MaxValue;
            bind.ReaderQuotas.MaxStringContentLength = int.MaxValue;
            bind.ReaderQuotas.MaxBytesPerRead = int.MaxValue;
            bind.ReaderQuotas.MaxNameTableCharCount = int.MaxValue;

            binding.Endpoint.Binding = bind;
            binding.Endpoint.Address = new EndpointAddress(lr.serverUrl);

            sheader.sessionId = lr.sessionId;

            binding.Endpoint.ListenUri = new Uri(lr.metadataServerUrl);

            foreach (var obj in sfObjects)
            {
                try
                {
                    QueryResult result;
                    binding.query(sheader, null, null, null, null,
                        $"SELECT COUNT() FROM {obj} " +
                        $"WHERE LastModifiedDate > {DateTime.UtcNow.AddYears(-3).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture)} " +
                        $"AND LastModifiedDate <= {DateTime.UtcNow.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture)}",
                        out result);
                    initialCounts.Add(obj.ToLower(), result.size);
                }
                catch (Exception e)
                {
                    return new ActionResponse(ActionStatus.Failure, JsonUtility.GetEmptyJObject(), e, "SalesforceInvalidType");
                }
            }

            request.DataStore.AddToDataStore("InitialCounts", JsonUtility.GetJObjectFromObject(initialCounts));
            return new ActionResponse(ActionStatus.Success);
        }
    }
}
