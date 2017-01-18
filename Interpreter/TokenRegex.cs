using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Interpreter
{
    public class TokenRegex
    {
        private Regex regEx;
        private TokenType type;
        private int length;

        public TokenRegex(string reg, TokenType type, int length = -1)
        {
            regEx = new Regex(reg, RegexOptions.IgnoreCase);
            var optiones = regEx.Options;
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
