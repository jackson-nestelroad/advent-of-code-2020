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

        private static int TreesHit(string[] map, Point2D slope)
        {
            int frameWidth = map[0].Length;
            int treeCount = 0;
            for (Point2D pos = new Point2D(0, 0); pos.Y < map.Length; pos += slope)
            {
                if (map[pos.Y][pos.X % frameWidth] == (char)Tiles.Tree)
                {
                    ++treeCount;
                }
            }
            return treeCount;
        }

        public object PartA(string[] input)
        {
            return TreesHit(input, new Point2D(3, 1));
        }

        public object PartB(string[] input)
        {
            Point2D[] slopes =
            {
                new Point2D(1, 1),
                new Point2D(3, 1),
                new Point2D(5, 1),
                new Point2D(7, 1),
                new Point2D(1, 2)
            };
            return slopes.Select(slope => TreesHit(input, slope)).Aggregate(1L, (total, trees) => total * trees);
        }
    }
}
