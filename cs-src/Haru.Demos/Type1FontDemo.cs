/*
 * Type 1 Font Demo
 * Demonstrates Type 1 font loading and rendering with code page support
 */

using System;
using System.IO;
using Haru.Doc;
using Haru.Font;

namespace BasicDemos
{
    public static class Type1FontDemo
    {
        public static void Run()
        {
            Console.WriteLine("Running Type 1 Font Demo...");

            var pdf = new HpdfDocument();

            // Get the Resource path
            string afmPath = "demo/Type1/a010013l.afm";
            string pfbPath = "demo/Type1/a010013l.pfb";

            if (!File.Exists(afmPath))
            {
                Console.WriteLine($"AFM file not found: {afmPath}");
                return;
            }

            if (!File.Exists(pfbPath))
            {
                Console.WriteLine($"PFB file not found: {pfbPath}");
                return;
            }

            try
            {
                // Add a page
                var page = pdf.AddPage();

                // Load Type 1 font with Western encoding (CP1252)
                var westernFont = HpdfType1Font.LoadFromFile(pdf.Xref, "Type1Western", afmPath, pfbPath, 1252);

                // Load Type 1 font with Cyrillic encoding (CP1251)
                var cyrillicFont = HpdfType1Font.LoadFromFile(pdf.Xref, "Type1Cyrillic", afmPath, pfbPath, 1251);

                // Title
                page.BeginText();
                page.SetFontAndSize(westernFont.AsFont(), 24);
                page.MoveTextPos(50, 800);
                page.ShowText("Type 1 Font Demo");
                page.EndText();

                // Western text
                page.BeginText();
                page.SetFontAndSize(westernFont.AsFont(), 16);
                page.MoveTextPos(50, 750);
                page.ShowText("Western (CP1252): Hello, World!");
                page.EndText();

                page.BeginText();
                page.SetFontAndSize(westernFont.AsFont(), 14);
                page.MoveTextPos(50, 720);
                page.ShowText("French: Salut! Ça va?");
                page.EndText();

                page.BeginText();
                page.SetFontAndSize(westernFont.AsFont(), 14);
                page.MoveTextPos(50, 690);
                page.ShowText("German: Grüße! Schön!");
                page.EndText();

                // Cyrillic text
                page.BeginText();
                page.SetFontAndSize(cyrillicFont.AsFont(), 16);
                page.MoveTextPos(50, 640);
                page.ShowText("Cyrillic (CP1251):");
                page.EndText();

                page.BeginText();
                page.SetFontAndSize(cyrillicFont.AsFont(), 14);
                page.MoveTextPos(50, 610);
                // Russian "Hello" - Привет
                page.ShowText("Russian: Привет!");
                page.EndText();

                // Font information
                page.BeginText();
                page.SetFontAndSize(westernFont.AsFont(), 12);
                page.MoveTextPos(50, 550);
                page.ShowText("Font: URW Gothic L Book (Type 1)");
                page.EndText();

                page.BeginText();
                page.SetFontAndSize(westernFont.AsFont(), 12);
                page.MoveTextPos(50, 530);
                page.ShowText("Format: AFM + PFB with custom code pages");
                page.EndText();

                page.BeginText();
                page.SetFontAndSize(westernFont.AsFont(), 12);
                page.MoveTextPos(50, 510);
                page.ShowText("Encoding: Custom with Differences array");
                page.EndText();

                page.BeginText();
                page.SetFontAndSize(westernFont.AsFont(), 12);
                page.MoveTextPos(50, 490);
                page.ShowText("ToUnicode: Enabled for text extraction");
                page.EndText();

                // Save the PDF
                string outputPath = "pdfs/Type1FontDemo.pdf";
                pdf.SaveToFile(outputPath);
                Console.WriteLine($"PDF saved to: {Path.GetFullPath(outputPath)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
    }
}
