using Microsoft.Deployment.Common.Model.Bpst;

namespace Microsoft.Deployment.Common.Model.ApiManagement
{
    public class ApiManagementLogger
    {
        public string Id;
        public string Name;
        public ApiManagementLoggerProperties Properties;
        public string Type = "Microsoft.ApiManagement/service/loggers";

        public ApiManagementLogger(string idApimService, string id, string nameEventHub, string connectionString)
        {
            Id = $"{idApimService}/loggers/{id}";
            Name = id;
            Properties = new ApiManagementLoggerProperties(nameEventHub, connectionString);
        }
    }
}