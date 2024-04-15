namespace WriteMetrics;

public class Node
{
    public string Heading { get; set; } = string.Empty;
    public int Level { get; set; } = 0;
    public int WordCount { get; set; } = 0;

    public List<Node> Children { get; set; } = [];

    public Node? Parent { get; set; } = null;

    public Node() { }

    public Node(string heading, int level, Node? parent = null)
    {
        Heading = heading;
        Parent = parent;
        Level = level;
    }
}
