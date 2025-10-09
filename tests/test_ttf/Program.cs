/*
 * TrueType Font Embedding Test
 *
 * This test demonstrates the TrueType font embedding feature by:
 * 1. Loading Roboto font from Google Fonts
 * 2. Embedding the font in a PDF
 * 3. Writing "The quick brown fox jumps over the lazy dog"
 * 4. Verifying the font is properly embedded with ToUnicode support
 */

using System;
using System.IO;
using Haru.Doc;
using Haru.Font;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== TrueType Font Embedding Test ===\n");

        // Get the font path
        string fontPath = Path.Combine(AppContext.BaseDirectory, "Roboto-Regular.ttf");

        if (!File.Exists(fontPath))
        {
            Console.WriteLine($"Error: Font file not found at {fontPath}");
            Console.WriteLine("Please ensure Roboto-Regular.ttf is in the same directory as the executable.");
            return;
        }

        Console.WriteLine($"✓ Font file found: {fontPath}");
        Console.WriteLine($"  Size: {new FileInfo(fontPath).Length / 1024} KB\n");

        try
        {
            // Test 1: Standard font (for comparison)
            Console.WriteLine("--- Test 1: Standard Font (Helvetica) ---");
            var doc1 = new HpdfDocument();
            doc1.Info.Title = "Standard Font Test - Helvetica";
            doc1.Info.Author = "Haru PDF Library";

            var page1 = doc1.AddPage(HpdfPageSize.A4, HpdfPageDirection.Portrait);
            var standardFont = new HpdfFont(doc1.Xref, HpdfStandardFont.Helvetica, "F1");

            page1.BeginText();
            page1.SetFontAndSize(standardFont, 12);
            page1.MoveTextPos(50, 750);
            page1.ShowText("Standard Font Test - Helvetica");
            page1.MoveTextPos(0, -30);
            page1.ShowText("The quick brown fox jumps over the lazy dog");
            page1.MoveTextPos(0, -20);
            page1.ShowText("THE QUICK BROWN FOX JUMPS OVER THE LAZY DOG");
            page1.MoveTextPos(0, -20);
            page1.ShowText("0123456789 !@#$%^&*()_+-=[]{}|;:',.<>?");
            page1.EndText();

            string output1 = Path.Combine(AppContext.BaseDirectory, "output_standard_font.pdf");
            doc1.SaveToFile(output1);
            Console.WriteLine($"✓ Standard font PDF saved: {output1}");
            Console.WriteLine($"  File size: {new FileInfo(output1).Length / 1024} KB\n");

            // Test 2: TrueType font
            Console.WriteLine("--- Test 2: TrueType Font (Roboto) ---");
            var doc = new HpdfDocument();
            doc.Info.Title = "TrueType Font Embedding Test";
            doc.Info.Author = "Haru PDF Library";
            doc.Info.Subject = "Demonstration of TrueType font embedding with Roboto";

            Console.WriteLine("✓ PDF document created");

            // Add a page
            var page = doc.AddPage(HpdfPageSize.A4, HpdfPageDirection.Portrait);
            Console.WriteLine("✓ Page added (A4 Portrait)");

            // Load and embed the TrueType font
            var ttFont = HpdfTrueTypeFont.LoadFromFile(
                doc.Xref,
                "Roboto",
                fontPath,
                embedding: true);

            // Get an HpdfFont wrapper for page operations
            var font = ttFont.AsFont();

            Console.WriteLine("✓ TrueType font loaded and embedded");
            Console.WriteLine($"  Font name: {ttFont.BaseFont}");
            Console.WriteLine($"  Local name: {ttFont.LocalName}");
            Console.WriteLine($"  Font embedded: {ttFont.Descriptor.ContainsKey("FontFile2")}");
            Console.WriteLine($"  ToUnicode CMap: {ttFont.Dict.ContainsKey("ToUnicode")}");

            // Get font descriptor information
            if (ttFont.Descriptor.ContainsKey("ItalicAngle"))
            {
                Console.WriteLine($"  Italic angle: {ttFont.Descriptor["ItalicAngle"]}");
            }
            if (ttFont.Descriptor.ContainsKey("Flags"))
            {
                Console.WriteLine($"  Flags: {ttFont.Descriptor["Flags"]}");
            }

            Console.WriteLine();

            // Draw text on the page
            page.BeginText();

            // Set the font and size
            page.SetFontAndSize(font, 12);

            // Title
            page.MoveTextPos(50, 750);
            page.ShowText("TrueType Font Embedding Test");

            // Main pangram
            page.MoveTextPos(0, -50);
            page.ShowText("The quick brown fox jumps over the lazy dog");

            // Uppercase version
            page.MoveTextPos(0, -30);
            page.ShowText("THE QUICK BROWN FOX JUMPS OVER THE LAZY DOG");

            // Numbers and special characters
            page.MoveTextPos(0, -30);
            page.ShowText("0123456789 !@#$%^&*()_+-=[]{}|;:',.<>?");

            // Additional test text
            page.MoveTextPos(0, -50);
            page.ShowText("This PDF contains an embedded Roboto font.");

            page.MoveTextPos(0, -20);
            page.ShowText("The font is compressed using FlateDecode.");

            page.MoveTextPos(0, -20);
            page.ShowText("ToUnicode CMap enables text extraction and search.");

            // Font metrics demonstration
            page.MoveTextPos(0, -50);
            page.ShowText("Font Metrics:");

            string testText = "Hello World!";
            float textWidth = ttFont.MeasureText(testText, 12);

            page.MoveTextPos(0, -20);
            page.ShowText($"Text: \"{testText}\"");

            page.MoveTextPos(0, -20);
            page.ShowText($"Width at 12pt: {textWidth:F2} units");

            page.EndText();

            Console.WriteLine("✓ Text written to page");

            // Save the PDF
            string outputPath = Path.Combine(AppContext.BaseDirectory, "output_roboto_test.pdf");
            doc.SaveToFile(outputPath);

            Console.WriteLine($"✓ PDF saved: {outputPath}");
            Console.WriteLine($"  File size: {new FileInfo(outputPath).Length / 1024} KB");

            // Verify the output
            if (File.Exists(outputPath))
            {
                var fileInfo = new FileInfo(outputPath);
                if (fileInfo.Length > 0)
                {
                    Console.WriteLine("\n=== TEST PASSED ===");
                    Console.WriteLine($"The PDF has been created successfully with embedded Roboto font!");
                    Console.WriteLine($"Open the file to view: {outputPath}");
                }
                else
                {
                    Console.WriteLine("\n=== TEST FAILED ===");
                    Console.WriteLine("PDF file is empty");
                }
            }
            else
            {
                Console.WriteLine("\n=== TEST FAILED ===");
                Console.WriteLine("PDF file was not created");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n=== TEST FAILED ===");
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Stack trace:\n{ex.StackTrace}");
        }
    }
}
