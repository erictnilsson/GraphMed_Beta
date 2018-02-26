using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphMed_Beta.CypherHandling
{
    /// <summary>
    /// Used to write 'Match' Cyphers. 
    /// </summary>
    class CypherMatch : CypherBase
    {
        public CypherMatch(int? limit) : base (limit) { }

        public CypherMatch() : base() { }
    }
}
