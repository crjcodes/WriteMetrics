using WriteMetrics;

var filename = (args?[0]) ?? throw new InvalidDataException("filename");

bool csvFormat = false;
if (args.Length > 1 && args[1] == "csv")
    csvFormat = true;


var collector = new RecursiveCollector(filename);
collector.Collect();
collector.Report(csvFormat: csvFormat);

