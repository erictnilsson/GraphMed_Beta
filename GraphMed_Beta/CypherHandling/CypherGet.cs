using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphMed_Beta.Model.Display;
using GraphMed_Beta.Model.Nodes;
using Neo4j.Driver.V1;
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
            try
            {
                return Connection.Cypher
                          .Match("(d:Description)-[:REFERS_TO]->(c:Concept)")
                          .Where("d.TypeId = '900000000000013009'")
                          .AndWhere("c.Id = '" + conceptId + "'")
                          .Return<string>("d.Term")
                          .Results.First();
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

        public Result Get(string searchTerm, char relatives, int limit, string acceptability, string langCode)
        {
            Result returnResult = new Result();
            var lang = "900000000000508004"; //default GB
            var point = "--"; //default point at everything; family
            var rel = "PREFERRED"; //default relationship is PREFERRED

            switch (relatives)
            {
                case 'p':
                    point = "-[*.." + limit + "]->";
                    break;
                case 'c':
                    point = "<-[*.." + limit + "]-";
                    break;
                case 'f':
                    point = "-[*.." + limit + "]-";
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

            try
            {
                if (int.TryParse(searchTerm, out int n))
                {
                    var result = Connection.Cypher
                                .Match("(t:Term)<-[:" + rel + "{RefsetId: '" + lang + "'}]-(d:Description)-[:REFERS_TO]->(c:Concept{Id: '" + searchTerm + "'})" + point + "(cc:Concept)<-[:REFERS_TO]-(dd:Description)-[:" + rel + "{RefsetId:'" + lang + "'}]->(tt:Term)")
                                .UsingIndex("c:Concept(Id)")
                                .Where("d.Active='1'")
                                .AndWhere("dd.Active='1'")
                                .Return((t, c, tt, cc) => new
                                {
                                    AnchorId = c.As<Concept>().Id,
                                    AnchorTerm = t.As<TermNode>().Term,
                                    cc.As<Concept>().Id,
                                    tt.As<TermNode>().Term
                                })
                                .Results;

                    returnResult = new Result(result.FirstOrDefault().Id, result.FirstOrDefault().Term);

                    foreach (var obj in result)
                        returnResult.Results.Add(new Display(obj.Id, obj.Term));
                }
                else
                {
                    var result = Connection.Cypher
                                 .Match("(t:Term{Term:'" + searchTerm + "'})<-[{RefsetId:'" + lang + "'}]-(d:Description)-[:REFERS_TO]->(c:Concept)" + point + "(cc:Concept)<-[REFERS_TO]-(dd:Description)-[:" + rel + "{RefsetId:'" + lang + "'}]->(tt:Term)")
                                 .Where("d.Active = '1'")
                                 .AndWhere("dd.Active = '1'")
                                 .Return((t, c, tt, cc) => new
                                 {
                                     AnchorId = c.As<Concept>().Id,
                                     AnchorTerm = t.As<TermNode>().Term,
                                     cc.As<Concept>().Id,
                                     tt.As<TermNode>().Term
                                 })
                                 .Results;

                    returnResult = new Result(result.FirstOrDefault().Id, result.FirstOrDefault().Term);

                    foreach (var obj in result)
                        returnResult.Results.Add(new Display(obj.Id, obj.Term));
                }
                return returnResult;
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
