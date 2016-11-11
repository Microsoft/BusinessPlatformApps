using System;
using Microsoft.Deployment.Common.ErrorCode;
using Microsoft.Deployment.Common.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microsoft.Deployment.Common.ActionModel
{
    public class ActionResponse
    {
        // Status of the request
        [JsonConverter(typeof(StringEnumConverter))]
        public ActionStatus Status { get; set; }

        [JsonConverter(typeof(ResponseObjectConverter))]
        public object Body { get; private set; }

        public bool IsResponseSensitive { get; private set; }

        public bool IsSuccess
        {
            get { return this.Status != ActionStatus.Failure && this.Status != ActionStatus.FailureExpected; }
        }

        public DataStore DataStore { get; set; }

        public ActionResponseExceptionDetail ExceptionDetail = new ActionResponseExceptionDetail();

        // Used to Serialize and Deserialize for testing purposes only
        public ActionResponse()
        {
        }

        public ActionResponse(ActionStatus status) : this(status, new object(), false)
        {
        }

        public ActionResponse(ActionStatus status, object response, bool isResponseSensitive = false)
        {
            this.IsResponseSensitive = isResponseSensitive;
            if (status == ActionStatus.Failure || status == ActionStatus.FailureExpected)
            {
                this.ExceptionDetail.FriendlyMessageCode = DefaultErrorCodes.DefaultErrorCode;
            }
            this.Status = status;
            this.Body = response;
        }


        public ActionResponse(ActionStatus status, object response, string friendlyErrorMessageCode)
        {
            if (status == ActionStatus.Failure)
            {
                this.ExceptionDetail.FriendlyMessageCode = friendlyErrorMessageCode;
            }

            this.Status = status;
            this.Body = JsonUtility.GetJObjectFromObject(response);
        }

        public ActionResponse(ActionStatus status, object response, Exception exception,
            string friendlyMessageCode, string additionaldetails)
        {
            this.Status = status;
            this.Body = response;
            this.ExceptionDetail.ExceptionCaught = exception;
            this.ExceptionDetail.FriendlyMessageCode = friendlyMessageCode;
            this.ExceptionDetail.AdditionalDetailsErrorMessage = additionaldetails;
        }

        public ActionResponse(ActionStatus status, object response, Exception exception,
            string friendlyMessageCode)
        {
            this.Status = status;
            this.Body = response;
            this.ExceptionDetail.ExceptionCaught = exception;
            this.ExceptionDetail.FriendlyMessageCode = friendlyMessageCode;
            this.ExceptionDetail.AdditionalDetailsErrorMessage = GetInnerExceptionText(exception);
        }

        private static string GetInnerExceptionText(Exception exception)
        {
            if (exception == null)
                return string.Empty;

            Exception loopException = exception;
            while (loopException.InnerException != null)
            {
                loopException = loopException.InnerException;
            }

            return loopException.Message;
        }
    }
}