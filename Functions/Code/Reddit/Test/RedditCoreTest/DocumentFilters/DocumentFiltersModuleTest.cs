using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ninject;
using Ninject.MockingKernel.Moq;
using RedditCore;
using RedditCore.DocumentFilters;
using System.Linq;

namespace RedditCoreTest.DocumentFilters
{
    [TestClass]
    public class DocumentFiltersModuleTest
    {
        [TestMethod]
        public void UsesContainsUDEFilter()
        {
            var config = new Mock<IConfiguration>();
            config.SetupGet(x => x.IngestOnlyDocumentsWithUserDefinedEntities).Returns(true);

            var kernel = new MoqMockingKernel(new NinjectSettings(), new DocumentFiltersModule(config.Object));

            var filters = kernel.Get<FilterHolder>();

            var result = filters.DocumentFilters.Any(x => x is ContainsUserDefinedEntitiesFilter);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void DoesNotUseContainsUDEFilter()
        {
            var config = new Mock<IConfiguration>();
            config.SetupGet(x => x.IngestOnlyDocumentsWithUserDefinedEntities).Returns(false);

            FilterHolder filters = CreateFilterHolder(config);

            var result = filters.DocumentFilters.Any(x => x is ContainsUserDefinedEntitiesFilter);

            Assert.IsFalse(result);
        }

        private static FilterHolder CreateFilterHolder(Mock<IConfiguration> config)
        {
            var kernel = new MoqMockingKernel(new NinjectSettings(), new DocumentFiltersModule(config.Object));

            var filters = kernel.Get<FilterHolder>();
            return filters;
        }

        private class FilterHolder
        {
            [Inject]
            public IDocumentFilter[] DocumentFilters { get; set; }
        }
    }
}
