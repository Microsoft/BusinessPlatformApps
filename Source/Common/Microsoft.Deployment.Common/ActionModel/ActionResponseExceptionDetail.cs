
namespace Microsoft.Deployment.Common.ActionModel
{
    using ErrorResources;
    using Helpers;
    using System;

    // TODO: rename variables since this is about error messages not codes
    public class ActionResponseExceptionDetail
    {
        public Exception ExceptionCaught = null;
        public string LogLocation = string.Empty;
        public string FriendlyMessageCode;

        public string FriendlyErrorMessage
        {
            get
            {
                
                return this.FriendlyMessageCode == null ? EnglishErrorCodes.DefaultErrorCode : ErrorUtility.GetErrorCode(this.FriendlyMessageCode);
            }
        }

        public string AdditionalDetailsErrorMessage { get; set; } = string.Empty;
    }
}
