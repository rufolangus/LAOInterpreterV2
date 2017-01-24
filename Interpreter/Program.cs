using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Interpreter
{
    class Program
    {
        private static Tokenizer tokenizer;
        private static StatementRecognizer statementRecognizer;
        static void Main(string[] args)
        {
            tokenizer = new Tokenizer();
            statementRecognizer = new StatementRecognizer();
            Console.WriteLine("LAO Interpreter");
            Console.WriteLine("Enter 'quit' to exit.");
            var quitReg = new Regex(@"^QUIT${1}", RegexOptions.IgnoreCase);
            string line = string.Empty;
            while (!quitReg.IsMatch((line = Console.ReadLine())))
            {
                var words = ParseInput(line);
                var sentenceTokens = words.Select(w => new WordTokens(w, tokenizer.Verify(w))).ToArray();
                var hasErrors = CheckForTokenErrors(sentenceTokens);
                if (!hasErrors)
                    statementRecognizer.Verify(sentenceTokens);
            }
        }

        static string [] ParseInput(string input)
        {
                return input.Split('"')
                            .Select((element, index) => index % 2 == 0 ? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                                                       : new string[] { element = "\"" + element + "\"" })
                            .SelectMany(element => element).ToArray();
        }

       static bool CheckForTokenErrors(WordTokens[] sentenceTokens)
        {
            
            var hasErrors = sentenceTokens == null || sentenceTokens.Length < 1;
            if (!hasErrors) 
            foreach(var wordToken in sentenceTokens)
                if (wordToken.tokens?.Length > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Input: " + wordToken.value + " is a token");
                    Console.WriteLine("Possible Tokens:");
                    foreach (var token in wordToken.tokens)
                        Console.WriteLine(token.type);

                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: " + wordToken.value + " not a token.");
                    Console.ForegroundColor = ConsoleColor.White;
                    return true;
                }
            return hasErrors;
        }
    }
}
