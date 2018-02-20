using GraphMed_Beta.CypherHandling;
using GraphMed_Beta.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphMed_Beta.FileHandling
{
    /// <summary>
    /// USED TO HANDLE THE CSV FILES. 
    /// </summary>
    static class FileHandler
    {
        private static string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        /// <summary>
        /// VALIDATES ALL OF THE CSV FILES NEEDED TO BUILD THE GRAPH DATABASE
        /// </summary>
        public static void ValidateCSVFiles()
        {
            var csvFiles = new string[] { "fullConcepts", "fullDescriptions", "fullRelationships" };

            foreach (var csv in csvFiles)
            {
                ValidateCSV(csv);
            }
        }


        /// <summary>
        /// VALIDATES THE SPECIFIED CSV-FILE, CORRECTING THE USE OF QUOTATION MARKS
        /// </summary>
        /// <param name="filepath"></param>
        public static void ValidateCSV(string configKey)
        {
            var filepath = path + ConfigurationManager.AppSettings[configKey];

            if (File.Exists(filepath))
            {
                var text = File.ReadAllLines(filepath);
                var row = new string[0];

                for (int i = 0; i < text.Length; i++)
                {
                    var tmp = "";
                    row = text[i].Split('\t');
                    for (int j = 0; j < row.Length; j++)
                    {
                        var val = row[j];
                        if (val.Contains("\""))
                        {
                            var line = new StringBuilder(val);
                            var tick = 0;
                            foreach (var index in Utils.FindAllIndexesOf(val, "\""))
                            {
                                line.Insert(index + tick, "\"");
                                tick++;
                            }
                            row[j] = "\"" + line.ToString() + "\"";
                        }
                        tmp += row[j] + "\t";
                    }
                    text[i] = tmp;
                }
                File.WriteAllLines(filepath, text);
            }
            else
            {
                Console.WriteLine("File not found");
            }
        }

        /// <summary>
        /// SPLITS UP THE RELATIONSHIP CSV-FILE, ADDING THE TYPEID AS THE KEY AND THE TEXT ROW AS THE VALUE IN A DICTIONARY 
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static void SplitRelationshipCSV(string configKey)
        {
            var filepath = path + ConfigurationManager.AppSettings[configKey];
            Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
            if (File.Exists(filepath))
            {
                var text = File.ReadAllLines(filepath);
                var row = new string[0];
                for (int i = 0; i < text.Length; i++)
                {
                    row = text[i].Split('\t');
                    if (!dict.ContainsKey(row[7]))
                        dict.Add(row[7], new List<string> { text[i] });
                    else
                        dict[row[7]].Add(text[i]);
                }
                var headers = dict.ElementAt(0).Value.FirstOrDefault();
                for (int i = 1; i < dict.Count; i++)
                {
                    dict.ElementAt(i).Value.Insert(0, headers);
                    var content = dict.ElementAt(i).Value.ToArray();
                    string fileName = Cypher.Get(null).Term(dict.ElementAt(i).Key);

                    File.WriteAllLines(filepath.Substring(filepath.LastIndexOf("/"), filepath.Length - filepath.LastIndexOf("/")) + fileName + ".txt", content);
                }
            }
            else
            {
                Console.WriteLine("File not found");
            }
        }

        /// <summary>
        /// FINDS ALL OF THE FILENAMES WITH THE SPECIFIED SEARCH PATTERN IN THE IMPORT DIRECTORY
        /// </summary>
        /// <param name="searchPattern"></param>
        /// <returns></returns>
        public static string[] GetFiles(string searchPattern)
        {
            string[] filePaths = Directory.GetFiles(path + "\\Neo4j\\default.graphdb\\import", "*" + searchPattern + "*");
            return filePaths;
        }
    }
}
