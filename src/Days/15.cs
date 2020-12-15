using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2020.Days
{
    [Solution(15)]
    class Day15 : ISolution
    {
        private static int[] ParseInput(string input) => input.Split(',').Select(n => int.Parse(n)).ToArray();

        private static int PlayGameUpToTurn(int[] startingNumbers, int target)
        {
            int t = 1;
            int previous = startingNumbers[0];
            Dictionary<int, int> history = new Dictionary<int, int>();
            for (int i = 1; i < startingNumbers.Length; ++i, ++t)
            {
                history[previous] = t;
                previous = startingNumbers[i];
            }

            for (; t < target; ++t)
            {
                int next = history.ContainsKey(previous) ? t - history[previous] : 0;
                history[previous] = t;
                previous = next;
            }

            return previous;
        }

        public object PartA(string input)
        {
            return PlayGameUpToTurn(ParseInput(input), 2020);
        }

        public object PartB(string input)
        {
            // Van Eck Sequence
            // No optimization possible, so we are forced to use a "brute force" solution
            return PlayGameUpToTurn(ParseInput(input), 30000000);
        }
    }
}
