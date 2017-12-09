using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.Deployment.Common.Model.Informatica;

namespace Microsoft.Deployment.Actions.Custom.Informatica
{
    [Export(typeof(IAction))]
    public class CreateInformaticaTask : BaseAction
    {
        private const string URL_REPLICATION_TASK = "api/v2/drstask";
        private const string URL_SCHEDULE = "api/v2/schedule";

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string username = request.DataStore.GetValue("InformaticaUsername");
            string password = request.DataStore.GetValue("InformaticaPassword");
            RestClient rc = await InformaticaUtility.Initialize(username, password);

            List<string> entities = JsonUtility.DeserializeEntities(request.DataStore.GetValue("Entities"), request.DataStore.GetValue("AdditionalObjects"));

            InformaticaTask task = new InformaticaTask
            {
                BulkApi = true,
                SourceConnectionId = await InformaticaUtility.GetConnectionId(rc, InformaticaUtility.BPST_SOURCE_NAME),
                TargetConnectionId = await InformaticaUtility.GetConnectionId(rc, InformaticaUtility.BPST_TARGET_NAME),
                Name = InformaticaUtility.BPST_TASK_NAME,
                OrgId = rc.ID,
                ReplicationObjects = entities.ToArray(),
                ScheduleId = await CreateSchedule(rc, "PBI_" + Guid.NewGuid().ToString("N"))
            };

            await rc.Post(URL_REPLICATION_TASK, JsonConvert.SerializeObject(task));

            await InformaticaUtility.Logout(rc, username, password);

            return new ActionResponse(ActionStatus.Success);
        }

        private async Task<string> CreateSchedule(RestClient rc, string name)
        {
            InformaticaSchedule schedule = new InformaticaSchedule { Name = name, OrgId = rc.ID };

            JsonSerializerSettings dateSettings = new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffK"
            };

            string response = await rc.Post(URL_SCHEDULE, JsonConvert.SerializeObject(schedule, dateSettings));

            schedule = JsonConvert.DeserializeObject<InformaticaSchedule>(response);

            return schedule.Id;
        }
    }
}