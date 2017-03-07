using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace Microsoft.Deployment.Site.Web.Tests
{
    public class HelperMethods
    {
        public static string baseURL;
        public static RemoteWebDriver driver;

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

            while (progressText == null && i < 10)
            {
                progressText = driver.FindElementsByCssSelector("span[class='semiboldFont st-progress-text']")
                                     .FirstOrDefault(e => e.Text == "All done! You can now download your Power BI report and start exploring your data.");

                error = driver.FindElementsByCssSelector("span[class='st-tab-text st-error']").FirstOrDefault(e => string.IsNullOrEmpty(e.Text));

                if (error != null && !string.IsNullOrEmpty(error.Text))
                {
                    Assert.Fail(error.Text);
                }

                i++;
                Thread.Sleep(new TimeSpan(0, 0, 10));
            }

            Assert.IsTrue(progressText.Text == "All done! You can now download your Power BI report and start exploring your data.");
        }
    }
}
