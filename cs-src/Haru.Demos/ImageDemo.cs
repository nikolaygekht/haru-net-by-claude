/*
 * << Haru Free PDF Library >> -- ImageDemo.cs
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

namespace BasicDemos
{
    public static class ImageDemo
    {
        private static void ShowDescription(HpdfPage page, float x, float y, string text)
        {
            page.MoveTo(x, y - 10);
            page.LineTo(x, y + 10);
            page.MoveTo(x - 10, y);
            page.LineTo(x + 10, y);
            page.Stroke();

            page.SetFontAndSize(page.CurrentFont, 8);
            page.SetRgbFill(0, 0, 0);

            page.BeginText();

            string buf = $"(x={x},y={y})";
            page.MoveTextPos(x - page.CurrentFont.MeasureText(buf, 8) - 5, y - 10);
            page.ShowText(buf);
            page.EndText();

            page.BeginText();
            page.MoveTextPos(x - 20, y - 25);
            page.ShowText(text);
            page.EndText();
        }

        public static void Run()
        {
            try
            {
                var pdf = new HpdfDocument();

                // Create default-font
                var font = new HpdfFont(pdf.Xref, HpdfStandardFont.Helvetica, "F1");

                // Add a new page object
                var page = pdf.AddPage();

                page.Width = 550;
                page.Height = 500;

                page.BeginText();
                page.SetFontAndSize(font, 20);
                page.MoveTextPos(220, page.Height - 70);
                page.ShowText("ImageDemo");
                page.EndText();

                // Load image file
                var image = pdf.LoadPngImageFromFile("demo/pngsuite/basn3p02.png");

                // image1 is masked by image2
                var image1 = pdf.LoadPngImageFromFile("demo/pngsuite/basn3p02.png");

                // image2 is a mask image
                var image2 = pdf.LoadPngImageFromFile("demo/pngsuite/basn0g01.png");

                // image3 is a RGB-color image. we use this image for color-mask demo
                var image3 = pdf.LoadPngImageFromFile("demo/pngsuite/maskimage.png");

                // image4 is an image with alpha channel
                var image4 = pdf.LoadPngImageFromFile("demo/pngsuite/basn6a08.png");

                float iw = image.Width;
                float ih = image.Height;

                page.SetLineWidth(0.5f);

                float x = 100;
                float y = page.Height - 150;

                // Draw image to the canvas (normal-mode with actual size)
                page.DrawImage(image, x, y, iw, ih);

                ShowDescription(page, x, y, "Actual Size");

                x += 150;

                // Scaling image (X direction)
                page.DrawImage(image, x, y, iw * 1.5f, ih);

                ShowDescription(page, x, y, "Scaling image (X direction)");

                x += 150;

                // Scaling image (Y direction)
                page.DrawImage(image, x, y, iw, ih * 1.5f);
                ShowDescription(page, x, y, "Scaling image (Y direction)");

                x = 100;
                y -= 120;

                // Skewing image
                float angle1 = 10;
                float angle2 = 20;
                float rad1 = angle1 / 180 * 3.141592f;
                float rad2 = angle2 / 180 * 3.141592f;

                page.GSave();

                page.Concat(iw, (float)Math.Tan(rad1) * iw, (float)Math.Tan(rad2) * ih,
                        ih, x, y);

                page.ExecuteXObject(image);
                page.GRestore();

                ShowDescription(page, x, y, "Skewing image");

                x += 150;

                // Rotating image
                float angle = 30;     // rotation of 30 degrees
                float rad = angle / 180 * 3.141592f; // Calculate the radian value

                page.GSave();

                page.Concat((float)(iw * Math.Cos(rad)),
                    (float)(iw * Math.Sin(rad)),
                    (float)(ih * -Math.Sin(rad)),
                    (float)(ih * Math.Cos(rad)),
                    x, y);

                page.ExecuteXObject(image);
                page.GRestore();

                ShowDescription(page, x, y, "Rotating image");

                x += 150;

                // Draw masked image

                // Set image2 to the mask image of image1
                image1.SetMaskImage(image2);

                page.SetRgbFill(0, 0, 0);
                page.BeginText();
                page.MoveTextPos(x - 6, y + 14);
                page.ShowText("MASKMASK");
                page.EndText();

                page.DrawImage(image1, x - 3, y - 3, iw + 6, ih + 6);

                ShowDescription(page, x, y, "masked image");

                x = 100;
                y -= 120;

                // Color mask
                page.SetRgbFill(0, 0, 0);
                page.BeginText();
                page.MoveTextPos(x - 6, y + 14);
                page.ShowText("MASKMASK");
                page.EndText();

                image3.SetColorMask(0, 255, 0, 0, 0, 255);
                page.DrawImage(image3, x, y, iw, ih);

                ShowDescription(page, x, y, "Color Mask");

                x += 150;

                // Draw image with alpha
                page.SetRgbFill(0, 0, 0);
                page.BeginText();
                page.MoveTextPos(x - 6, y + 14);
                page.ShowText("MASKMASK");
                page.EndText();

                page.DrawImage(image1, x - 3, y - 3, iw + 6, ih + 6);

                ShowDescription(page, x, y, "image/w alpha");

                x = 100;
                y -= 120;

                // Color mask
                page.SetRgbFill(0, 0, 0);
                page.BeginText();
                page.MoveTextPos(x - 6, y + 14);
                page.ShowText("MASKMASK");
                page.EndText();

                page.DrawImage(image4, x, y, iw, ih);

                ShowDescription(page, x, y, "Alpha");

                // Save the document to a file
                pdf.SaveToFile("pdfs/ImageDemo.pdf");

            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Error in ImageDemo: {e.Message}");
                Console.Error.WriteLine($"Stack trace: {e.StackTrace}");
                throw;
            }
        }
    }
}
