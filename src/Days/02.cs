using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode2020.Days
{
    [Solution(2)]
    class Day02 : ISolution
    {
        class Password
        {
            public int Min { get; set; }
            public int Max { get; set; }
            public char Required { get; set; }
            public string Value { get; set; }
        }

        private static Regex Pattern = new Regex(@"(\d+)-(\d+) ([a-z]): ([a-z]+)");

        private static List<Password> ParseInput(string[] input)
        {
            return input.Select(str =>
            {
                var match = Pattern.Match(str);
                if (!match.Success)
                {
                    throw new InputParseException();
                }
                var groups = match.Groups;
                return new Password 
                {
                    Min = int.Parse(groups[1].Value),
                    Max = int.Parse(groups[2].Value),
                    Required = char.Parse(groups[3].Value),
                    Value = groups[4].Value
                };
            }).ToList();
        }

        public object PartA(string[] input)
        {
            List<Password> passwords = ParseInput(input);
            return passwords
                .Where(password =>
                {
                    int count = password.Value.Count(c => c == password.Required);
                    return count >= password.Min && count <= password.Max;
                })
                .Count();
        }

        public object PartB(string[] input)
        {
            List<Password> passwords = ParseInput(input);
            return passwords
                .Where(password =>
                {
                    int first = password.Min - 1;
                    int second = password.Max - 1;
                    return first < password.Value.Length
                        && second < password.Value.Length
                        && (password.Value[first] == password.Required || password.Value[second] == password.Required)
                        && password.Value[first] != password.Value[second];
                })
                .Count();
        }
    }
}
