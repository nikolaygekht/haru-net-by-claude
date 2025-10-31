/*
 * << Haru Free PDF Library >> -- PdfADemo.cs
 *
 * Demonstrates PDF/A (Archival PDF) creation for long-term document preservation
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
    public static class PdfADemo
    {
        public static void Run()
        {
            try
            {
                CreateStandardPdf();
                CreatePdfADocument();

                Console.WriteLine("PdfADemo completed successfully!");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Error in PdfADemo: {e.Message}");
                Console.Error.WriteLine($"Stack trace: {e.StackTrace}");
                throw;
            }
        }

        private static void CreateStandardPdf()
        {
            var pdf = new HpdfDocument();
            pdf.SetCompressionMode(HpdfCompressionMode.All);

            pdf.Info.Title = "Standard PDF Document";
            pdf.Info.Author = "Haru.NET";
            pdf.Info.Subject = "Regular PDF for comparison";

            var font = new HpdfFont(pdf.Xref, HpdfStandardFont.Helvetica, "F1");
            var boldFont = new HpdfFont(pdf.Xref, HpdfStandardFont.HelveticaBold, "F2");

            var page = pdf.AddPage();
            float height = page.Height;

            DrawLabel(page, boldFont, 50, height - 50, "Standard PDF Document", 24);
            DrawLabel(page, font, 50, height - 80, "This is a regular PDF without archival compliance", 12);

            float y = height - 130;

            DrawLabel(page, font, 50, y, "Standard PDF Characteristics:", 14);
            y -= 30;

            DrawLabel(page, font, 70, y, "- May depend on external resources (fonts, color profiles)", 11);
            y -= 20;
            DrawLabel(page, font, 70, y, "- Can include dynamic content (JavaScript, multimedia)", 11);
            y -= 20;
            DrawLabel(page, font, 70, y, "- Can be encrypted or password-protected", 11);
            y -= 20;
            DrawLabel(page, font, 70, y, "- May not guarantee long-term visual fidelity", 11);
            y -= 20;
            DrawLabel(page, font, 70, y, "- Suitable for everyday document exchange", 11);
            y -= 40;

            page.SetRgbStroke(0.7f, 0.7f, 0.7f);
            page.SetLineWidth(1);
            page.Rectangle(50, y - 100, 500, 90);
            page.Stroke();

            DrawLabel(page, font, 60, y - 25, "Use Standard PDF when:", 11);
            DrawLabel(page, font, 70, y - 43, "- Short-term document exchange (days to months)", 10);
            DrawLabel(page, font, 70, y - 58, "- Dynamic or interactive content is needed", 10);
            DrawLabel(page, font, 70, y - 73, "- Security features like encryption are required", 10);
            DrawLabel(page, font, 70, y - 88, "- File size optimization is more important than archival compliance", 10);

            pdf.SaveToFile("pdfs/PdfADemo_Standard.pdf");
            Console.WriteLine($"Standard PDF created (Version: {pdf.Version})");
        }

        private static void CreatePdfADocument()
        {
            var pdf = new HpdfDocument();
            pdf.SetCompressionMode(HpdfCompressionMode.All);

            pdf.Info.Title = "PDF/A-1b Archival Document";
            pdf.Info.Author = "Haru.NET Library";
            pdf.Info.Subject = "Long-term archival PDF demonstration";
            pdf.Info.Keywords = "PDF/A, archival, preservation, ISO 19005";
            pdf.Info.Creator = "Haru.NET PDF/A Demo";

            pdf.SetPdfACompliance("1B");

            var font = new HpdfFont(pdf.Xref, HpdfStandardFont.Helvetica, "F1");
            var boldFont = new HpdfFont(pdf.Xref, HpdfStandardFont.HelveticaBold, "F2");
            var italicFont = new HpdfFont(pdf.Xref, HpdfStandardFont.HelveticaOblique, "F3");

            var page = pdf.AddPage();
            float height = page.Height;
            float width = page.Width;

            DrawLabel(page, boldFont, 50, height - 50, "PDF/A-1b Archival Document", 24);
            DrawLabel(page, italicFont, 50, height - 75, "ISO 19005-1 Level B Compliant", 12, 0.3f, 0.3f, 0.7f);

            float y = height - 120;

            page.SetRgbFill(0.95f, 0.98f, 1.0f);
            page.Rectangle(50, y - 140, width - 100, 130);
            page.Fill();

            page.SetRgbStroke(0.3f, 0.5f, 0.8f);
            page.SetLineWidth(2);
            page.Rectangle(50, y - 140, width - 100, 130);
            page.Stroke();

            DrawLabel(page, boldFont, 60, y - 25, "What is PDF/A?", 14, 0.0f, 0.3f, 0.6f);
            y -= 40;

            DrawLabel(page, font, 70, y, "PDF/A is an ISO standard (ISO 19005) for electronic document preservation.", 10);
            y -= 18;
            DrawLabel(page, font, 70, y, "It ensures documents remain readable and visually consistent for decades,", 10);
            y -= 18;
            DrawLabel(page, font, 70, y, "regardless of future changes in software, hardware, or technology.", 10);
            y -= 30;

            DrawLabel(page, italicFont, 70, y, "Think of it as a 'time capsule' for documents - everything needed to display", 9, 0.4f, 0.4f, 0.4f);
            y -= 15;
            DrawLabel(page, italicFont, 70, y, "the document is embedded inside the file itself.", 9, 0.4f, 0.4f, 0.4f);

            y -= 50;

            DrawLabel(page, boldFont, 50, y, "PDF/A-1b Requirements:", 14);
            y -= 25;

            DrawLabel(page, font, 70, y, "+ All fonts must be embedded", 11, 0.0f, 0.6f, 0.0f);
            DrawLabel(page, font, 340, y, "- No external font dependencies", 11, 0.7f, 0.0f, 0.0f);
            y -= 22;

            DrawLabel(page, font, 70, y, "+ Color profiles embedded (sRGB)", 11, 0.0f, 0.6f, 0.0f);
            DrawLabel(page, font, 340, y, "- No device-dependent colors", 11, 0.7f, 0.0f, 0.0f);
            y -= 22;

            DrawLabel(page, font, 70, y, "+ XMP metadata included", 11, 0.0f, 0.6f, 0.0f);
            DrawLabel(page, font, 340, y, "- No encryption allowed", 11, 0.7f, 0.0f, 0.0f);
            y -= 22;

            DrawLabel(page, font, 70, y, "+ All content embedded", 11, 0.0f, 0.6f, 0.0f);
            DrawLabel(page, font, 340, y, "- No external references", 11, 0.7f, 0.0f, 0.0f);
            y -= 22;

            DrawLabel(page, font, 70, y, "+ PDF version 1.4 or higher", 11, 0.0f, 0.6f, 0.0f);
            DrawLabel(page, font, 340, y, "- No dynamic/multimedia content", 11, 0.7f, 0.0f, 0.0f);

            y -= 40;

            DrawLabel(page, boldFont, 50, y, "Real-World Use Cases:", 14);
            y -= 25;

            string[] useCases = new[]
            {
                "Legal Documents: Court filings, contracts, agreements that must be preserved exactly",
                "Government Records: Tax documents, permits, official records, regulatory filings",
                "Medical Records: Patient files that must be kept for 7+ years",
                "Academic Archives: Research papers, theses, dissertations for long-term access",
                "Corporate Compliance: Financial reports, audit documents, certified records",
                "Cultural Heritage: Museum catalogs, historical documents, digital preservation"
            };

            for (int i = 0; i < useCases.Length; i++)
            {
                DrawLabel(page, font, 70, y, $"- {useCases[i]}", 10);
                y -= 18;
            }

            y -= 20;

            page.SetRgbFill(1.0f, 0.98f, 0.9f);
            page.Rectangle(50, y - 90, width - 100, 80);
            page.Fill();

            page.SetRgbStroke(0.8f, 0.6f, 0.2f);
            page.SetLineWidth(2);
            page.Rectangle(50, y - 90, width - 100, 80);
            page.Stroke();

            DrawLabel(page, boldFont, 60, y - 23, "This Document's PDF/A Features:", 12, 0.6f, 0.4f, 0.0f);
            DrawLabel(page, font, 70, y - 41, "- Embedded standard fonts (Helvetica family)", 10);
            DrawLabel(page, font, 70, y - 56, "- sRGB color profile in Output Intent", 10);
            DrawLabel(page, font, 70, y - 71, "- XMP metadata with PDF/A identification", 10);
            DrawLabel(page, font, 70, y - 86, "- Document ID for unique identification", 10);

            pdf.SaveToFile("pdfs/PdfADemo_Archival.pdf");
            Console.WriteLine($"PDF/A-1b document created (Version: {pdf.Version})");
            Console.WriteLine($"PDF/A Compliant: {pdf.IsPdfACompliant}");
            Console.WriteLine();
            Console.WriteLine("How to verify PDF/A compliance:");
            Console.WriteLine("  • Adobe Acrobat: Document Properties > Description (shows PDF/A-1b badge)");
            Console.WriteLine("  • Most PDF viewers will display a PDF/A indicator");
            Console.WriteLine("  • Online validators: https://www.pdf-online.com/osa/validate.aspx");
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
