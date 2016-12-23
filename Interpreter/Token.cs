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
        CommentKeyword, AssignmentOperator, PrintKeyword,
        ReadKeyword, IfKeyword, EndKeyword,
        IntegerVariable, StringVariable, RealVariable,
        Integer, Number, UnsignedInteger,
        Real, String, RelationalOperator,
        LogicalOperator, ArithmaticOperator, ThenKeyword,
    } 
}
