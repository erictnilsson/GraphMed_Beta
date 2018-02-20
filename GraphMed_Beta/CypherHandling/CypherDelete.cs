using GraphMed_Beta.Model.Nodes;
using Neo4jClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphMed_Beta.CypherHandling
{
    /// <summary>
    /// USED TO WRITE 'DELETE' CYPHERS
    /// </summary>
    class CypherDelete : CypherBase
    {
        public CypherDelete() : base() { }

        public CypherDelete(int? limit) : base(limit) { }

        /// <summary>
        /// DETACHES AND DELETES ALL NODES INDEPENDENT ON THE NODE TYPE
        /// </summary>
        public void DetachDelete()
        {
            try
            {
                Connection.Cypher
                          .Match("(n)")
                          .With("n")
                          .Limit(Limit)
                          .DetachDelete("n")
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
        /// DELETES ALL NODES INDEPENDENT ON THE NODE TYPE
        /// </summary>
        public void Delete()
        {
            try
            {
                Connection.Cypher
                          .Match("(n)")
                          .With("n")
                          .Limit(Limit)
                          .Delete("n")
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
        /// DELETES ALL SPECIFIED NODES
        /// </summary>
        /// <typeparam name="Node"></typeparam>
        public void Delete<Node>()
        {
            var node = typeof(Node);
            try
            {
                Connection.Cypher
                          .Match("(n: " + node.Name + ")")
                          .With("n")
                          .Limit(Limit)
                          .Delete("n")
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
        /// DETACHED AND DELETES ALL SPECIFIED NODES
        /// </summary>
        /// <typeparam name="Node"></typeparam>
        public void DetachDelete<Node>()
        {
            var node = typeof(Node);
            try
            {
                Connection.Cypher
                          .Match("(n: " + node.Name + ")")
                          .With("n")
                          .Limit(Limit)
                          .DetachDelete("n")
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
