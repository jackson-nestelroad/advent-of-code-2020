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
        public static string[] Lines(this string source)
        {
            string[] lines = source.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            if (string.IsNullOrWhiteSpace(lines.Last()))
            {
                lines = lines.Take(lines.Length - 1).ToArray();
            }
            return lines;
        }

        public static V GetValue<K, V>(this Dictionary<K, V> dict, K key, V defaultValue = default)
        {
            return dict.TryGetValue(key, out V value) ? value : defaultValue;
        }
    }
}
