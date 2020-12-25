using AdventOfCode2020.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2020.Days
{
    [Solution(20)]
    class Day20 : ISolution
    {
        private class Tile
        {
            public enum Data
            {
                Empty = '.',
                Hash = '#'
            }

            public enum Border
            {
                North,
                South,
                East,
                West
            }

            public enum Transformation
            {
                None = 0,
                FlipY = 1 << 0,
                FlipX = 1 << 1,
                RotateCW = 1 << 2,
                Max = 1 << 3
            }

            public const int Length = 10;
            public const int ImageLength = Length - 2;

            private readonly uint[] _borders = new uint[4];
            private readonly Tile[] _connections = new Tile[4];
            private int _transformationIndex = 1;

            public int Id { get; }
            public List<string> Image { get; private set; }
            public int Orientation { get; private set; }
            public bool Fixed { get; private set; }

            public IReadOnlyCollection<uint> Borders => Array.AsReadOnly(_borders);
            public IReadOnlyCollection<Tile> Connections => Array.AsReadOnly(_connections);

            public bool IsConnected => _connections.Any(c => c != null);
            public bool IsCorner => _connections.Where(c => c != null).Count() == 2;
            public bool IsEdge => _connections.Where(c => c != null).Count() == 3;

            public bool IsConnectedOnBorder(Border border)
            {
                return _connections[(int)border] != null;
            }

            private Tile(int id)
            {
                Id = id;
            }

            public uint GetBorder(Border border) => _borders[(int)border];
            public Tile GetConnection(Border border) => _connections[(int)border];

            public void Connect(Tile other, Border border)
            {
                _connections[(int)border] = other;
                other._connections[(int)border ^ 1] = this;
            }

            private static uint ReverseBits(uint num, int length = Length)
            {
                uint reversed = 0;
                for (int i = 0; i < length; ++i)
                {
                    if ((num & (1 << i)) != 0)
                    {
                        reversed |= 1u << (length - 1 - i);
                    }
                }
                return reversed;
            }

            private void ApplyNextBorderTransformation()
            {
                if (Fixed)
                {
                    throw new SolutionFailedException("Fixed tiles cannot be transformed");
                }

                Transformation toApply;
                if (_transformationIndex >= (int)Transformation.Max || _transformationIndex == 0)
                {
                    // Revert back to 0
                    toApply = (Transformation)Orientation;
                    Orientation = 0;
                    _transformationIndex = 1;
                }
                else
                {
                    int grayCode = _transformationIndex ^ (_transformationIndex >> 1);
                    int toggled = grayCode ^ Orientation;
                    Orientation = grayCode;
                    toApply = (Transformation)toggled;
                    ++_transformationIndex;
                }

                switch (toApply)
                {
                    case Transformation.FlipY:
                        {
                            uint temp = _borders[(int)Border.North];
                            _borders[(int)Border.North] = _borders[(int)Border.South];
                            _borders[(int)Border.South] = temp;
                            _borders[(int)Border.East] = ReverseBits(_borders[(int)Border.East]);
                            _borders[(int)Border.West] = ReverseBits(_borders[(int)Border.West]);
                        }
                        break;
                    case Transformation.FlipX:
                        {
                            uint temp = _borders[(int)Border.East];
                            _borders[(int)Border.East] = _borders[(int)Border.West];
                            _borders[(int)Border.West] = temp;
                            _borders[(int)Border.North] = ReverseBits(_borders[(int)Border.North]);
                            _borders[(int)Border.South] = ReverseBits(_borders[(int)Border.South]);
                        }
                        break;
                    case Transformation.RotateCW:
                        {
                            uint temp = _borders[(int)Border.North];
                            _borders[(int)Border.North] = ReverseBits(_borders[(int)Border.West]);
                            _borders[(int)Border.West] = _borders[(int)Border.South];
                            _borders[(int)Border.South] = ReverseBits(_borders[(int)Border.East]);
                            _borders[(int)Border.East] = temp;

                            //// Actually implemented as a counter-clockwise rotation when working with gray code
                            //uint temp = _borders[(int)Border.North];
                            //_borders[(int)Border.North] = _borders[(int)Border.West];
                            //_borders[(int)Border.West] = ReverseBits(_borders[(int)Border.South]);
                            //_borders[(int)Border.South] = _borders[(int)Border.East];
                            //_borders[(int)Border.East] = ReverseBits(temp);
                        }
                        break;
                    default: throw new SolutionFailedException($"Unknown transformation code: {toApply}");
                }
            }

            public void FixOrientation()
            {
                Fixed = true;
            }

            /// <summary>
            /// This method continually cycles through every possible orientation of the tile, starting with
            /// however the tile is oriented from the start. Each iteration applies one transformation to the tile.
            /// </summary>
            /// <returns></returns>
            public IEnumerable<int> CycleBorderOrientations()
            {
                yield return Orientation;

                if (!Fixed)
                {
                    for (int i = 1; i < (int)Transformation.Max; ++i)
                    {
                        ApplyNextBorderTransformation();
                        yield return Orientation;
                    }
                }
            }

            public IEnumerable<(Border Key, uint Value)> UnconnectedBorders()
            {
                return Enum.GetValues(typeof(Border)).Cast<Border>().Where(border => GetConnection(border) == null).Select(border => (border, GetBorder(border)));
            }

            private void OrientImage()
            {
                // Transform image data to match the recorded orientation

                if ((Orientation & (int)Transformation.RotateCW) != 0)
                {
                    List<string> newImage = new List<string>(ImageLength);
                    for (int y = 0; y < ImageLength; ++y)
                    {
                        char[] chars = new char[ImageLength];
                        for (int x = 0; x < ImageLength; ++x)
                        {
                            // Actually rotate counter-clockwise!
                            // Because of the way orientations are discovered and recorded (using gray codes),
                            // clockwise border rotations are actually counter-clockwise image rotations
                            // This was a huge pain to debug...
                            chars[x] = Image[x][ImageLength - 1 - y];
                        }
                        newImage.Add(new string(chars));
                    }
                    Image = newImage;
                }

                if ((Orientation & (int)Transformation.FlipY) != 0)
                {
                    Image.Reverse();
                }

                if ((Orientation & (int)Transformation.FlipX) != 0)
                {
                    for (int i = 0; i < ImageLength; ++i)
                    {
                        char[] chars = Image[i].ToCharArray();
                        Array.Reverse(chars);
                        Image[i] = new string(chars);
                    }
                }
            }

            public void CommitOrientation()
            {
                OrientImage();

                // This is the new base state
                Orientation = 0;
                _transformationIndex = 1;
            }

            public static Tile FromInput(string input)
            {
                string[] lines = input.Lines();
                Tile tile = new Tile(int.Parse(lines[0][5..9]));
                
                // Parse north border
                for (int i = 0; i < Length; ++i)
                {
                    tile._borders[(int)Border.North] |= (lines[1][i] == (char)Data.Hash ? 1u : 0u) << (Length - 1 - i);
                }

                // Parse south border
                for (int i = 0; i < Length; ++i)
                {
                    tile._borders[(int)Border.South] |= (lines[^1][i] == (char)Data.Hash ? 1u : 0u) << (Length - 1 - i);
                }

                // Parse east border
                for (int i = 0; i < Length; ++i)
                {
                    tile._borders[(int)Border.East] |= (lines[i + 1][^1] == (char)Data.Hash ? 1u : 0u) << (Length - 1 - i);
                }

                // Parse west border
                for (int i = 0; i < Length; ++i)
                {
                    tile._borders[(int)Border.West] |= (lines[i + 1][0] == (char)Data.Hash ? 1u : 0u) << (Length - 1 - i);
                }

                // Remove borders from image data
                tile.Image = lines.Skip(2).Take(ImageLength).Select(str => str[1..(ImageLength + 1)]).ToList();

                return tile;
            }
        }

        private static List<Tile> ParseInput(string input) => input.Lines(2).Select(group => Tile.FromInput(group)).ToList();

        private void ConnectTiles(List<Tile> tiles)
        {
            Queue<Tile> tileQueue = new Queue<Tile>();

            Tile anchor = tiles.First();
            anchor.FixOrientation();
            tileQueue.Enqueue(anchor);

            while (tileQueue.Count > 0)
            {
                Tile current = tileQueue.Dequeue();

                bool CheckForMatch(Tile other)
                {
                    foreach (int orientation in other.CycleBorderOrientations())
                    {
                        foreach ((Tile.Border position, uint border) in current.UnconnectedBorders())
                        {
                            // Found a match
                            if (border == other.GetBorder((Tile.Border)((int)position ^ 1)))
                            {
                                current.Connect(other, position);

                                // Once a tile is connected, its orientation should remain fixed
                                other.FixOrientation();

                                tileQueue.Enqueue(other);
                                return true;
                            }
                        }
                    }
                    return false;
                }
                int connectionsFound = 0;

                // Check all tiles for a match or until 4 tiles are connected
                foreach (Tile other in tiles)
                {
                    if (current.Id != other.Id && CheckForMatch(other))
                    {
                        if (++connectionsFound == 4)
                        {
                            break;
                        }
                    }
                }
            }
        }

        public object PartA(string input)
        {
            List<Tile> tiles = ParseInput(input);
            ConnectTiles(tiles);
            return tiles.Where(tile => tile.IsCorner).Aggregate(1L, (product, tile) => product * tile.Id);
        }

        private List<string> Compile(List<Tile> tiles)
        {
            Tile northWestCorner = tiles.Find(tile =>
                tile.IsCorner
                && tile.IsConnectedOnBorder(Tile.Border.South)
                && tile.IsConnectedOnBorder(Tile.Border.East)
            );
            List<string> compiled = new List<string>();

            // Iterate east from northwest corner
            Tile edge = northWestCorner;
            for (int tileIndex = 0; edge != null; edge = edge.GetConnection(Tile.Border.South), tileIndex += Tile.ImageLength)
            {
                compiled.AddRange(edge.Image);

                // Iterate east from west edge
                Tile next = edge.GetConnection(Tile.Border.East);
                while (next != null)
                {
                    for (int i = 0; i < Tile.ImageLength; ++i)
                    {
                        compiled[tileIndex + i] += next.Image[i];
                    }
                    next = next.GetConnection(Tile.Border.East);
                }
            }

            return compiled;
        }

        // Quicker implementation of reversing a compiled image
        private void TransformImage(ref List<string> image, int index)
        {
            int orientation = index ^ (index >> 1);
            int toggled = orientation ^ (index - 1 ^ ((index - 1) >> 1));
            switch ((Tile.Transformation)toggled)
            {
                case Tile.Transformation.FlipY:
                    {
                        image.Reverse();
                    }
                    break;
                case Tile.Transformation.FlipX:
                    {
                        for (int i = 0; i < image.Count; ++i)
                        {
                            char[] chars = image[i].ToCharArray();
                            Array.Reverse(chars);
                            image[i] = new string(chars);
                        }
                    }
                    break;
                case Tile.Transformation.RotateCW:
                    {
                        int imageLength = image.Count;
                        List<string> newImage = new List<string>(imageLength);
                        for (int y = 0; y < image.Count; ++y)
                        {
                            char[] chars = new char[imageLength];
                            for (int x = 0; x < imageLength; ++x)
                            {
                                chars[x] = image[imageLength - 1 - x][y];
                            }
                            newImage.Add(new string(chars));
                        }
                        image = newImage;
                    }
                    break;
            }
        }

        // Sea monster pattern as coordinates
        private int[][] SeaMonsterPattern = new int[3][]
        {
            new int[] { 18 },
            new int[] { 0, 5, 6, 11, 12, 17, 18, 19 },
            new int[] { 1, 4, 7, 10, 13, 16 }
        };

        public object PartB(string input)
        {
            List<Tile> tiles = ParseInput(input);
            ConnectTiles(tiles);
            foreach (Tile tile in tiles)
            {
                tile.CommitOrientation();
            }

            // Compile the tiles into a single list of strings
            List<string> compiled = Compile(tiles);

            int seaMonsterTileCount = 0;
            int patternHeight = SeaMonsterPattern.Length;
            int patternLength = SeaMonsterPattern.Select(arr => arr.Max()).Max();
            int seaMonsterSize = SeaMonsterPattern.Aggregate(0, (sum, arr) => sum + arr.Length);

            // We have to find the correct orientation of the entire scan for sea monsters to be found

            int scanOrientation = 0;
            while (scanOrientation < (int)Tile.Transformation.Max)
            {
                int maxY = compiled.Count - patternHeight + 1;
                int maxX = compiled.First().Length - patternLength + 1;
                for (int y = 0; y < maxY; ++y)
                {
                    for (int x = 0; x < maxX; ++x)
                    {
                        bool found = true;
                        for (int i = 0; found && i < patternHeight; ++i)
                        {
                            for (int j = 0; found && j < SeaMonsterPattern[i].Length; ++j)
                            {
                                found = compiled[y + i][x + SeaMonsterPattern[i][j]] == (char)Tile.Data.Hash;
                            }
                        }
                        if (found)
                        {
                            seaMonsterTileCount += seaMonsterSize;
                        }
                    }
                }
                
                // Found some sea monsters
                if (seaMonsterTileCount != 0)
                {
                    return compiled.Aggregate(0, (sum, row) => sum + row.Where(c => c == (char)Tile.Data.Hash).Count()) - seaMonsterTileCount;
                }

                TransformImage(ref compiled, ++scanOrientation);
            }

            throw new SolutionFailedException("No sea monsters found in any all possible transformations of the compiled scan");
        }
    }
}
