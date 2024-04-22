using System.Text;

namespace WriteMetrics;

public class RecursiveCollector
{
    public string CurrentFile { get; set; } = string.Empty;
    private FileStream DocumentFilestream { get; set; }
    private StreamReader DocumentStreamReader { get; set; }

    public Node ParentNode { get; set; }
    private Node CurrentNode { get; set; }

    private string CurrentTextSection { get; set; } = string.Empty;

    public RecursiveCollector(string filename)
    {
        CurrentFile = filename;

        DocumentFilestream = File.OpenRead(filename);
        DocumentStreamReader = new StreamReader(DocumentFilestream, Encoding.UTF8, true, 128);

        ParentNode = new Node(CurrentFile, 0);
        CurrentNode = ParentNode;
    }

    ~RecursiveCollector() 
    {
        DocumentFilestream?.Dispose();
        DocumentStreamReader?.Dispose();
    }

    public void Collect()
    {
        CurrentNode = ParentNode;

        if (DocumentStreamReader == null)
            throw new InvalidOperationException("No document streamed");

        string line;
        while ((line = DocumentStreamReader.ReadLine()) != null)
        {
            if (line.StartsWith('#'))
            {
                WrapUpSection();

                ParseAsHeading(line);
            }
            else
                CurrentTextSection += line;
        }

        // final section
        WrapUpSection();


    }

    internal void WrapUpSection()
    {
        if (!string.IsNullOrEmpty(CurrentTextSection))
        {
            CurrentNode.Text = CurrentTextSection;
            CurrentNode.WordCount = CurrentTextSection.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;           
            CurrentTextSection = string.Empty;

            var nodeToUpdate = CurrentNode.Parent;
            while (nodeToUpdate != null)
            {
                nodeToUpdate.WordCount += CurrentNode.WordCount;
                nodeToUpdate = nodeToUpdate.Parent;
            }
        }
    }

    internal void ParseAsHeading(string line)
    {
        var level = line.Count(l => l == '#');
        var withoutPrefix = line.Replace("#", string.Empty);
        var heading = withoutPrefix.Trim();

        var node = new Node(heading, level);
        
        Node? parent = null;

        // sibling
        if (level == CurrentNode.Level)
            parent = CurrentNode.Parent;
        // new uncle to the current
        else if (level < CurrentNode.Level)
        {
            parent = CurrentNode.Parent;
            for (int i = CurrentNode.Level; i > level; --i)
            {
                parent = parent?.Parent;
            }

            if (parent == null)
                throw new InvalidOperationException();
        }
        // child
        // TODO: fragile if bad nesting
        else if (level > CurrentNode.Level)
        {
            parent = CurrentNode;
        }

        if (parent == null)
            throw new InvalidOperationException();

        node.Parent = parent;
        parent.Children.Add(node);

        CurrentNode = node;
    }

    //public void Report(bool csvFormat = false)
    //{
    //    CsvOutputFormat = csvFormat;

    //    if (CsvOutputFormat)
    //        Console.WriteLine("Outline, Num, Name, Word Count");

    //    Traverse(ParentNode);
    //}

    //internal void Traverse(Node node)
    //{
    //    if (node.Parent != null)
    //        ReportInfo(node);

    //    foreach (var n in node.Children)
    //    {
    //        Traverse(n);
    //    }
    //}

    //internal void ReportInfo(Node node, string mode)
    //{

    //    // TODO: improve
    //    var padding = CsvOutputFormat ? string.Empty : new string(' ', node.Level * 4);

    //    if (node.Heading.Contains("Title"))
    //        return;

    //    var parsedHeading = ParsedHeading(node.Heading);
    //    var reportLine = $"{padding}{parsedHeading}, {node.WordCount}";

    //    Console.WriteLine(reportLine);
    //}

    //internal string ParsedHeading(string heading)
    //{
    //    // TODO: UGLY
    //    if (CsvOutputFormat)
    //    {
    //        heading = heading.Replace(":", ",")
    //            .Replace("Prolgue ", "Prologue, ")
    //            .Replace("Part ", "Part, ")
    //            .Replace("Episode ", "Episode, ")
    //            .Replace("Chapter ", "Chapter, ")
    //            .Replace("Scene ", "Scene, ");
    //    }

    //    return heading + ",";
    //}

}
