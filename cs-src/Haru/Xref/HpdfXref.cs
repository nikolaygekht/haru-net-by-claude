using System;
using System.Collections.Generic;
using Haru.Objects;
using Haru.Streams;

namespace Haru.Xref
{
    /// <summary>
    /// Represents a PDF cross-reference (xref) table that tracks all objects in a PDF.
    /// The xref table allows PDF readers to quickly locate objects without scanning the entire file.
    /// </summary>
    public class HpdfXref
    {
        /// <summary>
        /// Maximum number of entries allowed in a single xref table (PDF spec limit)
        /// </summary>
        public const int MaxXrefEntries = 8388607; // 2^23 - 1

        /// <summary>
        /// Gets the starting object ID offset for this xref section.
        /// Typically 0 for the main xref, but can be non-zero for incremental updates.
        /// </summary>
        public uint StartOffset { get; }

        /// <summary>
        /// Gets the list of xref entries
        /// </summary>
        public List<HpdfXrefEntry> Entries { get; }

        /// <summary>
        /// Gets or sets the byte position where this xref table was written in the PDF file
        /// </summary>
        public uint Address { get; set; }

        /// <summary>
        /// Gets or sets the previous xref table (for incremental updates)
        /// </summary>
        public HpdfXref Previous { get; set; }

        /// <summary>
        /// Gets the trailer dictionary that appears after the xref table
        /// </summary>
        public HpdfDict Trailer { get; }

        /// <summary>
        /// Initializes a new xref table with the specified starting offset
        /// </summary>
        /// <param name="startOffset">Starting object ID offset (usually 0)</param>
        public HpdfXref(uint startOffset = 0)
        {
            StartOffset = startOffset;
            Entries = new List<HpdfXrefEntry>();
            Address = 0;
            Previous = null;
            Trailer = new HpdfDict();

            // If this is the primary xref (offset 0), add the mandatory free entry
            if (startOffset == 0)
            {
                Entries.Add(HpdfXrefEntry.CreateFreeEntry());
            }
        }

        /// <summary>
        /// Adds a PDF object to the xref table and assigns it an object ID
        /// </summary>
        /// <param name="obj">The object to add</param>
        /// <returns>The assigned object ID</returns>
        /// <exception cref="ArgumentNullException">If obj is null</exception>
        /// <exception cref="HpdfException">If xref table is full</exception>
        public uint Add(HpdfObject obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            if (Entries.Count >= MaxXrefEntries)
                throw new HpdfException(HpdfErrorCode.XRefCountError);

            var entry = HpdfXrefEntry.CreateInUseEntry(obj);
            Entries.Add(entry);

            // Calculate object ID: start_offset + index
            uint objectId = StartOffset + (uint)Entries.Count - 1;

            // Assign object ID and generation number to the object
            obj.ObjectId = objectId;
            obj.GenerationNumber = entry.GenerationNumber;
            obj.ObjectType |= HpdfObjectType.Indirect;

            return objectId;
        }

        /// <summary>
        /// Gets an xref entry by its index within this xref section
        /// </summary>
        /// <param name="index">Zero-based index</param>
        /// <returns>The xref entry</returns>
        /// <exception cref="ArgumentOutOfRangeException">If index is invalid</exception>
        public HpdfXrefEntry GetEntry(int index)
        {
            if (index < 0 || index >= Entries.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            return Entries[index];
        }

        /// <summary>
        /// Gets an xref entry by object ID, searching this xref and previous xrefs
        /// </summary>
        /// <param name="objectId">The object ID to find</param>
        /// <returns>The xref entry, or null if not found</returns>
        public HpdfXrefEntry GetEntryByObjectId(uint objectId)
        {
            HpdfXref current = this;

            while (current != null)
            {
                // Check if object ID falls within this xref's range
                uint minId = current.StartOffset;
                uint maxId = current.StartOffset + (uint)current.Entries.Count;

                if (objectId >= minId && objectId < maxId)
                {
                    int index = (int)(objectId - current.StartOffset);
                    return current.Entries[index];
                }

                current = current.Previous;
            }

            return null;
        }

        /// <summary>
        /// Writes all objects and the xref table to a stream in PDF format
        /// </summary>
        /// <param name="stream">The stream to write to</param>
        public void WriteToStream(HpdfStream stream)
        {
            // Write all objects in the xref table (and previous xrefs)
            WriteObjects(stream);

            // Write the xref table itself
            WriteXrefTable(stream);

            // Write the trailer
            WriteTrailer(stream);
        }

        /// <summary>
        /// Writes all PDF objects to the stream (recursively including previous xrefs)
        /// </summary>
        private void WriteObjects(HpdfStream stream)
        {
            // Recursively write previous xrefs first
            Previous?.WriteObjects(stream);

            // Write objects for this xref
            // Determine starting index (skip entry 0 for primary xref)
            int startIndex = (StartOffset == 0) ? 1 : 0;

            for (int i = startIndex; i < Entries.Count; i++)
            {
                uint objectId = StartOffset + (uint)i;
                ushort genNo = Entries[i].GenerationNumber;

                // Record the byte offset where this object starts
                Entries[i].ByteOffset = (uint)stream.Size;

                // Write: "obj_id gen_no obj\n"
                stream.WriteString($"{objectId} {genNo} obj\n");

                // Write the object value
                Entries[i].Object.WriteValue(stream);

                // Write: "\nendobj\n"
                stream.WriteString("\nendobj\n");
            }
        }

        /// <summary>
        /// Writes the xref table(s) to the stream
        /// </summary>
        private void WriteXrefTable(HpdfStream stream)
        {
            // Recursively write previous xref tables first
            Previous?.WriteXrefTable(stream);

            // Write this xref table
            // Record where this xref table starts
            Address = (uint)stream.Size;

            // Write xref header: "xref\n"
            stream.WriteString("xref\n");

            // Write subsection header: "start_offset count\n"
            stream.WriteString($"{StartOffset} {Entries.Count}\n");

            // Write each xref entry
            foreach (var entry in Entries)
            {
                entry.WriteToStream(stream);
            }
        }

        /// <summary>
        /// Writes the trailer dictionary and file footer
        /// </summary>
        private void WriteTrailer(HpdfStream stream)
        {
            // Calculate total number of objects (Size entry in trailer)
            uint maxObjectId = StartOffset + (uint)Entries.Count;
            Trailer.Add("Size", new HpdfNumber((int)maxObjectId));

            // If there's a previous xref, add Prev entry
            if (Previous != null)
            {
                Trailer.Add("Prev", new HpdfNumber((int)Previous.Address));
            }

            // Write: "trailer\n"
            stream.WriteString("trailer\n");

            // Write the trailer dictionary
            Trailer.WriteValue(stream);

            // Write: "\nstartxref\n"
            stream.WriteString("\nstartxref\n");

            // Write the byte offset of this xref table
            stream.WriteString($"{Address}\n");

            // Write: "%%EOF\n"
            stream.WriteString("%%EOF\n");
        }

        /// <summary>
        /// Gets the total count of all entries including previous xrefs
        /// </summary>
        public int GetTotalEntryCount()
        {
            int count = Entries.Count;
            HpdfXref current = Previous;

            while (current != null)
            {
                count += current.Entries.Count;
                current = current.Previous;
            }

            return count;
        }
    }
}
