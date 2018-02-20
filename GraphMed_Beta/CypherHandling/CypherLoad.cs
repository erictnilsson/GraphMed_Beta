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
    /// USED TO WRITE 'LOAD' CYPHERS
    /// </summary>
    class CypherLoad : CypherBase
    {
        private int? CommitSize { get; set; }
        private string Identifier = "line";

        public CypherLoad(int? limit, int? commitSize) : base(limit)
        {
            this.CommitSize = commitSize;
        }

        /// <summary>
        /// LOADS ALL PARSED RELATIONSHIPS
        /// </summary>
        public void Relationships()
        {
            foreach (var uri in FileHandler.GetFiles("parsedRelationships-"))
                LoadRelationships(uri);
        }

        /// <summary>
        /// LOAD ALL CONCEPTS
        /// WITH OR WITHOUT INDEXING ON 'ID'
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
        /// LOAD ALL DESCRIPTIONS
        /// WITH OUR WITHOUT INDEXING ON 'CONCEPTID'
        /// AND CONSTRAINING ON 'ID'
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

        /// <summary>
        /// LOAD RELATIONSHIP OF THE SPECIFIED CSV URI
        /// </summary>
        /// <param name="uri"></param>
        private void LoadRelationships(string uri)
        {
            var relationship = uri.Substring(uri.IndexOf('-') + 1, uri.IndexOf('.') - uri.IndexOf('-') - 1).ToUpper();
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
        /// LOAD DESCRIPTION FROM THE SPECIFIED CSV URI
        /// CONNECTING IT TO IT'S ANCHORING CONCEPT
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
        /// LOAD NODES FROM THE SPECIFIED CSV URI
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
                          .With("line")
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
