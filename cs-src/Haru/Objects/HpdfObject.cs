using Haru.Streams;

namespace Haru.Objects
{
    /// <summary>
    /// Base class for all PDF objects
    /// </summary>
    public abstract class HpdfObject
    {
        /// <summary>
        /// Gets or sets the object ID (for indirect objects)
        /// </summary>
        public uint ObjectId { get; set; }

        /// <summary>
        /// Gets or sets the generation number (for versioning)
        /// </summary>
        public ushort GenerationNumber { get; set; }

        /// <summary>
        /// Gets the object class
        /// </summary>
        public abstract HpdfObjectClass ObjectClass { get; }

        /// <summary>
        /// Gets the object type flags
        /// </summary>
        public HpdfObjectType ObjectType { get; set; }

        /// <summary>
        /// Gets the object subclass (for specialized dictionaries)
        /// </summary>
        public virtual HpdfObjectSubclass ObjectSubclass => HpdfObjectSubclass.None;

        /// <summary>
        /// Gets whether this is a direct object (owned by container)
        /// </summary>
        public bool IsDirect => (ObjectType & HpdfObjectType.Direct) != 0;

        /// <summary>
        /// Gets whether this is an indirect object (managed by xref)
        /// </summary>
        public bool IsIndirect => (ObjectType & HpdfObjectType.Indirect) != 0;

        /// <summary>
        /// Gets whether this object is hidden
        /// </summary>
        public bool IsHidden => (ObjectType & HpdfObjectType.Hidden) != 0;

        /// <summary>
        /// Gets the real object ID (masking off the flag bits)
        /// </summary>
        public uint RealObjectId => ObjectId & 0x00FFFFFF;

        protected HpdfObject()
        {
            ObjectType = HpdfObjectType.Direct;
        }

        /// <summary>
        /// Writes the object value to a stream (without object wrapper)
        /// </summary>
        public abstract void WriteValue(HpdfStream stream);

        /// <summary>
        /// Writes the complete object to a stream (with object wrapper if indirect)
        /// </summary>
        public virtual void Write(HpdfStream stream)
        {
            if (IsIndirect)
            {
                // Write indirect object: "n g obj ... endobj"
                stream.WriteUInt(RealObjectId);
                stream.WriteChar(' ');
                stream.WriteUInt(GenerationNumber);
                stream.WriteString(" obj");
                stream.WriteLine();
                WriteValue(stream);
                stream.WriteLine();
                stream.WriteLine("endobj");
            }
            else
            {
                // Write direct object: just the value
                WriteValue(stream);
            }
        }
    }
}
