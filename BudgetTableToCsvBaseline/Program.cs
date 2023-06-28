using HtmlAgilityPack; 
using CsvHelper; 
using CsvHelper.Configuration;
using System.Globalization;
using ConsoleTableExt;

namespace BudgetTableToCsvBaseline 
{ 
    public static class Program 
    {
        private static void ConvertBudgetTableToCsv(string htmlData, string csvPath) {
            try {
                Console.WriteLine("### Started Parsing HTML table.");
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(htmlData);

                var tableHeader = htmlDoc.DocumentNode.SelectNodes("//table/thead/tr/th");
                var tableHeaderTexts = tableHeader.Select(x => x.InnerText.Trim()).ToList();
                var countHeaderColumns = tableHeader.Count;

                var tableRows = htmlDoc.DocumentNode.SelectNodes("//table/tbody/tr");
                var countRows = tableRows.Count;
                var tableCellTexts = new List<List<object>>(countRows) { new(countHeaderColumns) };

                var indexRow = 1;
                foreach (var row in tableRows)
                {
                    foreach (var cell in row.SelectNodes($"//table/tbody/tr[{indexRow}]/td"))
                    {
                        tableCellTexts[indexRow-1].Add(cell.InnerText.Trim());
                    }
                    indexRow++;
                    tableCellTexts.Add(new List<object>());
                }
                
                ConsoleTableBuilder
                    .From(tableCellTexts)
                    .WithColumn(tableHeaderTexts)
                    .ExportAndWriteLine();

                Console.WriteLine("### Started writing table to csv file.");
                WriteCsv(csvPath, tableCellTexts, tableHeaderTexts);
                
            } catch (Exception ex) {
                Console.WriteLine("ConvertBudgetTableToCsv Failed: {0}", ex.Message);
            }
        }

        private static void WriteCsv(string file, List<List<object>> rowRecords, List<string> tableHeader)
        {
            var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                Comment = '#',
                HasHeaderRecord = false,
                MissingFieldFound = null
            };
            
            using (StreamWriter sw = new StreamWriter(file))
            using (CsvWriter cw = new CsvWriter(sw, configuration))
            {
                foreach (var column in tableHeader)
                {
                    cw.WriteField(column);
                }
                cw.NextRecord();

                foreach (var row in rowRecords)
                {
                    foreach (var field in row)
                    {
                        cw.WriteField(field);
                    }
                    cw.NextRecord();
                }
            }
        }
        
        static async Task Main(string[] args) {
            var htmlText = await File.ReadAllTextAsync("C:\\Temp\\budget_table.html");
            var csvBaseline = "C:\\Temp\\budget_table.csv";
            
            ConvertBudgetTableToCsv(htmlText,csvBaseline);
        }
    } 
}