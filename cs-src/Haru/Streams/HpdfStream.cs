using System;
using System.IO;
using System.Text;
using Haru.Security;

namespace Haru.Streams
{
    /// <summary>
    /// Base class for HPDF streams
    /// </summary>
    public abstract class HpdfStream : IDisposable
    {
        /// <summary>
        /// Gets or sets the encryption handler for encrypting content during writing.
        /// When set, strings and streams will be encrypted as they are written.
        /// </summary>
        public HpdfEncrypt EncryptionContext { get; set; }

        /// <summary>
        /// Gets the type of this stream
        /// </summary>
        public abstract HpdfStreamType StreamType { get; }

        /// <summary>
        /// Gets whether this stream supports reading
        /// </summary>
        public abstract bool CanRead { get; }

        /// <summary>
        /// Gets whether this stream supports writing
        /// </summary>
        public abstract bool CanWrite { get; }

        /// <summary>
        /// Gets whether this stream supports seeking
        /// </summary>
        public abstract bool CanSeek { get; }

        /// <summary>
        /// Gets the current position in the stream
        /// </summary>
        public abstract long Position { get; }

        /// <summary>
        /// Gets the size of the stream
        /// </summary>
        public abstract long Size { get; }

        /// <summary>
        /// Gets whether the stream is at the end
        /// </summary>
        public virtual bool IsEof => Position >= Size;

        /// <summary>
        /// Writes bytes to the stream
        /// </summary>
        public abstract void Write(byte[] buffer, int offset, int count);

        /// <summary>
        /// Writes bytes to the stream
        /// </summary>
        public void Write(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Writes a single byte to the stream
        /// </summary>
        public void WriteByte(byte value)
        {
            Write(new[] { value }, 0, 1);
        }

        /// <summary>
        /// Reads bytes from the stream
        /// </summary>
        public abstract int Read(byte[] buffer, int offset, int count);

        /// <summary>
        /// Seeks to a position in the stream
        /// </summary>
        public abstract void Seek(long position, SeekOrigin origin);

        /// <summary>
        /// Flushes any buffered data
        /// </summary>
        public virtual void Flush()
        {
            // Default implementation does nothing
        }

        /// <summary>
        /// Disposes the stream
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the stream
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
