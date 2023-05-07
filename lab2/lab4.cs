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
        private static string currentFunType = "";

        private static bool waitingForMethod = false;

        public static void SemanticAnalisys(String[] words, Dictionary<String, String> variables)
        {
            var newWords = words.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            words = newWords;

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
                    while (!comparisonOperators.Contains(words[i + 2 + l]))
                    {
                        if (waitingForMethod)
                        {
                            if (words[i + 2 + l] == "count")
                            {
                                currentType = "Int";
                                waitingForMethod = false;
                                l += 2;
                            }
                            else
                            {
                                Console.WriteLine("Semantic error: incorrect if statement, waiting for method");
                                return;
                            }
                        }
                        else
                        {
                            if (variables.ContainsKey(words[i + 2 + l]))
                            {
                                currentType = variables[words[i + 2 + l++]];
                            }
                            else if (words[i + 2 + l] == ".")
                            {
                                waitingForMethod = true;
                                currentType = variables[words[i + 2 + l++]];
                            }
                            else
                            {
                                Console.WriteLine("Semantic error: incorrect if statement");
                                return;
                            }
                        }
                        
                    }
                    waitingForMethod = false;
                }
            }
        }
    }
}
