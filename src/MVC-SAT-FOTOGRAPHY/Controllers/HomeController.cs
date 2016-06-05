using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using MVC_SAT_FOTOGRAPHY.Models;


namespace MVC_SAT_FOTOGRAPHY.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public SortedDictionary<int, string> Index(List<Person> person)
        {

            Dictionary<string, List<string>> temp = Person.DictFromPersonList(person);
            SatSolver.Processor fotography = new SatSolver.Processor(temp);
            if (fotography.Run())
            {
                //satisfable
                SortedDictionary<int, string> peoplePositions = fotography.getPeoplePositions();

                
                return peoplePositions;
            }
            else
            {
                //unsatisfable
                return null;
            }
            return null;
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
