using AdventOfCode2020.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2020.Days
{
    [Solution(6)]
    class Day06 : ISolution
    {
        private static string[][] ParseInput(string input) => input.Lines(2).Select(group => group.Lines()).ToArray();
        public object PartA(string input)
        {
            return ParseInput(input).Select(group =>
            {
                HashSet<char> answeredQuestions = new HashSet<char>();
                foreach (string answerString in group)
                {
                    foreach (char answer in answerString)
                    {
                        answeredQuestions.Add(answer);
                    }
                }
                return answeredQuestions.Count;
            }).Aggregate(0, (sum, groupTotal) => sum + groupTotal);

        }

        public object PartB(string input)
        {
            return ParseInput(input).Select(group =>
            {
                Dictionary<char, int> answeredQuestions = new Dictionary<char, int>();
                foreach(string answerString in group)
                {
                    foreach (char answer in answerString)
                    {
                        if (answeredQuestions.ContainsKey(answer))
                        {
                            ++answeredQuestions[answer];
                        }
                        else
                        {
                            answeredQuestions.Add(answer, 1);
                        }
                    }
                }
                return answeredQuestions.Count(entry => entry.Value == group.Length);
            }).Aggregate(0, (sum, groupTotal) => sum + groupTotal);
        }
    }
}
