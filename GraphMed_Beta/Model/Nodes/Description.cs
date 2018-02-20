using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphMed_Beta.Model.Nodes
{
    /*
     * INIT COMMENT
     */
    class Description : Node
    {
        public string Id { get; set; }
        public string EffectiveTime { get; set; }
        public string Active { get; set; }
        public string ModuleId { get; set; }
        public string ConceptId { get; set; }
        public string LanguageCode { get; set; }
        public string TypeId { get; set; }
        public string Term { get; set; }
        public string CaseSignificanceId { get; set; }

        public Description() : base() { }

        public Description(string id, string effectiveTime, string active, string moduleId, string conceptId, string languageCode, string typeId, string term, string caseSignificanceId)
        {
            this.Id = id;
            this.EffectiveTime = effectiveTime;
            this.Active = active;
            this.ModuleId = moduleId;
            this.ConceptId = conceptId;
            this.LanguageCode = languageCode;
            this.TypeId = typeId;
            this.Term = term;
            this.CaseSignificanceId = caseSignificanceId;
        }
    }
}
