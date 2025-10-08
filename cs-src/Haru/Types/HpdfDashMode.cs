using System;
using System.Linq;

namespace Haru.Types
{
    /// <summary>
    /// Represents the dash pattern for line drawing.
    /// </summary>
    public struct HpdfDashMode : IEquatable<HpdfDashMode>
    {
        /// <summary>
        /// Dash pattern array (max 8 elements). Each element defines the length of a dash or gap.
        /// </summary>
        public ushort[] Pattern { get; set; }

        /// <summary>
        /// Number of elements in the pattern.
        /// </summary>
        public uint NumPattern { get; set; }

        /// <summary>
        /// Phase offset for the dash pattern.
        /// </summary>
        public uint Phase { get; set; }

        /// <summary>
        /// Maximum number of pattern elements.
        /// </summary>
        public const int MaxPatternLength = 8;

        /// <summary>
        /// Initializes a new instance of the <see cref="HpdfDashMode"/> struct.
        /// </summary>
        public HpdfDashMode(ushort[] pattern, uint phase = 0)
        {
            if (pattern == null || pattern.Length == 0)
            {
                Pattern = new ushort[MaxPatternLength];
                NumPattern = 0;
            }
            else
            {
                if (pattern.Length > MaxPatternLength)
                    throw new ArgumentException($"Pattern length cannot exceed {MaxPatternLength}", nameof(pattern));

                Pattern = new ushort[MaxPatternLength];
                Array.Copy(pattern, Pattern, pattern.Length);
                NumPattern = (uint)pattern.Length;
            }

            Phase = phase;
        }

        /// <summary>
        /// Gets the active pattern (only the used elements).
        /// </summary>
        public ushort[] GetActivePattern()
        {
            if (Pattern == null || NumPattern == 0)
                return Array.Empty<ushort>();

            var result = new ushort[NumPattern];
            Array.Copy(Pattern, result, NumPattern);
            return result;
        }

        /// <summary>
        /// Creates a solid line (no dashes).
        /// </summary>
        public static HpdfDashMode Solid => new HpdfDashMode(Array.Empty<ushort>(), 0);

        public bool Equals(HpdfDashMode other)
        {
            if (NumPattern != other.NumPattern || Phase != other.Phase)
                return false;

            if (Pattern == null && other.Pattern == null)
                return true;

            if (Pattern == null || other.Pattern == null)
                return false;

            for (int i = 0; i < NumPattern; i++)
            {
                if (Pattern[i] != other.Pattern[i])
                    return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            return obj is HpdfDashMode other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = (int)NumPattern;
                hash = (hash * 397) ^ (int)Phase;

                if (Pattern != null)
                {
                    for (int i = 0; i < NumPattern && i < Pattern.Length; i++)
                    {
                        hash = (hash * 397) ^ Pattern[i].GetHashCode();
                    }
                }

                return hash;
            }
        }

        public static bool operator ==(HpdfDashMode left, HpdfDashMode right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(HpdfDashMode left, HpdfDashMode right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            if (NumPattern == 0)
                return "Solid";

            var activePattern = GetActivePattern();
            return $"[{string.Join(" ", activePattern)}] Phase:{Phase}";
        }
    }
}
