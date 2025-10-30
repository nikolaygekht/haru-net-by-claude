using System.Text;
using FluentAssertions;
using Haru.Objects;
using Haru.Streams;
using Xunit;


namespace Haru.Test.Objects
{
    public class HpdfStringTests
    {
        private static string WriteToString(HpdfObject obj)
        {
            using var stream = new HpdfMemoryStream();
            obj.WriteValue(stream);
            return Encoding.ASCII.GetString(stream.ToArray());
        }

        [Fact]
        public void String_FromText_WritesAsLiteral()
        {
            var obj = new HpdfString("Hello World");

            string result = WriteToString(obj);

            result.Should().Be("(Hello World)");
        }

        [Fact]
        public void String_WithEscapes_EscapesCorrectly()
        {
            var obj = new HpdfString("Test (value)");

            string result = WriteToString(obj);

            result.Should().Be(@"(Test \(value\))");
        }

        [Fact]
        public void String_AsHex_WritesHexFormat()
        {
            var obj = new HpdfString(new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F })
            {
                WriteAsHex = true
            };

            string result = WriteToString(obj);

            result.Should().Be("<48656C6C6F>");
        }

        [Fact]
        public void String_FromBytes_StoresBytes()
        {
            byte[] data = { 0x01, 0x02, 0x03 };
            var obj = new HpdfString(data);

            obj.Value.Should().Equal(data);
        }

        [Fact]
        public void String_GetText_ReturnsText()
        {
            var obj = new HpdfString("Test");

            string text = obj.GetText();

            text.Should().Be("Test");
        }

        [Fact]
        public void Binary_WritesAsHex()
        {
            var obj = new HpdfBinary(new byte[] { 0xAB, 0xCD, 0xEF });

            string result = WriteToString(obj);

            result.Should().Be("<ABCDEF>");
        }

        [Fact]
        public void Binary_Length_ReturnsCorrectValue()
        {
            var obj = new HpdfBinary(new byte[] { 1, 2, 3, 4, 5 });

            obj.Length.Should().Be(5);
        }
    }
}
