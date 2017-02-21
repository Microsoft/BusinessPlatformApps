using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class HtmlCleanerResult
{
	/// <summary>
	/// Minimal HTML reformatted for display.  All tags except for image and paragrah have been removed.  This is useful for UI display
	/// and may be considered the canonical document for analytics that rely on text position.
	/// </summary>
	public String Scrubbed { get; internal set; }

	/// <summary>
	/// Plain text without any HTML tags.  This is useful for text analytics.
	/// </summary>
	public String NoTags { get; internal set; }

	/// <summary>
	/// List of images in the document
	/// </summary>
	public String Image { get; internal set; }
}
