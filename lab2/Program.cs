using System.Runtime.CompilerServices;
using System.Text;

namespace lab2
{


    /*public class functionObject : parseObject
    {
        string name;
        string returnType;
        string[] arguments;

        public functionObject(string name, string returnType, string[] arguments)
        {
            this.name = name;
            this.returnType = returnType;
            this.arguments = arguments;
        }

        public functionObject(string name, string returnType)
        {
            this.name = name;
            this.returnType = returnType;
        }
    }

    public class variableObject : parseObject
    {
        string name;
        string type;

        public variableObject(string name, string type)
        {
            this.name = name;
            this.type = type;
        }
    }

    public class ifObject : parseObject
    {
        string condition;
        List<parseObject> body = new();
    }*/

    class Program
    {
        private static String[] separators = { ";", "{", "}", ">", "<", "|", "&", "~", ":", ".", "#", "##", ",", "(", ")", "[", "]", "()", "[]" };
        private static String[] operators = { "&&", "||", "++", "--", "==", "<=", ">=", "!=", "*", "/", "%", "=", "+=", "*=", "/=", "-=", "+", "-" };
        private static String[] keywords = { "print", "var", "val", "while", "if", "return", "count", "filter", "println", "listOf", "fun", "Int", "Array", "String", "List", "it", "else" };
        private static String[] words;

        private static List<String> variables = new();
        private static List<String> storedSeparators = new();
        private static List<String> storedOperators = new();
        private static List<String> storedKeywords = new();
        private static List<String> storedIntegers = new();
        private static List<String> storedStrings = new();
        private static List<String> storedChars = new();
        private static List<String> storedErrors = new();



        private static int lineNum;
        private static int charNum;

        static void Main()
        {
            String data = File.ReadAllText("file.txt") + "\n$~";

            for (int j = 0; j < operators.Length; j++)
                data = data.Replace(operators[j], " " + operators[j] + " ");

            for (int i = 0; i < separators.Length; i++)
                data = data.Replace(separators[i], " " + separators[i] + " ");

            var newData = new StringBuilder(data);
            for (int i = 0; i < newData.Length; i++)
            {
                if (newData[i] == '\n')
                    newData[i] = '#';
            }

            data = newData.ToString();
            data = data.Replace("\n", String.Empty);
            data = data.Replace("\r", String.Empty);
            data = data.Replace("\t", String.Empty);
            data = data.Replace("$", String.Empty);
            data = data.Replace("#", "\n");
            data = data.Replace("~", String.Empty);
            words = data.Split(' ');

            for (int i = 0; i < words.Length; i++)
            {
                if (words[i] != "" && words[i][0] == '\"')
                {
                    for (int j = 1; ; j++)
                    {
                        string temp = words[i + j];
                        if (temp == "")
                        {
                            continue;
                        }
                        words[i] = words[i] + " " + words[i + j];
                        words[i + j] = "";
                        if (temp == "\"" || temp[temp.Length - 1] == '\"')
                        {
                            break;
                        }
                    }
                }

                if ((words[i] == "=" && words[i + 2] == "=") || (words[i] == "+" && words[i + 2] == "+") || (words[i] == "-" && words[i + 2] == "-") || (words[i] == "*" && words[i + 2] == "=") || (words[i] == "/" && words[i + 2] == "=") || (words[i] == "+" && words[i + 2] == "=") || (words[i] == "-" && words[i + 2] == "=") || (words[i] == "&" && words[i + 2] == "&") || (words[i] == "|" && words[i + 2] == "|") || (words[i] == "[" && words[i + 2] == "]") || (words[i] == "-" && words[i + 2] == "-") || (words[i] == "<" && words[i + 2] == "=") || (words[i] == ">" && words[i + 2] == "="))
                {
                    words[i] += words[i + 2];
                    words[i + 2] = String.Empty;
                }
                else if (words[i] == "!" && words[i + 1] == "=")
                {
                    words[i] += words[i + 1];
                    words[i + 1] = String.Empty;
                }
            }

            for (int i = 0; i < words.Length; i++)
            {
                charNum += words[i].Length;

                if (words[i].Length > 0 && words[i][0] == '\n')
                {
                    int j = 0;
                    while (words[i].Length > j && words[i][j] == '\n')
                    {
                        lineNum++;
                        charNum = 0;
                        j++;
                    }
                    string replaceString = "\n";
                    for (int k = 1; k > j; k++)
                    {
                        replaceString += "\n";
                    }
                    words[i] = words[i].Replace(replaceString, "");
                }

                if (words[i] == "var" || words[i] == "val")
                {
                    if (!variables.Contains(words[i + 1]))
                    {
                        if (Char.IsDigit(words[i + 1][0]))
                        {
                            storedErrors.Add("Variable name can't start with digit on line " + (lineNum + 1) + " symbol " + (charNum + 5));
                            Console.WriteLine(storedErrors[0]);
                            return;
                        } 
                        else
                        {

                            variables.Add(words[i + 1]);
                        }
                    }
                    else
                    {
                        storedErrors.Add("Trying to declare existing variable " + words[i + 1] + " on line " + (lineNum + 1) + " symbol " + (charNum + 5));
                        Console.WriteLine(storedErrors[0]);
                        return;
                    }
                }

                if (words[i] == "fun")
                {
                    string funName;
                    string returnType = "";
                    string argument = "";

                    if (!variables.Contains(words[i + 1]))
                    {
                        variables.Add(words[i + 1]);
                        funName = words[i + 1];
                        if (words[i + 2] == "(" && words[i + 3] != ")" && words[i + 4] == ":")
                        {
                            if (!variables.Contains(words[i + 3]))
                            {
                                variables.Add(words[i + 3]);
                            }
                        }

                        int j = 3;
                        int k = 1;
                        while (words[i + j] != ")")
                        {
                            j++;
                        }
                        if (words[i + j + 2] == ":")
                        {
                            while (words[i + j + k + 2] != "{")
                            {
                                returnType += words[i + j + k++ + 2];
                            }
                        }
                    }
                    else
                    {
                        storedErrors.Add("Trying to declare existing function " + words[i + 1] + " on line " + (lineNum + 1) + " symbol " + (charNum + 5));
                        Console.WriteLine(storedErrors[0]);
                        return;
                    }
                }

                int u = 1;
                while (i - u > 0 && words[i - u] == "")
                {
                    u++;
                }

                if (operators.Contains(words[i]) && operators.Contains(words[i - u]))
                {
                    storedErrors.Add("Incorrect operator " + words[i - u] + words[i] + " on line " + (lineNum + 1) + " symbol " + (charNum + 1));
                    Console.WriteLine(storedErrors[0]);
                    return;
                }

                if (words[i] == "if" && (words[i + 2] != "(" || (words[i+2] == "(" && words[i+3] == ")")))
                {
                    storedErrors.Add("Incorrect if statement on line " + (lineNum + 1) + " symbol " + (charNum + 5));
                }
                CheckLexicalAnalyzer(words[i]);
            }

            if (storedErrors.Count > 0)
            {
                Console.WriteLine(storedErrors[0]);
            }
            else
            {
                if (storedKeywords.Count > 0)
                {
                    Console.WriteLine("------------------------------------");
                    Console.WriteLine("|             Keywords             |");
                    Console.WriteLine("------------------------------------");
                    for (int i = 0; i < storedKeywords.Count; i++)
                    {
                        Console.WriteLine(storedKeywords[i]);
                    }
                    Console.WriteLine("------------------------------------\n");
                }

                if (storedOperators.Count > 0)
                {
                    Console.WriteLine("------------------------------------");
                    Console.WriteLine("|            Operators             |");
                    Console.WriteLine("------------------------------------");
                    for (int i = 0; i < storedOperators.Count; i++)
                    {
                        Console.WriteLine(storedOperators[i]);
                    }
                    Console.WriteLine("------------------------------------\n");
                }

                if (storedSeparators.Count > 0)
                {
                    Console.WriteLine("------------------------------------");
                    Console.WriteLine("|            Separators            |");
                    Console.WriteLine("------------------------------------");
                    for (int i = 0; i < storedSeparators.Count; i++)
                    {
                        Console.WriteLine(storedSeparators[i]);
                    }
                    Console.WriteLine("------------------------------------\n");
                }

                if (storedIntegers.Count > 0)
                {
                    Console.WriteLine("------------------------------------");
                    Console.WriteLine("|             Integers             |");
                    Console.WriteLine("------------------------------------");
                    for (int i = 0; i < storedIntegers.Count; i++)
                    {
                        Console.WriteLine(storedIntegers[i]);
                    }
                    Console.WriteLine("------------------------------------\n");
                }

                if (storedStrings.Count > 0)
                {
                    Console.WriteLine("------------------------------------");
                    Console.WriteLine("|             Strings              |");
                    Console.WriteLine("------------------------------------");
                    for (int i = 0; i < storedStrings.Count; i++)
                    {
                        Console.WriteLine(storedStrings[i]);
                    }
                    Console.WriteLine("------------------------------------\n");
                }
                
                if (storedChars.Count > 0)
                {
                    Console.WriteLine("------------------------------------");
                    Console.WriteLine("|               Chars              |");
                    Console.WriteLine("------------------------------------");
                    for (int i = 0; i < storedChars.Count; i++)
                    {
                        Console.WriteLine(storedChars[i]);
                    }
                    Console.WriteLine("------------------------------------\n");
                }
                
                if (variables.Count > 0)
                {
                    Console.WriteLine("------------------------------------");
                    Console.WriteLine("|             Variables            |");
                    Console.WriteLine("------------------------------------");
                    for (int i = 0; i < variables.Count; i++)
                    {
                        Console.WriteLine(variables[i]);
                    }
                    Console.WriteLine("------------------------------------\n");
                }

                lab3.SyntaxAnalysis(words, variables);
            }
        }

        private static String Parse(String item)
        {
            StringBuilder str = new StringBuilder();

            if (CheckOperators(item) == true && !storedOperators.Contains(item))
                storedOperators.Add(item);
            else if (CheckSeparator(item) == true && item != "#" && !storedSeparators.Contains(item))
                storedSeparators.Add(item);
            else if (CheckKeywords(item) == true && !storedKeywords.Contains(item))
                storedKeywords.Add(item);
            else if (item.Equals("\r") || item.Equals("\n") || item.Equals("\r\n"))
                str.Append(item);
            return str.ToString();
        }

        private static bool CheckSeparator(String str) => separators.Contains(str);
        private static bool CheckOperators(String str) => operators.Contains(str);
        private static bool CheckKeywords(String str) => keywords.Contains(str);

        private static void CheckLexicalAnalyzer(String str)
        {
            StringBuilder token = new StringBuilder();
            bool isCheck = false;
            if (CheckOperators(str.ToString()))
            {
                Parse(str.ToString());
                return;
            }

            for (int i = 0; i < str.Length; i++)
            {
                try
                {
                    int intValue;
                    if (Int32.TryParse(str, out intValue) && !isCheck)
                    {
                        if (!storedIntegers.Contains(str))
                        {
                            storedIntegers.Add(str);
                            isCheck = true;
                        }
                        //Console.WriteLine(" (integerValue, <" + str + ">) ");
                    }
                    else if (str.Equals("\r") || str.Equals("\n") || str.Equals("\r\n")) { }


                    else if (CheckOperators(str[i].ToString()))
                    {
                        if (CheckOperators(str.ToString()))
                        {
                            Parse(str.ToString());
                        }
                        else
                        {
                            Parse(str[i].ToString());
                        }
                    }
                        

                    else if (CheckSeparator(str[i].ToString()))
                        //Console.WriteLine(Parse(str[i].ToString()));
                        Parse(str[i].ToString());

                    else if (str.Contains("\""))
                    {
                        if (str[i + 1].ToString() != "\"")
                            //Console.WriteLine();
                        do { i++; } while (str == "\"");
                        if (i == 1)
                            //Console.WriteLine(" (String, <" + str + ">) ");
                            storedStrings.Add(str);
                    }

                    else if (str.Contains("\'"))
                    {
                        if (str[i + 1].ToString() != "\'")
                            //Console.WriteLine();
                        do { i++; } while (str == "\'");
                        if (i == 1)
                            //Console.WriteLine(" (Char, <" + str + ">) ");
                            storedChars.Add(str);
                    }

                    else
                    {
                        token.Append(str);
                        try
                        {
                            if (keywords.Contains(token.ToString()))
                                //Console.WriteLine(Parse(token.ToString()));
                                Parse(token.ToString());
                            if (variables.Contains(token.ToString()))
                                //Console.WriteLine(Parse(token.ToString()));
                                Parse(token.ToString());

                            else
                            {
                                int intValu;

                                if (!separators.Contains(str[i].ToString()))
                                    if (!operators.Contains(str[i].ToString()) || !operators.Contains(str.ToString()))
                                        if (!keywords.Contains(str.ToString()))
                                            if (!variables.Contains(str.ToString()))
                                                if (!str.Contains("\"") || !str.Contains("\'"))
                                                    if (!Int32.TryParse(str[i].ToString(), out intValu) && !isCheck)
                                                        if (!str.Equals("\r") || !str.Equals("\n") || !str.Equals('\r') || !str.Equals('\n') || !str.Equals("\r\n") || !str.Equals("#"))
                                                        {
                                                            storedErrors.Add(" Unknown token " + str + " detected on line " + (lineNum + 1) + " symbol " + (charNum + 1));
                                                            isCheck = true;
                                                        }
                            }
                        }
                        catch (Exception) { }
                        token.Remove(i, i);
                    }
                }
                catch (Exception) { }
            }
        }
    }

}
