using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphMed_Beta.Model.Relationships
{
/// <summary>
/// Model for the relationships that determines a relationship between two Nodes. E.G. 'IS_A'. 
/// </summary>
    class Determinant : Relationship
    {
        public string Id { get; set; }
        public string EffectiveTime { get; set; }
        public string Active { get; set; }
        public string ModuleId { get; set; }
        public string SourceId { get; set; }
        public string DestinationId { get; set; }
        public string RelationshipGroup { get; set; }
        public string TypeId { get; set; }
        public string CharacteristicTypeId { get; set; }
        public string ModifierId { get; set; }

        public Determinant() : base() { }
    }
}
