using AdventOfCode2020.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Days
{
    [Solution(1)]
    class Day01 : ISolution
    {
        private const int Target = 2020;

        private static int[] ParseInput(string input) => input.Lines().Select(num => int.Parse(num)).ToArray();

        public object PartA(string input)
        {
            int[] numbers = ParseInput(input);

            // Store the number needed to complete the sum
            HashSet<int> neededToCompleteSum = new HashSet<int>();
            foreach (int num in numbers)
            {
                int diff = Target - num;
                if (neededToCompleteSum.Contains(num))
                {
                    return num * diff;
                }

                neededToCompleteSum.Add(diff);
            }

            throw new SolutionFailedException();
        }

        public object PartB(string input)
        {
            // Run part A algorithm, but anchor the third number of the sum at index i
            // So the target is now (Target - numbers[i])

            int[] numbers = ParseInput(input);
            for (int i = 0; i < numbers.Length; ++i)
            {
                int target = Target - numbers[i];
                HashSet<int> neededToCompleteSum = new HashSet<int>();
                for (int j = i + 1; j < numbers.Length; ++j)
                {
                    int num = numbers[j];
                    int diff = target - num;
                    if (neededToCompleteSum.Contains(num))
                    {
                        return numbers[i] * num * diff;
                    }

                    neededToCompleteSum.Add(diff);
                }
            }

            throw new SolutionFailedException();
        }
    }
}
