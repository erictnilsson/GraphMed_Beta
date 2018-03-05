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
            var cypher = Connection.Cypher
                            .Match("(n)")
                            .With("n")
                            .Limit(Limit)
                            .DetachDelete("(n)");

            ExecuteWithoutResults(cypher); 
        }


        /// <summary>
        /// Deletes all nodes independent of the Node type
        /// </summary>
        public void Delete()
        {
            var cypher = Connection.Cypher
                          .Match("(n)")
                          .With("n")
                          .Limit(Limit)
                          .Delete("(n)");

            ExecuteWithoutResults(cypher); 
         
        }

        /// <summary>
        /// Deletes all specified Nodes. 
        /// </summary>
        /// <typeparam name="Node"></typeparam>
        public void Delete<Node>()
        {
            var node = typeof(Node);
            var cypher = Connection.Cypher
                           .Match("(n: " + node.Name + ")")
                           .With("n")
                           .Limit(Limit)
                           .Delete("(n)");

            ExecuteWithoutResults(cypher); 
        }

        /// <summary>
        /// Detaches and deletes all specified Nodes. 
        /// </summary>
        /// <typeparam name="Node"></typeparam>
        public void DetachDelete<Node>()
        {
            var node = typeof(Node);
            var cypher = Connection.Cypher
                            .Match("(n: " + node.Name + ")")
                            .With("n")
                            .Limit(Limit)
                            .DetachDelete("n");

            ExecuteWithoutResults(cypher); 
        }
    }
}
