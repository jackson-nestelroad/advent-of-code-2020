using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2020.Common
{
    public class Point2D : IEquatable<Point2D>
    {
        public int X { get; init; }
        public int Y { get; init; }

        public Point2D(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Point2D Translate(int x = 0, int y = 0) => new Point2D(X + x, Y + y);

        public void Deconstruct(out int x, out int y)
        {
            x = X;
            y = Y;
        }

        public static Point2D operator +(Point2D a, Point2D b) => new Point2D(a.X + b.X, a.Y + b.Y);
        public static Point2D operator -(Point2D a, Point2D b) => new Point2D(a.X - b.X, a.Y - b.Y);
        public static Point2D operator *(Point2D p, int m) => new Point2D(m * p.X, m * p.Y);

        public override bool Equals(object? obj) => obj is Point2D other && Equals(other);
        public virtual bool Equals(Point2D other) => X == other.X && Y == other.Y;
        public static bool operator ==(Point2D left, Point2D right) => left.Equals(right);
        public static bool operator !=(Point2D left, Point2D right) => !(left == right);
        public override int GetHashCode() => HashCode.Combine(X, Y);

        public virtual IEnumerable<Point2D> Adjacent()
        {
            for (int x = -1; x < 2; ++x)
            {
                for (int y = -1; y < 2; ++y)
                {
                    if (x != 0 || y != 0)
                    {
                        yield return Translate(x, y);
                    }
                }
            }
        }
    }

    public class Point3D : Point2D, IEquatable<Point3D>
    {
        public int Z { get; init; }

        public Point3D(int x, int y, int z)
            : base(x, y)
        {
            Z = z;
        }

        public Point3D Translate(int x = 0, int y = 0, int z = 0) => new Point3D(X + x, Y + y, Z + z);

        public void Deconstruct(out int x, out int y, out int z)
        {
            x = X;
            y = Y;
            z = Z;
        }

        public static Point3D operator +(Point3D a, Point3D b) => new Point3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Point3D operator -(Point3D a, Point3D b) => new Point3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Point3D operator *(Point3D p, int m) => new Point3D(m * p.X, m * p.Y, m * p.Z);

        public override bool Equals(object? obj) => obj is Point3D other && Equals(other);
        public bool Equals(Point3D other) => X == other.X && Y == other.Y && Z == other.Z;
        public static bool operator ==(Point3D left, Point3D right) => left.Equals(right);
        public static bool operator !=(Point3D left, Point3D right) => !(left == right);
        public override int GetHashCode() => HashCode.Combine(X, Y, Z);

        public override IEnumerable<Point3D> Adjacent()
        {
            for (int x = -1; x < 2; ++x)
            {
                for (int y = -1; y < 2; ++y)
                {
                    for (int z = -1; z < 2; ++z)
                    {
                        if (x != 0 || y != 0 || z != 0)
                        {
                            yield return Translate(x, y, z);
                        }
                    }
                }
            }
        }
    }

    public class Point4D : Point3D, IEquatable<Point4D>
    {
        public int W { get; init; }

        public Point4D(int x, int y, int z, int w)
            : base(x, y, z)
        {
            W = w;
        }

        public Point4D Translate(int x = 0, int y = 0, int z = 0, int w = 0) => new Point4D(X + x, Y + y, Z + z, W + w);

        public void Deconstruct(out int x, out int y, out int z, out int w)
        {
            x = X;
            y = Y;
            z = Z;
            w = Z;
        }

        public static Point4D operator +(Point4D a, Point4D b) => new Point4D(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
        public static Point4D operator -(Point4D a, Point4D b) => new Point4D(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
        public static Point4D operator *(Point4D p, int m) => new Point4D(m * p.X, m * p.Y, m * p.Z, m * p.W);

        public override bool Equals(object? obj) => obj is Point4D other && Equals(other);
        public bool Equals(Point4D other) => X == other.X && Y == other.Y && Z == other.Z && W == other.W;
        public static bool operator ==(Point4D left, Point4D right) => left.Equals(right);
        public static bool operator !=(Point4D left, Point4D right) => !(left == right);
        public override int GetHashCode() => HashCode.Combine(X, Y, Z, W);

        public override IEnumerable<Point4D> Adjacent()
        {
            for (int x = -1; x < 2; ++x)
            {
                for (int y = -1; y < 2; ++y)
                {
                    for (int z = -1; z < 2; ++z)
                    {
                        for (int w = -1; w < 2; ++w)
                        {
                            if (x != 0 || y != 0 || z != 0 || w != 0)
                            {
                                yield return Translate(x, y, z, w);
                            }
                        }
                    }
                }
            }
        }
    }
}
