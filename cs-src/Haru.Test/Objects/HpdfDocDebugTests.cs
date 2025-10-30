using System;
using Xunit;
using Xunit.Abstractions;
using Haru.Doc;

#pragma warning disable CA2000 // Dispose objects before losing scope

namespace Haru.Test.Objects
{
    public class HpdfDocDebugTests
    {
        private readonly ITestOutputHelper _output;

        public HpdfDocDebugTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void DebugDocumentOutput()
        {
            // Arrange
            var doc = new HpdfDocument();
            doc.AddPage();

            // Act
            var pdfBytes = doc.SaveToMemory();
            var pdfText = System.Text.Encoding.ASCII.GetString(pdfBytes);

            // Output for debugging
            _output.WriteLine("=== PDF OUTPUT ===");
            _output.WriteLine(pdfText);
            _output.WriteLine("=== END OUTPUT ===");
            _output.WriteLine($"Length: {pdfText.Length}");
            _output.WriteLine($"Last 40 chars: '{pdfText.Substring(Math.Max(0, pdfText.Length - 40))}'");
            _output.WriteLine($"Ends with %%EOF: {pdfText.EndsWith("%%EOF")}");
            _output.WriteLine($"Ends with %%EOF\\n: {pdfText.EndsWith("%%EOF\n")}");
        }
    }
}
