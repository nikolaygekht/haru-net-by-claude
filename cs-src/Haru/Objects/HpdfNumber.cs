using Haru.Streams;

namespace Haru.Objects
{
    /// <summary>
    /// PDF integer number object
    /// </summary>
    public class HpdfNumber : HpdfObject
    {
        /// <summary>
        /// Gets or sets the integer value
        /// </summary>
        public int Value { get; set; }

        /// <inheritdoc/>
        public override HpdfObjectClass ObjectClass => HpdfObjectClass.Number;

        /// <summary>
        /// Creates a new number object
        /// </summary>
        public HpdfNumber(int value)
        {
            Value = value;
        }

        /// <inheritdoc/>
        public override void WriteValue(HpdfStream stream)
        {
            stream.WriteInt(Value);
        }

        /// <inheritdoc/>
        public override string ToString() => Value.ToString();
    }
}
