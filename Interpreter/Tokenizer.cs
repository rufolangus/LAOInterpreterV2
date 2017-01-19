using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
namespace Interpreter
{
    class Tokenizer 
    {
        TokenRegex[] tokenRegexes;
        public Tokenizer()
        {
            //VARIABLES
            var realVar         = new TokenRegex(@"(?!(IF)$)(^[G-N]([A-Z]{0,8}){1}$)", TokenType.RealVariable);
            var stringVar       = new TokenRegex("(?!(REM|READ|PRINT|THEN)$)(^[O-Z]([A-Z]{0,8}){1}$)", TokenType.StringVariable);
            var intVar          = new TokenRegex("^[A-F]([A-Z]{0,8}){1}$", TokenType.IntegerVariable);

            //Logical Operators
            var andOP           = new TokenRegex(@"^\.(AND)\.$", TokenType.AndLogicalOperator);
            var orOP            = new TokenRegex(@"^\.(OR)\.$", TokenType.OrLogicalOperator);
            var notOp           = new TokenRegex(@"^\.(NOT)\.$", TokenType.NotLogicalOperator);

            //Relational Operators
            var gtOP            = new TokenRegex(@"^\.(GT)\.$", TokenType.GreaterThanRelationalOperator);
            var ltOP            = new TokenRegex(@"^\.(LT)\.$", TokenType.LessThanRelationalOperator);
            var eqOP            = new TokenRegex(@"^\.(EQ)\.$", TokenType.EqualRelationalOperator);
            var leOP            = new TokenRegex(@"^\.(LE)\.$", TokenType.EqualLessRelationalOperator);
            var neOP            = new TokenRegex(@"^\.(NE)\.$", TokenType.NotEqualRelationalOperator);
            
            //ARITHMATIC OPERATORS
            var addOP           = new TokenRegex(@"^\.(ADD)\.$", TokenType.AddArithmaticOperator);
            var subOP           = new TokenRegex(@"^\.(SUB)\.$", TokenType.SubArithmaticOperator);
            var mulOP           = new TokenRegex(@"^\.(MUL)\.$", TokenType.MulArithmaticOperator);
            var divOP           = new TokenRegex(@"^\.(DIV)\.$", TokenType.DivArithmaticOperator);
            
            //ASIGNMENT OPERATORS
            var assignOP        = new TokenRegex("^=$", TokenType.AssignmentOperator);

            //KEYWORDS
            var comment         = new TokenRegex("^REM$", TokenType.CommentKeyword);
            var end             = new TokenRegex(@"^END\.$", TokenType.EndKeyword);
            var read            = new TokenRegex("^READ$", TokenType.ReadKeyword);
            var ifReg           = new TokenRegex("^IF$", TokenType.IfKeyword);
            var printReg        = new TokenRegex("^PRINT$", TokenType.PrintKeyword);
            var thenReg         = new TokenRegex("^THEN$", TokenType.ThenKeyword);

            //TYPES
            var strng           = new TokenRegex("^\"(.*)\"${1}", TokenType.String);
            var number          = new TokenRegex(@"((^(\+|-)(\d{1,8}))|(^(\d{1,8})))(\.(\d{1,8}))$|((^(\+|-)(\d{1,8}))|(^(\d{1,8})))(\.(\d{1,8}))E(((\+|-)(\d{1,8}))|(((\d{1,8}))\.(\d{1,8})$)|((\d{1,8})$))|((^(\+|-)(\d{1,8})$)|(^(\d{1,8})$))", TokenType.Number);
            var unsignInteger   = new TokenRegex(@"^(\d{1,8})$", TokenType.UnsignedInteger);
            var integer         = new TokenRegex(@"((^(\+|-)(\d{1,8})$)|(^(\d{1,8})$))", TokenType.Integer);
            var real            = new TokenRegex(@"((^(\+|-)(\d{1,8}))|(^(\d{1,8})))(\.(\d{1,8}))$|((^(\+|-)(\d{1,8}))|(^(\d{1,8})))(\.(\d{1,8}))E(((\+|-)(\d{1,8}))|(((\d{1,8}))\.(\d{1,8})$)|((\d{1,8})$))", TokenType.Real);

            tokenRegexes        = new TokenRegex[] {
                                                    realVar, stringVar, intVar,
                                                    andOP, orOP, notOp,
                                                    gtOP, ltOP, eqOP, leOP, neOP,
                                                    addOP, subOP, mulOP, divOP,
                                                    assignOP, comment, end, read,
                                                    ifReg, printReg, thenReg, strng,
                                                    number, unsignInteger, integer, real,
                                                   };
        }

        public Token[] Verify(string value)
        {
            var result = tokenRegexes.Select(t => t.Verify(value))
                                     .Where(t => t != null)
                                     .ToArray();
            return result;
        }
        
    }

    
}
