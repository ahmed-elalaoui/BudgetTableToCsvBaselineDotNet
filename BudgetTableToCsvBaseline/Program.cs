using HtmlAgilityPack; 
using CsvHelper; 
using System.Globalization;
using System.Text;
using ConsoleTableExt;

namespace BudgetTableToCsvBaseline 
{ 
    public class Program 
    { 
        static void ParseHtml(string htmlData) {
            try {
                Console.WriteLine("### Started Parsing HTML.");
                var coinData = new Dictionary < string,
                    string > ();
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(htmlData);

                //var theTable = htmlDoc.DocumentNode.SelectSingleNode("table");
                //var cmcTableHeader = htmlDoc.DocumentNode.SelectNodes("//table/thead/tr/th");
                //var cmcTableBody = theTable.SelectSingleNode("//tbody");
                //var cmcTableRows = cmcTableBody.SelectNodes("tr");
                // if (cmcTableRows != null) {
                //     foreach(HtmlNode row in cmcTableRows) {
                //         var cmcTableColumns = row.SelectNodes("td");
                //         string name = cmcTableColumns[2].InnerText;
                //         string price = cmcTableColumns[3].InnerText;
                //         coinData.Add(name, price);
                //     }
                // }

                var tableHeader = htmlDoc.DocumentNode.SelectNodes("//table/thead/tr/th");
                List<string> tableHeaderTexts = tableHeader.Select(x => x.InnerText.Trim()).ToList();
                var countHeaderColumns = tableHeader.Count;


                var tableRows = htmlDoc.DocumentNode.SelectNodes("//table/tbody/tr");
                var countRows = tableRows.Count;
                //var tableCellTexts = new List<string[]>(countRows) { new string[countHeaderColumns] };
                var tableCellTexts = new List<List<object>>(countRows) { new(countHeaderColumns) };
                //tableCellTexts = new List<string[]> { new string[10] };
                int indexRow = 1;
                foreach (var row in tableRows)
                {
                    int indexCell = 0;
                    foreach (var cell in row.SelectNodes($"//table/tbody/tr[{indexRow}]/td"))
                    {
                        //tableCellTexts[indexRow-1][indexCell] = cell.InnerText;
                        tableCellTexts[indexRow-1].Add(cell.InnerText.Trim());
                        indexCell++;
                    }
                    indexRow++;
                    tableCellTexts.Add(new List<object>());
                }
                // List<string> tableRowTexts = htmlDoc.DocumentNode.SelectNodes("//table/tbody/tr/td")
                //     .Select(x => x.InnerText)
                //     .ToList();
                
                ConsoleTableBuilder
                    .From(tableCellTexts)
                    .WithColumn(tableHeaderTexts)
                    .ExportAndWriteLine();

                // var tableData = new List<List<object>>
                // {
                //     new List<object>{ "Sakura Yamamoto", "Support Engineer", "London", 46},
                //     new List<object>{ "Serge Baldwin", "Data Coordinator", "San Francisco", 28, "something else" },
                //     new List<object>{ "Shad Decker", "Regional Director", "Edinburgh"},
                // };
                //
                // ConsoleTableBuilder
                //     .From(tableData)
                //     .WithColumn("Id", "First Name", "Sur Name")
                //     .ExportAndWriteLine();

                
                WriteDataToCsv(coinData);
            } catch (Exception ex) {
                Console.WriteLine("ParseHtml Failed: {0}", ex.Message);
            }
        }
        
        static void WriteDataToCsv(Dictionary < string, string > cryptoCurrencyData) {
            try {
                var csvBuilder = new StringBuilder();

                csvBuilder.AppendLine("Name,Price");
                foreach(var item in cryptoCurrencyData) {
                    csvBuilder.AppendLine(string.Format("{0},\"{1}\"", item.Key, item.Value));
                }
                File.WriteAllText("C:\\Cryptocurrency-Prices.csv", csvBuilder.ToString());

                Console.WriteLine("### Completed Writing Data To CSV File.");
            } catch (Exception ex) {
                Console.WriteLine("WriteDataToCSV Failed: {0}", ex.Message);
            }
        }
        
        static async Task Main(string[] args) {
            string htmlText = await File.ReadAllTextAsync("C:\\Temp2\\aok.html");
            ParseHtml(htmlText);
        }
    } 
}