using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileStoneProdEvents.milestone
{
    public class MileStoneStartup
    {
        public void Start()
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;

            string filePath = @"E:\Camera details.xlsx";
            Dictionary<string, bool> cameraMap = new Dictionary<string, bool>();
            try
            {
                // Open and read the Excel file
                FileInfo fileInfo = new FileInfo(filePath);

                using (var package = new ExcelPackage(fileInfo))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Select the first worksheet (index 0)

                    int rowCount = worksheet.Dimension.Rows;
                    int colCount = worksheet.Dimension.Columns;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var cellValue = worksheet.Cells[row, 1].Value.ToString().Trim();
                        if (!cameraMap.ContainsKey(cellValue))
                        {
                            cameraMap[cellValue] = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }


            string rootFolder = @"E:\data";

            string[] subdirectories = Directory.GetDirectories(rootFolder);

            var missingCamera = new List<string>();
            var groupResult = new Dictionary<string, int>();
            foreach (string subdir in subdirectories)
            {
                string actualDirectory = Path.GetFileName(subdir);
                string sourceFolder = subdir;
                string[] arr = Directory.GetFiles(sourceFolder, "*.json");
                foreach (string file in arr)
                {
                    string fileName = System.IO.Path.GetFileName(file);

                    var data = File.ReadAllText(file);
                    var items = JsonConvert.DeserializeObject<List<MileStoneProdModel>>(data).Select(a => a.CameraName.Trim()).Distinct().ToList();

                    var aitems = JsonConvert.DeserializeObject<List<MileStoneProdModel>>(data).GroupBy(a => a.CameraName).ToList();
                    foreach (var item in aitems)
                    {
                        var ab = item.Key;
                        if (!groupResult.ContainsKey(item.Key))
                        {
                            groupResult[item.Key] = 1;
                        }
                        else
                        {
                            groupResult[item.Key]++;
                        }
                    }
                    //groupResult.AddRange(a);
                    var nonExistingItems = items.Except(cameraMap.Keys).ToList();
                    if (nonExistingItems.Any())
                    {
                        var pmcl = missingCamera.Count();
                        missingCamera = missingCamera.Union(nonExistingItems.Except(missingCamera)).ToList();
                        var nmcl = missingCamera.Count();
                        if (pmcl != nmcl)
                        {
                            Console.WriteLine("missing camera file directory : " + sourceFolder + "..." + fileName);
                        }

                    }
                }

            }


            Console.WriteLine("--------------------------------------------------------");
            foreach (string cm in missingCamera)
            {
                Console.WriteLine(cm);
            }

            Console.WriteLine("----------------------------------------------");
            foreach (var cm in groupResult.Keys)
            {
                Console.WriteLine(cm);
            }

        }
    }
}
