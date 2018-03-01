using GraphMed_Beta.FileHandling;
using GraphMed_Beta.Model.Nodes;
using GraphMed_Beta.Model.Relationships;
using GraphMed_Beta.Utilities;
using Neo4jClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphMed_Beta.CypherHandling
{
    /// <summary>
    /// Used to write 'Load' Cyphers. 
    /// </summary>
    class CypherLoad : CypherBase
    {
        private int? CommitSize { get; set; }
        private string Identifier = "line";

        public CypherLoad(int? limit, int? commitSize) : base(limit)
        {
            this.CommitSize = commitSize;
        }

        public void Refset(bool index)
        {
            foreach (var uri in FileHandler.GetFiles("parsedRefset-"))
                LoadRefset("file:///" + uri.Substring(uri.LastIndexOf("\\") + 1));

            if (index)
                Cypher.Create().Index("Term", "Term"); 
        }

        /// <summary>
        /// Loads all Relationships. 
        /// </summary>
        public void Relationships()
        {
            foreach (var uri in FileHandler.GetFiles("parsedRelationship-"))
                LoadRelationships("file:///" + uri.Substring(uri.LastIndexOf("\\") + 1));
        }

        /// <summary>
        /// Loads all Concepts with our without indexing the Concepts on 'Id'.
        /// </summary>
        /// <param name="index"></param>
        public void Concepts(bool index)
        {
            var uri = ConfigurationManager.AppSettings["concepts"];
            LoadNodes<Concept>(uri);

            if (index)
                Cypher.Create().Index<Concept>("Id");
        }

        /// <summary>
        /// Loads all Descriptions with or without indexing on 'ConceptId' and constraining on 'Id'.
        /// </summary>
        /// <param name="forceRelationship"></param>
        /// <param name="index"></param>
        /// <param name="constrain"></param>
        public void Descriptions(bool forceRelationship, bool index, bool constrain)
        {
            var uri = ConfigurationManager.AppSettings["descriptions"];

            if (forceRelationship)
                LoadRelationalDescriptions(uri);
            else
                LoadNodes<Description>(uri);

            if (index)
                Cypher.Create().Index<Description>("ConceptId");
            if (constrain)
                Cypher.Create().Constraint<Description>("Id");
        }

        private void LoadRefset(string uri)
        {
            var relationship = uri.Substring(uri.IndexOf('-') + 1, uri.LastIndexOf('.') - uri.IndexOf('-') - 1).ToUpper();
            try
            {
                Connection.Cypher
                          .LoadCsv(fileUri: new Uri(uri), identifier: Identifier, withHeaders: true, fieldTerminator: "\t", periodicCommit: CommitSize)
                          .With(Identifier)
                          .Limit(Limit)
                          .Match("(d:Description)")
                          .Where("d.Id = " + Identifier + ".referencedComponentId")
                          .Create("(d)-[:" + relationship + "{ " + Utils.GetBuildString<Refset>(Identifier) + "}]->(t:Term {Id: "+ Identifier + ".referencedComponentId, Term: d.Term})")
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
        /// Loads the relationships of the specified .CSV URI. 
        /// </summary>
        /// <param name="uri"></param>
        private void LoadRelationships(string uri)
        {
            var relationship = uri.Substring(uri.IndexOf('-') + 1, uri.LastIndexOf('.') - uri.IndexOf('-') - 1).ToUpper();
            try
            {
                Connection.Cypher
                          .LoadCsv(fileUri: new Uri(uri), identifier: Identifier, withHeaders: true, fieldTerminator: "\t", periodicCommit: CommitSize)
                          .With(Identifier)
                          .Limit(Limit)
                          .Match("(c:Concept)", "(cc:Concept)")
                          .Where("c.Id = " + Identifier + ".sourceId")
                          .AndWhere("cc.Id = " + Identifier + ".destinationId")
                          .Create("(c)-[:" + relationship + " {" + Utils.GetBuildString<Determinant>(Identifier) + "}]->(cc)")
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
        /// Loads Descriptions from the specified .CSV URI, connecting it to it's anchoring Concept. 
        /// </summary>
        /// <param name="uri"></param>
        private void LoadRelationalDescriptions(string uri)
        {
            try
            {
                Connection.Cypher
                          .LoadCsv(fileUri: new Uri(uri), identifier: Identifier, withHeaders: true, fieldTerminator: "\t", periodicCommit: CommitSize)
                          .With(Identifier)
                          .Limit(Limit)
                          .Match("(c:Concept)")
                          .Where("c.Id = " + Identifier + ".conceptId")
                          .Create("(d: Description {" + Utils.GetBuildString<Description>(Identifier) + "})-[:REFERS_TO]->(c)")
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
        /// Loads the Nodes from the specified .CSV URI. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        private void LoadNodes<T>(string uri)
        {
            var node = typeof(T);
            try
            {
                Connection.Cypher
                          .LoadCsv(fileUri: new Uri(uri), identifier: Identifier, withHeaders: true, fieldTerminator: "\t", periodicCommit: CommitSize)
                          .With(Identifier)
                          .Limit(Limit)
                          .Create("(n: " + node.Name + " {" + Utils.GetBuildString<T>(Identifier) + "})")
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
