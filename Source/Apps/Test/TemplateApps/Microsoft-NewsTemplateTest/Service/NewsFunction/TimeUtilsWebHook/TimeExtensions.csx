using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class TimeExtensions
{
	public static DateTime MinutePrecision(DateTime input)
	{
		return new DateTime(input.Year, input.Month, input.Day, input.Hour, input.Minute, 0);
	}

	public static DateTime HourPrecision(DateTime input)
	{
		return new DateTime(input.Year, input.Month, input.Day, input.Hour, 0, 0);
	}

	public static DateTime DayPrecision(DateTime input)
	{
		return new DateTime(input.Year, input.Month, input.Day, 0, 0, 0);
	}

	public static DateTime MonthPrecision(DateTime input)
	{
		return new DateTime(input.Year, input.Month, 1, 0, 0, 0);
	}

	public static DateTime DayOfWeekPrecision(DateTime input)
	{
		var day = DayPrecision(input);

		return day.Subtract(new TimeSpan((int)day.DayOfWeek, 0, 0, 0));
	}
}
