using System;

namespace Haru.Types
{
    /// <summary>
    /// Represents a 2D transformation matrix used for coordinate transformations.
    /// Matrix format: [a b c d x y]
    /// </summary>
    public struct HpdfTransMatrix : IEquatable<HpdfTransMatrix>
    {
        /// <summary>
        /// Horizontal scaling component.
        /// </summary>
        public float A { get; set; }

        /// <summary>
        /// Vertical skewing component.
        /// </summary>
        public float B { get; set; }

        /// <summary>
        /// Horizontal skewing component.
        /// </summary>
        public float C { get; set; }

        /// <summary>
        /// Vertical scaling component.
        /// </summary>
        public float D { get; set; }

        /// <summary>
        /// Horizontal translation component.
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Vertical translation component.
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HpdfTransMatrix"/> struct.
        /// </summary>
        public HpdfTransMatrix(float a, float b, float c, float d, float x, float y)
        {
            A = a;
            B = b;
            C = c;
            D = d;
            X = x;
            Y = y;
        }

        /// <summary>
        /// Gets the identity matrix.
        /// </summary>
        public static HpdfTransMatrix Identity => new HpdfTransMatrix(1, 0, 0, 1, 0, 0);

        /// <summary>
        /// Creates a translation matrix.
        /// </summary>
        public static HpdfTransMatrix CreateTranslation(float x, float y)
        {
            return new HpdfTransMatrix(1, 0, 0, 1, x, y);
        }

        /// <summary>
        /// Creates a scaling matrix.
        /// </summary>
        public static HpdfTransMatrix CreateScale(float scaleX, float scaleY)
        {
            return new HpdfTransMatrix(scaleX, 0, 0, scaleY, 0, 0);
        }

        /// <summary>
        /// Creates a rotation matrix.
        /// </summary>
        /// <param name="angleRadians">Rotation angle in radians.</param>
        public static HpdfTransMatrix CreateRotation(float angleRadians)
        {
            float cos = (float)Math.Cos(angleRadians);
            float sin = (float)Math.Sin(angleRadians);
            return new HpdfTransMatrix(cos, sin, -sin, cos, 0, 0);
        }

        public bool Equals(HpdfTransMatrix other)
        {
            return A.Equals(other.A) &&
                   B.Equals(other.B) &&
                   C.Equals(other.C) &&
                   D.Equals(other.D) &&
                   X.Equals(other.X) &&
                   Y.Equals(other.Y);
        }

        public override bool Equals(object obj)
        {
            return obj is HpdfTransMatrix other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = A.GetHashCode();
                hashCode = (hashCode * 397) ^ B.GetHashCode();
                hashCode = (hashCode * 397) ^ C.GetHashCode();
                hashCode = (hashCode * 397) ^ D.GetHashCode();
                hashCode = (hashCode * 397) ^ X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(HpdfTransMatrix left, HpdfTransMatrix right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(HpdfTransMatrix left, HpdfTransMatrix right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"[{A:F3} {B:F3} {C:F3} {D:F3} {X:F3} {Y:F3}]";
        }
    }
}
