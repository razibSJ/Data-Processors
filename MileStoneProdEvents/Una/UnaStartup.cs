using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileStoneProdEvents.Una
{
    public class UnaStartup
    {
        public void Start()
        {

            Dictionary<string, List<KeyValuePair<string, dynamic>>> dataMap = new Dictionary<string, List<KeyValuePair<string, dynamic>>>();

            string sourceFolder = "C:\\Users\\LENOVO\\Downloads\\DataSourceUna";
            string[] subdirectories = Directory.GetDirectories(sourceFolder);

            foreach (string subdir in subdirectories)
            {
                string[] arr = Directory.GetFiles(subdir, "*.json");
                string subDirectoryName = System.IO.Path.GetFileName(subdir);

                foreach (string file in arr)
                {
                    string fileName = System.IO.Path.GetFileName(file);

                    JObject jsonData = JObject.Parse(File.ReadAllText(file));
                    var data = jsonData.ToObject<dynamic>();

                    var deviceType = data.deviceType.ToString();

                    if (deviceType != null)
                    {
                        if (dataMap.ContainsKey(deviceType))
                        {
                            dataMap[deviceType].Add(new KeyValuePair<string, dynamic>(subDirectoryName + "/" + fileName, data));
                        }
                        else
                        {
                            var values = new List<KeyValuePair<string, dynamic>>();
                            values.Add(new KeyValuePair<string, dynamic>(subDirectoryName + "/" + fileName, data));

                            dataMap.Add(deviceType, values);
                        }
                    }

                    if (deviceType == "unabellV1M2" || deviceType == "unareadergenerator1.0.0" || deviceType == "detectifytp1.0.0")
                    {

                    }

                }

            }
            new GenerateOutputExcel().GenerateOutput(dataMap);

        }
    }
}
