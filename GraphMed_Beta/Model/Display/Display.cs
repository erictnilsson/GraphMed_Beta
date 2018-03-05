using GraphMed_Beta.Model.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphMed_Beta.Model.Display
{
    class Display
    {
        public string ConceptId { get; set; }
        public string Term { get; set; }
        public string Relationship { get; set; }

        public Display() { }

        public Display(string conceptId, string term)
        {
            this.ConceptId = conceptId;
            this.Term = term;
        }

        public Display(string conceptId, string term, string relationship)
        {
            this.ConceptId = conceptId;
            this.Term = term;
            this.Relationship = relationship; 
        }

        public void Print()
        {
            foreach (var prop in this.GetType().GetProperties())
            {
                Console.WriteLine(prop.Name + ": " + prop.GetValue(this));
            }
        }
    }
}
