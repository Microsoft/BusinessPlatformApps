using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DocumentTime
{
	public DateTime Timestamp { get; internal set; }

	public DateTime MonthPrecision { get; internal set; }

	public DateTime WeekPrecision { get; internal set; }

	public DateTime DayPrecision { get; internal set; }

	public DateTime HourPrecision { get; internal set; }

	public DateTime MinutePrecision { get; internal set; }
}
