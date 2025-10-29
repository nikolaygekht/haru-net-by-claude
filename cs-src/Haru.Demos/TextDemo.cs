/*
 * << Haru Free PDF Library >> -- TextDemo.cs
 *
 * Adapted from original C demo to use the new C# Haru library
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
    public static class TextDemo
    {
        private static void ShowStripePattern(HpdfPage page, HpdfFont font, float x, float y)
        {
            int iy = 0;

            while (iy < 50)
            {
                page.SetRgbStroke(0.0f, 0.0f, 0.5f);
                page.SetLineWidth(1);
                page.MoveTo(x, y + iy);
                page.LineTo(x + font.MeasureText("ABCabc123", page.GraphicsState.FontSize), y + iy);
                page.Stroke();
                iy += 3;
            }

            page.SetLineWidth(2.5f);
        }

        private static void ShowDescription(HpdfPage page, HpdfFont font, float x, float y, string text)
        {
            float fsize = page.GraphicsState.FontSize;
            var c = page.GraphicsState.RgbFill;

            page.BeginText();
            page.SetRgbFill(0, 0, 0);
            page.SetTextRenderingMode(HpdfTextRenderingMode.Fill);
            page.SetFontAndSize(font, 10);
            page.MoveTextPos(x, y - 12);
            page.ShowText(text);
            page.EndText();

            page.SetFontAndSize(font, fsize);
            page.SetRgbFill(c.R, c.G, c.B);
        }

        public static void Run()
        {
            const string pageTitle = "Text Demo";
            const string sampText = "abcdefgABCDEFG123!#$%&+-@?";
            const string sampText2 = "The quick brown fox jumps over the lazy dog.";

            try
            {
                var pdf = new HpdfDocument();

                // Create default-font
                var font = new HpdfFont(pdf.Xref, HpdfStandardFont.Helvetica, "F1");

                // Add a new page object
                var page = pdf.AddPage();

                // Print the title of the page (with positioning center)
                page.SetFontAndSize(font, 24);
                float tw = font.MeasureText(pageTitle, 24);
                page.BeginText();
                page.MoveTextPos((page.Width - tw) / 2, page.Height - 50);
                page.ShowText(pageTitle);
                page.EndText();

                page.BeginText();
                page.MoveTextPos(60, page.Height - 60);

                // Font size
                float fsize = 8;
                while (fsize < 60)
                {
                    // Set style and size of font
                    page.SetFontAndSize(font, fsize);

                    // Set the position of the text
                    page.MoveTextPos(0, -5 - fsize);

                    // Measure the number of characters which included in the page
                    float availWidth = page.Width - 120;
                    string displayText = sampText;
                    float textWidth = font.MeasureText(sampText, fsize);
                    if (textWidth > availWidth)
                    {
                        // Simple truncation - in real implementation would use MeasureText properly
                        displayText = sampText.Substring(0, Math.Max(1, (int)(sampText.Length * availWidth / textWidth)));
                    }

                    page.ShowText(displayText);

                    // Print the description
                    page.MoveTextPos(0, -10);
                    page.SetFontAndSize(font, 8);
                    page.ShowText($"Fontsize={fsize}");

                    fsize *= 1.5f;
                }

                // Font color
                page.SetFontAndSize(font, 8);
                page.MoveTextPos(0, -30);
                page.ShowText("Font color");

                page.SetFontAndSize(font, 18);
                page.MoveTextPos(0, -20);
                int len = sampText.Length;

                for (int i = 0; i < len; i++)
                {
                    float r = (float)i / (float)len;
                    float g = 1 - ((float)i / (float)len);
                    string buf = sampText.Substring(i, 1);

                    page.SetRgbFill(r, g, 0);
                    page.ShowText(buf);
                }
                page.MoveTextPos(0, -25);

                for (int i = 0; i < len; i++)
                {
                    float r = (float)i / (float)len;
                    float b = 1 - ((float)i / (float)len);
                    string buf = sampText.Substring(i, 1);

                    page.SetRgbFill(r, 0, b);
                    page.ShowText(buf);
                }
                page.MoveTextPos(0, -25);

                for (int i = 0; i < len; i++)
                {
                    float b = (float)i / (float)len;
                    float g = 1 - ((float)i / (float)len);
                    string buf = sampText.Substring(i, 1);

                    page.SetRgbFill(0, g, b);
                    page.ShowText(buf);
                }

                page.EndText();

                int ypos = 450;

                // Font rendering mode
                page.SetFontAndSize(font, 32);
                page.SetRgbFill(0.5f, 0.5f, 0);
                page.SetLineWidth(1.5f);

                // PDF_FILL
                ShowDescription(page, font, 60, ypos, "RenderingMode=PDF_FILL");
                page.SetTextRenderingMode(HpdfTextRenderingMode.Fill);
                page.BeginText();
                page.MoveTextPos(60, ypos);
                page.ShowText("ABCabc123");
                page.EndText();

                // PDF_STROKE
                ShowDescription(page, font, 60, ypos - 50, "RenderingMode=PDF_STROKE");
                page.SetTextRenderingMode(HpdfTextRenderingMode.Stroke);
                page.BeginText();
                page.MoveTextPos(60, ypos - 50);
                page.ShowText("ABCabc123");
                page.EndText();

                // PDF_FILL_THEN_STROKE
                ShowDescription(page, font, 60, ypos - 100, "RenderingMode=PDF_FILL_THEN_STROKE");
                page.SetTextRenderingMode(HpdfTextRenderingMode.FillThenStroke);
                page.BeginText();
                page.MoveTextPos(60, ypos - 100);
                page.ShowText("ABCabc123");
                page.EndText();

                // PDF_FILL_CLIPPING
                ShowDescription(page, font, 60, ypos - 150, "RenderingMode=PDF_FILL_CLIPPING");
                page.GSave();
                page.SetTextRenderingMode(HpdfTextRenderingMode.FillClipping);
                page.BeginText();
                page.MoveTextPos(60, ypos - 150);
                page.ShowText("ABCabc123");
                page.EndText();
                ShowStripePattern(page, font, 60, ypos - 150);
                page.GRestore();

                // PDF_STROKE_CLIPPING
                ShowDescription(page, font, 60, ypos - 200, "RenderingMode=PDF_STROKE_CLIPPING");
                page.GSave();
                page.SetTextRenderingMode(HpdfTextRenderingMode.StrokeClipping);
                page.BeginText();
                page.MoveTextPos(60, ypos - 200);
                page.ShowText("ABCabc123");
                page.EndText();
                ShowStripePattern(page, font, 60, ypos - 200);
                page.GRestore();

                // PDF_FILL_STROKE_CLIPPING
                ShowDescription(page, font, 60, ypos - 250, "RenderingMode=PDF_FILL_STROKE_CLIPPING");
                page.GSave();
                page.SetTextRenderingMode(HpdfTextRenderingMode.FillStrokeClipping);
                page.BeginText();
                page.MoveTextPos(60, ypos - 250);
                page.ShowText("ABCabc123");
                page.EndText();
                ShowStripePattern(page, font, 60, ypos - 250);
                page.GRestore();

                // Reset text attributes
                page.SetTextRenderingMode(HpdfTextRenderingMode.Fill);
                page.SetRgbFill(0, 0, 0);
                page.SetFontAndSize(font, 30);

                // Rotating text
                float angle1 = 30;                   // A rotation of 30 degrees
                float rad1 = angle1 / 180 * 3.141592f; // Calculate the radian value

                ShowDescription(page, font, 320, ypos - 60, "Rotating text");
                page.BeginText();
                page.SetTextMatrix((float)Math.Cos(rad1), (float)Math.Sin(rad1),
                    -(float)Math.Sin(rad1), (float)Math.Cos(rad1),
                    330, ypos - 60);
                page.ShowText("ABCabc123");
                page.EndText();

                // Skewing text
                ShowDescription(page, font, 320, ypos - 120, "Skewing text");
                page.BeginText();

                angle1 = 10;
                float angle2 = 20;
                rad1 = angle1 / 180 * 3.141592f;
                float rad2 = angle2 / 180 * 3.141592f;

                page.SetTextMatrix(1, (float)Math.Tan(rad1), (float)Math.Tan(rad2), 1,
                        320, ypos - 120);
                page.ShowText("ABCabc123");
                page.EndText();

                // Scaling text (X direction)
                ShowDescription(page, font, 320, ypos - 175, "Scaling text (X direction)");
                page.BeginText();
                page.SetTextMatrix(1.5f, 0, 0, 1, 320, ypos - 175);
                page.ShowText("ABCabc12");
                page.EndText();

                // Scaling text (Y direction)
                ShowDescription(page, font, 320, ypos - 250, "Scaling text (Y direction)");
                page.BeginText();
                page.SetTextMatrix(1, 0, 0, 2, 320, ypos - 250);
                page.ShowText("ABCabc123");
                page.EndText();

                // Char spacing, word spacing
                ShowDescription(page, font, 60, 140, "char-spacing 0");
                ShowDescription(page, font, 60, 100, "char-spacing 1.5");
                ShowDescription(page, font, 60, 60, "char-spacing 1.5, word-spacing 2.5");

                page.SetFontAndSize(font, 20);
                page.SetRgbFill(0.1f, 0.3f, 0.1f);

                // char-spacing 0
                page.BeginText();
                page.MoveTextPos(60, 140);
                page.ShowText(sampText2);
                page.EndText();

                // char-spacing 1.5
                page.SetCharSpace(1.5f);

                page.BeginText();
                page.MoveTextPos(60, 100);
                page.ShowText(sampText2);
                page.EndText();

                // char-spacing 1.5, word-spacing 3.5
                page.SetWordSpace(2.5f);

                page.BeginText();
                page.MoveTextPos(60, 60);
                page.ShowText(sampText2);
                page.EndText();

                // Save the document to a file
                pdf.SaveToFile("pdfs/TextDemo.pdf");

            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Error in TextDemo: {e.Message}");
                throw;
            }
        }
    }
}
