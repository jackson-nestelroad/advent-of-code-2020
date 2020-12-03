using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2020.Common
{
    /// <summary>
    /// Represents a single point on a 2D plane.
    /// </summary>
    public readonly struct Point2D : IEquatable<Point2D>
    {
        public int X { get; init; }
        public int Y { get; init; }

        public Point2D(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Point2D operator+(Point2D a, Point2D b) => new Point2D(a.X + b.X, a.Y + b.Y);
        public static Point2D operator-(Point2D a, Point2D b) => new Point2D(a.X - b.X, a.Y - b.Y);

        public override bool Equals(object? obj) => obj is Point2D other && Equals(other);
        public bool Equals(Point2D other) => X == other.X && Y == other.Y;
        public static bool operator ==(Point2D left, Point2D right) => left.Equals(right);
        public static bool operator !=(Point2D left, Point2D right) => !(left == right);
        public override int GetHashCode() => HashCode.Combine(X, Y);
    }
}
