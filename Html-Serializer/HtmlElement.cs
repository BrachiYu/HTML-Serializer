using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class HtmlElement
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<KeyValuePair<string, string>> Attributes { get; set; } = new List<KeyValuePair<string, string>>();
    public List<string> Classes { get; set; } = new List<string>();
    public string InnerHtml { get; set; } = "";
    public HtmlElement Parent { get; set; }
    public List<HtmlElement> Children { get; set; } = new List<HtmlElement>();

    public HtmlElement(string name)
    {
        Name = name;
    }
    public IEnumerable<HtmlElement> Descendants()
    {
        var queue = new Queue<HtmlElement>();
        queue.Enqueue(this);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            yield return current;

            foreach (var child in current.Children)
            {
                queue.Enqueue(child);
            }
        }
    }

    public IEnumerable<HtmlElement> Ancestors()
    {
        var current = Parent;

        while (current != null)
        {
            yield return current;
            current = current.Parent;
        }
    }

    public override string ToString()
    {
        var attributesString = Attributes.Any()
            ? " " + string.Join(" ", Attributes.Select(a => $"{a.Key}=\"{a.Value}\""))
            : "";
        var classesString = Classes.Any() ? $" class=\"{string.Join(" ", Classes)}\"" : "";
        string openingTag = $"<{Name}{(string.IsNullOrEmpty(Id) ? "" : $" id=\"{Id}\"")}{classesString}{attributesString}>";
        string closingTag = $"</{Name}>";
        return $"{openingTag}{InnerHtml}{closingTag}";
    }

}
public static class HtmlElementExtensions
{
    public static IEnumerable<HtmlElement> Query(this HtmlElement element, Selector selector)
    {
        var results = new HashSet<HtmlElement>();
        QueryRecursive(element, selector, results);
        return results;
    }

    private static void QueryRecursive(HtmlElement element, Selector selector, HashSet<HtmlElement> results)
    {
        var descendants = element.Descendants();

        var matchingElements = descendants.Where(e =>
    (string.IsNullOrEmpty(selector.TagName) || e.Name == selector.TagName) &&
    (string.IsNullOrEmpty(selector.Id) || e.Id == selector.Id) &&
    (!selector.Classes.Any() || selector.Classes.All(cls => e.Classes.Contains(cls)))
);
        if (!matchingElements.Any())
        {
            Console.WriteLine($"No elements matched the selector: {selector.TagName} {selector.Id} {string.Join(".", selector.Classes)}");
            return;
        }





        if (selector.Child == null)
        {
            foreach (var match in matchingElements)
            {
                results.Add(match);
            }
        }
        else
        {
            foreach (var match in matchingElements)
            {
                QueryRecursive(match, selector.Child, results);
            }
        }
    }
}
