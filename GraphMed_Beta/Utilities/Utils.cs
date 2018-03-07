using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GraphMed_Beta.Utilities
{
    /// <summary>
    /// MISCELLANEOUS FUNCTIONS
    /// </summary>
    public static class Utils
    {

        /// <summary>
        /// GETS THE ATTRIBUTES NEEDED TO CONSTRUCT AN ETITY IN NEO4J
        /// USED WHEN LOADING NODES OR RELATIONSHIPS
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public static string GetBuildString<T>(string identifier)
        {
            var buildString = "";
            var node = typeof(T);
            var nodeProps = node.GetProperties().Length;

            for (int i = 0; i < nodeProps; i++)
            {
                var propName = node.GetProperties()[i].Name;
                var propString = "";

                propString = propName + ": " + identifier + "." + propName.First().ToString().ToLower() + propName.Substring(1);
                if (i < nodeProps - 1)
                    propString += ", ";

                buildString += propString;
            }
            return buildString;
        }

        /// <summary>
        /// FINDS ALL INDEXES OF A SPECIFIED CHARACTER
        /// </summary>
        /// <param name="source"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static int[] FindAllIndexesOf(string source, string match)
        {
            var indexes = new List<int>();
            int index = 0;

            while ((index = source.IndexOf(match, index)) != -1)
            {
                indexes.Add(index++);
            }
            return indexes.ToArray();
        }
    }
}
