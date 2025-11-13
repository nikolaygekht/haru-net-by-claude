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
    /// <summary>
    /// Integration tests using real PDF stream examples
    /// </summary>
    public class HpdfStreamObjectIntegrationTests
    {
        private byte[] LoadEmbeddedResource(string resourceName)
        {
            var assembly = typeof(HpdfStreamObjectIntegrationTests).Assembly;
            var fullResourceName = $"Haru.Test.Resources.{resourceName}";

            using (var stream = assembly.GetManifestResourceStream(fullResourceName))
            {
                if (stream is null)
                {
                    throw new FileNotFoundException($"Embedded resource '{fullResourceName}' not found");
                }

                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }
            }
        }

        [Fact]
        public void StreamObject_UncompressedGraphicsStream_MatchesRealPDF()
        {
            // Load the real uncompressed stream from embedded resource
            byte[] fileContent = LoadEmbeddedResource("stream1.txt");
            string content = System.Text.Encoding.ASCII.GetString(fileContent);

            // Extract just the stream content (between "stream\n" and "\nendstream")
            int streamStart = content.IndexOf("stream\n") + 7;
            int streamEnd = content.LastIndexOf("\nendstream");
            string streamContent = content.Substring(streamStart, streamEnd - streamStart);

            // Create a stream object with the same content
            using var obj = new HpdfStreamObject();
            obj.Filter = HpdfStreamFilter.None; // No compression
            obj.WriteToStream(System.Text.Encoding.ASCII.GetBytes(streamContent));

            // Write and verify
            using var outputStream = new HpdfMemoryStream();
            obj.WriteValue(outputStream);
            string output = System.Text.Encoding.ASCII.GetString(outputStream.ToArray());

            // Should contain the graphics operators
            output.Should().Contain("stream");
            output.Should().Contain("endstream");
            output.Should().Contain("1 w"); // Line width operator
            output.Should().Contain("BT"); // Begin text
            output.Should().Contain("ET"); // End text
            output.Should().Contain("LineDemo"); // The text content
            output.Should().Contain("/F1 24 Tf"); // Font selection
            output.Should().Contain("50 50 495 731 re"); // Rectangle

            // Length should match the stream content length
            obj.TryGetValue("Length", out var lengthObj).Should().BeTrue();
            var length = lengthObj as HpdfNumber;
            length!.Value.Should().Be(streamContent.Length);
        }

        [Fact]
        public void StreamObject_CompressedStream_CanDecompressToOriginal()
        {
            // Load the compressed stream from embedded resource
            byte[] fileContent = LoadEmbeddedResource("stream2.txt");

            // Extract the compressed data (between "stream" and "endstream")
            string fileText = System.Text.Encoding.Latin1.GetString(fileContent);
            int streamKeywordPos = fileText.IndexOf("stream");
            if (streamKeywordPos < 0)
            {
                throw new InvalidOperationException("'stream' keyword not found");
            }

            // Skip past "stream" and any CR/LF
            int streamStart = streamKeywordPos + 6; // "stream" is 6 chars
            while (streamStart < fileText.Length && (fileText[streamStart] == '\r' || fileText[streamStart] == '\n'))
            {
                streamStart++;
            }

            int streamEnd = fileText.IndexOf("endstream");
            if (streamEnd < 0)
            {
                throw new InvalidOperationException("'endstream' keyword not found");
            }

            // Back up over any CR/LF before endstream
            while (streamEnd > streamStart && (fileText[streamEnd - 1] == '\r' || fileText[streamEnd - 1] == '\n'))
            {
                streamEnd--;
            }

            if (streamEnd <= streamStart)
            {
                throw new InvalidOperationException("Invalid stream format in test file");
            }

            int compressedLength = streamEnd - streamStart;
            byte[] compressedData = new byte[compressedLength];
            Array.Copy(fileContent, streamStart, compressedData, 0, compressedLength);

            // Decompress the data to see what the original looks like
            // PDF FlateDecode uses zlib format, which .NET's DeflateStream doesn't directly support
            // Zlib has a 2-byte header and 4-byte Adler32 checksum at the end
            // Skip the first 2 bytes (zlib header) and decompress the rest (except last 4 bytes)
            byte[] decompressed;
            if (compressedData.Length > 6) // At least header + some data + checksum
            {
                byte[] rawDeflateData = new byte[compressedData.Length - 6];
                Array.Copy(compressedData, 2, rawDeflateData, 0, rawDeflateData.Length);

                using (var input = new MemoryStream(rawDeflateData))
                using (var decompressor = new DeflateStream(input, CompressionMode.Decompress))
                using (var decompressStream = new MemoryStream())
                {
                    decompressor.CopyTo(decompressStream);
                    decompressed = decompressStream.ToArray();
                }
            }
            else
            {
                throw new InvalidOperationException("Compressed data too short");
            }

            // The decompressed data should be larger than compressed
            decompressed.Length.Should().BeGreaterThan(compressedData.Length);

            // Now create a stream object with the decompressed data and compress it
            using var obj = new HpdfStreamObject();
            obj.Filter = HpdfStreamFilter.FlateDecode;
            obj.WriteToStream(decompressed);

            // Write the object
            using var outputStream = new HpdfMemoryStream();
            obj.WriteValue(outputStream);
            byte[] output = outputStream.ToArray();

            string outputText = System.Text.Encoding.Latin1.GetString(output);

            // Should have the filter entry
            outputText.Should().Contain("/Filter");
            outputText.Should().Contain("/FlateDecode");
            outputText.Should().Contain("stream");
            outputText.Should().Contain("endstream");

            // The compressed size should be less than the original decompressed size
            obj.TryGetValue("Length", out var lengthObj).Should().BeTrue();
            var length = lengthObj as HpdfNumber;
            length!.Value.Should().BeLessThan(decompressed.Length);
        }

        [Fact]
        public void StreamObject_RealGraphicsOperators_PreservesFormatting()
        {
            // Create a stream with typical PDF graphics operators
            string graphicsContent = @"1 w
50 50 495 731 re
S
/F1 24 Tf
BT
242.81601 791 Td
(LineDemo) Tj
ET";

            using var obj = new HpdfStreamObject();
            obj.WriteToStream(System.Text.Encoding.ASCII.GetBytes(graphicsContent));

            using var outputStream = new HpdfMemoryStream();
            obj.WriteValue(outputStream);
            string output = System.Text.Encoding.ASCII.GetString(outputStream.ToArray());

            // Verify all operators are preserved
            output.Should().Contain("1 w");
            output.Should().Contain("50 50 495 731 re");
            output.Should().Contain("S");
            output.Should().Contain("/F1 24 Tf");
            output.Should().Contain("BT");
            output.Should().Contain("242.81601 791 Td");
            output.Should().Contain("(LineDemo) Tj");
            output.Should().Contain("ET");
        }

        [Fact]
        public void StreamObject_CompressionRoundTrip_MaintainsData()
        {
            // Create original content
            string originalContent = @"q
1 0 0 1 100 200 cm
/F1 12 Tf
BT
0 0 Td
(Hello World) Tj
ET
Q";
            byte[] originalBytes = System.Text.Encoding.ASCII.GetBytes(originalContent);

            // Create stream object with compression
            using var obj = new HpdfStreamObject();
            obj.Filter = HpdfStreamFilter.FlateDecode;
            obj.WriteToStream(originalBytes);

            // Write to output
            using var outputStream = new HpdfMemoryStream();
            obj.WriteValue(outputStream);
            byte[] output = outputStream.ToArray();

            string outputText = System.Text.Encoding.Latin1.GetString(output);

            // Extract compressed data
            int streamStart = outputText.IndexOf("stream\n") + 7;
            int streamEnd = outputText.LastIndexOf("\nendstream");
            byte[] compressedData = new byte[streamEnd - streamStart];
            Array.Copy(output, streamStart, compressedData, 0, compressedData.Length);

            // Decompress zlib format and verify it matches original
            byte[] rawDeflateData = new byte[compressedData.Length - 6]; // Skip zlib header and checksum
            Array.Copy(compressedData, 2, rawDeflateData, 0, rawDeflateData.Length);

            using (var input = new MemoryStream(rawDeflateData))
            using (var decompressor = new DeflateStream(input, CompressionMode.Decompress))
            using (var resultStream = new MemoryStream())
            {
                decompressor.CopyTo(resultStream);
                byte[] decompressed = resultStream.ToArray();
                string decompressedText = System.Text.Encoding.ASCII.GetString(decompressed);

                decompressedText.Should().Be(originalContent);
            }
        }

        [Fact]
        public void StreamObject_LargeContent_CompressesEfficiently()
        {
            // Create a large repetitive content (typical in PDF graphics)
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 100; i++)
            {
                sb.AppendLine("1 0 0 1 100 100 cm");
                sb.AppendLine("q");
                sb.AppendLine("0.5 w");
                sb.AppendLine("0 0 100 100 re");
                sb.AppendLine("S");
                sb.AppendLine("Q");
            }
            string largeContent = sb.ToString();
            byte[] largeBytes = System.Text.Encoding.ASCII.GetBytes(largeContent);

            // Without compression
            using var objUncompressed = new HpdfStreamObject();
            objUncompressed.WriteToStream(largeBytes);
            using var uncompressedStream = new HpdfMemoryStream();
            objUncompressed.WriteValue(uncompressedStream);

            // With compression
            using var objCompressed = new HpdfStreamObject();
            objCompressed.Filter = HpdfStreamFilter.FlateDecode;
            objCompressed.WriteToStream(largeBytes);
            using var compressedStream = new HpdfMemoryStream();
            objCompressed.WriteValue(compressedStream);

            // Compressed should be significantly smaller
            compressedStream.Size.Should().BeLessThan(uncompressedStream.Size / 2);
        }

        [Fact]
        public void StreamObject_WithDictionaryMetadata_WritesCorrectFormat()
        {
            // Create a stream object like you'd see for an image or content stream
            using var obj = new HpdfStreamObject();
            obj.Add("Type", new HpdfName("XObject"));
            obj.Add("Subtype", new HpdfName("Form"));
            obj.Add("BBox", new HpdfArray(
                new HpdfReal(0),
                new HpdfReal(0),
                new HpdfReal(100),
                new HpdfReal(100)
            ));

            string content = "q 1 0 0 1 50 50 cm 0 0 20 20 re f Q";
            obj.WriteToStream(System.Text.Encoding.ASCII.GetBytes(content));

            using var outputStream = new HpdfMemoryStream();
            obj.WriteValue(outputStream);
            string output = System.Text.Encoding.ASCII.GetString(outputStream.ToArray());

            // Verify structure: dictionary, then stream data
            int dictEnd = output.IndexOf(">>");
            int streamStart = output.IndexOf("stream");

            dictEnd.Should().BeGreaterThan(0);
            streamStart.Should().BeGreaterThan(dictEnd);

            // Dictionary should contain metadata
            string dictPart = output.Substring(0, dictEnd);
            dictPart.Should().Contain("/Type /XObject");
            dictPart.Should().Contain("/Subtype /Form");
            dictPart.Should().Contain("/BBox");
            dictPart.Should().Contain("/Length");

            // Stream part should contain content
            output.Should().Contain("stream");
            output.Should().Contain(content);
            output.Should().Contain("endstream");
        }
    }
}
