using System;
using Haru.Streams;

namespace Haru.Objects
{
    /// <summary>
    /// PDF real (floating-point) number object
    /// </summary>
    public class HpdfReal : HpdfObject
    {
        private float _value;

        /// <summary>
        /// Gets or sets the real value
        /// </summary>
        public float Value
        {
            get => _value;
            set
            {
                if (float.IsNaN(value) || float.IsInfinity(value))
                    throw new HpdfException(HpdfErrorCode.RealOutOfRange, "Value must be a finite number");
                _value = value;
            }
        }

        /// <inheritdoc/>
        public override HpdfObjectClass ObjectClass => HpdfObjectClass.Real;

        /// <summary>
        /// Creates a new real number object
        /// </summary>
        public HpdfReal(float value)
        {
            Value = value;
        }

        /// <inheritdoc/>
        public override void WriteValue(HpdfStream stream)
        {
            stream.WriteReal(Value);
        }

        /// <inheritdoc/>
        public override string ToString() => Value.ToString("G");
    }
}
