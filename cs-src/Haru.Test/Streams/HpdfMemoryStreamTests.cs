using System;
using System.IO;
using FluentAssertions;
using Haru.Streams;
using Xunit;

#pragma warning disable CA2000 // Dispose objects before losing scope

namespace Haru.Test.Streams
{
    public class HpdfMemoryStreamTests
    {
        [Fact]
        public void Constructor_Default_CreatesStream()
        {
            var stream = new HpdfMemoryStream();

            stream.StreamType.Should().Be(HpdfStreamType.Memory);
            stream.CanRead.Should().BeTrue();
            stream.CanWrite.Should().BeTrue();
            stream.CanSeek.Should().BeTrue();
            stream.Position.Should().Be(0);
            stream.Size.Should().Be(0);
        }

        [Fact]
        public void Constructor_WithBufferSize_SetsBufferSize()
        {
            var stream = new HpdfMemoryStream(1024);

            stream.BufferSize.Should().Be(1024);
        }

        [Fact]
        public void Constructor_WithInvalidBufferSize_ThrowsException()
        {
            Action act = () => new HpdfMemoryStream(0);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Write_SmallData_WritesSuccessfully()
        {
            var stream = new HpdfMemoryStream();
            byte[] data = { 1, 2, 3, 4, 5 };

            stream.Write(data);

            stream.Size.Should().Be(5);
            stream.Position.Should().Be(5);
        }

        [Fact]
        public void Write_LargeData_SpansMultipleBuffers()
        {
            var stream = new HpdfMemoryStream(10); // Small buffer for testing
            byte[] data = new byte[25];
            for (int i = 0; i < data.Length; i++)
                data[i] = (byte)i;

            stream.Write(data);

            stream.Size.Should().Be(25);
            stream.BufferCount.Should().Be(3); // 10 + 10 + 5
        }

        [Fact]
        public void Read_AfterWrite_ReturnsWrittenData()
        {
            var stream = new HpdfMemoryStream();
            byte[] writeData = { 10, 20, 30, 40, 50 };
            stream.Write(writeData);
            stream.Seek(0, SeekOrigin.Begin);

            byte[] readData = new byte[5];
            int bytesRead = stream.Read(readData, 0, 5);

            bytesRead.Should().Be(5);
            readData.Should().Equal(writeData);
        }

        [Fact]
        public void Read_AcrossBuffers_ReturnsCorrectData()
        {
            var stream = new HpdfMemoryStream(10);
            byte[] writeData = new byte[25];
            for (int i = 0; i < writeData.Length; i++)
                writeData[i] = (byte)i;
            stream.Write(writeData);
            stream.Seek(0, SeekOrigin.Begin);

            byte[] readData = new byte[25];
            int bytesRead = stream.Read(readData, 0, 25);

            bytesRead.Should().Be(25);
            readData.Should().Equal(writeData);
        }

        [Fact]
        public void Read_PastEnd_ReturnsZero()
        {
            var stream = new HpdfMemoryStream();
            byte[] data = { 1, 2, 3 };
            stream.Write(data);

            byte[] readBuffer = new byte[5];
            int bytesRead = stream.Read(readBuffer, 0, 5);

            bytesRead.Should().Be(0);
        }

        [Fact]
        public void Seek_Begin_SetsPositionFromStart()
        {
            var stream = new HpdfMemoryStream();
            stream.Write(new byte[20]);

            stream.Seek(10, SeekOrigin.Begin);

            stream.Position.Should().Be(10);
        }

        [Fact]
        public void Seek_Current_SetsPositionRelative()
        {
            var stream = new HpdfMemoryStream();
            stream.Write(new byte[20]);
            stream.Seek(5, SeekOrigin.Begin);

            stream.Seek(3, SeekOrigin.Current);

            stream.Position.Should().Be(8);
        }

        [Fact]
        public void Seek_End_SetsPositionFromEnd()
        {
            var stream = new HpdfMemoryStream();
            stream.Write(new byte[20]);

            stream.Seek(-5, SeekOrigin.End);

            stream.Position.Should().Be(15);
        }

        [Fact]
        public void Seek_NegativePosition_ThrowsException()
        {
            var stream = new HpdfMemoryStream();

            Action act = () => stream.Seek(-1, SeekOrigin.Begin);

            act.Should().Throw<IOException>();
        }

        [Fact]
        public void IsEof_AtEnd_ReturnsTrue()
        {
            var stream = new HpdfMemoryStream();
            stream.Write(new byte[10]);

            stream.IsEof.Should().BeTrue();
        }

        [Fact]
        public void IsEof_NotAtEnd_ReturnsFalse()
        {
            var stream = new HpdfMemoryStream();
            stream.Write(new byte[10]);
            stream.Seek(5, SeekOrigin.Begin);

            stream.IsEof.Should().BeFalse();
        }

        [Fact]
        public void GetBuffer_ValidIndex_ReturnsBuffer()
        {
            var stream = new HpdfMemoryStream(10);
            stream.Write(new byte[25]);

            var (buffer, length) = stream.GetBuffer(0);

            buffer.Should().NotBeNull();
            length.Should().Be(10);
        }

        [Fact]
        public void GetBuffer_LastBuffer_ReturnsPartialLength()
        {
            var stream = new HpdfMemoryStream(10);
            stream.Write(new byte[25]);

            var (buffer, length) = stream.GetBuffer(2);

            buffer.Should().NotBeNull();
            length.Should().Be(5);
        }

        [Fact]
        public void GetBuffer_InvalidIndex_ThrowsException()
        {
            var stream = new HpdfMemoryStream();

            Action act = () => stream.GetBuffer(10);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void ToArray_ReturnsAllData()
        {
            var stream = new HpdfMemoryStream(10);
            byte[] data = new byte[25];
            for (int i = 0; i < data.Length; i++)
                data[i] = (byte)i;
            stream.Write(data);

            byte[] result = stream.ToArray();

            result.Should().Equal(data);
        }

        [Fact]
        public void Rewrite_ReplacesData()
        {
            var stream = new HpdfMemoryStream();
            stream.Write(new byte[] { 1, 2, 3, 4, 5 });

            stream.Rewrite(new byte[] { 10, 20, 30 });

            stream.Size.Should().Be(3);
            stream.Position.Should().Be(0);
            byte[] result = stream.ToArray();
            result.Should().Equal(10, 20, 30);
        }

        [Fact]
        public void Clear_RemovesAllData()
        {
            var stream = new HpdfMemoryStream();
            stream.Write(new byte[] { 1, 2, 3, 4, 5 });

            stream.Clear();

            stream.Size.Should().Be(0);
            stream.Position.Should().Be(0);
            stream.BufferCount.Should().Be(1);
        }

        [Fact]
        public void WriteByte_WritesSingleByte()
        {
            var stream = new HpdfMemoryStream();

            stream.WriteByte(42);

            stream.Size.Should().Be(1);
            byte[] data = stream.ToArray();
            data[0].Should().Be(42);
        }

        [Fact]
        public void Write_WithOffset_WritesCorrectData()
        {
            var stream = new HpdfMemoryStream();
            byte[] data = { 1, 2, 3, 4, 5 };

            stream.Write(data, 2, 2);

            stream.Size.Should().Be(2);
            byte[] result = stream.ToArray();
            result.Should().Equal(3, 4);
        }

        [Fact]
        public void Read_WithOffset_ReadsIntoCorrectPosition()
        {
            var stream = new HpdfMemoryStream();
            stream.Write(new byte[] { 10, 20, 30 });
            stream.Seek(0, SeekOrigin.Begin);

            byte[] buffer = new byte[5];
            stream.Read(buffer, 2, 3);

            buffer[0].Should().Be(0);
            buffer[1].Should().Be(0);
            buffer[2].Should().Be(10);
            buffer[3].Should().Be(20);
            buffer[4].Should().Be(30);
        }
    }
}
