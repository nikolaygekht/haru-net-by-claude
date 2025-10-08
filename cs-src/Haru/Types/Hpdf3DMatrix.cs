using System;

namespace Haru.Types
{
    /// <summary>
    /// Represents a 3D transformation matrix (4x3 matrix).
    /// Matrix format: [a b c d e f g h i tx ty tz]
    /// </summary>
    public struct Hpdf3DMatrix : IEquatable<Hpdf3DMatrix>
    {
        // Row 1
        public float A { get; set; }
        public float B { get; set; }
        public float C { get; set; }

        // Row 2
        public float D { get; set; }
        public float E { get; set; }
        public float F { get; set; }

        // Row 3
        public float G { get; set; }
        public float H { get; set; }
        public float I { get; set; }

        // Translation
        public float Tx { get; set; }
        public float Ty { get; set; }
        public float Tz { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Hpdf3DMatrix"/> struct.
        /// </summary>
        public Hpdf3DMatrix(float a, float b, float c, float d, float e, float f,
                            float g, float h, float i, float tx, float ty, float tz)
        {
            A = a; B = b; C = c;
            D = d; E = e; F = f;
            G = g; H = h; I = i;
            Tx = tx; Ty = ty; Tz = tz;
        }

        /// <summary>
        /// Gets the identity matrix.
        /// </summary>
        public static Hpdf3DMatrix Identity =>
            new Hpdf3DMatrix(1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0);

        public bool Equals(Hpdf3DMatrix other)
        {
            return A.Equals(other.A) && B.Equals(other.B) && C.Equals(other.C) &&
                   D.Equals(other.D) && E.Equals(other.E) && F.Equals(other.F) &&
                   G.Equals(other.G) && H.Equals(other.H) && I.Equals(other.I) &&
                   Tx.Equals(other.Tx) && Ty.Equals(other.Ty) && Tz.Equals(other.Tz);
        }

        public override bool Equals(object obj)
        {
            return obj is Hpdf3DMatrix other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = A.GetHashCode();
                hash = (hash * 397) ^ B.GetHashCode();
                hash = (hash * 397) ^ C.GetHashCode();
                hash = (hash * 397) ^ D.GetHashCode();
                hash = (hash * 397) ^ E.GetHashCode();
                hash = (hash * 397) ^ F.GetHashCode();
                hash = (hash * 397) ^ G.GetHashCode();
                hash = (hash * 397) ^ H.GetHashCode();
                hash = (hash * 397) ^ I.GetHashCode();
                hash = (hash * 397) ^ Tx.GetHashCode();
                hash = (hash * 397) ^ Ty.GetHashCode();
                hash = (hash * 397) ^ Tz.GetHashCode();
                return hash;
            }
        }

        public static bool operator ==(Hpdf3DMatrix left, Hpdf3DMatrix right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Hpdf3DMatrix left, Hpdf3DMatrix right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"[{A:F2} {B:F2} {C:F2}] [{D:F2} {E:F2} {F:F2}] [{G:F2} {H:F2} {I:F2}] T:[{Tx:F2} {Ty:F2} {Tz:F2}]";
        }
    }
}
