using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphMed_Beta.Model.Relationships
{
    class Refset : Relationship
    {
        public string Id { get; set; }
        public string EffectiveTime { get; set; }
        public string Active { get; set; }
        public string ModuleId { get; set; }

        public string RefsetId { get; set; }
        public string ReferencedComponentId { get; set; }
        public string AcceptabilityId { get; set; }

        public Refset() : base() { }

        public Refset(string id, string effectiveTime, string active, string moduleId, string refsetId, string referencedComponentId, string acceptabilityId)
        {
            this.Id = id;
            this.EffectiveTime = effectiveTime;
            this.Active = active;
            this.ModuleId = moduleId;
            this.RefsetId = refsetId; 
            this.ReferencedComponentId = referencedComponentId;
            this.AcceptabilityId = acceptabilityId;
        }
    }
}
