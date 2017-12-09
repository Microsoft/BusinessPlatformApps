using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.Azure
{
    public class FunctionProperties
    {
        [JsonProperty("config")]
        public FunctionPropertiesConfig Config;
        [JsonProperty("config_href")]
        public string ConfigHref;
        [JsonProperty("files")]
        public string Files;
        [JsonProperty("function_app_id")]
        public string FunctionAppId;
        [JsonProperty("href")]
        public string Href;
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("script_href")]
        public string ScriptHref;
        [JsonProperty("script_root_path_href")]
        public string ScriptRootPathHref;
        [JsonProperty("secrets_file_href")]
        public string SecretsFileHref;
        [JsonProperty("test_data")]
        public string TestData;
    }
}