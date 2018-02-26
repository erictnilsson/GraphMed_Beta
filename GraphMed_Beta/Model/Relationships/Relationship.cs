using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GraphMed_Beta.Model.Relationships
{
    abstract class Relationship
    {
        public Relationship() { }

        public PropertyInfo GetProp(string propName)
        {
            return this.GetType().GetProperty(propName.First().ToString().ToUpper() + propName.Substring(1));
        }
    }
}
