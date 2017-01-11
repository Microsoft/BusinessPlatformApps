using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Deployment.Actions.AzureCustom.LogicApp
{
    public class LogicAppConnector
    {
        public string name { get; set; }
        public string id { get; set; }
        public string location { get; set; }
        public Properties properties { get; set; }
    }

    public class Properties
    {
        public string displayName { get; set; }
        public Api api { get; set; }

        public JToken parametervalues { get; set; }
    }

    public class Api
    {
        public string name { get; set; }
        public string location { get; set; }
        public string id { get; set; }
        public string type { get; set; }
    }
}
