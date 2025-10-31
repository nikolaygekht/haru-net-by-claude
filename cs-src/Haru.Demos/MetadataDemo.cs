/*
 * << Haru Free PDF Library >> -- MetadataDemo.cs
 *
 * Demonstrates document metadata and page background customization
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
    public static class MetadataDemo
    {
        public static void Run()
        {
            try
            {
                var pdf = new HpdfDocument();
                pdf.SetCompressionMode(HpdfCompressionMode.All);

                pdf.Info.Title = "Document Metadata Demo";
                pdf.Info.Author = "Haru.NET Development Team";
                pdf.Info.Subject = "Comprehensive guide to PDF metadata and document properties";
                pdf.Info.Keywords = "PDF, metadata, properties, document info, Haru";
                pdf.Info.Creator = "Haru.NET MetadataDemo Application";
                pdf.Info.SetCreationDate(DateTime.Now);
                pdf.Info.SetModificationDate(DateTime.Now);
                pdf.Info.Trapped = "False";

                pdf.Info.SetCustomMetadata("Company", "Acme Corporation");
                pdf.Info.SetCustomMetadata("Department", "Engineering");
                pdf.Info.SetCustomMetadata("Project", "PDF Library Demonstration");

                var font = new HpdfFont(pdf.Xref, HpdfStandardFont.Helvetica, "F1");
                var boldFont = new HpdfFont(pdf.Xref, HpdfStandardFont.HelveticaBold, "F2");
                var italicFont = new HpdfFont(pdf.Xref, HpdfStandardFont.HelveticaOblique, "F3");

                CreateMetadataPage(pdf, font, boldFont, italicFont);
                CreateCustomMetadataPage(pdf, font, boldFont);

                pdf.SaveToFile("pdfs/MetadataDemo.pdf");
                Console.WriteLine("MetadataDemo completed successfully!");
                Console.WriteLine();
                Console.WriteLine("View this document's metadata in your PDF viewer:");
                Console.WriteLine("  - Adobe Acrobat: File > Properties");
                Console.WriteLine("  - Most viewers: Document Properties or Document Information");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Error in MetadataDemo: {e.Message}");
                Console.Error.WriteLine($"Stack trace: {e.StackTrace}");
                throw;
            }
        }

        private static void CreateMetadataPage(HpdfDocument pdf, HpdfFont font, HpdfFont boldFont, HpdfFont italicFont)
        {
            var page = pdf.AddPage();
            float height = page.Height;
            float width = page.Width;

            DrawLabel(page, boldFont, 50, height - 50, "Document Metadata", 24);
            DrawLabel(page, italicFont, 50, height - 75, "Information stored in the PDF Info dictionary", 12, 0.4f, 0.4f, 0.4f);

            float y = height - 120;

            page.SetRgbFill(0.95f, 0.98f, 1.0f);
            page.Rectangle(50, y - 160, width - 100, 150);
            page.Fill();

            page.SetRgbStroke(0.3f, 0.5f, 0.8f);
            page.SetLineWidth(2);
            page.Rectangle(50, y - 160, width - 100, 150);
            page.Stroke();

            DrawLabel(page, boldFont, 60, y - 25, "What is PDF Metadata?", 14, 0.0f, 0.3f, 0.6f);
            y -= 43;

            DrawLabel(page, font, 70, y, "Metadata is descriptive information about the document stored in the PDF file.", 10);
            y -= 18;
            DrawLabel(page, font, 70, y, "This information is displayed in PDF viewers and helps with document management,", 10);
            y -= 18;
            DrawLabel(page, font, 70, y, "searchability, and organization.", 10);
            y -= 30;

            DrawLabel(page, italicFont, 70, y, "Right-click this PDF and select 'Properties' or 'Document Properties' to see", 9, 0.4f, 0.4f, 0.4f);
            y -= 15;
            DrawLabel(page, italicFont, 70, y, "the metadata values set by this demo.", 9, 0.4f, 0.4f, 0.4f);

            y -= 50;

            DrawLabel(page, boldFont, 50, y, "Standard Metadata Fields:", 14);
            y -= 25;

            DrawLabel(page, boldFont, 70, y, "Title:", 11, 0.2f, 0.2f, 0.6f);
            DrawLabel(page, font, 170, y, pdf.Info.Title ?? "(not set)", 10);
            y -= 20;

            DrawLabel(page, boldFont, 70, y, "Author:", 11, 0.2f, 0.2f, 0.6f);
            DrawLabel(page, font, 170, y, pdf.Info.Author ?? "(not set)", 10);
            y -= 20;

            DrawLabel(page, boldFont, 70, y, "Subject:", 11, 0.2f, 0.2f, 0.6f);
            DrawLabel(page, font, 170, y, pdf.Info.Subject ?? "(not set)", 10);
            y -= 20;

            DrawLabel(page, boldFont, 70, y, "Keywords:", 11, 0.2f, 0.2f, 0.6f);
            DrawLabel(page, font, 170, y, pdf.Info.Keywords ?? "(not set)", 10);
            y -= 20;

            DrawLabel(page, boldFont, 70, y, "Creator:", 11, 0.2f, 0.2f, 0.6f);
            DrawLabel(page, font, 170, y, pdf.Info.Creator ?? "(not set)", 10);
            y -= 20;

            DrawLabel(page, boldFont, 70, y, "Producer:", 11, 0.2f, 0.2f, 0.6f);
            DrawLabel(page, font, 170, y, pdf.Info.Producer ?? "(not set)", 10);
            y -= 20;

            DrawLabel(page, boldFont, 70, y, "Created:", 11, 0.2f, 0.2f, 0.6f);
            DrawLabel(page, font, 170, y, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 10);
            y -= 20;

            DrawLabel(page, boldFont, 70, y, "Trapped:", 11, 0.2f, 0.2f, 0.6f);
            DrawLabel(page, font, 170, y, pdf.Info.Trapped ?? "Not set", 10);
            y -= 40;

            DrawLabel(page, boldFont, 50, y, "Field Descriptions:", 14);
            y -= 25;

            string[] descriptions = new[]
            {
                "Title: The document's name or heading",
                "Author: Person or organization that created the document",
                "Subject: Brief description of the document's topic",
                "Keywords: Search terms for finding this document",
                "Creator: Application that created the original content",
                "Producer: Application that converted/produced the PDF",
                "Created/Modified: Timestamp when document was created or last modified",
                "Trapped: Indicates if trapping has been applied for printing"
            };

            foreach (var desc in descriptions)
            {
                DrawLabel(page, font, 70, y, desc, 10);
                y -= 18;
            }

            y -= 20;

            page.SetRgbStroke(0.5f, 0.5f, 0.5f);
            page.SetLineWidth(1);
            page.Rectangle(50, y - 70, width - 100, 60);
            page.Stroke();

            DrawLabel(page, boldFont, 60, y - 18, "Why Metadata Matters:", 11);
            DrawLabel(page, font, 70, y - 35, "- Improves searchability and discoverability of documents", 10);
            DrawLabel(page, font, 70, y - 50, "- Enables proper document management and organization", 10);
            DrawLabel(page, font, 70, y - 65, "- Provides context and attribution for legal/compliance purposes", 10);
        }

        private static void CreateCustomMetadataPage(HpdfDocument pdf, HpdfFont font, HpdfFont boldFont)
        {
            var page = pdf.AddPage();
            float height = page.Height;
            float width = page.Width;

            DrawLabel(page, boldFont, 50, height - 50, "Custom Metadata", 24);
            DrawLabel(page, font, 50, height - 75, "Extending metadata with custom fields", 12, 0.4f, 0.4f, 0.4f);

            float y = height - 130;

            DrawLabel(page, font, 50, y, "In addition to standard fields, you can add custom metadata fields", 11);
            y -= 18;
            DrawLabel(page, font, 50, y, "for application-specific information:", 11);
            y -= 40;

            DrawLabel(page, boldFont, 70, y, "Custom Metadata in This Document:", 13, 0.2f, 0.5f, 0.2f);
            y -= 30;

            DrawLabel(page, boldFont, 90, y, "Company:", 11, 0.2f, 0.2f, 0.6f);
            DrawLabel(page, font, 190, y, pdf.Info.GetCustomMetadata("Company") ?? "(not set)", 10);
            y -= 22;

            DrawLabel(page, boldFont, 90, y, "Department:", 11, 0.2f, 0.2f, 0.6f);
            DrawLabel(page, font, 190, y, pdf.Info.GetCustomMetadata("Department") ?? "(not set)", 10);
            y -= 22;

            DrawLabel(page, boldFont, 90, y, "Project:", 11, 0.2f, 0.2f, 0.6f);
            DrawLabel(page, font, 190, y, pdf.Info.GetCustomMetadata("Project") ?? "(not set)", 10);
            y -= 50;

            page.SetRgbFill(1.0f, 0.98f, 0.9f);
            page.Rectangle(50, y - 120, width - 100, 110);
            page.Fill();

            page.SetRgbStroke(0.8f, 0.6f, 0.2f);
            page.SetLineWidth(2);
            page.Rectangle(50, y - 120, width - 100, 110);
            page.Stroke();

            DrawLabel(page, boldFont, 60, y - 23, "Code Example:", 12, 0.6f, 0.4f, 0.0f);
            y -= 40;

            DrawLabel(page, font, 70, y, "pdf.Info.SetCustomMetadata(\"Company\", \"Acme Corporation\");", 9, 0.3f, 0.3f, 0.3f);
            y -= 18;
            DrawLabel(page, font, 70, y, "pdf.Info.SetCustomMetadata(\"Department\", \"Engineering\");", 9, 0.3f, 0.3f, 0.3f);
            y -= 18;
            DrawLabel(page, font, 70, y, "pdf.Info.SetCustomMetadata(\"Project\", \"PDF Library Demo\");", 9, 0.3f, 0.3f, 0.3f);
            y -= 30;

            DrawLabel(page, font, 70, y, "string company = pdf.Info.GetCustomMetadata(\"Company\");", 9, 0.3f, 0.3f, 0.3f);

            y -= 50;

            DrawLabel(page, boldFont, 50, y, "Common Use Cases for Custom Metadata:", 13);
            y -= 25;

            string[] useCases = new[]
            {
                "Document workflow tracking (status, approval, version)",
                "Business classifications (department, cost center, project code)",
                "Security and compliance (classification level, retention period)",
                "Document relationships (parent document, related files)",
                "Application-specific data (form ID, template version, data source)"
            };

            foreach (var useCase in useCases)
            {
                DrawLabel(page, font, 70, y, $"- {useCase}", 10);
                y -= 20;
            }

            y -= 30;

            page.SetRgbStroke(0.5f, 0.5f, 0.5f);
            page.SetLineWidth(1);
            page.Rectangle(50, y - 80, width - 100, 70);
            page.Stroke();

            DrawLabel(page, boldFont, 60, y - 20, "Best Practices:", 11);
            DrawLabel(page, font, 70, y - 37, "- Use descriptive, consistent key names", 10);
            DrawLabel(page, font, 70, y - 52, "- Store values as strings (API requirement)", 10);
            DrawLabel(page, font, 70, y - 67, "- Document your custom metadata schema for team/organization use", 10);
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
