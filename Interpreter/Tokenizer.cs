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
            var realVariable = new TokenRegex(@"(?!(IF)$)(^[G-N]([A-Z]{0,8}){1}$)", TokenType.RealVariable);
            var stringVariable = new TokenRegex("(?!(REM|READ|PRINT|THEN)$)(^[O-Z]([A-Z]{0,8}){1}$)", TokenType.StringVariable);
            var integerVariable = new TokenRegex("^[A-F]([A-Z]{0,8}){1}$", TokenType.IntegerVariable);
            var strng = new TokenRegex("^\"(.*)\"${1}", TokenType.String);
            var logicalOperator = new TokenRegex(@"^\.(OR|AND|NOT)\.$", TokenType.LogicalOperator);
            var relationalOperators = new TokenRegex(@"^\.(GT|LT|EQ|GE|LE|NE)\.$", TokenType.RelationalOperator);
            var arithmaticOperators = new TokenRegex(@"^\.(ADD|SUB|MUL|DIV)\.$", TokenType.ArithmaticOperator);
            var comment = new TokenRegex("^REM$", TokenType.CommentKeyword);
            var end = new TokenRegex(@"^END\.$", TokenType.EndKeyword);
            var read = new TokenRegex("^READ$", TokenType.ReadKeyword);
            var ifReg = new TokenRegex("^IF$", TokenType.IfKeyword);
            var printReg = new TokenRegex("^PRINT$", TokenType.PrintKeyword);
            var thenReg = new TokenRegex("^THEN$", TokenType.ThenKeyword);
            var unsignInteger = new TokenRegex(@"^(\d{1,6})$", TokenType.UnsignedInteger);
            var integer = new TokenRegex(@"((^(\+|-)(\d{1,6})$)|(^(\d{1,6})$))", TokenType.Integer);
            var real = new TokenRegex(@"((^(\+|-)(\d{1,6}))|(^(\d{1,6})))(\.(\d{1,6}))$|((^(\+|-)(\d{1,6}))|(^(\d{1,6})))(\.(\d{1,6}))E(((\+|-)(\d{1,6}))|(((\d{1,6}))\.(\d{1,6})$)|((\d{1,6})$))", TokenType.Real);
            var number = new TokenRegex(@"((^(\+|-)(\d{1,6}))|(^(\d{1,6})))(\.(\d{1,6}))$|((^(\+|-)(\d{1,6}))|(^(\d{1,6})))(\.(\d{1,6}))E(((\+|-)(\d{1,6}))|(((\d{1,6}))\.(\d{1,6})$)|((\d{1,6})$))|((^(\+|-)(\d{1,6})$)|(^(\d{1,6})$))", TokenType.Number);
            var assignment = new TokenRegex("^=$", TokenType.AssignmentOperator);

            tokenRegexes = new TokenRegex[] { realVariable, stringVariable,
                                              integerVariable, strng,
                                              logicalOperator, relationalOperators,
                                              arithmaticOperators, comment,
                                              end, read, ifReg, printReg,
                                              thenReg, unsignInteger, integer,
                                              real, number, assignment };
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
