using System.Globalization;
using System.Text;
using Newtonsoft.Json;

var currentDirectory = Directory.GetCurrentDirectory();
var storesDirectory = Path.Combine(currentDirectory, "stores");

var salesTotalDir = Path.Combine(currentDirectory, "salesTotalDir");
Directory.CreateDirectory(salesTotalDir);

var salesFiles = FindFiles(storesDirectory);

var salesTotal = CalculateSalesTotal(salesFiles);

File.AppendAllText(Path.Combine(salesTotalDir, "totals.txt"), $"{salesTotal}{Environment.NewLine}");

GenerateSalesSummary(salesFiles, storesDirectory, salesTotalDir);

IEnumerable<string> FindFiles(string folderName)
{
    List<string> salesFiles = new List<string>();

    var foundFiles = Directory.EnumerateFiles(folderName, "*", SearchOption.AllDirectories);

    foreach (var file in foundFiles)
    {
        var extension = Path.GetExtension(file);
        if (extension == ".json")
        {
            salesFiles.Add(file);
        }
    }

    return salesFiles;
}

double CalculateSalesTotal(IEnumerable<string> salesFiles)
{
    double salesTotal = 0;

    foreach (var file in salesFiles)
    {
        string salesJson = File.ReadAllText(file);

        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);

        salesTotal += data?.Total ?? 0;
    }

    return salesTotal;
}

// Builds a text report with the grand total and a per-file breakdown,
// then writes it to salesSummary.txt in the output directory.
void GenerateSalesSummary(IEnumerable<string> salesFiles, string storesDirectory, string outputDir)
{
    // Use en-US so the currency symbol is "$" regardless of regional settings.
    var culture = CultureInfo.GetCultureInfo("en-US");
    var details = new StringBuilder();
    double grandTotal = 0;

    // Only sales.json files hold a "Total" value; salestotals.json files do not.
    foreach (var file in salesFiles.Where(f => Path.GetFileName(f) == "sales.json"))
    {
        var data = JsonConvert.DeserializeObject<SalesData>(File.ReadAllText(file));
        double fileTotal = data?.Total ?? 0;
        grandTotal += fileTotal;

        var relativeName = Path.GetRelativePath(storesDirectory, file);
        details.AppendLine($"  {relativeName}: {fileTotal.ToString("C", culture)}");
    }

    var report = new StringBuilder();
    report.AppendLine("Sales Summary");
    report.AppendLine("----------------------------");
    report.AppendLine($" Total Sales: {grandTotal.ToString("C", culture)}");
    report.AppendLine();
    report.AppendLine(" Details:");
    report.Append(details);

    File.WriteAllText(Path.Combine(outputDir, "salesSummary.txt"), report.ToString());
}

record SalesData(double Total);