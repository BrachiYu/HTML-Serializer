using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Selector
{
    public string TagName { get; set; }
    public string Id { get; set; }
    public List<string> Classes { get; set; } = new List<string>();
    public Selector Parent { get; set; }
    public Selector Child { get; set; }

    public static Selector Parse(string query)
    {
        var parts = query.Split(' ');
        Selector root = null;
        Selector current = null;

        foreach (var part in parts)
        {
            var selector = new Selector();
            var segments = Regex.Split(part, "(?=#|\\.)");

            foreach (var segment in segments)
            {
                if (segment.StartsWith("#"))
                {
                    selector.Id = segment.Substring(1);
                }
                else if (segment.StartsWith("."))
                {
                    selector.Classes.Add(segment.Substring(1));
                }
                else if (HtmlHelper.Instance.AllTags.Contains(segment))
                {
                    selector.TagName = segment;
                }
            }

            if (root == null)
            {
                root = selector;
            }
            else
            {
                current.Child = selector;
            }

            current = selector;
        }

        return root;
    }
}