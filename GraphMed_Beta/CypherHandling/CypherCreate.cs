using Neo4jClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphMed_Beta.CypherHandling
{
    /// <summary>
    /// USED TO WRITE 'CREATE' CYPHERS. 
    /// </summary>
    class CypherCreate : CypherBase
    {
        public CypherCreate() : base() { }

        /// <summary>
        /// CREATE AN INDEX ON THE SPECIFIED NODE TYPE AND NODE IDENTIFIER
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="identifier"></param>
        public void Index<T>(string identifier)
        {
            var node = typeof(T);
            try
            {
                Connection.Cypher
                     .Create("INDEX ON: " + node.Name + "(" + identifier + ")")
                     .ExecuteWithoutResults();
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

        /// <summary>
        /// CREATE A CONSTRAINT ON THE SPECIFIED NODE TYPE AND NODE IDENTIFIER
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="identifier"></param>
        public void Constraint<T>(string identifier)
        {
            var node = typeof(T);
            try
            {
                Connection.Cypher
                          .CreateUniqueConstraint("n: " + node.Name, "n." + identifier)
                          .ExecuteWithoutResults();
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
