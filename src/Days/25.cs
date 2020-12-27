using AdventOfCode2020.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2020.Days
{
    [Solution(25)]
    class Day25 : ISolution
    {
        private const int subjectNumber = 7;
        private const uint divisor = 20201227;

        private static (uint Card, uint Door) ParseInput(string input)
        {
            IEnumerator<uint> publicKeys = input.Lines().Select(str => uint.Parse(str)).GetEnumerator();
            publicKeys.MoveNext();
            uint card = publicKeys.Current;
            publicKeys.MoveNext();
            uint door = publicKeys.Current;
            return (card, door);
        }

        // Initial brute force solution
        public object PartABruteForce(string input)
        {
            (uint card, uint door) = ParseInput(input);
            // Find loop size while also calculating encryption key, so we only need to loop once
            ulong encryptionKey = 1;
            for (ulong calculatedKey = 1; calculatedKey != card;)
            {
                calculatedKey = (calculatedKey * subjectNumber) % divisor;
                encryptionKey = (encryptionKey * door) % divisor;
            }

            return encryptionKey;
        }

        // Iterative modular exponentiation
        private static uint PowMod(ulong a, uint p, uint mod)
        {
            if (p == 0)
            {
                return 1;
            }

            ulong res = 1;
            a %= mod;
            while (p > 0)
            {
                // Odd power
                if ((p & 1) != 0)
                {
                    res = (res * a) % mod;
                }

                // Even power
                p >>= 1;
                a = (a * a) % mod;
            }
            return (uint)res;
        }

        // Compute x where g^x % mod == target
        // Baby-step giant-step algorithm
        private static uint? DiscreteLog(uint g, uint target, uint mod)
        {
            uint m = (uint)Math.Ceiling(Math.Sqrt(mod));
            Dictionary<uint, uint> table = new Dictionary<uint, uint>();
            ulong e = 1;
            for (uint i = 0; i < m; ++i)
            {
                table[(uint)e] = i;
                e = (e * g) % mod;
            }
            uint factor = PowMod(g, mod - m - 1, mod);
            e = target;
            for (uint i = 0; i < m; ++i)
            {
                if (table.TryGetValue((uint)e, out uint res))
                {
                    return i * m + res;
                }
                e = (e * factor) % mod;
            }
            return null;
        }

        public object PartA(string input)
        {
            (uint card, uint door) = ParseInput(input);
            // Calculate x, where 7 ^ x % 20201227 == card
            // Then calculate door ^ x % 20201227
            return PowMod(door, DiscreteLog(subjectNumber, card, divisor).Value, divisor);
        }

        public object PartB(string input)
        {
            return "Pay the Deposit";
        }
    }
}
