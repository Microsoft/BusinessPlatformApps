using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Deployment.Site.Web.Tests
{
    [TestClass]
    public class Constants
    {
        public static string Host { get; private set; }
        public static string Slot3 { get; private set; }
        public static string Slot1 { get; private set; }

        [AssemblyInitialize()]
        public static void AssemblyInit(TestContext context)
        {
            Host = Environment.GetEnvironmentVariables()["TestHost"]?.ToString() ?? "https://bpsolutiontemplates.com";
            Slot3 = "https://bpst-slot3.azurewebsites.net";
            Slot1 = "https://bpst-slot1.azurewebsites.net";
        }
    }
}
