using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class StringExtensions
{
	public static String Left(String input, int size)
	{
		if (input == null || input.Length <= size)
		{
			return input;
		}
		else
		{
			return input.Substring(0, size);
		}
	}

	public static int GetByteCount(String input)
	{
		return Encoding.ASCII.GetByteCount(input);
	}

	public static String LimitByBytes(String input, int sizeLimit)
	{
		if (GetByteCount(input) < sizeLimit)
		{
			return input;
		}
		else
		{
			var maxLen = sizeLimit - 3;
			return Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(input), 0, maxLen);
		}

	}
}
