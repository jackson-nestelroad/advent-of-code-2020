using AdventOfCode2020.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2020.Days
{
    [Solution(19)]
    class Day19 : ISolution
    {
        private abstract class AbstractRule
        {
            public int Id { get; }
            protected AbstractRule(int id)
            {
                Id = id;
            }

            public abstract string ToPattern();
            public abstract string ToUpdatedPattern();
        }

        private class BaseRule : AbstractRule
        {
            public char Char { get; }
            public BaseRule(int id, char c) : base(id)
            {
                Char = c;
            }

            public override string ToPattern() => Char.ToString();
            public override string ToUpdatedPattern() => Char.ToString();
        }

        private class CompoundRule : AbstractRule
        {
            public List<List<AbstractRule>> SubRules { get; }
            public CompoundRule(int id) : base(id)
            {
                SubRules = new List<List<AbstractRule>>();
            }

            public override string ToPattern()
            {
                string pattern = "(?:";
                pattern += string.Join('|', SubRules
                    .Select(list => list.Aggregate("", (subPattern, subRule) => subPattern + subRule.ToPattern()))
                    .Select(subPattern => "(?:" + subPattern + ')'));
                pattern += ')';
                return pattern;
            }

            public override string ToUpdatedPattern()
            {
                // We can manually create strings for the rules we need to update
                // If not, we would need to build an entire formal grammar parser!

                string pattern = "(?:";
                if (Id == 8)
                {
                    pattern += SubRules[0][0].ToUpdatedPattern();
                    pattern += '+';
                }
                else if (Id == 11)
                {
                    // Use .NET Balancing Group Definitions
                    pattern += "(?<open>";
                    pattern += SubRules[0][0].ToUpdatedPattern();
                    pattern += ")+(?<close-open>";
                    pattern += SubRules[0][1].ToUpdatedPattern();
                    pattern += ")+(?(open)(?!))";
                }
                else
                {
                    pattern += string.Join('|', SubRules
                        .Select(list => list.Aggregate("", (subPattern, subRule) => subPattern + subRule.ToUpdatedPattern()))
                        .Select(subPattern => "(?:" + subPattern + ')'));
                }
                pattern += ')';
                return pattern;
            }
        }

        private static Dictionary<int, AbstractRule> ParseInput(string input)
        {
            Dictionary<int, AbstractRule> rules = new Dictionary<int, AbstractRule>();
            Dictionary<int, string> compoundRules = new Dictionary<int, string>();
            foreach (string line in input.Lines())
            {
                int colonIndex = line.IndexOf(':');
                int id = -1;
                if (colonIndex == -1 || !int.TryParse(line[0..colonIndex], out id))
                {
                    throw new InputParseException($"No colon found for string \"{line}\"");
                }

                // Base rule found
                if (line[colonIndex + 2] == '"')
                {
                    rules.Add(id, new BaseRule(id, line[colonIndex + 3]));
                }
                // Compound rule found
                else
                {
                    rules.Add(id, new CompoundRule(id));
                    compoundRules.Add(id, line[(colonIndex + 1)..]);
                }
            }

            // Connect compound rules to object references
            foreach ((int id, string list) in compoundRules)
            {
                if (rules[id] is CompoundRule rule)
                {
                    rule.SubRules.AddRange(
                        list
                        .Split('|')
                        .Select(subList =>
                            Regex.Split(subList, @"\D+")
                            .Where(str => !string.IsNullOrEmpty(str))
                            .Select(n => rules[int.Parse(n)])
                            .ToList()
                        )
                    );
                }
            }

            return rules;
        }

        // The following solution is less efficient than using Regexes, but it is more generic

        private bool MatchesRules(string msg, Queue<AbstractRule> rules, int offset)
        {
            // Matching has finished
            if (rules.Count == 0 || offset == msg.Length)
            {
                // No more rules must be matched and end of string has been reached
                return rules.Count == 0 && offset == msg.Length;
            }

            AbstractRule nextRule = rules.Dequeue();
            if (nextRule is BaseRule baseRule)
            {
                if (msg[offset] == baseRule.Char)
                {
                    return MatchesRules(msg, rules, offset + 1);
                }
            }
            else if (nextRule is CompoundRule compoundRule)
            {
                return compoundRule.SubRules.Any(subRuleList =>
                {
                    // We lose the most time here, creating a copy of the current rule queue
                    // We create a copy so that the state of the rules needed to match is restored
                    // when one branch fails and another one is taken
                    Queue<AbstractRule> rulesCopy = new Queue<AbstractRule>(subRuleList);
                    foreach (AbstractRule laterRule in rules)
                    {
                        rulesCopy.Enqueue(laterRule);
                    }
                    return MatchesRules(msg, rulesCopy, offset);
                });
            }

            return false;
        }

        private bool MatchesRule(string msg, AbstractRule rule)
        {
            Queue<AbstractRule> rules = new Queue<AbstractRule>();
            rules.Enqueue(rule);
            return MatchesRules(msg, rules, 0);
        }

        public object PartA(string input)
        {
            string[] inputGroups = input.Lines(2);
            Dictionary<int, AbstractRule> rules = ParseInput(inputGroups[0]);
            string[] messages = inputGroups[1].Lines();

            return messages.Count(msg => MatchesRule(msg, rules[0]));

            // Regex ruleZero = new Regex('^' + rules[0].ToPattern() + '$');
            // return messages.Where(message => ruleZero.IsMatch(message)).Count();
        }

        public object PartB(string input)
        {
            string[] inputGroups = input.Lines(2);
            Dictionary<int, AbstractRule> rules = ParseInput(inputGroups[0]);
            string[] messages = inputGroups[1].Lines();

            CompoundRule rule8 = rules[8] as CompoundRule;
            rule8.SubRules.Add(new List<AbstractRule>() { rules[42], rules[8] });

            CompoundRule rule11 = rules[11] as CompoundRule;
            rule11.SubRules.Add(new List<AbstractRule>() { rules[42], rules[11], rules[31] });

            return messages.Count(msg => MatchesRule(msg, rules[0]));

            // Regex ruleZero = new Regex('^' + rules[0].ToUpdatedPattern() + '$');
            // return messages.Where(message => ruleZero.IsMatch(message)).Count();
        }
    }
}
