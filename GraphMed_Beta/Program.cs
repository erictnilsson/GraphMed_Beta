﻿using GraphMed_Beta.CypherHandling;
using GraphMed_Beta.FileHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using GraphMed_Beta.Model.Nodes;
using System.Diagnostics;
using System.Threading;
using GraphMed_Beta.Utilities;

namespace GraphMed_Beta
{
    class Program
    {
        static void Main(string[] args)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var a = Cypher.Get(null).Get("112009", 'f', 3, "acc", "GB");
            foreach (var b in a)
                Console.WriteLine(b + "\n");

            
            //Install(); 
            stopwatch.Stop();
            Console.WriteLine("Process completed in " + stopwatch.ElapsedMilliseconds + "ms");
            Console.Read();
        }

        private static void Init()
        {

        }

        private static void Install()
        {
            Console.WriteLine("Trying to validate all CSV-files...");
            FileHandler.ValidateCSVFiles();
            Console.WriteLine("All files was successfully validatet 100%");
            Thread.Sleep(1000);

            Console.WriteLine("Trying to load all concepts...");
            Cypher.Load(null, 20000).Concepts(true);
            Console.WriteLine("All concepts was succesfully loaded 100%");
            Thread.Sleep(3000);

            Console.WriteLine("Trying to load all descriptions...");
            Cypher.Load(null, 10000).Descriptions(true, true, true);
            Console.WriteLine("All descriptions was successfully loaded 100%");
            Thread.Sleep(1000);


            Console.WriteLine("Trying to parse the Relationship CSV-file...");
            FileHandler.SplitCSV("fullRelationships", "typeId", "Relationship");
            Console.WriteLine("The Relationship CSV-file was successfully parsed 100%");
            Thread.Sleep(3000);

            Console.WriteLine("Trying to load all relationships...");
            Cypher.Load(null, 20000).Relationships();
            Console.WriteLine("All relationships was successfully loaded 100%");
            Thread.Sleep(1000);


            Console.WriteLine("Trying to parse the Refset CSV-file...");
            FileHandler.SplitCSV("fullRefset", "acceptabilityId", "Refset");
            Console.WriteLine("The Refset CSV-file successfully was parsed 100%");
            Thread.Sleep(3000);

            Console.WriteLine("Trying to load all refset relationships...");
            Cypher.Load(limit: null, commit: 20000).Refset();
            Console.WriteLine("All refsets was successfully loaded 100%");

        }
    }
}
