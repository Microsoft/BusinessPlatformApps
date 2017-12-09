using System.IO;
using Microsoft.Deployment.Common.AppLoad;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Deployment.Actions.AzureCustom.Reddit;

namespace Microsoft.Deployment.Tests.Actions.Reddit
{
    [TestClass]
    public class DeployRedditAzureMLServiceFromFileTests
    {

        [TestMethod]
        public void TestSentimentReplacement()
        {
            var appPath = new AppFactory(true).AppPath;
            var jsonDefinition = File.ReadAllText(appPath + "\\Microsoft\\Released\\Microsoft-RedditTemplate\\Service\\AzureML\\RedditML.json");

            // if the model is no longer used, you should stop using DeployRedditAzureMLServiceFromFile and use DeployAzureMLServiceFromFile.
            // if the model is used, but the title has changed, you need to update this test so that the name is no longer expected to be "Mini Twitter..."
            Assert.IsTrue(jsonDefinition.Contains("Mini Twitter sentiment analysis [trained model] - Copy"), "The AzureML webservice file has changed and no longer contains the trained model asset.  Is the AzureML webservice file correct?  Is the model no longer in use?  Did it's name change?");
            Assert.IsTrue(!jsonDefinition.Contains("https://foo.com/bar.txt"));
            var newJsonDefinition = DeployRedditAzureMLWebServiceFromFile.ReplaceSentimentModel(
                "https://foo.com/bar.txt",
                "Mini Twitter sentiment analysis [trained model] - Copy", 
                jsonDefinition
            );

            Assert.IsTrue(newJsonDefinition.Contains("https://foo.com/bar.txt"), "Sentiment model URI was not replaced!");

        }

    }
}