using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2
{
    internal class lab4
    {
        public static Dictionary<String, String> variables = new Dictionary<String, String>();
        private static String[] comparisonOperators = { "==", "<=", ">=", "!=", "<", ">" };
        private static String[] ariphmeticalOperators = { "+", "-", "*", "/" };
        private static string currentFunType = "";

        private static bool waitingForMethod = false;
        private static bool waitingForVariable = false;
        private static bool waitingForComparison = false;

        public static void SemanticAnalisys(String[] words, Dictionary<String, String> variables)
        {
            var newWords = words.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            words = newWords;

            for (int i = 0; i < words.Length; i++)
            {
                int l = 0;
                string currentType = "";
                waitingForVariable = true;
                if (words[i] == "var" || words[i] == "val")
                {
                    while (words[i + 3 + l] != "#")
                    {
                        if (waitingForMethod)
                        {
                            if (words[i + 3 + l] == "count")
                            {
                                currentType = "Int";
                                waitingForMethod = false;
                                l += 3;
                            }
                            else if (words[i + 3 + l] == "filter")
                            {
                                waitingForMethod = false;
                                waitingForVariable = true;
                                currentType = "List<Int>";
                                l += 2;
                            }
                            else
                            {
                                Console.WriteLine("Semantic error: incorrect var statement, waiting for method of list");
                                return;
                            }
                        }
                        else if (waitingForVariable)
                        {
                            if (variables.ContainsKey(words[i + 3 + l]))
                            {
                                if (currentType == "")
                                {
                                    currentType = variables[words[i + 3 + l++]];
                                }
                                else
                                {
                                    l++;
                                }
                                waitingForVariable = false;
                            }
                            else if (int.TryParse(words[i + 3 + l], out _))
                            {
                                if (currentType == "")
                                {
                                    currentType = "Int";
                                }
                                waitingForVariable = false;
                                l++;
                            }
                            else if (words[i + 3 + l] == "it")
                            {
                                waitingForVariable = false;
                                waitingForComparison = true;
                                l++;
                            }
                            else if (words[i + 3 + l] == "listOf")
                            {
                                waitingForVariable = false;
                                currentType = "List<Int>";
                                l++;
                            }
                            else
                            {
                                Console.WriteLine("Semantic error: incorrect var statement, waiting for variable");
                                return;
                            }
                        }
                        else if (waitingForComparison)
                        {
                            if (comparisonOperators.Contains(words[i + 3 + l]))
                            {
                                waitingForVariable = true;
                                waitingForComparison = false;
                                l++;
                            }
                            else
                            {
                                Console.WriteLine("Semantic error: incorrect var statement, waiting for comparison");
                                return;
                            }
                        }
                        else
                        {
                            if (words[i + 3 + l] == ".")
                            {
                                waitingForMethod = true;
                                l++;
                            }
                            else if (words[i + 3 + l] == "(" || words[i + 3 + l] == ",")
                            {
                                waitingForVariable = true;
                                l++;
                            }
                            else if (words[i + 3 + l] == ")" || words[i + 3 + l] == "]" || words[i + 3 + l] == "}" || words[i + 3 + l] == ">")
                            {
                                l++;
                            }
                            else if (words[i + 3 + l] == "[")
                            {
                                waitingForVariable = true;
                                currentType = "Int";
                                l++;
                            }
                            else if (words[i + 3 + l] == "<")
                            {
                                l += 2;
                            }
                            else if (ariphmeticalOperators.Contains(words[i + 3 + l]))
                            {
                                waitingForVariable = true;
                                l++;
                            }
                            else
                            {
                                Console.WriteLine("Semantic error: incorrect var statement");
                                return;
                            }
                        }
                    }
                     variables.Add(words[i + 1], currentType);
                }
            }

            for (int i = 0; i < words.Length; i++)
            {
                if (words[i] == "fun")
                {
                    if (variables.TryGetValue(words[i + 1], out _))
                    {
                        currentFunType = variables[words[i + 1]];
                    }
                    else
                    {
                        Console.WriteLine("Seems like code have been changes after syntax and before semantic analysises. Please try again.");
                        return;
                    }
                }


                if (words[i] == "if")
                {
                    int l = 0;
                    string currentType = "";
                    string firstVariable = "";
                    waitingForVariable = true;
                    while (!comparisonOperators.Contains(words[i + 2 + l]))
                    {
                        if (waitingForMethod)
                        {
                            if (words[i + 2 + l] == "count" && currentType == "List<Int>")
                            {
                                currentType = "Int";
                                waitingForMethod = false;
                                l += 3;
                            }
                            else
                            {
                                Console.WriteLine("Semantic error: incorrect if statement, waiting for method of list");
                                return;
                            }
                        }
                        else if (waitingForVariable)
                        {
                            if (variables.ContainsKey(words[i + 2 + l]))
                            {
                                currentType = variables[words[i + 2 + l++]];
                                waitingForVariable = false;
                            }
                            else if (int.TryParse(words[i + 2 + l], out _))
                            {
                                currentType = "Int";
                            }
                            else
                            {
                                Console.WriteLine("Semantic error: incorrect if statement");
                                return;
                            }
                        }
                        else
                        {
                            if (words[i + 2 + l] == ".")
                            {
                                waitingForMethod = true;
                                l++;
                            }
                            else
                            {
                                Console.WriteLine("Semantic error: incorrect if statement");
                                return;
                            }
                        }
                    }
                    firstVariable = currentType;
                    waitingForMethod = false;
                    waitingForVariable = true;
                    l++;
                    while (words[i + 2 + l] != ")")
                    {
                        if (waitingForMethod)
                        {
                            if (words[i + 2 + l] == "count" && currentType == "List<Int>")
                            {
                                currentType = "Int";
                                waitingForMethod = false;
                                l += 3;
                            }
                            else
                            {
                                Console.WriteLine("Semantic error: incorrect if statement, waiting for method of list");
                                return;
                            }
                        }
                        else if (waitingForVariable)
                        {
                            if (variables.ContainsKey(words[i + 2 + l]))
                            {
                                currentType = variables[words[i + 2 + l++]];
                                waitingForVariable = false;
                            }
                            else if (int.TryParse(words[i + 2 + l], out _))
                            {
                                currentType = "Int";
                                l++;
                            }
                            else
                            {
                                Console.WriteLine("Semantic error: incorrect if statement");
                                return;
                            }
                        }
                        else
                        {
                            if (words[i + 2 + l] == ".")
                            {
                                waitingForMethod = true;
                                l++;
                            }
                            else
                            {
                                Console.WriteLine("Semantic error: incorrect if statement");
                                return;
                            }
                        }
                    }
                    if (firstVariable != currentType)
                    {
                        Console.WriteLine("Semantic error: incorrect if statement, types of two variables don't match");
                        return;
                    }
                }

                if (words[i] == "return")
                {
                    int l = 0;
                    string currentType = "";
                    waitingForVariable = true;
                    waitingForMethod = false;
                    waitingForComparison = false;
                    List<string> types = new();
                    while (words[i + 1 + l] != "#" && words[i + 1 + l] != "}")
                    {
                        if (waitingForMethod)
                        {
                            if (words[i + 1 + l] == "count" && currentType == "List<Int>")
                            {
                                currentType = "Int";
                                waitingForMethod = false;
                                l += 3;
                            }
                            else
                            {
                                Console.WriteLine("Semantic error: incorrect return statement, waiting for method of list");
                                return;
                            }
                        }
                        else if (waitingForVariable)
                        {
                            if (variables.ContainsKey(words[i + 1 + l]))
                            {
                                if (currentType == "")
                                {
                                    currentType = variables[words[i + 1 + l++]];
                                }
                                else
                                {
                                    l++;
                                }
                                waitingForVariable = false;
                            }
                            else if (int.TryParse(words[i + 1 + l], out _))
                            {
                                if (currentType == "")
                                {
                                    currentType = "Int";
                                }
                                l++;
                            }
                            else
                            {
                                Console.WriteLine("Semantic error: incorrect return statement, waiting for variable");
                                return;
                            }
                        }
                        else
                        {
                            if (words[i + 1 + l] == ".")
                            {
                                waitingForMethod = true;
                                l++;
                            }
                            else if (words[i + 1 + l] == "(")
                            {
                                waitingForVariable = true;
                                l++;
                            }
                            else if (words[i + 1 + l] == ")")
                            {
                                l++;
                            }
                            else if (ariphmeticalOperators.Contains(words[i + 1 + l]))
                            {
                                types.Add(currentType);
                                currentType = "";
                                waitingForVariable = true;
                                l++;
                            }
                            else
                            {
                                Console.WriteLine("Semantic error: incorrect return statement");
                                return;
                            }
                        }

                    }
                    for (int f = 0; f < types.Count; f++)
                    {
                        if (f + 1 > types.Count)
                        {
                            if (types[f] != types[f + 1])
                            {
                                Console.WriteLine("Semantic error: incorrect return statement, trying to return variable of incorrect type (check separate parts of return)");
                                return;
                            }
                        }
                    }
                    if (currentFunType != currentType)
                    {
                        Console.WriteLine("Semantic error: incorrect return statement, trying to return variable of incorrect type");
                        return;
                    }
                }
            }
            Console.WriteLine("No semantic errors found.");
        }
    }
}
