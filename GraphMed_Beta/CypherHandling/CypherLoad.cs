using GraphMed_Beta.FileHandling;
using GraphMed_Beta.Model.Nodes;
using GraphMed_Beta.Model.Relationships;
using GraphMed_Beta.Utilities;
using System;
using System.Configuration;

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

        public void Refset(bool index = true)
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
        /// Indexing is enabled as default. 
        /// </summary>
        /// <param name="index"></param>
        public void Concepts(bool constrain = true)
        {
            var uri = "file:///" + CurrentConfig.Instance.FullConcept.Substring(CurrentConfig.Instance.FullConcept.LastIndexOf('\\')+1);
            LoadNodes<Concept>(uri);

            if (constrain)
                Cypher.Create().Constraint<Concept>("Id");
        }

        /// <summary>
        /// Loads all Descriptions with or without forcing an anchoring relationship with a Concept-Node,
        /// indexing on 'ConceptId', 
        /// and constraining on 'Id'.
        /// </summary>
        /// <param name="forceRelationship"></param>
        /// <param name="index"></param>
        /// <param name="constrain"></param>
        public void Descriptions(bool forceRelationship = true, bool index = true, bool constrain = true)
        {
            var uri = "file:///" + CurrentConfig.Instance.FullDescription.Substring(CurrentConfig.Instance.FullDescription.LastIndexOf('\\') + 1);
            if (Connection.IsConnected)
            {
                if (forceRelationship)
                    LoadRelationalDescriptions(uri);
                else
                    LoadNodes<Description>(uri);

                if (index)
                    Cypher.Create().Index<Description>("ConceptId");
                if (constrain)
                    Cypher.Create().Constraint<Description>("Id");
            }
            else
            {
                Console.WriteLine("It seems like you don't have a connection with Neo4j... \n" +
                        "Make sure that an instance of Neo4j is connected and running before calling any cypher.");
            }
        }

        /// <summary>
        /// Loads the refsets of the specified .CSV URI; creating a refset-relationship and a Term-node
        /// </summary>
        /// <param name="uri"></param>
        private void LoadRefset(string uri)
        {
            if (uri != null)
            {
                var relationship = uri.Substring(uri.IndexOf('-') + 1, uri.LastIndexOf('.') - uri.IndexOf('-') - 1).ToUpper();
                var cypher = Connection.Cypher
                                  .LoadCsv(fileUri: new Uri(uri), identifier: Identifier, withHeaders: true, fieldTerminator: "\t", periodicCommit: CommitSize)
                                  .With(Identifier)
                                  .Limit(Limit)
                                  .Match("(d:Description)")
                                  .Where("d.Id = " + Identifier + ".referencedComponentId")
                                  .Create("(d)-[:" + relationship + "{ " + Utils.GetBuildString<Refset>(Identifier) + "}]->(t:Term {Id: " + Identifier + ".referencedComponentId, Term: d.Term})");

                ExecuteWithoutResults(cypher);
            }
            else
                Console.WriteLine("The URI seems to not exist... " +
                  "Make sure it does.");
        }

        /// <summary>
        /// Loads the relationships of the specified .CSV URI. 
        /// </summary>
        /// <param name="uri"></param>
        private void LoadRelationships(string uri)
        {
            if (uri != null)
            {
                var relationship = uri.Substring(uri.IndexOf('-') + 1, uri.LastIndexOf('.') - uri.IndexOf('-') - 1).ToUpper();
                var cypher = Connection.Cypher
                                  .LoadCsv(fileUri: new Uri(uri), identifier: Identifier, withHeaders: true, fieldTerminator: "\t", periodicCommit: CommitSize)
                                  .With(Identifier)
                                  .Limit(Limit)
                                  .Match("(c:Concept)", "(cc:Concept)")
                                  .Where("c.Id = " + Identifier + ".sourceId")
                                  .AndWhere("cc.Id = " + Identifier + ".destinationId")
                                  .Create("(c)-[:" + relationship + " {" + Utils.GetBuildString<Determinant>(Identifier) + "}]->(cc)");

                ExecuteWithoutResults(cypher);
            }
            else
                Console.WriteLine("The URI seems to not exist... " +
                  "Make sure it does.");

        }

        /// <summary>
        /// Loads Descriptions from the specified .CSV URI, connecting it to it's anchoring Concept. 
        /// </summary>
        /// <param name="uri"></param>
        private void LoadRelationalDescriptions(string uri)
        {
            if (uri != null)
            {
                var cypher = Connection.Cypher
                           .LoadCsv(fileUri: new Uri(uri), identifier: Identifier, withHeaders: true, fieldTerminator: "\t", periodicCommit: CommitSize)
                           .With(Identifier)
                           .Limit(Limit)
                           .Match("(c:Concept)")
                           .Where("c.Id = " + Identifier + ".conceptId")
                           .Create("(d: Description {" + Utils.GetBuildString<Description>(Identifier) + "})-[:REFERS_TO]->(c)");

                ExecuteWithoutResults(cypher);
            }
            else
                Console.WriteLine("The URI seems to not exist... " +
                   "Make sure it does.");

        }

        /// <summary>
        /// Loads the Nodes from the specified .CSV URI. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        private void LoadNodes<T>(string uri)
        {
            if (uri != null)
            {
                var node = typeof(T);
                var query = Connection.Cypher
                                  .LoadCsv(fileUri: new Uri(uri), identifier: Identifier, withHeaders: true, fieldTerminator: "\t", periodicCommit: CommitSize)
                                  .With(Identifier)
                                  .Limit(Limit)
                                  .Create("(n: " + node.Name + " {" + Utils.GetBuildString<T>(Identifier) + "})");

                ExecuteWithoutResults(query);
            }
            else
                Console.WriteLine("The URI seems to not exist... " +
                    "Make sure it does.");

        }
    }
}
