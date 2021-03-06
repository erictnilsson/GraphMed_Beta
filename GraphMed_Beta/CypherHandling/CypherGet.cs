﻿using System;
using System.Linq;
using GraphMed_Beta.Model.Display;
using GraphMed_Beta.Model.Nodes;
using Neo4jClient;

namespace GraphMed_Beta.CypherHandling
{
    /// <summary>
    /// Used to write 'Get' Cyphers. 
    /// </summary>
    class CypherGet : CypherBase
    {
        public CypherGet() : base() { }

        public CypherGet(int? limit) : base(limit) { }
        /// <summary>
        /// Gets the synonym term of the specified Concept. 
        /// Used primarily to get the relationship term when constructing relationships. 
        /// </summary>
        /// <param name="conceptId"></param>
        /// <returns></returns>
        public string Term(string conceptId)
        {
            var cypher = Connection.Cypher
                              .Match("(d:Description)-[:REFERS_TO]->(c:Concept)")
                              .Where("d.TypeId = '900000000000013009'")
                              .AndWhere("c.Id = '" + conceptId + "'")
                              .Return<string>("d.Term");

            return ExecuteWithResults(cypher).First();
        }

        public Result Nodes(string searchTerm, string relatives, string limit, string acceptability, string langCode)
        {
            Result returnResult = new Result();
            var lang = "900000000000508004"; //default GB
            var point = "--"; //default point at everything; family
            var rel = "PREFERRED"; //default relationship is PREFERRED
            var lim = Int32.Parse(limit);
            switch (relatives)
            {
                case "p":
                    point = "-[*.." + lim + "]->";
                    break;
                case "c":
                    point = "<-[*.." + lim + "]-";
                    break;
                case "f":
                    point = "-[*.." + lim + "]-";
                    break;
            }

            switch (acceptability)
            {
                case "pref":
                    rel = "PREFERRED";
                    break;
                case "acc":
                    rel = "ACCEPTABLE";
                    break;
            }

            switch (langCode)
            {
                case "GB":
                    lang = "900000000000508004";
                    break;
                case "US":
                    lang = "900000000000509007";
                    break;
            }

            return Get(searchTerm, point, lang, rel);
        }

        private Result Get(string searchTerm, string point, string lang, string rel)
        {
            Result returnResult = new Result();
            int n = 0;
            if (int.TryParse(searchTerm, out n))
            {
                var cypher = Connection.Cypher
                            .Match("(t:Term)<-[:" + rel + "{RefsetId: '" + lang + "'}]-(d:Description)-[:REFERS_TO]->(c:Concept{Id: '" + searchTerm + "'})" + point + "(cc:Concept)<-[:REFERS_TO]-(dd:Description)-[:" + rel + "{RefsetId:'" + lang + "'}]->(tt:Term)")
                            .UsingIndex("c:Concept(Id)")
                            .Where("d.Active='1'")
                            .AndWhere("dd.Active='1'")
                            .Return((t, c, tt, cc, r) => new
                            {
                                AnchorId = c.As<Concept>().Id,
                                AnchorTerm = t.As<TermNode>().Term,
                                cc.As<Concept>().Id,
                                tt.As<TermNode>().Term,
                            });

                var result = ExecuteWithResults(cypher);
                returnResult = new Result(result.FirstOrDefault().AnchorId, result.FirstOrDefault().AnchorTerm);

                foreach (var obj in result)
                    returnResult.Results.Add(new Display(obj.Id, obj.Term));
            }
            else
            {
                var result = Connection.Cypher
                             .Match("(t:Term{Term:'" + searchTerm + "'})<-[{RefsetId:'" + lang + "'}]-(d:Description)-[:REFERS_TO]->(c:Concept)" + point + "(cc:Concept)<-[:REFERS_TO]-(dd:Description)-[:" + rel + "{RefsetId:'" + lang + "'}]->(tt:Term)")
                             .Where("d.Active = '1'")
                             .AndWhere("dd.Active = '1'")
                             .Return((t, c, cc, tt) => new
                             {
                                 AnchorId = c.As<Concept>().Id,
                                 AnchorTerm = t.As<TermNode>().Term,
                                 cc.As<Concept>().Id,
                                 tt.As<TermNode>().Term
                             })
                             .Results;
                // breaks if no result
                if (result.Count() > 0)
                {
                    returnResult = new Result(result.FirstOrDefault().AnchorId, result.FirstOrDefault().AnchorTerm);

                    foreach (var obj in result)
                        returnResult.Results.Add(new Display(obj.Id, obj.Term));
                }
            }
            return returnResult;
        }


        /// <summary>
        /// Used internally to count all of the nodes in the database. 
        /// </summary>
        /// <returns></returns>
        public int CountNodes()
        {
            var cypher = Connection.Cypher
                .Match("(n)")
                .With("n")
                .Return<int>("count(n)");

            return ExecuteWithResults(cypher).First();
        }
    }
}
