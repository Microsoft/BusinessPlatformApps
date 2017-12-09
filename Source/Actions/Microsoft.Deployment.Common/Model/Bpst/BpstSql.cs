using Microsoft.Deployment.Common.ActionModel;

namespace Microsoft.Deployment.Common.Model.Bpst
{
    public class BpstSql
    {
        public string ConnectionDatabase;
        public string ConnectionServer;
        public string UserName;
        public string UserPassword;

        public BpstSql(DataStore ds)
        {
            ConnectionDatabase = ds.GetValue("Database");
            ConnectionServer = ds.GetValue("Server");
            UserName = ds.GetValue("Username");
            UserPassword = ds.GetValue("Password");
        }
    }
}