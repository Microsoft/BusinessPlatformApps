using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.ErrorCode;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.AzureCustom.Twitter
{
    [Export(typeof(IAction))]
    public class ValidateTwitterAccount : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var accountsWithSpaces = request.DataStore.GetValue("Accounts");
            var accounts = accountsWithSpaces.Split(' ').ToList();

            List<string> invalid = new List<string>();
            Dictionary<string, string> valid = new Dictionary<string, string>();

            foreach (var accountItem in accounts)
            {
                var accountTrimmed = accountItem.ToString().Trim();
                accountTrimmed = accountTrimmed.Replace("@", "");

                AzureHttpClient client = new AzureHttpClient(new Dictionary<string, string>()
                {
                    { "X-Push-State-Request", "true" }
                });

                string json = await client.GetJson(HttpMethod.Get, $"https://www.twitter.com/{accountTrimmed}");

                if (!string.IsNullOrEmpty(json))
                {
                    var obj = JsonUtility.GetJObjectFromJsonString(json);
                    var id = obj.SelectToken("init_data")?.SelectToken("profile_user")?.SelectToken("id_str")?.ToString();
                    valid.Add(accountTrimmed, id);
                }
                else
                {
                    invalid.Add(accountItem.ToString());
                }
            }

            dynamic response = new ExpandoObject();
            response.InvalidAccounts = invalid;
            response.ValidAccounts = valid;
            response.twitterHandle = string.Join(",", valid.Keys);
            response.twitterHandleId = string.Join(",", valid.Values);

            if (invalid.Any())
            {
                return new ActionResponse(ActionStatus.FailureExpected, JsonUtility.GetJObjectFromObject(response), null, AzureErrorCodes.TwitterAccountsInvalid);
            }

            return new ActionResponse(ActionStatus.Success, JsonUtility.GetJObjectFromObject(response));
        }
    }
}