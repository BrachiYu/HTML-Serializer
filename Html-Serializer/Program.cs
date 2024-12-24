using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


var html = await Load("https://gemini.google.com/app");

//var cleanHtml = new Regex("\\s").Replace(html, "");

//var htmlLines = new Regex("<(.*?)").Split(cleanHtml).Where(s => s.Length > 0);
//var htmlElement = "<div id=\"my-id\" class=\"my-class-1 my-class-2\" width=\"100%\">text</div>";
//var attributes = new Regex("([^\\s]*?)=\"(.*?)\"").Matches(htmlElement);

var htmlLines = Regex.Split(html, "(?=<)").Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
Console.WriteLine(html);


var root = new HtmlElement("root");
var current = root;

foreach (var line in htmlLines)
{
    var trimmedLine = line.Trim();

    if (trimmedLine.StartsWith("</"))
    {
        current = current.Parent ?? root;
        continue;
    }

    var tagMatch = Regex.Match(trimmedLine, "<(\\w+)(.*?)>");
    if (tagMatch.Success)
    {
        var tagName = tagMatch.Groups[1].Value;
        var attributesString = tagMatch.Groups[2].Value;

        var newElement = new HtmlElement(tagName);
        ParseAttributes(newElement, attributesString);

        if (HtmlHelper.Instance.VoidTags.Contains(tagName) || trimmedLine.EndsWith("/"))
        {
            current.Children.Add(newElement);
        }
        else
        {
            current.Children.Add(newElement);
            newElement.Parent = current;
            current = newElement;
        }

        continue;
    }

    if (!trimmedLine.StartsWith("<"))
    {
        current.InnerHtml += trimmedLine;
    }
}
Console.WriteLine("Enter a selector query (e.g., 'div.class-name'):");
var query = Console.ReadLine();
var selector = Selector.Parse(query);
var matchingElements = root.Query(selector);

Console.WriteLine("Matching elements:");
foreach (var element in matchingElements)
{
    Console.WriteLine(element);
}
Console.ReadLine();

static void ParseAttributes(HtmlElement element, string attributesString)
{
    var attributeRegex = new Regex("(\\w+)=\\\"(.*?)\\\"");
    var matches = attributeRegex.Matches(attributesString);

    foreach (Match match in matches)
    {
        var attributeName = match.Groups[1].Value;
        var attributeValue = match.Groups[2].Value;

        if (attributeName == "id")
        {
            element.Id = attributeValue;
        }
        else if (attributeName == "class")
        {
            element.Classes.AddRange(attributeValue.Split(' '));
        }
        else
        {
            element.Attributes.Add(new KeyValuePair<string, string>(attributeName, attributeValue));
        }
    }
}

static void PrintTree(HtmlElement element, int indent)
{
    Console.WriteLine($"{new string(' ', indent * 2)}<{element.Name} id=\"{element.Id}\" class=\"{string.Join(" ", element.Classes)}\">");

    if (!string.IsNullOrEmpty(element.InnerHtml))
    {
        Console.WriteLine($"{new string(' ', (indent + 1) * 2)}{element.InnerHtml}");
    }

    foreach (var child in element.Children)
    {
        PrintTree(child, indent + 1);
    }

    Console.WriteLine($"{new string(' ', indent * 2)}</{element.Name}>");
}

async Task<string> Load(string url)
{
    HttpClient client = new HttpClient();
    var response = await client.GetAsync(url);
    var html = await response.Content.ReadAsStringAsync();
    return html;
}

