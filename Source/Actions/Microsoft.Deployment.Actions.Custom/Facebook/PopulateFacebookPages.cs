using System.ComponentModel.Composition;
using System.Data.SqlClient;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.Custom.Facebook
{
    [Export(typeof(IAction))]
    public class PopulateFacebookPages : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string page = request.DataStore.GetValue("FacebookPageId");
            string permanentPageToken = request.DataStore.GetValue("FacebookPageToken");
            string connString = request.DataStore.GetValue("SqlConnectionString");
            string cmdInsert = "INSERT INTO fbpa.PageTable(idpage, name) VALUES(@pageId, @pageName)";

            string pageName = await GetPageName(page, permanentPageToken);

            SqlUtility.RunCommand(connString, cmdInsert, Common.Enums.SqlCommandType.ExecuteWithoutData, new SqlParameter[] { new SqlParameter("@pageId", page),
                                                                                                                              new SqlParameter("@pageName", pageName)
                                                                                                                            }
                                 );

            return new ActionResponse(ActionStatus.Success);

        }

        public async Task<string> GetPageName(string pageId, string token)
        {
            string pageRequestUri = $"https://graph.facebook.com/v2.10/{pageId}/?access_token={token}";
            HttpResponseMessage response;
            string responseObj;
            string name = string.Empty;

            using (HttpClient client = new HttpClient())
            {
                response = await client.GetAsync(pageRequestUri);
                responseObj = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    name = JsonUtility.GetJObjectFromJsonString(responseObj)["name"].ToString();
                }

                else name = string.Empty;
            }

            return name;
        }
    }
}
