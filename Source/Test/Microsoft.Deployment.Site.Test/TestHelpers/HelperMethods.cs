using System;
using System.Linq;
using System.Threading;
using Microsoft.Azure;
using Microsoft.Azure.Management.Resources;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace Microsoft.Deployment.Site.Web.Tests
{
    public class HelperMethods
    {
        public static string baseURL;
        public static RemoteWebDriver driver;
        public static string resourceGroupName;

        public static void OpenWebBrowserOnPage(string page)
        {
            var url = baseURL + $"#/{page}";
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = new TimeSpan(0, 0, 30);
            driver.Navigate().GoToUrl(url);
        }

        public static void ClickValidateButton()
        {
            var button = driver.FindElementsByTagName("Button").First(e => e.Text == "Validate");
            button.Click();
        }

        public static void ClickNextButton()
        {
            var button = driver.FindElementsByTagName("Button").First(e => e.Text == "Next");
            button.Click();
        }

        public static void ClickButton(string buttonText)
        {
            var button = driver.FindElementsByTagName("Button").FirstOrDefault(e => e.Enabled && e.Text == buttonText);

            while (button == null || !button.Enabled)
            {
                button = driver.FindElementsByTagName("Button").FirstOrDefault(e => e.Enabled && e.Text == buttonText);
                Thread.Sleep(1000);
            }

            var js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("arguments[0].click()", button);
        }

        public static void AzurePage(string username, string password, string subscriptionName)
        {
            ClickButton("Connect to Azure");

            var usernameBox = driver.FindElementById("cred_userid_inputtext");
            usernameBox.SendKeys(username);

            var passwordBox = driver.FindElementById("cred_password_inputtext");
            passwordBox.SendKeys(password);

            Thread.Sleep(new TimeSpan(0, 0, 1));
            var signInButton = driver.FindElementById("cred_sign_in_button");

            var js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("arguments[0].click()", signInButton);

            var passLink = driver.FindElementsByClassName("normalText").First(e => e.Text == "Sign in with a username and password instead");
            passLink.Click();

            passwordBox = driver.FindElementById("passwordInput");
            passwordBox.SendKeys(password);

            Thread.Sleep(new TimeSpan(0, 0, 1));
            signInButton = driver.FindElementById("submitButton");
            js.ExecuteScript("arguments[0].click()", signInButton);

            Thread.Sleep(new TimeSpan(0, 0, 1));
            var acceptButton = driver.FindElementById("cred_accept_button");
            acceptButton.Click();

            var azurePage = driver.FindElementsByClassName("st-text").FirstOrDefault(e => e.Text == "Azure Subscription:");

            for (int i = 0; i < 10; i++)
            {
                azurePage = driver.FindElementsByClassName("st-text").FirstOrDefault(e => e.Text == "Azure Subscription:");
                if (azurePage != null)
                {
                    var advanced = driver.FindElementByCssSelector("p[class='st-float st-text au-target']");
                    advanced.Click();

                    var resourceGroup = driver.FindElementsByCssSelector("input[class='st-input au-target']")
                                        .First(e => e.GetAttribute("value.bind").Contains("selectedResourceGroup"));

                    resourceGroupName = Guid.NewGuid().ToString().Replace("-", "");

                    resourceGroup.Clear();
                    resourceGroup.SendKeys(resourceGroupName);

                    var option = driver.FindElementByCssSelector("select[class='btn btn-default dropdown-toggle st-input au-target']");

                    if (option != null && option.Enabled == true)
                    {
                        option = driver.FindElementByCssSelector("select[class='btn btn-default dropdown-toggle st-input au-target']");
                        option.SendKeys(subscriptionName);
                        break;
                    }
                    Thread.Sleep(new TimeSpan(0, 0, 10));
                }
                Thread.Sleep(new TimeSpan(0, 0, 10));
            }
        }

        public static void SqlPageExistingDatabase(string server, string username, string password)
        {
            Thread.Sleep(new TimeSpan(0, 0, 10));

            var option = driver.FindElementByCssSelector("select[class='btn btn-default dropdown-toggle st-input au-target']");

            for (int i = 0; i < 10; i++)
            {
                option = driver.FindElementByCssSelector("select[class='btn btn-default dropdown-toggle st-input au-target']");

                if (option != null && option.Enabled == true)
                {
                    option.SendKeys("Existing SQL Instance");
                    break;
                }
                Thread.Sleep(new TimeSpan(0, 0, 10));
            }

            var elements = driver.FindElementsByCssSelector("input[class='st-input au-target']");

            var serverBox = elements.First(e => e.GetAttribute("value.bind").Contains("sqlServer"));
            serverBox.SendKeys(server);

            var usernameBox = elements.First(e => e.GetAttribute("value.bind").Contains("username"));
            usernameBox.SendKeys(username);

            var passwordBox = elements.First(e => e.GetAttribute("value.bind").Contains("password"));
            passwordBox.SendKeys(password);

            ClickValidateButton();
        }

        public static void NoAnalysisServices()
        {
            var button = driver.FindElementByCssSelector("select[class='btn btn-default dropdown-toggle st-input au-target']");

            while (button.Enabled != true)
            {
                Thread.Sleep(new TimeSpan(0, 0, 1));
                button = driver.FindElementByCssSelector("select[class='btn btn-default dropdown-toggle st-input au-target']");
            }

            button.SendKeys("No");
        }

        public static void NewAnalysisServices(string server, string username, string password)
        {
            var button = driver.FindElementByCssSelector("select[class='btn btn-default dropdown-toggle st-input au-target']");

            while (button.Enabled != true)
            {
                Thread.Sleep(new TimeSpan(0, 0, 1));
                button = driver.FindElementByCssSelector("select[class='btn btn-default dropdown-toggle st-input au-target']");
            }

            button.SendKeys("Yes");

            ClickNextButton();
            Thread.Sleep(new TimeSpan(0, 0, 2));
            var newAas = driver.FindElementByCssSelector("select[class='btn btn-default dropdown-toggle st-input au-target']");

            while (newAas.Enabled != true)
            {
                Thread.Sleep(new TimeSpan(0, 0, 1));
                newAas = driver.FindElementByCssSelector("select[class='btn btn-default dropdown-toggle st-input au-target']");
            }

            newAas.SendKeys("New");

            var elements = driver.FindElementsByCssSelector("input[class='st-input au-target']");

            var serverBox = elements.First(e => e.GetAttribute("value.bind").Contains("server"));
            var usernameBox = elements.First(e => e.GetAttribute("value.bind").Contains("email"));
            var passwordBox = elements.First(e => e.GetAttribute("value.bind").Contains("password"));

            while (usernameBox.Enabled != true && passwordBox.Enabled != true && passwordBox.Enabled != true)
            {
                Thread.Sleep(new TimeSpan(0, 0, 1));
            }

            passwordBox.SendKeys(password);
            usernameBox.Clear();
            usernameBox.SendKeys(username);
            serverBox.SendKeys(server);

            var aasSku = driver.FindElementByCssSelector("select[class='btn btn-default dropdown-toggle st-input au-target']");

            while (aasSku.Enabled != true)
            {
                Thread.Sleep(new TimeSpan(0, 0, 1));
                aasSku = driver.FindElementByCssSelector("select[class='btn btn-default dropdown-toggle st-input au-target']");
            }

            aasSku.SendKeys("Developer");

            ClickValidateButton();
        }

        public static void SelectSqlDatabase(string databaseName)
        {
            var database = driver.FindElementsByCssSelector("select[class='btn btn-default dropdown-toggle st-input au-target']")
                            .FirstOrDefault(e => !e.Text.Contains(databaseName));

            while (database == null)
            {
                database = driver.FindElementsByCssSelector("select[class='btn btn-default dropdown-toggle st-input au-target']")
                            .FirstOrDefault(e => !e.Text.Contains(databaseName));
            }

            database.SendKeys(databaseName);
        }


        public static void CheckDeploymentStatus()
        {
            var popup = driver.FindElementByCssSelector("span[class='glyphicon pbi-glyph-close st-contact-us-close au-target']");
            popup.Click();

            var progressText = driver.FindElementsByCssSelector("span[class='semiboldFont st-progress-text']")
                                     .FirstOrDefault(e => e.Text == "All done! You can now download your Power BI report and start exploring your data.");
            var error = driver.FindElementsByCssSelector("span[class='st-tab-text st-error']")
                                     .FirstOrDefault(e => string.IsNullOrEmpty(e.Text));

            int i = 0;

            while (progressText == null && i < 60)
            {
                progressText = driver.FindElementsByCssSelector("span[class='semiboldFont st-progress-text']")
                                     .FirstOrDefault(e => e.Text == "All done! You can now download your Power BI report and start exploring your data.");

                error = driver.FindElementsByCssSelector("span[class='st-tab-text st-error']").FirstOrDefault(e => string.IsNullOrEmpty(e.Text));

                if (error != null && !string.IsNullOrEmpty(error.Text))
                {
                    Assert.Fail(error.Text);
                }

                if (progressText != null && !string.IsNullOrEmpty(progressText.Text))
                {
                    break;
                }

                i++;
                Thread.Sleep(new TimeSpan(0, 0, 10));
            }

            Assert.IsTrue(progressText != null);
            Assert.IsTrue(progressText.Text == "All done! You can now download your Power BI report and start exploring your data.");
        }

        public static void CleanSubscription(string username, string password, string tenantId, string clientId, string subscriptionId)
        {
            var creds = new UserPasswordCredential(username, password);

            var ctx = new AuthenticationContext($"https://login.windows.net/{tenantId}/oauth2/authorize");

            var token = ctx.AcquireTokenAsync("https://management.azure.com/", clientId, creds).Result;

            SubscriptionCloudCredentials cloudSubCreds = new TokenCloudCredentials(subscriptionId, token.AccessToken);
            ResourceManagementClient client = new ResourceManagementClient(cloudSubCreds);

            client.ResourceGroups.Delete(resourceGroupName);
        }
    }
}
