using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Deployment.Common;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Controller;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Deployment.Site.Service.Controllers
{
    public class ActionController : ApiController
    {
        [HttpPost]
        public async Task<ActionResponse> ExecuteAction(string id, [FromBody] ActionRequest body)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("Service", "Online");

            string operationId = string.Empty;
            string userGenId = string.Empty;
            string appName = string.Empty;
            string userId = string.Empty;
            string sessionId = string.Empty;
            string uniqueId = string.Empty;


            if (this.Request.Headers.Contains("OperationId"))
            {
                operationId = this.Request.Headers.GetValues("OperationId").First();
            }

            if (this.Request.Headers.Contains("UserGeneratedId"))
            {
                userGenId = this.Request.Headers.GetValues("UserGeneratedId").First();
            }

            if (this.Request.Headers.Contains("TemplateName"))
            {
                appName = this.Request.Headers.GetValues("TemplateName").First();
            }

            if (this.Request.Headers.Contains("UserId"))
            {
                userId = this.Request.Headers.GetValues("UserId").First();
            }

            if (this.Request.Headers.Contains("SessionId"))
            {
                sessionId = this.Request.Headers.GetValues("SessionId").First();
            }

            if (this.Request.Headers.Contains("UniqueId"))
            {
                uniqueId = this.Request.Headers.GetValues("UniqueId").First();
            }

            string referer = Request.RequestUri.GetLeftPart(UriPartial.Authority);
            string[] url = Request.Headers.Referrer?.ToString().Split('/');
            if (url?.Length >= 3)
            {
                referer = url[0] + "//" + url[2];
            }

            UserInfo info = new UserInfo()
            {
                ActionName = id,
                AppName = appName,
                OperationId = operationId,
                SessionId = sessionId,
                UniqueLink = uniqueId,
                UserGenId = userGenId,
                UserId = userId,
                WebsiteRootUrl = referer,
                SerivceRootUrl = this.Request.RequestUri.GetLeftPart(UriPartial.Authority)
            };

            ActionResponse response =  await new CommonController(WebApiConfig.CommonControllerModel)
                .ExecuteAction(info, body);

            return response;
        }
    }
}