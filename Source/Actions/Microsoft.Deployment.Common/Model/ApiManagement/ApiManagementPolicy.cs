namespace Microsoft.Deployment.Common.Model.ApiManagement
{
    public class ApiManagementPolicy
    {
        public string Id;
        public string Name = "policy";
        public ApiManagementPolicyProperties Properties;
        public string Type = "Microsoft.ApiManagement/service/policies";
        
        public ApiManagementPolicy(string idApimService, string policyContent)
        {
            Id = $"{idApimService}/policies/policy";
            Properties = new ApiManagementPolicyProperties(policyContent);
        }
    }
}