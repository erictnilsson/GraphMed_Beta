using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphMed_Beta.CypherHandling
{
    /// <summary>
    /// USED TO WRITE 'MATCH' CYPHERS
    /// </summary>
    class CypherMatch : CypherBase
    {
        public CypherMatch(int? limit) : base (limit) { }

        public CypherMatch() : base() { }
    }
}
