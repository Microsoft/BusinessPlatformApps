using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Enums;
using Microsoft.Deployment.Common.Helpers;

using System.ComponentModel.Composition;
using System.Data;
using System.Threading.Tasks;

namespace Microsoft.Deployment.Actions.SQL
{
    [Export(typeof(IAction))]
    public class CompareSqlCollations : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string cnSrc = request.DataStore.GetAllValues("SqlConnectionString")[0];
            string cnDest = request.DataStore.GetAllValues("SqlConnectionString")[1];

            DataTable dtSrcCollation = SqlUtility.RunCommand(cnSrc, "SELECT DATABASEPROPERTYEX(Db_Name(), 'Collation') SQLCollation", SqlCommandType.ExecuteWithData);
            DataTable dtDestCollation = SqlUtility.RunCommand(cnDest, "SELECT DATABASEPROPERTYEX(Db_Name(), 'Collation') SQLCollation", SqlCommandType.ExecuteWithData);

            if (dtSrcCollation == null || dtSrcCollation.Rows.Count==0 || dtDestCollation== null || dtDestCollation.Rows.Count==0)
            {
                return new ActionResponse(ActionStatus.Failure, JsonUtility.GetEmptyJObject(), "SQL_CannotRetrieveCollations");
            }

            string sourceCollation = dtSrcCollation.Rows[0]["SQLCollation"].ToString();
            string destCollation = dtDestCollation.Rows[0]["SQLCollation"].ToString();
            
            if (sourceCollation.EqualsIgnoreCase(destCollation))
                return new ActionResponse(ActionStatus.Success, JsonUtility.GetEmptyJObject());
            else
                return new ActionResponse(ActionStatus.Failure, JsonUtility.GetEmptyJObject(), "SQL_TargetCollationDoesntMatch");

        }
    }
}