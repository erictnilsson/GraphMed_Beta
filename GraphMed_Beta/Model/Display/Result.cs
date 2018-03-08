using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphMed_Beta.Model.Display
{
    class Result
    {
        public string Id { get; set; }
        public string Term { get; set; }
        public List<Display> Results { get; set; }

        public Result()
        {
            Results = new List<Display>();
        }

        public Result(string id, string term, List<Display> results)
        {
            this.Id = id;
            this.Term = term;
            this.Results = results;
        }

        public Result(string id, string term)
        {
            this.Id = id;
            this.Term = term;
            this.Results = new List<Display>();
        }
    }
}
