using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Dynamic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

using Microsoft.Deployment.Actions.Salesforce.Helpers;
using Microsoft.Deployment.Actions.Salesforce.SalesforceSOAP;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.Salesforce
{
    public static class SalesforceUtility
    {
        public static async Task<List<DescribeSObjectResult>> GetMetadata(ActionRequest request)
        {
            string objects = request.DataStore.GetValue("ObjectTables");
            string sfUsername = request.DataStore.GetValue("SalesforceUser");
            string sfPassword = request.DataStore.GetValue("SalesforcePassword");
            string sfToken = request.DataStore.GetValue("SalesforceToken");
            string sfTestUrl = request.DataStore.GetValue("SalesforceUrl");
            var additionalObjects = request.DataStore.GetValue("AdditionalObjects");

            List<string> sfObjects = objects.SplitByCommaSpaceTabReturnList();
            var metadata = new List<DescribeSObjectResult>();

            if (!string.IsNullOrEmpty(additionalObjects))
            {
                var add = additionalObjects.SplitByCommaSpaceTabReturnList();
                sfObjects.AddRange(add);
            }

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
                DescribeSObjectResult sobject;

                binding.describeSObject(sheader, null, null, null, obj, out sobject);

                var trimObject = new DescribeSObjectResult();
                trimObject.fields = sobject.fields;
                trimObject.name = sobject.name;

                metadata.Add(trimObject);
            }

            return metadata;
        }
    }
}