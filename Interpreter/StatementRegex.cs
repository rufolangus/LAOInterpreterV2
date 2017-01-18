using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Interpreter
{
    public class StatementRegex
    {
        private Regex regEx;
        private StatementType type;

        public StatementRegex(string regEx, StatementType type)
        {
            this.regEx = new Regex(regEx);
            this.type = type;
        }

        public StatementType Verify(string sentence)
        {
            return regEx.IsMatch(sentence) ? type : StatementType.None;
        }
    };
}
