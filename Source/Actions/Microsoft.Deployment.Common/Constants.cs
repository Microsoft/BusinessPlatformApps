namespace Microsoft.Deployment.Common
{
    public static class Constants
    {
        // Azure ARM
        public const int ACTION_WAIT_INTERVAL = 5000; // in milliseconds
        // END Azure ARM

        public const string AppsPath = "Apps";
        public const string AppsWebPath = "Web";
        public const string SiteCommonPath = "SiteCommon";
        public const string ServiceUrl = "https://bpstservice.azurewebsites.net/";

        public const string ActionsPath = "Actions";
        public const string InitFile = "init.json";
        public const string BinPath = "bin";

        public const string AzureManagementApi = "https://management.azure.com/";
        public const string AzureManagementCoreApi = "https://management.core.windows.net/";
        public const string AzureKeyVaultApi = "https://vault.azure.net"; // Do not add a trailing slash
        public const string AzureWebSite = ".scm.azurewebsites.net/";
        public const string PowerBIService = "https://analysis.windows.net/PowerBI/api";

        public const string AzureAuthUri = "https://login.microsoftonline.com/{0}/oauth2/authorize?";
        public const string AzureTokenUri = "https://login.microsoftonline.com/{0}/oauth2/token";

        public const string MicrosoftClientId = "6b317a7c-0749-49bd-9e8c-d906aa43f64b";
        public const string ASClientId = "ecbb98ca-18e7-4f63-bb90-4534267a71b7";
        public const string MicrosoftClientIdCrm = "affab8de-076f-4b2c-b62d-29860cb41ff8";
        public const string MicrosoftClientIdPowerBI = "728cc6b6-9854-4601-a95a-152077d65d58";
        public const string MicrosoftClientSecret = "";
        public const string WebsiteRedirectPath = "/redirect.html";
        public const string AppInsightsKey = "app_insights_key";

        // Notifications
        public static string BpstDeploymentIdDatabase = string.Empty;
        public static string BpstNotifierUrl = "https://prod-27.northcentralus.logic.azure.com:443/workflows/0cdb1a801fa84af99477894c1fa202ba/triggers/manual/paths/invoke?api-version=2016-06-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=HKNuTDXQWw5KosQlKnSxRR-yHGxiVlCr0np732KpOXQ";

        public const string Office365ClientId = "5a75fd0b-23ec-4e09-ac55-b2c2050286de";

        // Ax
        public const string AxClientId = "bcf1cb1c-74d6-460c-941a-22f0110f1090";
        public const string AxErpResource = "00000015-0000-0000-c000-000000000000";
        public static string AxLocatorClientId = string.Empty;
        public static string AxLocatorSecret = string.Empty;
        public const string AxLocatorBaseUrl = "https://infra.locator.dynamics.com";
        public const string AxLocatorLoginAuthority = "https://login.windows.net/{0}";

        // Crm
        public const string CrmProfileName = "bpst-mscrm-profile";
        public const string MsCrmAuthority = "https://login.windows.net/common/oauth2/authorize?";
        public const string MsCrmClientId = "fb430120-4027-46b2-8499-95e0e8a3e646";
        public const string MsCrmResource = "b861dbcc-a7ef-4219-a005-0e4de4ea7dcf";
        public const string MsCrmToken = "https://login.windows.net/common/oauth2/token";

        // Facebook
        public const string FacebookGraphUrl = "https://graph.facebook.com";
        public const string FacebookClientId = "1566056443462682";
        public static string FacebookClientSecret = string.Empty;

        // Informatica
        public static string InformaticaRegistrationCode = string.Empty;

        // SocialGist (Reddit)
        public const string SocialGistProvisionKeyUrl = "https://api.boardreader.com/v1/Keys/List";
        public static string SocialGistProvisionKeyUserName = string.Empty;
        public static string SocialGistProvisionKeyPassphrase = string.Empty;
    }
}