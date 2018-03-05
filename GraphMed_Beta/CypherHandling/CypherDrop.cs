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
    /// Used to write 'Drop' Cyphers. 
    /// </summary>
    class CypherDrop : CypherBase
    {
        public CypherDrop() : base() { }

        public CypherDrop(int? limit) : base(limit) { }

        /// <summary>
        /// Drops all indexes and constraints. 
        /// </summary>
        public void Everything()
        {
            Index<Description>();
            Constraint<Description>();
            Index<Concept>();
        }

        /// <summary>
        /// Drops index on the specifed Node. 
        /// </summary>
        /// <typeparam name="Node"></typeparam>
        public void Index<Node>()
        {
            var node = typeof(Node);
            var identifier = "";
            if (node.Name.Equals("Concept"))
            {
                identifier = "Id";
            }
            else if (node.Name.Equals("Description"))
            {
                identifier = "ConceptId";
            }
            DropIndex<Node>(identifier);
        }

        /// <summary>
        /// Drops constraint on the specified Node. 
        /// </summary>
        /// <typeparam name="Node"></typeparam>
        public void Constraint<Node>()
        {
            DropConstraint<Node>("Id", "IS UNIQUE");
        }

        /// <summary>
        /// Used privately to generically drop a constraint. 
        /// </summary>
        /// <typeparam name="Node"></typeparam>
        /// <param name="identifier"></param>
        /// <param name="constraint"></param>
        private void DropConstraint<Node>(string identifier, string constraint)
        {
            var node = typeof(Node);
            var cypher = Connection.Cypher.Drop("CONSTRAINT ON (n:" + node.Name + ") ASSERT n." + identifier + " " + constraint);

            ExecuteWithoutResults(cypher);
        }

        /// <summary>
        /// Used privately to generically drop an index. 
        /// </summary>
        /// <typeparam name="Node"></typeparam>
        /// <param name="identifier"></param>
        private void DropIndex<Node>(string identifier)
        {
            var node = typeof(Node);
            var cypher = Connection.Cypher.Drop("INDEX ON :" + node.Name + " (" + identifier + ")");

            ExecuteWithoutResults(cypher);
        }
    }
}
