using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using SatSolver;

namespace MVC_SAT_FOTOGRAPHY.Tests
{
    public class SolverTest
    {
        [Fact]
        public void CommonTest()
        {
            //arrange
            Processor fotography = new Processor(new Dictionary<string, List<string>> {
                {"Betty",   new List<string> {"Mary" } },
                {"Chris",   new List<string> {"Betty","Gary" }},
                {"Donald",  null },
                {"Fred",    new List<string> {"Donald","Mary" }},
                {"Gary",    null },
                {"Mary",    new List<string> {"Mary"}},
                {"Paul",    new List<string> {"Donald"}}
            });
            SortedDictionary<int, string> dict = null;

            SortedDictionary<int, string> expected = new SortedDictionary<int, string>
            {
                { 1, "Paul"},
                { 2, "Donald"},
                { 3, "Fred"},
                { 4, "Mary"},
                { 5, "Betty"},
                { 6, "Chris"},
                { 7, "Gary"}
            };

            //act
            if (fotography.Run())
            {
                dict = fotography.getPeoplePositions();
            }

            //assert
            Assert.Equal(expected, dict);
        }

        [Fact]
        public void AllWantOnePersonTest()
        {
            //arrange
            Processor fotography = new Processor(new Dictionary<string, List<string>> {
                {"Betty", new List<string> { "Donald" } },
                {"Chris", new List<string> { "Donald" } },
                {"Donald", null},
                {"Fred", new List<string> { "Donald" } },
                //{"Gary", new List<string> {"Gary"} },
                //{"Mary", new List<string> {"Gary"} },
                //{"Paul", new List<string> {"Gary"} }
            });
            SortedDictionary<int, string> dict = null;

            //SortedDictionary<int, string> expected = new SortedDictionary<int, string>
            //{
            //    { 1, "Paul"},
            //    { 2, "Mary"},
            //    { 3, "Gary"},
            //    { 4, "Fred"},
            //    { 5, "Donald"},
            //    { 6, "Betty"},
            //    { 7, "Chris"}
            //};

            //act
            if (fotography.Run())
            {
                dict = fotography.getPeoplePositions();
            }

            //assert
            Assert.Null(dict);
        }
    }
}
