using System.Text.RegularExpressions;

namespace WriteMetrics;

public class Node
{
    public string SectionCategory { get; set; } = string.Empty;
    public string SectionOutlineReference { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;

    public int Level { get; set; } = 0;
    public string Text { get; set; } = string.Empty;

    public int WordCount { get; set; } = 0;

    public List<Node> Children { get; set; } = [];

    public Node? Parent { get; set; } = null;

    public Node() { }

    public Node(string heading, int level, Node? parent = null)
    {
        //Heading = heading;
        ParseHeading(heading);
        Parent = parent;
        Level = level;
    }

    internal void ParseHeading(string heading)
    {
        var supportedHeadingsRegExPartial = string.Join('|', SupportedHeadings.All);
        var categoryPattern = @"(" + supportedHeadingsRegExPartial + @")";

        var m = Regex.Match(heading, categoryPattern);
        SectionCategory = m.Success ? m.Value : string.Empty;

        var partial = string.IsNullOrWhiteSpace(SectionCategory) 
            ? heading : heading.Replace(SectionCategory, "").Trim();

        m = Regex.Match(partial, @"\s*([0-9\.]*):\s*[A-Za-z]*");
        if (m.Success)
        {
            SectionOutlineReference = m.Groups[1].Value;

            partial = string.IsNullOrWhiteSpace(SectionOutlineReference)
                ? partial : partial.Replace(SectionOutlineReference, "");

            partial = partial
                .Replace(":", "")
                .Trim();
        }

        SectionName = partial;
    }

    public bool MatchesCategory(string category) =>
        SectionCategory.Equals(category, StringComparison.InvariantCultureIgnoreCase);

    public override string ToString() => $"{SectionCategory}, {SectionOutlineReference}, {SectionName}, {WordCount}";

    public string ToHeader() => $"{SectionCategory} {SectionOutlineReference}: {SectionName}";
}
