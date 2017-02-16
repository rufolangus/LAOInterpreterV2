using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions; 

namespace Interpreter
{
    public class StatementRecognizer
    {
        StatementRegex[] statementRegexes;
        public event Action<string> OnError;


        /* Token Type ENUM:
         * CommentKeyword                   = 0
         * Assignment Operator              = 1
         * PrinterKeyword                   = 2
         * ReadKeyword                      = 3
         * IfKeyword                        = 4
         * EndKeyWord                       = 5
         * IntegerVariable                  = 6
         * StringVariable                   = 7
         * RealVariable                     = 8
         * Integer                          = 9
         * Number                           = 10
         * UnsignedInteger                  = 11
         * Real                             = 12
         * String                           = 13
         * GreaterThanRelationalOperator    = 14
         * LessThanRelationalOperator       = 15
         * EqualRelationalOperator          = 16
         * EqualGreaterRelationalOperator   = 17
         * EqualLessRelationalOperator      = 18
         * NotEqualRelationalOperator       = 19
         * AndLogicalOperator               = 20
         * OrLogicalOperator                = 21
         * NotLogicalOperator               = 22
         * AddArithmaticOperator            = 23
         * SubArithmaticOperator            = 24
         * MulArithmaticOperator            = 25
         * DivArithmaticOperator            = 26
         * ThenKeyword                      = 27
         *  */

        public StatementRecognizer()
        {
            var commentStatement        = new StatementRegex(@"^0",StatementType.CommentStatement);
            var printStatement          = new StatementRegex(@"^2(-(13|10|6|8|7))?$", StatementType.PrintStatement);
            var readStatement           = new StatementRegex(@"^3-(6|8|7)$", StatementType.ReadStatement);
            var endStatement            = new StatementRegex(@"^5$", StatementType.EndStatement);
            var arithmaticStatement     = new StatementRegex(@"^(((6|7|8|10|13)(-(23)-(6|7|8|10|13))+)|((6|8|10)(-(23|24|25|26)-(6|8|10))+))$", StatementType.ArithmaticStatement);
            //conditionalStatement needs some fixes
            var conditionalStatement    = new StatementRegex(@"^(22-)?(((13|7)(-(14|15|16|18|19)-(13|7)))|((6|8|10)(-(14|15|16|18|19)-(6|8|10))))((-(20|21)-(((13|7)(-(14|15|16|18|19)-(13|7)))|((6|8|10)(-(14|15|16|18|19)-(6|8|10)))))+)?$", StatementType.ConditionalStatement);
            //assignmentStatmentNeedsSomeFixes ex: a = 3 .add. b
            var assignmentStatment      = new StatementRegex(@"^((7-1-(7|13))|(6-1-(6|10))|(8-1-(8|10)))$", StatementType.AssignmentStatement);
            var thenStatement           = new StatementRegex(@"^(27-)((3-(6|8|7))|(2(-(13|10|6|8|7))?$)|(((7-1-(7|13))|(6-1-(6|10))|(8-1-(8|10)))$))", StatementType.ThenStatement);
            var ifStatement             = new StatementRegex(@"^(4-)(22-)?(((13|7)(-(14|15|16|18|19)-(13|7)))|((6|8|10)(-(14|15|16|18|19)-(6|8|10))))((-(20|21)-(((13|7)(-(14|15|16|18|19)-(13|7)))|((6|8|10)(-(14|15|16|18|19)-(6|8|10)))))+)?(-27-)((3-(6|8|7))|(2(-(13|10|6|8|7))?$)|(((7-1-(7|13))|(6-1-(6|10))|(8-1-(8|10)))$))$", StatementType.IfStatement);
            statementRegexes            = new StatementRegex[] { commentStatement, printStatement,
                                                                 readStatement, endStatement,
                                                                 arithmaticStatement, conditionalStatement,
                                                                 assignmentStatment, thenStatement, ifStatement
                                                               };

        }


        public Statement Verify(WordTokens[] wordTokens)
        {

            var list = new List<WordTokens>();
            Statement expression = null;
            foreach(var token in wordTokens)
            {
                list.Add(token);
                var result = IdentifyStatements(list.ToArray());
                if (result != null && (result.type == StatementType.PrintStatement && wordTokens.Count() > list.Count()))
                    continue;
                else
                {
                    if (result?.type == StatementType.IfStatement)
                    {
                        var statement = (IfStatement)result;
                        var exp = (Statement)statement.expression;
                        if ((exp.type == StatementType.PrintStatement || exp.type == StatementType.AssignmentStatement) && wordTokens.Count() > list.Count())
                            continue;
                    }
                    else if (result?.type == StatementType.AssignmentStatement && wordTokens.Count() > list.Count())
                        continue;
                   
                }
                if(result != null)
                {
                    if (expression != null)
                    {
                        expression.SetSubStatement(result);
                    }
                    else
                        expression = result;
                    list = new List<WordTokens>();
                }
            }
            return expression;
        }

        
        PrintStatement CheckForPrintStatement(WordTokens[] tokens)
        {
            var allowedTokenTypes = new TokenType[] { TokenType.String, TokenType.StringVariable,
                                                      TokenType.Real, TokenType.RealVariable,
                                                      TokenType.Integer, TokenType.IntegerVariable,
                                                      TokenType.Number };
            if (tokens == null || tokens.Length == 0 || tokens.First().tokens.First().type != TokenType.PrintKeyword)
                return null;
            if (tokens.Length == 1)
                return new PrintStatement(tokens,null);
            else if (tokens.Length == 2)
            {
                var secondToken = tokens[1];
                if (allowedTokenTypes.Any(t => t == secondToken.tokens.First().type))
                    return new PrintStatement(tokens, secondToken);
                else
                {
                    var errorString = allowedTokenTypes.Select(t => t.ToString()).Aggregate((current, next) => { return current + "\n" + next; });
                    errorString = "Error: Second token must be any of the following:\n" + errorString;
                    OnError?.Invoke(errorString);
                    return null;
                }
            }
            else if(tokens.Length > 2)
            {
                OnError?.Invoke("Too many arguments for print statement.");
                return null;
            }
            OnError?.Invoke("Unidentified Error.");
            return null;
        }


        AssignmentStatement CheckForAssignment(WordTokens[] tokens)
        {
            var intTypes = new TokenType[] { TokenType.Integer, TokenType.IntegerVariable, TokenType.UnsignedInteger };
            var realTypes = new TokenType[] { TokenType.Real, TokenType.RealVariable };
            var stringTypes = new TokenType[] { TokenType.String, TokenType.StringVariable };

            if(tokens.Length > 2)
            {
                var firstTokenType = tokens.First().tokens.First().type;
                var firstToken = tokens.First();

                var secondTokenType = tokens[1].tokens.First().type;
                var thirdTokenType = tokens[2].tokens;

                if (secondTokenType == TokenType.AssignmentOperator)
                {
                    var isArithmatic = CheckForArithmaticStatement(tokens.Skip(2).ToArray());
                    switch (firstTokenType)
                    {
                        case TokenType.IntegerVariable:

                            if (isArithmatic == null && thirdTokenType.Any(t => intTypes.Any(type => t.type == type)))
                                return new AssignmentStatement(tokens, firstToken, tokens[2]);
                            else if (isArithmatic != null && thirdTokenType.Any(t => intTypes.Any(type => t.type == type)))
                                return new AssignmentStatement(tokens, firstToken, tokens[2], isArithmatic);
                            break;
                        case TokenType.RealVariable:
                            if (isArithmatic == null && thirdTokenType.Any(t => realTypes.Any(type => t.type == type)))
                                return new AssignmentStatement(tokens, firstToken, tokens[2]);
                            else if (isArithmatic != null && thirdTokenType.Any(t => realTypes.Any(type => t.type == type)))
                                return new AssignmentStatement(tokens, firstToken, tokens[2], isArithmatic);
                            break;
                        case TokenType.StringVariable:
                            if (isArithmatic == null && thirdTokenType.Any(t => stringTypes.Any(type => t.type == type)))
                                return new AssignmentStatement(tokens, firstToken, tokens[2]);
                            else if (isArithmatic != null && thirdTokenType.Any(t => intTypes.Any(type => t.type == type)))
                                return new AssignmentStatement(tokens, firstToken, tokens[2], isArithmatic);

                            break;
                        default:
                            OnError?.Invoke("First token type is not a variable.");
                            return null;
                    }
                }else
                {
                    OnError?.Invoke("Second argument has to be an assignment operator.");
                    return null;
                }
            }
            OnError?.Invoke("Not enough arguments for Assignment Statement");
            return null;
        }

        RelationalStatement CheckForRelational(WordTokens[] tokens)
        {
            var conditionalOperators = new TokenType[] { 
                                                         TokenType.EqualRelationalOperator, TokenType.NotEqualRelationalOperator,
                                                         TokenType.GreaterThanRelationalOperator, TokenType.LessThanRelationalOperator,
                                                         TokenType.EqualGreaterRelationalOperator, TokenType.EqualLessRelationalOperator,
                                                        };

            var stringTypes = new TokenType[] { TokenType.String, TokenType.StringVariable };
            var realTypes = new TokenType[] { TokenType.Real, TokenType.RealVariable };
            var intTypes = new TokenType[] { TokenType.Integer, TokenType.IntegerVariable, TokenType.UnsignedInteger };

            var firstToken = tokens.First();
            bool isRelational = false;
            if (tokens.Length >= 3)
            {
                var op = tokens.Select((value, index) => new { value, index }).FirstOrDefault(t => t.value.tokens.Any(to => conditionalOperators.Any(o => o == to.type)));
                if (op != null)
                {
                    var index = op.index;
                    var before = tokens[index - 1];
                    if (index + 1 >= tokens.Length)
                        return null;
                    var after = tokens[index + 1];
                    var isStrings = (stringTypes.Any(t => before.tokens.Any(tk => tk.type == t)) && stringTypes.Any(t => after.tokens.Any(tk => tk.type == t)));
                    var isReals = (realTypes.Any(t => before.tokens.Any(tk => tk.type == t)) && realTypes.Any(t => after.tokens.Any(tk => tk.type == t)));
                    var isInts = (intTypes.Any(t => before.tokens.Any(tk => tk.type == t)) && intTypes.Any(t => after.tokens.Any(tk => tk.type == t)));
                    isRelational = isStrings || isReals || isInts;
                    if (isRelational)
                        return new RelationalStatement(tokens, op.value, before, after);
                }

            }
            return null;
        }

        ArithmaticStatement CheckForArithmaticStatement(WordTokens[] tokens)
        {
            if (tokens?.Length == 0)
                return null;
            var typesForAdd = new TokenType[] { TokenType.String, TokenType.StringVariable, TokenType.RealVariable, TokenType.IntegerVariable, TokenType.Number, TokenType.Real, TokenType.Integer };
            var typesForRest = new TokenType[] { TokenType.RealVariable, TokenType.IntegerVariable, TokenType.Number, TokenType.Real, TokenType.Integer };
            var firstToken = tokens.First().tokens.First();
            var arithmaticOperators = new TokenType[] { TokenType.AddArithmaticOperator, TokenType.SubArithmaticOperator, TokenType.MulArithmaticOperator, TokenType.DivArithmaticOperator };
            if(tokens.Length > 2)
            {
                var secondToken = tokens[1].tokens.First();
                var thirdToken = tokens[2].tokens.First();
                if(arithmaticOperators.Any(op => secondToken.type == op))
                {
                    switch (secondToken.type)
                    {
                        case TokenType.AddArithmaticOperator:
                            if (typesForAdd.Any(t => t == firstToken.type) && typesForAdd.Any(t => t == thirdToken.type))
                                return new ArithmaticStatement(tokens, tokens.First(), tokens[2], tokens[1]);
                            break;
                        case TokenType.SubArithmaticOperator:
                            if (typesForRest.Any(t => t == firstToken.type) && typesForRest.Any(t => t == thirdToken.type))
                                return new ArithmaticStatement(tokens, tokens.First(), tokens[2], tokens[1]);
                            break; 
                        case TokenType.MulArithmaticOperator:
                            if (typesForRest.Any(t => t == firstToken.type) && typesForRest.Any(t => t == thirdToken.type))
                                return new ArithmaticStatement(tokens, tokens.First(), tokens[2], tokens[1]);
                            break;
                        case TokenType.DivArithmaticOperator:
                            if (typesForRest.Any(t => t == firstToken.type) && typesForRest.Any(t => t == thirdToken.type))
                                return new ArithmaticStatement(tokens, tokens.First(), tokens[2], tokens[1]);
                            break;
                    }
                }else
                {
                    OnError?.Invoke("Second argument must be an arithmatic operator");
                    return null;
                }
            }
            OnError?.Invoke("Not enough arguments for Arithmatic Statement");
            return null;
        }

        ReadStatement CheckForReadStatement(WordTokens[] tokens)
        {
            var allowedTokenTypes = new TokenType[] { TokenType.StringVariable,
                                                      TokenType.RealVariable,
                                                      TokenType.IntegerVariable };

            if (tokens.Length == 2)
            {
                var secondToken = tokens[1];
                if (allowedTokenTypes.Any(t => t == secondToken.tokens.First().type))
                    return new ReadStatement(tokens,secondToken);
                else
                {
                    var errorString = allowedTokenTypes.Select(t => t.ToString()).Aggregate((current, next) => { return current + "\n" + next; });
                    errorString = "Error: Second token must be any of the following:\n" + errorString;
                    OnError?.Invoke(errorString);
                    return null;
                }
            }
            else if (tokens.Length > 2)
            {
                OnError?.Invoke("Too many arguments for print statement.");
                return null;
            }
            OnError?.Invoke("Unidentified Error.");
            return null;
        }

        IfStatement CheckForIfStatement(WordTokens[] tokens)
        {
            var firstToken = tokens.First();
            var thenTokenPair = tokens.Select((value, index) => new { value, index }).FirstOrDefault(i => i.value.tokens.Any(t => t.type == TokenType.ThenKeyword));
            if (thenTokenPair != null)
            {
                var thenIndex = thenTokenPair.index;
                var condition = tokens.Skip(1).Take(thenIndex - 1).ToArray();
                var statment = tokens.Skip(thenIndex + 1).ToArray();
                var isConditional = CheckForConditional(condition);
                var isRelational = CheckForRelational(condition);
                var conditionStatement = (ConditionStatement)isConditional == null ? (ConditionStatement)isRelational : (ConditionStatement)isConditional;
                if (conditionStatement != null)
                {
                    var asisgnment = CheckForAssignment(statment);
                    var print = CheckForPrintStatement(statment);
                    var read = CheckForReadStatement(statment);
                    var arithmatic = CheckForArithmaticStatement(tokens);
                     if (arithmatic != null)
                        return new IfStatement(tokens, conditionStatement, arithmatic);
                    else if (asisgnment != null)
                        return new IfStatement(tokens, conditionStatement, asisgnment);
                    else if (print != null)
                        return new IfStatement(tokens, conditionStatement, print);
                    else if (read != null)
                        return new IfStatement(tokens, conditionStatement, read);

                } else if (isRelational != null)
                {

                }
               

            }

            return null;

        }

        LogicalStatement CheckForConditional(WordTokens[] tokens)
        {
            var firstToken = tokens.First();
            var validOperators = new TokenType[] {TokenType.AndLogicalOperator, TokenType.OrLogicalOperator };
            if (tokens.First().tokens.Any(t => t.type == TokenType.NotLogicalOperator))
            {
                var statement = tokens.Skip(1).ToArray();
                var isConditional = CheckForConditional(statement);
                var isRelational = CheckForRelational(statement);
                if (isConditional != null)
                    return new LogicalStatement(tokens, null, isConditional, tokens.First());
                if (isRelational != null)
                    return new LogicalStatement(tokens, null, isRelational,tokens.First());
            }
            else
            {
                var op = tokens.Select((value, index) => new { value, index }).FirstOrDefault(t => t.value.tokens.Any(to => validOperators.Any(vOp => vOp == to.type)));
                if(op != null)
                {
                    var index = op.index;
                    var firstStatement = tokens.Take(index).ToArray();
                    var secondStatement = tokens.Skip(index + 1).ToArray();

                    var firstConditional = CheckForConditional(firstStatement);
                    var firstRelational = CheckForRelational(firstStatement);

                    var secondConditional = CheckForConditional(secondStatement);
                    var secondRelational = CheckForRelational(secondStatement);

                    ConditionStatement first = ((ConditionStatement)firstConditional) == null ? (ConditionStatement)firstRelational : (ConditionStatement)firstConditional;
                    ConditionStatement second = ((ConditionStatement)secondConditional) == null ? (ConditionStatement)secondRelational : (ConditionStatement)secondConditional;
                    if (first  != null && second != null)
                        return new LogicalStatement(tokens, first, second, op.value);
                }
                
            }
            return null;
        }

        Statement CheckForArithmaticAssingmentRelationalConditional(WordTokens[] tokens)
        {
            var assignment = CheckForAssignment(tokens);
            var relational = CheckForRelational(tokens);
            var arithmatic = CheckForArithmaticStatement(tokens);
            var conditional = CheckForConditional(tokens);

            if (assignment != null)
                return assignment;
            else if (relational != null)
                return relational;
            else if (arithmatic != null)
                return arithmatic;
            else
                return conditional;
        }

        Statement CheckForArithmaticStatementRelationalConditional(WordTokens[] tokens)
        {
            var arithmatic = CheckForArithmaticStatement(tokens);
            var relational = CheckForRelational(tokens);
            var conditional = CheckForConditional(tokens);
            if (arithmatic != null)
                return arithmatic;
            else if (relational != null)
                return relational;
            else
                return conditional;
                
        }

        Statement IdentifyStatements(WordTokens[] tokens)
        {
            TokenType firstToken;
            WordTokens token;
            
            if(tokens?.Length > 0 && (token = tokens.First()).tokens?.Length > 0)
            {
                firstToken = token.tokens.First().type;
                switch (firstToken)
                {
                    case TokenType.CommentKeyword:

                        var commentStatement = new Statement(tokens);
                        commentStatement.type = StatementType.CommentStatement;
                        return commentStatement;
                    case TokenType.AssignmentOperator:
                        OnError?.Invoke("No statement starts with the Assignment Operator");
                        break;
                    case TokenType.PrintKeyword:
                        return CheckForPrintStatement(tokens);
                    case TokenType.ReadKeyword:
                        return CheckForReadStatement(tokens);
                    case TokenType.IfKeyword:
                        return CheckForIfStatement(tokens);
                    case TokenType.EndKeyword:
                        var statement = new Statement(tokens);
                        statement.type = StatementType.EndStatement;
                        return statement;
                    case TokenType.IntegerVariable:
                        
                        return CheckForArithmaticAssingmentRelationalConditional(tokens);
                    case TokenType.StringVariable:
                        return CheckForArithmaticAssingmentRelationalConditional(tokens);
                    case TokenType.RealVariable:
                        return CheckForArithmaticAssingmentRelationalConditional(tokens);
                    case TokenType.Integer:
                        return CheckForArithmaticStatementRelationalConditional(tokens);
                    case TokenType.Number:
                        return CheckForArithmaticStatementRelationalConditional(tokens);
                    case TokenType.UnsignedInteger:
                        return CheckForArithmaticStatementRelationalConditional(tokens);
                    case TokenType.Real:
                        return CheckForArithmaticStatementRelationalConditional(tokens);
                    case TokenType.String:
                        return CheckForArithmaticStatementRelationalConditional(tokens);
                    case TokenType.GreaterThanRelationalOperator:
                        OnError?.Invoke("No statement starts with the Greater Than Operator");
                        break;
                    case TokenType.LessThanRelationalOperator:
                        OnError?.Invoke("No statement starts with the Less Than Operator");
                        break;
                    case TokenType.EqualRelationalOperator:
                        OnError?.Invoke("No statement starts with the Equal Operator");
                        break;
                    case TokenType.EqualGreaterRelationalOperator:
                        OnError?.Invoke("No statement starts with the Equal Greater Than Operator");
                        break;
                    case TokenType.EqualLessRelationalOperator:
                        OnError?.Invoke("No statement starts with the Equal Less Than Operator");
                        break;
                    case TokenType.NotEqualRelationalOperator:
                        OnError?.Invoke("No statement starts with the Not Equal Operator");
                        break;
                    case TokenType.AndLogicalOperator:
                        OnError?.Invoke("No statement starts with the And Operator");
                        break;
                    case TokenType.OrLogicalOperator:
                        OnError?.Invoke("No statement starts with the Or Operator");
                        break;
                    case TokenType.NotLogicalOperator:
                        break;
                    case TokenType.AddArithmaticOperator:
                        OnError?.Invoke("No statement starts with the Add Operator");
                        break;
                    case TokenType.SubArithmaticOperator:
                        OnError?.Invoke("No statement starts with the Substract Operator");
                        break;
                    case TokenType.MulArithmaticOperator:
                        OnError?.Invoke("No statement starts with the Multiplication Operator");
                        break;
                    case TokenType.DivArithmaticOperator:
                        OnError?.Invoke("No statement starts with the Division Operator");
                        break;
                    case TokenType.ThenKeyword:
                        OnError?.Invoke("No statement starts with the Then Operator");
                        break;
                    default:
                        OnError?.Invoke("Undefined error ocurred.");
                        break;
                }
            }
            return null;
        }

        public string TranslateToString(WordTokens[] wordTokens)
        {
            //This has to change a bit. I am selecting the first token, i should test all.
            var result = wordTokens.Where(w => w.tokens?.Length > 0)
                                   .Select(w => ((int)w.tokens.First().type).ToString())
                                   .Aggregate((current, next) => current.ToString() + "-" + next.ToString());
            Console.WriteLine("Resulting REGEX STRING: " + result);
            return result;

        }
        
    }

    

   
}
