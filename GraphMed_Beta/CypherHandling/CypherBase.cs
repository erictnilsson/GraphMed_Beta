using GraphMed_Beta.ConnectionHandling;
using Neo4jClient;

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
        protected GraphClient Connection { get; set; }

        public CypherBase()
        {
            this.Connection = new Connection(timeout: 10D).Connect(); 
        }

        public CypherBase(int? limit)
        {
            this.Limit = limit;
            this.Connection = new Connection(timeout: 10D).Connect(); 
        }
    }
}
