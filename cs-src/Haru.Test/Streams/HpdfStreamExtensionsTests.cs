using System;
using System.IO;
using System.Text;
using FluentAssertions;
using Haru.Streams;
using Xunit;

#pragma warning disable CA2000 // Dispose objects before losing scope

namespace Haru.Test.Streams
{
    public class HpdfStreamExtensionsTests
    {
        [Fact]
        public void WriteChar_WritesCharacter()
        {
            var stream = new HpdfMemoryStream();

            stream.WriteChar('A');

            stream.ToArray().Should().Equal((byte)'A');
        }

        [Fact]
        public void WriteString_WritesAsciiString()
        {
            var stream = new HpdfMemoryStream();

            stream.WriteString("Hello");

            byte[] expected = Encoding.ASCII.GetBytes("Hello");
            stream.ToArray().Should().Equal(expected);
        }

        [Fact]
        public void WriteInt_WritesIntegerAsText()
        {
            var stream = new HpdfMemoryStream();

            stream.WriteInt(12345);

            byte[] expected = Encoding.ASCII.GetBytes("12345");
            stream.ToArray().Should().Equal(expected);
        }

        [Fact]
        public void WriteInt_NegativeNumber_WritesWithSign()
        {
            var stream = new HpdfMemoryStream();

            stream.WriteInt(-999);

            byte[] expected = Encoding.ASCII.GetBytes("-999");
            stream.ToArray().Should().Equal(expected);
        }

        [Fact]
        public void WriteUInt_WritesUnsignedInteger()
        {
            var stream = new HpdfMemoryStream();

            stream.WriteUInt(4294967295);

            byte[] expected = Encoding.ASCII.GetBytes("4294967295");
            stream.ToArray().Should().Equal(expected);
        }

        [Fact]
        public void WriteReal_WritesFloatAsText()
        {
            var stream = new HpdfMemoryStream();

            stream.WriteReal(3.14159f);

            string result = Encoding.ASCII.GetString(stream.ToArray());
            result.Should().Be("3.14159");
        }

        [Fact]
        public void WriteReal_RemovesTrailingZeros()
        {
            var stream = new HpdfMemoryStream();

            stream.WriteReal(5.0f);

            string result = Encoding.ASCII.GetString(stream.ToArray());
            result.Should().Be("5");
        }

        [Fact]
        public void WriteReal_HandlesSmallNumbers()
        {
            var stream = new HpdfMemoryStream();

            stream.WriteReal(0.123456f);

            string result = Encoding.ASCII.GetString(stream.ToArray());
            result.Should().StartWith("0.123");
        }

        [Fact]
        public void WriteLine_WritesStringWithNewline()
        {
            var stream = new HpdfMemoryStream();

            stream.WriteLine("Test");

            byte[] expected = Encoding.ASCII.GetBytes("Test\n");
            stream.ToArray().Should().Equal(expected);
        }

        [Fact]
        public void WriteLine_EmptyString_WritesOnlyNewline()
        {
            var stream = new HpdfMemoryStream();

            stream.WriteLine();

            stream.ToArray().Should().Equal((byte)'\n');
        }

        [Fact]
        public void WriteEscapedName_WritesSimpleName()
        {
            var stream = new HpdfMemoryStream();

            stream.WriteEscapedName("Font");

            byte[] expected = Encoding.ASCII.GetBytes("/Font");
            stream.ToArray().Should().Equal(expected);
        }

        [Fact]
        public void WriteEscapedName_EscapesSpaces()
        {
            var stream = new HpdfMemoryStream();

            stream.WriteEscapedName("My Font");

            byte[] expected = Encoding.ASCII.GetBytes("/My#20Font");
            stream.ToArray().Should().Equal(expected);
        }

        [Fact]
        public void WriteEscapedName_EscapesSpecialCharacters()
        {
            var stream = new HpdfMemoryStream();

            stream.WriteEscapedName("Font#1");

            string result = Encoding.ASCII.GetString(stream.ToArray());
            result.Should().Be("/Font#231");
        }

        [Fact]
        public void WriteEscapedText_WritesSimpleText()
        {
            var stream = new HpdfMemoryStream();

            stream.WriteEscapedText("Hello World");

            byte[] expected = Encoding.ASCII.GetBytes("(Hello World)");
            stream.ToArray().Should().Equal(expected);
        }

        [Fact]
        public void WriteEscapedText_EscapesParentheses()
        {
            var stream = new HpdfMemoryStream();

            stream.WriteEscapedText("(test)");

            byte[] expected = Encoding.ASCII.GetBytes(@"(\(test\))");
            stream.ToArray().Should().Equal(expected);
        }

        [Fact]
        public void WriteEscapedText_EscapesBackslash()
        {
            var stream = new HpdfMemoryStream();

            stream.WriteEscapedText(@"C:\path");

            byte[] expected = Encoding.ASCII.GetBytes(@"(C:\\path)");
            stream.ToArray().Should().Equal(expected);
        }

        [Fact]
        public void WriteEscapedText_EscapesNewlineAndReturn()
        {
            var stream = new HpdfMemoryStream();

            stream.WriteEscapedText("Line1\r\nLine2");

            byte[] expected = Encoding.ASCII.GetBytes(@"(Line1\r\nLine2)");
            stream.ToArray().Should().Equal(expected);
        }

        [Fact]
        public void WriteEscapedText_EscapesNonPrintableCharacters()
        {
            var stream = new HpdfMemoryStream();

            // Create string with control character using char cast to avoid any encoding issues
            string testString = "A" + (char)1 + "B";

            stream.WriteEscapedText(testString);

            byte[] result = stream.ToArray();
            // Expected: (A\001B)
            byte[] expected = { 0x28, 0x41, 0x5C, 0x30, 0x30, 0x31, 0x42, 0x29 };
            result.Should().Equal(expected);
        }

        [Fact]
        public void WriteHexString_WritesEmptyArray()
        {
            var stream = new HpdfMemoryStream();

            stream.WriteHexString(Array.Empty<byte>());

            byte[] expected = Encoding.ASCII.GetBytes("<>");
            stream.ToArray().Should().Equal(expected);
        }

        [Fact]
        public void WriteHexString_WritesHexData()
        {
            var stream = new HpdfMemoryStream();

            stream.WriteHexString(new byte[] { 0x12, 0xAB, 0xCD, 0xEF });

            byte[] expected = Encoding.ASCII.GetBytes("<12ABCDEF>");
            stream.ToArray().Should().Equal(expected);
        }

        [Fact]
        public void WriteHexString_HandlesZeroes()
        {
            var stream = new HpdfMemoryStream();

            stream.WriteHexString(new byte[] { 0x00, 0x0F, 0xF0 });

            byte[] expected = Encoding.ASCII.GetBytes("<000FF0>");
            stream.ToArray().Should().Equal(expected);
        }

        [Fact]
        public void ReadLine_ReadsSimpleLine()
        {
            var stream = new HpdfMemoryStream();
            stream.WriteString("Hello World\n");
            stream.Seek(0, SeekOrigin.Begin);

            string? line = stream.ReadLine();

            line.Should().Be("Hello World");
        }

        [Fact]
        public void ReadLine_HandlesCarriageReturn()
        {
            var stream = new HpdfMemoryStream();
            stream.WriteString("Line1\rLine2\n");
            stream.Seek(0, SeekOrigin.Begin);

            string? line1 = stream.ReadLine();
            string? line2 = stream.ReadLine();

            line1.Should().Be("Line1");
            line2.Should().Be("Line2");
        }

        [Fact]
        public void ReadLine_HandlesCarriageReturnLineFeed()
        {
            var stream = new HpdfMemoryStream();
            stream.WriteString("Line1\r\nLine2\n");
            stream.Seek(0, SeekOrigin.Begin);

            string? line1 = stream.ReadLine();
            string? line2 = stream.ReadLine();

            line1.Should().Be("Line1");
            line2.Should().Be("Line2");
        }

        [Fact]
        public void ReadLine_AtEndOfStream_ReturnsNull()
        {
            var stream = new HpdfMemoryStream();
            stream.WriteString("Test\n");
            stream.Seek(0, SeekOrigin.Begin);
            stream.ReadLine();

            string? line = stream.ReadLine();

            line.Should().BeNull();
        }

        [Fact]
        public void ReadLine_NoNewline_ReturnsPartialLine()
        {
            var stream = new HpdfMemoryStream();
            stream.WriteString("Incomplete");
            stream.Seek(0, SeekOrigin.Begin);

            string? line = stream.ReadLine();

            line.Should().Be("Incomplete");
        }

        [Fact]
        public void ReadLine_RespectsMaxLength()
        {
            using var stream = new HpdfMemoryStream();
            stream.WriteString("Very long line that exceeds maximum length\n");
            stream.Seek(0, SeekOrigin.Begin);

            string? line = stream.ReadLine(10);

            line!.Length.Should().Be(10);
        }

        [Theory]
        [InlineData(0, "0")]
        [InlineData(1, "1")]
        [InlineData(-1, "-1")]
        [InlineData(999999, "999999")]
        public void WriteInt_VariousValues_WritesCorrectly(int value, string expected)
        {
            var stream = new HpdfMemoryStream();

            stream.WriteInt(value);

            string result = Encoding.ASCII.GetString(stream.ToArray());
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData(0.0f, "0")]
        [InlineData(1.5f, "1.5")]
        [InlineData(-2.75f, "-2.75")]
        [InlineData(100.0f, "100")]
        public void WriteReal_VariousValues_WritesCorrectly(float value, string expected)
        {
            var stream = new HpdfMemoryStream();

            stream.WriteReal(value);

            string result = Encoding.ASCII.GetString(stream.ToArray());
            result.Should().Be(expected);
        }
    }
}
