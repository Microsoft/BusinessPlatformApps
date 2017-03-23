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
        private string baseURL = Constants.Slot3;
        private RemoteWebDriver driver;

        [TestMethod]
        public void Given_CorrectInformation_And_AS_When_RunTwitter_ThenSuccess()
        {
            Given_CorrectCredentials_When_AzureAuth_Then_Success();
            HelperMethods.ClickNextButton();
            Given_CorrectSqlCredentials_When_ExistingSqlSelected_Then_PageValidatesSuccessfully();
            HelperMethods.ClickNextButton();
            Given_CorrectTwitterCredentials_When_Authenticating_Then_Success();
            Given_CorrectSearchTerms_When_Validating_Then_Success();
            HelperMethods.ClickNextButton();
            Given_CorrectHandles_When_Validating_Then_Success();
            HelperMethods.ClickNextButton();
            HelperMethods.NewAnalysisServices("twitteraas", Credential.Instance.ServiceAccount.Username, Credential.Instance.ServiceAccount.Password);
            HelperMethods.ClickButton("Next");
            HelperMethods.ClickButton("Run");
            HelperMethods.CheckDeploymentStatus();

            HelperMethods.CleanSubscription(
                Credential.Instance.ServiceAccount.Username,
                Credential.Instance.ServiceAccount.Password,
                Credential.Instance.ServiceAccount.TenantId,
                Credential.Instance.ServiceAccount.ClientId,
                Credential.Instance.ServiceAccount.SubscriptionId);
        }

        [TestMethod]
        public void Given_CorrectInformation_And_No_AS_When_RunTwitter_ThenSuccess()
        {
            Given_CorrectCredentials_When_AzureAuth_Then_Success();
            HelperMethods.ClickNextButton();
            Given_CorrectSqlCredentials_When_ExistingSqlSelected_Then_PageValidatesSuccessfully();
            HelperMethods.ClickNextButton();
            Given_CorrectTwitterCredentials_When_Authenticating_Then_Success();
            Given_CorrectSearchTerms_When_Validating_Then_Success();
            HelperMethods.ClickNextButton();
            Given_CorrectHandles_When_Validating_Then_Success();
            HelperMethods.ClickNextButton();
            HelperMethods.NoAnalysisServices();
            HelperMethods.ClickButton("Next");
            HelperMethods.ClickButton("Run");
            HelperMethods.CheckDeploymentStatus();

            HelperMethods.CleanSubscription(
                Credential.Instance.ServiceAccount.Username,
                Credential.Instance.ServiceAccount.Password,
                Credential.Instance.ServiceAccount.TenantId,
                Credential.Instance.ServiceAccount.ClientId,
                Credential.Instance.ServiceAccount.SubscriptionId);
        }

        [TestMethod]
        [Ignore]
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

        [TestMethod]
        [Ignore]
        public void Given_CorrectSqlCredentials_When_ExistingSqlSelected_Then_PageValidatesSuccessfully()
        {
            string server = Credential.Instance.Sql.Server;
            string username = Credential.Instance.Sql.Username;
            string password = Credential.Instance.Sql.Password;
            string database = Credential.Instance.Sql.TwitterDatabase;

            //HelperMethods.OpenWebBrowserOnPage("source");
            HelperMethods.SqlPageExistingDatabase(server, username, password);

            var validated = driver.FindElementByClassName("st-validated");

            Assert.IsTrue(validated.Text == "Successfully validated");

            HelperMethods.SelectSqlDatabase(database);
        }

        [TestMethod]
        [Ignore]
        public void Given_CorrectTwitterCredentials_When_Authenticating_Then_Success()
        {
            //HelperMethods.OpenWebBrowserOnPage("twitter");
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

        [TestMethod]
        [Ignore]
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

            HelperMethods.ClickValidateButton();

            var validated = driver.FindElementByClassName("st-validated");

            Assert.IsTrue(validated.Text == "Successfully validated");
        }

        [TestMethod]
        [Ignore]
        public void Given_CorrectHandles_When_Validating_Then_Success()
        {
            Thread.Sleep(new TimeSpan(0, 0, 10));
            //HelperMethods.OpenWebBrowserOnPage("twitterhandles");
            string handles = "@MSPowerBI @Azure @Microsoft";

            var handlesInput = driver.FindElementByCssSelector("input[class='st-input au-target']");

            handlesInput.SendKeys(handles);

            HelperMethods.ClickValidateButton();

            var validated = driver.FindElementByClassName("st-validated");

            Assert.IsTrue(validated.Text == "Successfully validated");
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
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
        }
    }
}
