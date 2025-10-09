using System;
using Haru.Streams;

namespace Haru.Objects
{
    /// <summary>
    /// PDF stream object - a dictionary with associated binary data
    /// Format: &lt;&lt; dictionary &gt;&gt; stream...endstream
    /// </summary>
    public class HpdfStreamObject : HpdfDict
    {
        private HpdfMemoryStream _stream;
        private HpdfStreamFilter _filter;

        /// <summary>
        /// Gets the stream containing the binary data
        /// </summary>
        public HpdfMemoryStream Stream
        {
            get
            {
                if (_stream == null)
                {
                    _stream = new HpdfMemoryStream();
                }
                return _stream;
            }
        }

        /// <summary>
        /// Gets or sets the filter(s) to apply when writing the stream
        /// </summary>
        public HpdfStreamFilter Filter
        {
            get => _filter;
            set => _filter = value;
        }

        /// <summary>
        /// Creates a new stream object
        /// </summary>
        public HpdfStreamObject()
        {
            _filter = HpdfStreamFilter.None;

            // Add the Length entry (will be updated when writing)
            this["Length"] = new HpdfNumber(0);
        }

        /// <summary>
        /// Writes data to the stream
        /// </summary>
        public void WriteToStream(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            Stream.Write(data);
        }

        /// <summary>
        /// Writes data to the stream
        /// </summary>
        public void WriteToStream(byte[] data, int offset, int count)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            Stream.Write(data, offset, count);
        }

        /// <summary>
        /// Clears the stream data
        /// </summary>
        public void ClearStream()
        {
            if (_stream != null)
            {
                _stream.Clear();
            }
        }

        /// <summary>
        /// Writes the complete stream object to a PDF stream
        /// </summary>
        public override void WriteValue(HpdfStream stream)
        {
            // Before writing, update the Filter array if filters are set
            UpdateFilterArray();

            // If there's stream data, prepare it and update Length BEFORE writing dictionary
            byte[] streamDataToWrite = null;
            if (_stream != null && _stream.Size > 0)
            {
                streamDataToWrite = PrepareStreamData(stream);
                // Update the Length entry with the actual size
                this["Length"] = new HpdfNumber(streamDataToWrite.Length);
            }

            // Write the dictionary part
            stream.WriteString("<<");
            stream.WriteLine();

            foreach (var kvp in this)
            {
                stream.WriteEscapedName(kvp.Key);
                stream.WriteChar(' ');

                // Write indirect reference if object is indirect, otherwise write value
                if (kvp.Value.IsIndirect && kvp.Value.ObjectId != 0)
                {
                    stream.WriteUInt(kvp.Value.RealObjectId);
                    stream.WriteChar(' ');
                    stream.WriteUInt(kvp.Value.GenerationNumber);
                    stream.WriteString(" R");
                }
                else
                {
                    kvp.Value.WriteValue(stream);
                }
                stream.WriteLine();
            }

            stream.WriteString(">>");

            // If there's stream data, write the stream section
            if (streamDataToWrite != null)
            {
                stream.WriteLine();
                stream.WriteString("stream");
                stream.WriteLine();

                // Write the prepared stream data
                stream.Write(streamDataToWrite);

                stream.WriteLine();
                stream.WriteString("endstream");
            }
        }

        /// <summary>
        /// Updates the Filter array in the dictionary based on the current filter settings
        /// </summary>
        private void UpdateFilterArray()
        {
            if (_filter == HpdfStreamFilter.None)
            {
                // Remove Filter entry if no filters
                Remove("Filter");
            }
            else
            {
                // Create or get the Filter array
                HpdfArray filterArray;
                if (TryGetValue("Filter", out var existingFilter) && existingFilter is HpdfArray array)
                {
                    filterArray = array;
                    filterArray.Clear();
                }
                else
                {
                    filterArray = new HpdfArray();
                    this["Filter"] = filterArray;
                }

                // Add filter names based on flags
                if ((_filter & HpdfStreamFilter.FlateDecode) != 0)
                    filterArray.Add(new HpdfName("FlateDecode"));

                if ((_filter & HpdfStreamFilter.AsciiHexDecode) != 0)
                    filterArray.Add(new HpdfName("ASCIIHexDecode"));

                if ((_filter & HpdfStreamFilter.Ascii85Decode) != 0)
                    filterArray.Add(new HpdfName("ASCII85Decode"));

                if ((_filter & HpdfStreamFilter.DctDecode) != 0)
                    filterArray.Add(new HpdfName("DCTDecode"));

                if ((_filter & HpdfStreamFilter.CcittFaxDecode) != 0)
                    filterArray.Add(new HpdfName("CCITTFaxDecode"));
            }
        }

        /// <summary>
        /// Prepares the stream data with filters and encryption applied
        /// </summary>
        /// <returns>The prepared stream data ready to write</returns>
        private byte[] PrepareStreamData(HpdfStream outputStream)
        {
            if (_stream == null || _stream.Size == 0)
                return null;

            // Get the raw data
            byte[] data = _stream.ToArray();
            byte[] filteredData = data;

            // Apply filters
            if ((_filter & HpdfStreamFilter.FlateDecode) != 0)
            {
                filteredData = ApplyFlateFilter(filteredData);
            }

            // Encrypt the data if encryption context is set
            // NOTE: Encryption happens AFTER compression (per PDF spec)
            if (outputStream.EncryptionContext != null)
            {
                filteredData = outputStream.EncryptionContext.Encrypt(filteredData);
            }

            return filteredData;
        }

        /// <summary>
        /// Applies Flate (deflate/zlib) compression to the data
        /// PDF FlateDecode uses zlib format, which includes a 2-byte header and 4-byte Adler32 checksum
        /// </summary>
        private byte[] ApplyFlateFilter(byte[] data)
        {
            using (var outputStream = new System.IO.MemoryStream())
            {
                // Write zlib header (CMF and FLG bytes)
                // CMF: Compression Method (8 = deflate) and Info (7 = 32K window)
                outputStream.WriteByte(0x78); // CMF: 0111 1000
                outputStream.WriteByte(0x9C); // FLG: 1001 1100 (default compression, FCHECK makes header valid)

                // Write deflated data
                using (var deflateStream = new System.IO.Compression.DeflateStream(
                    outputStream,
                    System.IO.Compression.CompressionMode.Compress,
                    leaveOpen: true))
                {
                    deflateStream.Write(data, 0, data.Length);
                }

                // Write Adler32 checksum (big-endian)
                uint adler = ComputeAdler32(data);
                outputStream.WriteByte((byte)((adler >> 24) & 0xFF));
                outputStream.WriteByte((byte)((adler >> 16) & 0xFF));
                outputStream.WriteByte((byte)((adler >> 8) & 0xFF));
                outputStream.WriteByte((byte)(adler & 0xFF));

                return outputStream.ToArray();
            }
        }

        /// <summary>
        /// Computes Adler32 checksum as required by zlib format
        /// </summary>
        private static uint ComputeAdler32(byte[] data)
        {
            const uint MOD_ADLER = 65521;
            uint a = 1, b = 0;

            foreach (byte by in data)
            {
                a = (a + by) % MOD_ADLER;
                b = (b + a) % MOD_ADLER;
            }

            return (b << 16) | a;
        }
    }
}
