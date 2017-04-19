using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Deployment.Site.Test
{
    public class PsaCreds
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public string Organization { get; set; }
        public string SubscriptionId { get; set; }

        public string TenantId { get; set; }
    }
}
