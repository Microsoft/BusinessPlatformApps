using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.AzureCustom.PowerApp
{
    [Export(typeof(IAction))]
    public class ConfigurePowerAppTables : BaseAction
    {
        private const string INSERT_INTO_TWITTER_QUERY = "INSERT INTO [dbo].[TwitterQuery] ([Id], [IsAdvanced], [QueryString]) VALUES ({0}, {1}, '{2}')";
        private const string INSERT_INTO_TWITTER_QUERY_DETAILS = "INSERT INTO [dbo].[TwitterQueryDetails] ([Id], [ReadableId], [Operator], [Operand]) VALUES ({0}, {1}, '{2}', '{3}')";
        private const string INSERT_INTO_TWITTER_QUERY_READABLE = "INSERT INTO [dbo].[TwitterQueryReadable] ([Id], [QueryId], [QueryReadable], [Query]) VAULES ({0}, {1}, '{2}', '{3}')";

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string searchQuery = request.DataStore.GetValue("SearchQuery") ?? string.Empty;
            string sqlConnectionString = request.DataStore.GetValueAtIndex("SqlConnectionString", "SqlServerIndex");

            searchQuery = searchQuery.ToLowerInvariant();
            //SqlUtility.InvokeSqlCommand(sqlConnectionString, string.Format(INSERT_INTO_TWITTER_QUERY, 1, 0, searchQuery), null);

            //string[] conditions = searchQuery.Split(new string[] { "or" }, StringSplitOptions.None);

            //for (int i = 0; i < conditions.Length; i++)
            //{
            //    string condition = conditions[i];
            //}

            //SqlUtility.InvokeSqlCommand(sqlConnectionString, string.Empty, null);

            return new ActionResponse(ActionStatus.Success, JsonUtility.GetEmptyJObject());
        }
    }
}