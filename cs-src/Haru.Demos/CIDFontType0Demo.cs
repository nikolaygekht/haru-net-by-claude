/*
 * << Haru Free PDF Library >> -- CIDFontType0Demo.cs
 *
 * POC Demo for CIDFontType0 (predefined CJK fonts)
 */

using Haru.Doc;
using Haru.Font;
using Haru.Font.CID;

namespace Haru.Demos
{
    /// <summary>
    /// Demonstrates CIDFontType0 (predefined CJK fonts without font embedding).
    /// This is a POC with SimSun font only.
    /// </summary>
    public static class CIDFontType0Demo
    {
        public static void Run(string outputPath)
        {
            Console.WriteLine("=== CIDFontType0 POC Demo ===");
            Console.WriteLine("Testing predefined SimSun font (Chinese Simplified)");
            Console.WriteLine();

            try
            {
                // Create PDF document
                var doc = new HpdfDocument();

                // Test 1: Create SimSun using JSON resource with GBKEucHEncoder
                Console.WriteLine("Test 1: Loading SimSun from JSON resource with GBKEucHEncoder...");
                HpdfFont font;
                try
                {
                    var encoder = new GBKEucHEncoder();
                    var fontObj = HpdfCIDFontType0.Create(doc, "SimSun", "F1", 936, "GBK-EUC-H", encoder);
                    font = fontObj.AsFont();
                    Console.WriteLine("  SUCCESS: SimSun loaded with GBKEucHEncoder");
                    Console.WriteLine($"  Encoder: {encoder.Name} (CodePage: {encoder.CodePage})");
                    Console.WriteLine($"  CID System: {encoder.Registry}-{encoder.Ordering}-{encoder.Supplement}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  FAILED: {ex.Message}");
                    Console.WriteLine("  Falling back to hardcoded SimSun...");
                    var fontObj = HpdfCIDFontType0.CreateSimSun(doc, "F1");
                    font = fontObj.AsFont();
                }

                // Add a page
                var page = doc.AddPage();
                page.SetSize(HpdfPageSize.A4, HpdfPageDirection.Portrait);

                float pageWidth = page.Width;
                float pageHeight = page.Height;

                // Draw title
                page.BeginText();
                page.SetFontAndSize(font, 24);
                page.MoveTextPos(50, pageHeight - 100);

                // Test with Chinese text: "你好世界" (Hello World)
                string chineseText = "你好世界";
                Console.WriteLine($"Rendering text: {chineseText}");

                page.ShowText(chineseText);
                page.EndText();

                // Draw some ASCII text for comparison
                page.BeginText();
                page.SetFontAndSize(font, 14);
                page.MoveTextPos(50, pageHeight - 150);
                page.ShowText("CIDFontType0 POC - SimSun Font");
                page.EndText();

                // Add info
                page.BeginText();
                page.SetFontAndSize(font, 10);
                page.MoveTextPos(50, pageHeight - 200);
                page.ShowText("Font: SimSun (JSON + GBKEucHEncoder)");
                page.EndText();

                page.BeginText();
                page.SetFontAndSize(font, 10);
                page.MoveTextPos(50, pageHeight - 220);
                page.ShowText("Type: CIDFontType0 (No embedding)");
                page.EndText();

                page.BeginText();
                page.SetFontAndSize(font, 10);
                page.MoveTextPos(50, pageHeight - 240);
                page.ShowText("Encoding: GBK-EUC-H");
                page.EndText();

                // Save PDF
                var directory = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                using (var stream = File.Create(outputPath))
                {
                    doc.Save(stream);
                }

                Console.WriteLine($"\nPDF saved to: {outputPath}");
                Console.WriteLine("\nPDF Info:");
                Console.WriteLine($"  Version: {doc.Version}");
                Console.WriteLine($"  Pages: 1");
                Console.WriteLine($"  Font Type: CIDFontType0");
                Console.WriteLine($"  Font Name: SimSun");
                Console.WriteLine($"  Encoding: GBK-EUC-H");
                Console.WriteLine($"  File Size: {new FileInfo(outputPath).Length} bytes");

                Console.WriteLine("\nNOTE: This PDF requires SimSun font to be installed on the viewer's system.");
                Console.WriteLine("      If the font is not available, a substitute font will be used.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }
    }
}
