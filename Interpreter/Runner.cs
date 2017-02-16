using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
namespace Interpreter
{
    public class Runner
    {
        private Dictionary<string, string> variables;
        private Tokenizer tokenizer;

        public Runner(Tokenizer tokenizer)
        {
            this.tokenizer = tokenizer;
            variables = new Dictionary<string, string>();
        }

       public bool Run(Statement expresion)
        {
            string result = string.Empty;
            TokenType resultType;
            var type = expresion.type;
            switch (type)
            {
                case StatementType.None:
                    break;
                case StatementType.CommentStatement:
                    break;
                case StatementType.AssignmentStatement:
                    ((AssignmentStatement)expresion).Execute(variables,out result, out resultType, tokenizer);
                    break;
                case StatementType.PrintStatement:
                    ((PrintStatement)expresion).Execute(variables, out result, out resultType, tokenizer);
                    break;
                case StatementType.ReadStatement:
                    ((ReadStatement)expresion).Execute(variables, out result, out resultType, tokenizer);
                    break;
                case StatementType.IfStatement:
                    ((IfStatement)expresion).Execute(variables, out result, out resultType, tokenizer);
                    break;
                case StatementType.EndStatement:
                    return false;
                case StatementType.ConditionalStatement:
                    ((ConditionStatement)expresion).Evaluate(variables);
                    break;
                case StatementType.ArithmaticStatement:
                    ((ArithmaticStatement)expresion).Execute(variables, out result, out resultType, tokenizer);
                    break;
                case StatementType.ThenStatement:
                    break;
                default:
                    break;
            }
            return true;
        }
    }
}
