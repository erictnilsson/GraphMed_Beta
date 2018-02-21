using GraphMed_Beta.CypherHandling;
using GraphMed_Beta.FileHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using GraphMed_Beta.Model.Nodes;
using System.Diagnostics;

namespace GraphMed_Beta
{
    class Program
    {
        static void Main(string[] args)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            //  ----------------------------
            FileHandler.SplitCSV(configKey: "fullRefset", identifier: "acceptabilityId"); 
            //  ----------------------------

            stopwatch.Stop(); 
            Console.WriteLine("Process completed in " + stopwatch.ElapsedMilliseconds + "ms");
            
        }
    }
}
