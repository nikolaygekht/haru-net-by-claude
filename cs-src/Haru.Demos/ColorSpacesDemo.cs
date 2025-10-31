/*
 * << Haru Free PDF Library >> -- ColorSpacesDemo.cs
 *
 * Demonstrates different color spaces (RGB, CMYK, Grayscale) for print and screen
 *
 * Copyright (c) 1999-2025 Haru Free PDF Library
 *
 * Permission to use, copy, modify, distribute and sell this software
 * and its documentation for any purpose is hereby granted without fee,
 */

using System;
using Haru.Doc;
using Haru.Font;
using Haru.Types;

namespace BasicDemos
{
    public static class ColorSpacesDemo
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

                CreateCMYKIntroPage(pdf, font, boldFont, italicFont);
                CreateRGBvsCMYKPage(pdf, font, boldFont);
                CreateGrayscaleAndRichBlacksPage(pdf, font, boldFont);

                pdf.SaveToFile("pdfs/ColorSpacesDemo.pdf");
                Console.WriteLine("ColorSpacesDemo completed successfully!");
                Console.WriteLine();
                Console.WriteLine("This demo shows different color spaces:");
                Console.WriteLine("  - RGB: For screen display (web, presentations)");
                Console.WriteLine("  - CMYK: For professional printing");
                Console.WriteLine("  - Grayscale: For black and white printing");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Error in ColorSpacesDemo: {e.Message}");
                Console.Error.WriteLine($"Stack trace: {e.StackTrace}");
                throw;
            }
        }

        private static void CreateCMYKIntroPage(HpdfDocument pdf, HpdfFont font, HpdfFont boldFont, HpdfFont italicFont)
        {
            var page = pdf.AddPage();
            float height = page.Height;
            float width = page.Width;

            DrawLabel(page, boldFont, 50, height - 50, "Color Spaces in PDFs", 24);
            DrawLabel(page, italicFont, 50, height - 75, "RGB, CMYK, and Grayscale color models", 12, 0.4f, 0.4f, 0.4f);

            float y = height - 120;

            page.SetRgbFill(0.95f, 0.98f, 1.0f);
            page.Rectangle(50, y - 130, width - 100, 120);
            page.Fill();

            page.SetRgbStroke(0.3f, 0.5f, 0.8f);
            page.SetLineWidth(2);
            page.Rectangle(50, y - 130, width - 100, 120);
            page.Stroke();

            DrawLabel(page, boldFont, 60, y - 25, "What Are Color Spaces?", 14, 0.0f, 0.3f, 0.6f);
            y -= 43;

            DrawLabel(page, font, 70, y, "Color spaces define how colors are represented in digital documents.", 10);
            y -= 18;
            DrawLabel(page, font, 70, y, "Each color space uses different components to create colors:", 10);
            y -= 25;

            DrawLabel(page, font, 80, y, "- RGB: Red, Green, Blue (additive, for screens)", 10);
            y -= 18;
            DrawLabel(page, font, 80, y, "- CMYK: Cyan, Magenta, Yellow, Black (subtractive, for print)", 10);
            y -= 18;
            DrawLabel(page, font, 80, y, "- Grayscale: Single channel from black to white", 10);

            y -= 50;

            DrawLabel(page, boldFont, 50, y, "CMYK Color Model", 14, 0.2f, 0.2f, 0.6f);
            y -= 25;
            DrawLabel(page, font, 50, y, "CMYK is essential for professional printing. Each component is a percentage (0.0-1.0):", 11);
            y -= 30;

            float swatchSize = 60;
            float swatchSpacing = 95;
            float startX = 80;

            DrawLabel(page, boldFont, startX, y, "C", 16, 0.0f, 0.6f, 0.8f);
            page.SetCmykFill(1.0f, 0.0f, 0.0f, 0.0f);
            page.Rectangle(startX - 10, y - 75, swatchSize, swatchSize);
            page.Fill();
            page.SetRgbStroke(0.5f, 0.5f, 0.5f);
            page.SetLineWidth(1);
            page.Rectangle(startX - 10, y - 75, swatchSize, swatchSize);
            page.Stroke();
            DrawLabel(page, font, startX - 5, y - 85, "Cyan", 9);
            DrawLabel(page, font, startX - 10, y - 98, "100% C", 8, 0.5f, 0.5f, 0.5f);

            startX += swatchSpacing;
            DrawLabel(page, boldFont, startX, y, "M", 16, 0.8f, 0.0f, 0.6f);
            page.SetCmykFill(0.0f, 1.0f, 0.0f, 0.0f);
            page.Rectangle(startX - 10, y - 75, swatchSize, swatchSize);
            page.Fill();
            page.SetRgbStroke(0.5f, 0.5f, 0.5f);
            page.SetLineWidth(1);
            page.Rectangle(startX - 10, y - 75, swatchSize, swatchSize);
            page.Stroke();
            DrawLabel(page, font, startX - 10, y - 85, "Magenta", 9);
            DrawLabel(page, font, startX - 10, y - 98, "100% M", 8, 0.5f, 0.5f, 0.5f);

            startX += swatchSpacing;
            DrawLabel(page, boldFont, startX, y, "Y", 16, 0.8f, 0.8f, 0.0f);
            page.SetCmykFill(0.0f, 0.0f, 1.0f, 0.0f);
            page.Rectangle(startX - 10, y - 75, swatchSize, swatchSize);
            page.Fill();
            page.SetRgbStroke(0.5f, 0.5f, 0.5f);
            page.SetLineWidth(1);
            page.Rectangle(startX - 10, y - 75, swatchSize, swatchSize);
            page.Stroke();
            DrawLabel(page, font, startX - 5, y - 85, "Yellow", 9);
            DrawLabel(page, font, startX - 10, y - 98, "100% Y", 8, 0.5f, 0.5f, 0.5f);

            startX += swatchSpacing;
            DrawLabel(page, boldFont, startX, y, "K", 16, 0.2f, 0.2f, 0.2f);
            page.SetCmykFill(0.0f, 0.0f, 0.0f, 1.0f);
            page.Rectangle(startX - 10, y - 75, swatchSize, swatchSize);
            page.Fill();
            page.SetRgbStroke(0.5f, 0.5f, 0.5f);
            page.SetLineWidth(1);
            page.Rectangle(startX - 10, y - 75, swatchSize, swatchSize);
            page.Stroke();
            DrawLabel(page, font, startX - 5, y - 85, "Black", 9);
            DrawLabel(page, font, startX - 10, y - 98, "100% K", 8, 0.5f, 0.5f, 0.5f);

            startX += swatchSpacing;
            DrawLabel(page, boldFont, startX, y, "CMY", 14, 0.3f, 0.3f, 0.3f);
            page.SetCmykFill(1.0f, 1.0f, 1.0f, 0.0f);
            page.Rectangle(startX - 10, y - 75, swatchSize, swatchSize);
            page.Fill();
            page.SetRgbStroke(0.5f, 0.5f, 0.5f);
            page.SetLineWidth(1);
            page.Rectangle(startX - 10, y - 75, swatchSize, swatchSize);
            page.Stroke();
            DrawLabel(page, font, startX - 10, y - 85, "100% All", 9);
            DrawLabel(page, font, startX - 15, y - 98, "(Composite)", 8, 0.5f, 0.5f, 0.5f);

            y -= 130;

            DrawLabel(page, boldFont, 50, y, "CMYK Color Mixing Examples", 14, 0.2f, 0.2f, 0.6f);
            y -= 30;

            float mixX = 70;
            float mixSpacing = 85;

            page.SetCmykFill(0.5f, 0.0f, 0.5f, 0.0f);
            page.Rectangle(mixX, y - 55, 55, 55);
            page.Fill();
            page.SetRgbStroke(0.5f, 0.5f, 0.5f);
            page.SetLineWidth(1);
            page.Rectangle(mixX, y - 55, 55, 55);
            page.Stroke();
            DrawLabel(page, font, mixX + 5, y - 68, "C50+Y50", 8, 0.5f, 0.5f, 0.5f);
            DrawLabel(page, font, mixX + 10, y - 80, "Green", 9);

            mixX += mixSpacing;
            page.SetCmykFill(0.0f, 0.5f, 0.5f, 0.0f);
            page.Rectangle(mixX, y - 55, 55, 55);
            page.Fill();
            page.SetRgbStroke(0.5f, 0.5f, 0.5f);
            page.SetLineWidth(1);
            page.Rectangle(mixX, y - 55, 55, 55);
            page.Stroke();
            DrawLabel(page, font, mixX + 5, y - 68, "M50+Y50", 8, 0.5f, 0.5f, 0.5f);
            DrawLabel(page, font, mixX + 12, y - 80, "Red", 9);

            mixX += mixSpacing;
            page.SetCmykFill(0.5f, 0.5f, 0.0f, 0.0f);
            page.Rectangle(mixX, y - 55, 55, 55);
            page.Fill();
            page.SetRgbStroke(0.5f, 0.5f, 0.5f);
            page.SetLineWidth(1);
            page.Rectangle(mixX, y - 55, 55, 55);
            page.Stroke();
            DrawLabel(page, font, mixX + 5, y - 68, "C50+M50", 8, 0.5f, 0.5f, 0.5f);
            DrawLabel(page, font, mixX + 10, y - 80, "Blue", 9);

            mixX += mixSpacing;
            page.SetCmykFill(0.2f, 0.7f, 1.0f, 0.0f);
            page.Rectangle(mixX, y - 55, 55, 55);
            page.Fill();
            page.SetRgbStroke(0.5f, 0.5f, 0.5f);
            page.SetLineWidth(1);
            page.Rectangle(mixX, y - 55, 55, 55);
            page.Stroke();
            DrawLabel(page, font, mixX + 2, y - 68, "C20M70Y100", 7, 0.5f, 0.5f, 0.5f);
            DrawLabel(page, font, mixX + 8, y - 80, "Orange", 9);

            mixX += mixSpacing;
            page.SetCmykFill(0.6f, 0.0f, 0.6f, 0.0f);
            page.Rectangle(mixX, y - 55, 55, 55);
            page.Fill();
            page.SetRgbStroke(0.5f, 0.5f, 0.5f);
            page.SetLineWidth(1);
            page.Rectangle(mixX, y - 55, 55, 55);
            page.Stroke();
            DrawLabel(page, font, mixX + 5, y - 68, "C60+Y60", 8, 0.5f, 0.5f, 0.5f);
            DrawLabel(page, font, mixX - 5, y - 80, "Forest Green", 9);

            mixX += mixSpacing;
            page.SetCmykFill(0.5f, 0.7f, 0.0f, 0.0f);
            page.Rectangle(mixX, y - 55, 55, 55);
            page.Fill();
            page.SetRgbStroke(0.5f, 0.5f, 0.5f);
            page.SetLineWidth(1);
            page.Rectangle(mixX, y - 55, 55, 55);
            page.Stroke();
            DrawLabel(page, font, mixX + 5, y - 68, "C50+M70", 8, 0.5f, 0.5f, 0.5f);
            DrawLabel(page, font, mixX + 8, y - 80, "Purple", 9);

            y -= 110;

            page.SetRgbStroke(0.5f, 0.5f, 0.5f);
            page.SetLineWidth(1);
            page.Rectangle(50, y - 70, width - 100, 60);
            page.Stroke();

            DrawLabel(page, boldFont, 60, y - 20, "Code Example:", 11);
            DrawLabel(page, font, 70, y - 37, "page.SetCmykFill(0.5f, 0.0f, 0.5f, 0.0f);  // 50% Cyan + 50% Yellow = Green", 9, 0.3f, 0.3f, 0.3f);
            DrawLabel(page, font, 70, y - 52, "page.SetCmykStroke(1.0f, 0.0f, 0.0f, 0.0f);  // 100% Cyan for stroke", 9, 0.3f, 0.3f, 0.3f);
        }

        private static void CreateRGBvsCMYKPage(HpdfDocument pdf, HpdfFont font, HpdfFont boldFont)
        {
            var page = pdf.AddPage();
            float height = page.Height;
            float width = page.Width;

            DrawLabel(page, boldFont, 50, height - 50, "RGB vs CMYK Comparison", 24);
            DrawLabel(page, font, 50, height - 75, "Understanding the difference between screen and print color spaces", 12, 0.4f, 0.4f, 0.4f);

            float y = height - 130;

            DrawLabel(page, boldFont, 50, y, "RGB (Screen/Additive)", 14, 0.2f, 0.2f, 0.6f);
            DrawLabel(page, boldFont, 320, y, "CMYK (Print/Subtractive)", 14, 0.2f, 0.2f, 0.6f);
            y -= 30;

            float rgbX = 80;
            float cmykX = 350;
            float compSize = 70;

            string[] colorNames = { "Red", "Green", "Blue", "Yellow", "Cyan", "Magenta" };
            float[,] rgbColors = {
                { 1.0f, 0.0f, 0.0f },
                { 0.0f, 1.0f, 0.0f },
                { 0.0f, 0.0f, 1.0f },
                { 1.0f, 1.0f, 0.0f },
                { 0.0f, 1.0f, 1.0f },
                { 1.0f, 0.0f, 1.0f }
            };
            float[,] cmykColors = {
                { 0.0f, 1.0f, 1.0f, 0.0f },
                { 1.0f, 0.0f, 1.0f, 0.0f },
                { 1.0f, 1.0f, 0.0f, 0.0f },
                { 0.0f, 0.0f, 1.0f, 0.0f },
                { 1.0f, 0.0f, 0.0f, 0.0f },
                { 0.0f, 1.0f, 0.0f, 0.0f }
            };

            for (int i = 0; i < 6; i++)
            {
                float rowY = y - i * 80;

                page.SetRgbFill(rgbColors[i, 0], rgbColors[i, 1], rgbColors[i, 2]);
                page.Rectangle(rgbX, rowY - compSize, compSize, compSize);
                page.Fill();
                page.SetRgbStroke(0.5f, 0.5f, 0.5f);
                page.SetLineWidth(1);
                page.Rectangle(rgbX, rowY - compSize, compSize, compSize);
                page.Stroke();

                page.SetCmykFill(cmykColors[i, 0], cmykColors[i, 1], cmykColors[i, 2], cmykColors[i, 3]);
                page.Rectangle(cmykX, rowY - compSize, compSize, compSize);
                page.Fill();
                page.SetRgbStroke(0.5f, 0.5f, 0.5f);
                page.SetLineWidth(1);
                page.Rectangle(cmykX, rowY - compSize, compSize, compSize);
                page.Stroke();

                DrawLabel(page, font, rgbX + compSize + 10, rowY - 30, colorNames[i], 11);
                DrawLabel(page, font, rgbX + compSize + 10, rowY - 45, $"RGB({rgbColors[i, 0]:F0}, {rgbColors[i, 1]:F0}, {rgbColors[i, 2]:F0})", 8, 0.5f, 0.5f, 0.5f);
                DrawLabel(page, font, rgbX + compSize + 10, rowY - 58, $"CMYK({cmykColors[i, 0]:F0}, {cmykColors[i, 1]:F0}, {cmykColors[i, 2]:F0}, {cmykColors[i, 3]:F0})", 8, 0.5f, 0.5f, 0.5f);
            }

            y = y - 6 * 80 - 20;

            page.SetRgbFill(1.0f, 0.98f, 0.9f);
            page.Rectangle(50, y - 130, width - 100, 120);
            page.Fill();

            page.SetRgbStroke(0.8f, 0.6f, 0.2f);
            page.SetLineWidth(2);
            page.Rectangle(50, y - 130, width - 100, 120);
            page.Stroke();

            DrawLabel(page, boldFont, 60, y - 23, "Key Differences:", 12, 0.6f, 0.4f, 0.0f);
            y -= 40;

            DrawLabel(page, font, 70, y, "RGB (Additive):", 10);
            DrawLabel(page, font, 200, y, "CMYK (Subtractive):", 10);
            y -= 18;

            DrawLabel(page, font, 70, y, "- Light-based (screens)", 9);
            DrawLabel(page, font, 200, y, "- Ink-based (printers)", 9);
            y -= 15;

            DrawLabel(page, font, 70, y, "- Colors add to white", 9);
            DrawLabel(page, font, 200, y, "- Colors add to black", 9);
            y -= 15;

            DrawLabel(page, font, 70, y, "- Wider color gamut", 9);
            DrawLabel(page, font, 200, y, "- Limited by ink physics", 9);
            y -= 15;

            DrawLabel(page, font, 70, y, "- Use for: web, presentations", 9);
            DrawLabel(page, font, 200, y, "- Use for: professional printing", 9);
            y -= 15;

            DrawLabel(page, font, 70, y, "- Display variations common", 9);
            DrawLabel(page, font, 200, y, "- More predictable on paper", 9);
        }

        private static void CreateGrayscaleAndRichBlacksPage(HpdfDocument pdf, HpdfFont font, HpdfFont boldFont)
        {
            var page = pdf.AddPage();
            float height = page.Height;
            float width = page.Width;

            DrawLabel(page, boldFont, 50, height - 50, "Grayscale & Rich Blacks", 24);
            DrawLabel(page, font, 50, height - 75, "Black and white printing with advanced techniques", 12, 0.4f, 0.4f, 0.4f);

            float y = height - 130;

            DrawLabel(page, boldFont, 50, y, "Grayscale Color Space", 14, 0.2f, 0.2f, 0.6f);
            y -= 25;
            DrawLabel(page, font, 50, y, "Single channel from white (0.0) to black (1.0):", 11);
            y -= 35;

            float grayX = 70;
            float graySize = 50;
            float graySpacing = 65;

            // First row: 0% to 50%
            for (int i = 0; i <= 5; i++)
            {
                float gray = i / 10.0f;
                page.SetGrayFill(gray);
                page.Rectangle(grayX + i * graySpacing, y - graySize, graySize, graySize);
                page.Fill();

                page.SetRgbStroke(0.5f, 0.5f, 0.5f);
                page.SetLineWidth(1);
                page.Rectangle(grayX + i * graySpacing, y - graySize, graySize, graySize);
                page.Stroke();

                DrawLabel(page, font, grayX + i * graySpacing + 8, y - graySize / 2 - 4, $"{(int)(gray * 100)}%", 8, 1.0f, 1.0f, 1.0f);
            }

            float row2Y = y - graySize - 20;

            // Second row: 60% to 100%
            for (int i = 6; i <= 10; i++)
            {
                float gray = i / 10.0f;
                page.SetGrayFill(gray);
                page.Rectangle(grayX + (i - 6) * graySpacing, row2Y - graySize, graySize, graySize);
                page.Fill();

                page.SetRgbStroke(0.5f, 0.5f, 0.5f);
                page.SetLineWidth(1);
                page.Rectangle(grayX + (i - 6) * graySpacing, row2Y - graySize, graySize, graySize);
                page.Stroke();

                DrawLabel(page, font, grayX + (i - 6) * graySpacing + 8, row2Y - graySize / 2 - 4, $"{(int)(gray * 100)}%", 8, 0.0f, 0.0f, 0.0f);
            }

            y -= 140;

            DrawLabel(page, font, 50, y, "Code: page.SetGrayFill(0.5f);  // 50% gray", 9, 0.3f, 0.3f, 0.3f);

            y -= 50;

            DrawLabel(page, boldFont, 50, y, "Rich Blacks for Print", 14, 0.2f, 0.2f, 0.6f);
            y -= 25;
            DrawLabel(page, font, 50, y, "Adding color to pure black creates deeper, richer blacks for professional printing:", 11);
            y -= 35;

            float blackX = 70;
            float blackSize = 80;
            float blackSpacing = 110;

            DrawLabel(page, font, blackX + 5, y, "Standard", 10);
            page.SetCmykFill(0.0f, 0.0f, 0.0f, 1.0f);
            page.Rectangle(blackX, y - 90, blackSize, blackSize);
            page.Fill();
            page.SetRgbStroke(0.5f, 0.5f, 0.5f);
            page.SetLineWidth(1);
            page.Rectangle(blackX, y - 90, blackSize, blackSize);
            page.Stroke();
            DrawLabel(page, font, blackX, y - 103, "K100", 9, 0.5f, 0.5f, 0.5f);
            DrawLabel(page, font, blackX - 5, y - 116, "C0 M0 Y0 K100", 8, 0.5f, 0.5f, 0.5f);

            blackX += blackSpacing;
            DrawLabel(page, font, blackX + 15, y, "Cool", 10);
            page.SetCmykFill(0.6f, 0.4f, 0.4f, 1.0f);
            page.Rectangle(blackX, y - 90, blackSize, blackSize);
            page.Fill();
            page.SetRgbStroke(0.5f, 0.5f, 0.5f);
            page.SetLineWidth(1);
            page.Rectangle(blackX, y - 90, blackSize, blackSize);
            page.Stroke();
            DrawLabel(page, font, blackX, y - 103, "C60+K100", 9, 0.5f, 0.5f, 0.5f);
            DrawLabel(page, font, blackX - 5, y - 116, "C60 M40 Y40 K100", 8, 0.5f, 0.5f, 0.5f);

            blackX += blackSpacing;
            DrawLabel(page, font, blackX + 12, y, "Warm", 10);
            page.SetCmykFill(0.4f, 0.6f, 0.6f, 1.0f);
            page.Rectangle(blackX, y - 90, blackSize, blackSize);
            page.Fill();
            page.SetRgbStroke(0.5f, 0.5f, 0.5f);
            page.SetLineWidth(1);
            page.Rectangle(blackX, y - 90, blackSize, blackSize);
            page.Stroke();
            DrawLabel(page, font, blackX, y - 103, "M60+Y60+K100", 8, 0.5f, 0.5f, 0.5f);
            DrawLabel(page, font, blackX - 5, y - 116, "C40 M60 Y60 K100", 8, 0.5f, 0.5f, 0.5f);

            blackX += blackSpacing;
            DrawLabel(page, font, blackX + 15, y, "Rich", 10);
            page.SetCmykFill(0.6f, 0.5f, 0.5f, 1.0f);
            page.Rectangle(blackX, y - 90, blackSize, blackSize);
            page.Fill();
            page.SetRgbStroke(0.5f, 0.5f, 0.5f);
            page.SetLineWidth(1);
            page.Rectangle(blackX, y - 90, blackSize, blackSize);
            page.Stroke();
            DrawLabel(page, font, blackX, y - 103, "Balanced", 9, 0.5f, 0.5f, 0.5f);
            DrawLabel(page, font, blackX - 5, y - 116, "C60 M50 Y50 K100", 8, 0.5f, 0.5f, 0.5f);

            y -= 150;

            DrawLabel(page, font, 50, y, "Rich blacks provide:", 10);
            y -= 18;
            DrawLabel(page, font, 70, y, "- Deeper, more saturated black", 9);
            y -= 15;
            DrawLabel(page, font, 70, y, "- Better coverage on coated paper", 9);
            y -= 15;
            DrawLabel(page, font, 70, y, "- More luxurious appearance for high-end prints", 9);
            y -= 15;
            DrawLabel(page, font, 70, y, "- Warning: Don't exceed 300% total ink coverage", 9);

            y -= 40;

            page.SetRgbStroke(0.5f, 0.5f, 0.5f);
            page.SetLineWidth(1);
            page.Rectangle(50, y - 100, width - 100, 90);
            page.Stroke();

            DrawLabel(page, boldFont, 60, y - 23, "Print-Ready Best Practices:", 11);
            y -= 40;

            DrawLabel(page, font, 70, y, "- Use CMYK for all commercial printing", 10);
            y -= 18;
            DrawLabel(page, font, 70, y, "- Consult your printer for specific color profiles", 10);
            y -= 18;
            DrawLabel(page, font, 70, y, "- Test print to verify colors before large runs", 10);
            y -= 18;
            DrawLabel(page, font, 70, y, "- Rich blacks: great for backgrounds, not for small text", 10);
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
