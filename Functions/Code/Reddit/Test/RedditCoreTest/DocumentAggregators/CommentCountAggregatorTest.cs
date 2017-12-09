using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RedditCore.DataModel;
using RedditCore.DataModel.Repositories;
using RedditCore.DocumentAggregators;
using System.Collections.Generic;
using System.Linq;

namespace RedditCoreTest.DocumentAggregators
{
    [TestClass]
    public class CommentCountAggregatorTest
    {
        [TestMethod]
        public void BasicTest()
        {
            var repositoryMock = new Mock<IRepository<PostCommentCount>>();
            IEnumerable<PostCommentCount> result = null;
            repositoryMock.Setup(x => x.Save(It.IsAny<IEnumerable<PostCommentCount>>())).Callback<IEnumerable<PostCommentCount>>(items => { result = items; });

            var aggregator = new CommentCountAggregator(repositoryMock.Object);

            var posts = new List<Post>() { CreatePost("postOne"), CreatePost("postTwo") };
            var comments = new List<Comment>() { CreateComment("postOne"), CreateComment("postTwo"), CreateComment("postTwo") };

            aggregator.Aggregate(posts, comments);

            Assert.AreEqual(2, result.Count());
            Assert.IsNotNull(result.FirstOrDefault(x => x.PostId == "postOne" && x.CommentCount == 1));
            Assert.IsNotNull(result.FirstOrDefault(x => x.PostId == "postTwo" && x.CommentCount == 2));
        }

        [TestMethod]
        public void NullData()
        {
            var repositoryMock = new Mock<IRepository<PostCommentCount>>();
            IEnumerable<PostCommentCount> result = null;
            repositoryMock.Setup(x => x.Save(It.IsAny<IEnumerable<PostCommentCount>>())).Callback<IEnumerable<PostCommentCount>>(items => { result = items; });

            var aggregator = new CommentCountAggregator(repositoryMock.Object);

            aggregator.Aggregate(null, null);

            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void NoData()
        {
            var repositoryMock = new Mock<IRepository<PostCommentCount>>();
            IEnumerable<PostCommentCount> result = null;
            repositoryMock.Setup(x => x.Save(It.IsAny<IEnumerable<PostCommentCount>>())).Callback<IEnumerable<PostCommentCount>>(items => { result = items; });

            var aggregator = new CommentCountAggregator(repositoryMock.Object);

            var posts = new List<Post>();
            var comments = new List<Comment>();

            aggregator.Aggregate(posts, comments);

            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void CommentWithoutPost()
        {
            var repositoryMock = new Mock<IRepository<PostCommentCount>>();
            IEnumerable<PostCommentCount> result = null;
            repositoryMock.Setup(x => x.Save(It.IsAny<IEnumerable<PostCommentCount>>())).Callback<IEnumerable<PostCommentCount>>(items => { result = items; });

            var aggregator = new CommentCountAggregator(repositoryMock.Object);

            var posts = new List<Post>() { CreatePost("postOne") };
            var comments = new List<Comment>() { CreateComment("postOne"), CreateComment("postTwo"), CreateComment("postTwo") };

            aggregator.Aggregate(posts, comments);

            Assert.AreEqual(2, result.Count());
            Assert.IsNotNull(result.FirstOrDefault(x => x.PostId == "postOne" && x.CommentCount == 1));
            Assert.IsNotNull(result.FirstOrDefault(x => x.PostId == "postTwo" && x.CommentCount == 2));
        }

        [TestMethod]
        public void PostWithoutComment()
        {
            var repositoryMock = new Mock<IRepository<PostCommentCount>>();
            IEnumerable<PostCommentCount> result = null;
            repositoryMock.Setup(x => x.Save(It.IsAny<IEnumerable<PostCommentCount>>())).Callback<IEnumerable<PostCommentCount>>(items => { result = items; });

            var aggregator = new CommentCountAggregator(repositoryMock.Object);

            var posts = new List<Post>() { CreatePost("postOne"), CreatePost("postTwo") };
            var comments = new List<Comment>() { CreateComment("postTwo"), CreateComment("postTwo") };

            aggregator.Aggregate(posts, comments);

            Assert.AreEqual(2, result.Count());
            Assert.IsNotNull(result.FirstOrDefault(x => x.PostId == "postOne" && x.CommentCount == 0));
            Assert.IsNotNull(result.FirstOrDefault(x => x.PostId == "postTwo" && x.CommentCount == 2));
        }

        [TestMethod]
        public void TenComments()
        {
            var repositoryMock = new Mock<IRepository<PostCommentCount>>();
            IEnumerable<PostCommentCount> result = null;
            repositoryMock.Setup(x => x.Save(It.IsAny<IEnumerable<PostCommentCount>>())).Callback<IEnumerable<PostCommentCount>>(items => { result = items; });

            var aggregator = new CommentCountAggregator(repositoryMock.Object);

            var posts = new List<Post>() { CreatePost("postOne"), CreatePost("postTwo") };
            var comments = new List<Comment>() {
                CreateComment("postTwo"),
                CreateComment("postTwo"),
                CreateComment("postTwo"),
                CreateComment("postTwo"),
                CreateComment("postTwo"),

                CreateComment("postOne"),

                CreateComment("postTwo"),
                CreateComment("postTwo"),
                CreateComment("postTwo"),
                CreateComment("postTwo"),
                CreateComment("postTwo"),
            };

            aggregator.Aggregate(posts, comments);

            Assert.AreEqual(2, result.Count());
            Assert.IsNotNull(result.FirstOrDefault(x => x.PostId == "postOne" && x.CommentCount == 1));
            Assert.IsNotNull(result.FirstOrDefault(x => x.PostId == "postTwo" && x.CommentCount == 10));
        }

        private Post CreatePost(string postId)
        {
            return new Post()
            {
                Id = postId
            };
        }

        private Comment CreateComment(string postId)
        {
            return new Comment()
            {
                PostId = postId
            };
        }
    }
}
