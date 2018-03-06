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
        /// Validates all of the .CSV-files needed to build the database. 
        /// </summary>
        public static void ValidateCSVFiles()
        {
            var csvFiles = new string[] {
                CurrentConfig.Instance.FullConcept,
                CurrentConfig.Instance.FullDescription,
                CurrentConfig.Instance.FullRelationship,
                CurrentConfig.Instance.FullRefset
            };

            foreach (var csv in csvFiles)
            {
                ValidateCSV(csv);
            }
        }

        public static void SplitRelationshipFile()
        {
            SplitCSV("Relationship", "typeId");
        }

        public static void SplitRefsetFile()
        {
            SplitCSV("Refset", "acceptabilityId");
        }


        /// <summary>
        /// Validates the targeted .txt-file; correcting the use of quotation marks so it can be loaded into Neo4j. 
        /// </summary>
        /// <param name="filepath"></param>
        public static void ValidateCSV(string file)
        {
            var filepath = path + ConfigurationManager.AppSettings[file];
            if (File.Exists(filepath))
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
                                //if the next character after the first quotation mark is also a quotation mark,
                                //we can assume that the file is already validated so break
                                if (!val[val.IndexOf("\"") + 2].Equals("\""))
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
        public static void SplitCSV(string configKey, string identifier)
        {
            var filepath = configKey;

            if (configKey.Equals("Refset"))
                filepath = CurrentConfig.Instance.FullRefset;
            else if (configKey.Equals("Relationship"))
                filepath = CurrentConfig.Instance.FullRelationship;

            if (File.Exists(filepath))
            {
                if (identifier == null)
                {
                    Console.WriteLine("The identifier seems to be null. Please make sure that the field is a valid strings.");
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
                            File.WriteAllLines(CurrentConfig.Instance.TargetPath + "\\import\\parsed" + configKey + "-" + fileName + ".txt", content);
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
            string[] filePaths = Directory.GetFiles(CurrentConfig.Instance.TargetPath + "\\import", "*" + searchPattern + "*");
            return filePaths;
        }

        /// <summary>
        /// Moves all necessary files from the :D drive to installation folder. 
        /// </summary>
        public static void MoveFiles()
        {
            List<string> pathList = new List<string>();
            string fileName = "";
            string destFile = "";

            pathList.Add(CurrentConfig.Instance.Descriptions);
            pathList.Add(CurrentConfig.Instance.Concepts);
            pathList.Add(CurrentConfig.Instance.Relationships);
            pathList.Add(CurrentConfig.Instance.Refset);

            string targetPath = CurrentConfig.Instance.TargetPath + "\\import";

            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }
            for (int i = 0; i < pathList.Count; i++)
            {
                //If one file path is wrong, delete all previous files.
                if (!File.Exists(pathList.ElementAt(i)))
                {
                    Console.WriteLine("The path: " + pathList.ElementAt(i) + " is not correct");
                    for (int j = 0; j < i; i++)
                        File.Delete(Path.Combine(targetPath, (Path.GetFileName(pathList.ElementAt(j)))));

                    break;
                }
                else
                {
                    fileName = Path.GetFileName(pathList.ElementAt(i));
                    CurrentConfig.Instance.destPath[i] = destFile = Path.Combine(targetPath, fileName);
                    File.Copy(pathList.ElementAt(i), destFile, true);
                }
            }
            SetFullPath(CurrentConfig.Instance.destPath);
            Console.WriteLine("Succesfully moved files!");
        }
        public static void SetFullPath(string[] paths)
        {
            CurrentConfig.Instance.FullDescription = @paths[0];
            CurrentConfig.Instance.FullConcept = @paths[1];
            CurrentConfig.Instance.FullRelationship = @paths[2];
            CurrentConfig.Instance.FullRefset = @paths[3];
        }
    }
}
