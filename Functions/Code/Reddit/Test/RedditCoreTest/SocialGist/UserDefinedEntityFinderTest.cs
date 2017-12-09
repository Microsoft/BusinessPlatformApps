using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Ninject.MockingKernel.Moq;
using RedditCore.DataModel;
using RedditCore.DataModel.Repositories;
using RedditCore.SocialGist;

namespace RedditCoreTest.SocialGist
{
    [TestClass]
    public class UserDefinedEntityFinderTest
    {
        private class Document : IDocument
        {
            public string Id { get; set; }
            public string Content { get; set; }
        }

        private readonly MoqMockingKernel kernel;

        public UserDefinedEntityFinderTest()
        {
            kernel = new MoqMockingKernel();
            kernel.Bind<IUserDefinedEntityFinder>().To<UserDefinedEntityFinder>();
        }

        [TestInitialize]
        public void SetUp()
        {
            kernel.Reset();
            //kernel.Inject(this);

        }

        [TestMethod]
        public void SingleEntityDefinition_SingleDocument()
        {
            // Setup the repository
            var definitions = new List<UserDefinedEntityDefinition>
            {
                new UserDefinedEntityDefinition()
                {
                    Regex = "parks?",
                    EntityType = "NationalTreasure",
                    EntityValue = "PaRk"
                }
            };
            this.kernel.GetMock<IRepository<UserDefinedEntityDefinition>>().Setup(x => x.GetAll()).Returns(definitions);

            // Load the finder
            var entityFinder = kernel.Get<IUserDefinedEntityFinder>();

            var result = entityFinder.FindAllUserDefinedEntities(new IDocument[]
                {new Document() {Id = "DOC1", Content = "The national parks are great"}}).ToList();

            Assert.AreEqual(1, result.Count());
            CollectionAssert.AreEquivalent(
                new List<UserDefinedEntity>()
                {
                    new UserDefinedEntity()
                    {
                        DocumentId = "DOC1",
                        Entity = "PaRk",
                        EntityLength = 5,
                        EntityOffset = 13,
                        EntityType = "NationalTreasure",
                        Id = 0
                    }
                },
                result);
        }

        [TestMethod]
        public void TwoEntityDefinition_SingleDocument()
        {
            // Setup the repository
            var definitions = new List<UserDefinedEntityDefinition>
            {
                new UserDefinedEntityDefinition()
                {
                    Regex = "parks?",
                    EntityType = "NationalTreasure",
                    EntityValue = "PaRk"
                },
                new UserDefinedEntityDefinition()
                {
                    Regex = "National",
                    EntityType = "SomeType",
                    EntityValue = "NATIONAL"
                }
            };
            this.kernel.GetMock<IRepository<UserDefinedEntityDefinition>>().Setup(x => x.GetAll()).Returns(definitions);

            // Load the finder
            var entityFinder = kernel.Get<IUserDefinedEntityFinder>();

            var result = entityFinder.FindAllUserDefinedEntities(new IDocument[]
                {new Document() {Id = "DOC1", Content = "The national parks are great"}}).ToList();

            Assert.AreEqual(2, result.Count());
            CollectionAssert.AreEquivalent(
                new List<UserDefinedEntity>()
                {
                    new UserDefinedEntity()
                    {
                        DocumentId = "DOC1",
                        Entity = "PaRk",
                        EntityLength = 5,
                        EntityOffset = 13,
                        EntityType = "NationalTreasure",
                        Id = 0
                    },
                    new UserDefinedEntity()
                    {
                        DocumentId = "DOC1",
                        Entity = "NATIONAL",
                        EntityLength = 8,
                        EntityOffset = 4,
                        EntityType = "SomeType",
                        Id = 0
                    }
                },
                result);
        }

        [TestMethod]
        public void CharacterGroupTest()
        {
            // Setup the repository
            var definitions = new List<UserDefinedEntityDefinition>
            {
                new UserDefinedEntityDefinition()
                {
                    Regex = "(national )?parks?",
                    EntityType = "NationalTreasure",
                    EntityValue = "NationalPaRk"
                }
            };
            this.kernel.GetMock<IRepository<UserDefinedEntityDefinition>>().Setup(x => x.GetAll()).Returns(definitions);

            // Load the finder
            var entityFinder = kernel.Get<IUserDefinedEntityFinder>();

            var result = entityFinder.FindAllUserDefinedEntities(new IDocument[]
                {new Document() {Id = "DOC1", Content = "The national parks are great.  Our local park is cool."}}).ToList();

            Assert.AreEqual(2, result.Count());
            CollectionAssert.AreEquivalent(
                new List<UserDefinedEntity>()
                {
                    new UserDefinedEntity()
                    {
                        DocumentId = "DOC1",
                        Entity = "NationalPaRk",
                        EntityLength = 14,
                        EntityOffset = 4,
                        EntityType = "NationalTreasure",
                        Id = 0
                    },
                    new UserDefinedEntity()
                    {
                        DocumentId = "DOC1",
                        Entity = "NationalPaRk",
                        EntityLength = 4,
                        EntityOffset = 41,
                        EntityType = "NationalTreasure",
                        Id = 0
                    }
                },
                result);
        }

        [TestMethod]
        public void SingleEntityDefinition_TwoDocuments()
        {
            // Setup the repository
            var definitions = new List<UserDefinedEntityDefinition>
            {
                new UserDefinedEntityDefinition()
                {
                    Regex = "parks?",
                    EntityType = "NationalTreasure",
                    EntityValue = "PaRk"
                }
            };
            this.kernel.GetMock<IRepository<UserDefinedEntityDefinition>>().Setup(x => x.GetAll()).Returns(definitions);

            // Load the finder
            var entityFinder = kernel.Get<IUserDefinedEntityFinder>();

            var result = entityFinder.FindAllUserDefinedEntities(new IDocument[]
            {
                new Document() {Id = "DOC1", Content = "The national parks are great"},
                new Document() {Id = "DOC2", Content = "park is great"}
                
            }).ToList();

            Assert.AreEqual(2, result.Count());
            CollectionAssert.AreEquivalent(
                new List<UserDefinedEntity>()
                {
                    new UserDefinedEntity()
                    {
                        DocumentId = "DOC1",
                        Entity = "PaRk",
                        EntityLength = 5,
                        EntityOffset = 13,
                        EntityType = "NationalTreasure",
                        Id = 0
                    },
                    new UserDefinedEntity()
                    {
                        DocumentId = "DOC2",
                        Entity = "PaRk",
                        EntityLength = 4,
                        EntityOffset = 0,
                        EntityType = "NationalTreasure",
                        Id = 0
                    }
                },
                result);
        }

        [TestMethod]
        public void SingleEntityDefinition_OneDocAndOneNullDoc()
        {
            // Setup the repository
            var definitions = new List<UserDefinedEntityDefinition>
            {
                new UserDefinedEntityDefinition()
                {
                    Regex = "parks?",
                    EntityType = "NationalTreasure",
                    EntityValue = "PaRk"
                }
            };
            this.kernel.GetMock<IRepository<UserDefinedEntityDefinition>>().Setup(x => x.GetAll()).Returns(definitions);

            // Load the finder
            var entityFinder = kernel.Get<IUserDefinedEntityFinder>();

            var result = entityFinder.FindAllUserDefinedEntities(new IDocument[]
            {
                new Document() {Id = "DOC1", Content = "The national parks are great"},
                new Document() {Id = "DOC2", Content = null}
                
            }).ToList();

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("DOC1", result[0].DocumentId);
            Assert.AreEqual("PaRk", result[0].Entity);
            Assert.AreEqual("NationalTreasure", result[0].EntityType);
        }

        [TestMethod]
        public void SingleEntityDefinition_OneDocAndOneEmptyDoc()
        {
            // Setup the repository
            var definitions = new List<UserDefinedEntityDefinition>
            {
                new UserDefinedEntityDefinition()
                {
                    Regex = "parks?",
                    EntityType = "NationalTreasure",
                    EntityValue = "PaRk"
                }
            };
            this.kernel.GetMock<IRepository<UserDefinedEntityDefinition>>().Setup(x => x.GetAll()).Returns(definitions);

            // Load the finder
            var entityFinder = kernel.Get<IUserDefinedEntityFinder>();

            var result = entityFinder.FindAllUserDefinedEntities(new IDocument[]
            {
                new Document() {Id = "DOC1", Content = "The national parks are great"},
                new Document() {Id = "DOC2", Content = String.Empty}

            }).ToList();

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("DOC1", result[0].DocumentId);
            Assert.AreEqual("PaRk", result[0].Entity);
            Assert.AreEqual("NationalTreasure", result[0].EntityType);
        }

        [TestMethod]
        public void SingleEntityDefinition_NoDocuments()
        {
            // Setup the repository
            var definitions = new List<UserDefinedEntityDefinition>
            {
                new UserDefinedEntityDefinition()
                {
                    Regex = "parks?",
                    EntityType = "NationalTreasure",
                    EntityValue = "PaRk"
                }
            };
            this.kernel.GetMock<IRepository<UserDefinedEntityDefinition>>().Setup(x => x.GetAll()).Returns(definitions);

            // Load the finder
            var entityFinder = kernel.Get<IUserDefinedEntityFinder>();

            var result = entityFinder.FindAllUserDefinedEntities(new IDocument[] { });

            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void NoEntityDefinitions_TwoDocuments()
        {
            // Setup the repository
            this.kernel.GetMock<IRepository<UserDefinedEntityDefinition>>().Setup(x => x.GetAll())
                .Returns(new List<UserDefinedEntityDefinition>());

            // Load the finder
            var entityFinder = kernel.Get<IUserDefinedEntityFinder>();

            var result = entityFinder.FindAllUserDefinedEntities(new IDocument[]
            {
                new Document() {Id = "DOC1", Content = "The national parks are great"},
                new Document() {Id = "DOC2", Content = "park is great"}

            }).ToList();

            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void SingleEntityDefinition_NullDocumentList()
        {
            // Setup the repository
            var definitions = new List<UserDefinedEntityDefinition>
            {
                new UserDefinedEntityDefinition()
                {
                    Regex = "parks?",
                    EntityType = "NationalTreasure",
                    EntityValue = "PaRk"
                }
            };
            this.kernel.GetMock<IRepository<UserDefinedEntityDefinition>>().Setup(x => x.GetAll()).Returns(definitions);

            // Load the finder
            var entityFinder = kernel.Get<IUserDefinedEntityFinder>();

            var result = entityFinder.FindAllUserDefinedEntities(null);

            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void NullEntityDefinitions_TwoDocuments()
        {
            // Setup the repository
            this.kernel.GetMock<IRepository<UserDefinedEntityDefinition>>().Setup(x => x.GetAll())
                .Returns<IList<UserDefinedEntityDefinition>>(null);

            // Load the finder
            var entityFinder = kernel.Get<IUserDefinedEntityFinder>();

            var result = entityFinder.FindAllUserDefinedEntities(new IDocument[]
            {
                new Document() {Id = "DOC1", Content = "The national parks are great"},
                new Document() {Id = "DOC2", Content = "park is great"}

            }).ToList();

            Assert.AreEqual(0, result.Count());
        }
    }
}
