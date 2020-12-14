using AdventOfCode2020.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2020.Days
{
    [Solution(14)]
    class Day14 : ISolution
    {
        private const int Bits = 36;
        private static readonly Regex MaskRegex = new Regex(@"^mask = ([0-1X]{36})$");
        private static readonly Regex MemRegex = new Regex(@"^mem\[([0-9]+)\] = ([0-9]+)$");

        private class InputLine 
        { };

        private class BitMask : InputLine
        {
            private ulong And { get; init; }
            private ulong Or { get; init; }
            private List<int> Floating { get; init; }

            public BitMask(string mask)
            {
                And = Convert.ToUInt64(mask.Replace('X', '1'), 2);
                Or = Convert.ToUInt64(mask.Replace('X', '0'), 2);
                Floating = new List<int>();
                for (int i = 0; i < mask.Length; ++i)
                {
                    if (mask[i] == 'X')
                    {
                        Floating.Add(i);
                    }
                }
            }

            public ulong ApplyToValue(ulong bits)
            {
                bits &= And;
                bits |= Or;
                return bits;
            }

            public IEnumerable<ulong> ApplyToAddress(ulong bits)
            {
                bits |= Or;
                
                // Unset all floating bits to start
                foreach (int bit in Floating)
                {
                    bits &= ~(1UL << (Bits - 1 - bit));
                }

                yield return bits;

                // Iterate using gray codes, toggling a bit at a time
                int combinations = 1 << Floating.Count;
                uint prev = 0;
                for (int i = 1; i < combinations; ++i)
                {
                    // Next gray code
                    uint grayCode = (uint)(i ^ (i >> 1));
                    uint toggled = grayCode ^ prev;
                    prev = grayCode;

                    // Get the index of the bit that was shifted
                    int bitIndex = toggled.CountTrailingZeros();

                    // The next bit to toggle
                    ulong nextBit = 1UL << (Bits - 1 - Floating[bitIndex]);

                    // Unset the bit
                    if ((grayCode & toggled) == 0)
                    {
                        bits &= ~nextBit;
                    }
                    // Set the bit
                    else
                    {
                        bits |= nextBit;
                    }

                    yield return bits;
                }
            }
        }

        private class MemoryWrite : InputLine
        {
            public ulong Address { get; init; }
            public ulong Value { get; init; }
        }

        private static List<InputLine> ParseInput(string input)
        {
            return input.Lines().Select<string, InputLine>(line =>
            {
                Match maskMatch = MaskRegex.Match(line);
                if (maskMatch.Success)
                {
                    return new BitMask(maskMatch.Groups[1].Value);
                }
                else
                {
                    Match memMatch = MemRegex.Match(line);
                    if (memMatch.Success)
                    {
                        return new MemoryWrite()
                        {
                            Address = uint.Parse(memMatch.Groups[1].Value),
                            Value = uint.Parse(memMatch.Groups[2].Value)
                        };
                    }
                }
                throw new InputParseException();
            }).ToList();
        }

        public object PartA(string input)
        {
            List<InputLine> lines = ParseInput(input);
            Dictionary<ulong, ulong> memory = new Dictionary<ulong, ulong>();
            BitMask currentMask = null;
            foreach (InputLine line in lines)
            {
                if (line is BitMask mask)
                {
                    currentMask = mask;
                }
                else if (line is MemoryWrite memoryWrite)
                {
                    memory[memoryWrite.Address] = currentMask.ApplyToValue(memoryWrite.Value);
                }
            }
            return memory.Values.Aggregate(0UL, (sum, value) => sum + value);
        }

        public object PartB(string input)
        {
            List<InputLine> lines = ParseInput(input);
            Dictionary<ulong, ulong> memory = new Dictionary<ulong, ulong>();
            BitMask currentMask = null;
            foreach (InputLine line in lines)
            {
                if (line is BitMask mask)
                {
                    currentMask = mask;
                }
                else if (line is MemoryWrite memoryWrite)
                {
                    foreach (ulong address in currentMask.ApplyToAddress(memoryWrite.Address))
                    {
                        memory[address] = memoryWrite.Value;
                    }
                }
            }
            return memory.Values.Aggregate(0UL, (sum, value) => sum + value);
        }
    }
}
