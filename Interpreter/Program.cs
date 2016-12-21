using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            string line = string.Empty;
            while ((line = Console.ReadLine()) != "quit")
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
