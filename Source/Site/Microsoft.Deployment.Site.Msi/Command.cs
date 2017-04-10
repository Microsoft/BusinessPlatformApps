using System.Collections.Generic;
using System.Windows;
using Microsoft.Deployment.Common;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.AppLoad;
using Microsoft.Deployment.Common.Controller;
using Microsoft.Deployment.Common.Helpers;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Threading;

namespace Microsoft.Deployment.Build
{
    public class Command
    {
        public static AppFactory Templates { get; private set; }

        public Command()
        {
            Templates = new AppFactory(true);
        }

        public void close(bool isFinished)
        {
            if(!isFinished)
            {
                Installer.App.exitCode = 1;
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                Thread.Sleep(new TimeSpan(0, 0, 2));
                Application.Current.Shutdown(Installer.App.exitCode);
            });
        }

        public static string GetDirectory()
        {
            return "file:///" + System.AppDomain.CurrentDomain.BaseDirectory;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userGenId"></param>
        /// <param name="sessionId"></param>
        /// <param name="operationId"></param>
        /// <param name="uniqueId"></param>
        /// <param name="templateName"></param>
        /// <param name="id"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public string executeaction(string userId, string userGenId, string sessionId, string operationId, string uniqueId, string templateName, string id, string body)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("Service", "Offline");
            var action = JsonConvert.DeserializeObject<ActionRequest>(body);

            var userInfo = new UserInfo()
            {
                UserId = userId,
                UserGenId = userGenId,
                SessionId = sessionId,
                OperationId = operationId,
                UniqueLink = uniqueId,
                AppName = templateName,
                ActionName = id,
                WebsiteRootUrl = "https://msi",
                ServiceRootUrl = GetDirectory(),
                ServiceRelativePath = ""
            };

            var controller = new CommonControllerModel();
            controller.AppFactory = Templates;
            controller.AppRootFilePath = Templates.AppPath;
            controller.SiteCommonFilePath = Templates.SiteCommonPath;
            controller.ServiceRootFilePath = Templates.SiteCommonPath + "../";
            controller.Source = "API";

            var response = new CommonController(controller)
                .ExecuteAction(userInfo, action).Result;

            return JsonUtility.GetJsonStringFromObject(response);
        }

        public string getalltemplates(string userId, string userGenId, string sessionId, string operationId, string uniqueId)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("Service", "Offline");

            var userInfo = new UserInfo()
            {
                UserId = userId,
                UserGenId = userGenId,
                SessionId = sessionId,
                OperationId = operationId,
                UniqueLink = uniqueId,
                WebsiteRootUrl = "https://msi",
                ServiceRootUrl = GetDirectory(),
                ServiceRelativePath = ""
            };

            var controller = new CommonControllerModel();
            controller.AppFactory = Templates;
            controller.AppRootFilePath = Templates.AppPath;
            controller.SiteCommonFilePath = Templates.SiteCommonPath;
            controller.ServiceRootFilePath = Templates.SiteCommonPath + "../";
            controller.Source = "API";

            var response = new CommonController(controller)
                .GetAllApps(userInfo);

            return JsonUtility.GetJsonStringFromObject(response);
        }

        public string gettemplate(string userId, string userGenId, string sessionId, string operationId, string uniqueId, string templateName)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("Service", "Offline");

            var userInfo = new UserInfo()
            {
                UserId = userId,
                UserGenId = userGenId,
                SessionId = sessionId,
                OperationId = operationId,
                UniqueLink = uniqueId,
                AppName = templateName,
                WebsiteRootUrl = "https://msi",
                ServiceRootUrl = GetDirectory(),
                ServiceRelativePath = ""
            };

            var controller = new CommonControllerModel();
            controller.AppFactory = Templates;
            controller.AppRootFilePath = Templates.AppPath;
            controller.SiteCommonFilePath = Templates.SiteCommonPath;
            controller.ServiceRootFilePath = Templates.SiteCommonPath + "../";
            controller.Source = "API";

            var response = new CommonController(controller)
                .GetApp(userInfo);
            return JsonUtility.GetJsonStringFromObject(response);
        }
    }
}