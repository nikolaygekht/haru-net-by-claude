/*
 * << Haru Free PDF Library >> -- PageLabelAndBoundaryDemo.cs
 *
 * Demonstrates page labels (custom page numbering) and page boundaries
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
    public static class PageLabelAndBoundaryDemo
    {
        private static void DrawBoundaryBox(HpdfPage page, HpdfBox box, float r, float g, float b, string label)
        {
            // Draw colored rectangle showing the boundary box
            page.GSave();
            page.SetRgbStroke(r, g, b);
            page.SetLineWidth(2);
            page.Rectangle(box.Left, box.Bottom, box.Width, box.Height);
            page.Stroke();

            // Add label
            page.BeginText();
            page.MoveTextPos(box.Left + 10, box.Top - 20);
            page.ShowText(label);
            page.EndText();

            page.GRestore();
        }

        public static void Run()
        {
            try
            {
                var pdf = new HpdfDocument();

                // Create font for all pages
                var font = new HpdfFont(pdf.Xref, HpdfStandardFont.Helvetica, "F1");
                var boldFont = new HpdfFont(pdf.Xref, HpdfStandardFont.HelveticaBold, "F2");

                // ====== SECTION 1: PAGE LABELS ======
                Console.WriteLine("Creating pages with custom labels...");

                // Front matter pages (lowercase Roman numerals: i, ii, iii)
                pdf.AddPageLabel(0, HpdfPageNumStyle.LowerRoman, 1);

                var titlePage = pdf.AddPage();
                titlePage.SetFontAndSize(boldFont, 24);
                titlePage.BeginText();
                titlePage.MoveTextPos(200, 700);
                titlePage.ShowText("Page Label Demo");
                titlePage.EndText();

                titlePage.SetFontAndSize(font, 14);
                titlePage.BeginText();
                titlePage.MoveTextPos(150, 650);
                titlePage.ShowText("This page should be labeled 'i' (Roman numeral)");
                titlePage.EndText();

                var copyrightPage = pdf.AddPage();
                copyrightPage.SetFontAndSize(font, 12);
                copyrightPage.BeginText();
                copyrightPage.MoveTextPos(100, 700);
                copyrightPage.ShowText("Copyright Page");
                copyrightPage.EndText();
                copyrightPage.MoveTextPos(0, -20);
                copyrightPage.ShowText("This page should be labeled 'ii' (Roman numeral)");
                copyrightPage.EndText();

                var tocPage = pdf.AddPage();
                tocPage.SetFontAndSize(boldFont, 16);
                tocPage.BeginText();
                tocPage.MoveTextPos(100, 700);
                tocPage.ShowText("Table of Contents");
                tocPage.EndText();

                tocPage.SetFontAndSize(font, 12);
                tocPage.MoveTextPos(0, -30);
                tocPage.ShowText("This page should be labeled 'iii' (Roman numeral)");
                tocPage.EndText();

                // Main content pages (decimal: 1, 2, 3)
                pdf.AddPageLabel(3, HpdfPageNumStyle.Decimal, 1);

                for (int i = 1; i <= 3; i++)
                {
                    var page = pdf.AddPage();
                    page.SetFontAndSize(boldFont, 18);
                    page.BeginText();
                    page.MoveTextPos(100, 700);
                    page.ShowText($"Chapter {i}");
                    page.EndText();

                    page.SetFontAndSize(font, 12);
                    page.MoveTextPos(0, -30);
                    page.ShowText($"This page should be labeled '{i}' (decimal)");
                    page.EndText();
                }

                // Appendix pages (prefix + decimal: A-1, A-2, A-3)
                pdf.AddPageLabel(6, HpdfPageNumStyle.Decimal, 1, "A-");

                for (int i = 1; i <= 3; i++)
                {
                    var page = pdf.AddPage();
                    page.SetFontAndSize(boldFont, 18);
                    page.BeginText();
                    page.MoveTextPos(100, 700);
                    page.ShowText($"Appendix {i}");
                    page.EndText();

                    page.SetFontAndSize(font, 12);
                    page.MoveTextPos(0, -30);
                    page.ShowText($"This page should be labeled 'A-{i}' (prefix + decimal)");
                    page.EndText();
                }

                // ====== SECTION 2: PAGE BOUNDARIES ======
                Console.WriteLine("Creating page with boundary boxes...");

                // Page demonstrating all boundary boxes
                var boundaryPage = pdf.AddPage();
                boundaryPage.SetFontAndSize(boldFont, 20);

                // Set up boundary boxes (in points, Letter size = 612×792)
                // MediaBox: 612×792 pt (Letter size - full sheet) - already set by default
                var bleedBox = new HpdfBox(9, 9, 603, 783);     // 9pt bleed on all sides
                var trimBox = new HpdfBox(18, 18, 594, 774);    // Final trimmed size
                var artBox = new HpdfBox(54, 54, 558, 738);     // Content area with margins

                boundaryPage.SetBleedBox(bleedBox);
                boundaryPage.SetTrimBox(trimBox);
                boundaryPage.SetArtBox(artBox);

                // Draw title
                boundaryPage.BeginText();
                boundaryPage.MoveTextPos(200, 750);
                boundaryPage.ShowText("Page Boundaries Demo");
                boundaryPage.EndText();

                boundaryPage.SetFontAndSize(font, 10);

                // Draw and label each boundary box
                var mediaBox = new HpdfBox(0, 0, 612, 792);  // Default MediaBox
                DrawBoundaryBox(boundaryPage, mediaBox, 1.0f, 0.0f, 0.0f, "MediaBox (Red) - Full page");
                DrawBoundaryBox(boundaryPage, bleedBox, 0.0f, 0.0f, 1.0f, "BleedBox (Blue) - Print bleed");
                DrawBoundaryBox(boundaryPage, trimBox, 0.0f, 0.8f, 0.0f, "TrimBox (Green) - Trim size");
                DrawBoundaryBox(boundaryPage, artBox, 1.0f, 0.5f, 0.0f, "ArtBox (Orange) - Content area");

                // Add explanation text
                boundaryPage.BeginText();
                boundaryPage.MoveTextPos(80, 400);
                boundaryPage.ShowText("Boundary Box Hierarchy:");
                boundaryPage.MoveTextPos(0, -20);
                boundaryPage.ShowText("MediaBox >= CropBox >= BleedBox >= TrimBox >= ArtBox");

                boundaryPage.MoveTextPos(0, -30);
                boundaryPage.ShowText("MediaBox: Full physical page");
                boundaryPage.MoveTextPos(0, -15);
                boundaryPage.ShowText("BleedBox: Clipping for production (printing)");
                boundaryPage.MoveTextPos(0, -15);
                boundaryPage.ShowText("TrimBox: Final page dimensions after trimming");
                boundaryPage.MoveTextPos(0, -15);
                boundaryPage.ShowText("ArtBox: Meaningful content area");
                boundaryPage.EndText();

                // Page with CropBox demonstration
                Console.WriteLine("Creating page with CropBox...");
                var cropPage = pdf.AddPage();

                // Set CropBox to show only part of the page
                var cropBox = new HpdfBox(100, 100, 512, 692);
                cropPage.SetCropBox(cropBox);

                cropPage.SetFontAndSize(boldFont, 18);
                cropPage.BeginText();
                cropPage.MoveTextPos(200, 700);
                cropPage.ShowText("CropBox Demo");
                cropPage.EndText();

                cropPage.SetFontAndSize(font, 12);
                cropPage.MoveTextPos(0, -30);
                cropPage.ShowText("This page has a CropBox set.");
                cropPage.MoveTextPos(0, -15);
                cropPage.ShowText("Content outside CropBox is clipped.");
                cropPage.EndText();

                // Draw something outside the CropBox (will be clipped)
                cropPage.SetRgbFill(0.9f, 0.9f, 0.9f);
                cropPage.Rectangle(0, 0, 100, 792);  // Left margin - outside CropBox
                cropPage.Fill();

                // Draw the CropBox boundary
                cropPage.SetRgbStroke(1.0f, 0.0f, 1.0f);
                cropPage.SetLineWidth(2);
                cropPage.Rectangle(cropBox.Left, cropBox.Bottom, cropBox.Width, cropBox.Height);
                cropPage.Stroke();

                // Save the document
                pdf.SaveToFile("pdfs/PageLabelAndBoundaryDemo.pdf");

                Console.WriteLine("PDF created successfully: pdfs/PageLabelAndBoundaryDemo.pdf");
                Console.WriteLine("");
                Console.WriteLine("Page Labels:");
                Console.WriteLine("  Pages 1-3: Roman numerals (i, ii, iii)");
                Console.WriteLine("  Pages 4-6: Decimal (1, 2, 3)");
                Console.WriteLine("  Pages 7-9: Prefix + Decimal (A-1, A-2, A-3)");
                Console.WriteLine("");
                Console.WriteLine("Page Boundaries:");
                Console.WriteLine("  Page 10: All boundary boxes visualized");
                Console.WriteLine("  Page 11: CropBox demonstration");
                Console.WriteLine("");
                Console.WriteLine("Open the PDF in a viewer (like Adobe Acrobat) to see the custom page labels!");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Error in PageLabelAndBoundaryDemo: {e.Message}");
                Console.Error.WriteLine($"Stack trace: {e.StackTrace}");
                throw;
            }
        }
    }
}
