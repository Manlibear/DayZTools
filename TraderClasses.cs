using System.Globalization;
using System.Text.Json;
using ClosedXML.Excel;
using CsvHelper;
using DayZTools.Models;

namespace DayZTools.Functions
{


    public static class TraderClasses
    {
        public static void XLSXToJson(string inputFile)
        {
            var inputWorkbook = new XLWorkbook(inputFile);
            var outputTraderFile = new TraderFile()
            {
                Version = "2.5",
                EnableAutoCalculation = 1,
                EnableDefaultTraderStock = 2,
                EnableAutoDestockAtRestart = 1,
                TraderCategories = new List<TraderCategory>()
            };

            TraderCategory? traderCategoryGroup = null;

            foreach (var row in inputWorkbook.Worksheet(1).Rows())
            {
                if (row.Cell("B").Value.ToString() == "coefficient") // jank way to check if it's a header row, but whatever
                {
                    traderCategoryGroup = new TraderCategory()
                    {
                        CategoryName = row.Cell("A").Value.ToString(),
                        Products = []
                    };

                    outputTraderFile.TraderCategories.Add(traderCategoryGroup);

                }
                else
                {
                    traderCategoryGroup?.Products.Add(string.Join(',', row.Cells("1:7").Select(x => x.Value.ToString())));
                }
            }

            var outputFile = Path.GetFileNameWithoutExtension(inputFile);
            var outputPath = Path.GetDirectoryName(inputFile);

            File.WriteAllText($"{outputPath}/{outputFile}.json", JsonSerializer.Serialize(outputTraderFile));

        }

        public static void JsonToXLSX(string inputFile)
        {
            var fileText = File.ReadAllText(inputFile);
            var traderFile = JsonSerializer.Deserialize<TraderFile>(fileText);

            foreach (var tc in traderFile?.TraderCategories ?? [])
            {
                foreach (var p in tc.Products)
                {

                    var readerConf = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        HasHeaderRecord = false,
                        MissingFieldFound = null
                    };

                    using var reader = new StringReader(p);
                    using var csvReader = new CsvReader(reader, readerConf);

                    tc.ProductObjects.Add(csvReader.GetRecords<Product>().First());
                }
            }


            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet 1");

                int rowIdx = 1;
                foreach (var tc in traderFile?.TraderCategories ?? [])
                {
                    worksheet.Row(rowIdx).Cell("A").Value = tc.CategoryName;
                    worksheet.Row(rowIdx).Cell("A").Style.Font.Bold = true;

                    worksheet.Row(rowIdx).Cell("B").Value = "coefficient";
                    worksheet.Row(rowIdx).Cell("B").Style.Font.FontColor = XLColor.SapGreen;

                    worksheet.Row(rowIdx).Cell("C").Value = "maxstock";
                    worksheet.Row(rowIdx).Cell("C").Style.Font.FontColor = XLColor.SapGreen;

                    worksheet.Row(rowIdx).Cell("D").Value = "trade quantity";
                    worksheet.Row(rowIdx).Cell("D").Style.Font.FontColor = XLColor.SapGreen;

                    worksheet.Row(rowIdx).Cell("E").Value = "buy price";
                    worksheet.Row(rowIdx).Cell("E").Style.Font.FontColor = XLColor.SapGreen;

                    worksheet.Row(rowIdx).Cell("F").Value = "sell price";
                    worksheet.Row(rowIdx).Cell("F").Style.Font.FontColor = XLColor.SapGreen;

                    worksheet.Row(rowIdx).Cell("G").Value = "destock coefficient";
                    worksheet.Row(rowIdx).Cell("G").Style.Font.FontColor = XLColor.SapGreen;

                    foreach (var p in tc.ProductObjects)
                    {
                        rowIdx++;
                        worksheet.Row(rowIdx).Cell("A").Value = p.Name;
                        worksheet.Row(rowIdx).Cell("B").Value = p.Coefficient;
                        worksheet.Row(rowIdx).Cell("C").Value = p.MaxStock;
                        worksheet.Row(rowIdx).Cell("D").Value = p.TradeQuantity;
                        worksheet.Row(rowIdx).Cell("E").Value = p.BuyPrice;
                        worksheet.Row(rowIdx).Cell("F").Value = p.SellPrice;
                        worksheet.Row(rowIdx).Cell("G").Value = p.DestockCoefficient;
                    }

                    rowIdx += 2;
                }

                var outputFile = Path.GetFileNameWithoutExtension(inputFile);
                var outputPath = Path.GetDirectoryName(inputFile);
                workbook.SaveAs($"{outputPath}/{outputFile}.xlsx");
            }
        }
    }
}