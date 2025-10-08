using Haru.Objects;
using Haru.Streams;

namespace Haru.Xref
{
    /// <summary>
    /// Represents a single entry in a PDF cross-reference table.
    /// Each entry tracks an object's location in the PDF file.
    /// </summary>
    public class HpdfXrefEntry
    {
        /// <summary>
        /// Maximum generation number as defined by PDF spec (65535)
        /// </summary>
        public const ushort MaxGenerationNumber = 65535;

        /// <summary>
        /// Gets or sets the entry type (Free or InUse)
        /// </summary>
        public HpdfXrefEntryType EntryType { get; set; }

        /// <summary>
        /// Gets or sets the byte offset in the PDF file where the object starts.
        /// For free entries, this is the object number of the next free entry.
        /// </summary>
        public uint ByteOffset { get; set; }

        /// <summary>
        /// Gets or sets the generation number (version) of the object.
        /// Starts at 0 for new objects, incremented when object is freed and reused.
        /// </summary>
        public ushort GenerationNumber { get; set; }

        /// <summary>
        /// Gets or sets the PDF object associated with this entry.
        /// Null for free entries or before the object is written.
        /// </summary>
        public HpdfObject Object { get; set; }

        /// <summary>
        /// Initializes a new in-use xref entry with default values
        /// </summary>
        public HpdfXrefEntry()
        {
            EntryType = HpdfXrefEntryType.InUse;
            ByteOffset = 0;
            GenerationNumber = 0;
            Object = null;
        }

        /// <summary>
        /// Creates a free entry (used for entry 0 in xref table)
        /// </summary>
        public static HpdfXrefEntry CreateFreeEntry()
        {
            return new HpdfXrefEntry
            {
                EntryType = HpdfXrefEntryType.Free,
                ByteOffset = 0,
                GenerationNumber = MaxGenerationNumber
            };
        }

        /// <summary>
        /// Creates an in-use entry for a PDF object
        /// </summary>
        public static HpdfXrefEntry CreateInUseEntry(HpdfObject obj)
        {
            return new HpdfXrefEntry
            {
                EntryType = HpdfXrefEntryType.InUse,
                ByteOffset = 0, // Will be set when written
                GenerationNumber = 0,
                Object = obj
            };
        }

        /// <summary>
        /// Writes this xref entry to a stream in PDF format.
        /// Format: "nnnnnnnnnn ggggg n \r\n" where n is byte offset, g is generation, and last char is entry type
        /// </summary>
        public void WriteToStream(HpdfStream stream)
        {
            // PDF xref format: 10-digit byte offset, space, 5-digit generation, space, type, CRLF
            // Example: "0000000017 00000 n \r\n"
            var offset = ByteOffset.ToString("D10");
            var gen = GenerationNumber.ToString("D5");
            var type = (char)EntryType;

            stream.WriteString($"{offset} {gen} {type} \r\n");
        }
    }
}
