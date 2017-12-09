using Microsoft.VisualStudio.TestTools.UnitTesting;
using RedditCore.Http;
using RedditCore.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RedditCoreTest.Http
{
    [TestClass]
    public class UrlFinderTests
    {
        private IUrlFinder finder = new UrlFinder(new ConsoleLog());

        [TestMethod]
        public void EmptyUrl()
        {
            var results = finder.FindUrls("");

            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void NullUrl()
        {
            var results = finder.FindUrls(null);

            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void UrlIsEntireBody()
        {
            var results = finder.FindUrls("http://www.example.com/stuff.aspx");

            AssertResults(results, "http://www.example.com/stuff.aspx");
        }

        [TestMethod]
        public void UrlIsEntireBody_https()
        {
            var results = finder.FindUrls("https://www.example.com/stuff.aspx");

            AssertResults(results, "https://www.example.com/stuff.aspx");
        }

        [TestMethod]
        public void MultipleUrls()
        {
            var results = finder.FindUrls("https://www.example.com/stuff.aspx AND http://test.sample.net");

            AssertResults(results, "https://www.example.com/stuff.aspx", "http://test.sample.net");
        }

        [TestMethod]
        public void UrlEndsInParenthesis()
        {
            // Found this inside a Reddit post
            var results = finder.FindUrls("Trades Only [Latrialum White + Violet Crystal Mine](http://imgur.com/DEZskc4) | MX Stem | USPS");

            AssertResults(results, "http://imgur.com/DEZskc4");
        }

        [TestMethod]
        public void UrlEndsInMultipleBadChars()
        {
            var results = finder.FindUrls("This sentence has a URL (http://www.empowdermd.com), and other stuff as well");

            AssertResults(results, "http://www.empowdermd.com");
        }

        [TestMethod]
        public void UrlEndsInPeriod()
        {
            var results = finder.FindUrls("This sentence has a URL https://www.example.com.");

            AssertResults(results, "https://www.example.com");
        }

        [TestMethod]
        public void UrlEndsInComma()
        {
            var results = finder.FindUrls("This sentence has a URL https://www.example.com, and a few other things as well.");

            AssertResults(results, "https://www.example.com");
        }

        [TestMethod]
        public void HasARealDocument()
        {
            var results = finder.FindUrls("This is the first url http://test.example.com/stuff AND a second one http://stuff.sample.com.  This is the rest of the document.");

            AssertResults(results, "http://test.example.com/stuff", "http://stuff.sample.com");
        }

        [TestMethod]
        public void ThreeUrls()
        {
            var results = finder.FindUrls("https://www.example.com/stuff.aspx AND http://test.sample.net OR http://junk.here.com/none/second/stuff.php");

            AssertResults(results, "https://www.example.com/stuff.aspx", "http://test.sample.net", "http://junk.here.com/none/second/stuff.php");
        }

        private void AssertResults(IList<Uri> results, params string[] expectedUris)
        {
            List<Uri> expected = new List<Uri>();

            foreach(var uri in expectedUris)
            {
                expected.Add(new Uri(uri));
            }

            CollectionAssert.AreEqual(expected, results.ToList<Uri>());
        }
    }
}
