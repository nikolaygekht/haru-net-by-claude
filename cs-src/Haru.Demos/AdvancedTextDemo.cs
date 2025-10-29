/*
 * << Haru Free PDF Library >> -- AdvancedTextDemo.cs
 *
 * Demonstrates advanced text rendering and layout features
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
    public static class AdvancedTextDemo
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

                CreateRenderingModesPage(pdf, font, boldFont);
                CreateTextTransformationsPage(pdf, font, boldFont);
                CreateTextFeaturesPage(pdf, font, boldFont, italicFont);

                pdf.SaveToFile("pdfs/AdvancedTextDemo.pdf");
                Console.WriteLine("AdvancedTextDemo completed successfully!");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Error in AdvancedTextDemo: {e.Message}");
                Console.Error.WriteLine($"Stack trace: {e.StackTrace}");
                throw;
            }
        }

        private static void CreateRenderingModesPage(HpdfDocument pdf, HpdfFont font, HpdfFont boldFont)
        {
            var page = pdf.AddPage();
            float height = page.Height;
            float width = page.Width;

            DrawLabel(page, boldFont, 50, height - 50, "Text Rendering Modes", 24);
            DrawLabel(page, font, 50, height - 75, "Different ways to render text in PDFs", 12, 0.4f, 0.4f, 0.4f);

            float y = height - 130;

            DrawLabel(page, font, 50, y, "Text can be rendered in different modes beyond simple fill:", 11);
            y -= 30;

            page.SetFontAndSize(boldFont, 36);

            DrawLabel(page, boldFont, 70, y, "Fill (Default)", 13, 0.2f, 0.2f, 0.6f);
            y -= 25;
            page.SetRgbFill(0.2f, 0.2f, 0.2f);
            page.BeginText();
            page.SetTextRenderingMode(HpdfTextRenderingMode.Fill);
            page.MoveTextPos(70, y);
            page.ShowText("Fill Mode");
            page.EndText();
            DrawLabel(page, font, 70, y - 20, "Text filled with color", 9, 0.5f, 0.5f, 0.5f);

            y -= 70;

            DrawLabel(page, boldFont, 70, y, "Stroke (Outline)", 13, 0.2f, 0.2f, 0.6f);
            y -= 25;
            page.SetRgbStroke(0.8f, 0.2f, 0.2f);
            page.SetLineWidth(1.5f);
            page.BeginText();
            page.SetTextRenderingMode(HpdfTextRenderingMode.Stroke);
            page.MoveTextPos(70, y);
            page.ShowText("Stroke Mode");
            page.EndText();
            DrawLabel(page, font, 70, y - 20, "Text outlined with stroke color", 9, 0.5f, 0.5f, 0.5f);

            y -= 70;

            DrawLabel(page, boldFont, 70, y, "Fill + Stroke (Combined)", 13, 0.2f, 0.2f, 0.6f);
            y -= 25;
            page.SetRgbFill(1.0f, 0.8f, 0.0f);
            page.SetRgbStroke(0.6f, 0.3f, 0.0f);
            page.SetLineWidth(2.0f);
            page.BeginText();
            page.SetTextRenderingMode(HpdfTextRenderingMode.FillThenStroke);
            page.MoveTextPos(70, y);
            page.ShowText("Fill+Stroke");
            page.EndText();
            DrawLabel(page, font, 70, y - 20, "Text filled and outlined for emphasis", 9, 0.5f, 0.5f, 0.5f);

            y -= 70;

            DrawLabel(page, boldFont, 70, y, "Invisible (Hidden)", 13, 0.2f, 0.2f, 0.6f);
            y -= 25;
            page.BeginText();
            page.SetTextRenderingMode(HpdfTextRenderingMode.Invisible);
            page.MoveTextPos(70, y);
            page.ShowText("You can't see this!");
            page.EndText();
            DrawLabel(page, font, 70, y, "[Text exists but is invisible - useful for searchable overlay text]", 9, 0.5f, 0.5f, 0.5f);
            y -= 20;
            DrawLabel(page, font, 70, y, "Common use: Making scanned documents searchable without visible text", 9, 0.5f, 0.5f, 0.5f);

            y -= 60;

            DrawLabel(page, boldFont, 70, y, "Clipping Modes", 13, 0.2f, 0.2f, 0.6f);
            y -= 25;
            DrawLabel(page, font, 70, y, "Text can also be used as a clipping path for graphics:", 10);
            y -= 30;

            page.GSave();
            page.SetRgbFill(0.3f, 0.3f, 0.3f);
            page.SetFontAndSize(boldFont, 48);
            page.BeginText();
            page.SetTextRenderingMode(HpdfTextRenderingMode.FillClipping);
            page.MoveTextPos(70, y);
            page.ShowText("CLIPPED");
            page.EndText();

            for (int i = 0; i < 15; i++)
            {
                float hue = i / 15.0f;
                page.SetRgbFill(hue, 1.0f - hue, 0.5f);
                page.Rectangle(70, y - 10 + i * 4, 250, 4);
                page.Fill();
            }
            page.GRestore();
            DrawLabel(page, font, 70, y - 25, "Text acts as a 'window' to show graphics behind it", 9, 0.5f, 0.5f, 0.5f);

            y -= 70;

            page.SetRgbStroke(0.5f, 0.5f, 0.5f);
            page.SetLineWidth(1);
            page.Rectangle(50, y - 90, width - 100, 80);
            page.Stroke();

            DrawLabel(page, boldFont, 60, y - 20, "Rendering Mode Use Cases:", 11);
            DrawLabel(page, font, 70, y - 37, "- Stroke: Headlines, logos, decorative text", 10);
            DrawLabel(page, font, 70, y - 52, "- Fill+Stroke: Emphasis, titles with strong visual impact", 10);
            DrawLabel(page, font, 70, y - 67, "- Invisible: OCR text over scanned images, hidden searchable text", 10);
            DrawLabel(page, font, 70, y - 82, "- Clipping: Creative text effects with patterns/gradients", 10);
        }

        private static void CreateTextTransformationsPage(HpdfDocument pdf, HpdfFont font, HpdfFont boldFont)
        {
            var page = pdf.AddPage();
            float height = page.Height;
            float width = page.Width;

            DrawLabel(page, boldFont, 50, height - 50, "Text Transformations", 24);
            DrawLabel(page, font, 50, height - 75, "Rotating, scaling, and skewing text with transformation matrices", 12, 0.4f, 0.4f, 0.4f);

            float y = height - 130;

            DrawLabel(page, boldFont, 50, y, "Rotation", 14, 0.2f, 0.2f, 0.6f);
            y -= 30;

            float centerX = 150;
            float centerY = y - 60;

            for (int angle = 0; angle < 360; angle += 30)
            {
                double rad = angle * Math.PI / 180.0;
                float cos = (float)Math.Cos(rad);
                float sin = (float)Math.Sin(rad);

                page.SetRgbFill(angle / 360.0f, 1.0f - angle / 360.0f, 0.5f);
                page.SetFontAndSize(font, 12);
                page.BeginText();

                var matrix = new HpdfTransMatrix
                {
                    A = cos,
                    B = sin,
                    C = -sin,
                    D = cos,
                    X = centerX,
                    Y = centerY
                };
                page.SetTextMatrix(matrix);
                page.ShowText($"{angle}°");
                page.EndText();
            }

            DrawLabel(page, font, 50, centerY - 100, "Text rotated at different angles around a center point", 9, 0.5f, 0.5f, 0.5f);

            y = centerY - 130;

            DrawLabel(page, boldFont, 300, height - 160, "Scaling", 14, 0.2f, 0.2f, 0.6f);

            float scaleY = height - 200;
            float[] scales = { 0.5f, 0.75f, 1.0f, 1.25f, 1.5f };

            for (int i = 0; i < scales.Length; i++)
            {
                float scale = scales[i];
                page.SetRgbFill(0.2f, 0.2f, 0.2f);
                page.SetFontAndSize(boldFont, 18);
                page.BeginText();

                var matrix = new HpdfTransMatrix
                {
                    A = scale,
                    B = 0,
                    C = 0,
                    D = 1.0f,
                    X = 300,
                    Y = scaleY - i * 30
                };
                page.SetTextMatrix(matrix);
                page.ShowText($"Scale {scale:F2}x");
                page.EndText();
            }

            DrawLabel(page, font, 300, scaleY - 180, "Horizontal scaling (condensed/expanded text)", 9, 0.5f, 0.5f, 0.5f);

            DrawLabel(page, boldFont, 50, y, "Skewing (Italic/Oblique Effect)", 14, 0.2f, 0.2f, 0.6f);
            y -= 40;

            float[] skews = { 0.0f, 0.15f, 0.3f, 0.45f };
            string[] skewLabels = { "Normal", "Slight skew", "Medium skew", "Heavy skew" };

            for (int i = 0; i < skews.Length; i++)
            {
                page.SetRgbFill(0.2f, 0.2f, 0.2f);
                page.SetFontAndSize(font, 16);
                page.BeginText();

                var matrix = new HpdfTransMatrix
                {
                    A = 1.0f,
                    B = 0,
                    C = skews[i],
                    D = 1.0f,
                    X = 70,
                    Y = y - i * 30
                };
                page.SetTextMatrix(matrix);
                page.ShowText(skewLabels[i]);
                page.EndText();
            }

            y -= 150;

            DrawLabel(page, boldFont, 50, y, "Combined Transformations", 14, 0.2f, 0.2f, 0.6f);
            y -= 40;

            page.SetRgbFill(0.8f, 0.2f, 0.2f);
            page.SetFontAndSize(boldFont, 28);
            page.BeginText();

            double angle45 = Math.PI / 4;
            var combined = new HpdfTransMatrix
            {
                A = (float)Math.Cos(angle45) * 1.5f,
                B = (float)Math.Sin(angle45) * 1.5f,
                C = -(float)Math.Sin(angle45) * 1.5f + 0.2f,
                D = (float)Math.Cos(angle45) * 1.5f,
                X = 200,
                Y = y
            };
            page.SetTextMatrix(combined);
            page.ShowText("Rotated + Scaled + Skewed");
            page.EndText();

            DrawLabel(page, font, 50, y - 35, "Combine rotation, scaling, and skewing in one transformation matrix", 9, 0.5f, 0.5f, 0.5f);

            y -= 80;

            page.SetRgbStroke(0.5f, 0.5f, 0.5f);
            page.SetLineWidth(1);
            page.Rectangle(50, y - 80, width - 100, 70);
            page.Stroke();

            DrawLabel(page, boldFont, 60, y - 20, "Transformation Matrix Explanation:", 11);
            DrawLabel(page, font, 70, y - 37, "Matrix = [A B C D X Y] where:", 10);
            DrawLabel(page, font, 70, y - 52, "- A, D: scaling (width, height)  - B, C: rotation/skewing", 10);
            DrawLabel(page, font, 70, y - 67, "- X, Y: translation (position)", 10);
        }

        private static void CreateTextFeaturesPage(HpdfDocument pdf, HpdfFont font, HpdfFont boldFont, HpdfFont italicFont)
        {
            var page = pdf.AddPage();
            float height = page.Height;
            float width = page.Width;

            DrawLabel(page, boldFont, 50, height - 50, "Text Positioning & Spacing", 24);
            DrawLabel(page, font, 50, height - 75, "Fine control over text layout", 12, 0.4f, 0.4f, 0.4f);

            float y = height - 130;

            DrawLabel(page, boldFont, 50, y, "Text Rise (Superscript/Subscript)", 14, 0.2f, 0.2f, 0.6f);
            y -= 30;

            page.SetRgbFill(0.2f, 0.2f, 0.2f);
            page.SetFontAndSize(font, 14);
            page.BeginText();
            page.MoveTextPos(70, y);
            page.ShowText("E = mc");
            page.SetTextRise(7);
            page.SetFontAndSize(font, 10);
            page.ShowText("2");
            page.SetTextRise(0);
            page.SetFontAndSize(font, 14);
            page.ShowText("   (famous equation)");
            page.EndText();

            y -= 25;

            page.BeginText();
            page.MoveTextPos(70, y);
            page.SetFontAndSize(font, 14);
            page.ShowText("H");
            page.SetTextRise(-5);
            page.SetFontAndSize(font, 10);
            page.ShowText("2");
            page.SetTextRise(0);
            page.SetFontAndSize(font, 14);
            page.ShowText("O   (water)");
            page.EndText();

            DrawLabel(page, font, 70, y - 20, "SetTextRise() moves text up (positive) or down (negative)", 9, 0.5f, 0.5f, 0.5f);

            y -= 60;

            DrawLabel(page, boldFont, 50, y, "Character Spacing", 14, 0.2f, 0.2f, 0.6f);
            y -= 30;

            page.SetRgbFill(0.2f, 0.2f, 0.2f);
            page.SetFontAndSize(font, 14);
            page.BeginText();
            page.MoveTextPos(70, y);
            page.SetCharSpace(0);
            page.ShowText("Normal spacing");
            page.EndText();

            y -= 25;
            page.BeginText();
            page.MoveTextPos(70, y);
            page.SetCharSpace(2);
            page.ShowText("Expanded spacing");
            page.EndText();

            y -= 25;
            page.BeginText();
            page.MoveTextPos(70, y);
            page.SetCharSpace(5);
            page.ShowText("Wide spacing");
            page.EndText();

            page.SetCharSpace(0);

            DrawLabel(page, font, 70, y - 20, "SetCharSpace() adds extra space between characters", 9, 0.5f, 0.5f, 0.5f);

            y -= 60;

            DrawLabel(page, boldFont, 50, y, "Word Spacing", 14, 0.2f, 0.2f, 0.6f);
            y -= 30;

            page.SetFontAndSize(font, 14);
            page.BeginText();
            page.MoveTextPos(70, y);
            page.SetWordSpace(0);
            page.ShowText("The quick brown fox jumps");
            page.EndText();

            y -= 25;
            page.BeginText();
            page.MoveTextPos(70, y);
            page.SetWordSpace(10);
            page.ShowText("The quick brown fox jumps");
            page.EndText();

            y -= 25;
            page.BeginText();
            page.MoveTextPos(70, y);
            page.SetWordSpace(20);
            page.ShowText("The quick brown fox jumps");
            page.EndText();

            page.SetWordSpace(0);

            DrawLabel(page, font, 70, y - 20, "SetWordSpace() adds extra space between words", 9, 0.5f, 0.5f, 0.5f);

            y -= 60;

            DrawLabel(page, boldFont, 50, y, "Combining Text with Graphics", 14, 0.2f, 0.2f, 0.6f);
            y -= 35;

            float boxX = 70;
            float boxY = y - 80;

            page.SetRgbFill(1.0f, 0.95f, 0.85f);
            page.Rectangle(boxX, boxY, 200, 70);
            page.Fill();

            page.SetRgbStroke(0.8f, 0.4f, 0.1f);
            page.SetLineWidth(2);
            page.Rectangle(boxX, boxY, 200, 70);
            page.Stroke();

            page.SetRgbFill(0.8f, 0.3f, 0.0f);
            page.SetFontAndSize(boldFont, 18);
            page.BeginText();
            page.MoveTextPos(boxX + 15, boxY + 42);
            page.ShowText("IMPORTANT");
            page.EndText();

            page.SetRgbFill(0.2f, 0.2f, 0.2f);
            page.SetFontAndSize(font, 11);
            page.BeginText();
            page.MoveTextPos(boxX + 15, boxY + 20);
            page.ShowText("Text layered over");
            page.EndText();

            page.BeginText();
            page.MoveTextPos(boxX + 15, boxY + 8);
            page.ShowText("graphics elements");
            page.EndText();

            DrawLabel(page, font, boxX, boxY - 15, "Text positioned precisely within graphics using MoveTextPos()", 9, 0.5f, 0.5f, 0.5f);

            y = boxY - 50;

            page.SetRgbStroke(0.5f, 0.5f, 0.5f);
            page.SetLineWidth(1);
            page.Rectangle(50, y - 90, width - 100, 80);
            page.Stroke();

            DrawLabel(page, boldFont, 60, y - 20, "Key Text Positioning Methods:", 11);
            DrawLabel(page, font, 70, y - 37, "- MoveTextPos(x, y): Absolute positioning for precise text placement", 10);
            DrawLabel(page, font, 70, y - 52, "- SetTextLeading(leading): Line spacing for multi-line text", 10);
            DrawLabel(page, font, 70, y - 67, "- SetTextRise(rise): Vertical offset for super/subscripts", 10);
            DrawLabel(page, font, 70, y - 82, "- SetCharSpace/SetWordSpace: Fine-tune spacing for justified text", 10);
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
