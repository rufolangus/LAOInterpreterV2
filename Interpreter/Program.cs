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
        static void Main(string[] args)
        {

            tokenizer = new Tokenizer();
            Console.WriteLine("LAO Interpreter");
            Console.WriteLine("Enter 'quit' to exit.");
            var regex = @"^(q|Q)(u|U)(i|I)(t|T)${1}";
            var quitReg = new Regex(regex);
            string line = string.Empty;
            while (!quitReg.IsMatch((line = Console.ReadLine())))
            {
                
                var tokens = tokenizer.Verify(line);
                if (tokens != null && tokens.Length > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Input: " + line + " is a token");
                    Console.WriteLine("Possible Tokens:");
                    foreach (var token in tokens)
                        Console.WriteLine(token.type);

                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: " + line + " not a token.");
                }
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}
