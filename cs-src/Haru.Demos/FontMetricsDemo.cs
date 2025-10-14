/*
 * << Haru Free PDF Library >> -- FontMetricsDemo.cs
 *
 * Demonstrates font metrics functionality (GetAscent, GetDescent, GetXHeight, GetBBox)
 * and text measurement with and without word wrapping for all font types.
 *
 * Copyright (c) 1999-2025 Haru Free PDF Library
 *
 * Permission to use, copy, modify, distribute and sell this software
 * and its documentation for any purpose is hereby granted without fee,
 */

using System;
using System.IO;
using Haru.Doc;
using Haru.Font;
using Haru.Font.CID;
using Haru.Types;

namespace BasicDemos
{
    public static class FontMetricsDemo
    {
        private const float LEFT_MARGIN = 60;
        private const float RIGHT_MARGIN = 60;

        /// <summary>
        /// Draws a text sample with measurement rectangle and metrics visualization.
        /// </summary>
        private static float DrawTextSample(
            HpdfPage page,
            HpdfFont font,
            HpdfFont labelFont,
            string text,
            float fontSize,
            float yPos,
            string label,
            bool showWordWrap = false)
        {
            float pageWidth = page.Width;
            float maxWidth = pageWidth - LEFT_MARGIN - RIGHT_MARGIN;

            // Draw label first (before calculating text position)
            page.BeginText();
            page.SetFontAndSize(labelFont, 10);
            page.MoveTextPos(LEFT_MARGIN, yPos);
            page.ShowText(label);
            page.EndText();

            // Get font metrics for the ACTUAL font being measured (not the label font)
            int ascent = font.GetAscent();
            int descent = font.GetDescent();
            int xHeight = font.GetXHeight();

            // Scale metrics to user space using the ACTUAL fontSize
            float ascentScaled = ascent * fontSize / 1000f;
            float descentScaled = descent * fontSize / 1000f;
            float xHeightScaled = xHeight * fontSize / 1000f;

            // Measure text
            float textWidth = font.MeasureText(text, fontSize);

            // Calculate text position (20pt below the label)
            float textY = yPos - 20;
            float textX = LEFT_MARGIN;

            // Draw baseline
            page.SetRgbStroke(0.7f, 0.7f, 0.7f);
            page.SetLineWidth(0.5f);
            page.MoveTo(textX, textY);
            page.LineTo(textX + textWidth, textY);
            page.Stroke();

            // Draw ascent line (blue)
            page.SetRgbStroke(0.0f, 0.0f, 0.8f);
            page.SetLineWidth(0.3f);
            page.MoveTo(textX, textY + ascentScaled);
            page.LineTo(textX + textWidth, textY + ascentScaled);
            page.Stroke();

            // Draw descent line (red)
            page.SetRgbStroke(0.8f, 0.0f, 0.0f);
            page.MoveTo(textX, textY + descentScaled);
            page.LineTo(textX + textWidth, textY + descentScaled);
            page.Stroke();

            // Draw x-height line (green)
            page.SetRgbStroke(0.0f, 0.8f, 0.0f);
            page.MoveTo(textX, textY + xHeightScaled);
            page.LineTo(textX + textWidth, textY + xHeightScaled);
            page.Stroke();

            // Draw bounding rectangle around text
            page.SetRgbStroke(0.0f, 0.0f, 0.0f);
            page.SetLineWidth(0.5f);
            page.Rectangle(textX, textY + descentScaled, textWidth, ascentScaled - descentScaled);
            page.Stroke();

            // Draw the text
            page.BeginText();
            page.SetFontAndSize(font, fontSize);
            page.MoveTextPos(textX, textY);
            page.ShowText(text);
            page.EndText();

            // Draw metrics info
            page.BeginText();
            page.SetFontAndSize(labelFont, 8);
            page.MoveTextPos(textX, textY + descentScaled - 12);
            page.ShowText($"Width: {textWidth:F1}pt  Ascent: {ascentScaled:F1}pt  Descent: {descentScaled:F1}pt  X-Height: {xHeightScaled:F1}pt");
            page.EndText();

            // Return next Y position using font metrics
            return textY + descentScaled - 25;
        }

        /// <summary>
        /// Draws a section header.
        /// </summary>
        private static float DrawSectionHeader(HpdfPage page, HpdfFont font, string title, float yPos)
        {
            page.SetRgbFill(0.2f, 0.2f, 0.2f);
            page.BeginText();
            page.SetFontAndSize(font, 14);
            page.MoveTextPos(LEFT_MARGIN, yPos);
            page.ShowText(title);
            page.EndText();

            page.SetRgbFill(0.0f, 0.0f, 0.0f);

            // Use font metrics to calculate spacing
            int ascent = font.GetAscent();
            int descent = font.GetDescent();
            float lineHeight = (ascent - descent) * 14 / 1000f;

            return yPos - lineHeight - 10;
        }

        public static void Run()
        {
            try
            {
                Console.WriteLine("Creating Font Metrics Demo...");

                var pdf = new HpdfDocument();
                pdf.SetCompressionMode(HpdfCompressionMode.All);

                var page = pdf.AddPage();
                float height = page.Height;
                float width = page.Width;

                // Create fonts
                var helvetica = new HpdfFont(pdf.Xref, HpdfStandardFont.Helvetica, "Helv");
                var helveticaBold = new HpdfFont(pdf.Xref, HpdfStandardFont.HelveticaBold, "HelvB");

                // Draw title
                page.SetRgbFill(0.0f, 0.0f, 0.0f);
                page.BeginText();
                page.SetFontAndSize(helveticaBold, 20);
                string title = "Font Metrics Demo";
                float titleWidth = helveticaBold.MeasureText(title, 20);
                page.MoveTextPos((width - titleWidth) / 2, height - 40);
                page.ShowText(title);
                page.EndText();

                // Draw subtitle
                page.BeginText();
                page.SetFontAndSize(helvetica, 12);
                string subtitle = "Demonstrating GetAscent, GetDescent, GetXHeight, GetBBox, and MeasureText";
                float subtitleWidth = helvetica.MeasureText(subtitle, 12);
                page.MoveTextPos((width - subtitleWidth) / 2, height - 58);
                page.ShowText(subtitle);
                page.EndText();

                // Current Y position (calculated using font metrics)
                float yPos = height - 90;

                // ========== SECTION 1: Standard Font (Helvetica) ==========
                yPos = DrawSectionHeader(page, helveticaBold, "1. Standard Font (Helvetica)", yPos);

                yPos = DrawTextSample(
                    page, helvetica, helvetica,
                    "The quick brown fox jumps over the lazy dog",
                    12,
                    yPos,
                    "Sample text with metrics visualization:");

                yPos -= 10; // Extra spacing between samples

                yPos = DrawTextSample(
                    page, helvetica, helvetica,
                    "UPPERCASE and lowercase: AaBbCcDdEe 12345",
                    14,
                    yPos,
                    "Different case and numbers:");

                yPos -= 30; // Section spacing

                // ========== SECTION 2: TrueType Font ==========
                if (File.Exists("demo/ttfont/Roboto-Regular.ttf"))
                {
                    var robotoFont = HpdfTrueTypeFont.LoadFromFile(pdf.Xref, "Roboto", "demo/ttfont/Roboto-Regular.ttf", true);

                    yPos = DrawSectionHeader(page, helveticaBold, "2. TrueType Font (Roboto)", yPos);

                    yPos = DrawTextSample(
                        page, robotoFont.AsFont(), helvetica,
                        "TrueType fonts provide precise metrics",
                        12,
                        yPos,
                        "Roboto Regular with embedded font data:");

                    yPos -= 10;

                    yPos = DrawTextSample(
                        page, robotoFont.AsFont(), helvetica,
                        "Modern sans-serif typeface: 0123456789",
                        14,
                        yPos,
                        "Larger size with numbers:");

                    yPos -= 30;
                }
                else
                {
                    Console.WriteLine("Warning: Roboto-Regular.ttf not found, skipping TrueType section");
                }

                // Check if we need a new page
                if (yPos < 200)
                {
                    page = pdf.AddPage();
                    height = page.Height;
                    width = page.Width;
                    yPos = height - 60;
                }

                // ========== SECTION 3: Type 1 Font ==========
                if (File.Exists("demo/Type1/a010013l.afm") && File.Exists("demo/Type1/a010013l.pfb"))
                {
                    var type1Font = HpdfType1Font.LoadFromFile(pdf.Xref, "URW", "demo/Type1/a010013l.afm", "demo/Type1/a010013l.pfb", 1252);

                    yPos = DrawSectionHeader(page, helveticaBold, "3. Type 1 Font (URW Gothic L)", yPos);

                    yPos = DrawTextSample(
                        page, type1Font.AsFont(), helvetica,
                        "Type 1 fonts use AFM metrics data",
                        12,
                        yPos,
                        "URW Gothic L Book with AFM/PFB:");

                    yPos -= 10;

                    yPos = DrawTextSample(
                        page, type1Font.AsFont(), helvetica,
                        "Classic PostScript font format",
                        14,
                        yPos,
                        "Traditional Type 1 rendering:");

                    yPos -= 30;
                }
                else
                {
                    Console.WriteLine("Warning: Type 1 font files not found, skipping Type 1 section");
                }

                // Check if we need a new page
                if (yPos < 200)
                {
                    page = pdf.AddPage();
                    height = page.Height;
                    width = page.Width;
                    yPos = height - 60;
                }

                // ========== SECTION 4: CID Font (Japanese) ==========
                if (File.Exists("demo/ttfont/noto-jp.ttf"))
                {
                    var japaneseFont = HpdfCIDFont.LoadFromTrueTypeFile(pdf, "JP", "demo/ttfont/noto-jp.ttf", 932);

                    yPos = DrawSectionHeader(page, helveticaBold, "4. CID Font (Japanese)", yPos);

                    yPos = DrawTextSample(
                        page, japaneseFont.AsFont(), helvetica,
                        "こんにちは世界",
                        14,
                        yPos,
                        "Japanese text (Hiragana + Kanji):");

                    yPos -= 10;

                    yPos = DrawTextSample(
                        page, japaneseFont.AsFont(), helvetica,
                        "日本語テスト",
                        16,
                        yPos,
                        "CID fonts support multi-byte characters:");

                    yPos -= 30;
                }
                else
                {
                    Console.WriteLine("Warning: noto-jp.ttf not found, skipping CID font section");
                }

                // ========== Legend ==========
                if (yPos < 150)
                {
                    page = pdf.AddPage();
                    height = page.Height;
                    width = page.Width;
                    yPos = height - 60;
                }

                yPos = DrawSectionHeader(page, helveticaBold, "Legend", yPos);

                // Draw legend items
                float legendX = LEFT_MARGIN + 20;
                float legendY = yPos - 5;

                // Blue line - Ascent
                page.SetRgbStroke(0.0f, 0.0f, 0.8f);
                page.SetLineWidth(2);
                page.MoveTo(legendX, legendY);
                page.LineTo(legendX + 30, legendY);
                page.Stroke();
                page.BeginText();
                page.SetFontAndSize(helvetica, 10);
                page.MoveTextPos(legendX + 35, legendY - 3);
                page.ShowText("Ascent (maximum height above baseline)");
                page.EndText();

                legendY -= 18;

                // Red line - Descent
                page.SetRgbStroke(0.8f, 0.0f, 0.0f);
                page.SetLineWidth(2);
                page.MoveTo(legendX, legendY);
                page.LineTo(legendX + 30, legendY);
                page.Stroke();
                page.BeginText();
                page.SetFontAndSize(helvetica, 10);
                page.MoveTextPos(legendX + 35, legendY - 3);
                page.ShowText("Descent (maximum depth below baseline)");
                page.EndText();

                legendY -= 18;

                // Green line - X-height
                page.SetRgbStroke(0.0f, 0.8f, 0.0f);
                page.SetLineWidth(2);
                page.MoveTo(legendX, legendY);
                page.LineTo(legendX + 30, legendY);
                page.Stroke();
                page.BeginText();
                page.SetFontAndSize(helvetica, 10);
                page.MoveTextPos(legendX + 35, legendY - 3);
                page.ShowText("X-Height (height of lowercase 'x')");
                page.EndText();

                legendY -= 18;

                // Gray line - Baseline
                page.SetRgbStroke(0.7f, 0.7f, 0.7f);
                page.SetLineWidth(2);
                page.MoveTo(legendX, legendY);
                page.LineTo(legendX + 30, legendY);
                page.Stroke();
                page.BeginText();
                page.SetFontAndSize(helvetica, 10);
                page.MoveTextPos(legendX + 35, legendY - 3);
                page.ShowText("Baseline (reference line for text)");
                page.EndText();

                legendY -= 18;

                // Black rectangle - Bounding box
                page.SetRgbStroke(0.0f, 0.0f, 0.0f);
                page.SetLineWidth(1);
                page.Rectangle(legendX, legendY - 8, 30, 10);
                page.Stroke();
                page.BeginText();
                page.SetFontAndSize(helvetica, 10);
                page.MoveTextPos(legendX + 35, legendY - 3);
                page.ShowText("Bounding Box (text area from descent to ascent)");
                page.EndText();

                // Add footer note
                legendY -= 30;
                page.BeginText();
                page.SetFontAndSize(helvetica, 9);
                page.MoveTextPos(LEFT_MARGIN, legendY);
                page.ShowText("Note: All measurements are scaled from 1000-unit glyph space to user space based on font size.");
                page.EndText();

                legendY -= 15;
                page.BeginText();
                page.SetFontAndSize(helvetica, 9);
                page.MoveTextPos(LEFT_MARGIN, legendY);
                page.ShowText("Position calculations use font metrics instead of hardcoded constants for accurate spacing.");
                page.EndText();

                pdf.SaveToFile("pdfs/FontMetricsDemo.pdf");
                Console.WriteLine("Font Metrics Demo created: pdfs/FontMetricsDemo.pdf");
                Console.WriteLine("Demonstrates font metrics for Standard, TrueType, Type 1, and CID fonts.");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Error in FontMetricsDemo: {e.Message}");
                Console.Error.WriteLine($"Stack trace: {e.StackTrace}");
                throw;
            }
        }
    }
}
