namespace Microsoft.Deployment.Common.Model.ApiManagement
{
    public class ApiManagementLoggerPropertiesCredentials
    {
        public string ConnectionString;
        public string Name;

        public ApiManagementLoggerPropertiesCredentials(string nameEventHub, string connectionString)
        {
            ConnectionString = connectionString;
            Name = nameEventHub;
        }
    }
}