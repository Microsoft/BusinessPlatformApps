using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RedditCore;

namespace RedditCoreTest
{
    [TestClass]
    public class DateTimesTests
    {
        [TestMethod]
        public void TimestampIsCorrect()
        {
            var time = DateTimes.UnixTimeStampToDateTime(1510350355L);

            Assert.AreEqual(DateTime.Parse("11/10/2017 1:45:55 PM"), time);
        }
    }
}
