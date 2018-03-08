using GraphMed_Beta.CypherHandling;
using GraphMed_Beta.FileHandling;
using GraphMed_Beta.Model.Nodes;
using Neo4jClient;
using System;
using System.Diagnostics;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace GraphMed_Beta
{
    class Program
    {
        static void Main(string[] args)
        {
            bool running = true;
            Console.WriteLine("GraphMed");
            while (running)
            {
                var input = Console.ReadLine();
                var command = input.Split('=');
                if (command.Length != 2)
                    Console.WriteLine("Wrong input. Please specify the command and the parameters in this fashion: \"-Command= param1-param2-param3\" \n" +
                        "See \"-Help=\" for more info.");
                else
                {
                    var cmd = command[0].ToLower();
                    args = command[1].Split('-');
                    try
                    {
                        switch (cmd)
                        {
                            case "-search":
                                if (IsConnected())
                                    if (args.Length == 5)
                                        Search(searchword: args[0], relatives: args[1], limit: args[2], acceptability: args[3], langCode: args[4]);
                                else
                                        Console.WriteLine("Enter your search in this fashion: \"-Search= searchTerm-relatives-limit-acceptability-languageCode\"");
                                break;

                            case "-login":
                                if (args.Length == 3)
                                    Login(user: args[0], pass: args[1], uri: args[2]);
                                else
                                    Console.WriteLine("Enter your username, password, and the uri you want to connect to in this fashion: \"-Login= username-password-uri\"");
                                break;

                            case "-install":
                                MoveFiles(import: @args[0], target: @args[1], version: @args[2]);
                                Install();
                                Console.WriteLine("Successfully loaded your Neo4j database.");
                                break;
                            case "-delete":
                                if (IsConnected())
                                    DeleteAll();
                                break;
                            case "-help":
                                Help();
                                break;
                            case "-exit":
                                Exit();
                                break;
                            default:
                                Help();
                                break;

                        }
                        args = new string[0];
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Invalid command. Reason: " + e.Message);
                        Help();
                    }
                }

            }
        }

        private static bool IsConnected()
        {
            if (Cypher.Get().Connection != null)
            {
                return true;
            }
            else
            {
                Console.WriteLine("Please login to the Neo4j database using \"-Login= username-password-uri\". ");
                return false;
            }
        }

        private static void Search(string searchword, string relatives, string limit, string acceptability, string langCode)
        {
            var search = Cypher.Get().Nodes(searchTerm: searchword.Trim(), relatives: relatives.Trim(), limit: limit.Trim(), acceptability: acceptability.Trim(), langCode: langCode.Trim());
            Console.WriteLine("\n----BASE----");
            Console.WriteLine("ConceptId: " + search.Id);
            Console.WriteLine("Term: " + search.Term);
            Console.WriteLine("----------------------" + "\n");
            foreach (var s in search.Results)
            {
                Console.WriteLine("ConceptId: " + s.ConceptId);
                Console.WriteLine("Term: " + s.Term);
            }
            Console.Write("\n");
        }

        private static void Exit()
        {
            // Environment.Exit(1);
        }

        private static void MoveFiles(string import, string target, string version)
        {
            CurrentConfig.Instance.SnomedVersion = version;
            CurrentConfig.Instance.SnomedImportPath = import;
            CurrentConfig.Instance.TargetPath = target;

            FileHandler.MoveFiles();
        }

        private static void Login(string user, string pass, string uri)
        {
            CurrentConfig.Instance.GraphDBUser = user.Trim();
            CurrentConfig.Instance.GraphDBPassword = pass.Trim();
            CurrentConfig.Instance.GraphDBUri = uri.Trim();
            if (Cypher.Get().Connection.IsConnected)
                Console.WriteLine("Logged in as \"" + CurrentConfig.Instance.GraphDBUser + "\" on \"" + CurrentConfig.Instance.GraphDBUri + "\".");
            else
                Console.WriteLine("Failed to login as \"" + CurrentConfig.Instance.GraphDBUser + "\" on \"" + CurrentConfig.Instance.GraphDBUri + "\".");
        }
        private static void DeleteAll()
        {
            Cypher.Delete().Alles();
        }

        private static void Help()
        {
            Console.WriteLine("Please take a few minutes to read the README file.");
            Console.WriteLine("-Login= [Username]-[Password]-[Database Uri]");
            Console.WriteLine("-Install= [Snomed folder]-[Your database folder]-[Snomed version]");
            Console.WriteLine("-Search= [Term]-[Relatives]-[Limit]-[Acceptability]-[Language]");
            Console.WriteLine("DeleteAll= 'Deletes entire database'");
            Console.WriteLine("-Help=");
            Console.WriteLine("-Exit= 'Exit the application'");
        }

        private static void Install()
        {
            Console.WriteLine("Trying to validate all CSV-files...");
            FileHandler.ValidateCSVFiles();
            Console.WriteLine("All files successfully validated 100%");
            Thread.Sleep(1000);

            Console.WriteLine("Trying to load all concepts...");
            Cypher.Load().Concepts();
            Console.WriteLine("All concepts succesfully loaded 100%");
            Thread.Sleep(3000);

            Console.WriteLine("Trying to load all descriptions...");
            Cypher.Load().Descriptions();
            Console.WriteLine("All descriptions successfully loaded 100%");
            Thread.Sleep(1000);

            Console.WriteLine("Trying to parse the Relationship CSV-file...");
            FileHandler.SplitCSV("Relationship", "typeId");
            Console.WriteLine("The Relationship CSV-file successfully parsed 100%");
            Thread.Sleep(3000);

            Console.WriteLine("Trying to load all relationships...");
            Cypher.Load().Relationships();
            Console.WriteLine("All relationships successfully loaded 100%");
            Thread.Sleep(1000);

            Console.WriteLine("Trying to parse the Refset CSV-file...");
            FileHandler.SplitCSV("Refset", "acceptabilityId");
            Console.WriteLine("The Refset CSV-file successfully parsed 100%");
            Thread.Sleep(3000);

            Console.WriteLine("Trying to load all refset relationships...");
            Cypher.Load().Refset();
            Console.WriteLine("All refsets successfully loaded 100%");
        }
    }
}
