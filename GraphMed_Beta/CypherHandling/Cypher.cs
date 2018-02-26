using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphMed_Beta.CypherHandling
{
/// <summary>
/// Handler used to manage and write Cyphers. 
/// Call the methods and set a result limit and possible commit size as params. 
/// </summary>
    class Cypher
    {
        public static CypherCreate Create()
        {
            return new CypherCreate();
        }

        public static CypherLoad Load(int? limit, int? commit)
        {
            return new CypherLoad(limit, commit);
        }

        public static CypherMatch Match(int? limit)
        {
            return new CypherMatch(limit);
        }

        public static CypherGet Get(int? limit)
        {
            return new CypherGet(limit);
        }

        public static CypherDelete Delete(int? limit)
        {
            return new CypherDelete(limit); 
        }

        public static CypherDrop Drop()
        {
            return new CypherDrop(); 
        }


    }
}
