using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_SAT_FOTOGRAPHY.Models
{
    public class PeopleViewModel
    {
        List<Person> People { get; set; }

        public Dictionary<string, List<string>> DictFromPersonList()
        {
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();

            foreach (var item in People)
            {
                result.Add(item.Name, item.PreferedNeighbors);
            }

            return result;
        }
    }
}
