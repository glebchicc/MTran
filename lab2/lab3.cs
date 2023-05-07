using System.Globalization;

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
        private static String[] endOfVar = { "return", "val", "var", "println", "#" };
        private static String[] varSeparators = { ".", "filter", "it", "listOf", "Int", ",", "[", "]", "count", "()", "{", "}", "(", ")" };
        public static Dictionary<String, String> variableTypes = new();

        private static bool wasIfRecently = false;
        private static bool correctElse = false;
        private static bool wasComaBefore = true;

        private static int lineNum;
        private static int charNum;

        public static void SyntaxAnalysis(String[] words, List<String> variables)
        {
            stack.Push(startObject);
            var newWords = words.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            words = newWords;


            for (int i = 0; i < words.Length; i++)
            {
                charNum += words[i].Length;

                if (words[i] == "#")
                {
                    lineNum++;
                    charNum = 0;
                    continue;
                }

                if (words[i] == "fun")
                {
                    if (stack.Count <= 1)
                    {
                        Dictionary<String, String> arguments = new();
                        parseObject tempStack = stack.Peek();
                        parseObject newObject = new parseObject("Function declaration", words[i], new List<parseObject>());
                        foreach (char c in words[i + 1])
                        {
                            if (!Char.IsLetterOrDigit(c) && c != '_')
                            {
                                Console.WriteLine("Incorrect fun declaration: incorrect name on line" + (lineNum + 1) + " symbol " + (charNum + 1));
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
                                arguments.Add(words[i + 3], "");
                                variableTypes.Add(words[i + 3], tempType);
                                if (words[i + 5 + m] == ")" && words[i + 6 + m] == ":")
                                {
                                    string funType = "";
                                    int tick = 0;
                                    while (words[i + 7 + m] != "{")
                                    {
                                        funType += words[i + 7 + m++];
                                        tick++;
                                        if (tick > 6)
                                        {
                                            Console.WriteLine("Incorrect fun declaration: type ( '}' bracket on line" + (lineNum + 1) + " symbol " + (charNum + 1));
                                            return;
                                        }
                                    }
                                    //newObject.children.Add(new parseObject("Function type", funType, new List<parseObject>()));
                                    variableTypes.Add(words[i + 1], funType);
                                }
                                else if (words[i + 5 + m] == ")" && words[i + 6 + m] == "{")
                                {
                                    variableTypes.Add(words[i + 1], "void");
                                    //newObject.children.Add(new parseObject("Function type", "void", new List<parseObject>()));
                                }
                                else
                                {
                                    Console.WriteLine("Incorrect fun declaration: type on line" + (lineNum + 1) +  " symbol " + (charNum + 1));
                                    return;
                                }
                            }
                            else
                            {
                                Console.WriteLine("Incorrect fun declaration: strange on line" + (lineNum + 1) + " symbol " + (charNum + 1));
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
                            variableTypes.Add(words[i + 1], returnType);
                            //newObject.children.Add(new parseObject("Function type", returnType, new List<parseObject>()));
                        }
                        else if (words[i + 2] == "(" && words[i + 3] == ")") { }
                        else
                        {
                            Console.WriteLine("Incorrect fun declaration: check () brackets on line" + (lineNum + 1) + " symbol " + (charNum + 1));
                            return;
                        }
                        newObject.children.Add(funNameObject);
                        if (arguments.Count > 0)
                        {
                            foreach (var argument in arguments)
                            {
                                //variableTypes.Add(argument.Key, argument.Value);
                                newObject.children.Add(new parseObject("Function argument", argument.Key, new List<parseObject>()));
                            }
                        }
                        tempStack.children.Add(newObject);
                        stack.Push(newObject);
                    }
                    else
                    {
                        Console.WriteLine("Incorrect fun statement: fun inside of fun on line" + (lineNum + 1) + " symbol " + (charNum + 1));
                        return;
                    }
                }

                if (words[i] == "while")
                {
                    if (words[i + 1] != "(" || (words[i + 1] == "(" && words[i + 2] == ")"))
                    {
                        Console.WriteLine("Incorrect while statement: check () brackets on line" + (lineNum + 1) + " symbol " + (charNum + 1));
                        return;
                    }
                    else
                    {
                        parseObject tempObject = stack.Peek();
                        parseObject newObject = new parseObject("Keyword", "while", new List<parseObject>());
                        int l = 0;
                        string tempVariable = "";
                        while (!comparisonOperators.Contains(words[i + 2 + l]))
                        {
                            tempVariable += words[i + 2 + l++];
                        }
                        if (tempVariable == "")
                        {
                            Console.WriteLine("Incorrect while statement: noFirstVariable on line" + (lineNum + 1) + " symbol " + (charNum + 1));
                            return;
                        }
                        parseObject whileVariable = new parseObject("Variable", tempVariable, new List<parseObject>());
                        if (!comparisonOperators.Contains(words[i + 2 + l]))
                        {
                            Console.WriteLine("Incorrect while statement: operator on line" + (lineNum + 1) + " symbol " + (charNum + 1));
                            return;
                        }
                        parseObject whileOperator = new parseObject("Operator", words[i + 2 + l], new List<parseObject>());
                        string tempVariable2 = "";
                        while (words[i + 3 + l] != ")")
                        {
                            tempVariable2 += words[i + 3 + l++];
                        }
                        if (tempVariable2 == "")
                        {
                            Console.WriteLine("Incorrect while statement: no second variable on line" + (lineNum + 1) + " symbol " + (charNum + 1));
                            return;
                        }
                        parseObject whileVariable2 = new parseObject("Variable/Constant", tempVariable2, new List<parseObject>());
                        parseObject whileBody = new parseObject("Body of while", "", new List<parseObject>());

                        newObject.children.Add(whileVariable);
                        newObject.children.Add(whileOperator);
                        newObject.children.Add(whileVariable2);
                        newObject.children.Add(whileBody);
                        tempObject.children.Add(newObject);
                        stack.Push(newObject);
                    }
                }

                if (words[i] == "if")
                {
                    if (words[i + 1] != "(" || (words[i + 1] == "(" && words[i + 2] == ")"))
                    {
                        Console.WriteLine("Incorrect if statement: check () brackets on line" + (lineNum + 1) + " symbol " + (charNum + 1));
                        return;
                    }
                    else
                    {
                        parseObject tempObject = stack.Peek();
                        parseObject newObject = new parseObject("Keyword", "if", new List<parseObject>());
                        int l = 0;
                        string tempVariable = "";
                        while (!comparisonOperators.Contains(words[i + 2 + l]) )
                        {
                            tempVariable += words[i + 2 + l++];
                        }
                        if (tempVariable == "")
                        {
                            Console.WriteLine("Incorrect if statement: noFirstVariable on line" + (lineNum + 1) + " symbol " + (charNum + 1));
                            return;
                        }
                        parseObject ifVariable = new parseObject("Variable", tempVariable, new List<parseObject>());
                        if (!comparisonOperators.Contains(words[i + 2 + l]))
                        {
                            Console.WriteLine("Incorrect if statement: operator on line" + (lineNum + 1) + " symbol " + (charNum + 1));
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
                            Console.WriteLine("Incorrect if statement: no second variable on line" + (lineNum + 1) + " symbol " + (charNum + 1));
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
                        Console.WriteLine("Incorrect else statement: no if on line" + (lineNum + 1) + " symbol " + (charNum + 1));
                        return;
                    }
                }

                if (words[i] == "for")
                {
                    if (words[i + 1] != "(")
                    {
                        Console.WriteLine("Incorrect for statement: check () brackets on line" + (lineNum + 1) + " symbol " + (charNum + 1));
                        return;
                    }
                    else
                    {
                        variables.Add(words[i + 2]);
                        if (words[i + 3] != "in")
                        {
                            Console.WriteLine("Incorrect for statement: no in on line" + (lineNum + 1) + " symbol " + (charNum + 1));
                            return;
                        }
                        else
                        {
                            parseObject tempObject = stack.Peek();
                            parseObject newObject = new parseObject("Keyword", "for", new List<parseObject>());
                            parseObject variableObject = new parseObject("For variable", words[i + 2], new List<parseObject>());
                            int l = 0;
                            string tempRange = "";
                            while (words[i + 4 + l] != ")")
                            {
                                if (variables.Contains(words[i + 4 + l]) || words[i + 4 + l] == "." || int.TryParse(words[i + 4 + l], out _))
                                {
                                    tempRange += words[i + 4 + l++];
                                }
                                else
                                {
                                    Console.WriteLine("Incorrect for statement: incorrect range on line" + (lineNum + 1) + " symbol " + (charNum + 1));
                                    return;
                                }
                            }
                            parseObject rangeObject = new parseObject("Range of for", tempRange, new List<parseObject>());
                            parseObject bodyObject = new parseObject("Body of for", "", new List<parseObject>());
                            newObject.children.Add(variableObject);
                            newObject.children.Add(rangeObject);
                            newObject.children.Add(bodyObject);
                            tempObject.children.Add(newObject);
                            stack.Push(newObject);
                         }
                    }
                }

                if (words[i] == "filter")
                {
                    if (words[i + 1] != "{")
                    {
                        Console.WriteLine("Incorrect filter statement: check brackets on line" + (lineNum + 1) + " symbol " + (charNum + 1));
                        return;
                    }
                    else
                    {
                        if (words[i + 2] == "it")
                        {
                            parseObject tempObject = stack.Peek();
                            parseObject newObject = new parseObject("Keyword", "filter", new List<parseObject>());
                            parseObject operatorObject = new parseObject("Operator of filter", words[i + 3], new List<parseObject>());
                            int l = 1;
                            string tempVariable = "";
                            while (words[i + 3 + l] != "}")
                            {
                                tempVariable += words[i + 3 + l++];
                            }
                            if (tempVariable == "")
                            {
                                Console.WriteLine("Incorrect filter statement: no filter variable on line" + (lineNum + 1) + " symbol " + (charNum + 1));
                                return;
                            }
                            parseObject variableObject = new parseObject("Filter variable", tempVariable, new List<parseObject>());
                            newObject.children.Add(operatorObject);
                            newObject.children.Add(variableObject);
                            var childOfStack = tempObject.children[^1];
                            childOfStack.children.Add(newObject);
                        }
                        else
                        {
                            Console.WriteLine("Incorrect filter statement: no it variable on line" + (lineNum + 1) + " symbol " + (charNum + 1));
                            return;
                        }
                    }
                }

                if (words[i] == "listOf")
                {
                    if (words[i + 1] != "<" || words[i + 3] != ">")
                    {
                        Console.WriteLine("Incorrect filter statement: check <> brackets on line" + (lineNum + 1) + " symbol " + (charNum + 1));
                        return;
                    }
                    else
                    {
                        parseObject tempObject = stack.Peek();
                        parseObject newObject = new parseObject("Keyword", "listOf", new List<parseObject>());
                        //parseObject listType = new parseObject("List type", words[i + 2], new List<parseObject>());
                        if (words[i + 4] != "(")
                        {
                            Console.WriteLine("Incorrect list statement: check () brackets on line" + (lineNum + 1) + " symbol " + (charNum + 1));
                            return;
                        }
                        else
                        {
                            int l = 1;
                            while (words[i + 4 + l] != ")")
                            {
                                if (words[i + 2] == "Int")
                                {
                                    if (int.TryParse(words[i + 4 + l], out _) || variables.Contains(words[i + 4 + l]))
                                    {
                                        parseObject listItem = new parseObject("List item", words[i + 4 + l++], new List<parseObject>());
                                        newObject.children.Add(listItem);
                                        wasComaBefore = false;
                                    }
                                    else if (words[i + 4 + l] == ",")
                                    {
                                        if (!wasComaBefore)
                                        {
                                            l++;
                                            wasComaBefore = true;
                                        }
                                        else
                                        {
                                            Console.WriteLine("Incorrect list statement: check ,, on line" + (lineNum + 1) + " symbol " + (charNum + 1));
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Incorrect list statement: incorrect list initialization on line" + (lineNum + 1) + " symbol " + (charNum + 1));
                                        return;
                                    }
                                }
                            }
                            wasComaBefore = true;
                        }
                        var childOfStack = tempObject.children[^1];
                        childOfStack.children.Add(newObject);
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
                                Console.WriteLine($"Incorrect variable initialization: {words[i + 2 + j]} on line" + (lineNum + 1) + " symbol " + (charNum + 1));
                                return;
                            }
                        }
                        parseObject variableValue = new parseObject("Variable value", varValue, new List<parseObject>());
                        newObject.children.Add(variableValue);
                    }
                    else
                    {
                        Console.WriteLine("Incorrect variable initialization on line" + (lineNum + 1) + " symbol " + (charNum + 1));
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
                        while (words[i + 1 + j] != "}" && words[i + 1 + j] != "#")
                        {
                            if (variables.Contains(words[i + 1 + j]) || ariphmeticalOperators.Contains(words[i + 1 + j]) 
                                || words[i + 1 + j] == "()" || words[i + 1 + j] == "(" || words[i + 1 + j] == ")" || words[i + 1 + j] == "[" && words[i + 1 + j] == "]")
                            {
                                tempVariable += words[i + 1 + j++];
                            }
                            else
                            {
                                Console.WriteLine($"Incorrect return statement: {words[i + 1 + j]} on line" + (lineNum + 1) + " symbol " + (charNum + 1));
                                return;
                            }
                        }
                        parseObject variableObject = new parseObject("return variable", tempVariable, new List<parseObject>());
                        newObject.children.Add(variableObject);
                        tempObject.children.Add(newObject);
                    }
                    else
                    {
                        Console.WriteLine("Incorrect return statement on line" + (lineNum + 1) + " symbol " + (charNum + 1));
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
