﻿using Neo4jClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphMed_Beta.CypherHandling
{
    /// <summary>
    /// Used to write 'Create' Cyphers. 
    /// </summary>
    class CypherCreate : CypherBase
    {
        public CypherCreate() : base() { }

        /// <summary>
        /// Create an index for the specified Node on the specified attribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="identifier"></param>
        public void Index<T>(string identifier)
        {
            var node = typeof(T);
            var cypher = Connection.Cypher
                       .Create("INDEX ON: " + node.Name + "(" + identifier + ")");

            ExecuteWithoutResults(cypher);
        }

        /// <summary>
        /// Create an index for the specified Node on the specified attribute if the project's node type is not named exactly as the node. E.g. TermNode -> Term
        /// </summary>
        /// <param name="node"></param>
        /// <param name="identifier"></param>
        public void Index(string node, string identifier)
        {
            var cypher = Connection.Cypher
                      .Create("INDEX ON: " + node + "(" + identifier + ")");

            ExecuteWithoutResults(cypher);
        }

        /// <summary>
        /// Create a constraint on the specified Node on the specified attribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="identifier"></param>
        public void Constraint<T>(string identifier)
        {
            var node = typeof(T);
            var cypher = Connection.Cypher
                           .CreateUniqueConstraint("n: " + node.Name, "n." + identifier);

            ExecuteWithoutResults(cypher);
        }
    }
}
