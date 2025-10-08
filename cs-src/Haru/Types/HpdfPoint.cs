using System;

namespace Haru.Types
{
    /// <summary>
    /// Represents a point in 2D space.
    /// </summary>
    public struct HpdfPoint : IEquatable<HpdfPoint>
    {
        /// <summary>
        /// X coordinate.
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Y coordinate.
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HpdfPoint"/> struct.
        /// </summary>
        public HpdfPoint(float x, float y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(HpdfPoint other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }

        public override bool Equals(object obj)
        {
            return obj is HpdfPoint other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }

        public static bool operator ==(HpdfPoint left, HpdfPoint right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(HpdfPoint left, HpdfPoint right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}
