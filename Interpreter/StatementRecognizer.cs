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


        public void Verify(WordTokens[] wordTokens)
        {

            var result = Test(wordTokens);
            Console.WriteLine(wordTokens.Select(t => t.value).Aggregate((current, next) => { return current + " " + next; }));
            Console.WriteLine("Is Token? " + result);
            return;
            var matchString = TranslateToString(wordTokens);
            var list = new List<string>();
            
            //Using anonymous type. Should create an class for this.
            var results = statementRegexes.Select(r => new { statementType = r.Verify(matchString),  })
                                          .Where(a => a.statementType != StatementType.None)
                                          .ToArray();
            if (results?.Length > 0)
            {
                var line = string.Empty;
                var words = wordTokens.Select(w => w.value).ToArray();
                foreach (var word in words)
                    line += word + " ";
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("STATEMENT RECOGNIZED: ");
                Console.WriteLine(line);
                foreach(var statement in results)
                    Console.WriteLine(statement.statementType);

            }else
            {
                var line = string.Empty;
                //Console.ForegroundColor = ConsoleColor.White;

                
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error");
                Console.WriteLine("Print requires a variable, literal or none.");
                Console.ForegroundColor = ConsoleColor.White;
            }
            
        }

        
        bool CheckForPrintStatement(WordTokens[] tokens)
        {
            var allowedTokenTypes = new TokenType[] { TokenType.String, TokenType.StringVariable,
                                                      TokenType.Real, TokenType.RealVariable,
                                                      TokenType.Integer, TokenType.IntegerVariable,
                                                      TokenType.Number };

            if (tokens.Length == 1)
                return true;
            else if (tokens.Length == 2)
            {
                var secondToken = tokens[1];
                if (allowedTokenTypes.Any(t => t == secondToken.tokens.First().type))
                    return true;
                else
                {
                    var errorString = allowedTokenTypes.Select(t => t.ToString()).Aggregate((current, next) => { return current + "\n" + next; });
                    errorString = "Error: Second token must be any of the following:\n" + errorString;
                    OnError?.Invoke(errorString);
                    return false;
                }
            }
            else if(tokens.Length > 2)
            {
                OnError?.Invoke("Too many arguments for print statement.");
                return false;
            }
            OnError?.Invoke("Unidentified Error.");
            return false;
        }


        bool CheckForAssignment(WordTokens[] tokens)
        {
            var firstTokenType = tokens.First().tokens.First().type;
            if(tokens.Length > 2)
            {
                var secondTokenType = tokens[1].tokens.First().type;
                var thirdTokenType = tokens[2].tokens;

                if (secondTokenType == TokenType.AssignmentOperator)
                {
                    switch (firstTokenType)
                    {
                        case TokenType.IntegerVariable:
                            return thirdTokenType.Any(t => t.type == TokenType.Integer ||
                                                           t.type == TokenType.IntegerVariable || 
                                                           t.type == TokenType.Number);
                        case TokenType.RealVariable:
                            return thirdTokenType.Any(t => t.type == TokenType.Real || 
                                                           t.type == TokenType.RealVariable || 
                                                           t.type == TokenType.Number);
                        case TokenType.StringVariable:
                            return thirdTokenType.Any(t => t.type == TokenType.StringVariable ||
                               t.type == TokenType.String);
                        default:
                            OnError?.Invoke("First token type is not a variable.");
                            return false;
                    }
                }else
                {
                    OnError?.Invoke("Second argument has to be an assignment operator.");
                    return false;
                }
            }
            OnError?.Invoke("Not enough arguments for Assignment Statement");
            return false;
        }

        bool CheckForConditionalstatement(WordTokens [] tokens)
        {
            var conditionalOperators = new TokenType[] { TokenType.AndLogicalOperator, TokenType.OrLogicalOperator, TokenType.NotLogicalOperator,
                                                         TokenType.EqualRelationalOperator, TokenType.NotEqualRelationalOperator, 
                                                         TokenType.GreaterThanRelationalOperator, TokenType.LessThanRelationalOperator,
                                                         TokenType.EqualGreaterRelationalOperator, TokenType.EqualLessRelationalOperator,
                                                        };
            var validTypes = new TokenType[] { TokenType.String, TokenType.StringVariable, TokenType.RealVariable, TokenType.IntegerVariable, TokenType.Number, TokenType.Real, TokenType.Integer };
            var firstToken = tokens.First().tokens.First();

            if(tokens.Length > 2)
            {
                var secondToken = tokens[1].tokens.First();
                var thirdToken = tokens[2].tokens.First();
                if(conditionalOperators.Any(op => secondToken.type == op))
                {
                    switch (secondToken.type)
                    {
                        case TokenType.AndLogicalOperator:
                            break;
                        case TokenType.OrLogicalOperator:
                            break;
                        case TokenType.NotLogicalOperator:
                            break;
                        case TokenType.EqualRelationalOperator:
                            break;
                        case TokenType.NotEqualRelationalOperator:
                            break;
                        case TokenType.GreaterThanRelationalOperator:
                            break;
                        case TokenType.LessThanRelationalOperator:
                            break;
                        case TokenType.EqualGreaterRelationalOperator:
                            break;
                        case TokenType.EqualLessRelationalOperator:
                            break;
                    }
                }
            }
            return false;
        }

        bool CheckForArithmaticStatement(WordTokens[] tokens)
        {

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
                            return typesForAdd.Any(t => t == firstToken.type) && typesForAdd.Any(t => t == thirdToken.type);
                        case TokenType.SubArithmaticOperator:
                            return typesForRest.Any(t => t == firstToken.type) && typesForRest.Any(t => t == thirdToken.type);
                        case TokenType.MulArithmaticOperator:
                            return typesForRest.Any(t => t == firstToken.type) && typesForRest.Any(t => t == thirdToken.type);
                        case TokenType.DivArithmaticOperator:
                            return typesForRest.Any(t => t == firstToken.type) && typesForRest.Any(t => t == thirdToken.type);
                    }
                }else
                {
                    OnError?.Invoke("Second argument must be an arithmatic operator");
                    return false;
                }
            }
            OnError?.Invoke("Not enough arguments for Arithmatic Statement");
            return false;
        }

        bool CheckForReadStatement(WordTokens[] tokens)
        {
            var allowedTokenTypes = new TokenType[] { TokenType.StringVariable,
                                                      TokenType.RealVariable,
                                                      TokenType.IntegerVariable };

            if (tokens.Length == 2)
            {
                var secondToken = tokens[1];
                if (allowedTokenTypes.Any(t => t == secondToken.tokens.First().type))
                    return true;
                else
                {
                    var errorString = allowedTokenTypes.Select(t => t.ToString()).Aggregate((current, next) => { return current + "\n" + next; });
                    errorString = "Error: Second token must be any of the following:\n" + errorString;
                    OnError?.Invoke(errorString);
                    return false;
                }
            }
            else if (tokens.Length > 2)
            {
                OnError?.Invoke("Too many arguments for print statement.");
                return false;
            }
            OnError?.Invoke("Unidentified Error.");
            return false;
        }

        bool Test(WordTokens[] tokens)
        {
            TokenType firstToken;
            WordTokens token;

            if(tokens?.Length > 0 && (token = tokens.First()).tokens?.Length > 0)
            {
                firstToken = token.tokens.First().type;
                switch (firstToken)
                {
                    case TokenType.CommentKeyword:
                        return true;
                    case TokenType.AssignmentOperator:
                        OnError?.Invoke("No statement starts with the Assignment Operator");
                        break;
                    case TokenType.PrintKeyword:
                        return CheckForPrintStatement(tokens);
                    case TokenType.ReadKeyword:
                        return CheckForReadStatement(tokens);
                    case TokenType.IfKeyword:
                        break;
                    case TokenType.EndKeyword:
                        return true;
                    case TokenType.IntegerVariable:
                        return CheckForAssignment(tokens) || CheckForArithmaticStatement(tokens);
                    case TokenType.StringVariable:
                        return CheckForAssignment(tokens) || CheckForArithmaticStatement(tokens);
                    case TokenType.RealVariable:
                        return CheckForAssignment(tokens) || CheckForArithmaticStatement(tokens);
                    case TokenType.Integer:
                        return CheckForArithmaticStatement(tokens);
                    case TokenType.Number:
                        return CheckForArithmaticStatement(tokens);
                    case TokenType.UnsignedInteger:
                        return CheckForArithmaticStatement(tokens);
                    case TokenType.Real:
                        return CheckForArithmaticStatement(tokens);
                    case TokenType.String:
                        return CheckForArithmaticStatement(tokens);
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
            return false;
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
