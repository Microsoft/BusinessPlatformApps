using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RedditCore.SocialGist;

namespace RedditCoreTest.SocialGist
{
    [TestClass]
    public class TimeExtensionsTest
    {
        [TestMethod]
        public void BasicTest()
        {
            var initialTimestamp = DateTime.Parse("2016-03-30T03:41:23");

            Assert.AreEqual(DateTime.Parse("2016-03-30T03:41:00"), initialTimestamp.MinutePrecision());
            Assert.AreEqual(DateTime.Parse("2016-03-30T03:00:00"), initialTimestamp.HourPrecision());
            Assert.AreEqual(DateTime.Parse("2016-03-30T00:00:00"), initialTimestamp.DayPrecision());
            Assert.AreEqual(DateTime.Parse("2016-03-01T00:00:00"), initialTimestamp.MonthPrecision());

            Assert.AreEqual(DateTime.Parse("2016-03-27T00:00:00"), initialTimestamp.DayOfWeekPrecision());
        }

        [TestMethod]
        public void WeekSpansMonth()
        {
            var initialTimestamp = DateTime.Parse("2016-03-02T23:41:23");

            Assert.AreEqual(DateTime.Parse("2016-03-02T23:41:00"), initialTimestamp.MinutePrecision());
            Assert.AreEqual(DateTime.Parse("2016-03-02T23:00:00"), initialTimestamp.HourPrecision());
            Assert.AreEqual(DateTime.Parse("2016-03-02T00:00:00"), initialTimestamp.DayPrecision());
            Assert.AreEqual(DateTime.Parse("2016-03-01T00:00:00"), initialTimestamp.MonthPrecision());

            Assert.AreEqual(DateTime.Parse("2016-02-28T00:00:00"), initialTimestamp.DayOfWeekPrecision());
        }
    }
}
