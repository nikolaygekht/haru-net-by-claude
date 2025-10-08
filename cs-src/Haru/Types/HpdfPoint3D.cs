using System;

namespace Haru.Types
{
    /// <summary>
    /// Represents a point in 3D space.
    /// </summary>
    public struct HpdfPoint3D : IEquatable<HpdfPoint3D>
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
        /// Z coordinate.
        /// </summary>
        public float Z { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HpdfPoint3D"/> struct.
        /// </summary>
        public HpdfPoint3D(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public bool Equals(HpdfPoint3D other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
        }

        public override bool Equals(object obj)
        {
            return obj is HpdfPoint3D other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(HpdfPoint3D left, HpdfPoint3D right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(HpdfPoint3D left, HpdfPoint3D right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }
    }
}
