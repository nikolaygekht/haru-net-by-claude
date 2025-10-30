using System;
using System.Collections.Generic;
using System.IO;

namespace Haru.Streams
{
    /// <summary>
    /// Memory-based HPDF stream that grows dynamically
    /// </summary>
    public class HpdfMemoryStream : HpdfStream
    {
        private readonly List<byte[]> _buffers;
        private readonly int _bufferSize;
        private long _position;
        private long _size;
        private byte[] _currentWriteBuffer;
        private int _currentWritePosition;

        /// <summary>
        /// Creates a new HpdfMemoryStream with the default buffer size
        /// </summary>
        public HpdfMemoryStream()
            : this(4096)
        {
        }

        /// <summary>
        /// Creates a new HpdfMemoryStream with the specified buffer size
        /// </summary>
        /// <param name="bufferSize">Size of each internal buffer block</param>
        public HpdfMemoryStream(int bufferSize)
        {
            if (bufferSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(bufferSize), "Buffer size must be positive");

            _bufferSize = bufferSize;
            _buffers = new List<byte[]>();
            _position = 0;
            _size = 0;
            _currentWriteBuffer = new byte[bufferSize];
            _currentWritePosition = 0;
            _buffers.Add(_currentWriteBuffer);
        }

        /// <inheritdoc/>
        public override HpdfStreamType StreamType => HpdfStreamType.Memory;

        /// <inheritdoc/>
        public override bool CanRead => true;

        /// <inheritdoc/>
        public override bool CanWrite => true;

        /// <inheritdoc/>
        public override bool CanSeek => true;

        /// <inheritdoc/>
        public override long Position => _position;

        /// <inheritdoc/>
        public override long Size => _size;

        /// <summary>
        /// Gets the number of buffer blocks allocated
        /// </summary>
        public int BufferCount => _buffers.Count;

        /// <summary>
        /// Gets the size of each buffer block
        /// </summary>
        public int BufferSize => _bufferSize;

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (buffer is null)
                throw new ArgumentNullException(nameof(buffer));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (offset + count > buffer.Length)
                throw new ArgumentException("Offset and count exceed buffer length");

            while (count > 0)
            {
                // Check if we need a new buffer
                if (_currentWritePosition >= _bufferSize)
                {
                    _currentWriteBuffer = new byte[_bufferSize];
                    _currentWritePosition = 0;
                    _buffers.Add(_currentWriteBuffer);
                }

                // Write as much as we can to the current buffer
                int bytesToWrite = Math.Min(count, _bufferSize - _currentWritePosition);
                Array.Copy(buffer, offset, _currentWriteBuffer, _currentWritePosition, bytesToWrite);

                _currentWritePosition += bytesToWrite;
                offset += bytesToWrite;
                count -= bytesToWrite;
                _position += bytesToWrite;

                if (_position > _size)
                    _size = _position;
            }
        }

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer is null)
                throw new ArgumentNullException(nameof(buffer));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (offset + count > buffer.Length)
                throw new ArgumentException("Offset and count exceed buffer length");

            if (_position >= _size)
                return 0;

            long bytesAvailable = _size - _position;
            int bytesToRead = (int)Math.Min(count, bytesAvailable);
            int bytesRead = 0;

            while (bytesToRead > 0)
            {
                int bufferIndex = (int)(_position / _bufferSize);
                int positionInBuffer = (int)(_position % _bufferSize);
                int bytesInCurrentBuffer = Math.Min(bytesToRead, _bufferSize - positionInBuffer);

                if (bufferIndex >= _buffers.Count)
                    break;

                Array.Copy(_buffers[bufferIndex], positionInBuffer, buffer, offset, bytesInCurrentBuffer);

                offset += bytesInCurrentBuffer;
                bytesToRead -= bytesInCurrentBuffer;
                bytesRead += bytesInCurrentBuffer;
                _position += bytesInCurrentBuffer;
            }

            return bytesRead;
        }

        /// <inheritdoc/>
        public override void Seek(long position, SeekOrigin origin)
        {
            long newPosition = origin switch
            {
                SeekOrigin.Begin => position,
                SeekOrigin.Current => _position + position,
                SeekOrigin.End => _size + position,
                _ => throw new ArgumentException("Invalid seek origin", nameof(origin))
            };

            if (newPosition < 0)
                throw new IOException("Cannot seek before the beginning of the stream");

            _position = newPosition;
        }

        /// <summary>
        /// Gets a buffer block by index
        /// </summary>
        /// <param name="index">Index of the buffer</param>
        /// <returns>The buffer and its valid length</returns>
        public (byte[] Buffer, int Length) GetBuffer(int index)
        {
            if (index < 0 || index >= _buffers.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            byte[] buffer = _buffers[index];
            int length = index < _buffers.Count - 1 ? _bufferSize : _currentWritePosition;
            return (buffer, length);
        }

        /// <summary>
        /// Copies all data to a single byte array
        /// </summary>
        public byte[] ToArray()
        {
            byte[] result = new byte[_size];
            int offset = 0;

            for (int i = 0; i < _buffers.Count; i++)
            {
                var (buffer, length) = GetBuffer(i);
                Array.Copy(buffer, 0, result, offset, length);
                offset += length;
            }

            return result;
        }

        /// <summary>
        /// Resets the stream and optionally writes new data
        /// </summary>
        public void Rewrite(byte[] data)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            Clear();
            Write(data, 0, data.Length);
            _position = 0;
        }

        /// <summary>
        /// Clears all data from the stream
        /// </summary>
        public void Clear()
        {
            _buffers.Clear();
            _currentWriteBuffer = new byte[_bufferSize];
            _currentWritePosition = 0;
            _buffers.Add(_currentWriteBuffer);
            _position = 0;
            _size = 0;
        }
    }
}
