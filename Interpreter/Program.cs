using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
namespace Interpreter
{
    class Program
    {
        private static Tokenizer tokenizer;
        private static StatementRecognizer statementRecognizer;
        private static Runner runner;
        private static string error = string.Empty;
        private static bool verbose = false;
        static void PrintError(string e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(e);
            Console.ForegroundColor = ConsoleColor.White;
        }

        static void Main(string[] args)
        {
            tokenizer = new Tokenizer();
            statementRecognizer = new StatementRecognizer();
            runner = new Runner(tokenizer);
            statementRecognizer.OnError += (e) =>
            {
                error = e;
            };
            if (args.Length == 1)
            {
                var filePath = args[0];
                if (Directory.Exists(filePath) || File.Exists(filePath))
                {
                    var file = File.ReadAllLines(filePath);
                    foreach (var sentence in file)
                        MainRoutine(sentence);
                    Console.Read();
                    return;
                }
            }

            Console.WriteLine("LAO Interpreter");
            Console.WriteLine("Enter 'quit' to exit.");
            var quitReg = new Regex(@"^QUIT${1}", RegexOptions.IgnoreCase);
            string line = string.Empty;
            while (!quitReg.IsMatch((line = Console.ReadLine())))
                   MainRoutine(line);
            
        }

        static void MainRoutine(string line)
        {
            var words = ParseInput(line);
            var sentenceTokens = words.Select(w => new WordTokens(w, tokenizer.Verify(w))).ToArray();
            var hasErrors = CheckForTokenErrors(sentenceTokens);
            if (!hasErrors)
            {
               var expresion =  statementRecognizer.Verify(sentenceTokens);
                if (expresion != null)
                    runner.Run(expresion);
                else
                    PrintError(error);
            }
        }

        static string[] ParseInput(string input)
        {
            var splitOptions = StringSplitOptions.RemoveEmptyEntries;
            Func<int, bool> IsEven = (index) => 
            {
                return index % 2 == 0;
            };
                return input.Split('"')
                            .Select((e, i) => IsEven(i) ? e.Split(new[] { ' ' }, splitOptions)
                                                        : new string[] { e = "\"" + e + "\"" })
                            .SelectMany(e => e).ToArray();
        }

       static bool CheckForTokenErrors(WordTokens[] sentenceTokens)
        { 
            var hasErrors = sentenceTokens?.Length < 1;
            if (!hasErrors && verbose) 
            foreach(var wordToken in sentenceTokens)
                if (wordToken.tokens?.Length > 0 && verbose)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Input: " + wordToken.value + " is a token");
                    Console.WriteLine("Possible Tokens:");
                    foreach (var token in wordToken.tokens)
                        Console.WriteLine(token.type);
                        Console.ForegroundColor = ConsoleColor.White;
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
