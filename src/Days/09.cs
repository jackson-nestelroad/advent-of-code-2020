using AdventOfCode2020.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2020.Days
{
    [Solution(9)]
    class Day09 : ISolution
    {
        private const int PreambleLength = 25;

        private static long[] ParseInput(string input) => input.Lines().Select(str => long.Parse(str)).ToArray();

        public object PartA(string input)
        {
            long[] data = ParseInput(input);
            for (int i = PreambleLength; i < data.Length; ++i)
            {
                long current = data[i];
                bool valid = false;
                
                // Look for two numbers that sum to the current number in the last p numbers - O(p)
                HashSet<long> needed = new HashSet<long>();
                for (int j = i - PreambleLength; j < i && !valid; ++j)
                {
                    if (needed.Contains(data[j]))
                    {
                        valid = true;
                    }
                    else
                    {
                        needed.Add(current - data[j]);
                    }
                }
                
                // First number to not be valid
                if (!valid)
                {
                    return current;
                }
            }

            throw new SolutionFailedException();
        }

        private (int Begin, int End) GetContiguousSubarrayWithSum(long[] data, long target)
        {
            int n = data.Length;
            long[,] sumTable = new long[n, n];

            // Initialize diagonals
            for (int i = 0; i < n; ++i)
            {
                sumTable[i, i] = data[i];
            }

            // Fill in the sum table diagonally until we find a match
            for (int l = 1; l < n; ++l)
            {
                // l == j - i
                int end = n - l;
                for (int i = 0; i < end; ++i)
                {
                    int j = i + l;
                    sumTable[i, j] = sumTable[i, i] + sumTable[i + 1, j];

                    // Found our contiguous subarray with target sum
                    if (sumTable[i, j] == target)
                    {
                        return (i, j);
                    }
                }
            }

            throw new SolutionFailedException();
        }

        public object PartB(string input)
        {
            long[] data = ParseInput(input);
            long invalidNumber = (long)PartA(input);
            (int i, int j) = GetContiguousSubarrayWithSum(data, invalidNumber);
            long[] subarray = data[i..(j + 1)];
            return subarray.Min() + subarray.Max();
        }
    }
}
