using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    public class WordTokens 
    {
        public string value;
        public Token[] tokens;

        public WordTokens(string value, Token[] tokens)
        {
            this.value = value;
            this.tokens = tokens;
        }

    }
}
