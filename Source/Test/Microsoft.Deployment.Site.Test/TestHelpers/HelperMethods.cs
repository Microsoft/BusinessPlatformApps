using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using Microsoft.Azure;
using Microsoft.Azure.Management.Resources;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System.Collections.Generic;

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

        public static void WaitForPage()
        {
            IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));

            wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
        }

        public static void ClickButton(string buttonText)
        {
            WaitForPage();
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

            try
            {
                Thread.Sleep(new TimeSpan(0, 0, 1));
                var signInButton = driver.FindElementById("cred_sign_in_button");

                var js = (IJavaScriptExecutor)driver;
                js.ExecuteScript("arguments[0].click()", signInButton);
            }
            catch
            {
                //MSI ccase
            }
            var djs = (IJavaScriptExecutor)driver;
            var passLink = driver.FindElementsByClassName("normalText").First(e => e.Text == "Sign in with a username and password instead");
            djs.ExecuteScript("arguments[0].click()", passLink);

            passwordBox = driver.FindElementById("passwordInput");
            passwordBox.SendKeys(password);

            Thread.Sleep(new TimeSpan(0, 0, 5));
            var submitButton = driver.FindElementById("submitButton");
            djs.ExecuteScript("arguments[0].click()", submitButton);

            Thread.Sleep(new TimeSpan(0, 0, 5));
            var acceptButton = driver.FindElementById("cred_accept_button");
            djs.ExecuteScript("arguments[0].click()", acceptButton);

            WaitForPage();

            var azurePage = driver.FindElementsByClassName("st-text").FirstOrDefault(e => e.Text == "Azure Subscription:");

            for (int i = 0; i < 10; i++)
            {
                azurePage = driver.FindElementsByClassName("st-text").FirstOrDefault(e => e.Text == "Azure Subscription:");
                if (azurePage != null)
                {
                    var advanced = driver.FindElementByCssSelector("p[class='st-float st-text au-target']");
                    djs.ExecuteScript("arguments[0].click()", advanced);

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

        public static void Dynamics365Page(string username, string password, string organization)
        {
            ClickButton("Connect");
            Thread.Sleep(new TimeSpan(0, 0, 10));

            var usernameBox = driver.FindElementById("cred_userid_inputtext");
            usernameBox.SendKeys(username);

            var passwordBox = driver.FindElementById("cred_password_inputtext");
            passwordBox.SendKeys(password);

            try
            {
                Thread.Sleep(new TimeSpan(0, 0, 1));
                var signInButton = driver.FindElementById("cred_sign_in_button");

                var js = (IJavaScriptExecutor)driver;
                js.ExecuteScript("arguments[0].click()", signInButton);
            }
            catch
            {
                //MSI ccase
                var js = (IJavaScriptExecutor)driver;
                var passLink = driver.FindElementsByClassName("normalText").First(e => e.Text == "Sign in with a username and password instead");
                js.ExecuteScript("arguments[0].click()", passLink);

                passwordBox = driver.FindElementById("passwordInput");
                passwordBox.SendKeys(password);

                Thread.Sleep(new TimeSpan(0, 0, 5));
                var submitButton = driver.FindElementById("submitButton");
                js.ExecuteScript("arguments[0].click()", submitButton);
            }

            var djs = (IJavaScriptExecutor)driver;
            Thread.Sleep(new TimeSpan(0, 0, 5));
            var acceptButton = driver.FindElementById("cred_accept_button");
            djs.ExecuteScript("arguments[0].click()", acceptButton);

            WaitForPage();

            var azurePage = driver.FindElementsByClassName("st-text").FirstOrDefault(e => e.Text == "Dynamics 365 Organization:");

            for (int i = 0; i < 10; i++)
            {
                azurePage = driver.FindElementsByClassName("st-text").FirstOrDefault(e => e.Text == "Dynamics 365 Organization:");
                if (azurePage != null)
                {
                    Thread.Sleep(new TimeSpan(0, 0, 10));

                    var org = driver.FindElementByCssSelector("select[class='btn btn-default dropdown-toggle st-input au-target']");
                    org.SendKeys(organization);

                    var advanced = driver.FindElementByCssSelector("p[class='st-float st-text au-target']");
                    djs.ExecuteScript("arguments[0].click()", advanced);

                    var resourceGroup = driver.FindElementsByCssSelector("input[class='st-input au-target']")
                                        .First(e => e.GetAttribute("value.bind").Contains("selectedResourceGroup"));

                    resourceGroupName = "delete_" + Guid.NewGuid().ToString().Replace("-", "");

                    resourceGroup.Clear();
                    resourceGroup.SendKeys(resourceGroupName);

                    Thread.Sleep(new TimeSpan(0, 0, 50));

                    return;
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

            Thread.Sleep(new TimeSpan(0, 0, 2));

            var passwordBox = elements.First(e => e.GetAttribute("value.bind").Contains("password"));
            passwordBox.SendKeys(password);

            ClickButton("Validate");
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

            ClickButton("Next");
            Thread.Sleep(new TimeSpan(0, 0, 2));
            var newAas = driver.FindElementByCssSelector("select[class='btn btn-default dropdown-toggle st-input au-target']");

            while (newAas.Enabled != true)
            {
                Thread.Sleep(new TimeSpan(0, 0, 1));
                newAas = driver.FindElementByCssSelector("select[class='btn btn-default dropdown-toggle st-input au-target']");
            }

            newAas.SendKeys("New");
            Thread.Sleep(new TimeSpan(0, 0, 2));

            var elements = driver.FindElementsByCssSelector("input[class='st-input au-target']");

            var serverBox = elements.FirstOrDefault(e => e.GetAttribute("value.bind").Contains("server"));
            var usernameBox = elements.FirstOrDefault(e => e.GetAttribute("value.bind").Contains("email"));
            var passwordBox = elements.FirstOrDefault(e => e.GetAttribute("value.bind").Contains("password"));

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

            ClickButton("Validate");
        }

        public static void SelectSqlDatabase(string databaseName)
        {
            var database = driver.FindElementsByCssSelector("select[class='btn btn-default dropdown-toggle st-input au-target']")
                            .FirstOrDefault(e => e.Text.Contains(databaseName));

            while (database == null)
            {
                database = driver.FindElementsByCssSelector("select[class='btn btn-default dropdown-toggle st-input au-target']")
                            .FirstOrDefault(e => e.Text.Contains(databaseName));
            }

            database.SendKeys(databaseName);
        }


        public static void CheckDeploymentStatus()
        {
            Thread.Sleep(new TimeSpan(0, 0, 5));
            try
            {
                var popup = driver.FindElementByCssSelector("span[class='glyphicon pbi-glyph-close st-contact-us-close au-target']");
                popup.Click();
            }
            catch
            {
                //MSI scenario - carry on
            }
            var progressText = driver.FindElementsByCssSelector("span[class='semiboldFont st-progress-text']")
                                     .FirstOrDefault(e => e.Text.Contains("The deployment has been successfully completed!"));
            var error = driver.FindElementsByCssSelector("span[class='st-tab-text st-error']")
                                     .FirstOrDefault(e => !string.IsNullOrEmpty(e.Text));

            for (int i = 0; progressText == null && i < 60; i++)
            {
                error = driver.FindElementsByCssSelector("span[class='st-tab-text st-error']").FirstOrDefault(e => !string.IsNullOrEmpty(e.Text));

                if (error != null && !string.IsNullOrEmpty(error.Text))
                {
                    Assert.Fail(error.Text);
                }

                progressText = driver.FindElementsByCssSelector("span[class='semiboldFont st-progress-text']")
                                    .FirstOrDefault(e => e.Text.Contains("The deployment has been successfully completed!"));

                if (progressText != null && !string.IsNullOrEmpty(progressText.Text))
                {
                    break;
                }
                
                Thread.Sleep(new TimeSpan(0, 0, 10));
            }

            Assert.IsTrue(progressText != null);
            Assert.IsTrue(progressText.Text.Contains("The deployment has been successfully completed!"));
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

        public static void CreateDatabase(string server, string username, string password, string database)
        {
            string sqlConnectionString = getSqlConnectionString(server, username, password, "master");
            string script = "CREATE DATABASE " + database +
                            " COLLATE Latin1_General_100_CI_AS ( EDITION = 'Standard', SERVICE_OBJECTIVE = 'S1' )";

            using (SqlConnection connection = new SqlConnection(sqlConnectionString))
            {
                using (SqlCommand command = new SqlCommand(script, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteDatabase(string server, string username, string password, string database)
        {
            string sqlConnectionString = getSqlConnectionString(server, username, password, "master");
            string script = "DROP DATABASE " + database;

            using (SqlConnection connection = new SqlConnection(sqlConnectionString))
            {
                using (SqlCommand command = new SqlCommand(script, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public static int rowsInAllTables(string server, string username, string password, string database)
        {
            int result = -1;
            string sqlConnectionString = getSqlConnectionString(server, username, password, database);
            string script = "SELECT t.name, s.row_count " + 
                            "FROM sys.tables t " +
                            "JOIN sys.dm_db_partition_stats s " +
                            "ON t.object_id = s.object_id";

            using (SqlConnection connection = new SqlConnection(sqlConnectionString))
            {
                using (SqlCommand command = new SqlCommand(script, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    result = 0;
                    while (reader.Read())
                    {
                        string table = (string)reader.GetSqlString(0);
                        int numRows = (int)reader.GetInt64(1);

                        if (!string.IsNullOrEmpty(table))
                        {
                            result += numRows;
                        }
                    }
                    reader.Close();
                }
            }

            return result;
        }

        public static string getSqlConnectionString(string server, string username, string password, string database)
        {
            string result = $"Data Source={server};Initial Catalog={database};Integrated Security=False;User ID={username};Password={password};" +
                            "Connect Timeout=75;Encrypt=True;TrustServerCertificate=False";
            return result;
        }


    }
}
