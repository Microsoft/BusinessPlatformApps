using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace Microsoft.Deployment.Site.Web.Tests
{
    [TestClass]
    [Ignore]
    public class SCCMTemplateTests
    {
        // For the tests to work we need the following on the test box
        // -SQL Server 2014/2016 or other flavours we want to test
        // -SCCM database name CM_PBI (Ramond's backup)
        // -Empty target database CM_PBI_Target
        // -A local administrator user TestUser with P@ssw0rd as the password
        // !!!!!! The tests need to be ran as administrator

        private string baseDownloadURL = Constants.Host + "?name=Microsoft-SCCMTemplate";
        private string msiPath = @"C:\Program Files\Microsoft Templates\Microsoft-SCCMTemplate\Microsoft.Bpst.App.Msi.exe";
        private string administratorUser = "TestUser";
        private string administratorPassword = "P@ssw0rd";
        private string sourceDatabase = "CM_PBI";
        private string targetDatabase = "CM_PBI_Target";
        private string hostName;
        private RemoteWebDriver driver;

        [TestMethod]
        public void RunSCCMTests()
        {
            GetHostName();
            DownloadAndInstallMSI();
            OpenWebBrowser();
            ClickButton("Next");
            Given_AlternativeWindowsCredentials_When_Validate_Then_Success();
            ClickButton("Next");
            Thread.Sleep(1000);
            Given_CorrectSqlCredentials_When_Validate_Then_Success();
            SelectSqlDatabase(sourceDatabase);
            ClickButton("Next");
            Thread.Sleep(1000);
            Given_CorrectSqlCredentials_When_Validate_Then_Success();
            SelectSqlDatabase(targetDatabase);
            ClickButton("Next");
            ClickButton("Validate");
            ClickButton("Next");
            Given_AllInformationCorrect_When_DeploymentFinish_Then_SuccessMessageDisplayed();
        }

        public void GetHostName()
        {
            this.hostName = System.Environment.GetEnvironmentVariable("COMPUTERNAME");
        }

        public void Given_AllInformationCorrect_When_DeploymentFinish_Then_SuccessMessageDisplayed()
        {
            var progressText = driver.FindElementsByCssSelector("span[class='semiboldFont st-progress-text']")
                                     .FirstOrDefault(e => e.Text == "All done! You can now download your Power BI report and start exploring your data.");

            int i = 0;

            while (progressText == null && i < 5)
            {
                progressText = driver.FindElementsByCssSelector("span[class='semiboldFont st-progress-text']")
                                     .FirstOrDefault(e => e.Text == "All done! You can now download your Power BI report and start exploring your data.");
                i++;
                Thread.Sleep(new TimeSpan(0, 0, 5));
            }

            Assert.IsTrue(progressText != null);
        }

        public void SelectSqlDatabase(string databaseName)
        {
            var database = driver.FindElementsByCssSelector("select[class='btn btn-default dropdown-toggle st-input au-target']")
                            .FirstOrDefault(e => !e.Text.Contains("Windows"));

            while (database == null)
            {
                database = driver.FindElementsByCssSelector("select[class='btn btn-default dropdown-toggle st-input au-target']")
                            .FirstOrDefault(e => !e.Text.Contains("Windows"));
            }
            database.SendKeys(databaseName);
        }

        public void Given_CorrectSqlCredentials_When_Validate_Then_Success()
        {
            var button = driver.FindElementsByTagName("Button").FirstOrDefault(e => e.Text == "Validate");
            while (button == null || !button.Enabled)
            {
                button = driver.FindElementsByTagName("Button").FirstOrDefault(e => e.Text == "Validate");
                Thread.Sleep(1000);
            }

            var elements = driver.FindElementsByCssSelector("input[class='st-input au-target']");
            var serverBox = elements.FirstOrDefault(e => e.GetAttribute("value.bind").Contains("sqlServer"));
            while (serverBox == null)
            {
                serverBox = elements.FirstOrDefault(e => e.GetAttribute("value.bind").Contains("sqlServer"));
            }

            serverBox.SendKeys(hostName);

            ClickButton("Validate");

            var validated = driver.FindElementByClassName("st-validated");
            Assert.IsTrue(validated.Text == "Successfully validated");
        }

        public void ValidateCurentUserCredentialsOnSourcePage()
        {
            var checkBox = driver.FindElementsByCssSelector("input[class='au-target']").First(e => e.GetAttribute("type") == "checkbox");
            var js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("arguments[0].click()", checkBox);

            var elements = driver.FindElementsByCssSelector("input[class='st-input au-target']");
            var passwordBox = elements.First(e => e.GetAttribute("value.bind").Contains("password"));
            passwordBox.SendKeys("");

            var validated = driver.FindElementByClassName("st-validated");
            Assert.IsTrue(validated.Text == "Successfully validated");
        }

        public void Given_AlternativeWindowsCredentials_When_Validate_Then_Success()
        {
            var elements = driver.FindElementsByCssSelector("input[class='st-input au-target']");

            var usernameBox = elements.First(e => e.GetAttribute("value.bind").Contains("username"));
            while (!usernameBox.Enabled)
            {
                Thread.Sleep(1000);
            }
            usernameBox.Clear();
            usernameBox.SendKeys($@"{hostName}\{administratorUser}");

            var passwordBox = elements.First(e => e.GetAttribute("value.bind").Contains("password"));
            passwordBox.SendKeys(administratorPassword);

            ClickButton("Validate");

            var validated = driver.FindElementByClassName("st-validated");

            Assert.IsTrue(validated.Text == "Successfully validated");
        }

        public void ClickButton(string buttonText)
        {
            var button = driver.FindElementsByTagName("Button").FirstOrDefault(e => e.Text == buttonText);

            while (button == null || !button.Enabled)
            {
                button = driver.FindElementsByTagName("Button").FirstOrDefault(e => e.Text == buttonText);
                Thread.Sleep(1000);
            }

            var js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("arguments[0].click()", button);
        }

        public void OpenWebBrowser()
        {
            ChromeOptions options = new ChromeOptions();
            options.BinaryLocation = msiPath;
            options.AddArgument("?name=Microsoft-SCCMTemplate");

            driver = new ChromeDriver(options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
        }

        public void DownloadAndInstallMSI()
        {
            if (File.Exists("SCCM.exe"))
            {
                File.Delete("SCCM.exe");
            }

            var downloadUrl = "https://bpstservice.azurewebsites.net/bin//Apps/Microsoft/Released/Microsoft-SCCMTemplate/Microsoft-SCCMTemplate.exe";
            using (var client = new WebClient())
            {
                client.DownloadFile(downloadUrl, "SCCM.exe");
            }

            using (var p = new Process())
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "SCCM.exe";
                startInfo.Arguments = "/install /quiet";

                p.StartInfo = startInfo;

                if (p.Start())
                {
                    while (!p.HasExited)
                    {
                        Thread.Sleep(1000);
                    }
                }

                var installer = Process.GetProcessesByName("Microsoft.Bpst.App.Msi");
                installer[0].Kill();
            }
        }
    }
}