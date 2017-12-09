using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Fakes;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RedditCore;
using RedditCore.AzureML;
using RedditCore.Logging;

namespace RedditCoreTest.AzureML
{
    [TestClass]
    public class AzureMLExperimentRunnerTest
    {
        [TestMethod]
        public async Task JobSubmitFails()
        {
            var config = new Mock<IConfiguration>();
            config.SetupGet(x => x.AzureMLApiKey).Returns("MyAPIKey");
            config.SetupGet(x => x.AzureMLBaseUrl).Returns("https://azureml.example.com");

            using (ShimsContext.Create())
            {
                // Setup the shim
                ShimHttpClient.AllInstances.PostAsyncStringHttpContent = (client, url, content) =>
                {
                    // Checks for correct auth parameters
                    Assert.AreEqual(new AuthenticationHeaderValue("Bearer", "MyAPIKey"),
                        client.DefaultRequestHeaders.Authorization);

                    if (url == "https://azureml.example.com?api-version=2.0")
                    {
                        var result = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                        result.Content = new ByteArrayContent(Encoding.ASCII.GetBytes("SomeError"));

                        return Task.FromResult(result);
                    }
                    // URL must not be correct.  Fail the test
                    Assert.Fail($"Tried to connect to URL {url} that was not expected");

                    throw new Exception("ShouldNotGetHere");
                };

                // Run the tests
                var experimentCompletionWaiter = new Mock<IExperimentCompletionWaiter>();

                var runner = new AzureMLExperimentRunner(
                    new ConsoleLog(), 
                    config.Object,
                    experimentCompletionWaiter.Object,
                    new NoOpObjectLogger());

                var finalResult = await runner.RunExperimentWithData("MyData", false);

                // Assert results
                Assert.IsTrue(finalResult.Success.HasValue);
                Assert.IsFalse(finalResult.Success.Value, "Job should have failed");
                Assert.AreEqual("Unable to submit job", finalResult.Error);

                // Verify that the waiter was never called.
                experimentCompletionWaiter.Verify(x => x.WaitForJobCompletion(It.IsAny<string>()), Times.Never);
            }
        }

        [TestMethod]
        public async Task JobStartFailsAfterSubmitSucceeds()
        {
            var config = new Mock<IConfiguration>();
            config.SetupGet(x => x.AzureMLApiKey).Returns("MyAPIKey");
            config.SetupGet(x => x.AzureMLBaseUrl).Returns("https://azureml.example.com");

            using (ShimsContext.Create())
            {
                // Setup the shim
                ShimHttpClient.AllInstances.PostAsyncStringHttpContent = (client, url, content) =>
                {
                    // Checks for correct auth parameters
                    Assert.AreEqual(new AuthenticationHeaderValue("Bearer", "MyAPIKey"),
                        client.DefaultRequestHeaders.Authorization);

                    if (url == "https://azureml.example.com?api-version=2.0")
                    {
                        var result = new HttpResponseMessage(HttpStatusCode.OK);
                        result.Content = new ByteArrayContent(Encoding.ASCII.GetBytes("\"MyJobID\""));

                        return Task.FromResult(result);
                    }
                    if (url == "https://azureml.example.com/MyJobID/start?api-version=2.0")
                    {
                        var result = new HttpResponseMessage(HttpStatusCode.InternalServerError);

                        return Task.FromResult(result);
                    }
                    // URL must not be correct.  Fail the test
                    Assert.Fail($"Tried to connect to URL {url} that was not expected");

                    throw new Exception("ShouldNotGetHere");
                };

                // Run the tests
                var experimentCompletionWaiter = new Mock<IExperimentCompletionWaiter>();

                var runner = new AzureMLExperimentRunner(
                    new ConsoleLog(), 
                    config.Object,
                    experimentCompletionWaiter.Object,
                    new NoOpObjectLogger());

                var finalResult = await runner.RunExperimentWithData("MyData", false);

                // Assert results
                Assert.IsTrue(finalResult.Success.HasValue);
                Assert.IsFalse(finalResult.Success.Value, "Job should have failed");
                Assert.AreEqual("Unable to start job MyJobID", finalResult.Error);

                // Verify that the waiter was never called.
                experimentCompletionWaiter.Verify(x => x.WaitForJobCompletion(It.IsAny<string>()), Times.Never);
            }
        }

        [TestMethod]
        public async Task SubmitsJobDoNotWaitForCompletion()
        {
            var config = new Mock<IConfiguration>();
            config.SetupGet(x => x.AzureMLApiKey).Returns("MyAPIKey");
            config.SetupGet(x => x.AzureMLBaseUrl).Returns("https://azureml.example.com");

            using (ShimsContext.Create())
            {
                // Setup the shim
                ShimHttpClient.AllInstances.PostAsyncStringHttpContent = (client, url, content) =>
                {
                    // Checks for correct auth parameters
                    Assert.AreEqual(new AuthenticationHeaderValue("Bearer", "MyAPIKey"),
                        client.DefaultRequestHeaders.Authorization);

                    if (url == "https://azureml.example.com?api-version=2.0")
                    {
                        var result = new HttpResponseMessage(HttpStatusCode.OK);
                        result.Content = new ByteArrayContent(Encoding.ASCII.GetBytes("\"MyJobID\""));

                        return Task.FromResult(result);
                    }
                    if (url == "https://azureml.example.com/MyJobID/start?api-version=2.0")
                    {
                        var result = new HttpResponseMessage(HttpStatusCode.OK);

                        return Task.FromResult(result);
                    }
                    // URL must not be correct.  Fail the test
                    Assert.Fail($"Tried to connect to URL {url} that was not expected");

                    throw new Exception("ShouldNotGetHere");
                };

                // Run the tests
                var experimentCompletionWaiter = new Mock<IExperimentCompletionWaiter>();

                var runner = new AzureMLExperimentRunner(
                    new ConsoleLog(), 
                    config.Object,
                    experimentCompletionWaiter.Object,
                    new NoOpObjectLogger());

                var finalResult = await runner.RunExperimentWithData("MyData", false);

                // Assert results
                Assert.IsFalse(finalResult.Success.HasValue,
                    "Result should not have a value since we did not wait for the job to finish");

                // Verify that the waiter was never called.
                experimentCompletionWaiter.Verify(x => x.WaitForJobCompletion(It.IsAny<string>()), Times.Never);
            }
        }

        [TestMethod]
        public async Task SubmitsJobWaitForCompletion()
        {
            var config = new Mock<IConfiguration>();
            config.SetupGet(x => x.AzureMLApiKey).Returns("MyAPIKey");
            config.SetupGet(x => x.AzureMLBaseUrl).Returns("https://azureml.example.com");

            using (ShimsContext.Create())
            {
                // Setup the shim
                ShimHttpClient.AllInstances.PostAsyncStringHttpContent = (client, url, content) =>
                {
                    // Checks for correct auth parameters
                    Assert.AreEqual(new AuthenticationHeaderValue("Bearer", "MyAPIKey"),
                        client.DefaultRequestHeaders.Authorization);

                    if (url == "https://azureml.example.com?api-version=2.0")
                    {
                        var result = new HttpResponseMessage(HttpStatusCode.OK);
                        result.Content = new ByteArrayContent(Encoding.ASCII.GetBytes("\"MyJobID\""));

                        return Task.FromResult(result);
                    }
                    if (url == "https://azureml.example.com/MyJobID/start?api-version=2.0")
                    {
                        var result = new HttpResponseMessage(HttpStatusCode.OK);

                        return Task.FromResult(result);
                    }
                    // URL must not be correct.  Fail the test
                    Assert.Fail($"Tried to connect to URL {url} that was not expected");

                    throw new Exception("ShouldNotGetHere");
                };

                // Run the tests
                var experimentCompletionWaiter = new Mock<IExperimentCompletionWaiter>();
                experimentCompletionWaiter.Setup(x => x.WaitForJobCompletion(It.IsIn("MyJobID")))
                    .Returns(Task.FromResult(new AzureMLResult {Success = true}));

                var runner = new AzureMLExperimentRunner(
                    new ConsoleLog(), 
                    config.Object,
                    experimentCompletionWaiter.Object,
                    new NoOpObjectLogger());

                var finalResult = await runner.RunExperimentWithData("MyData", true);

                // Assert results
                Assert.IsTrue(finalResult.Success.HasValue,
                    "Result should have a value since we waited for the job to finish");
                Assert.IsTrue(finalResult.Success.Value, "Job should have succeeded");

                // Verify that the waiter was never called.
                experimentCompletionWaiter.Verify(x => x.WaitForJobCompletion(It.IsAny<string>()), Times.Once);
            }
        }
    }
}
