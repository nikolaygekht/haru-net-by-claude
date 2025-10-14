/*
 * << Haru Free PDF Library >> -- JpegDemo.cs
 *
 * Demo showing JPEG image support in Haru PDF library
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
    public static class JpegDemo
    {
        private static void DrawDescription(HpdfPage page, HpdfFont font, float x, float y, string text)
        {
            page.SetFontAndSize(font, 10);
            page.SetRgbFill(0, 0, 0);

            page.BeginText();
            page.MoveTextPos(x, y - 15);
            page.ShowText(text);
            page.EndText();
        }

        public static void Run()
        {
            try
            {
                var pdf = new HpdfDocument();

                // Create default font
                var font = new HpdfFont(pdf.Xref, HpdfStandardFont.Helvetica, "F1");

                // Add a new page
                var page = pdf.AddPage();
                page.Width = 595;  // A4 width
                page.Height = 842; // A4 height

                // Title
                page.BeginText();
                page.SetFontAndSize(font, 24);
                page.MoveTextPos(50, page.Height - 50);
                page.ShowText("JPEG Image Demo");
                page.EndText();

                // Description
                page.BeginText();
                page.SetFontAndSize(font, 12);
                page.MoveTextPos(50, page.Height - 80);
                page.ShowText("Demonstrating JPEG image support in Haru.NET");
                page.EndText();

                page.BeginText();
                page.SetFontAndSize(font, 10);
                page.MoveTextPos(50, page.Height - 100);
                page.ShowText("JPEG images are embedded directly without re-encoding, preserving quality and size.");
                page.EndText();

                float currentY = page.Height - 150;

                // Load and display RGB JPEG image
                Console.WriteLine("Loading RGB JPEG image...");
                var rgbImage = pdf.LoadJpegImageFromFile("demo/images/rgb.jpg");

                float rgbWidth = rgbImage.Width;
                float rgbHeight = rgbImage.Height;

                // Scale image to fit page (max width 400)
                float scale = 400.0f / rgbWidth;
                if (scale > 1.0f) scale = 1.0f;

                float displayWidth = rgbWidth * scale;
                float displayHeight = rgbHeight * scale;

                page.DrawImage(rgbImage, 50, currentY - displayHeight, displayWidth, displayHeight);
                DrawDescription(page, font, 50, currentY - displayHeight,
                    $"RGB JPEG Image ({rgbImage.Width}x{rgbImage.Height} pixels)");

                currentY -= displayHeight + 50;

                // Load and display grayscale JPEG image
                Console.WriteLine("Loading Grayscale JPEG image...");
                var grayImage = pdf.LoadJpegImageFromFile("demo/images/gray.jpg");

                float grayWidth = grayImage.Width;
                float grayHeight = grayImage.Height;

                // Scale image to fit page (max width 400)
                scale = 400.0f / grayWidth;
                if (scale > 1.0f) scale = 1.0f;

                displayWidth = grayWidth * scale;
                displayHeight = grayHeight * scale;

                if (currentY - displayHeight > 50)
                {
                    page.DrawImage(grayImage, 50, currentY - displayHeight, displayWidth, displayHeight);
                    DrawDescription(page, font, 50, currentY - displayHeight,
                        $"Grayscale JPEG Image ({grayImage.Width}x{grayImage.Height} pixels)");
                }
                else
                {
                    // Add new page if current page is full
                    page = pdf.AddPage();
                    page.Width = 595;
                    page.Height = 842;
                    page.SetFontAndSize(font, 10); // Set default font for new page

                    currentY = page.Height - 50;
                    page.DrawImage(grayImage, 50, currentY - displayHeight, displayWidth, displayHeight);
                    DrawDescription(page, font, 50, currentY - displayHeight,
                        $"Grayscale JPEG Image ({grayImage.Width}x{grayImage.Height} pixels)");
                }

                // Add page with image transformations
                page = pdf.AddPage();
                page.Width = 595;
                page.Height = 842;
                page.SetFontAndSize(font, 10); // Set default font for new page

                // Title for transformations page
                page.BeginText();
                page.SetFontAndSize(font, 20);
                page.MoveTextPos(50, page.Height - 50);
                page.ShowText("JPEG Image Transformations");
                page.EndText();

                currentY = page.Height - 100;

                // Original size
                float x = 50;
                float y = currentY;
                scale = 150.0f / Math.Max(rgbWidth, rgbHeight);
                displayWidth = rgbWidth * scale;
                displayHeight = rgbHeight * scale;

                page.DrawImage(rgbImage, x, y - displayHeight, displayWidth, displayHeight);
                DrawDescription(page, font, x, y - displayHeight, "Original");

                // Scaled (2x width)
                x = 250;
                page.DrawImage(rgbImage, x, y - displayHeight, displayWidth * 1.5f, displayHeight);
                DrawDescription(page, font, x, y - displayHeight, "Scaled X (1.5x)");

                // Scaled (2x height)
                x = 450;
                page.DrawImage(rgbImage, x, y - displayHeight * 1.5f, displayWidth, displayHeight * 1.5f);
                DrawDescription(page, font, x, y - displayHeight * 1.5f, "Scaled Y (1.5x)");

                // Rotated
                y -= displayHeight + 100;
                x = 50;

                float angle = 45; // 45 degrees rotation
                float rad = angle / 180 * (float)Math.PI;

                page.GSave();
                page.Concat(
                    (float)(displayWidth * Math.Cos(rad)),
                    (float)(displayWidth * Math.Sin(rad)),
                    (float)(displayHeight * -Math.Sin(rad)),
                    (float)(displayHeight * Math.Cos(rad)),
                    x + 50, y - 50);
                page.ExecuteXObject(rgbImage);
                page.GRestore();
                DrawDescription(page, font, x + 50, y - 50, "Rotated 45°");

                // Add information page
                page = pdf.AddPage();
                page.Width = 595;
                page.Height = 842;
                page.SetFontAndSize(font, 10); // Set default font for new page

                page.BeginText();
                page.SetFontAndSize(font, 18);
                page.MoveTextPos(50, page.Height - 50);
                page.ShowText("About JPEG Support");
                page.EndText();

                currentY = page.Height - 100;

                string[] info = new string[]
                {
                    "JPEG Support in Haru.NET:",
                    "",
                    "- Direct JPEG embedding without re-compression",
                    "- Preserves original image quality",
                    "- Smaller PDF file sizes",
                    "- Supports grayscale and RGB color spaces",
                    "- Supports CMYK color space (for print)",
                    "",
                    "Implementation:",
                    "- Uses StbImageSharp for PNG decoding",
                    "- Native JPEG parsing for metadata extraction",
                    "- Raw JPEG data embedded with DCTDecode filter",
                    "",
                    "Benefits:",
                    "- Zero dependencies on platform-specific libraries",
                    "- Cross-platform compatibility",
                    "- Lightweight and efficient"
                };

                page.BeginText();
                page.SetFontAndSize(font, 11);

                foreach (var line in info)
                {
                    page.MoveTextPos(50, currentY);
                    page.ShowTextNextLine(line);
                    currentY -= 20;
                }
                page.EndText();

                // Save the document
                pdf.SaveToFile("pdfs/JpegDemo.pdf");
                Console.WriteLine("PDF created successfully: pdfs/JpegDemo.pdf");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Error in JpegDemo: {e.Message}");
                Console.Error.WriteLine($"Stack trace: {e.StackTrace}");
                throw;
            }
        }
    }
}
