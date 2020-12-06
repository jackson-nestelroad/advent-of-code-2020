using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2020.Common
{
    static class Extensions
    {
        /// <summary>
        /// Splits the string into a series of lines as if read from a file. Uses <c>\r\n</c> and <c>\n</c> as delimiters.
        /// </summary>
        /// <param name="source">String to split</param>
        /// <returns>Array of lines</returns>
        public static string[] Lines(this string source, int newlines = 1)
        {
            string[] delims = { "\r\n", "\n" };
            for (int i = 0; i < delims.Length; ++i)
            {
                delims[i] = string.Concat(Enumerable.Repeat(delims[i], newlines));
            }

            string[] lines = source.Split(delims, StringSplitOptions.None);

            // Trailing element
            if (string.IsNullOrWhiteSpace(lines.Last()))
            {
                lines = lines.Take(lines.Length - 1).ToArray();
            }
            else
            {
                // Remove trailing newlines
                foreach (string delim in delims)
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
    }
}
