using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo4jClient;

namespace GraphMed_Beta.CypherHandling
{
    /// <summary>
    /// Used to write 'Get' Cyphers. 
    /// </summary>
    class CypherGet : CypherBase
    {
        public CypherGet() : base() { }

        public CypherGet(int? limit) : base(limit) { }

        /// <summary>
        /// Gets the synonym term of the specified Concept. 
        /// Used primarily to get the relationship term when constructing relationships. 
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
