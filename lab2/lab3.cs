namespace lab2
{
    public class parseObject
    {
        public string name;
        public string value;
        public List<parseObject> children;

        public parseObject(string name, string value, List<parseObject> children)
        {
            this.name = name;
            this.value = value;
            this.children = children;
        }
    }

    internal class lab3
    {
        private static Stack<parseObject> stack = new();
        static parseObject startObject = new parseObject("Analyzing file", "file.txt", new List<parseObject>());
        private static String[] comparisonOperators = { "==", "<=", ">=", "!=", "<", ">" };
        private static String[] ariphmeticalOperators = { "+", "-", "*", "/", "=" };
        private static String[] endOfVar = { "return", "val", "var", "println" };
        private static String[] varSeparators = { ".", "filter", "it", "listOf", "Int", ",", "[", "]", "count", "()", "{", "}", "(", ")" };

        private static bool wasIfRecently = false;
        private static bool correctElse = false;

        public static void SyntaxAnalysis(String[] words, List<String> variables)
        {
            stack.Push(startObject);
            var newWords = words.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            words = newWords;


            for (int i = 0; i < words.Length; i++)
            {
                if (words[i] == "fun")
                {
                    Dictionary<String, String> arguments = new();
                    parseObject tempStack = stack.Peek();
                    parseObject newObject = new parseObject("Function declaration", words[i], new List<parseObject>());
                    foreach (char c in words[i + 1])
                    {
                        if (!Char.IsLetterOrDigit(c) && c != '_')
                        {
                            Console.WriteLine("Incorrect fun declaration: incorrect name");
                            return;
                        }
                    }
                    parseObject funNameObject = new parseObject("Function name", words[i + 1], new List<parseObject>());
                    if (words[i + 2] == "(" && words[i + 3] != ")")
                    {
                        if (words[i + 4] == ":")
                        {
                            int m = 0;
                            string tempType = "";
                            while (words[i + 5 + m] != ")")
                            {
                                tempType += words[i + 5 + m++];
                            }
                            arguments.Add(words[i + 3], tempType);
                            if (words[i + 5 + m] == ")" && words[i + 6 + m] == ":")
                            {
                                //newObject.children.Add(new parseObject("Function type", words[i + 8 + m], new List<parseObject>()));
                            } 
                            else if (words[i + 5 + m] == ")" && words[i + 6 + m] == "{")
                            {
                                //newObject.children.Add(new parseObject("Function type", "void", new List<parseObject>()));
                            }
                            else
                            {
                                Console.WriteLine("Incorrect fun declaration: type");
                                return;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Incorrect fun declaration: strange");
                            return;
                        }
                    }
                    else if (words[i + 2] == "(" && words[i + 3] == ")" && (words[i + 4] == ":" || words[i + 4] == "}"))
                    {
                        int k = 0;
                        string returnType = "";
                        while (words[i + 5 + k] != "{")
                        {
                            returnType += words[i + 5 + k++];
                        }
                        newObject.children.Add(new parseObject("Function type", returnType, new List<parseObject>()));
                    }
                    else if (words[i + 2] == "(" && words[i + 3] == ")") { }
                    else
                    {
                        Console.WriteLine("Incorrect fun declaration");
                        return;
                    }
                    newObject.children.Add(funNameObject);
                    if (arguments.Count > 0)
                    {
                        foreach (var argument in arguments)
                        {
                            newObject.children.Add(new parseObject("Function argument " + argument.Key, argument.Value + " type", new List<parseObject>()));
                        }
                    }
                    tempStack.children.Add(newObject);
                    stack.Push(newObject);
                }

                if (words[i] == "if")
                {
                    if (words[i + 1] != "(" || (words[i + 1] == "(" && words[i + 2] == ")"))
                    {
                        Console.WriteLine("Incorrect if statement");
                        return;
                    }
                    else
                    {
                        parseObject tempObject = stack.Peek();
                        parseObject newObject = new parseObject("Keyword", "if", new List<parseObject>());
                        int l = 0;
                        string tempVariable = "";
                        while (!comparisonOperators.Contains(words[i + 2 + l]))
                        {
                            tempVariable += words[i + 2 + l++];
                        }
                        if (tempVariable == "")
                        {
                            Console.WriteLine("Incorrect if statement: noTempVariable");
                            return;
                        }
                        parseObject ifVariable = new parseObject("Variable", tempVariable, new List<parseObject>());
                        if (!comparisonOperators.Contains(words[i + 2 + l]))
                        {
                            Console.WriteLine("Incorrect if statement: operator");
                            return;
                        }
                        parseObject ifOperator = new parseObject("Operator", words[i + 2 + l], new List<parseObject>());
                        string tempVariable2 = "";
                        while (words[i + 3 + l] != ")")
                        {
                            tempVariable2 += words[i + 3 + l++];
                        }
                        if (tempVariable2 == "")
                        {
                            Console.WriteLine("Incorrect if statement: no second variable");
                            return;
                        }
                        parseObject ifVariable2 = new parseObject("Variable/Constant", tempVariable2, new List<parseObject>());
                        parseObject ifBody = new parseObject("Body of if", "", new List<parseObject>());
                        wasIfRecently = true;

                        newObject.children.Add(ifVariable);
                        newObject.children.Add(ifOperator);
                        newObject.children.Add(ifVariable2);
                        newObject.children.Add(ifBody);
                        tempObject.children.Add(newObject);
                        stack.Push(newObject);
                    }
                }

                if (words[i] == "else")
                {
                    if (correctElse)
                    {
                        parseObject tempObject = stack.Peek();
                        parseObject newObject = new parseObject("Keyword", "else", new List<parseObject>());
                        tempObject.children.Add(newObject);
                        if (words[i + 1] != "if")
                        {
                            stack.Push(newObject);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Incorrect else statement: no if");
                        return;
                    }
                }

                if (words[i] == "println")
                {
                    wasIfRecently = false;
                    parseObject tempObject = stack.Peek();
                    parseObject newObject = new parseObject("Keyword", "println", new List<parseObject>());
                    int j = 0;
                    string outputString = "";
                    while (words[i + 2 + j] != ")")
                    {
                        outputString += words[i + 2 + j++];
                    }
                    newObject.children.Add(new parseObject("Output string", outputString, new List<parseObject>()));
                    tempObject.children.Add(newObject);
                }

                if (words[i] == "var" || words[i] == "val")
                {

                    parseObject tempObject = stack.Peek();
                    parseObject newObject = new parseObject("Variable initialisation", words[i], new List<parseObject>());
                    parseObject varName = new parseObject("Variable name", words[i + 1], new List<parseObject>());
                    newObject.children.Add(varName);
                    if (words[i + 2] == "=")
                    {
                        int j = 1;
                        string varValue = "";
                        while (!endOfVar.Contains(words[i + 2 + j]))
                        {
                            if (int.TryParse(words[i + 2 + j], out _) || variables.Contains(words[i + 2 + j]) || comparisonOperators.Contains(words[i + 2 + j]) || varSeparators.Contains(words[i + 2 + j]) || ariphmeticalOperators.Contains(words[i + 2 + j]))
                            {
                                varValue += words[i + 2 + j++];
                            }
                            else
                            {
                                Console.WriteLine($"Incorrect variable initialization: {words[i + 2 + j]}");
                                return;
                            }
                        }
                        parseObject variableValue = new parseObject("Variable value", varValue, new List<parseObject>());
                        newObject.children.Add(variableValue);
                    }
                    else
                    {
                        Console.WriteLine("Incorrect variable initialization");
                        return;
                    }
                    tempObject.children.Add(newObject);
                }

                if (words[i] == "return")
                {
                    if (variables.Contains(words[i + 1]))
                    {
                        parseObject tempObject = stack.Peek();
                        parseObject newObject = new parseObject("Keyword", "return", new List<parseObject>());
                        int j = 0;
                        string tempVariable = "";
                        while (words[i + 1 + j] != "}")
                        {
                            if (variables.Contains(words[i + 1 + j]) || ariphmeticalOperators.Contains(words[i + 1 + j]) 
                                || words[i + 1 + j] == "()" || words[i + 1 + j] == "(" || words[i + 1 + j] == ")" || words[i + 1 + j] == "[" && words[i + 1 + j] == "]")
                            {
                                tempVariable += words[i + 1 + j++];
                            }
                            else
                            {
                                Console.WriteLine($"Incorrect return statement: {words[i + 1 + j]}");
                                return;
                            }
                        }
                        parseObject variableObject = new parseObject("return variable", tempVariable, new List<parseObject>());
                        newObject.children.Add(variableObject);
                        tempObject.children.Add(newObject);
                    }
                    else
                    {
                        Console.WriteLine("Incorrect return statement");
                        return;
                    }
                }

                if (words[i] == "}" && stack.Count > 1 && words[i - 1] != "pivot")
                {
                    stack.Pop();
                    if (i + 1 < words.Length)
                    {
                        if (wasIfRecently && words[i + 1] == "else")
                        {
                            correctElse = true;
                        }
                        else
                        {
                            wasIfRecently = false;
                        }
                    }
                }
            }

            Console.WriteLine($"{startObject.name}: {startObject.value}");
            OutputChildrenOfNode(startObject, 1);
        }

        public static void OutputChildrenOfNode(parseObject parseObject, int numOfTubs)
        {
            for (int i = 0; i < parseObject.children.Count; i++)
            {
                string tabString = "";
                for (int j = 0; j < numOfTubs; j++)
                {
                    tabString += "\t";
                }
                Console.WriteLine($"{tabString}{parseObject.children[i].name}: {parseObject.children[i].value}");
                OutputChildrenOfNode(parseObject.children[i], numOfTubs + 1);
            }
        }
    }
}
