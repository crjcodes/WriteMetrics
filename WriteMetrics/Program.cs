using Microsoft.Extensions.Configuration;
using WriteMetrics;

// SETTINGS

var configuration = new ConfigurationBuilder()
    .AddCommandLine(args.Select(a => a.ToLower()).ToArray<string>())
    .Build();

if (configuration == null)
    configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?>()
        {
            { "file", "*.md" },
            { "format", "console" },
            { "wordcount", "sum" }
        })
        .Build();

var file = configuration["file"];

if (string.IsNullOrEmpty(file))
    throw new ArgumentException("Must provide a filename as 'file={myfile}`");

var format = configuration["format"] ?? "console";
var metricsToCollect = SupportedMetrics.MetricCategories.Where(c => !string.IsNullOrWhiteSpace(configuration[c]));

// COLLECT

var collector = new RecursiveCollector(file);
collector.Collect();

// REPORT

Reporter.Configuration = configuration;
Reporter.Report(collector.ParentNode, format);

