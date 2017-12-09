using System;

namespace FacebookETL
{
    public class DateUtility
    {
        public static string GetDate(string daysAsString)
        {
            // Set name to query string or body data
            int days = int.Parse(daysAsString);

            if (days > 0)
            {
                days = days * -1;
            }

            string dateToReturn = DateTime.Now.AddDays(days).ToString();
            return dateToReturn;
        }

        public static string GetUnixFromDate(string date)
        {
            // Get request body
            DateTime dateTime = DateTime.Parse(date);
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan span = (dateTime.ToUniversalTime() - epoch);
            string timeStamp = Math.Floor(span.TotalSeconds).ToString();
            return timeStamp;
        }

        public static string GetDateTimeRelativeFromNow(int days)
        {
            if (days > 0)
            {
                days = days * -1;
            }

            return DateTime.Now.AddDays(days).ToString();
        }

        public static string GetDateTimeRelativeFromNow(string date, int days)
        {
            if (days > 0)
            {
                days = days * -1;
            }

            return DateTime.Parse(date).AddDays(days).ToString();
        }
    }
}
