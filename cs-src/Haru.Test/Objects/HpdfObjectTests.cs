using System.Text;
using FluentAssertions;
using Haru.Objects;
using Haru.Streams;
using Xunit;

namespace Haru.Test.Objects
{
    public class HpdfObjectTests
    {
        private static string WriteToString(HpdfObject obj, bool useWrite = false)
        {
            var stream = new HpdfMemoryStream();
            if (useWrite)
                obj.Write(stream);
            else
                obj.WriteValue(stream);
            return Encoding.ASCII.GetString(stream.ToArray());
        }

        [Fact]
        public void Object_DirectByDefault()
        {
            var obj = new HpdfNumber(1);

            obj.IsDirect.Should().BeTrue();
            obj.IsIndirect.Should().BeFalse();
        }

        [Fact]
        public void Object_IndirectObject_WritesWithWrapper()
        {
            var obj = new HpdfNumber(42)
            {
                ObjectId = 1,
                GenerationNumber = 0,
                ObjectType = HpdfObjectType.Indirect
            };

            string result = WriteToString(obj, useWrite: true);

            result.Should().Contain("1 0 obj");
            result.Should().Contain("42");
            result.Should().Contain("endobj");
        }

        [Fact]
        public void Object_DirectObject_WritesValueOnly()
        {
            var obj = new HpdfNumber(42)
            {
                ObjectType = HpdfObjectType.Direct
            };

            string result = WriteToString(obj, useWrite: true);

            result.Should().Be("42");
            result.Should().NotContain("obj");
            result.Should().NotContain("endobj");
        }

        [Fact]
        public void Object_RealObjectId_MasksFlags()
        {
            var obj = new HpdfNumber(1)
            {
                ObjectId = 0x80000042 // Has direct flag set
            };

            obj.RealObjectId.Should().Be(0x42);
        }

        [Fact]
        public void Object_ObjectClass_ReturnsCorrectType()
        {
            var nullObj = new HpdfNull();
            var boolObj = new HpdfBoolean(true);
            var numObj = new HpdfNumber(1);
            var realObj = new HpdfReal(1.5f);
            var nameObj = new HpdfName("Test");
            var strObj = new HpdfString("Test");
            var binObj = new HpdfBinary(new byte[] { 1 });
            var arrObj = new HpdfArray();
            var dictObj = new HpdfDict();

            nullObj.ObjectClass.Should().Be(HpdfObjectClass.Null);
            boolObj.ObjectClass.Should().Be(HpdfObjectClass.Boolean);
            numObj.ObjectClass.Should().Be(HpdfObjectClass.Number);
            realObj.ObjectClass.Should().Be(HpdfObjectClass.Real);
            nameObj.ObjectClass.Should().Be(HpdfObjectClass.Name);
            strObj.ObjectClass.Should().Be(HpdfObjectClass.String);
            binObj.ObjectClass.Should().Be(HpdfObjectClass.Binary);
            arrObj.ObjectClass.Should().Be(HpdfObjectClass.Array);
            dictObj.ObjectClass.Should().Be(HpdfObjectClass.Dict);
        }

        [Fact]
        public void Object_Hidden_FlagWorks()
        {
            var obj = new HpdfNumber(1)
            {
                ObjectType = HpdfObjectType.Hidden
            };

            obj.IsHidden.Should().BeTrue();
        }

        [Fact]
        public void Object_ComplexDocument_WritesCorrectly()
        {
            // Create a dictionary representing a simple PDF page
            var page = new HpdfDict
            {
                { "Type", new HpdfName("Page") },
                { "MediaBox", new HpdfArray
                    {
                        new HpdfNumber(0),
                        new HpdfNumber(0),
                        new HpdfNumber(612),
                        new HpdfNumber(792)
                    }
                },
                { "Resources", new HpdfDict
                    {
                        { "Font", new HpdfDict
                            {
                                { "F1", new HpdfName("Helvetica") }
                            }
                        }
                    }
                }
            };

            string result = WriteToString(page);

            result.Should().Contain("/Type /Page");
            result.Should().Contain("/MediaBox");
            result.Should().Contain("[0 0 612 792]");
            result.Should().Contain("/Resources");
            result.Should().Contain("/Font");
            result.Should().Contain("/F1 /Helvetica");
        }
    }
}
