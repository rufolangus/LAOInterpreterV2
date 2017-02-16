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
        public static TokenType[] NumericTypes { get { return numericTypes; } }
        public static TokenType[] StringTypes { get { return stringTypes; } }

        public static TokenType[] VariableTypes { get { return variableTypes; } }

        private static TokenType[] numericTypes = new TokenType[] { TokenType.Number, TokenType.Integer, TokenType.Real, TokenType.RealVariable, TokenType.IntegerVariable };
        private static TokenType[] stringTypes = new  TokenType[] { TokenType.String, TokenType.StringVariable };
        private static TokenType[] variableTypes = new TokenType[] { TokenType.StringVariable, TokenType.RealVariable, TokenType.IntegerVariable };


        public Token(string value, TokenType type)
        {
            this.value = value;
            this.type = type;
        }
    }

    public enum TokenType
    {
        CommentKeyword, AssignmentOperator, PrintKeyword,
        ReadKeyword, IfKeyword, EndKeyword,
        IntegerVariable, StringVariable, RealVariable,
        Integer, Number, UnsignedInteger,
        Real, String, GreaterThanRelationalOperator,
        LessThanRelationalOperator, EqualRelationalOperator,
        EqualGreaterRelationalOperator, EqualLessRelationalOperator,
        NotEqualRelationalOperator, AndLogicalOperator,
        OrLogicalOperator, NotLogicalOperator,
        AddArithmaticOperator, SubArithmaticOperator,
        MulArithmaticOperator, DivArithmaticOperator,
        ThenKeyword,
    } 
}
