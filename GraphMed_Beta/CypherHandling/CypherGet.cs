using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphMed_Beta.Model.Display;
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

        public IEnumerable<string> Get(string searchTerm, char relatives, int limit, string acceptability, string langCode)
        {
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
                    return Connection.Cypher
                                .Match("(c:Concept{Id:'" + searchTerm + "'})" + point + "(cc:Concept)<-[:REFERS_TO]-(d:Description{Active:'1'})-[r:" + rel + "{RefsetId: '" + lang + "'}]->(t:Term)")
                                .UsingIndex("c:Concept(Id)")
                                .Return<string>("t.Term")
                                .Results; 
                } else
                {
                }

            }
            catch (NeoException)
            {
                throw;
            }
            finally
            {
                Connection.Dispose();
            }
            return null;
        }
    }
}
