using GraphMed_Beta.ConnectionHandling;
using Neo4jClient;

namespace GraphMed_Beta.CypherHandling
{
/// <summary>
/// USED AS THE BASE FOR ALL CYPHERS. 
/// HOLDS A NULLABE 'LIMIT', LIMITING THE NUMBER OF RESULTS A RETURNABLE CYPHER HAS. 
/// HOLDS A CONNECTION TO THE NEO4J CLIENT
/// </summary>
    abstract class CypherBase
    {
        protected int? Limit { get; set; }
        protected GraphClient Connection { get; set; }

        public CypherBase()
        {
            this.Connection = new Connection().Connect(); 
        }

        public CypherBase(int? limit)
        {
            this.Limit = limit;
            this.Connection = new Connection().Connect(); 
        }
    }
}
