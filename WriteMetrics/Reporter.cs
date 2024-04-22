using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

namespace WriteMetrics;

public static class Reporter
{
    public static IConfiguration? Configuration {  get; set; }

    private static Node _parentNode;
    private static Action<Node> _reportLineFunc;

    public static void Report(Node node, string format)
    {
        if (Configuration == null)
            throw new InvalidDataException("No configuration set");

        _parentNode = node;

        switch (format)
        {
            case "console":
                ConsoleReport(node);
                break;

            case "csv":
            case "chart":
            default:
                throw new ArgumentException("Unrecognized mode");
        }

        //_reportLineFunc = format switch
        //{
        //    "console" => (Action<Node>)ConsoleReportLine,
        //    "csv" => (Action<Node>)CsvReportLine,
        //    "chart" => (Action<Node>)ChartDataReportLine,
        //    _ => throw new ArgumentException("Unrecognized mode")
        //};

        //Traverse(_parentNode);
    }

    internal static void ConsoleReport(Node parentNode)
    {
        _reportLineFunc = ConsoleReportLine;
        Traverse(parentNode);
    }

    internal static void ConsoleReportLine(Node node)
    {
        // exclusion rules
        if (node.MatchesCategory("Title"))
            return;

        var padding = new string(' ', node.Level * 4);
        var heading = node.ToHeader();
        var reportLine = $"{padding}{heading} - {node.WordCount}";

        Console.WriteLine(reportLine);
    }

    internal static void CsvReportLine(Node node)
    {
        // exclusion rules
        if (node.MatchesCategory("Title"))
            return;

        Console.WriteLine(node.ToString());
    }

    internal static void ChartDataReportLine(Node node)
    {

    }

    internal static string ParseCommaDelimitedHeader(string heading)
    {
        var supportedHeadingsRegExPartial = string.Join('|', SupportedHeadings.All);
        var categoryPattern = @"(" + supportedHeadingsRegExPartial + @")";

        var m = Regex.Match(heading, categoryPattern);
        var category = m.Success ? m.Value : string.Empty;

        var partial = heading.Replace(category, "").Trim();

        m = Regex.Match(partial, @"\s*([0-9\.]*):\s*[A-Za-z]*");

        var outlineNumber = string.Empty;
        if (m.Success)
        {
            outlineNumber = m.Groups[1].Value;
            partial = partial
                .Replace(outlineNumber, "")
                .Replace(":", "")
                .Trim();
        }

        var replacedHeading = $"{category}, {outlineNumber}, {partial}";

        return replacedHeading;
    }

    internal static void Traverse(Node node)
    {
        if (node.Parent != null)
            _reportLineFunc(node);

        foreach (var n in node.Children)
            Traverse(n);
    }
}
