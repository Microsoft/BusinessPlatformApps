using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RedditCore.Http;
using System.Collections.Generic;

namespace RedditCoreTest.Http
{
    [TestClass]
    public class UriBuilderExtensionsTest
    {
        [TestMethod]
        public void SingleQueryParam()
        {
            var builder = new UriBuilder("https://www.example.com");
            builder.AddQueryParam("myName", "myValue");

            Assert.AreEqual("?myName=myValue", builder.Query);
        }

        [TestMethod]
        public void SingleIntegerQueryParam()
        {
            var builder = new UriBuilder("https://www.example.com");
            builder.AddQueryParam("myName", 99);

            Assert.AreEqual("?myName=99", builder.Query);
        }

        [TestMethod]
        public void SingleNegativeIntegerQueryParam()
        {
            var builder = new UriBuilder("https://www.example.com");
            builder.AddQueryParam("myName", -129);

            Assert.AreEqual("?myName=-129", builder.Query);
        }

        [TestMethod]
        public void SingleQueryParamWithInvalidChars()
        {
            var builder = new UriBuilder("https://www.example.com");
            builder.AddQueryParam("my=Name", "myV&alue");

            Assert.AreEqual("?my%3dName=myV%26alue", builder.Query);
        }

        [TestMethod]
        public void MultipleQueryParams()
        {
            var builder = new UriBuilder("https://www.example.com");
            builder.AddQueryParam("myName", "myValue").AddQueryParam("SecondName", "SecondValue");

            Assert.AreEqual("?myName=myValue&SecondName=SecondValue", builder.Query);
        }

        [TestMethod]
        public void EmptyDictionary()
        {
            var builder = new UriBuilder("https://www.example.com");
            var dict = new Dictionary<String, Object>();
            builder.QueryParamsFromDictionary(dict);
            Assert.AreEqual("https://www.example.com:443/", builder.ToString());
        }

        [TestMethod]
        public void SingleElementDictionary()
        {
            var builder = new UriBuilder("https://www.example.com");
            var dict = new Dictionary<String, Object>();
            dict.Add("myName", "myValue");
            builder.QueryParamsFromDictionary(dict);
            Assert.AreEqual("https://www.example.com:443/?myName=myValue", builder.ToString());
        }

        [TestMethod]
        public void MultipleStringElementsDictionary()
        {
            var builder = new UriBuilder("https://www.example.com");
            var dict = new Dictionary<String, Object>();
            dict.Add("myName", "myValue");
            dict.Add("myOtherName", "myOtherValue");
            builder.QueryParamsFromDictionary(dict);
            var queryString = builder.Query;
            AssertParameterExistsInQueryString(queryString, "myName=myValue");
            AssertParameterExistsInQueryString(queryString, "myOtherName=myOtherValue");
        }

        [TestMethod]
        public void MultipleIntElementsDictionary()
        {
            var builder = new UriBuilder("https://www.example.com");
            var dict = new Dictionary<String, Object>();
            dict.Add("myName", "5");
            dict.Add("myOtherName", "10");
            builder.QueryParamsFromDictionary(dict);
            var queryString = builder.Query;
            AssertParameterExistsInQueryString(queryString, "myName=5");
            AssertParameterExistsInQueryString(queryString, "myOtherName=10");
        }

        [TestMethod]
        public void MultipleVariableTypesDictionary()
        {
            var builder = new UriBuilder("https://www.example.com");
            var dict = new Dictionary<String, Object>();
            dict.Add("myName", "5");
            dict.Add("myOtherName", "myOtherValue");
            builder.QueryParamsFromDictionary(dict);
            var queryString = builder.Query;
            AssertParameterExistsInQueryString(queryString, "myName=5");
            AssertParameterExistsInQueryString(queryString, "myOtherName=myOtherValue");
        }

        private void AssertParameterExistsInQueryString(
            string queryString, 
            string parameterAndValue
        )
        {
            Assert.IsTrue(queryString.Contains(parameterAndValue), "[{0}] was not found in [{1}]", new object[] { parameterAndValue, queryString });
        }
    }
}
