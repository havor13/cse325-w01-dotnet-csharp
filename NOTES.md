# W01 Assignment Notes: Build .NET Applications with C#

Environment: .NET 8 LTS SDK (8.0.427), pinned with `global.json`. Visual Studio Code.

Projects in this repository:

- `Module6-PizzaApi`: ASP.NET Core web API (PizzaController, PizzaService)
- `Module5-Files`: files and directories app, including the sales summary report function

---

## 1. Existing pizzas and my additional pizza

In `Services/PizzaService.cs` the seed data originally held Classic Italian (Id 1) and Veggie (Id 2). I added a third pizza, Hawaiian (Id 3), and set `nextId` to 4 so new pizzas do not reuse an existing Id.

```csharp
Pizzas = new List<Pizza>
{
    new Pizza { Id = 1, Name = "Classic Italian", IsGlutenFree = false },
    new Pizza { Id = 2, Name = "Veggie", IsGlutenFree = true },
    new Pizza { Id = 3, Name = "Hawaiian", IsGlutenFree = false }
};
```

Evidence: `GET https://localhost:7005/Pizza` returned `200 OK` with all three pizzas:

```json
[
  {
    "id": 1,
    "name": "Classic Italian",
    "isGlutenFree": false
  },
  {
    "id": 2,
    "name": "Veggie",
    "isGlutenFree": true
  },
  {
    "id": 3,
    "name": "Hawaiian",
    "isGlutenFree": false
  }
]
```

---

## 2. API requests, responses, and status codes

All requests were sent from Swagger UI at `https://localhost:7005/swagger`.

### GET (all pizzas)

Request:

```
GET https://localhost:7005/Pizza
accept: text/plain
```

Response: `200 OK`, body is the three-pizza list shown in section 1.

Status code: **200**

### POST (create a pizza)

Request:

```
POST https://localhost:7005/Pizza
Content-Type: application/json

{
  "id": 0,
  "name": "string",
  "isGlutenFree": true
}
```

Response (header `location: https://localhost:7005/Pizza/4`):

```json
{
  "id": 4,
  "name": "string",
  "isGlutenFree": true
}
```

Status code: **201 Created**

### PUT (update a pizza)

Request:

```
PUT https://localhost:7005/Pizza/4
Content-Type: application/json

{
  "id": 4,
  "name": "Pepperoni Supreme",
  "isGlutenFree": false
}
```

Response: no body.

Status code: **204 No Content**

To confirm the update, `GET https://localhost:7005/Pizza/4` returned **200 OK**:

```json
{
  "id": 4,
  "name": "Pepperoni Supreme",
  "isGlutenFree": false
}
```

### DELETE (remove a pizza)

Request:

```
DELETE https://localhost:7005/Pizza/4
```

Response: no body.

Status code: **204 No Content**

To confirm the delete, `GET https://localhost:7005/Pizza` returned **200 OK** with the original three pizzas (Ids 1, 2, and 3).

---

## 3. Sales summary function (Module 5, Work with files and directories)

Location: `Module5-Files/Program.cs`. It is called after the totals are calculated:

```csharp
GenerateSalesSummary(salesFiles, storesDirectory, salesTotalDir);
```

Function (uses `System.Globalization`, `System.Text`, and `Newtonsoft.Json`):

```csharp
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
```

Output written to `salesTotalDir/salesSummary.txt`:

```
Sales Summary
----------------------------
 Total Sales: $2,012.20

 Details:
  sales.json: $88.88
  201\sales.json: $501.22
  202\sales.json: $1,234.22
  203\sales.json: $99.00
  204\sales.json: $88.88
```