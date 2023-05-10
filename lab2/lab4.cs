namespace lab2
{
    internal class lab4
    {
        public static Dictionary<String, String> variables = new Dictionary<String, String>();
        public static Dictionary<String, bool> isVarUsed = new Dictionary<String, bool>();
        public static Dictionary<String, int> variableScope = new Dictionary<String, int>();

        private static String[] comparisonOperators = { "==", "<=", ">=", "!=", "<", ">" };
        private static String[] ariphmeticalOperators = { "+", "-", "*", "/" };
        private static string currentFunType = "";

        private static bool waitingForMethod = false;
        private static bool waitingForVariable = false;
        private static bool waitingForComparison = false;

        private static int currentScope = 0;

        public static void SemanticAnalisys(String[] words, Dictionary<String, String> variables)
        {
            var newWords = words.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            words = newWords;
            variableScope.Add("items", 0);
            variableScope.Add("quicksort", 0);

            foreach (var vars in variables.Keys) 
            {
                isVarUsed.Add(vars, false);
            }

            for (int i = 0; i < words.Length; i++)
            {
                if (words[i] == "{")
                {
                    currentScope += 1;
                    continue;
                }
                else if (words[i] == "}")
                {
                    currentScope -= 1;
                    continue;
                }

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
                                isVarUsed[words[i + 3 + l]] = true;
                                if (!variableScope.TryGetValue(words[i + 3 + l], out _) || currentScope < variableScope[words[i + 3 + l]])
                                {
                                    Console.WriteLine("Semantic error: incorrect var statement, check scope of used var " + words[i + 3 + l]);
                                    return;
                                }
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
                                if (words[i + 3 + l] == "/" && words[i + 4 + l] == "0")
                                {
                                    Console.WriteLine("Semantic error: incorrect var statement, dividing by zero");
                                    return;
                                }
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
                    isVarUsed.Add(words[i + 1], false);
                    variableScope.Add(words[i + 1], currentScope);
                }
            }

            for (int i = 0; i < words.Length; i++)
            {
                if (words[i] == "{")
                {
                    currentScope += 1;
                    continue;
                }
                else if (words[i] == "}")
                {
                    currentScope -= 1;
                    continue;
                }

                if (words[i] == "fun")
                {
                    if (variables.TryGetValue(words[i + 1], out _))
                    {
                        currentFunType = variables[words[i + 1]];
                        variableScope.Clear();
                        variableScope.Add(words[i + 1], 0);
                        variableScope.Add("items", 0);
                    }
                    else
                    {
                        Console.WriteLine("Seems like code have been changes after syntax and before semantic analysises. Please try again.");
                        return;
                    }
                }

                if (words[i] == "var" || words[i] == "val")
                {
                    variableScope.Add(words[i + 1], currentScope);
                }

                if (words[i] == "println")
                {
                    if (isVarUsed.TryGetValue(words[i + 2], out _))
                    {
                        isVarUsed[words[i + 2]] = true;
                    }
                    
                    if (words[i + 2][0] != '"')
                    {
                        if (!variableScope.TryGetValue(words[i + 2], out _) || currentScope < variableScope[words[i + 2]])
                        {
                            Console.WriteLine("Semantic error: incorrect var statement, check scope of used var " + words[i + 2]);
                            return;
                        }
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
                                isVarUsed[words[i + 2 + l]] = true;
                                if (!variableScope.TryGetValue(words[i + 2 + l], out _) || currentScope < variableScope[words[i + 2 + l]])
                                {
                                    Console.WriteLine("Semantic error: incorrect var statement, check scope of used var " + words[i + 2 + l]);
                                    return;
                                }
                                currentType = variables[words[i + 2 + l++]];
                                waitingForVariable = false;
                            }
                            else if (int.TryParse(words[i + 2 + l], out _))
                            {
                                if (words[i + 2 + l] == "/" && words[i + 3 + l] == "0")
                                {
                                    Console.WriteLine("Semantic error: incorrect if statement, dividing by zero");
                                    return;
                                }
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
                                isVarUsed[words[i + 2 + l]] = true;
                                if (!variableScope.TryGetValue(words[i + 2 + l], out _) || currentScope < variableScope[words[i + 2 + l]])
                                {
                                    Console.WriteLine("Semantic error: incorrect var statement, check scope of used var " + words[i + 2 + l]);
                                    return;
                                }
                                currentType = variables[words[i + 2 + l++]];
                                waitingForVariable = false;
                            }
                            else if (int.TryParse(words[i + 2 + l], out _))
                            {
                                if (words[i + 2 + l] == "/" && words[i + 3 + l] == "0")
                                {
                                    Console.WriteLine("Semantic error: incorrect var statement, dividing by zero");
                                    return;
                                }
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
                                isVarUsed[words[i + 1 + l]] = true;
                                if (!variableScope.TryGetValue(words[i + 1 + l], out _) || currentScope < variableScope[words[i + 1 + l]])
                                {
                                    Console.WriteLine("Semantic error: incorrect return statement, check scope of used var " + words[i + 1 + l]);
                                    return;
                                }
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
                                if (words[i + 3 + l] == "/" && words[i + 4 + l] == "0")
                                {
                                    Console.WriteLine("Semantic error: incorrect return statement, dividing by zero");
                                    return;
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
                                if (words[i + 1 + l] == "/" && words[i + 2 + l] == "0")
                                {
                                    Console.WriteLine("Semantic error: incorrect return statement, dividing by zero");
                                    return;
                                }
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

            foreach (var key in isVarUsed.Keys)
            {
                if (!isVarUsed[key] && key != "main") 
                {
                    Console.WriteLine("Semantic error: unused variable " + key);
                    return;
                }
            }
            Console.WriteLine("No semantic errors found.");
        }
    }
}
