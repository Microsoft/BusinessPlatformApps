using Microsoft.ApplicationInsights;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Helpers;

using System;
using System.Collections.Generic;
using Microsoft.Deployment.Common.Enums;

namespace Microsoft.Deployment.Common.Controller
{
    public class Logger
    {
        private TelemetryClient telemetryClient;

        private Dictionary<string, string> globalParams;

        private UserInfo Info { get; }
        public CommonControllerModel CommonControllerModel { get; }

        public Logger(UserInfo info, CommonControllerModel commonControllerModel)
        {
            this.Info = info;
            this.CommonControllerModel = commonControllerModel;
            this.telemetryClient = new TelemetryClient { InstrumentationKey = Constants.AppInsightsKey };

            this.telemetryClient.Context.Operation.Id = info.OperationId;
            this.telemetryClient.Context.User.Id = info.UserId;
            this.telemetryClient.Context.Session.Id = info.SessionId;
            this.telemetryClient.Context.Operation.Name = info.ActionName;

            if (this.globalParams == null)
            {
                this.globalParams = new Dictionary<string, string>();
            }

            this.globalParams.Add("TemplateName", info.AppName);
            this.globalParams.Add("UserGenId", info.UserGenId);
            this.globalParams.Add("EntryPointAction", info.ActionName);
            this.globalParams.Add("OriginatingSource", commonControllerModel.Source);
            this.globalParams.Add("LinkedOperationId", info.OperationId);
            this.globalParams.Add("UniqueLink", info.UniqueLink);
            this.globalParams.Add("Build", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());

            this.AddToDictionary(this.globalParams, this.telemetryClient.Context.Properties);
        }

        public void LogEvent(string eventName, Dictionary<string, string> properties = null)
        {
            this.telemetryClient.TrackEvent(eventName, properties);
        }

        public void LogCustomProperty(string propKey, string propValue)
        {
            Dictionary<string, string> customProperty = new Dictionary<string, string>();
            customProperty.Add("Custom Property Key", propKey);
            customProperty.Add("Custom Property Value", propValue);
            this.LogEvent("Custom-Property", customProperty);
        }

        public void LogMetric(string metricName, double value, Dictionary<string, string> properties)
        {
            //this.telemetryClient.TrackMetric(metricName, value, properties);
        }

        public void LogPageView(string page)
        {
            this.telemetryClient.TrackPageView(page);
        }

        public void LogRequest(string request, TimeSpan duration, bool sucess)
        {
            string respone = sucess ? "200" : "504";
            this.telemetryClient.TrackRequest(request, DateTimeOffset.Now, duration, respone, sucess);
        }

        public void LogDependancyCall(string dependancy, string callName, TimeSpan duration, bool sucess)
        {
            this.telemetryClient.TrackDependency(dependancy, callName, DateTime.Now, duration, sucess);
        }

        public void LogException(Exception exception, Dictionary<string, string> properties = null)
        {
            this.telemetryClient.TrackException(exception, properties);
        }

        public void LogTrace(string objectString, ActionRequest obj, string traceId)
        {
            var objToLog = JsonUtility.GetJObjectFromObject(obj);
            if (obj?.DataStore?.PrivateDataStore != null)
            {
                objToLog["DataStore"]["PrivateDataStore"] = null;
                LogTraceInAppInsights(objectString, objToLog, traceId);
            }
        }

        public void LogTrace(string objectString, ActionResponse obj, string traceId)
        {
            var objToLog = JsonUtility.GetJObjectFromObject(obj);

            if (obj.IsResponseSensitive && obj.Body != null)
            {
                objToLog["Body"] = null;
            }

            if (obj?.DataStore?.PrivateDataStore != null)
            {
                objToLog["DataStore"]["PrivateDataStore"] = null;
                LogTraceInAppInsights(objectString, objToLog, traceId);
            }
        }

        public void LogTraceInAppInsights(string objectString, object obj, string traceId)
        {
            Dictionary<string, object> container = new Dictionary<string, object>();
            container.Add("TraceId", traceId);
            container.Add(objectString, obj);

            foreach (var globParam in this.globalParams)
            {
                container.Add(globParam.Key, globParam.Value);
            }

            this.AddTraceId(traceId);

            this.telemetryClient.TrackTrace(JsonUtility.GetJsonStringFromObject(container));

            this.ClearTraceId();
        }

        public void AddTraceId(string traceId)
        {
            if (!this.telemetryClient.Context.Properties.Keys.Contains("TraceId"))
            {
                this.telemetryClient.Context.Properties.Add("TraceId", traceId);
            }

            this.telemetryClient.Context.Properties["TraceId"] = traceId;
        }

        public void ClearTraceId()
        {
            if (this.telemetryClient.Context.Properties.Keys.Contains("TraceId"))
            {
                this.telemetryClient.Context.Properties.Remove("TraceId");
            }
        }

        internal void LogEvent(string eventName, Dictionary<string, string> properties, ActionRequest requestBody, ActionResponse responseToReturn)
        {
            // Add the reqyest/response onto body
            if (properties == null)
            {
                properties = new Dictionary<string, string>();
            }

            string traceId = Guid.NewGuid().ToString();
            this.AddTraceId(traceId);
            this.LogTrace("RequestBody", requestBody, traceId);
            this.LogTrace("ResponseBody", responseToReturn, traceId);
            this.telemetryClient.TrackEvent(eventName, properties);
            this.ClearTraceId();
        }

        internal void LogException(Exception exception, Dictionary<string, string> properties, ActionRequest requestBody, ActionResponse responseToReturn)
        {
            // Add the reqyest/response onto body
            if (properties == null)
            {
                properties = new Dictionary<string, string>();
            }

            string traceId = Guid.NewGuid().ToString();
            this.AddTraceId(traceId);
            this.LogTrace("RequestBody", requestBody, traceId);
            this.LogTrace("ResponseBody", responseToReturn, traceId);
            this.telemetryClient.TrackException(exception, properties);
            this.ClearTraceId();
        }

        public void LogEmailSubscription(string emailAddress, string nameFirst, string nameLast)
        {
            Dictionary<string, string> emailSubscription = new Dictionary<string, string>();
            emailSubscription.Add("EmailAddress", emailAddress);
            emailSubscription.Add("FirstName", nameFirst);
            emailSubscription.Add("LastName", nameLast);
            this.LogEvent("Email-Subscription", emailSubscription);
        }

        public void LogPowerBiLogin(string tenantId, string directory)
        {
            Dictionary<string, string> powerBiLogin = new Dictionary<string, string>();
            powerBiLogin.Add("Tenant ID", tenantId);
            powerBiLogin.Add("Directory Name", directory);
            this.LogEvent("PowerBi-Login", powerBiLogin);
        }

        public void LogResource(
            DataStore ds,
            string resourceName,
            DeployedResourceType type, 
            CreatedBy createdBy, 
            string createdAt, 
            string resourceId = null,
            string tier = null)
        {
            string tenantId = ds.GetValue("PowerBITenantId");
            string subscriptionId = ds.GetJson("SelectedSubscription")?["SubscriptionId"]?.ToString();
            string subscriptionName = ds.GetJson("SelectedSubscription")?["DisplayName"]?.ToString();
            string resourceGroupName = ds.GetValue("SelectedResourceGroup");

            Dictionary<string, string> resourceParams = new Dictionary<string, string>();
            resourceParams.Add("tenantId", tenantId);
            resourceParams.Add("subscriptionId", subscriptionId);
            resourceParams.Add("subscriptionName", subscriptionName);
            resourceParams.Add("resourceGroupName", resourceGroupName);

            resourceParams.Add("resourceName", resourceName);
            resourceParams.Add("resourceType", type.ToString());
            resourceParams.Add("createdBy", createdBy.ToString());
            resourceParams.Add("createdAt", createdAt);
            resourceParams.Add("resourceId", resourceId);
            resourceParams.Add("tier", tier);

            this.LogEvent("LogResource", resourceParams);
        }

        internal void LogRequest(string request, TimeSpan duration, bool sucess, ActionRequest requestBody, ActionResponse responseToReturn)
        {
            if (responseToReturn == null)
            {
                responseToReturn = new ActionResponse(ActionStatus.Failure);
            }

            string respone = responseToReturn.Status.ResponseCode();

            // Add the request/response onto body
            string traceId = Guid.NewGuid().ToString();
            this.AddTraceId(traceId);
            this.LogTrace("RequestBody", requestBody, traceId);
            this.LogTrace("ResponseBody", responseToReturn, traceId);

            this.telemetryClient.TrackRequest(request, DateTimeOffset.Now, duration, respone, sucess);
            this.ClearTraceId();
        }

        private void AddToDictionary(IDictionary<string, string> properties, IDictionary<string, string> targetDictionary)
        {
            foreach (var prop in properties)
            {
                targetDictionary.Add(prop.Key, prop.Value);
            }
        }
    }
}