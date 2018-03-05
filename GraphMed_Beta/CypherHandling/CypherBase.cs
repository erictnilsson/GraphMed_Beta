using GraphMed_Beta.ConnectionHandling;
using Neo4jClient;
using Neo4jClient.Cypher;
using System;
using System.Collections.Generic;

namespace GraphMed_Beta.CypherHandling
{
    /// <summary>
    /// Used as the base for all cyphers. 
    /// Holds a nullable 'Limit', setting the number of results a returnable cypher should have. 
    /// Also holds a connection to the Neo4j Client. 
    /// </summary>
    abstract class CypherBase
    {
        protected int? Limit { get; set; }
        public GraphClient Connection { get; protected set; }

        public CypherBase()
        {
            this.Connection = new Connection(timeout: 10D).Connect();
        }

        public CypherBase(int? limit)
        {
            this.Limit = limit;
            this.Connection = new Connection(timeout: 10D).Connect();
        }

        public void ExecuteWithoutResults(ICypherFluentQuery cypher)
        {
            try
            {
                if (Connection.IsConnected)
                    cypher.ExecuteWithoutResults();
                else
                    Console.WriteLine("It seems like you don't have a connection with Neo4j... \n" +
                        "Make sure that an instance of Neo4j is connected and running before calling any cyphers.");
            }
            catch (NeoException e)
            {
                Console.WriteLine("There was an error when executing the cypher: \n" +
                    "\"" + e.Message + "\"");
            }
            finally
            {
                Connection.Dispose();
            }
        }

        public IEnumerable<T> ExecuteWithResults<T>(ICypherFluentQuery<T> cypher)
        {
            try
            {
                return cypher.Results; 
            } catch (NeoException)
            {
                throw; 
            } finally
            {
                Connection.Dispose(); 
            }
        }
    }
}
