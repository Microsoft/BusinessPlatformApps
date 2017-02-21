#load "HtmlCleanerResult.csx"

using NSoup;
using NSoup.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class HtmlCleaner
{
	public HtmlCleanerResult Clean(String html)
	{
		var root = NSoupClient.Parse(html);
		var noTags = root.Text();

		var builder = new StringBuilder();
		var imageList = new List<string>();

		HandleNode(root, builder, imageList);

		var htmlresult = new HtmlCleanerResult()
		{
			NoTags = noTags,
			Scrubbed = builder.ToString(),
			Image = imageList.Count > 0 ? imageList.First() : null
		};
		return htmlresult;
	}

	private void HandleNode(Node node, StringBuilder builder, IList<string> images)
	{
		if (node is TextNode)
		{
			HandleTextNode(node as TextNode, builder);
			HandleChildren(node, builder, images);
		}
		else if (node is Element)
		{
			HandleElement(node as Element, builder, images);
		}
		else
		{
			HandleChildren(node, builder, images);
		}
	}

	private void HandleElement(Element node, StringBuilder builder, IList<string> images)
	{
		var tagName = node.TagName().ToLower();

		switch (tagName)
		{
			case "img":
				images.Add(node.Attributes["src"]);
				builder.Append(node.OuterHtml());
				break;
			case "br":
				builder.Append(node.OuterHtml());
				break;
			case "p":
				builder.Append("<p>");
				HandleChildren(node, builder, images);
				builder.Append("</p>");
				break;
			default:
				HandleChildren(node, builder, images);
				break;
		}
	}

	private void HandleChildren(Node node, StringBuilder builder, IList<string> images)
	{
		foreach (var x in node.ChildNodes)
		{
			HandleNode(x, builder, images);
		}
	}

	private void HandleTextNode(TextNode node, StringBuilder builder)
	{
		if (builder.Length != 0)
		{
			var lastCharacter = builder[builder.Length - 1];

			if (lastCharacter != ' ' && lastCharacter != '>')
			{
				builder.Append(' ');
			}
		}

		builder.Append(node.GetWholeText());
	}
}
