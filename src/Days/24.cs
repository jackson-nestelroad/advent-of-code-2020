using AdventOfCode2020.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2020.Days
{
    [Solution(24)]
    class Day24 : ISolution
    {
        private enum Direction
        {
            North = 'n',
            South = 's',
            East = 'e',
            West = 'w'
        }

        private HashSet<Point2D> GetBlackTiles(string[] directions)
        {
            // Represent hexagons using an axial coordinate system
            // See https://www.redblobgames.com/grids/hexagons/#coordinates
            // x => q, y => r
            HashSet<Point2D> blackTiles = new HashSet<Point2D>();
            foreach (string line in directions)
            {
                int q = 0;
                int r = 0;
                for (int i = 0; i < line.Length; ++i)
                {
                    // Moving diagonally
                    switch ((Direction)line[i])
                    {
                        case Direction.North:
                            {
                                --r;
                                if ((Direction)line[++i] == Direction.East) ++q;
                            }
                            break;
                        case Direction.South:
                            {
                                ++r;
                                if ((Direction)line[++i] == Direction.West) --q;
                            }
                            break;
                        case Direction.East: ++q; break;
                        case Direction.West: --q; break;
                    }
                }

                // Flip hexagon color
                Point2D hexagon = new Point2D(q, r);
                if (blackTiles.Contains(hexagon))
                {
                    blackTiles.Remove(hexagon);
                }
                else
                {
                    blackTiles.Add(hexagon);
                }
            }

            return blackTiles;
        }

        public object PartA(string input)
        {
            return GetBlackTiles(input.Lines()).Count;
        }

        // Six transformations to get all adjacent hexagons 
        private IEnumerable<Point2D> AdjacentHexagonTransformations = new Point2D[]
        {
            new Point2D(0, -1),
            new Point2D(1, -1),
            new Point2D(1, 0),
            new Point2D(0, 1),
            new Point2D(-1, 1),
            new Point2D(-1, 0)
        };

        private IEnumerable<Point2D> AdjacentHexagons(Point2D center) => AdjacentHexagonTransformations.Select(transform => transform + center);

        public object PartB(string input)
        {
            const int days = 100;
            HashSet<Point2D> blackTiles = GetBlackTiles(input.Lines());

            // This program is very similar to Day 17, Part B
            for (int day = 0; day < days; ++day)
            {
                Dictionary<Point2D, int> count = new Dictionary<Point2D, int>();
                HashSet<Point2D> nextState = new HashSet<Point2D>();

                // For each tile adjacent to a black tile, count the number of adjacent black tiles
                foreach (Point2D hexagon in blackTiles)
                {
                    foreach (Point2D adjacent in AdjacentHexagons(hexagon))
                    {
                        if (count.ContainsKey(adjacent))
                        {
                            ++count[adjacent];
                        }
                        else
                        {
                            count[adjacent] = 1;
                        }
                    }
                }

                // Apply flip rules
                foreach ((Point2D hexagon, int adjacentBlackCount) in count)
                {
                    if (blackTiles.Contains(hexagon))
                    {
                        if (adjacentBlackCount == 1 || adjacentBlackCount == 2)
                        {
                            nextState.Add(hexagon);
                        }
                    }
                    else if (adjacentBlackCount == 2)
                    {
                        nextState.Add(hexagon);
                    }
                }

                // Update state
                blackTiles = nextState;
            }

            return blackTiles.Count;
        }
    }
}
