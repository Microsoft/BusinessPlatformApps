namespace Microsoft.Deployment.Common.Model.ApiManagement
{
    public class ApiManagementLoggerProperties
    {
        public ApiManagementLoggerPropertiesCredentials Credentials;
        public string Description = "BPST Template Logger";
        public bool IsBuffered = true;
        public string LoggerType = "azureEventHub";

        public ApiManagementLoggerProperties(string nameEventHub, string connectionString)
        {
            Credentials = new ApiManagementLoggerPropertiesCredentials(nameEventHub, connectionString);
        }
    }
}