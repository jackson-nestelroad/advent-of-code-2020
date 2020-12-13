using AdventOfCode2020.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2020.Days
{
    [Solution(13)]
    class Day13 : ISolution
    {
        public object PartA(string input)
        {
            string[] lines = input.Lines();
            int timestamp = int.Parse(lines[0]);
            IEnumerable<int> buses = lines[1].Split(',').Where(num => num != "x").Select(num => int.Parse(num));
            (int maxId, int maxMod) = (0, 0);
            foreach (int busId in buses)
            {
                int mod = timestamp % busId;
                if (mod > maxMod)
                {
                    maxId = busId;
                    maxMod = mod;
                }
            }
            return maxId * (maxId - maxMod);
        }

        private long ModularInverse(long a, long m)
        {
            if (m == 1)
            {
                return 0;
            }

            long m0 = m;
            (long x, long y) = (1, 0);
            while (a > 1)
            {
                long q = a / m;
                (a, m) = (m, a % m);
                (x, y) = (y, x - q * y);
            }
            return x < 0 ? x + m0 : x;
        }

        private long ChineseRemainderTheorem(List<(int A, int N)> equations)
        {
            long prod = equations.Aggregate(1L, (prod, e) => prod * e.N);
            long sum = 0;
            for (int i = 0; i < equations.Count; ++i)
            {
                (int a, int n) = equations[i];
                long p = prod / n;
                sum += (a < 0 ? a + n : a) * ModularInverse(p, n) * p;
            }
            return sum % prod;
        }

        public object PartB(string input)
        {
            string[] entries = input.Lines()[1].Split(',');
            List<(int A, int N)> equations = new List<(int A, int N)>();
            for (int i = 0; i < entries.Length; ++i)
            {
                if (entries[i] != "x")
                {
                    equations.Add((A: -i, N: int.Parse(entries[i])));
                }
            }
            return ChineseRemainderTheorem(equations);
        }
    }
}
