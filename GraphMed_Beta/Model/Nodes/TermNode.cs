using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphMed_Beta.Model.Nodes
{
    class TermNode : Node
    {
        public string Term { get; set; }
        public string Id { get; set; }

        public TermNode() : base() { }

        public TermNode(string term, string id) : base()
        {
            this.Term = term;
            this.Id = id;
        }
    }
}
