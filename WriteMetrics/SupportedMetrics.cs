namespace WriteMetrics;

// TODO: move to a config
public static class SupportedMetrics
{
    public static List<string> MetricCategories => 
        (typeof(SupportedMetrics)).GetFields().Select(f => f.Name.ToLower()).ToList();

    public static readonly List<string> WordCount =
    [
        "sum",
        "average",
        "keyword"
    ];
}