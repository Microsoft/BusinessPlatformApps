using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Microsoft.Deployment.Common.Model.Informatica;

namespace Microsoft.Deployment.Common.Helpers
{
    public class InformaticaUtility
    {
        public const string BPST_SOURCE_NAME = "BPST Source";
        public const string BPST_TARGET_NAME = "BPST Target";
        public const string BPST_TASK_NAME = "BPST Task";

        private const string MSG_MISSING_LICENSES = "You do not have the following required licenses: {0}. Contact Informatica Support to obtain them. Alternatively, you can choose to create a new Informatica account as a 90-day trial account.";

        private const string ENDPOINT_LOGIN = "https://app.informaticaondemand.com";
        //private const string ENDPOINT_LOGIN = "https://expo.informaticaondemand.com";

        private const string URL_AGENT = "api/v2/runtimeEnvironment";
        private const string URL_AGENT_DOWNLOAD = "/saas/download/win64/installer/agent64_install.exe";
        private const string URL_CONNECTIONS = "api/v2/connection";
        private const string URL_LICENSE_INFO = "api/v2/licenseInfo";
        private const string URL_LOGIN = "ma/api/v2/user/login";
        private const string URL_LOGOUT = "api/v2/user/logout";
        private const string URL_LOGOUTALL = "ma/api/v2/user/logoutall";
        private const string URL_TASKS = "api/v2/task?type=DRS";

        public static async Task<RestClient> Initialize(string username, string password, bool isRegistration = false)
        {
            RestClient rc = new RestClient(ENDPOINT_LOGIN);

            if (isRegistration) return rc;

            //InformaticaLogout lo = new InformaticaLogout() { UserName = username, Password = password };
            //await rc.Post(URL_LOGOUTALL, JsonConvert.SerializeObject(lo));

            InformaticaLogin informaticaLogin = new InformaticaLogin { UserName = username, Password = password };
            string response = await rc.Post(URL_LOGIN, JsonConvert.SerializeObject(informaticaLogin));
            InformaticaUser u = JsonConvert.DeserializeObject<InformaticaUser>(response);

            rc = new RestClient(u.ServerUrl[u.ServerUrl.Length - 1] == '/' ? u.ServerUrl : u.ServerUrl + '/', null, new Dictionary<string, string> { { "icSessionId", u.IcSessionId } }, u.OrgId);

            InformaticaOrganizationLicenseInformation li = await GetLicensingInformation(rc);

            Dictionary<string, bool> licenses = new Dictionary<string, bool>
            {
                {"Salesforce Connector", false},
                {"Data Replication Service", false},
                {"Cloud Runtime", false},
                {"REST API", false},
            };

            int licenseCount = 0;
            foreach (InformaticaLicense t in li.LicenseInformation.Licenses)
            {
                switch (t.LicenseName.ToUpperInvariant())
                {
                    case "SALESFORCE":
                        licenses["Salesforce Connector"] = true;
                        licenseCount++;
                        break;
                    case "DRS":
                        licenses["Data Replication Service"] = true;
                        licenseCount++;
                        break;
                    case "ENVIRONMENT_CLOUD_AGENT":
                        licenses["Cloud Runtime"] = true;
                        licenseCount++;
                        break;
                    case "REST_API":
                        licenses["REST API"] = true;
                        licenseCount++;
                        break;
                }
            }

            if (licenseCount >= 4) return rc; // All is well, we found the licenses needed!

            string licensesNeeded = null;
            foreach (var k in licenses.Keys)
            {
                if (!licenses[k])
                    licensesNeeded += licensesNeeded == null ? k : ", " + k;
            }

            throw new Exception(string.Format(CultureInfo.InvariantCulture, MSG_MISSING_LICENSES, licensesNeeded));
        }

        public static string GetAgentDownloadLocation()
        {
            return ENDPOINT_LOGIN + URL_AGENT_DOWNLOAD;
        }

        public static async Task<string> GetConnectionId(RestClient rc, string connectionName)
        {
            string response = await rc.Get(URL_CONNECTIONS);
            InformaticaConnection[] connections = JsonConvert.DeserializeObject<InformaticaConnection[]>(response);

            if (connections == null) return null;

            string connectionId = null;
            for (int i = 0; i < connections.Length; i++)
            {
                if (!connections[i].Name.EqualsIgnoreCase(connectionName)) continue;
                connectionId = connections[i].Id;
                break;
            }

            return connectionId;
        }

        public static async Task<string> GetRuntimeEnvironmentId(RestClient rc, string name)
        {
            string response = await rc.Get(URL_AGENT);
            InformaticaRuntimeEnvironment[] environments = JsonConvert.DeserializeObject<InformaticaRuntimeEnvironment[]>(response);

            string id = null;
            for (int i = 0; i < environments.Length; i++)
            {
                if (environments[i].Name.EqualsIgnoreCase(name))
                {
                    id = environments[i].Id;
                    break;
                }
            }

            return id;
        }

        public static async Task<string> GetTaskId(RestClient rc, string taskName)
        {
            string response = await rc.Get(URL_TASKS);
            InformaticaTask[] tasks = JsonConvert.DeserializeObject<InformaticaTask[]>(response);

            if (tasks == null) return null;

            string taskId = null;
            for (int i = 0; i < tasks.Length; i++)
            {
                if (!tasks[i].Name.EqualsIgnoreCase(taskName)) continue;
                taskId = tasks[i].Id;
                break;
            }

            return taskId;
        }

        public static async Task Logout(RestClient rc, string username, string password)
        {
            InformaticaLogout logout = new InformaticaLogout { UserName = username, Password = password };
            await rc.Post(URL_LOGOUT, JsonConvert.SerializeObject(logout));
        }

        private static async Task<InformaticaOrganizationLicenseInformation> GetLicensingInformation(RestClient rc)
        {
            string response = await rc.Get(URL_LICENSE_INFO);
            return JsonConvert.DeserializeObject<InformaticaOrganizationLicenseInformation>(response);
        }
    }
}