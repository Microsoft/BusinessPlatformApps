using System;

namespace RedditCore.SocialGist
{
    public static class TimeExtensions
    {
        public static DateTime MinutePrecision(this DateTime input)
        {
            return new DateTime(input.Year, input.Month, input.Day, input.Hour, input.Minute, 0);
        }

        public static DateTime HourPrecision(this DateTime input)
        {
            return new DateTime(input.Year, input.Month, input.Day, input.Hour, 0, 0);
        }

        public static DateTime DayPrecision(this DateTime input)
        {
            return new DateTime(input.Year, input.Month, input.Day, 0, 0, 0);
        }

        public static DateTime MonthPrecision(this DateTime input)
        {
            return new DateTime(input.Year, input.Month, 1, 0, 0, 0);
        }

        public static DateTime DayOfWeekPrecision(this DateTime input)
        {
            var day = input.DayPrecision();

            return day.Subtract(new TimeSpan((int) day.DayOfWeek, 0, 0, 0));
        }
    }
}
