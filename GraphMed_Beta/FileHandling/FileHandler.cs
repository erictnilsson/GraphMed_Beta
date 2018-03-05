using GraphMed_Beta.CypherHandling;
using GraphMed_Beta.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace GraphMed_Beta.FileHandling
{
    /// <summary>
    /// Used to handle the .CSV-files that makes the database. 
    /// </summary>
    public static class FileHandler
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
                Console.WriteLine("Validation check failed: \n" + e.Message);
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

        public static void SplitRelationshipFile()
        {
            SplitCSV("fullRelationship", "typeId", "Relationship");
        }

        public static void SplitRefsetFile()
        {
            SplitCSV("fullRefset", "acceptabilityId", "Refset");
        }


        /// <summary>
        /// Validates the targeted .txt-file; correcting the use of quotation marks so it can be loaded into Neo4j. 
        /// </summary>
        /// <param name="filepath"></param>
        public static void ValidateCSV(string file)
        {
            var filepath = path + ConfigurationManager.AppSettings[file];
            if (!IsValidated(file) && File.Exists(filepath))
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
                    Console.WriteLine("Validation failed: \n" + e.Message);
                }
            }
            else
            {
                Console.WriteLine("Validation failed. \n" +
                    "The filepath \"" + filepath + "\" does not seem to be valid or exist.\n" +
                    "Make sure that the file is a \".txt\"-file delimited by tabs.");
            }
        }
        /// <summary>
        /// Splits up the .CSV-file into smaller ones, divided by the specified identifier. 
        /// </summary>
        /// <param name="configKey"></param>
        /// <param name="identifier"></param>
        public static void SplitCSV(string configKey, string identifier, string type)
        {
            var filepath = path + ConfigurationManager.AppSettings[configKey];
            if (File.Exists(filepath))
            {
                if (identifier == null || type == null)
                {
                    Console.WriteLine("Either the identifier or type is null. Please make sure that both fields are valid strings.");
                }

                // Dictionary that holds the text as Value and the identifier as the key
                Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
                var text = File.ReadAllLines(filepath);
                var row = new string[0];
                var index = 0;
                for (int i = 0; i < text.Length; i++)
                {
                    row = text[i].Split('\t');

                    if (i == 0)
                        index = Array.IndexOf(row, identifier);
                    if (index == -1)
                    {
                        Console.WriteLine("The identifier \"" + identifier + "\" does not seem to exist withing the given context and therefore the file could not be parsed. \n" +
                            "Please make sure that the identifier is a valid column header in the .txt-file \"" + filepath + "\".");
                        break;
                    }
                    else
                    {
                        if (!dict.ContainsKey(row[index]))
                            dict.Add(row[index], new List<string> { text[i] });
                        else
                            dict[row[index]].Add(text[i]);
                    }
                }
                if (index != -1)
                {
                    var headers = dict.ElementAt(0).Value.FirstOrDefault();
                    for (int i = 1; i < dict.Count; i++)
                    {
                        dict.ElementAt(i).Value.Insert(0, headers);
                        var content = dict.ElementAt(i).Value.ToArray();
                        var getCypher = Cypher.Get(null);
                        if (getCypher.Connection.IsConnected)
                        {
                            string fileName = getCypher.Term(dict.ElementAt(i).Key).Replace("-", "").Replace(" ", "_").ToUpper();
                            File.WriteAllLines(path + "\\Neo4j\\default.graphdb\\import\\parsed" + type + "-" + fileName + ".txt", content);
                        }
                        else
                        {
                            Console.WriteLine("Make sure that Neo4j is connected and running before splitting any \".txt\"-files.");
                            break;
                        }

                    }
                }
            }
            else
            {
                Console.WriteLine("Validation failed. \n" +
                   "The filepath \"" + filepath + "\" does not seem to be valid or exist.\n" +
                   "Make sure that the file is a \".txt\"-file delimited by tabs.");
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
