using System;

namespace Haru.Types
{
    /// <summary>
    /// Represents an RGB color with values from 0.0 to 1.0.
    /// </summary>
    public struct HpdfRgbColor : IEquatable<HpdfRgbColor>
    {
        /// <summary>
        /// Red component (0.0 - 1.0).
        /// </summary>
        public float R { get; set; }

        /// <summary>
        /// Green component (0.0 - 1.0).
        /// </summary>
        public float G { get; set; }

        /// <summary>
        /// Blue component (0.0 - 1.0).
        /// </summary>
        public float B { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HpdfRgbColor"/> struct.
        /// </summary>
        public HpdfRgbColor(float r, float g, float b)
        {
            R = Math.Clamp(r, 0f, 1f);
            G = Math.Clamp(g, 0f, 1f);
            B = Math.Clamp(b, 0f, 1f);
        }

        /// <summary>
        /// Creates an RGB color from byte values (0-255).
        /// </summary>
        public static HpdfRgbColor FromBytes(byte r, byte g, byte b)
        {
            return new HpdfRgbColor(r / 255f, g / 255f, b / 255f);
        }

        public bool Equals(HpdfRgbColor other)
        {
            return R.Equals(other.R) && G.Equals(other.G) && B.Equals(other.B);
        }

        public override bool Equals(object obj)
        {
            return obj is HpdfRgbColor other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = R.GetHashCode();
                hashCode = (hashCode * 397) ^ G.GetHashCode();
                hashCode = (hashCode * 397) ^ B.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(HpdfRgbColor left, HpdfRgbColor right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(HpdfRgbColor left, HpdfRgbColor right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"RGB({R:F3}, {G:F3}, {B:F3})";
        }

        // Common colors
        public static HpdfRgbColor Black => new HpdfRgbColor(0, 0, 0);
        public static HpdfRgbColor White => new HpdfRgbColor(1, 1, 1);
        public static HpdfRgbColor Red => new HpdfRgbColor(1, 0, 0);
        public static HpdfRgbColor Green => new HpdfRgbColor(0, 1, 0);
        public static HpdfRgbColor Blue => new HpdfRgbColor(0, 0, 1);
    }
}
