#load "TimeExtensions.csx"
#load "DocumentTime.csx"

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DocumentTimeFactory
{
	public DocumentTime CreateDocumentTime(DateTime input)
	{
		return new DocumentTime()
		{
			Timestamp = input,
			MonthPrecision = TimeExtensions.MonthPrecision(input),
			WeekPrecision = TimeExtensions.DayOfWeekPrecision(input),
			DayPrecision = TimeExtensions.DayPrecision(input),
			HourPrecision = TimeExtensions.HourPrecision(input),
			MinutePrecision = TimeExtensions.MinutePrecision(input)
		};
	}
}
