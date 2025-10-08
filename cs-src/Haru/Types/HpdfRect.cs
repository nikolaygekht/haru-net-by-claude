using System;

namespace Haru.Types
{
    /// <summary>
    /// Represents a rectangle defined by left, bottom, right, and top coordinates.
    /// </summary>
    public struct HpdfRect : IEquatable<HpdfRect>
    {
        /// <summary>
        /// Left coordinate.
        /// </summary>
        public float Left { get; set; }

        /// <summary>
        /// Bottom coordinate.
        /// </summary>
        public float Bottom { get; set; }

        /// <summary>
        /// Right coordinate.
        /// </summary>
        public float Right { get; set; }

        /// <summary>
        /// Top coordinate.
        /// </summary>
        public float Top { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HpdfRect"/> struct.
        /// </summary>
        public HpdfRect(float left, float bottom, float right, float top)
        {
            Left = left;
            Bottom = bottom;
            Right = right;
            Top = top;
        }

        /// <summary>
        /// Gets the width of the rectangle.
        /// </summary>
        public float Width => Right - Left;

        /// <summary>
        /// Gets the height of the rectangle.
        /// </summary>
        public float Height => Top - Bottom;

        public bool Equals(HpdfRect other)
        {
            return Left.Equals(other.Left) &&
                   Bottom.Equals(other.Bottom) &&
                   Right.Equals(other.Right) &&
                   Top.Equals(other.Top);
        }

        public override bool Equals(object obj)
        {
            return obj is HpdfRect other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Left.GetHashCode();
                hashCode = (hashCode * 397) ^ Bottom.GetHashCode();
                hashCode = (hashCode * 397) ^ Right.GetHashCode();
                hashCode = (hashCode * 397) ^ Top.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(HpdfRect left, HpdfRect right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(HpdfRect left, HpdfRect right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"[L:{Left}, B:{Bottom}, R:{Right}, T:{Top}]";
        }
    }

    /// <summary>
    /// Alias for <see cref="HpdfRect"/>. Used interchangeably in PDF context.
    /// </summary>
    public struct HpdfBox : IEquatable<HpdfBox>
    {
        private HpdfRect _rect;

        public float Left { get => _rect.Left; set => _rect.Left = value; }
        public float Bottom { get => _rect.Bottom; set => _rect.Bottom = value; }
        public float Right { get => _rect.Right; set => _rect.Right = value; }
        public float Top { get => _rect.Top; set => _rect.Top = value; }
        public float Width => _rect.Width;
        public float Height => _rect.Height;

        public HpdfBox(float left, float bottom, float right, float top)
        {
            _rect = new HpdfRect(left, bottom, right, top);
        }

        public bool Equals(HpdfBox other) => _rect.Equals(other._rect);
        public override bool Equals(object obj) => obj is HpdfBox other && Equals(other);
        public override int GetHashCode() => _rect.GetHashCode();
        public static bool operator ==(HpdfBox left, HpdfBox right) => left.Equals(right);
        public static bool operator !=(HpdfBox left, HpdfBox right) => !left.Equals(right);
        public override string ToString() => _rect.ToString();
    }
}
