using AdventOfCode2020.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2020.Days
{
    [Solution(5)]
    class Day05 : ISolution
    {
        private const int RowBits = 7;
        private const int ColBits = 3;
        private const int TotalBits = RowBits + ColBits;

        private static int[] ParseInput(string input)
        {
            return input.Lines().Select(str =>
            {
                int id = 0;
                int i = 0;
                for (; i < RowBits; ++i)
                {
                    if (str[i] == 'B')
                    {
                        id |= 1 << (TotalBits - 1 - i);
                    }
                }
                for (; i < TotalBits; ++i)
                {
                    if (str[i] == 'R')
                    {
                        id |= 1 << (TotalBits - 1 - i);
                    }
                }
                return id;
            }).ToArray();
        }

        public object PartA(string input)
        {
            return ParseInput(input).Max();
        }

        public object PartB(string input)
        {
            int[] seats = ParseInput(input);

            HashSet<int> seenSeats = new HashSet<int>();
            foreach (int id in seats)
            {
                seenSeats.Add(id);
            }

            return 1.To(1 << TotalBits)
                .FirstOrDefault(id => !seenSeats.Contains(id) && seenSeats.Contains(id + 1) && seenSeats.Contains(id - 1));
        }
    }
}
