using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.IO.RecyclableMemoryStreamManager;

namespace MileStoneProdEvents.Una
{
    public class GenerateOutputExcel
    {
        public void GenerateOutput(Dictionary<string, List<KeyValuePair<string, dynamic>>> dataMap)
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            using (var package = new ExcelPackage())
            {
                foreach (KeyValuePair<string, List<KeyValuePair<string, dynamic>>> kvp in dataMap)
                {
                    string deviceType = kvp.Key;
                    List<KeyValuePair<string, dynamic>> values = kvp.Value;

                    var worksheet = package.Workbook.Worksheets.Add(deviceType);
                    PopulateWorksheet(worksheet, values);
                }

                // Save the Excel file
                var customPath = @"C:\Users\LENOVO\Downloads\dummy\OutputFile.xlsx";
                var excelFile = new FileInfo(customPath);
                package.SaveAs(excelFile);
            }
        }

        static void PopulateWorksheet(ExcelWorksheet worksheet, List<KeyValuePair<string, dynamic>> values)
        {
            int row = 1;
            int col = 1;

            //set headers
            worksheet.Cells[row, col].Value = "FileName";
            col++;
            worksheet.Cells[row, col].Value = "Data";

            values = values.OrderBy(kvp => UnixTimeStampToDateTime((long)kvp.Value.timestamp)).ToList();

            //values = values.OrderBy(kvp => (long)kvp.Value.timestamp).ToList();

            // Populate Excel cells with data from the object
            foreach (var item in values)
            {
                row++;
                col = 1;
                worksheet.Cells[row, col++].Value = item.Key;
                worksheet.Cells[row, col].Value = item.Value.ToString();

            }
        }

        static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            return DateTimeOffset.FromUnixTimeSeconds((long)unixTimeStamp).UtcDateTime;
        }


    }
}

