using GraphMed_Beta.CypherHandling;
using GraphMed_Beta.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GraphMed_Beta.FileHandling
{
    /// <summary>
    /// Used to handle the .CSV-files that makes the database. 
    /// </summary>
    static class FileHandler
    {
        private static string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        /// <summary>
        /// Checks to see whether the file already is validated or not. 
        /// Returns true if validated, false if not.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool IsValidated(string file)
        {
            // gets the current directory and combines the path with Config.txt in the project folder
            // then takes the second line of the file (Config.txt), splitting the values by ','
            var valids = File.ReadAllLines(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, "Config.txt"))[1].Split(',');
            try
            {
                foreach (var v in valids)
                    if (v.Trim().Equals(file))
                        return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Validation check failed. Make sure that the file exists. \n" + e.Message);
            }
            return false;
        }


        /// <summary>
        /// Validates all of the .CSV-files needed to build the database. 
        /// </summary>
        public static void ValidateCSVFiles()
        {
            var csvFiles = new string[] { "fullConcepts", "fullDescriptions", "fullRelationships", "fullRefset" };

            foreach (var csv in csvFiles)
            {
                ValidateCSV(csv);
            }
        }


        /// <summary>
        /// Validates the targeted .CSV-file; correcting the use of quotation marks so it can be loaded into Neo4j. 
        /// </summary>
        /// <param name="filepath"></param>
        public static void ValidateCSV(string file)
        {
            var filepath = path + ConfigurationManager.AppSettings[file];
            if (!IsValidated(file))
            {
                try
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
                    Utils.WriteToConfig(file);
                    File.WriteAllLines(filepath, text);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Validation failed. Make sure that the file exists. \n" + e.Message);
                }
            }
        }
        /// <summary>
        /// Splits up the .CSV-file into smaller ones, divided by the TypeId. 
        /// </summary>
        /// <param name="configKey"></param>
        /// <param name="identifier"></param>
        public static void SplitCSV(string configKey, string identifier, string type)
        {
            var filepath = path + ConfigurationManager.AppSettings[configKey];
            // Dictionary that holds the text as Value and the identifier as the key
            Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
            if (File.Exists(filepath))
            {
                var text = File.ReadAllLines(filepath);
                var row = new string[0];
                var index = 0;
                for (int i = 0; i < text.Length; i++)
                {
                    row = text[i].Split('\t');

                    if (i == 0)
                        index = Array.IndexOf(row, identifier);

                    if (!dict.ContainsKey(row[index]))
                        dict.Add(row[index], new List<string> { text[i] });
                    else
                        dict[row[index]].Add(text[i]);
                }
                var headers = dict.ElementAt(0).Value.FirstOrDefault();
                for (int i = 1; i < dict.Count; i++)
                {
                    dict.ElementAt(i).Value.Insert(0, headers);
                    var content = dict.ElementAt(i).Value.ToArray();
                    string fileName = Cypher.Get(null).Term(dict.ElementAt(i).Key).Replace("-", "").Replace(" ", "_").ToUpper();

                    File.WriteAllLines(path + "\\Neo4j\\default.graphdb\\import\\parsed" + type + "-" + fileName + ".txt", content); ;
                }
            }
        }

        /// <summary>
        /// Finds all of the filenames in with the specified search pattern in the import directory of Neo4j. 
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
