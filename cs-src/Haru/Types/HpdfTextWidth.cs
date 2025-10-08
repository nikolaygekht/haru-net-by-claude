using System;

namespace Haru.Types
{
    /// <summary>
    /// Contains text measurement information.
    /// </summary>
    public struct HpdfTextWidth : IEquatable<HpdfTextWidth>
    {
        /// <summary>
        /// Number of characters in the text.
        /// </summary>
        public uint NumChars { get; set; }

        /// <summary>
        /// Number of words in the text.
        /// Note: This value may change in future versions; use NumSpace as an alternative.
        /// </summary>
        public uint NumWords { get; set; }

        /// <summary>
        /// Width of the text in the current font and size.
        /// </summary>
        public uint Width { get; set; }

        /// <summary>
        /// Number of spaces in the text.
        /// </summary>
        public uint NumSpace { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HpdfTextWidth"/> struct.
        /// </summary>
        public HpdfTextWidth(uint numChars, uint numWords, uint width, uint numSpace)
        {
            NumChars = numChars;
            NumWords = numWords;
            Width = width;
            NumSpace = numSpace;
        }

        public bool Equals(HpdfTextWidth other)
        {
            return NumChars == other.NumChars &&
                   NumWords == other.NumWords &&
                   Width == other.Width &&
                   NumSpace == other.NumSpace;
        }

        public override bool Equals(object obj)
        {
            return obj is HpdfTextWidth other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = (int)NumChars;
                hash = (hash * 397) ^ (int)NumWords;
                hash = (hash * 397) ^ (int)Width;
                hash = (hash * 397) ^ (int)NumSpace;
                return hash;
            }
        }

        public static bool operator ==(HpdfTextWidth left, HpdfTextWidth right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(HpdfTextWidth left, HpdfTextWidth right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"Chars:{NumChars}, Words:{NumWords}, Width:{Width}, Spaces:{NumSpace}";
        }
    }
}
