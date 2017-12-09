using Microsoft.VisualStudio.TestTools.UnitTesting;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Tests.Actions.TestHelpers;

namespace Microsoft.Deployment.Tests.Actions.News
{
    [TestClass]
    public class NewsTests
    {
        [TestMethod]
        public void PopulateUserDefinedEntities()
        {
            var dataStore = new DataStore();
            dataStore.AddToDataStore("UserDefinedEntities", @"[{""color"":""#6699ff"",""icon"":""user"",""name"":""New Entity"",""values"":""Value 1\r\n Value 2\r\n Value 3""},{""color"":""#6699ff"",""icon"":""user"",""name"":""New Entity"",""values"":""Value 1\r\n Value 2\r\n Value 3""}]");
            var response = TestManager.ExecuteAction("Microsoft-PopulateUserDefinedEntities", dataStore);
            Assert.IsTrue(response.IsSuccess);
        }
    }
}