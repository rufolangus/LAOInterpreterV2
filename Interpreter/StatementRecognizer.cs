using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions; 

namespace Interpreter
{
    public class StatementRecognizer
    {
        StatementRegex[] statementRegexes;

        /* Token Type ENUM:
         * CommentKeyword                   = 0
         * Assignment Operator              = 1
         * PrinterKeyword                   = 2
         * ReadKeyword                      = 3
         * IfKeyword                        = 4
         * EndKeyWord                       = 5
         * IntegerVariable                  = 6
         * StringVariable                   = 7
         * RealVariable                     = 8
         * Integer                          = 9
         * Number                           = 10
         * UnsignedInteger                  = 11
         * Real                             = 12
         * String                           = 13
         * GreaterThanRelationalOperator    = 14
         * LessThanRelationalOperator       = 15
         * EqualRelationalOperator          = 16
         * EqualGreaterRelationalOperator   = 17
         * EqualLessRelationalOperator      = 18
         * NotEqualRelationalOperator       = 19
         * AndLogicalOperator               = 20
         * OrLogicalOperator                = 21
         * NotLogicalOperator               = 22
         * AddArithmaticOperator            = 23
         * SubArithmaticOperator            = 24
         * MulArithmaticOperator            = 25
         * DivArithmaticOperator            = 26
         * ThenKeyword = 27
         *  */
        public StatementRecognizer()
        {
            var commentStatement = new StatementRegex("^0",StatementType.Comment);
            var printStatement = new StatementRegex("((^2(13|10|6|8|7)$)|(^2$))", StatementType.Print);
            var readStatement = new StatementRegex("^3(6|8|7)$", StatementType.Read);
            var endStatement = new StatementRegex("^5$", StatementType.End);
            var arithmaticStatement = new StatementRegex("", StatementType.Airthmetic);

            statementRegexes = new StatementRegex[] { commentStatement, printStatement, readStatement, endStatement };
        }


        public void Verify(WordTokens[] wordTokens)
        {
            var matchString = TranslateToString(wordTokens);
            var results = statementRegexes.Select(r => new { statementType = r.Verify(matchString),  })
                                          .Where(a => a.statementType != StatementType.None)
                                          .ToArray();
            if (results?.Length > 0)
            {
                var line = string.Empty;
                var words = wordTokens.Select(w => w.value);
                foreach (var word in words)
                {
                    line += word;
                    if (word != words.Last())
                        line += " ";
                }

                Console.WriteLine("STATEMENT RECOGNIZED");
                Console.WriteLine(line);
                foreach(var statement in results)
                    Console.WriteLine(statement.statementType);
                
            }
        }

        public string TranslateToString(WordTokens[] wordTokens)
        {
            //This has to change a bit. I am selecting the first token, i should test all.
            var values = wordTokens.Where(w => w.tokens?.Length > 0).Select(w => (int)w.tokens.First().type);
            var result = string.Empty;
            foreach (var value in values)
                result += value.ToString();
            return result;

        }
        
    }

    

   
}
