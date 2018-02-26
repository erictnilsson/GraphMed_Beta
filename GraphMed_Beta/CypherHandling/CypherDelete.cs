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
    /// Used to write 'Delete' Cyphers. 
    /// </summary>
    class CypherDelete : CypherBase
    {
        public CypherDelete() : base() { }

        public CypherDelete(int? limit) : base(limit) { }

        /// <summary>
        /// Detaches and deletes all nodes independent on the Node type. 
        /// </summary>
        public void DetachDelete()
        {
            try
            {
                Connection.Cypher
                          .Match("(n)")
                          .With("n")
                          .Limit(Limit)
                          .DetachDelete("(n)")
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
        /// Deletes all nodes independent of the Node type
        /// </summary>
        public void Delete()
        {
            try
            {
                Connection.Cypher
                          .Match("(n)")
                          .With("n")
                          .Limit(Limit)
                          .Delete("(n)")
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
        /// Deletes all specified Nodes. 
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
                          .Delete("(n)")
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
        /// Detaches and deletes all specified Nodes. 
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
