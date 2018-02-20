using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration; 
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Neo4jClient;

namespace GraphMed_Beta.ConnectionHandling
{
/// <summary>
/// HANDLES THE CONNECTION TO THE NEO4J CLIENT
/// </summary>
    class Connection : IDisposable
    {
        private static string User { get; set; }
        private static string Pass { get; set; }
        private static string Uri { get; set; }

        private static HttpClient HttpClient { get; set; }

        /// <summary>
        /// CONSTRUCTOR WITH DEFAULT TIMEOUT SET TO 1 MINUTE
        /// </summary>
        public Connection()
        {
            User = ConfigurationManager.AppSettings["GraphDBUser"];
            Pass = ConfigurationManager.AppSettings["GraphDBPassword"];
            Uri = ConfigurationManager.AppSettings["ClientUri"];

            HttpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(1D) }; 
        }

        /// <summary>
        /// CONSTRUCTOR WITH CUSTOM TIMEOUT
        /// </summary>
        /// <param name="timeout"></param>
        public Connection(double timeout)
        {
            User = ConfigurationManager.AppSettings["GraphDBUser"];
            Pass = ConfigurationManager.AppSettings["GraphDBPassword"];
            Uri = ConfigurationManager.AppSettings["ClientUri"];

            HttpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(timeout)};
        }

        /// <summary>
        /// CONNECTS TO THE CLIENT 
        /// </summary>
        /// <returns></returns>
        public GraphClient Connect()
        {
            var client = new GraphClient(new Uri(Uri), new HttpClientWrapper(User, Pass, HttpClient));
            client.Connect();
            return client; 
        }

        /// <summary>
        /// DISPOSES THE CLIENT
        /// </summary>
        public void Dispose()
        {
            HttpClient?.Dispose();
            GC.SuppressFinalize(this); 
        }
    }
}
