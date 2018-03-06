using GraphMed_Beta.CypherHandling;
using GraphMed_Beta.FileHandling;
using Neo4jClient;
using System;
using System.Diagnostics;
using System.Threading;

namespace GraphMed_Beta
{
    class Program
    {
        static void Main(string[] args)
        {
            bool running = true;

            while (running)
            {
                args = Console.ReadLine().Split(' ');
                string cmd = args[0].Substring(0, 3).ToUpper();
                switch (cmd)
                {
                    case "-S=":
                        var search = Cypher.Get().Nodes(searchTerm: args[1], relatives: args[2], limit: args[3], acceptability: args[4], langCode: args[5]);
                        Console.WriteLine("Id: " + search.Id);
                        Console.WriteLine("Term: " + search.Term);
                        foreach (var s in search.Results)
                        {
                            Console.WriteLine("ConceptId: " + s.ConceptId);
                            Console.WriteLine("Term: " + s.Term);
                        }

                        break;
                    case "-L=":
                        string user = args[0].Remove(0, 3);
                        CurrentConfig.Instance.GraphDBUser = user;
                        CurrentConfig.Instance.GraphDBPassword = args[1];
                        CurrentConfig.Instance.GraphDBUri = args[2];

                        Console.WriteLine("L!");
                        break;
                    case "-I=":
                        string path = args[0].Remove(0, 3);
                        CurrentConfig.Instance.SnomedVersion = @args[2];
                        CurrentConfig.Instance.SnomedImportPath = @path;
                        CurrentConfig.Instance.TargetPath = @args[1];
                        FileHandler.MoveFiles();

                        Install();
                        Console.WriteLine("I!");
                        break;
                    case "-Q=":
                        running = false;
                        break;
                }
                args = new string[0];
            }
        }

        private static void Init()
        {
            //var a = Cypher.Get().Nodes("Duckbill flathead", 'f', 1, "pref", "GB");
            //Console.WriteLine(a.Id + ", " + a.Term + ", " + a.Results);
        }

        private static void Install()
        {
            Console.WriteLine("Trying to validate all CSV-files...");
            FileHandler.ValidateCSVFiles();
            Console.WriteLine("All files was successfully validatet 100%");
            Thread.Sleep(1000);

            Console.WriteLine("Trying to load all concepts...");
            Cypher.Load().Concepts();
            Console.WriteLine("All concepts was succesfully loaded 100%");
            Thread.Sleep(3000);

            Console.WriteLine("Trying to load all descriptions...");
            Cypher.Load().Descriptions();
            Console.WriteLine("All descriptions was successfully loaded 100%");
            Thread.Sleep(1000);

            Console.WriteLine("Trying to parse the Relationship CSV-file...");
            FileHandler.SplitCSV("Relationship", "typeId");
            Console.WriteLine("The Relationship CSV-file was successfully parsed 100%");
            Thread.Sleep(3000);

            Console.WriteLine("Trying to load all relationships...");
            Cypher.Load().Relationships();
            Console.WriteLine("All relationships was successfully loaded 100%");
            Thread.Sleep(1000);

            Console.WriteLine("Trying to parse the Refset CSV-file...");
            FileHandler.SplitCSV("Refset", "acceptabilityId");
            Console.WriteLine("The Refset CSV-file successfully was parsed 100%");
            Thread.Sleep(3000);

            Console.WriteLine("Trying to load all refset relationships...");
            Cypher.Load().Refset();
            Console.WriteLine("All refsets was successfully loaded 100%");
        }
    }
}
