using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    public class Token
    {
        public string value;
        public TokenType type;

        public Token(string value, TokenType type)
        {
            this.value = value;
            this.type = type;
        }
    }

    public enum TokenType
    {
        Comment, Assignment, Print,
        Read, If, End, IntegerVariable,StringVariable, RealVariable,
        Letter, Integer, Number,
        UnsignedInteger, Real, DecimalPart,
        DecimalPoint, ExponentPart, Exponent,
        Digit, Sign, String, AnyString,
        RelationalOperator, LogicalOperator,
        ThenStatement,
    } 
}
