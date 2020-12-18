using AdventOfCode2020.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2020.Days
{
    [Solution(18)]
    class Day18 : ISolution
    {
        private class Operator : IEquatable<Operator>
        {
            public delegate long Perform(long left, long right);

            public string Symbol { get; init; }
            public int Precedence { get; init; } = 0;
            public bool RightAssociative { get; init; } = false;
            public Perform Operation { get; init; } = null;

            public Operator(string symbol)
            {
                Symbol = symbol;
            }

            public override bool Equals(object? obj) => obj is Operator other && Equals(other);
            public bool Equals(Operator other) => Symbol == other.Symbol;
            public static bool operator ==(Operator left, Operator right) => left.Equals(right);
            public static bool operator !=(Operator left, Operator right) => !(left == right);
            public override int GetHashCode() => Symbol.GetHashCode();

            public static readonly Operator LeftParenthesis = new Operator("(");
            public static readonly Operator RightParenthesis = new Operator(")");
        }

        private static string[] ParseInput(string input) => input.Lines();

        // Implementation of the shunting-yard algorithm, but evaluating immediately rather than building a postfix string

        private long Evaluate(string equation, Dictionary<string, Operator> operators)
        {
            Stack<long> operandStack = new Stack<long>();
            Stack<Operator> operatorStack = new Stack<Operator>();

            for (int i = 0; i < equation.Length; ++i)
            {
                // Ignore whitespace
                if (char.IsWhiteSpace(equation[i]))
                {
                    continue;
                }

                // Parse number
                if (char.IsDigit(equation[i]))
                {
                    // Input only has single digit numbers
                    // Could parse whole digits here if needed
                    operandStack.Push(equation[i] - '0');
                }
                // Parse operator
                else
                {
                    // Generalized token parsing that allows for multi-character operators
                    string token = equation[i].ToString();

                    // While next character can be added to the operator, add it
                    for (int j = i + 1; j < equation.Length; ++j)
                    {
                        char next = equation[j];
                        if (char.IsWhiteSpace(next) || char.IsDigit(next))
                        {
                            break;
                        }

                        token += next;
                        i = j;
                    }

                    // Match the longest operator possible
                    bool matched = operators.ContainsKey(token);
                    while (!matched && token.Length > 0)
                    {
                        // Remove last character, try to match again
                        token = token.Remove(token.Length - 1, 1);
                        --i;
                        matched = operators.ContainsKey(token);
                    }
                    
                    // Token is an operator
                    if (matched)
                    {
                        Operator op = operators[token];
                        if (op == Operator.LeftParenthesis)
                        {
                            operatorStack.Push(op);
                        }
                        else if (op == Operator.RightParenthesis)
                        {
                            // Evaluate everything within the parenthesis
                            while (operatorStack.Peek() != Operator.LeftParenthesis)
                            {
                                Operator nextOp = operatorStack.Pop();
                                long right = operandStack.Pop();
                                long left = operandStack.Pop();
                                operandStack.Push(nextOp.Operation(left, right));
                            }

                            // Remove left parenthesis
                            // Fails on mismatched parenthesis
                            operatorStack.Pop();
                        }
                        else
                        {
                            // Evaluate the operator if precedence and associativity allows
                            while (operatorStack.Count > 0
                                && (operatorStack.Peek().Precedence > op.Precedence
                                    || (operatorStack.Peek().Precedence == op.Precedence && !op.RightAssociative)
                                )
                                && operatorStack.Peek() != Operator.LeftParenthesis)
                            {
                                Operator nextOp = operatorStack.Pop();
                                long right = operandStack.Pop();
                                long left = operandStack.Pop();
                                operandStack.Push(nextOp.Operation(left, right));
                            }
                            operatorStack.Push(op);
                        }
                    }
                    // Token is some unknown symbol
                    else
                    {
                        throw new SolutionFailedException($"Unknown symbol {token}");
                    }
                }
            }

            // Evaluate any remaining operators
            while (operatorStack.Count > 0)
            {
                Operator nextOp = operatorStack.Pop();
                long right = operandStack.Pop();
                long left = operandStack.Pop();
                operandStack.Push(nextOp.Operation(left, right));
            }

            // Entire result is the last value on the operand stack
            return operandStack.Peek();
        }

        public object PartA(string input)
        {
            string[] equations = ParseInput(input);
            List<Operator> operators = new List<Operator>
            {
                new Operator("+") { Operation = (long a, long b) => a + b },
                new Operator("*") { Operation = (long a, long b) => a * b },
                Operator.LeftParenthesis,
                Operator.RightParenthesis
            };

            Dictionary<string, Operator> operatorMap = operators.ToDictionary(op => op.Symbol);

            long sum = 0;
            foreach (string equation in equations)
            {
                sum += Evaluate(equation, operatorMap);
            }
            return sum;
        }

        public object PartB(string input)
        {
            string[] equations = ParseInput(input);
            List<Operator> operators = new List<Operator>
            {
                new Operator("+") { Precedence = 2, Operation = (long a, long b) => a + b },
                new Operator("*") { Precedence = 1, Operation = (long a, long b) => a * b },
                Operator.LeftParenthesis,
                Operator.RightParenthesis
            };

            Dictionary<string, Operator> operatorMap = operators.ToDictionary(op => op.Symbol);

            long sum = 0;
            foreach (string equation in equations)
            {
                sum += Evaluate(equation, operatorMap);
            }
            return sum;
        }
    }
}
