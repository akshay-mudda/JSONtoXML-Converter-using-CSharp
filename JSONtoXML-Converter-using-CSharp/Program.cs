using System;
using System.IO;
using System.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml;

namespace JSONtoXML_Converter_using_CSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Read source and destination paths from app.config
            string sourcePath = ConfigurationManager.AppSettings["SourcePath"];
            string destinationPath = ConfigurationManager.AppSettings["DestinationPath"];

            // Check if destination folder exists, create if not
            if (!Directory.Exists(destinationPath))
            {
                Directory.CreateDirectory(destinationPath);
            }

            // Get all JSON files in the source folder
            string[] jsonFiles = Directory.GetFiles(sourcePath, "*.json");

            if (jsonFiles.Length == 0)
            {
                Console.WriteLine("No JSON files found in the source folder.");
                return;
            }

            foreach (var jsonFilePath in jsonFiles)
            {
                try
                {
                    // Get file name without extension
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(jsonFilePath);
                    string xmlFilePath = Path.Combine(destinationPath, fileNameWithoutExtension + ".xml");

                    // Check if the XML already exists
                    if (File.Exists(xmlFilePath))
                    {
                        Console.WriteLine($"XML already exists for {fileNameWithoutExtension}. Skipping file.");
                        continue;
                    }

                    // Convert JSON file to XML
                    ConvertJsonToXml(jsonFilePath, xmlFilePath);

                    // After conversion, delete the source JSON file
                    File.Delete(jsonFilePath);

                    Console.WriteLine($"Successfully converted {fileNameWithoutExtension}.json to XML.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error converting {jsonFilePath}: {ex.Message}");
                }
            }
        }

        // Convert JSON file to XML using Newtonsoft.Json
        static void ConvertJsonToXml(string jsonFilePath, string xmlFilePath)
        {
            // Read JSON content from file
            string jsonText = File.ReadAllText(jsonFilePath);

            // Parse JSON into a JObject
            var jsonObject = JObject.Parse(jsonText);

            // Convert JObject to XML
            var xmlDoc = JsonConvert.DeserializeXmlNode(jsonObject.ToString(), "Root");

            // Write XML to the destination file
            xmlDoc.Save(xmlFilePath);
        }
    }
}