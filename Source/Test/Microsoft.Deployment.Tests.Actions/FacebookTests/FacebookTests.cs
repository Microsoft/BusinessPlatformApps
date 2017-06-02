using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.Deployment.Tests.Actions.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Deployment.Common.ActionModel;

namespace Microsoft.Deployment.Tests.Actions.Facebook
{
    [TestClass]
    public class FacebookTests
    {
        [TestMethod]
        public void ValidateCorrectFacebookCredentials()
        {
            var dataStore = new DataStore();
            dataStore.AddToDataStore("FacebookClientId", "422676881457852");
            dataStore.AddToDataStore("FacebookClientSecret", "bf5fca097936ece936290031623b577b");
            var response = TestManager.ExecuteAction("Microsoft-ValidateFacebookDeveloperAccount", dataStore);
            Assert.IsTrue(response.IsSuccess);
        }

        [TestMethod]
        public void ValidateIncorrectFacebookCredentials()
        {
            var dataStore = new DataStore();
            dataStore.AddToDataStore("FacebookClientId", "422676881457851");
            dataStore.AddToDataStore("FacebookClientSecret", "bf5fca097936ece936290031623b577a");
            var response = TestManager.ExecuteAction("Microsoft-ValidateFacebookDeveloperAccount", dataStore);
            Assert.IsTrue(!response.IsSuccess);

            dataStore.AddToDataStore("FacebookClientId", "");
            dataStore.AddToDataStore("FacebookClientSecret", "");
            response = TestManager.ExecuteAction("Microsoft-ValidateFacebookDeveloperAccount", dataStore);
            Assert.IsTrue(!response.IsSuccess);
        }

        [TestMethod]
        public void ValidateFacebookPage()
        {
            var dataStore = new DataStore();
            dataStore.AddToDataStore("FacebookClientId", "422676881457852");
            dataStore.AddToDataStore("FacebookClientSecret", "bf5fca097936ece936290031623b577b");
            dataStore.AddToDataStore("FacebookPage", "walmart");
            var response = TestManager.ExecuteAction("Microsoft-ValidateFacebookPage", dataStore);
            Assert.IsTrue(response.IsSuccess);
        }

        [TestMethod]
        public void ValidateIncorrectFacebookPage()
        {
            var dataStore = new DataStore();
            dataStore.AddToDataStore("FacebookClientId", "422676881457852");
            dataStore.AddToDataStore("FacebookClientSecret", "bf5fca097936ece936290031623b577b");
            dataStore.AddToDataStore("FacebookPage", "walmartsfakepagethatdoesnotexist");
            var response = TestManager.ExecuteAction("Microsoft-ValidateFacebookPage", dataStore);
            Assert.IsTrue(!response.IsSuccess);
        }

        [TestMethod]
        public void SearchFacebookPage()
        {
            var dataStore = new DataStore();
            dataStore.AddToDataStore("FacebookClientId", "422676881457852");
            dataStore.AddToDataStore("FacebookClientSecret", "bf5fca097936ece936290031623b577b");
            dataStore.AddToDataStore("FacebookPage", "walmart");
            var response = TestManager.ExecuteAction("Microsoft-SearchFacebookPage", dataStore);
            Assert.IsTrue(!response.IsSuccess);
        }
    }
}
