using AdventOfCode2020.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2020.Days
{
    [Solution(17)]
    class Day17 : ISolution
    {
        private enum Cube
        {
            Inactive = '.',
            Active = '#'
        }

        private static Dictionary<Point3D, Cube> ParseInput3D(string input)
        {
            Dictionary<Point3D, Cube> map = new Dictionary<Point3D, Cube>();
            string[] lines = input.Lines();
            for (int y = 0; y < lines.Length; ++y)
            {
                for (int x = 0; x < lines[y].Length; ++x)
                {
                    map[new Point3D(x, y, 0)] = (Cube)lines[y][x];
                }
            }
            return map;
        }

        private Dictionary<Point3D, Cube> Transform3D(Dictionary<Point3D, Cube> map)
        {
            Dictionary<Point3D, Cube> nextState = new Dictionary<Point3D, Cube>();
            IEnumerable<Point3D> points = map.Keys.Select(point => point.Adjacent()).Aggregate(map.Keys.AsEnumerable(), (set, points) => set.Union(points));
            foreach (Point3D point in points)
            {
                Cube cube = map.GetValueOrDefault(point, Cube.Inactive);
                int activeCount = point.Adjacent().Count(point => map.GetValueOrDefault(point, Cube.Inactive) == Cube.Active);

                // Only record active cubes
                if (activeCount == 3 || (activeCount == 2 && cube == Cube.Active))
                {
                    nextState.Add(point, Cube.Active);
                }
            }
            return nextState;
        }

        public object PartA(string input)
        {
            const int cycles = 6;
            Dictionary<Point3D, Cube> map = ParseInput3D(input);
            for (int cycle = 0; cycle < cycles; ++cycle)
            {
                map = Transform3D(map);
            }
            return map.Count(pair => pair.Value == Cube.Active);
        }
        
        // If we use the above algorithm with a fourth dimension, the runtime of Part B is around 2 seconds
        // Too slow!
        // So, here is an optimized form of the problem to decrease the runtime
        
        // Only store points that have an active cube
        // Count number of active cubes for each neighbor
        // Add points to the next state if the activation rule applies

        // We could apply this same logic to Part A, but I wanted to keep the original solution for reference

        private static HashSet<Point4D> ParseInput4D(string input)
        {
            HashSet<Point4D> map = new HashSet<Point4D>();
            string[] lines = input.Lines();
            for (int y = 0; y < lines.Length; ++y)
            {
                for (int x = 0; x < lines[y].Length; ++x)
                {
                    if (lines[y][x] == (char)Cube.Active)
                    {
                        map.Add(new Point4D(x, y, 0, 0));
                    }
                }
            }
            return map;
        }

        private HashSet<Point4D> Transform4D(HashSet<Point4D> active)
        {
            Dictionary<Point4D, int> count = new Dictionary<Point4D, int>();
            HashSet<Point4D> nextState = new HashSet<Point4D>();
            foreach (Point4D point in active)
            {
                foreach (Point4D adj in point.Adjacent())
                {
                    if (count.ContainsKey(adj))
                    {
                        ++count[adj];
                    }
                    else
                    {
                        count[adj] = 1;
                    }
                }
            }

            foreach ((Point4D point, int activeNeighbors) in count)
            {
                bool isActive = active.Contains(point);
                if (activeNeighbors == 3 || (isActive && activeNeighbors == 2))
                {
                    nextState.Add(point);
                }
            }
            return nextState;
        }

        public object PartB(string input)
        {
            const int cycles = 6;
            HashSet<Point4D> active = ParseInput4D(input);
            for (int cycle = 0; cycle < cycles; ++cycle)
            {
                active = Transform4D(active);
            }
            return active.Count;
        }
    }
}
