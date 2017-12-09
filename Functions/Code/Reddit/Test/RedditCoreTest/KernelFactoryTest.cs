using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ninject;
using RedditCore;
using RedditCore.AzureML;
using RedditCore.Logging;
using RedditCore.SocialGist;
using RedditCore.Telemetry;

namespace RedditCoreTest
{
    [TestClass]
    public class KernelFactoryTest
    {
        /// <summary>
        ///     This is a basic test to ensure that everything is wired correctly in the container.
        /// </summary>
        [TestMethod]
        public void CreatesAzureMLExperimentRunner()
        {
            // Create an empty config
            var config = Mock.Of<IConfiguration>();

            var kernel = new KernelFactory().GetKernel(new ConsoleLog(), config);

            var runner = kernel.Get<IAzureMLExperimentRunner>();

            Assert.IsNotNull(runner);
        }

        /// <summary>
        ///     This is a basic test to ensure that everything is wired correctly in the container.
        /// </summary>
        [TestMethod]
        public void CreatesSocialGist()
        {
            // Create an empty config
            var config = Mock.Of<IConfiguration>();

            var kernel = new KernelFactory().GetKernel(new ConsoleLog(), config);

            var runner = kernel.Get<ISocialGist>();

            Assert.IsNotNull(runner);
        }

        [TestMethod]
        public void CreateRedditPostProcessor()
        {
            var config = Mock.Of<IConfiguration>();

            var kernel = new KernelFactory().GetKernel(new ConsoleLog(), config);

            var processor = kernel.Get<IThreadProcessor>();

            Assert.IsNotNull(processor);
        }

        [TestMethod]
        public void CreateTelemetryClient()
        {
            var config = new Mock<IConfiguration>();
            config.SetupGet(x => x.FunctionInvocationId).Returns(new System.Guid("00000000-0000-0000-0000-000000001234"));
            config.SetupGet(x => x.FunctionName).Returns("MyTestFunction");

            var kernel = new KernelFactory().GetKernel(new ConsoleLog(), config.Object);

            var telemetryClient = kernel.Get<ITelemetryClient>();

            Assert.IsNotNull(telemetryClient);

            telemetryClient.TrackEvent("My Event Name");
        }

        /// <summary>
        ///     This is a basic test to ensure that everything is wired correctly in the container.
        /// </summary>
        [TestMethod]
        public void CreatesScheduledAzureMlProcessor()
        {
            // Create an empty config
            var config = Mock.Of<IConfiguration>();

            var kernel = new KernelFactory().GetKernel(new ConsoleLog(), config);

            var processor = kernel.Get<IScheduledAzureMLProcessor>();

            Assert.IsNotNull(processor);
        }
    }
}
