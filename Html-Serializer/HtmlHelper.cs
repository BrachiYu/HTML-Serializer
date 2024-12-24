using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

public class HtmlHelper
{
    private static readonly HtmlHelper _instance = new HtmlHelper();
    public static HtmlHelper Instance => _instance;

    public List<string> AllTags { get; private set; }
    public List<string> VoidTags { get; private set; }

    private HtmlHelper()
    {
        AllTags = LoadTags("HtmlTags.json");
        VoidTags = LoadTags("HtmlVoidTags.json");
    }

    private List<string> LoadTags(string filePath)
    {
        try
        {
            var content = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<string>>(content) ?? new List<string>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading file {filePath}: {ex.Message}");
            return new List<string>();
        }
    }
}