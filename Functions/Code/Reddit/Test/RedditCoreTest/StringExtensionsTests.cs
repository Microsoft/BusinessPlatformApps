using Microsoft.VisualStudio.TestTools.UnitTesting;
using RedditCore;

namespace RedditCoreTest
{
    [TestClass]
    public class StringExtensionsTests
    {
        [TestMethod]
        public void DoNotTruncateString_null()
        {
            string input = null;
            var result = input.Truncate(100);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void DoNotTruncateString_emptyString()
        {
            var result = string.Empty.Truncate(100);
            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void DoNotTruncateString()
        {
            var result = "This is the string".Truncate(100);
            Assert.AreEqual("This is the string", result);
        }

        [TestMethod]
        public void TruncateString_Basic()
        {
            var result = "This is the string".Truncate(10);
            Assert.AreEqual(10, result.Length);
            Assert.AreEqual("This is...", result);
        }

        [TestMethod]
        public void TruncateString_ExactlyRightLength()
        {
            var result = "This is the string".Truncate(18);
            Assert.AreEqual(18, result.Length);
            Assert.AreEqual("This is the string", result);
        }

        [TestMethod]
        public void TruncateString_AlmostExactLength()
        {
            // Input is 18 chars.  What happens when we want 17 characters?
            var result = "This is the string".Truncate(17);
            Assert.AreEqual(17, result.Length);
            Assert.AreEqual("This is the st...", result);
        }

        [TestMethod]
        public void DoNotTrimEndString_null()
        {
            string input = null;
            var result = input.TrimEndString("none");
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void DoNotTrimEndString_emptyString()
        {
            var result = string.Empty.TrimEndString("none");
            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void DoNotTrimEndString()
        {
            var result = "This is the string".TrimEndString("This");
            Assert.AreEqual("This is the string", result);
        }

        [TestMethod]
        public void TrimEndString_Basic()
        {
            var result = "This is the string".TrimEndString(" string");
            Assert.AreEqual("This is the", result);
        }

        [TestMethod]
        public void TrimEndString_ExactlySameLength()
        {
            var result = "This is the string".TrimEndString("This is the string");
            Assert.AreEqual(0, result.Length);
        }

        [TestMethod]
        public void TrimEnd_AlmostExactLength()
        {
            var result = "This is the string".TrimEndString("his is the string");
            Assert.AreEqual("T", result);
        }
    }
}
