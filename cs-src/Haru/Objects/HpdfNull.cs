using Haru.Streams;

namespace Haru.Objects
{
    /// <summary>
    /// PDF null object
    /// </summary>
    public class HpdfNull : HpdfObject
    {
        /// <summary>
        /// Singleton instance of null object
        /// </summary>
        public static readonly HpdfNull Instance = new HpdfNull();

        /// <inheritdoc/>
        public override HpdfObjectClass ObjectClass => HpdfObjectClass.Null;

        /// <summary>
        /// Creates a new null object
        /// </summary>
        public HpdfNull()
        {
        }

        /// <inheritdoc/>
        public override void WriteValue(HpdfStream stream)
        {
            stream.WriteString("null");
        }

        /// <inheritdoc/>
        public override string ToString() => "null";
    }
}
