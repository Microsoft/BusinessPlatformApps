using System.IO;
using Newtonsoft.Json;

namespace Microsoft.Deployment.Site.Test.TestHelpers
{
    public class Credential
    {
        public static Credential Instance { get; private set; }

        public SqlCreds Sql { get; set; }

        public SalesforceCreds Salesforce { get; set; }

        public ServiceAccount ServiceAccount { get; set; }

        public TwitterAccount TwitterAccount { get; set; }

        public SccmSql SccmSql { get; set; }

        public PsaCreds PsaCreds { get; set; }

        public static void Load()
        {
            Credential cred = new Credential();

            if (File.Exists("../../../../../../Private/Credentials/credentials.json"))
            {
                string text = File.ReadAllText("../../../../../../Private/Credentials/credentials.json");
                cred = JsonConvert.DeserializeObject<Credential>(text);
            }

            Instance = cred;
        }
    }
}
