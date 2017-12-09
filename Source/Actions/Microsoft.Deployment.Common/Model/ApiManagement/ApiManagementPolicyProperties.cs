namespace Microsoft.Deployment.Common.Model.ApiManagement
{
    public class ApiManagementPolicyProperties
    {
        public string PolicyContent;

        public ApiManagementPolicyProperties(string policyContent)
        {
            PolicyContent = policyContent;
        }
    }
}