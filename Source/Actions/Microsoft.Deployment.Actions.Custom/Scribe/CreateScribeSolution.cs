using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.Deployment.Common.Model.Scribe;

namespace Microsoft.Deployment.Actions.Custom.Scribe
{
    [Export(typeof(IAction))]
    public class CreateScribeSolution : BaseAction
    {
        private const string URL_AGENTS = "/v1/orgs/{0}/agents";
        private const string URL_CONNECTIONS = "/v1/orgs/{0}/connections";
        private const string URL_SOLUTION_SCHEDULE = "/v1/orgs/{0}/solutions/{1}/schedule";
        private const string URL_SOLUTIONS = "/v1/orgs/{0}/solutions";

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            RestClient rc = ScribeUtility.Initialize(request.DataStore.GetValue("ScribeUsername"), request.DataStore.GetValue("ScribePassword"));

            string orgId = request.DataStore.GetValue("ScribeOrganizationId");

            ScribeSolution solution = new ScribeSolution
            {
                Name = ScribeUtility.BPST_SOLUTION_NAME,
                Description = string.Empty,
                SolutionType = "Replication",
                ReplicationSettings = new ScribeReplicationSettings(request.DataStore.GetValue("Entities").Split(new[] { ',', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)),
                ConnectionIdForSource = await GetConnectionId(rc, orgId, ScribeUtility.BPST_SOURCE_NAME),
                ConnectionIdForTarget = await GetConnectionId(rc, orgId, ScribeUtility.BPST_TARGET_NAME),
                AgentId = await GetAgentId(rc, orgId, "Cloud Agent")
            };

            string response = await rc.Post(string.Format(CultureInfo.InvariantCulture, URL_SOLUTIONS, orgId), JsonConvert.SerializeObject(solution));
            ScribeSolution result = JsonConvert.DeserializeObject<ScribeSolution>(response);

            ScribeSolutionSchedule schedule = new ScribeSolutionSchedule("Daily");
            await rc.Put(string.Format(CultureInfo.InvariantCulture, URL_SOLUTION_SCHEDULE, orgId, result.Id), JsonConvert.SerializeObject(schedule));

            return new ActionResponse(ActionStatus.Success, JsonUtility.GetEmptyJObject());
        }

        private async Task<string> GetAgentId(RestClient rc, string orgId, string agentName)
        {
            string response = await rc.Get(string.Format(CultureInfo.InvariantCulture, URL_AGENTS, orgId));
            List<ScribeAgent> agents = JsonConvert.DeserializeObject<List<ScribeAgent>>(response);

            foreach (ScribeAgent agent in agents)
            {
                if (agent.Name.Equals(agentName, StringComparison.OrdinalIgnoreCase))
                {
                    return agent.Id;
                }
            }

            return null;
        }

        private async Task<string> GetConnectionId(RestClient rc, string orgId, string connectionName)
        {
            string response = await rc.Get(string.Format(CultureInfo.InvariantCulture, URL_CONNECTIONS, orgId),
                string.Format(CultureInfo.InvariantCulture, "name={0}&limit=1&expand=false", connectionName), null);
            List<ScribeConnection> result = JsonConvert.DeserializeObject<List<ScribeConnection>>(response);
            return result.Count == 1 ? result[0].Id : null;
        }
    }
}