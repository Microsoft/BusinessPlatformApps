using Microsoft.VisualStudio.TestTools.UnitTesting;
using RedditCore.DocumentFilters;
using RedditCore.SocialGist;
using Moq;
using System.Collections.Generic;
using RedditCore.DataModel;

namespace RedditCoreTest.DocumentFilters
{
    [TestClass]
    public class ContainsUserDefinedEntitiesFilterTest
    {
        [TestMethod]
        public void NoUserDefinedEntities()
        {
            var entityFinder = new Mock<IUserDefinedEntityFinder>();
            var filter = new ContainsUserDefinedEntitiesFilter(entityFinder.Object);

            entityFinder.Setup(x => x.FindAllUserDefinedEntities(It.IsAny<IEnumerable<IDocument>>())).Returns(new UserDefinedEntity[] { });

            var result = filter.ShouldKeep(CreateDocument("id", "text"));

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void OneUserDefinedEntity()
        {
            var entityFinder = new Mock<IUserDefinedEntityFinder>();
            var filter = new ContainsUserDefinedEntitiesFilter(entityFinder.Object);

            entityFinder.Setup(x => x.FindAllUserDefinedEntities(It.IsAny<IEnumerable<IDocument>>()))
                .Returns(
                new UserDefinedEntity[] {
                    new UserDefinedEntity() {
                        DocumentId = "id",
                        EntityLength = 3,
                        EntityOffset = 1,
                        EntityType = "type",
                        Entity = "value"
                    }
                });

            var result = filter.ShouldKeep(CreateDocument("id", "text"));

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TwoUserDefinedEntities()
        {
            var entityFinder = new Mock<IUserDefinedEntityFinder>();
            var filter = new ContainsUserDefinedEntitiesFilter(entityFinder.Object);

            entityFinder.Setup(x => x.FindAllUserDefinedEntities(It.IsAny<IEnumerable<IDocument>>()))
                .Returns(
                new UserDefinedEntity[] {
                    new UserDefinedEntity() {
                        DocumentId = "id",
                        EntityLength = 3,
                        EntityOffset = 1,
                        EntityType = "type",
                        Entity = "value"
                    },
                    new UserDefinedEntity() {
                        DocumentId = "id",
                        EntityLength = 3,
                        EntityOffset = 10,
                        EntityType = "type1",
                        Entity = "value1"
                    }
                });

            var result = filter.ShouldKeep(CreateDocument("id", "text"));

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void OneUserDefinedEntity_InvalidComment()
        {
            var entityFinder = new Mock<IUserDefinedEntityFinder>();
            var filter = new ContainsUserDefinedEntitiesFilter(entityFinder.Object);

            entityFinder.Setup(x => x.FindAllUserDefinedEntities(It.IsAny<IEnumerable<IDocument>>()))
                .Returns(
                new UserDefinedEntity[] {
                    new UserDefinedEntity() {
                        DocumentId = "id",
                        EntityLength = 3,
                        EntityOffset = 1,
                        EntityType = "type",
                        Entity = "value"
                    }
                });

            var result = filter.ShouldKeep(new InvalidComment() { Id = "id" });

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void NoUserDefinedEntities_InvalidComment()
        {
            var entityFinder = new Mock<IUserDefinedEntityFinder>();
            var filter = new ContainsUserDefinedEntitiesFilter(entityFinder.Object);

            entityFinder.Setup(x => x.FindAllUserDefinedEntities(It.IsAny<IEnumerable<IDocument>>())).Returns(new UserDefinedEntity[] { });

            var result = filter.ShouldKeep(new InvalidComment() { Id = "id" });

            Assert.IsTrue(result);
        }

        private IDocument CreateDocument(string id, string text)
        {
            var doc = new Mock<IDocument>();
            doc.SetupGet(x => x.Id).Returns(id);
            doc.SetupGet(x => x.Content).Returns(text);

            return doc.Object;
        }
    }
}
