using System.IO;
using Newtonsoft.Json;

namespace Microsoft.Deployment.Tests.Actions.TestHelpers
{
    public class Credential
    {
        public static Credential Instance { get; private set; }

        public AAD AAD { get; set; }

        public SqlCreds Sql { get; set; }

        public SalesforceCreds Salesforce { get; set; }

        public StorageAccountCreds StorageAccount { get; set; }

        public SocialGistCreds SocialGist { get; set; }

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
