using System.Threading.Tasks;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Tests.Actions.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Deployment.Tests.Actions.Twitter
{
    [TestClass]
    public class TwitterTests
    {
        [TestMethod]
        public async Task ValidateCognitiveKey()
        {
            var ds = new DataStore();

            ds.AddToDataStore("CognitiveServiceKey", "d06016d1b742488eb7ff03edeb5020aa");

            Assert.IsTrue(await TestManager.IsSuccessAsync("Microsoft-ValidateCognitiveKey", ds));
        }

        [TestMethod]
        public async Task ValidateTwitterAccount()
        {
            var ds = new DataStore();

            ds.AddToDataStore("Accounts", "@mohaali45");

            Assert.IsTrue(await TestManager.IsSuccessAsync("Microsoft-ValidateTwitterAccount", ds));
        }
    }
}