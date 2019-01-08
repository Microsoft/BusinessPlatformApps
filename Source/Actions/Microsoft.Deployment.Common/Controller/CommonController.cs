using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.AppLoad;
using Microsoft.Deployment.Common.ErrorCode;
using Microsoft.Deployment.Common.Exceptions;

namespace Microsoft.Deployment.Common.Controller
{
    public class CommonController
    {
        private HashSet<string> disabledApps = new HashSet<string>()
        {
            "BlueMargin-VSTS",
            "EdFi",
            "FBI-Quickbooks",
            "iTalent-SocialAnalyticsByLithium",
            "Microsoft-AcademicTemplate",
            "Microsoft-ActivityLogTemplate",
            "Microsoft-APIManagementTemplate",
            "Microsoft-CreditAndCollections",
            "Microsoft-CRMSalesManagement",
            "Microsoft-Cuna",
            "Microsoft-D365CustomerService",
            "Microsoft-D365FieldService",
            "Microsoft-FacebookPageAnalytics",
            "Microsoft-FacebookTemplate",
            "Microsoft-FinanceInsightsCreditAndCollections",
            "Microsoft-NewsTemplate",
            "Microsoft-O365UsageAnalytics-Removed",
            "Microsoft-PSA",
            "Microsoft-RedditTemplate",
            "Microsoft-SalesforceSalesManagement",
            "Microsoft-SCCM2",
            "Microsoft-SCCMTemplate",
            "Microsoft-TwitterTemplate",
            "NealAnalytics-PbiUsageMetrics",
            "Simplement-SAP-ARTemplate"
        };

        public CommonController(CommonControllerModel commonControllerModel)
        {
            this.CommonControllerModel = commonControllerModel;
        }

        public CommonControllerModel CommonControllerModel { get; }


        public IEnumerable<string> GetAllApps(UserInfo info)
        {
            Logger logger = new Logger(info, this.CommonControllerModel);
            info.ActionName = "GetAllApps";

            var start = DateTime.Now;
            var templateNames = this.CommonControllerModel.AppFactory.Apps.Select(p => p.Key);
            var end = DateTime.Now;

            var allTemplates = templateNames as IList<string> ?? templateNames.ToList();
            logger.LogRequest("GetAllApps", end - start, allTemplates.Any());
            return allTemplates;
        }

        public App GetApp(UserInfo info)
        {
            Logger logger = new Logger(info, this.CommonControllerModel);
            info.ActionName = "GetApp";

            if (disabledApps.Contains(info.AppName))
                info.AppName = "Microsoft-Disabled";

            var start = DateTime.Now;
            var app = this.CommonControllerModel.AppFactory.Apps[info.AppName];
            var end = DateTime.Now;

            logger.LogRequest("GetApp-" + info.AppName, end - start, app != null);
            return app;
        }

        public async Task<ActionResponse> ExecuteAction(UserInfo info, ActionRequest request)
        {
            Logger logger = new Logger(info, this.CommonControllerModel);
            logger.LogEvent("Start-" + info.ActionName, null);
            var start = DateTime.Now;
            var action = this.CommonControllerModel.AppFactory.Actions[info.ActionName];
            var app = this.CommonControllerModel.AppFactory.Apps[info.AppName];
            info.App = app;

            request.ControllerModel = this.CommonControllerModel;
            request.Info = info;
            request.Logger = logger;

            if (action != null)
            {
                int loopCount = 0;

                ActionResponse responseToReturn = await RunActionAsync(request, logger, action, loopCount);
                responseToReturn.DataStore = request.DataStore;

                if (!responseToReturn.IsSuccess)
                {
                    // temporary code
                }

                logger.LogEvent("End-" + info.ActionName, null, request, responseToReturn);
                logger.LogRequest(action.OperationUniqueName, DateTime.Now - start,
                    responseToReturn.Status.IsSucessfullStatus(), request, responseToReturn);
                return responseToReturn;
            }

            logger.LogEvent("End-" + info.ActionName, null, request, null);
            logger.LogRequest(info.ActionName, DateTime.Now - start, false, request, null);
            var ex = new ActionNotFoundException();
            logger.LogException(ex, null, request, null);
            throw ex;
        }

        private async Task<ActionResponse> RunActionAsync(ActionRequest request, Logger logger,
            IAction action, int loopCount)
        {
            ActionResponse responseToReturn = null;
            do
            {
                try
                {
                    responseToReturn = await this.RunActionWithInterceptor(action, request);
                }
                catch (Exception exceptionFromAction)
                {
                    responseToReturn = await RunExceptionHandler(request, exceptionFromAction);
                }

                loopCount += 1;
            } while (loopCount <= 1 && responseToReturn.Status == ActionStatus.Retry);

            if (responseToReturn.Status == ActionStatus.Retry)
            {
                responseToReturn.Status = ActionStatus.Failure;
            }

            return responseToReturn;
        }

        private async Task<ActionResponse> RunExceptionHandler(ActionRequest request, Exception exceptionFromAction)
        {
            ActionResponse responseToReturn = null;
            if (exceptionFromAction is AggregateException)
            {
                AggregateException exc = exceptionFromAction as AggregateException;
                exceptionFromAction = exc.GetBaseException();
            }

            var exceptionHandler = this.CommonControllerModel.AppFactory.ActionExceptionsHandlers
                .FirstOrDefault(p => p.ExceptionExpected == exceptionFromAction.GetType());

            bool showGenericException = true;

            if (exceptionHandler != null)
            {
                try
                {
                    request.Logger.LogEvent("StartExceptionHandler-" + exceptionFromAction.GetType().Name, null);
                    responseToReturn = await exceptionHandler.HandleExceptionAsync(request, exceptionFromAction);
                    showGenericException = false;
                }
                catch
                {
                }
                finally
                {
                    request.Logger.LogEvent("EndExceptionHandler-" + exceptionFromAction.GetType().Name, null, null,
                        responseToReturn);
                }
            }

            if (showGenericException || responseToReturn.Status == ActionStatus.UnhandledException)
            {
                responseToReturn = new ActionResponse(ActionStatus.Failure, null, exceptionFromAction,
                    DefaultErrorCodes.DefaultErrorCode);
                request.Logger.LogException(exceptionFromAction, null);
            }

            return responseToReturn;
        }

        private async Task<ActionResponse> RunActionWithInterceptor(IAction action, ActionRequest request)
        {
            ActionResponse responseToReturn;
            List<IActionRequestInterceptor> actionInterceptors = new List<IActionRequestInterceptor>();

            IActionRequestInterceptor actionInterceptorHandle = null;
            foreach (var actionRequestInterceptor in this.CommonControllerModel.AppFactory.RequestInterceptors.ToList())
            {
                var InterceptStatus = await actionRequestInterceptor.CanInterceptAsync(action, request);
                if (InterceptStatus == InterceptorStatus.Intercept)
                {
                    actionInterceptors.Add(actionRequestInterceptor);
                }

                if (InterceptStatus == InterceptorStatus.IntercepAndHandleAction)
                {
                    actionInterceptorHandle = actionRequestInterceptor;
                }
            }

            // This code handles all interceptors which dont affect the action execution
            // Token refreshes, db creation tasks and generally actions which need prework before they
            // can be executed

            foreach (var requestInterceptor in actionInterceptors)
            {
                // Check to see what happened
                var response = await requestInterceptor.InterceptAsync(action, request);
            }

            // Check to make sure there is only one interceptor which can handle action otherwise use default
            // This could be either (delegate/elevated/non elevated handler)

            if (actionInterceptorHandle != null)
            {
                try
                {
                    // No need to log as it will be picked up by the caller
                    request.Logger.LogEvent("StartIntercepAndHandleAction-" + actionInterceptorHandle.GetType(), null);
                    responseToReturn = await actionInterceptorHandle.InterceptAsync(action, request);
                }
                finally
                {
                    request.Logger.LogEvent("EndIntercepAndHandleAction-" + actionInterceptorHandle.GetType(), null);
                }
            }
            else
            {
                // Execute default if none found
                responseToReturn = await action.ExecuteActionAsync(request);
            }

            return responseToReturn;
        }
    }
}