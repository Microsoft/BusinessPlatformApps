using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Tests.Actions.TestHelpers;

namespace Microsoft.Deployment.Tests.Actions.PBI
{
    [TestClass]
    public class PBITests
    {
        [TestMethod]
        public async Task ValidatePBIPublish()
        {
            DataStore ds = await AAD.GetUserTokenFromPopup("powerbi");

            Assert.IsTrue(await TestManager.IsSuccessAsync("Microsoft-GetPBIClusterUri", ds));
        }
    }
}