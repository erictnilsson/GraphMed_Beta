using GraphMed_Beta.Model.Nodes;
using Neo4jClient;
using Neo4jClient.Cypher;
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
        /// Deletes all nodes independent of the Node type
        /// </summary>
        public void All(bool detach)
        {
            var cypher = Connection.Cypher
                     .Match("(n)")
                     .With("n")
                     .Limit(Limit)
                     .Delete("(n)");

            if (detach)
            {
                cypher = Connection.Cypher
                            .Match("(n)")
                            .With("n")
                            .Limit(Limit)
                            .DetachDelete("(n)");
            }
            ExecuteWithoutResults(cypher);

        }

        /// <summary>
        /// Deletes all specified Nodes. 
        /// </summary>
        /// <typeparam name="Node"></typeparam>
        public void Node<Node>(bool detach)
        {
            var node = typeof(Node);
            var cypher = Connection.Cypher
                           .Match("(n: " + node.Name + ")")
                           .With("n")
                           .Limit(Limit)
                           .Delete("(n)");
            if (detach)
            {
                cypher = Connection.Cypher
                            .Match("(n: " + node.Name + ")")
                            .With("n")
                            .Limit(Limit)
                            .DetachDelete("n");
            }

            ExecuteWithoutResults(cypher);
        }

        /// <summary>
        /// Deletes everything in the database
        /// </summary>
        public void Everything()
        {
            var count = Cypher.Get().CountNodes();
            var orig = count;
            Console.WriteLine("There are " + count + " nodes in the database. The deleting process may take several minutes. Please hold...");

            Cypher.Drop().Constraint<Concept>();
            Cypher.Drop().Constraint<Description>();
            Cypher.Drop().Index<Concept>();
            Cypher.Drop().Index<Description>();
            Cypher.Drop().Index<TermNode>();

            while (count > 0)
            {
                Cypher.Delete(100000).All(true);
                Console.WriteLine(count + " nodes left");
            }
            Console.WriteLine("All " + orig + " nodes deleted");
        }
    }
}
