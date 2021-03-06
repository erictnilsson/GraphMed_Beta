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
        public static string User { get; private set; }
        public static string Pass { get; private set; }
        public static string Uri { get; private set; }

        public static HttpClient HttpClient { get; private set; }

        /// <summary>
        /// Connection to the Neo4j Client with a default timeout set to 1 minute. 
        /// </summary>
        public Connection()
        {
            User = CurrentConfig.Instance.GraphDBUser;
            Pass = CurrentConfig.Instance.GraphDBPassword;
            Uri = CurrentConfig.Instance.GraphDBUri;

            HttpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(1D) };
        }

        /// <summary>
        /// Connection to the Neo4j Client with a custom timeout. 
        /// </summary>
        /// <param name="timeout"></param>
        public Connection(double timeout)
        {
            User = CurrentConfig.Instance.GraphDBUser;
            Pass = CurrentConfig.Instance.GraphDBPassword;
            Uri = CurrentConfig.Instance.GraphDBUri;

            HttpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(timeout) };
        }

        /// <summary>
        /// Connects to the Neo4j Client. 
        /// </summary>
        /// <returns></returns>
        public GraphClient Connect()
        {
            GraphClient client = null;
            try
            {
                if (Uri != null && User != null && Pass != null && HttpClient != null)
                {
                    client = new GraphClient(new Uri(Uri), new HttpClientWrapper(User, Pass, HttpClient));
                    client.Connect();
                }
            }
            catch (HttpRequestException)
            {
                Dispose();
            }
            catch (Exception e)
            {
                Dispose();
            }

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
