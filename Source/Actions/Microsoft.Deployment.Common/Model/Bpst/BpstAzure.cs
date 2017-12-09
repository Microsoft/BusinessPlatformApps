using Microsoft.Deployment.Common.ActionModel;

namespace Microsoft.Deployment.Common.Model.Bpst
{
    public class BpstAzure
    {
        public string IdSubscription;
        public string NameResourceGroup;
        public string TokenAzure;

        public BpstAzure(DataStore ds)
        {
            IdSubscription = ds.GetJson("SelectedSubscription", "SubscriptionId");
            NameResourceGroup = ds.GetValue("SelectedResourceGroup");
            TokenAzure = ds.GetJson("AzureToken", "access_token");
        }
    }
}