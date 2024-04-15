using WriteMetrics;

var filename = (args?[0]) ?? throw new InvalidDataException("filename");

var collector = new RecursiveCollector(filename);

collector.Collect();
collector.Report();

