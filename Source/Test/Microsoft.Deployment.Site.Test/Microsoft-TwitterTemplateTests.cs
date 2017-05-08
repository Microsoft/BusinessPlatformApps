using System;
using System.Linq;
using System.Threading;
using Microsoft.Deployment.Site.Test.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;

namespace Microsoft.Deployment.Site.Web.Tests
{
    [TestClass]
    public class TwitterTemplateTests
    {
        private string baseURL = Constants.Slot1;
        private RemoteWebDriver driver;

        [TestMethod]
        public void Given_CorrectInformation_And_AS_When_RunTwitter_ThenSuccess()
        {
            Given_CorrectCredentials_When_AzureAuth_Then_Success();
            Thread.Sleep(new TimeSpan(0, 0, 5));
            HelperMethods.ClickButton("Next");
            Given_CorrectSqlCredentials_When_ExistingSqlSelected_Then_PageValidatesSuccessfully();
            HelperMethods.WaitForPage();
            HelperMethods.ClickButton("Next");
            Given_CorrectTwitterCredentials_When_Authenticating_Then_Success();
            HelperMethods.WaitForPage();
            Given_CorrectSearchTerms_When_Validating_Then_Success();
            HelperMethods.WaitForPage();
            HelperMethods.ClickButton("Next");
            Given_CorrectHandles_When_Validating_Then_Success();
            HelperMethods.WaitForPage();
            HelperMethods.ClickButton("Next");
            HelperMethods.NewAnalysisServices("twitteraas" + HelperMethods.resourceGroupName, Credential.Instance.ServiceAccount.Username, Credential.Instance.ServiceAccount.Password);
            HelperMethods.ClickButton("Next");
            HelperMethods.ClickButton("Run");
            HelperMethods.CheckDeploymentStatus();

            int totalNumRows = HelperMethods.rowsInAllTables(Credential.Instance.Sql.Server,
                            Credential.Instance.Sql.Username, Credential.Instance.Sql.Password,
                            Credential.Instance.Sql.TwitterDatabase);
            if (totalNumRows <= 0)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void Given_CorrectInformation_And_No_AS_When_RunTwitter_ThenSuccess()
        {
            Given_CorrectCredentials_When_AzureAuth_Then_Success();
            Thread.Sleep(new TimeSpan(0, 0, 5));
            HelperMethods.ClickButton("Next");
            Given_CorrectSqlCredentials_When_ExistingSqlSelected_Then_PageValidatesSuccessfully();
            HelperMethods.WaitForPage();
            HelperMethods.ClickButton("Next");
            HelperMethods.WaitForPage();
            Given_CorrectTwitterCredentials_When_Authenticating_Then_Success();
            HelperMethods.WaitForPage();
            Given_CorrectSearchTerms_When_Validating_Then_Success();
            HelperMethods.WaitForPage();
            HelperMethods.ClickButton("Next");
            Given_CorrectHandles_When_Validating_Then_Success();
            HelperMethods.WaitForPage();
            HelperMethods.ClickButton("Next");
            HelperMethods.NoAnalysisServices();
            HelperMethods.ClickButton("Next");
            HelperMethods.ClickButton("Run");
            HelperMethods.CheckDeploymentStatus();

            int totalNumRows = HelperMethods.rowsInAllTables(Credential.Instance.Sql.Server,
                            Credential.Instance.Sql.Username, Credential.Instance.Sql.Password,
                            Credential.Instance.Sql.TwitterDatabase);
            if (totalNumRows <= 0)
            {
                Assert.Fail();
            }
        }

        public void Given_CorrectCredentials_When_AzureAuth_Then_Success()
        {
            HelperMethods.OpenWebBrowserOnPage("login");
            string username = Credential.Instance.ServiceAccount.Username;
            string password = Credential.Instance.ServiceAccount.Password;
            string subscriptionName = Credential.Instance.ServiceAccount.SubscriptionName;

            HelperMethods.AzurePage(username, password, subscriptionName);

            var validated = driver.FindElementByClassName("st-validated");

            Assert.IsTrue(validated.Text == "Successfully validated");
        }

        public void Given_CorrectSqlCredentials_When_ExistingSqlSelected_Then_PageValidatesSuccessfully()
        {
            string server = Credential.Instance.Sql.Server;
            string username = Credential.Instance.Sql.Username;
            string password = Credential.Instance.Sql.Password;
            string database = Credential.Instance.Sql.TwitterDatabase;

            HelperMethods.SqlPageExistingDatabase(server, username, password);

            var validated = driver.FindElementByClassName("st-validated");

            Assert.IsTrue(validated.Text == "Successfully validated");

            HelperMethods.SelectSqlDatabase(database);
        }

        public void Given_CorrectTwitterCredentials_When_Authenticating_Then_Success()
        {
            HelperMethods.ClickButton("Connect to Twitter");

            string username = Credential.Instance.TwitterAccount.Username;
            string password = Credential.Instance.TwitterAccount.Password;

            var usernameBox = driver.FindElementById("username_or_email");
            usernameBox.SendKeys(username);

            var passwordBox = driver.FindElementById("password");
            passwordBox.SendKeys(password);

            var authorizeButton = driver.FindElementById("allow");
            authorizeButton.Click();
        }

        public void Given_CorrectSearchTerms_When_Validating_Then_Success()
        {
            HelperMethods.OpenWebBrowserOnPage("searchterms");
            string searchTerms = "@MSPowerBI OR Azure";

            var searchTermsInput = driver.FindElementByCssSelector("input[class='st-input au-target']");

            while (!searchTermsInput.Enabled)
            {
                Thread.Sleep(new TimeSpan(0, 0, 2));
            }

            searchTermsInput.SendKeys(searchTerms);

            HelperMethods.ClickButton("Validate");

            var validated = driver.FindElementByClassName("st-validated");

            Assert.IsTrue(validated.Text == "Successfully validated");
        }

        public void Given_CorrectHandles_When_Validating_Then_Success()
        {
            Thread.Sleep(new TimeSpan(0, 0, 10));
            string handles = "@MSPowerBI @Azure @Microsoft";

            var handlesInput = driver.FindElementByCssSelector("input[class='st-input au-target']");

            handlesInput.SendKeys(handles);

            HelperMethods.ClickButton("Validate");

            var validated = driver.FindElementByClassName("st-validated");

            Assert.IsTrue(validated.Text == "Successfully validated");
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
            try
            {
                HelperMethods.CleanSubscription(
                    Credential.Instance.ServiceAccount.Username,
                    Credential.Instance.ServiceAccount.Password,
                    Credential.Instance.ServiceAccount.TenantId,
                    Credential.Instance.ServiceAccount.ClientId,
                    Credential.Instance.ServiceAccount.SubscriptionId);
            }
            catch
            {
                //if the clean subscription fails, then test probably failed before creation
            }

            try
            {
                HelperMethods.DeleteDatabase(Credential.Instance.Sql.Server,
                                            Credential.Instance.Sql.Username, Credential.Instance.Sql.Password,
                                            Credential.Instance.Sql.TwitterDatabase);
            }
            catch
            {
                //if the delete DB fails, then the test probably failed to create
            }

            HelperMethods.driver.Quit();
        }

        [TestInitialize]
        public void Initialize()
        {
            Credential.Load();
            HelperMethods.baseURL = baseURL + "?name=Microsoft-TwitterTemplate";
            var options = new ChromeOptions();
            options.AddArgument("no-sandbox");
            HelperMethods.driver = new ChromeDriver(options);
            this.driver = HelperMethods.driver;
            try
            {
                HelperMethods.CreateDatabase(Credential.Instance.Sql.Server,
                                                Credential.Instance.Sql.Username, Credential.Instance.Sql.Password,
                                                Credential.Instance.Sql.TwitterDatabase);
            }
            catch { }
        }
    }
}
