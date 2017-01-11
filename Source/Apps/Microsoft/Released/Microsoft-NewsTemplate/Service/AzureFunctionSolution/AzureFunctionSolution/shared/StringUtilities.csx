using System;
using System.Text;

public class StringUtilities
{
	public String RemoveUnicode(String input)
	{
		return Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(input));
	}
}
