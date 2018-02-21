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
    /// USED TO WRITE 'DROP' CYPHERS
    /// </summary>
    class CypherDrop : CypherBase
    {
        public CypherDrop() : base() { }

        public CypherDrop(int? limit) : base(limit) { }

        /// <summary>
        /// DROPS ALL INDEXES AND CONSTRAINTS
        /// </summary>
        public void Everything()
        {
            Index<Description>();
            Constraint<Description>();
            Index<Concept>(); 
        }

        /// <summary>
        /// DROP INDEX ON THE SPECIFIED NODE
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
        /// DROP CONSTRAINT ON THE SPECIFIED NODE
        /// </summary>
        /// <typeparam name="Node"></typeparam>
        public void Constraint<Node>()
        {
            DropConstraint<Node>("Id", "IS UNIQUE");
        }

        /// <summary>
        /// USED PRIVATELY TO GENERICALLY DROP A CONSTRAINT
        /// </summary>
        /// <typeparam name="Node"></typeparam>
        /// <param name="identifier"></param>
        /// <param name="constraint"></param>
        private void DropConstraint<Node>(string identifier, string constraint)
        {
            var node = typeof(Node);
            try
            {
                Connection.Cypher.Drop("CONSTRAINT ON (n:" + node.Name + ") ASSERT n." + identifier + " " + constraint)
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
        /// USED PRIVATELY TO GENERICALLY DROP AN INDEX
        /// </summary>
        /// <typeparam name="Node"></typeparam>
        /// <param name="identifier"></param>
        private void DropIndex<Node>(string identifier)
        {
            var node = typeof(Node);
            try
            {
                Connection.Cypher.Drop("INDEX ON :" + node.Name + " (" + identifier + ")")
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
