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
        List<TokenRegex> tokenRegexes;

        public Tokenizer()
        {
            tokenRegexes = new List<TokenRegex>();
            var realVariable = new TokenRegex(@"^[G-Ng-n]([a-zA-Z]{0,8}){1}$", TokenType.RealVariable);
            var stringVariable = new TokenRegex("^[O-Zo-z]([a-zA-Z]{0,8}){1}$", TokenType.StringVariable);
            var integerVariable = new TokenRegex("^[A-Fa-f]([a-zA-Z]{0,8}){1}$", TokenType.IntegerVariable);
            var strng = new TokenRegex("^\"(.*)\"${1}", TokenType.String);
            var logicalOperator = new TokenRegex(@"^\.((o|O)(r|R)|(a|A)(n|N)(d|D)|(n|N)(o|O)(t|T))\.$", TokenType.LogicalOperator);
            var relationalOperators = new TokenRegex(@"^\.((((g|G)(t|T)|(l|L)(t|T))|((e|E)(q|Q)|(g|G)(e|E)))|((l|L)(e|E)|(n|N)(e|E)))\.$", TokenType.RelationalOperator);
            var arithmaticOperators = new TokenRegex(@"^\.(((((a|A)(d|D)(d|D))|((m|M)(u|U)(l|L))))|((((d|D)(I|i)(v|V))|((s|S)(u|U)(b|B)))))\.$", TokenType.ArithmaticOperator);
            var comment = new TokenRegex("^(R|r)(E|e)(M|m)$", TokenType.CommentKeyword);
            var end = new TokenRegex(@"^(E|e)(N|n)(D|d)\.$", TokenType.EndKeyword);
            var read = new TokenRegex("^(R|r)(E|e)(A|a)(D|d)$", TokenType.ReadKeyword);
            var ifReg = new TokenRegex("^(I|i)(f|F)$", TokenType.IfKeyword);
            var printReg = new TokenRegex("^(P|p)(r|R)(I|i)(n|N)(t|T)$", TokenType.PrintKeyword);
            var thenReg = new TokenRegex("^(T|t)(H|h)(E|e)(N|n)$", TokenType.ThenKeyword);
            var unsignInteger = new TokenRegex(@"^(\d{1,6})$", TokenType.UnsignedInteger);
            var integer = new TokenRegex(@"((^(\+|-)(\d{1,6})$)|(^(\d{1,6})$))", TokenType.Integer);
            var real = new TokenRegex(@"((^(\+|-)(\d{1,6}))|(^(\d{1,6})))(\.(\d{1,6}))$|((^(\+|-)(\d{1,6}))|(^(\d{1,6})))(\.(\d{1,6}))(e|E)(((\+|-)(\d{1,6}))|(((\d{1,6}))\.(\d{1,6})$)|((\d{1,6})$))", TokenType.Real);
            var number = new TokenRegex(@"((^(\+|-)(\d{1,6}))|(^(\d{1,6})))(\.(\d{1,6}))$|((^(\+|-)(\d{1,6}))|(^(\d{1,6})))(\.(\d{1,6}))(e|E)(((\+|-)(\d{1,6}))|(((\d{1,6}))\.(\d{1,6})$)|((\d{1,6})$))|((^(\+|-)(\d{1,6})$)|(^(\d{1,6})$))", TokenType.Number);
            var assignment = new TokenRegex("^=$", TokenType.AssignmentOperator);
            tokenRegexes.Add(printReg);
            tokenRegexes.Add(strng);
            tokenRegexes.Add(logicalOperator);
            tokenRegexes.Add(relationalOperators);
            tokenRegexes.Add(arithmaticOperators);
            tokenRegexes.Add(comment);
            tokenRegexes.Add(end);
            tokenRegexes.Add(read);
            tokenRegexes.Add(ifReg);
            tokenRegexes.Add(thenReg);
            tokenRegexes.Add(unsignInteger);
            tokenRegexes.Add(integer);
            tokenRegexes.Add(assignment);
            tokenRegexes.Add(real);
            tokenRegexes.Add(number);
            tokenRegexes.Add(stringVariable);
            tokenRegexes.Add(integerVariable);
            tokenRegexes.Add(realVariable);
        }

        public Token[] Verify(string value)
        {
            var result = tokenRegexes.Select(t => t.Verify(value))
                                     .Where(t => t != null)
                                     .ToArray();
            return result;
        }
        
    }

    public class TokenRegex
    {
        private Regex regEx;
        private TokenType type;
        private int length;

        public TokenRegex(string reg, TokenType type, int length = -1)
        {
            regEx = new Regex(reg);
            this.type = type;
            this.length = length;
        }

        public Token Verify(string expresion)
        {
           var isMatch = regEx.IsMatch(expresion);
           if (isMatch && (expresion.Length == length || length == -1))
               return new Token(expresion, type);
            
          return null;
        }
    }
}
