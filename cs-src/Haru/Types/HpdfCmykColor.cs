using System;

namespace Haru.Types
{
    /// <summary>
    /// Represents a CMYK color with values from 0.0 to 1.0.
    /// </summary>
    public struct HpdfCmykColor : IEquatable<HpdfCmykColor>
    {
        /// <summary>
        /// Cyan component (0.0 - 1.0).
        /// </summary>
        public float C { get; set; }

        /// <summary>
        /// Magenta component (0.0 - 1.0).
        /// </summary>
        public float M { get; set; }

        /// <summary>
        /// Yellow component (0.0 - 1.0).
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// Black (Key) component (0.0 - 1.0).
        /// </summary>
        public float K { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HpdfCmykColor"/> struct.
        /// </summary>
        public HpdfCmykColor(float c, float m, float y, float k)
        {
            C = Math.Clamp(c, 0f, 1f);
            M = Math.Clamp(m, 0f, 1f);
            Y = Math.Clamp(y, 0f, 1f);
            K = Math.Clamp(k, 0f, 1f);
        }

        public bool Equals(HpdfCmykColor other)
        {
            return C.Equals(other.C) &&
                   M.Equals(other.M) &&
                   Y.Equals(other.Y) &&
                   K.Equals(other.K);
        }

        public override bool Equals(object obj)
        {
            return obj is HpdfCmykColor other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = C.GetHashCode();
                hashCode = (hashCode * 397) ^ M.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ K.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(HpdfCmykColor left, HpdfCmykColor right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(HpdfCmykColor left, HpdfCmykColor right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"CMYK({C:F3}, {M:F3}, {Y:F3}, {K:F3})";
        }

        // Common colors
        public static HpdfCmykColor Black => new HpdfCmykColor(0, 0, 0, 1);
        public static HpdfCmykColor White => new HpdfCmykColor(0, 0, 0, 0);
    }
}
