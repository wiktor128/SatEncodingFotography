using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatSolver
{
    class Resolver
    {
        // person, and prefered neighbour (max 2)
        private Dictionary<string, List<string>> people;
        //variable for every possible neighbourhood id, name_1, name_2
        private Dictionary<int, Tuple<string, string>> variablesNeighborhood;
        //variable for every possible people position in a row
        private Dictionary<int, Tuple<string, int>> variablesPosition;

        public Dictionary<int, Tuple<string, string>> getNeighborhoodVars
        {
            get { return variablesNeighborhood; }
        }

        public Dictionary<int, Tuple<string, int>> getVariablesPosition
        {
            get { return variablesPosition; }
        }

        int clauseCount = 0;
        int countVariable = 0;
        int numberOfPositions = 0;

        string _lastGeneratedCNF = "";

        public Resolver(Dictionary<string, List<string>> peoplePreferenceList)
        {
            people = peoplePreferenceList;
            numberOfPositions = people.Count;
        }

        private Dictionary<int, Tuple<string, string>> GenerateNeighborhoodVariables()
        {
            Dictionary<int, Tuple<string, string>> temp = new Dictionary<int, Tuple<string, string>>();


            foreach (string item in people.Keys)
            {
                temp.Add(++countVariable, new Tuple<string, string>(item, null));
            }

            foreach (string item in people.Keys)
            {
                foreach (string item2 in people.Keys)
                {
                    if (item != item2)
                    {
                        temp.Add(++countVariable, new Tuple<string, string>(item, item2));
                    }
                }
            }

            variablesNeighborhood = temp;
            return temp;
        }
        private Dictionary<int, Tuple<string, int>> GeneratePositionVariables()
        {
            Dictionary<int, Tuple<string, int>> temp = new Dictionary<int, Tuple<string, int>>();

            foreach (string item in people.Keys)
            {
                for (int i = 1; i <= numberOfPositions; i++)
                {
                    temp.Add(++countVariable, new Tuple<string, int>(item, i));
                }
            }
            variablesPosition = temp;
            return temp;
        }

        private string FirstCondition() //constraint: at most 2 person has one neighbor
        {
            string cnf = "";
            for (int index = 0; index < variablesNeighborhood.Count; index++)
            {
                var item = variablesNeighborhood.ElementAt(index);
                var itemKey = item.Key;
                var itemValue = item.Value;

                for (int index2 = index + 1; index2 < variablesNeighborhood.Count; index2++)
                {
                    var item2 = variablesNeighborhood.ElementAt(index2);
                    var itemKey2 = item2.Key;
                    var itemValue2 = item2.Value;

                    for (int index3 = index2 + 1; index3 < variablesNeighborhood.Count; index3++)
                    {
                        var item3 = variablesNeighborhood.ElementAt(index3);
                        var itemKey3 = item3.Key;
                        var itemValue3 = item3.Value;

                        if (itemValue.Item2 == null && itemValue2.Item2 == null && itemValue3.Item2 == null)
                        {
                            // nie są możliwe ustawienia z 3 'pustymi sąsiadami'
                            cnf += " -" + itemKey + " -" + itemKey2 + " -" + itemKey3 + " 0 \n";
                            ++clauseCount;
                        }
                    }
                }
            }
            return cnf;
        }

        private string SecondCondition() //constraint: at least 2 person has one neighbor
        {
            string cnf = "";
            for (int index = 0; index < variablesNeighborhood.Count; index++)
            {
                var item = variablesNeighborhood.ElementAt(index);
                var itemKey = item.Key;
                var itemValue = item.Value;

                for (int index2 = 0; index2 < variablesNeighborhood.Count; index2++)
                {
                    var item2 = variablesNeighborhood.ElementAt(index2);
                    var itemKey2 = item2.Key;
                    var itemValue2 = item2.Value;

                    if (itemValue.Item2 == null && itemValue2.Item2 == null && itemValue.Item1 != itemValue2.Item1)
                    {
                        // nie są możliwe ustawienia z 3 'pustymi sąsiadami'
                        cnf += " " + itemKey2;
                    }

                }
                if (itemValue.Item2 == null)
                {
                    cnf += " 0 \n";
                    ++clauseCount;
                }
            }
            return cnf;
        }

        private string ThirdCondition() //constraint: every person can have max 2 neighbours (or empty neighbour)
        {
            string cnf = "";
            for (int index = 0; index < variablesNeighborhood.Count; index++)
            {
                var item = variablesNeighborhood.ElementAt(index);
                var itemKey = item.Key;
                var itemValue = item.Value;

                for (int index2 = index + 1; index2 < variablesNeighborhood.Count; index2++)
                {
                    var item2 = variablesNeighborhood.ElementAt(index2);
                    var itemKey2 = item2.Key;
                    var itemValue2 = item2.Value;

                    for (int index3 = index2 + 1; index3 < variablesNeighborhood.Count; index3++)
                    {
                        var item3 = variablesNeighborhood.ElementAt(index3);
                        var itemKey3 = item3.Key;
                        var itemValue3 = item3.Value;

                        if (itemValue.Item1 == itemValue2.Item1 && itemValue2.Item1 == itemValue3.Item1)
                        {
                            cnf += " -" + itemKey + " -" + itemKey2 + " -" + itemKey3 + " 0 \n";
                            ++clauseCount;
                        }

                    }

                }
            }
            return cnf;
        }

        private string FourthCondition() //constraint: every person can have exacly (at least?) 2 neighbours (or empty neighbour)
        {
            string cnf = "";
            Dictionary<string, List<int>> temp = new Dictionary<string, List<int>>();
            foreach (var item in people)
            {
                //tworzy listę ludzi z dopiętą listą wszystkich zmiennych/możliwych sąsiadów
                temp.Add(item.Key, new List<int>());
            }

            for (int index = 0; index < variablesNeighborhood.Count; index++)
            {
                var item = variablesNeighborhood.ElementAt(index);
                var itemKey = item.Key;
                var itemValue = item.Value;

                if (temp.ContainsKey(itemValue.Item1))
                {
                    temp[itemValue.Item1].Add(itemKey);
                }
                else
                {
                    throw new KeyNotFoundException();
                }
            }

            foreach (var item in temp)
            {
                foreach (var item2 in item.Value)
                {
                    foreach (var item3 in item.Value)
                    {
                        if (item2 != item3)
                        {
                            cnf += " " + item3;
                        }
                    }

                    cnf += " 0 \n";
                    ++clauseCount;
                }
            }
            return cnf;
        }

        private string FifthCondition() //constraint: person at ends can't be neighbours
        {
            string cnf = "";
            for (int index = 0; index < variablesNeighborhood.Count; index++)
            {
                var item = variablesNeighborhood.ElementAt(index);
                var itemKey = item.Key;
                var itemValue = item.Value;

                for (int index2 = index + 1; index2 < variablesNeighborhood.Count; index2++)
                {
                    var item2 = variablesNeighborhood.ElementAt(index2);
                    var itemKey2 = item2.Key;
                    var itemValue2 = item2.Value;

                    for (int index3 = index2 + 1; index3 < variablesNeighborhood.Count; index3++)
                    {
                        var item3 = variablesNeighborhood.ElementAt(index3);
                        var itemKey3 = item3.Key;
                        var itemValue3 = item3.Value;

                        if (itemValue.Item2 == null && itemValue2.Item2 == null && ((itemValue2.Item1 == itemValue3.Item1 && itemValue3.Item1 == itemValue.Item2) || (itemValue2.Item1 == itemValue3.Item2 && itemValue3.Item1 == itemValue.Item1)))
                        {
                            cnf += " -" + itemKey + " -" + itemKey2 + " -" + itemKey3 + " 0 \n";
                            ++clauseCount;
                        }

                    }

                }
            }
            return cnf;
        }

        private string SixthCondition() //constraint: person implication for matching variables
        {
            string cnf = "";
            for (int index = 0; index < variablesNeighborhood.Count; index++)
            {
                var item = variablesNeighborhood.ElementAt(index);
                var itemKey = item.Key;
                var itemValue = item.Value;

                for (int index2 = index + 1; index2 < variablesNeighborhood.Count; index2++)
                {
                    var item2 = variablesNeighborhood.ElementAt(index2);
                    var itemKey2 = item2.Key;
                    var itemValue2 = item2.Value;

                    if (itemValue.Item1 == itemValue2.Item2 && itemValue.Item2 == itemValue2.Item1)
                    {
                        cnf += " -" + itemKey + " " + itemKey2 + " 0 \n";
                        ++clauseCount;
                        cnf += " -" + itemKey2 + " " + itemKey + " 0 \n";
                        ++clauseCount;
                    }
                }
            }
            return cnf;
        }

        private string SeventhCondition() //constraint: every person have at most one position
        {
            string cnf = "";
            for (int index = 0; index < variablesPosition.Count; index++)
            {
                var item = variablesPosition.ElementAt(index);
                var itemKey = item.Key;
                var itemValue = item.Value;

                for (int index2 = index + 1; index2 < variablesPosition.Count; index2++)
                {
                    var item2 = variablesPosition.ElementAt(index2);
                    var itemKey2 = item2.Key;
                    var itemValue2 = item2.Value;

                    if (itemValue.Item1 == itemValue2.Item1)
                    {
                        cnf += " -" + itemKey + " -" + itemKey2 + " 0 \n";
                        ++clauseCount;
                    }
                }
            }
            return cnf;
        }

        private string EighthCondition() //constraint: every person have at least one position 
        {
            string cnf = "";
            for (int index = 0; index < variablesPosition.Count; index++)
            {
                var item = variablesPosition.ElementAt(index);
                var itemKey = item.Key;
                var itemValue = item.Value;

                for (int index2 = 0; index2 < variablesPosition.Count; index2++)
                {
                    var item2 = variablesPosition.ElementAt(index2);
                    var itemKey2 = item2.Key;
                    var itemValue2 = item2.Value;

                    if (itemValue.Item1 == itemValue2.Item1 /*&& itemValue.Item2 != itemValue2.Item2*/)
                    {
                        cnf += " " + itemKey2;
                    }
                }
                cnf += " 0 \n";
                ++clauseCount;
            }
            return cnf;
        }

        private string NinthCondition()
        {
            string cnf = "";

            for (int index = 0; index < variablesPosition.Count; index++)
            {
                var item = variablesPosition.ElementAt(index);
                var itemKey = item.Key;
                var itemValue = item.Value;

                for (int index2 = index + 1; index2 < variablesPosition.Count; index2++)
                {
                    var item2 = variablesPosition.ElementAt(index2);
                    var itemKey2 = item2.Key;
                    var itemValue2 = item2.Value;

                    if (itemValue.Item2 == itemValue2.Item2)
                    {
                        cnf += " -" + itemKey + " -" + itemKey2 + " 0 \n";
                        ++clauseCount;
                    }
                }
            }
            return cnf;
        } //constraint: every position have at most one person

        private string TenthCondition()
        {
            string cnf = "";
            for (int index = 0; index < variablesNeighborhood.Count; index++)
            {
                var item = variablesNeighborhood.ElementAt(index);
                var itemKey = item.Key;
                var itemValue = item.Value;

                if (itemValue.Item2 == null) // ma sąsiada pustego
                {

                    for (int index2 = 0; index2 < variablesPosition.Count; index2++)
                    {
                        var item2 = variablesPosition.ElementAt(index2);
                        var itemKey2 = item2.Key;
                        var itemValue2 = item2.Value;

                        if ((itemValue.Item1 == itemValue2.Item1) && (itemValue2.Item2 != 1) && (itemValue2.Item2 != numberOfPositions))
                        {
                            cnf += " -" + itemKey + " -" + itemKey2 + " 0 \n";
                            ++clauseCount;
                        }
                    }
                }
            }
            return cnf;
        } //constraint: person at first and last position have only one neighbor

        private string EleventhCondition()
        {
            string cnf = "";
            for (int index = 0; index < variablesNeighborhood.Count; index++)
            {
                var item = variablesNeighborhood.ElementAt(index);
                var itemKey = item.Key;
                var itemValue = item.Value;

                for (int index2 = index + 1; index2 < variablesNeighborhood.Count; index2++)
                {
                    var item2 = variablesNeighborhood.ElementAt(index2);
                    var itemKey2 = item2.Key;
                    var itemValue2 = item2.Value;

                    if (itemValue.Item1 == itemValue2.Item2 && itemValue.Item2 == itemValue2.Item1)
                    {
                        for (int index3 = 0; index3 < variablesPosition.Count; index3++)
                        {
                            var itemPos = variablesPosition.ElementAt(index3);
                            var itemPosKey = itemPos.Key;
                            var itemPosValue = itemPos.Value;

                            if (itemValue.Item1 == itemPosValue.Item1)
                            {
                                cnf += " -" + itemKey + " -" + itemPosKey;

                                for (int index4 = 0; index4 < variablesPosition.Count; index4++)
                                {
                                    var itemPos2 = variablesPosition.ElementAt(index4);
                                    var itemPosKey2 = itemPos2.Key;
                                    var itemPosValue2 = itemPos2.Value;

                                    if (itemValue2.Item1 == itemPosValue2.Item1 && ((itemPosValue.Item2 == (itemPosValue2.Item2 - 1)) || (itemPosValue.Item2 == (itemPosValue2.Item2 + 1))))
                                    {
                                        cnf += " " + itemPosKey2;
                                    }
                                }
                                cnf += " 0 \n";
                                ++clauseCount;
                            }
                        }
                    }
                }
            }
            return cnf;
        }

        public string GenerateCNF()
        {
            this.clauseCount = 0;
            this.countVariable = 0;
            GenerateNeighborhoodVariables();
            GeneratePositionVariables();

            string cnf = "";

            cnf += FirstCondition();
            cnf += SecondCondition();
            cnf += ThirdCondition();
            cnf += FourthCondition();
            cnf += FifthCondition();
            cnf += SixthCondition();
            cnf += SeventhCondition();
            cnf += EighthCondition();
            cnf += NinthCondition();
            cnf += TenthCondition();
            cnf += EleventhCondition();

            _lastGeneratedCNF = "p cnf " + (variablesNeighborhood.Count + variablesPosition.Count) + " " + clauseCount + "\n" + cnf;

            return _lastGeneratedCNF;
        }

    }
}
