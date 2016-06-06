using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_SAT_FOTOGRAPHY.Models
{
    public class Person
    {
        public string Name { get; set; }

        public List<string> PreferedNeighbors { get; set; } // should be maximum 2

        public static Dictionary<string, List<string>> DictFromPersonList(List<Person> people)
        {
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();

            foreach (var item in people)
            {
                result.Add(item.Name, item.PreferedNeighbors);
            }

            return result;
        }
    }
}
