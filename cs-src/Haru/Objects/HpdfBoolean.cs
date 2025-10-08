using Haru.Streams;

namespace Haru.Objects
{
    /// <summary>
    /// PDF boolean object
    /// </summary>
    public class HpdfBoolean : HpdfObject
    {
        /// <summary>
        /// Singleton instance for true
        /// </summary>
        public static readonly HpdfBoolean True = new HpdfBoolean(true);

        /// <summary>
        /// Singleton instance for false
        /// </summary>
        public static readonly HpdfBoolean False = new HpdfBoolean(false);

        /// <summary>
        /// Gets or sets the boolean value
        /// </summary>
        public bool Value { get; set; }

        /// <inheritdoc/>
        public override HpdfObjectClass ObjectClass => HpdfObjectClass.Boolean;

        /// <summary>
        /// Creates a new boolean object
        /// </summary>
        public HpdfBoolean(bool value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets a boolean object for the specified value
        /// </summary>
        public static HpdfBoolean Of(bool value) => value ? True : False;

        /// <inheritdoc/>
        public override void WriteValue(HpdfStream stream)
        {
            stream.WriteString(Value ? "true" : "false");
        }

        /// <inheritdoc/>
        public override string ToString() => Value ? "true" : "false";
    }
}
