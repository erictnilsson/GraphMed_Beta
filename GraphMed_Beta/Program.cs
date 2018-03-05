using GraphMed_Beta.CypherHandling;
using GraphMed_Beta.FileHandling;
using System;
using System.Diagnostics;
using System.Threading;

namespace GraphMed_Beta
{
    class Program
    {
        static void Main(string[] args)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Init(); 
            stopwatch.Stop(); 
            Console.WriteLine("Process completed in " + stopwatch.ElapsedMilliseconds + "ms");
            Console.Read();
        }

        private static void Init()
        {
            var a = Cypher.Get().Nodes("Duckbill flathead", 'f', 1, "pref", "GB");
            Console.WriteLine(a.Id + ", " + a.Term + ", " + a.Results); 
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
            FileHandler.SplitCSV("fullRelationships", "typeId", "Relationship");
            Console.WriteLine("The Relationship CSV-file was successfully parsed 100%");
            Thread.Sleep(3000);

            Console.WriteLine("Trying to load all relationships...");
            Cypher.Load().Relationships();
            Console.WriteLine("All relationships was successfully loaded 100%");
            Thread.Sleep(1000);

            Console.WriteLine("Trying to parse the Refset CSV-file...");
            FileHandler.SplitCSV("fullRefset", "acceptabilityId", "Refset");
            Console.WriteLine("The Refset CSV-file successfully was parsed 100%");
            Thread.Sleep(3000);

            Console.WriteLine("Trying to load all refset relationships...");
            Cypher.Load().Refset();
            Console.WriteLine("All refsets was successfully loaded 100%");
        }
    }
}
