/*
 * << Haru Free PDF Library >> -- CompressionDemo.cs
 *
 * Demonstrates different PDF compression modes for file size optimization
 *
 * Copyright (c) 1999-2025 Haru Free PDF Library
 *
 * Permission to use, copy, modify, distribute and sell this software
 * and its documentation for any purpose is hereby granted without fee,
 */

using Haru.Doc;
using Haru.Font;
using Haru.Types;

namespace BasicDemos
{
    public static class CompressionDemo
    {
        public static void Run()
        {
            try
            {
                var pdf = new HpdfDocument();
                pdf.SetCompressionMode(HpdfCompressionMode.All);

                var font = new HpdfFont(pdf.Xref, HpdfStandardFont.Helvetica, "F1");
                var boldFont = new HpdfFont(pdf.Xref, HpdfStandardFont.HelveticaBold, "F2");
                var italicFont = new HpdfFont(pdf.Xref, HpdfStandardFont.HelveticaOblique, "F3");
                var monoFont = new HpdfFont(pdf.Xref, HpdfStandardFont.Courier, "F4");

                CreateCompressionIntroPage(pdf, font, boldFont, italicFont);
                CreateCompressionModesPage(pdf, font, boldFont, monoFont);

                pdf.SaveToFile("pdfs/CompressionDemo.pdf");
                Console.WriteLine("CompressionDemo completed successfully!");
                Console.WriteLine();
                Console.WriteLine("This demo shows PDF compression modes:");
                Console.WriteLine("  - None: No compression (largest files)");
                Console.WriteLine("  - Text: Compress text and graphics");
                Console.WriteLine("  - Image: Compress embedded images");
                Console.WriteLine("  - Metadata: Compress metadata streams");
                Console.WriteLine("  - All: Compress everything (smallest files)");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Error in CompressionDemo: {e.Message}");
                Console.Error.WriteLine($"Stack trace: {e.StackTrace}");
                throw;
            }
        }

        private static void CreateCompressionIntroPage(HpdfDocument pdf, HpdfFont font, HpdfFont boldFont, HpdfFont italicFont)
        {
            var page = pdf.AddPage();
            float height = page.Height;
            float width = page.Width;

            DrawLabel(page, boldFont, 50, height - 50, "PDF Compression Modes", 24);
            DrawLabel(page, italicFont, 50, height - 75, "Optimizing file sizes with intelligent compression", 12, 0.4f, 0.4f, 0.4f);

            float y = height - 200;

            page.SetRgbFill(0.95f, 0.98f, 1.0f);
            page.Rectangle(50, y - 130, width - 100, 200);
            page.Fill();

            page.SetRgbStroke(0.3f, 0.5f, 0.8f);
            page.SetLineWidth(2);
            page.Rectangle(50, y - 130, width - 100, 200);
            page.Stroke();

            DrawLabel(page, boldFont, 60, y + 43, "What Is PDF Compression?", 14, 0.0f, 0.3f, 0.6f);
            y += 23;

            DrawLabel(page, font, 70, y, "PDF compression reduces file sizes by encoding data more efficiently using", 10);
            y -= 18;
            DrawLabel(page, font, 70, y, "the Flate algorithm (similar to ZIP). Haru.NET provides granular control over", 10);
            y -= 18;
            DrawLabel(page, font, 70, y, "what gets compressed, allowing you to balance file size and generation speed.", 10);
            y -= 30;

            DrawLabel(page, boldFont, 70, y, "Benefits of Compression:", 11);
            y -= 20;
            DrawLabel(page, font, 80, y, "- Smaller file sizes (often 50-90% reduction)", 10);
            y -= 18;
            DrawLabel(page, font, 80, y, "- Faster file transfers and downloads", 10);
            y -= 18;
            DrawLabel(page, font, 80, y, "- Reduced storage requirements", 10);
            y -= 18;
            DrawLabel(page, font, 80, y, "- Standard PDF feature (supported by all viewers)", 10);

            y -= 50;

            DrawLabel(page, boldFont, 50, y, "How It Works", 14, 0.2f, 0.2f, 0.6f);
            y -= 25;
            DrawLabel(page, font, 50, y, "Haru.NET uses FlateDecode compression (deflate algorithm from System.IO.Compression).", 11);
            y -= 25;

            page.SetRgbFill(1.0f, 0.98f, 0.9f);
            page.Rectangle(70, y - 112, width - 140, 110);
            page.Fill();

            page.SetRgbStroke(0.8f, 0.6f, 0.2f);
            page.SetLineWidth(1);
            page.Rectangle(70, y - 112, width - 140, 110);
            page.Stroke();

            DrawLabel(page, boldFont, 80, y - 20, "Compression Process:", 11);
            y -= 40;

            DrawLabel(page, font, 90, y, "1. Uncompressed Data: 'Hello World Hello World Hello World...'", 9);
            y -= 18;
            DrawLabel(page, font, 90, y, "2. Flate Compression: Finds repeated patterns and encodes efficiently", 9);
            y -= 18;
            DrawLabel(page, font, 90, y, "3. Compressed Stream: Binary data much smaller than original", 9);
            y -= 18;
            DrawLabel(page, font, 90, y, "4. PDF Viewer: Automatically decompresses when reading the PDF", 9);

            y -= 50;

            DrawLabel(page, boldFont, 50, y, "Compression Is Lossless", 14, 0.2f, 0.2f, 0.6f);
            y -= 25;
            DrawLabel(page, font, 50, y, "All compression in Haru.NET is lossless - no data is lost during compression.", 11);
            y -= 18;
            DrawLabel(page, font, 50, y, "Images, text, and metadata are perfectly preserved and appear identical to users.", 11);
        }

        private static void CreateCompressionModesPage(HpdfDocument pdf, HpdfFont font, HpdfFont boldFont, HpdfFont monoFont)
        {
            var page = pdf.AddPage();
            float height = page.Height;
            float width = page.Width;

            DrawLabel(page, boldFont, 50, height - 50, "Compression Modes", 24);
            DrawLabel(page, font, 50, height - 75, "Five modes to control what gets compressed", 12, 0.4f, 0.4f, 0.4f);

            float y = height - 130;

            // Mode 1: None
            page.SetRgbFill(0.98f, 0.95f, 0.95f);
            page.Rectangle(50, y - 67, width - 100, 70);
            page.Fill();

            page.SetRgbStroke(0.8f, 0.3f, 0.3f);
            page.SetLineWidth(2);
            page.Rectangle(50, y - 67, width - 100, 70);
            page.Stroke();

            DrawLabel(page, boldFont, 60, y - 20, "None - No Compression", 13, 0.6f, 0.0f, 0.0f);
            y -= 37;
            DrawLabel(page, font, 70, y, "Nothing is compressed. Largest file sizes but fastest generation.", 10);
            y -= 18;
            DrawLabel(page, monoFont, 70, y, "pdf.SetCompressionMode(HpdfCompressionMode.None);", 9, 0.3f, 0.3f, 0.3f);

            y -= 55;

            // Mode 2: Text
            page.SetRgbFill(0.95f, 0.98f, 0.95f);
            page.Rectangle(50, y - 67, width - 100, 70);
            page.Fill();

            page.SetRgbStroke(0.3f, 0.7f, 0.3f);
            page.SetLineWidth(2);
            page.Rectangle(50, y - 67, width - 100, 70);
            page.Stroke();

            DrawLabel(page, boldFont, 60, y - 20, "Text - Compress Text & Graphics", 13, 0.0f, 0.5f, 0.0f);
            y -= 37;
            DrawLabel(page, font, 70, y, "Compresses text streams and vector graphics. Best for text-heavy documents.", 10);
            y -= 18;
            DrawLabel(page, monoFont, 70, y, "pdf.SetCompressionMode(HpdfCompressionMode.Text);", 9, 0.3f, 0.3f, 0.3f);

            y -= 55;

            // Mode 3: Image
            page.SetRgbFill(0.95f, 0.95f, 0.98f);
            page.Rectangle(50, y - 67, width - 100, 70);
            page.Fill();

            page.SetRgbStroke(0.3f, 0.3f, 0.8f);
            page.SetLineWidth(2);
            page.Rectangle(50, y - 67, width - 100, 70);
            page.Stroke();

            DrawLabel(page, boldFont, 60, y - 20, "Image - Compress Images", 13, 0.0f, 0.0f, 0.6f);
            y -= 37;
            DrawLabel(page, font, 70, y, "Compresses embedded image data. Best for image-heavy documents.", 10);
            y -= 18;
            DrawLabel(page, monoFont, 70, y, "pdf.SetCompressionMode(HpdfCompressionMode.Image);", 9, 0.3f, 0.3f, 0.3f);

            y -= 55;

            // Mode 4: Metadata
            page.SetRgbFill(0.98f, 0.95f, 0.98f);
            page.Rectangle(50, y - 67, width - 100, 70);
            page.Fill();

            page.SetRgbStroke(0.7f, 0.3f, 0.7f);
            page.SetLineWidth(2);
            page.Rectangle(50, y - 67, width - 100, 70);
            page.Stroke();

            DrawLabel(page, boldFont, 60, y - 20, "Metadata - Compress Metadata Streams", 13, 0.5f, 0.0f, 0.5f);
            y -= 37;
            DrawLabel(page, font, 70, y, "Compresses metadata streams (XMP, etc.). Rarely used alone.", 10);
            y -= 18;
            DrawLabel(page, monoFont, 70, y, "pdf.SetCompressionMode(HpdfCompressionMode.Metadata);", 9, 0.3f, 0.3f, 0.3f);

            y -= 55;

            // Mode 5: All
            page.SetRgbFill(0.95f, 0.98f, 1.0f);
            page.Rectangle(50, y - 97, width - 100, 100);
            page.Fill();

            page.SetRgbStroke(0.2f, 0.5f, 0.9f);
            page.SetLineWidth(3);
            page.Rectangle(50, y - 97, width - 100, 100);
            page.Stroke();

            DrawLabel(page, boldFont, 60, y - 22, "All - Compress Everything (RECOMMENDED)", 13, 0.0f, 0.3f, 0.8f);
            y -= 40;
            DrawLabel(page, font, 70, y, "Compresses all content: text, images, and metadata. Smallest file sizes.", 10);
            y -= 18;
            DrawLabel(page, font, 70, y, "This is the default mode used in most Haru.NET demos.", 10);
            y -= 20;
            DrawLabel(page, monoFont, 70, y, "pdf.SetCompressionMode(HpdfCompressionMode.All);", 9, 0.3f, 0.3f, 0.3f);
        }

        private static void DrawLabel(HpdfPage page, HpdfFont font, float x, float y, string text,
            float size = 11, float r = 0, float g = 0, float b = 0)
        {
            page.SetFontAndSize(font, size);
            page.SetRgbFill(r, g, b);
            page.BeginText();
            page.MoveTextPos(x, y);
            page.ShowText(text);
            page.EndText();
        }
    }
}
