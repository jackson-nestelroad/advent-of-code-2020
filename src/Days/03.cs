using AdventOfCode2020.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2020.Days
{
    [Solution(3)]
    class Day03 : ISolution
    {
        private enum Tiles
        {
            Open = '.',
            Tree = '#'
        }

        private static int TreesHit(string[] input, Point slope)
        {
            int frameWidth = input[0].Length;
            int treeCount = 0;
            for (int x = 0, y = 0; y < input.Length; y += slope.Y, x += slope.X)
            {
                if (input[y][x % frameWidth] == (char)Tiles.Tree)
                {
                    ++treeCount;
                }
            }
            return treeCount;
        }

        public object PartA(string[] input)
        {
            return TreesHit(input, new Point(3, 1));
        }

        public object PartB(string[] input)
        {
            Point[] slopes =
            {
                new Point(1, 1),
                new Point(3, 1),
                new Point(5, 1),
                new Point(7, 1),
                new Point(1, 2)
            };
            return slopes.Select(slope => TreesHit(input, slope)).Aggregate(1L, (total, trees) => total * trees);
        }
    }
}
