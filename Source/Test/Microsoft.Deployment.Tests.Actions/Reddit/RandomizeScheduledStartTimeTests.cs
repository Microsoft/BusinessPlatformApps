using System.Threading.Tasks;
using Microsoft.Deployment.Actions.AzureCustom.Reddit;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Tests.Actions.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Deployment.Tests.Actions.Reddit
{
    [TestClass]
    public class RandomizeScheduledStartTimeTests
    {
        // test no starttime end time (verify between default range)
        [TestMethod]
        public async Task TestNoStartTimeEndTime()
        {
            // test is not exhaustive, if it ever fails, Random() is not behaving the way I expected
            var dataStore = await TestManager.GetDataStore();
            var response = await TestManager.ExecuteActionAsync($"Microsoft-{nameof(RandomizeScheduledStartTime)}", dataStore, "Microsoft-RedditTemplate");
            Assert.IsTrue(response.Status == ActionStatus.Success, "Action should have been an overall success");
            var cronTime = response.DataStore.GetValue(RandomizeScheduledStartTime.RandomScheduleCronTime);
            Assert.IsNotNull(cronTime, $"The cron time was not placed in the DataStore where we expected it {RandomizeScheduledStartTime.RandomScheduleCronTime}");
            var splitCronTime = cronTime.Split(' ');
            Assert.AreEqual(6, splitCronTime.Length, $"The cron time seems to be in the wrong format, got {cronTime} and expected something like 0 0 5 * * *");
            int hour = int.Parse(splitCronTime[2]); // seconds, minutes, hours, daily, monthly, day-of-weekly 
            int minute = int.Parse(splitCronTime[1]);

            Assert.IsTrue(hour < RandomizeScheduledStartTime.DefaultEndTimeUtc, $"Hour generated {hour} in cron spec {cronTime} is greater than or equal to our default end time");
            Assert.IsTrue(hour >= RandomizeScheduledStartTime.DefaultStartTimeUtc, $"Hour generated {hour} in cron spec {cronTime} is less than our default start time");

            Assert.IsTrue(minute >= 0, $"Minute generated {minute} in cron spec {cronTime} is less than 0");
            Assert.IsTrue(minute < 60, "Minute generated {minute} in cron spec {cronTime} is greater than or equal to than 60");
        }

        // test valid start range / end range
        [TestMethod]
        public async Task TestValidStartAndEndTime()
        {
            // test is not exhaustive, if it ever fails, Random() is not behaving the way I expected
            var dataStore = await TestManager.GetDataStore();
            dataStore.AddToDataStore(RandomizeScheduledStartTime.StartHourParameter, "3");
            dataStore.AddToDataStore(RandomizeScheduledStartTime.EndHourParameter, "5"); // value can now be either 3 or 4
            var response = await TestManager.ExecuteActionAsync($"Microsoft-{nameof(RandomizeScheduledStartTime)}", dataStore, "Microsoft-RedditTemplate");
            Assert.IsTrue(response.Status == ActionStatus.Success, "Action should have been an overall success");
            var cronTime = response.DataStore.GetValue(RandomizeScheduledStartTime.RandomScheduleCronTime);
            Assert.IsNotNull(cronTime, $"The cron time was not placed in the DataStore where we expected it {RandomizeScheduledStartTime.RandomScheduleCronTime}");
            var splitCronTime = cronTime.Split(' ');
            Assert.AreEqual(6, splitCronTime.Length, $"The cron time seems to be in the wrong format, got {cronTime} and expected something like 0 0 5 * * *");
            int hour = int.Parse(splitCronTime[2]); // seconds, minutes, hours, daily, monthly, day-of-weekly 
            int minute = int.Parse(splitCronTime[1]);

            Assert.IsTrue(hour < 5, $"Hour generated {hour} in cron spec {cronTime} is greater than or equal to our default end time");
            Assert.IsTrue(hour >= 3, $"Hour generated {hour} in cron spec {cronTime} is less than our default start time");

            Assert.IsTrue(minute >= 0, $"Minute generated {minute} in cron spec {cronTime} is less than 0");
            Assert.IsTrue(minute < 60, "Minute generated {minute} in cron spec {cronTime} is greater than or equal to than 60");
        }

        // test start time only
        [TestMethod]
        public async Task TestStartTimeOnly()
        {
            var dataStore = await TestManager.GetDataStore();
            dataStore.AddToDataStore(RandomizeScheduledStartTime.StartHourParameter, "3");
            var response = await TestManager.ExecuteActionAsync($"Microsoft-{nameof(RandomizeScheduledStartTime)}", dataStore, "Microsoft-RedditTemplate");
            Assert.IsTrue(response.Status == ActionStatus.Failure, "Action should have been an abject failure");
            Assert.AreEqual($"{RandomizeScheduledStartTime.EndHourParameter} was not provided yet {RandomizeScheduledStartTime.StartHourParameter} was.  Both are required.", response.ExceptionDetail.AdditionalDetailsErrorMessage);
        }

        // test end time only
        [TestMethod]
        public async Task TestEndTimeOnly()
        {
            var dataStore = await TestManager.GetDataStore();
            dataStore.AddToDataStore(RandomizeScheduledStartTime.EndHourParameter, "3");
            var response = await TestManager.ExecuteActionAsync($"Microsoft-{nameof(RandomizeScheduledStartTime)}", dataStore, "Microsoft-RedditTemplate");
            Assert.IsTrue(response.Status == ActionStatus.Failure, "Action should have been an abject failure");
            Assert.AreEqual($"{RandomizeScheduledStartTime.StartHourParameter} was not provided yet {RandomizeScheduledStartTime.EndHourParameter} was.  Both are required.", response.ExceptionDetail.AdditionalDetailsErrorMessage);
        }

        // test start time NAN

        // test start time only
        [TestMethod]
        public async Task TestStartTimeNaN()
        {
            var dataStore = await TestManager.GetDataStore();
            dataStore.AddToDataStore(RandomizeScheduledStartTime.StartHourParameter, "sandwich");
            dataStore.AddToDataStore(RandomizeScheduledStartTime.EndHourParameter, RandomizeScheduledStartTime.DefaultEndTimeUtc); 
            var response = await TestManager.ExecuteActionAsync($"Microsoft-{nameof(RandomizeScheduledStartTime)}", dataStore, "Microsoft-RedditTemplate");
            Assert.IsTrue(response.Status == ActionStatus.Failure, "Action should have been an abject failure");
        }

        // test end time NAN
        [TestMethod]
        public async Task TestEndTimeNaN()
        {
            var dataStore = await TestManager.GetDataStore();
            dataStore.AddToDataStore(RandomizeScheduledStartTime.EndHourParameter, "sandwich");
            dataStore.AddToDataStore(RandomizeScheduledStartTime.StartHourParameter, RandomizeScheduledStartTime.DefaultStartTimeUtc);
            var response = await TestManager.ExecuteActionAsync($"Microsoft-{nameof(RandomizeScheduledStartTime)}", dataStore, "Microsoft-RedditTemplate");
            Assert.IsTrue(response.Status == ActionStatus.Failure, "Action should have been an abject failure");
        }

        // test end time before start time
        [TestMethod]
        public async Task TestEndTimeBeforeStartTime()
        {
            var dataStore = await TestManager.GetDataStore();
            dataStore.AddToDataStore(RandomizeScheduledStartTime.StartHourParameter, "4");
            dataStore.AddToDataStore(RandomizeScheduledStartTime.EndHourParameter, "3");
            var response = await TestManager.ExecuteActionAsync($"Microsoft-{nameof(RandomizeScheduledStartTime)}", dataStore, "Microsoft-RedditTemplate");
            Assert.IsTrue(response.Status == ActionStatus.Failure, "Action should have been an abject failure");
            Assert.AreEqual($"{RandomizeScheduledStartTime.StartHourParameter}(4) is larger than {RandomizeScheduledStartTime.EndHourParameter}(3).  Please provide a range between 0 and 23.", response.ExceptionDetail.AdditionalDetailsErrorMessage);
        }
    }
}
