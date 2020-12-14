using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2020.Common
{
    static class Extensions
    {
        private static readonly string[] Newlines = { "\r\n", "\n" };

        /// <summary>
        /// Splits the string into a series of lines as if read from a file. Uses <c>\r\n</c> and <c>\n</c> as delimiters.
        /// </summary>
        /// <param name="source">String to split</param>
        /// <returns>Array of lines</returns>
        public static string[] Lines(this string source, int newlines = 1)
        {
            string[] lines = source.Split(
                Newlines.Select(delim => string.Concat(Enumerable.Repeat(delim, newlines))).ToArray(), 
                StringSplitOptions.None
            );

            // Trailing element
            if (string.IsNullOrWhiteSpace(lines.Last()))
            {
                lines = lines.Take(lines.Length - 1).ToArray();
            }
            else
            {
                // Remove trailing newlines
                foreach (string delim in Newlines)
                {
                    while (lines.Last().EndsWith(delim))
                    {
                        string before = lines[^1];
                        lines[^1] = before.Substring(0, before.Length - delim.Length);
                    }
                }
            }
            return lines;
        }

        public static IEnumerable<int> To(this int start, int end, int step = 1)
        {
            if (step > 0)
            {
                while (start < end)
                {
                    yield return start;
                    start += step;
                }
            }
            else if (step < 0)
            {
                while (start > end)
                {
                    yield return start;
                    start += step;
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(step), "step cannot be zero");
            }
        }

        public static int CountTrailingZeros(this uint i)
        {
            if (i == 0) return 32;

            uint y, n = 31;
            y = i << 16; if (y != 0) { n &= ~16u; i = y; }
            y = i << 8; if (y != 0) { n &= ~8u; i = y; }
            y = i << 4; if (y != 0) { n &= ~4u; i = y; }
            y = i << 2; if (y != 0) { n &= ~2u; i = y; }
            return (int)(n - ((i << 1) >> 31));
        }

        public static int CountTrailingZeros(this ulong i)
        {
            if (i == 0) return 64;

            uint x, y, n = 63;
            y = (uint)i;
            if (y != 0) { n &= ~32u; x = y; }
            else x = (uint)(i >> 32);

            y = x << 16; if (y != 0) { n &= ~16u; x = y; }
            y = x << 8; if (y != 0) { n &= ~8u; x = y; }
            y = x << 4; if (y != 0) { n &= ~4u; x = y; }
            y = x << 2; if (y != 0) { n &= ~2u; x = y; }
            return (int)(n - ((x << 1) >> 31));
        }

        public static int CountLeadingZeros(this uint i)
        {
            if (i == 0) return 32;

            uint n = 1;
            if (i >> 16 == 0) { n |= 16; i <<= 16; }
            if (i >> 24 == 0) { n |= 8; i <<= 8; }
            if (i >> 28 == 0) { n |= 4; i <<= 4; }
            if (i >> 30 == 0) { n |= 2; i <<= 2; }
            n -= i >> 31;
            return (int)n;
        }

        public static int CountLeadingZeros(this ulong i)
        {
            if (i == 0) return 64;

            uint n = 1;
            uint x = (uint)(i >> 32);
            if (x == 0) { n |= 32; x = (uint)i; }

            if (x >> 16 == 0) { n |= 16; x <<= 16; }
            if (x >> 24 == 0) { n |= 8; x <<= 8; }
            if (x >> 28 == 0) { n |= 4; x <<= 4; }
            if (x >> 30 == 0) { n |= 2; x <<= 2; }
            n -= (x >> 31);
            return (int)n;
        }
    }
}
