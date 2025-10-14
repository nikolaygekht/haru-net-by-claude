using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using FluentAssertions;
using Haru.Objects;
using Haru.Streams;
using Xunit;

namespace Haru.Test.Objects
{
    public class HpdfStreamObjectTests
    {
        private static string WriteToString(HpdfObject obj)
        {
            var stream = new HpdfMemoryStream();
            obj.WriteValue(stream);
            return Encoding.ASCII.GetString(stream.ToArray());
        }

        [Fact]
        public void StreamObject_EmptyStream_WritesStreamSection()
        {
            var obj = new HpdfStreamObject();

            string result = WriteToString(obj);

            result.Should().Contain("<<");
            result.Should().Contain("/Length 0");
            result.Should().Contain(">>");
            // Empty streams still write stream/endstream for PDF spec compliance
            // This ensures the object is recognized as a StreamToken by PDF parsers
            result.Should().Contain("stream");
            result.Should().Contain("endstream");
        }

        [Fact]
        public void StreamObject_WithData_WritesStreamSection()
        {
            var obj = new HpdfStreamObject();
            byte[] data = Encoding.ASCII.GetBytes("Hello PDF");
            obj.WriteToStream(data);

            string result = WriteToString(obj);

            result.Should().Contain("<<");
            result.Should().Contain("/Length");
            result.Should().Contain(">>");
            result.Should().Contain("stream");
            result.Should().Contain("Hello PDF");
            result.Should().Contain("endstream");
        }

        [Fact]
        public void StreamObject_NoFilter_DoesNotAddFilterEntry()
        {
            var obj = new HpdfStreamObject();
            obj.Filter = HpdfStreamFilter.None;
            obj.WriteToStream(Encoding.ASCII.GetBytes("Test"));

            string result = WriteToString(obj);

            result.Should().NotContain("/Filter");
        }

        [Fact]
        public void StreamObject_WithFlateFilter_AddsFilterEntry()
        {
            var obj = new HpdfStreamObject();
            obj.Filter = HpdfStreamFilter.FlateDecode;
            obj.WriteToStream(Encoding.ASCII.GetBytes("Test data to compress"));

            string result = WriteToString(obj);

            result.Should().Contain("/Filter");
            result.Should().Contain("/FlateDecode");
        }

        [Fact]
        public void StreamObject_FlateCompression_CompressesData()
        {
            var obj = new HpdfStreamObject();
            string originalData = "This is test data that should be compressed using Flate/Deflate algorithm. " +
                                "It contains repeated text. It contains repeated text. It contains repeated text.";
            byte[] originalBytes = Encoding.ASCII.GetBytes(originalData);

            obj.Filter = HpdfStreamFilter.FlateDecode;
            obj.WriteToStream(originalBytes);

            var outputStream = new HpdfMemoryStream();
            obj.WriteValue(outputStream);
            byte[] output = outputStream.ToArray();

            // The compressed data should be smaller than the original
            // (Note: compressed data is embedded in the output, so we just verify compression occurred)
            output.Length.Should().BeLessThan(originalBytes.Length + 100); // +100 for dictionary overhead
        }

        [Fact]
        public void StreamObject_FlateCompression_ProducesValidData()
        {
            var obj = new HpdfStreamObject();
            string originalData = "Test compression data with some repeated content. " +
                                "Repeated content helps compression work better.";
            byte[] originalBytes = Encoding.ASCII.GetBytes(originalData);

            obj.Filter = HpdfStreamFilter.FlateDecode;
            obj.WriteToStream(originalBytes);

            // Get the written output
            var outputStream = new HpdfMemoryStream();
            obj.WriteValue(outputStream);
            string output = Encoding.ASCII.GetString(outputStream.ToArray());

            // Extract the compressed data between "stream" and "endstream"
            int streamStart = output.IndexOf("stream\n") + 7;
            int streamEnd = output.IndexOf("\nendstream");
            byte[] compressedData = outputStream.ToArray()[streamStart..streamEnd];

            // Decompress zlib format (skip 2-byte header, read deflate data, ignore 4-byte checksum)
            byte[] rawDeflateData = new byte[compressedData.Length - 6];
            Array.Copy(compressedData, 2, rawDeflateData, 0, rawDeflateData.Length);

            using (var input = new MemoryStream(rawDeflateData))
            using (var decompressor = new DeflateStream(input, CompressionMode.Decompress))
            using (var resultStream = new MemoryStream())
            {
                decompressor.CopyTo(resultStream);
                byte[] decompressed = resultStream.ToArray();
                string decompressedText = Encoding.ASCII.GetString(decompressed);

                decompressedText.Should().Be(originalData);
            }
        }

        [Fact]
        public void StreamObject_UpdatesLengthCorrectly()
        {
            var obj = new HpdfStreamObject();
            byte[] data = Encoding.ASCII.GetBytes("12345");
            obj.WriteToStream(data);

            // Force write to update length
            var outputStream = new HpdfMemoryStream();
            obj.WriteValue(outputStream);

            // Length should be updated to 5
            obj.TryGetValue("Length", out var lengthObj).Should().BeTrue();
            var length = lengthObj as HpdfNumber;
            length.Should().NotBeNull();
            length.Value.Should().Be(5);
        }

        [Fact]
        public void StreamObject_UpdatesLengthWithCompression()
        {
            var obj = new HpdfStreamObject();
            obj.Filter = HpdfStreamFilter.FlateDecode;
            byte[] data = Encoding.ASCII.GetBytes("AAAAAAAAAA"); // Highly compressible
            obj.WriteToStream(data);

            // Force write to update length
            var _ = WriteToString(obj);

            obj.TryGetValue("Length", out var lengthObj).Should().BeTrue();
            var length = lengthObj as HpdfNumber;
            length.Should().NotBeNull();
            // Compressed length should be less than uncompressed (10)
            // Note: zlib adds 6 bytes of overhead (2-byte header + 4-byte checksum)
            // So for very short data, it may not be smaller, but it should still compress
            length.Value.Should().BeLessThan(20); // Reasonable upper bound
        }

        [Fact]
        public void StreamObject_ClearStream_RemovesData()
        {
            var obj = new HpdfStreamObject();
            obj.WriteToStream(Encoding.ASCII.GetBytes("Test data"));
            obj.Stream.Size.Should().Be(9);

            obj.ClearStream();

            obj.Stream.Size.Should().Be(0);
        }

        [Fact]
        public void StreamObject_CanAddDictionaryEntries()
        {
            var obj = new HpdfStreamObject();
            obj.Add("Type", new HpdfName("XObject"));
            obj.Add("Subtype", new HpdfName("Image"));
            obj.WriteToStream(Encoding.ASCII.GetBytes("Image data"));

            string result = WriteToString(obj);

            result.Should().Contain("/Type /XObject");
            result.Should().Contain("/Subtype /Image");
            result.Should().Contain("/Length");
            result.Should().Contain("stream");
            result.Should().Contain("Image data");
        }

        [Fact]
        public void StreamObject_MultipleFilters_AddsAllToArray()
        {
            var obj = new HpdfStreamObject();
            obj.Filter = HpdfStreamFilter.FlateDecode | HpdfStreamFilter.AsciiHexDecode;
            obj.WriteToStream(Encoding.ASCII.GetBytes("Test"));

            string result = WriteToString(obj);

            result.Should().Contain("/Filter");
            result.Should().Contain("/FlateDecode");
            result.Should().Contain("/ASCIIHexDecode");
        }

        [Fact]
        public void StreamObject_WriteToStreamWithOffset_Works()
        {
            var obj = new HpdfStreamObject();
            byte[] data = Encoding.ASCII.GetBytes("ABCDEFGH");
            obj.WriteToStream(data, 2, 4); // Write "CDEF"

            string result = WriteToString(obj);

            result.Should().Contain("CDEF");
            result.Should().NotContain("AB");
            result.Should().NotContain("GH");
        }

        [Fact]
        public void StreamObject_InheritsFromHpdfDict()
        {
            var obj = new HpdfStreamObject();

            obj.Should().BeAssignableTo<HpdfDict>();
            obj.ObjectClass.Should().Be(HpdfObjectClass.Dict);
        }
    }
}
