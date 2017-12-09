using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ninject;
using Ninject.MockingKernel.Moq;
using RedditCore;
using RedditCore.AzureML;
using RedditCore.Http;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace RedditCoreTest.AzureML
{
    [TestClass]
    public class ExperimentCompletionWaiterTest
    {
        private ExperimentCompletionWaiter waiter;
        private Mock<IHttpClient> httpClient;

        [TestInitialize]
        public void SetupTest()
        {
            var kernel = new MoqMockingKernel();

            kernel.GetMock<IConfiguration>().SetupGet(x => x.AzureMLBaseUrl).Returns("https://www.example.com/jobs?api-version=2.0");
            kernel.GetMock<IConfiguration>().SetupGet(x => x.AzureMLApiKey).Returns("MyAPIKey");

            this.httpClient = kernel.GetMock<IHttpClient>();
            this.waiter = kernel.Get<ExperimentCompletionWaiter>();
        }

        [TestMethod]
        public async Task ImmediateSuccess()
        {
            this.httpClient.Setup(x => x.GetJsonAsync<BatchScoreStatus>(
                new Uri("https://www.example.com/jobs/MyJobId?api-version=2.0"),
                null,
                new AuthenticationHeaderValue("Bearer", "MyAPIKey")
                ))
                .Returns(Task.FromResult(CreateSuccessResponse()));
            var result = await waiter.WaitForJobCompletion("MyJobId");

            // Job completed successfully
            Assert.IsTrue(result.Success.GetValueOrDefault(false));
            Assert.AreEqual(BatchScoreStatusCode.Finished, result.LastJobStatus);
            Assert.AreEqual("MyJobId", result.JobId);
        }

        [TestMethod]
        public async Task NotStartedThenSuccess()
        {
            this.httpClient.SetupSequence(x => x.GetJsonAsync<BatchScoreStatus>(
                new Uri("https://www.example.com/jobs/MyJobId?api-version=2.0"),
                null,
                new AuthenticationHeaderValue("Bearer", "MyAPIKey")
                ))
                .Returns(Task.FromResult(CreateNotStartedResponse()))
                .Returns(Task.FromResult(CreateSuccessResponse()));
            var result = await waiter.WaitForJobCompletion("MyJobId");

            // Job completed successfully
            Assert.IsTrue(result.Success.GetValueOrDefault(false));
            Assert.AreEqual(BatchScoreStatusCode.Finished, result.LastJobStatus);
            Assert.AreEqual("MyJobId", result.JobId);

            this.httpClient.Verify(x => x.GetJsonAsync<BatchScoreStatus>(
                    new Uri("https://www.example.com/jobs/MyJobId?api-version=2.0"),
                    null,
                    new AuthenticationHeaderValue("Bearer", "MyAPIKey")),
                Times.Exactly(2));
        }

        [TestMethod]
        public async Task NotStartedThenRunningThenSuccess()
        {
            this.httpClient.SetupSequence(x => x.GetJsonAsync<BatchScoreStatus>(
                new Uri("https://www.example.com/jobs/MyJobId?api-version=2.0"),
                null,
                new AuthenticationHeaderValue("Bearer", "MyAPIKey")
                ))
                .Returns(Task.FromResult(CreateNotStartedResponse()))
                .Returns(Task.FromResult(CreateRunningResponse()))
                .Returns(Task.FromResult(CreateSuccessResponse()));
            var result = await waiter.WaitForJobCompletion("MyJobId");

            // Job completed successfully
            Assert.IsTrue(result.Success.GetValueOrDefault(false));
            Assert.AreEqual(BatchScoreStatusCode.Finished, result.LastJobStatus);
            Assert.AreEqual("MyJobId", result.JobId);

            this.httpClient.Verify(x => x.GetJsonAsync<BatchScoreStatus>(
                    new Uri("https://www.example.com/jobs/MyJobId?api-version=2.0"),
                    null,
                    new AuthenticationHeaderValue("Bearer", "MyAPIKey")),
                Times.Exactly(3));
        }

        [TestMethod]
        public async Task NotStartedThenRunningThenFailed()
        {
            this.httpClient.SetupSequence(x => x.GetJsonAsync<BatchScoreStatus>(
                new Uri("https://www.example.com/jobs/MyJobId?api-version=2.0"),
                null,
                new AuthenticationHeaderValue("Bearer", "MyAPIKey")
                ))
                .Returns(Task.FromResult(CreateNotStartedResponse()))
                .Returns(Task.FromResult(CreateRunningResponse()))
                .Returns(Task.FromResult(CreateFailureResponse()));
            var result = await waiter.WaitForJobCompletion("MyJobId");

            // Job failed
            Assert.IsTrue(result.Success.HasValue);
            Assert.IsFalse(result.Success.Value);
            Assert.AreEqual(BatchScoreStatusCode.Failed, result.LastJobStatus);
            Assert.AreEqual("MyJobId", result.JobId);

            this.httpClient.Verify(x => x.GetJsonAsync<BatchScoreStatus>(
                    new Uri("https://www.example.com/jobs/MyJobId?api-version=2.0"),
                    null,
                    new AuthenticationHeaderValue("Bearer", "MyAPIKey")),
                Times.Exactly(3));
        }

        [TestMethod]
        public async Task RunsForeverAndNeedsToBeKilled()
        {
            this.httpClient.Setup(x => x.GetJsonAsync<BatchScoreStatus>(
                new Uri("https://www.example.com/jobs/MyJobId?api-version=2.0"),
                null,
                new AuthenticationHeaderValue("Bearer", "MyAPIKey")
                ))
                .Returns(Task.FromResult(CreateRunningResponse()));

            // Set the AzureML timeout really low so we can test the fail-safe cancel mechanism
            var result = await waiter.WaitForJobCompletion("MyJobId", TimeSpan.FromSeconds(3), true);

            // Job failed
            Assert.IsTrue(result.Success.HasValue);
            Assert.IsFalse(result.Success.Value);
            Assert.AreEqual("MyJobId", result.JobId);

            // Task was running when checked so this will be "running"
            Assert.AreEqual(BatchScoreStatusCode.Running, result.LastJobStatus);

            this.httpClient.Verify(x => x.DeleteAsync(
                    new Uri("https://www.example.com/jobs/MyJobId?api-version=2.0"),
                    null,
                    new AuthenticationHeaderValue("Bearer", "MyAPIKey")),
                Times.Exactly(1));
        }

        [TestMethod]
        public async Task RunsForeverAndIsNotKilled()
        {
            this.httpClient.Setup(x => x.GetJsonAsync<BatchScoreStatus>(
                new Uri("https://www.example.com/jobs/MyJobId?api-version=2.0"),
                null,
                new AuthenticationHeaderValue("Bearer", "MyAPIKey")
                ))
                .Returns(Task.FromResult(CreateRunningResponse()));

            // Set the AzureML timeout really low so we can test the fail-safe cancel mechanism
            var result = await waiter.WaitForJobCompletion("MyJobId", TimeSpan.FromSeconds(3), false);

            // Job failed
            Assert.IsTrue(result.Success.HasValue);
            Assert.IsFalse(result.Success.Value);
            Assert.AreEqual(BatchScoreStatusCode.Running, result.LastJobStatus);
            Assert.AreEqual("MyJobId", result.JobId);

            // Verify that the delete was never called
            this.httpClient.Verify(x => x.DeleteAsync(
                    new Uri("https://www.example.com/jobs/MyJobId?api-version=2.0"),
                    null,
                    new AuthenticationHeaderValue("Bearer", "MyAPIKey")),
                Times.Never());
        }

        private static IHttpJsonResponseMessage<BatchScoreStatus> CreateRunningResponse()
        {
            return new HttpJsonResponseMessage<BatchScoreStatus>()
            {
                Object = new BatchScoreStatus() { StatusCode = BatchScoreStatusCode.Running },
                ResponseMessage = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("This is the content") }
            };
        }

        private static IHttpJsonResponseMessage<BatchScoreStatus> CreateNotStartedResponse()
        {
            return new HttpJsonResponseMessage<BatchScoreStatus>()
            {
                Object = new BatchScoreStatus() { StatusCode = BatchScoreStatusCode.NotStarted },
                ResponseMessage = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("This is the content") }
            };
        }

        private static IHttpJsonResponseMessage<BatchScoreStatus> CreateSuccessResponse()
        {
            return new HttpJsonResponseMessage<BatchScoreStatus>()
            {
                Object = new BatchScoreStatus() { StatusCode = BatchScoreStatusCode.Finished },
                ResponseMessage = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("This is the content") }
            };
        }

        private static IHttpJsonResponseMessage<BatchScoreStatus> CreateFailureResponse()
        {
            return new HttpJsonResponseMessage<BatchScoreStatus>()
            {
                Object = new BatchScoreStatus() { StatusCode = BatchScoreStatusCode.Failed },
                ResponseMessage = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("This is the content") }
            };
        }
    }
}
