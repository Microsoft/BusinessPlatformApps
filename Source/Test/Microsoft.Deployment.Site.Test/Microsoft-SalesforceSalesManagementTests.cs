using Microsoft.Deployment.Site.Test.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using System;
using System.Linq;
using System.Threading;

namespace Microsoft.Deployment.Site.Web.Tests
{
    [TestClass]
    public class SalesforceSalesManagementTests
    {
        private string baseURL = Constants.Slot3;
        private RemoteWebDriver driver;

        [TestMethod]
        public void Given_CorrectInformation_And_No_AS_When_RunSalesforce_ThenSuccess()
        {
            Given_CorrectCredentials_When_AzureAuth_Then_Success();
            Thread.Sleep(new TimeSpan(0, 0, 5));
            HelperMethods.ClickButton("Next");
            HelperMethods.WaitForPage();
            HelperMethods.ClickButton("Next");
            Given_CorrectSalesforceCredentials_When_Validate_Then_PageValidatesSuccesfully();
            HelperMethods.WaitForPage();
            HelperMethods.ClickButton("Next");
            Given_CorrectSqlCredentials_When_ExistingSqlSelected_Then_PageValidatesSuccessfully();
            HelperMethods.WaitForPage();
            HelperMethods.ClickButton("Next");
            HelperMethods.WaitForPage();
            HelperMethods.NoAnalysisServices();
            HelperMethods.WaitForPage();
            HelperMethods.ClickButton("Next");
            HelperMethods.WaitForPage();
            Thread.Sleep(new TimeSpan(0, 0, 5));
            HelperMethods.ClickButton("Next");
            HelperMethods.WaitForPage();
            HelperMethods.ClickButton("Run");
            HelperMethods.CheckDeploymentStatus();

            int totalNumRows = HelperMethods.rowsInAllTables(Credential.Instance.Sql.Server,
                            Credential.Instance.Sql.Username, Credential.Instance.Sql.Password,
                            Credential.Instance.Sql.SalesforceDatabase);
            if (totalNumRows <= 0)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void Given_CorrectInformation_And_AS_When_RunSalesforce_ThenSuccess()
        {
            Given_CorrectCredentials_When_AzureAuth_Then_Success();
            Thread.Sleep(new TimeSpan(0, 0, 5));
            HelperMethods.ClickButton("Next");
            HelperMethods.WaitForPage();
            HelperMethods.ClickButton("Next");
            Given_CorrectSalesforceCredentials_When_Validate_Then_PageValidatesSuccesfully();
            HelperMethods.WaitForPage();
            HelperMethods.ClickButton("Next");
            Given_CorrectSqlCredentials_When_ExistingSqlSelected_Then_PageValidatesSuccessfully();
            HelperMethods.WaitForPage();
            HelperMethods.ClickButton("Next");
            HelperMethods.WaitForPage();
            HelperMethods.NewAnalysisServices("salesforceas" + HelperMethods.resourceGroupName, Credential.Instance.ServiceAccount.Username, Credential.Instance.ServiceAccount.Password);
            HelperMethods.WaitForPage();
            HelperMethods.ClickButton("Next");
            HelperMethods.WaitForPage();
            HelperMethods.ClickButton("Next");
            HelperMethods.WaitForPage();
            HelperMethods.ClickButton("Run");
            HelperMethods.CheckDeploymentStatus();

            int totalNumRows = HelperMethods.rowsInAllTables(Credential.Instance.Sql.Server,
                            Credential.Instance.Sql.Username, Credential.Instance.Sql.Password,
                            Credential.Instance.Sql.SalesforceDatabase);
            if (totalNumRows <= 0)
            {
                Assert.Fail();
            }
        }

        public void Given_CorrectCredentials_When_AzureAuth_Then_Success()
        {
            HelperMethods.OpenWebBrowserOnPage("azure");
            string username = Credential.Instance.ServiceAccount.Username;
            string password = Credential.Instance.ServiceAccount.Password;
            string subscriptionName = Credential.Instance.ServiceAccount.SubscriptionName;

            HelperMethods.AzurePage(username, password, subscriptionName);

            var validated = driver.FindElementByClassName("st-validated");

            Assert.IsTrue(validated.Text == "Successfully validated");
        }

        public void Given_CorrectSalesforceCredentials_When_Validate_Then_PageValidatesSuccesfully()
        {
            string username = Credential.Instance.Salesforce.Username;
            string password = Credential.Instance.Salesforce.Password;
            string token = Credential.Instance.Salesforce.Token;

            SalesforcePage(username, password, token);

            var validated = driver.FindElementByClassName("st-validated");

            Assert.IsTrue(validated.Text == "Successfully validated");
        }

        public void Given_NoSalesforceCredentials_When_Validate_Then_PageFailsWithErrorMessage()
        {
            HelperMethods.OpenWebBrowserOnPage("salesforce");

            SalesforcePage(string.Empty, string.Empty, string.Empty);

            var validated = driver.FindElementByClassName("st-validated");

            Assert.IsTrue(validated.Text == "INVALID_LOGIN: Invalid username, password, security token; or user locked out.");
        }

        public void Given_CorrectSqlCredentials_When_ExistingSqlSelected_Then_PageValidatesSuccessfully()
        {
            string server = Credential.Instance.Sql.Server;
            string username = Credential.Instance.Sql.Username;
            string password = Credential.Instance.Sql.Password;
            string database = Credential.Instance.Sql.SalesforceDatabase;

            HelperMethods.SqlPageExistingDatabase(server, username, password);

            Thread.Sleep(new TimeSpan(0, 0, 3));

            var validated = driver.FindElementByClassName("st-validated");

            Assert.IsTrue(validated.Text == "Successfully validated");

            HelperMethods.SelectSqlDatabase(database);
        }

        public void Given_WrongSqlCredentials_When_ExistingSqlSelected_Then_PageFailsWithErrorMessage()
        {
            HelperMethods.OpenWebBrowserOnPage("target");

            string server = Credential.Instance.Sql.Server;
            string username = Credential.Instance.Sql.Username;
            string password = "wrongPassword";

            HelperMethods.SqlPageExistingDatabase(server, username, password);

            var validated = driver.FindElementByClassName("st-validated");
            Assert.IsTrue(validated.Text == "Login to SQL failed");
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
                                            Credential.Instance.Sql.SalesforceDatabase);
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
            HelperMethods.baseURL = baseURL + "?name=Microsoft-SalesforceSalesManagement";
            var options = new ChromeOptions();
            options.AddArgument("no-sandbox");
            options.AddUserProfilePreference("profile.password_manager_enabled", false);
            try
            {
                HelperMethods.CreateDatabase(Credential.Instance.Sql.Server,
                                                Credential.Instance.Sql.Username, Credential.Instance.Sql.Password,
                                                Credential.Instance.Sql.SalesforceDatabase);
            }
            catch
            {
                //probably the old DB is still there from last time
                HelperMethods.DeleteDatabase(Credential.Instance.Sql.Server,
                                            Credential.Instance.Sql.Username, Credential.Instance.Sql.Password,
                                            Credential.Instance.Sql.SalesforceDatabase);
                HelperMethods.CreateDatabase(Credential.Instance.Sql.Server,
                                                Credential.Instance.Sql.Username, Credential.Instance.Sql.Password,
                                                Credential.Instance.Sql.SalesforceDatabase);
            }
            HelperMethods.driver = new ChromeDriver(options);
            this.driver = HelperMethods.driver;
        }

        public void SalesforcePage(string username, string password, string token)
        {

            var elements = driver.FindElementsByCssSelector("input[class='st-input au-target']");

            while (elements.Count < 3)
            {
                elements = driver.FindElementsByCssSelector("input[class='st-input au-target']");
            }

            var tokenBox = elements.First(e => e.GetAttribute("value.bind").Contains("salesforceToken"));
            tokenBox.SendKeys(token);

            var usernameBox = elements.First(e => e.GetAttribute("value.bind").Contains("salesforceUsername"));
            usernameBox.SendKeys(username);

            var passwordBox = elements.First(e => e.GetAttribute("value.bind").Contains("salesforcePassword"));
            passwordBox.SendKeys(password);

            HelperMethods.ClickButton("Validate");
        }
    }
}
