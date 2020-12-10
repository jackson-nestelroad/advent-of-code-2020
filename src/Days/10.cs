using AdventOfCode2020.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2020.Days
{
    [Solution(10)]
    class Day10 : ISolution
    {
        private static List<int> ParseInput(string input) => input.Lines().Select(num => int.Parse(num)).ToList();

        public object PartA(string input)
        {
            List<int> joltageRatings = ParseInput(input);

            // Connect the adapters
            joltageRatings.Sort();

            Dictionary<int, int> differences = new Dictionary<int, int>()
            {
                { 1, 0 },
                { 2, 0 },
                { 3, 1 }    // Last difference is always 3
            };
            // First difference is joltageRatings[0] - 0
            ++differences[joltageRatings[0]];

            // Get all differences
            for (int i = 1; i < joltageRatings.Count; ++i)
            {
                ++differences[joltageRatings[i] - joltageRatings[i - 1]];
            }

            return differences[1] * differences[3];
        }

        public object PartB(string input)
        {
            List<int> joltageRatings = ParseInput(input);

            // Add minimum and maximum to generalize solution
            const int outletJoltage = 0;
            int deviceJoltage = joltageRatings.Max() + 3;
            joltageRatings.Add(outletJoltage);
            joltageRatings.Add(deviceJoltage);
            
            // Sort to build the default connection
            joltageRatings.Sort();

            Func<int, int> nC2 = n => (n * (n - 1)) / 2; 

            int end = joltageRatings.Count - 1;
            long combinations = 1;
            for (int i = 0; i < end; ++i)
            {
                int diff = joltageRatings[i + 1] - joltageRatings[i];
                if (diff == 1)
                {
                    int j = i + 1;
                    for (; j < end && joltageRatings[j + 1] - joltageRatings[j] == 1; ++j) ;
                    // [i, j] is a series of adapters with a difference of 1
                    int seriesLength = j - i;
                    
                    // C(n, 2) + 1
                    int multiplier = nC2(seriesLength) + 1;

                    // Series of 1's has a preceding 2
                    if (i > 0 && joltageRatings[i] - joltageRatings[i - 1] == 2)
                    {
                        multiplier += nC2(seriesLength + 1) + 1;
                    }

                    combinations *= multiplier;
                    i = j;
                }
            }
            return combinations;
        }
    }
}
