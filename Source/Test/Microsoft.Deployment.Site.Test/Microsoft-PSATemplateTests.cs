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
    public class PSATemplateTests
    {
        private string baseURL = Constants.Slot1;
        private RemoteWebDriver driver;

        [TestMethod]
        public void Given_CorrectInformation_And_AS_When_RunPSA_ThenSuccess()
        {

            HelperMethods.CreateDatabase(Credential.Instance.Sql.Server,
                        Credential.Instance.Sql.Username, Credential.Instance.Sql.Password,
                        Credential.Instance.Sql.PSADatabase);
            Given_CorrectCredentials_When_Dynam365Auth_Then_Success();
            Thread.Sleep(new TimeSpan(0, 0, 5));
            HelperMethods.ClickButton("Next");
            Thread.Sleep(new TimeSpan(0, 0, 5));
            HelperMethods.ClickButton("Connect");
            HelperMethods.WaitForPage();
            Given_CorrectPSACredentials_When_Connecting_AzureKeyVault_Then_Success();
            HelperMethods.ClickButton("Next");
            HelperMethods.WaitForPage();
            Given_CorrectSqlCredentials_When_ExistingSqlSelected_Then_PageValidatesSuccessfully();
            HelperMethods.ClickButton("Next");
            HelperMethods.WaitForPage();
            HelperMethods.ClickButton("Run");
            HelperMethods.CheckDeploymentStatus();

            //int totalNumRows = HelperMethods.rowsInAllTables(Credential.Instance.Sql.Server,
            //                Credential.Instance.Sql.Username, Credential.Instance.Sql.Password,
            //                Credential.Instance.Sql.PSADatabase);
            //if(totalNumRows <= 0)
            //{
            //    Assert.Fail();
            //}

        }

        public void Given_CorrectCredentials_When_Dynam365Auth_Then_Success()
        {
            HelperMethods.OpenWebBrowserOnPage("dynamics365login");
            string username = Credential.Instance.PsaCreds.Username;
            string password = Credential.Instance.PsaCreds.Password;
            string organization = Credential.Instance.PsaCreds.Organization;

            HelperMethods.Dynamics365Page(username, password, organization);

            var validated = driver.FindElementByClassName("st-validated");

            Assert.IsTrue(validated.Text == "Successfully validated");
        }

        public void Given_CorrectSqlCredentials_When_ExistingSqlSelected_Then_PageValidatesSuccessfully()
        {
            string server = Credential.Instance.Sql.Server;
            string username = Credential.Instance.Sql.Username;
            string password = Credential.Instance.Sql.Password;
            string database = Credential.Instance.Sql.PSADatabase;

            HelperMethods.SqlPageExistingDatabase(server, username, password);

            var validated = driver.FindElementByClassName("st-validated");

            Assert.IsTrue(validated.Text == "Successfully validated");

            HelperMethods.SelectSqlDatabase(database);
        }

        public void Given_CorrectPSACredentials_When_Connecting_AzureKeyVault_Then_Success()
        {
            //HelperMethods.ClickButton("Connect to PSA");
            var otherAccount = driver.FindElementById("use_another_account_link");
            otherAccount.Click();

            string username = Credential.Instance.PsaCreds.Username;
            string password = Credential.Instance.PsaCreds.Password;

            var usernameBox = driver.FindElementById("cred_userid_inputtext");
            usernameBox.SendKeys(username);

            var passwordBox = driver.FindElementById("cred_password_inputtext");
            passwordBox.SendKeys(password);

            Thread.Sleep(new TimeSpan(0, 0, 10));

            var SignInButton = driver.FindElementById("cred_sign_in_button");
            SignInButton.Click();

            Thread.Sleep(new TimeSpan(0, 0, 10));

            var acceptButton = driver.FindElementById("cred_accept_button");
            acceptButton.Click(); 

            Thread.Sleep(new TimeSpan(0, 0, 2));

            var validated = driver.FindElementByClassName("st-validated");
            Assert.IsTrue(validated.Text == "Successfully validated");
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
                    Credential.Instance.PsaCreds.Username,
                    Credential.Instance.PsaCreds.Password,
                    Credential.Instance.PsaCreds.TenantId,
                    Credential.Instance.ServiceAccount.ClientId,
                    Credential.Instance.PsaCreds.SubscriptionId);
            }
            catch
            { 
                //if the clean subscription fails, then test probably failed before creation
            }

            try
            {
                HelperMethods.DeleteDatabase(Credential.Instance.Sql.Server, 
                                            Credential.Instance.Sql.Username, Credential.Instance.Sql.Password, 
                                            Credential.Instance.Sql.PSADatabase);
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
            HelperMethods.baseURL = baseURL + "?name=Microsoft-PSA";
            var options = new ChromeOptions();
            options.AddArgument("no-sandbox");
            HelperMethods.driver = new ChromeDriver(options);
            this.driver = HelperMethods.driver;
        }
    }
}
