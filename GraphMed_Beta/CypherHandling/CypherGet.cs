using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo4jClient;

namespace GraphMed_Beta.CypherHandling
{
    /// <summary>
    /// USED TO WRITE 'GET' CYPHERS
    /// </summary>
    class CypherGet : CypherBase
    {
        public CypherGet() : base() { }

        public CypherGet(int? limit) : base(limit) { }

        /// <summary>
        /// GET THE SYNONYM TERM OF THE SPECIFIED CONCEPT. 
        /// USED PRIMARILY TO RELATIONSHIP TERM WHEN CONSTRUCTING RELATIONSHIPS
        /// </summary>
        /// <param name="conceptId"></param>
        /// <returns></returns>
        public string Term(string conceptId)
        {
            try
            {
                return Connection.Cypher
                          .Match("(d:Description)-[:REFERS_TO]->(c:Concept)")
                          .Where("d.TypeId = '900000000000013009'")
                          .AndWhere("c.Id = '" + conceptId + "'")
                          .Return<string>("d.Term")
                          .Results.First();
            }
            catch (NeoException)
            {
                throw;
            }
            finally
            {
                Connection.Dispose();
            }
        }
    }
}
