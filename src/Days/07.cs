using AdventOfCode2020.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2020.Days
{
    [Solution(7)]
    class Day07 : ISolution
    {
        private static readonly string BagPattern = @"([a-z]+ [a-z]+) bags?";
        private static readonly string OtherBagPattern = @$"([1-9]+) {BagPattern}";

        private static readonly Regex OtherBagRegex = new Regex(OtherBagPattern);
        private static readonly Regex LineRegex = new Regex($"^{BagPattern} contain (.*)$");

        private readonly string MyBag = "shiny gold";

        private static Dictionary<string, List<(string Type, int Count)>> ParseInputAsAncestorMap(string input) {
            var parsedBags = new Dictionary<string, List<(string Type, int Count)>>();
            foreach (string line in input.Lines())
            {
                Match match = LineRegex.Match(line);
                string biggerBag = match.Groups[1].Value;
                string nestedBags = match.Groups[2].Value;

                if (!nestedBags.StartsWith("no"))
                {
                    foreach (Match nestedMatch in OtherBagRegex.Matches(match.Groups[2].Value))
                    {
                        string smallerBagType = nestedMatch.Groups[2].Value;
                        int num = int.Parse(nestedMatch.Groups[1].Value);

                        if (parsedBags.ContainsKey(smallerBagType))
                        {
                            parsedBags[smallerBagType].Add((biggerBag, num));
                        }
                        else
                        {
                            parsedBags.Add(smallerBagType, new List<(string Type, int Count)> { (biggerBag, num) });
                        }
                    }
                }
            }

            return parsedBags;
        }

        private static Dictionary<string, List<(string Type, int Count)>> ParseInputAsChildMap(string input)
        {
            var parsedBags = new Dictionary<string, List<(string Type, int Count)>>();
            foreach (string line in input.Lines())
            {
                Match match = LineRegex.Match(line);
                string biggerBag = match.Groups[1].Value;
                string nestedBags = match.Groups[2].Value;

                var smallerBagMap = new List<(string Type, int Count)>();
                if (!nestedBags.StartsWith("no"))
                {
                    foreach (Match nestedMatch in OtherBagRegex.Matches(match.Groups[2].Value))
                    {
                        string smallerBagType = nestedMatch.Groups[2].Value;
                        int num = int.Parse(nestedMatch.Groups[1].Value);

                        smallerBagMap.Add((smallerBagType, num));
                    }
                }
                parsedBags.Add(biggerBag, smallerBagMap);
            }

            return parsedBags;
        }

        private int CountBiggerBags(string bagType, Dictionary<string, List<(string Type, int Count)>> ancestorMap, HashSet<string> seen)
        {
            seen.Add(bagType);
            int count = 0;
            if (ancestorMap.ContainsKey(bagType))
            {
                foreach (var entry in ancestorMap[bagType])
                {
                    if (!seen.Contains(entry.Type))
                    {
                        count += 1 + CountBiggerBags(entry.Type, ancestorMap, seen);
                    }
                }
            }
            return count;
        }

        private int CountNestedBags(string bagType, Dictionary<string, List<(string Type, int Count)>> childMap, bool nested = false)
        {
            int count = nested ? 1 : 0;
            foreach (var entry in childMap[bagType])
            {
                count += entry.Count * CountNestedBags(entry.Type, childMap, true);
            }
            return count;
        }

        public object PartA_original(string input)
        {
            return CountBiggerBags(MyBag, ParseInputAsAncestorMap(input), new HashSet<string>());
        }

        public object PartB_original(string input)
        {
            return CountNestedBags(MyBag, ParseInputAsChildMap(input));
        }

        // After solving, I decided to go back and use an object-oriented design for the problem
        // The following solution is much cleaner and closer to C#-style code

        private class Bag
        {
            public string Type;
            public Dictionary<Bag, int> Children = new Dictionary<Bag, int>();
            public List<Bag> Ancestors = new List<Bag>();
        }

        private static Dictionary<string, Bag> ParseInput(string input)
        {
            Dictionary<string, Bag> parsedBags = new Dictionary<string, Bag>();
            Dictionary<string, string> contentToParse = new Dictionary<string, string>();

            // Create an instance for each bag
            foreach (string line in input.Lines())
            {
                Match match = LineRegex.Match(line);
                string bagType = match.Groups[1].Value;
                string nestedBags = match.Groups[2].Value;

                parsedBags.Add(bagType, new Bag() { Type = bagType });
                contentToParse.Add(bagType, nestedBags);
            }

            // Parse each bag further
            foreach (Bag bag in parsedBags.Values)
            {
                string content = contentToParse[bag.Type];
                if (!content.StartsWith("no"))
                {
                    foreach (Match nestedMatch in OtherBagRegex.Matches(content))
                    {
                        string smallerBagType = nestedMatch.Groups[2].Value;
                        int num = int.Parse(nestedMatch.Groups[1].Value);

                        bag.Children.Add(parsedBags[smallerBagType], num);
                        parsedBags[smallerBagType].Ancestors.Add(bag);
                    }
                }
            }

            return parsedBags;
        }

        public object PartA(string input)
        {
            Dictionary<string, Bag> bags = ParseInput(input);
            HashSet<string> biggerBags = new HashSet<string>();
            Stack<Bag> stack = new Stack<Bag>();
            stack.Push(bags[MyBag]);

            // DFS
            while (stack.Count > 0)
            {
                Bag next = stack.Pop();
                foreach (Bag ancestor in next.Ancestors)
                {
                    biggerBags.Add(ancestor.Type);
                    stack.Push(ancestor);
                }
            }

            return biggerBags.Count;
        }

        private int CountTotalBagsInside(Bag bag, Dictionary<string, Bag> bags)
        {
            int count = 1;
            foreach (var entry in bag.Children)
            {
                count += entry.Value * CountTotalBagsInside(entry.Key, bags);
            }
            return count;
        }

        public object PartB(string input)
        {
            Dictionary<string, Bag> bags = ParseInput(input);
            // Subtract 1 because it counts our bag as one bag
            return CountTotalBagsInside(bags[MyBag], bags) - 1;
        }
    }
}
