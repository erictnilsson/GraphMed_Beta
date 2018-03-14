using GraphMed_Beta.CypherHandling;
using GraphMed_Beta.FileHandling;
using System;
using System.Threading;

namespace GraphMed_Beta
{
    class Program
    {
        static void Main(string[] args)
        {
            bool running = true;
            Console.WriteLine("GraphMed Client version 1.0");
            string cmd = "-login";
            while (running)
            {
                if (args.Length == 0)
                {
                    var input = Console.ReadLine();
                    var command = input.Split(new char[] { ' ' }, 2);

                    if (command.Length == 2)
                        args = command[1].Replace("\"", "").Split('-');

                    cmd = command[0].ToLower().Trim(new char[] { ' ', '"' });
                }


                foreach (var arg in args)
                    arg.Trim();

                try
                {
                    switch (cmd)
                    {
                        case "-search":
                            if (IsConnected())
                                if (args.Length == 5)
                                    Search(searchword: args[0], relatives: args[1], limit: args[2], acceptability: args[3], langCode: args[4]);
                                else
                                    Console.WriteLine("Enter your search like: \"-Search searchTerm-relatives-limit-acceptability-languageCode\"\n" +
                                        "See -help for more information. \n");
                            break;

                        case "-login":
                            if (args.Length == 3)
                                Login(user: args[0], pass: args[1], uri: args[2]);
                            else
                                Console.WriteLine("Enter your username, password, and the uri you want to connect to like: \"-Login username-password-uri\" \n" +
                                    "See -help for more information. \n");
                            break;
                        case "-logout":
                            Logout();
                            break;
                        case "-install":
                            if (args.Length == 3)
                                Install(import: args[0], target: args[1], version: args[2]);
                            else if (args.Length == 4)
                                Install(import: args[0], target: args[1], version: args[2], commit: Int32.Parse(args[4]));
                            else
                                Console.WriteLine("Enter the import- and target-folder,the version, and optionally the commit-size like: \"-Install import-target-version-(commit)\"\n" +
                                    "See -help for more information. \n");
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

        private static void Logout()
        {
            Console.WriteLine("Logged out \"" + CurrentConfig.Instance.GraphDBUser + "\" from \"" + CurrentConfig.Instance.GraphDBUri + "\".");

            CurrentConfig.Instance.GraphDBUser = "";
            CurrentConfig.Instance.GraphDBPassword = "";
            CurrentConfig.Instance.GraphDBUri = "";
        }

        private static bool IsConnected()
        {
            if (Cypher.Get().Connection != null)
            {
                return true;
            }
            else
            {
                Console.WriteLine("Please login to the Neo4j database using \"-Login username-password-uri\". ");
                return false;
            }
        }

        private static void Search(string searchword, string relatives, string limit, string acceptability, string langCode)
        {
            var search = Cypher.Get().Nodes(searchTerm: searchword.Trim(), relatives: relatives.Trim(), limit: limit.Trim(), acceptability: acceptability.Trim(), langCode: langCode.Trim());

            if (search.Id == null || search.Id == "" || search.Term == null || search.Term == "")
                Console.WriteLine("\n" + "No search result.");
            else
            {
                Console.WriteLine("\nConceptId: " + search.Id);
                Console.WriteLine("Term: " + search.Term + "\n");

                foreach (var s in search.Results)
                {
                    Console.WriteLine("ConceptId: " + s.ConceptId);
                    Console.WriteLine("Term: " + s.Term);
                }
            }
            Console.Write("\n");
        }

        private static void Exit()
        {
            Environment.Exit(0);
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
            if (CurrentConfig.Instance.GraphDBUser != null || CurrentConfig.Instance.GraphDBPassword != null || CurrentConfig.Instance.GraphDBUri != null)
            {
                Console.WriteLine("Already logged in as \"" + CurrentConfig.Instance.GraphDBUser + "\" on " + CurrentConfig.Instance.GraphDBUri);
            }
            else
            {
                CurrentConfig.Instance.GraphDBUser = user.Trim();
                CurrentConfig.Instance.GraphDBPassword = pass.Trim();
                CurrentConfig.Instance.GraphDBUri = uri.Trim();

                if (Cypher.Get().Connection.IsConnected)
                    Console.WriteLine("Logged in as \"" + CurrentConfig.Instance.GraphDBUser + "\" on \"" + CurrentConfig.Instance.GraphDBUri + "\".");
                else
                {
                    Console.WriteLine("Failed to login as \"" + CurrentConfig.Instance.GraphDBUser + "\" on \"" + CurrentConfig.Instance.GraphDBUri + "\".");
                    CurrentConfig.Instance.GraphDBUser = null;
                    CurrentConfig.Instance.GraphDBPassword = null;
                    CurrentConfig.Instance.GraphDBUri = null;
                }

            }

        }
        private static void DeleteAll()
        {
            Cypher.Delete().Everything();
        }

        private static void Help()
        {
            Console.WriteLine("Please take a few minutes to read the README file.");
            Console.WriteLine("-Login [Username]-[Password]-[Database Uri]");
            Console.WriteLine("-Install [Snomed folder]-[Your database folder]-[Snomed version]");
            Console.WriteLine("-Search [Term]-[Relatives]-[Limit]-[Acceptability]-[Language]");
            Console.WriteLine("Delete 'Deletes entire database'");
            Console.WriteLine("-Help");
            Console.WriteLine("-Exit 'Exit the application'");
        }

        private static void Install(string @import, string @target, string @version, int? commit = 20_000)
        {
            Console.WriteLine("Preparing to install the database... \n" +
                "Do not exit the program until the installation is complete. \n");
            try
            {
                // move the database files into the import directory in neo4j
                Console.WriteLine("Moving the database-files into the import directory");
                MoveFiles(import: import, target: target, version: version);
                Console.WriteLine("All files moved successfully \n");

                // validate all the csv-files, correcting the use of quotation marks
                Console.WriteLine("Validating the database-files");
                FileHandler.ValidateCSVFiles();
                Console.WriteLine("All files validated successfully \n");

                Thread.Sleep(1000);

                // loading the Concepts into Neo4j
                Console.WriteLine("Loading the Concepts into Neo4j");
                Cypher.Load(commit: commit).Concepts();
                Console.WriteLine("All Concepts loaded succesfully \n");

                Thread.Sleep(3000);

                // loading the Descriptions into Neo4j
                Console.WriteLine("Loading the Descriptions into Neo4j");
                Cypher.Load(commit: commit).Descriptions();
                Console.WriteLine("All descriptions loaded successfully \n");

                Thread.Sleep(1000);

                // parsing the Relationships
                Console.WriteLine("Parsing the \"Relationship\"-files");
                FileHandler.SplitCSV("Relationship", "typeId");
                Console.WriteLine("All files parsed successfully \n");

                Thread.Sleep(3000);

                // loading the Relationships
                Console.WriteLine("Loading the Relationships into Neo4j");
                Cypher.Load(commit: commit).Relationships();
                Console.WriteLine("All Relationships loaded successfully \n");

                Thread.Sleep(1000);

                // parse the Refsets
                Console.WriteLine("Parsing the \"Refset\"-file");
                FileHandler.SplitCSV("Refset", "acceptabilityId");
                Console.WriteLine("All files parsed successfully \n");

                Thread.Sleep(5000);

                // loading the Refsets
                Console.WriteLine("Loading the Refsets into Neo4j");
                Cypher.Load().Refset();
                Console.WriteLine("All Refsets loaded successfully \n");

                Console.WriteLine("The database was installed successfully! \n");
            }
            catch (Exception e)
            {
                Console.WriteLine("Oops! It seems like something went wrong... Error at " + e.Source + ": \n" + e.Message + "\n" +
                    "Aborting installation...");
            }
        }
    }
}
