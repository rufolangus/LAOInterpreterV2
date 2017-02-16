using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    public class Statement
    {
        private WordTokens[] tokens;
        public StatementType type;
        protected Statement subStatement;


        public Statement(WordTokens[] tokens, Statement subStatement = null)
        {
            this.tokens = tokens;
            this.subStatement = subStatement;
        }

        public void SetSubStatement(Statement statement)
        {
            if (subStatement == null)
                subStatement = statement;
            else
                subStatement.SetSubStatement(statement);
        }


        public bool IsNumeric(WordTokens token)
        {
           return token.tokens.Any(tok => Token.NumericTypes.Any(t => tok.type == t));
        }

        public bool IsVariable(WordTokens token)
        {
            return token.tokens.Any(tok => Token.VariableTypes.Any(t => tok.type == t));
        }
    }

    public class IfStatement : Statement , ExpressionStatement
    {
        public ConditionStatement condition;
        public ExpressionStatement expression;

        public IfStatement(WordTokens[] tokens,  ConditionStatement condition, ExpressionStatement expression, Statement subStatement = null) : base(tokens, subStatement)
        {
            this.condition = condition;
            this.expression = expression;
            type = StatementType.IfStatement;
        }

        public void Execute(Dictionary<string, string> variables, out string result, out TokenType resultType, Tokenizer tokenizer = null)
        {
            resultType = TokenType.IfKeyword;
            result = string.Empty;
            if (condition.Evaluate(variables))
                expression.Execute(variables, out result, out resultType, tokenizer);
        }
    }
    public interface ExpressionStatement
    {
        void Execute(Dictionary<string, string> variables, out string result, out TokenType resultType, Tokenizer  tokenizer = null);
    }


    public interface ConditionStatement
    {
        bool Evaluate(Dictionary<string,string> variables);
    }

    public class RelationalStatement : Statement, ConditionStatement
    {
        private WordTokens op;
        private WordTokens left;
        private WordTokens right;

        public RelationalStatement(WordTokens[] tokens, WordTokens op, WordTokens left, WordTokens right, Statement subStatement = null) : base(tokens, subStatement)
        {
            this.left = left;
            this.right = right;
            this.op = op;
            this.type = StatementType.ConditionalStatement;
        }

        public bool Evaluate(Dictionary<string, string> variables)
        {
            var isNumeric = IsNumeric(left);
            var firstIsVariable = IsVariable(left);
            var secondIsVariable = IsVariable(right);
            var firstvalue = firstIsVariable ? variables[left.value] : left.value;
            var secondValue = secondIsVariable ? variables[right.value] : right.value;
            var result = false;
            if (isNumeric)
            {
                var firstParam = float.Parse(firstvalue);
                var secondParam = float.Parse(secondValue);
                switch (op.tokens.First().type)
                {
                    case TokenType.EqualRelationalOperator:
                        result = firstParam == secondParam;
                        break;
                    case TokenType.NotEqualRelationalOperator:
                        result = firstParam != secondParam;
                        break;
                    case TokenType.GreaterThanRelationalOperator:
                        result = firstParam > secondParam;
                        break;
                    case TokenType.LessThanRelationalOperator:
                        result = firstParam < secondParam;
                        break;
                    case TokenType.EqualGreaterRelationalOperator:
                        result = firstParam >= secondParam;
                        break;
                    case TokenType.EqualLessRelationalOperator:
                        result = firstParam <= secondParam;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                var firstParam = firstvalue;
                var secondParam = secondValue;
                switch (op.tokens.First().type)
                {
                    case TokenType.EqualRelationalOperator:
                        result = firstParam == secondParam;
                        break;
                    case TokenType.NotEqualRelationalOperator:
                        result = firstParam != secondParam;
                        break;
                    case TokenType.GreaterThanRelationalOperator:
                        result = firstParam.Length > secondParam.Length;
                        break;
                    case TokenType.LessThanRelationalOperator:
                        result = firstParam.Length < secondParam.Length;
                        break;
                    case TokenType.EqualGreaterRelationalOperator:
                        result = firstParam.Length >= secondParam.Length;
                        break;
                    case TokenType.EqualLessRelationalOperator:
                        result = firstParam.Length <= secondParam.Length;
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

    }

    public class LogicalStatement : Statement, ConditionStatement
    {
        private ConditionStatement left;
        private WordTokens op;
        private ConditionStatement right;

        public LogicalStatement(WordTokens[] tokens, ConditionStatement left, ConditionStatement right, WordTokens op,  Statement subStatement = null) : base(tokens, subStatement)
        {
            this.left = left;
            this.right = right;
            this.op = op;
            type = StatementType.ConditionalStatement;
        }

        public bool Evaluate(Dictionary<string, string> variables)
        {
            switch (op.tokens.First().type)
            {
                case TokenType.AndLogicalOperator:
                    return left.Evaluate(variables) && right.Evaluate(variables);
                case TokenType.NotLogicalOperator:
                    return !right.Evaluate(variables);
                case TokenType.OrLogicalOperator:
                    return left.Evaluate(variables) || right.Evaluate(variables);
                default:
                    return false;
            }
            throw new NotImplementedException();
        }
    }

    public class ArithmaticStatement : Statement, ExpressionStatement
    {
        private WordTokens op;
        private WordTokens left;
        private WordTokens right;

        public ArithmaticStatement(WordTokens[] tokens, WordTokens left, WordTokens right, WordTokens op, Statement subStatement = null) : base(tokens, subStatement)
        {
            this.left = left;
            this.right = right;
            this.op = op;
            this.type = StatementType.ArithmaticStatement;
        }

        public void Execute(Dictionary<string, string> variables, out string result, out TokenType resultType, Tokenizer tokenizer = null)
        {
            var firstIsNumeric = IsNumeric(left);
            var secondIsNumeric = IsNumeric(right);
            var firstIsVariable = IsVariable(left);
            var secondIsVariable = IsVariable(right);
            var firstvalue = firstIsVariable ? variables[left.value] : left.value;
            var secondValue = secondIsVariable ? variables[right.value] : right.value;
            resultType = TokenType.Number;
            result = string.Empty;
            switch (op.tokens.First().type)
            {
                case TokenType.AddArithmaticOperator:
                    if(firstIsNumeric && secondIsNumeric)
                    {
                        result = (float.Parse(firstvalue) + float.Parse(secondValue)).ToString();
                        resultType = TokenType.Number;
                    }else
                    if (firstIsNumeric || secondIsNumeric || (!firstIsNumeric && !secondIsNumeric))
                    {
                        result = firstvalue + secondValue;
                        resultType = TokenType.String;
                    }
                    
                    break;
                case TokenType.MulArithmaticOperator:
                    result = (float.Parse(firstvalue) * float.Parse(secondValue)).ToString();
                    resultType = TokenType.Number;
                    break;
                case TokenType.DivArithmaticOperator:
                    var res = (float.Parse(firstvalue) / float.Parse(secondValue));
                    res = (int)res;
                    result = res.ToString();
                    resultType = TokenType.Number;
                    break;
                case TokenType.SubArithmaticOperator:
                    result = (float.Parse(firstvalue) - float.Parse(secondValue)).ToString();
                    resultType = TokenType.Number;
                    break;
            }
        }
    }

    public class PrintStatement : Statement, ExpressionStatement
    {
        private WordTokens variable;

        public PrintStatement(WordTokens[] tokens,  WordTokens variable, Statement subStatement = null) : base(tokens, subStatement)
        {
            this.variable = variable;
            type = StatementType.PrintStatement;
        }

        public void Execute(Dictionary<string, string> variables, out string result, out TokenType resultType, Tokenizer tokenizer = null)
        {
            if(variable == null && subStatement != null)
            {
                if(subStatement.GetType().GetInterfaces().Any(i => i == typeof(ExpressionStatement)))
                {
                    var expression = (ExpressionStatement)subStatement;
                    expression.Execute(variables, out result, out resultType, tokenizer);
                    Console.WriteLine(result);
                    result = string.Empty;
                    resultType = TokenType.PrintKeyword;

                }
            }else
            {
                if(variable != null)
                {
                    if (variables.ContainsKey(variable.value))
                    {
                        var variab = variables[variable.value];
                        Console.WriteLine(variab);
                    }
                    else if (variable.tokens.Any(t => Token.NumericTypes.Any(tt => tt == t.type)))
                        Console.WriteLine(variable.value);
                    else if (variable.tokens.Any(t => t.type == TokenType.String))
                        Console.WriteLine(variable.value.Replace('\"', ' '));
                    else
                        Console.WriteLine("Error: Variable does not exist");
                    

                }else
                    Console.WriteLine("");
                
            }
            result = string.Empty;
            resultType = TokenType.PrintKeyword;

        }
    }

    public class ReadStatement : Statement, ExpressionStatement
    {
        private WordTokens variable;

        public ReadStatement(WordTokens[] tokens,  WordTokens variable, Statement subStatement = null) : base(tokens, subStatement)
        {
            this.variable = variable;
            type = StatementType.ReadStatement;
            

        }

        public void Execute(Dictionary<string, string> variables, out string result, out TokenType resultType, Tokenizer tokenizer = null)
        {

            var input = Console.ReadLine();
            var variableType = variable.tokens.FirstOrDefault(t => Token.VariableTypes.Any(vt => t.type == vt));
            if (variableType != null)
            {
                var resultTokens = tokenizer.Verify(input);
                if (resultTokens?.Length > 0)
                {
                    result = string.Empty;
                    resultType = TokenType.ReadKeyword;
                    if (!variables.ContainsKey(variable.value))
                        variables.Add(variable.value, resultTokens.First().value);
                    else
                        variables[variable.value] = resultTokens.First().value;
                }
               
            }

            resultType = TokenType.ReadKeyword;
            result = string.Empty;
        }
    }

    public class AssignmentStatement : Statement, ExpressionStatement
    {
        private WordTokens variable;
        private WordTokens value;

        public AssignmentStatement(WordTokens[] tokens,  WordTokens variable, WordTokens value, Statement subStatement = null) : base(tokens, subStatement)
        {
            this.variable = variable;
            this.value = value;
            type = StatementType.AssignmentStatement;
            
        }

        public void Execute(Dictionary<string, string> variables, out string result, out TokenType resultType, Tokenizer tokenizer = null)
        {
            result = string.Empty;
            resultType = TokenType.AssignmentOperator;
            

            if(subStatement != null)
            {
                if (subStatement.GetType().GetInterfaces().Any(i => i == typeof(ExpressionStatement)))
                {
                    var expression = (ExpressionStatement)subStatement;
                    expression.Execute(variables, out result, out resultType, tokenizer);
                    if (variables.ContainsKey(variable.value))
                    {
                        variables[variable.value] = result;
                    }
                    else
                    {
                        variables.Add(variable.value, result);
                    }

                }
            }else if

            (value != null)
            {
                if(variables.ContainsKey(variable.value))
                {
                    variables[variable.value] = value.value;
                }
                else
                {
                    variables.Add(variable.value, value.value);
                }
            }
        }
    }












}
