using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    public enum StatementType
    {
        None, CommentStatement,
        AssignmentStatement, PrintStatement,
        ReadStatement, IfStatement, EndStatement,
        ConditionalStatement,
        ArithmaticStatement,  ThenStatement
    }
}
