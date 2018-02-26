﻿using System;
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
/// Used to connect to the Neo4j client enabling the use of cyphers. 
/// </summary>
    class Connection : IDisposable
    {
        private static string User { get; set; }
        private static string Pass { get; set; }
        private static string Uri { get; set; }

        private static HttpClient HttpClient { get; set; }

        /// <summary>
        /// Connection to the Neo4j Client with a default timeout set to 1 minute. 
        /// </summary>
        public Connection()
        {
            User = ConfigurationManager.AppSettings["GraphDBUser"];
            Pass = ConfigurationManager.AppSettings["GraphDBPassword"];
            Uri = ConfigurationManager.AppSettings["ClientUri"];

            HttpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(1D) }; 
        }

        /// <summary>
        /// Connection to the Neo4j Client with a custom timeout. 
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
        /// Connects to the Neo4j Client. 
        /// </summary>
        /// <returns></returns>
        public GraphClient Connect()
        {
            var client = new GraphClient(new Uri(Uri), new HttpClientWrapper(User, Pass, HttpClient));
            client.Connect();
            return client; 
        }

        /// <summary>
        /// Disposes the Neo4j Client. 
        /// </summary>
        public void Dispose()
        {
            HttpClient?.Dispose();
            GC.SuppressFinalize(this); 
        }
    }
}
