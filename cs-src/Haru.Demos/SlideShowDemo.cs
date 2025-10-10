/*
 * << Haru Free PDF Library >> -- SlideShowDemo.cs
 *
 * Adapted from original C demo to use the new C# Haru library
 *
 * Copyright (c) 1999-2025 Haru Free PDF Library
 *
 * Permission to use, copy, modify, distribute and sell this software
 * and its documentation for any purpose is hereby granted without fee,
 */

using System;
using Haru.Annotations;
using Haru.Doc;
using Haru.Font;
using Haru.Types;

namespace BasicDemos
{
    public static class SlideShowDemo
    {
        private static Random rand = new Random();

        private static void PrintPage(HpdfPage page, HpdfFont font, string caption,
            HpdfTransitionStyle type, HpdfPage? prev, HpdfPage? next)
        {
            float r = (float)rand.Next(255) / 255f;
            float g = (float)rand.Next(255) / 255f;
            float b = (float)rand.Next(255) / 255f;

            page.Width = 800;
            page.Height = 600;

            // Fill background with random color
            page.SetRgbFill(r, g, b);
            page.Rectangle(0, 0, 800, 600);
            page.Fill();

            // Set complementary color for text
            page.SetRgbFill(1.0f - r, 1.0f - g, 1.0f - b);

            // Print caption with skewed text
            page.SetFontAndSize(font, 30);
            page.BeginText();
            page.SetTextMatrix(0.8f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f);
            page.MoveTextPos(50, 530);
            page.ShowText(caption);
            page.EndText();

            // Print instruction text
            page.BeginText();
            page.SetTextMatrix(1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f);
            page.SetFontAndSize(font, 20);
            page.MoveTextPos(55, 300);
            page.ShowText("Type \"Ctrl+L\" in order to return from full screen mode.");
            page.EndText();

            // Set slide show transition
            page.SetSlideShow(type, 5.0f, 1.0f);

            page.SetFontAndSize(font, 20);

            // Add "Next" link if there's a next page
            if (next != null)
            {
                page.BeginText();
                page.MoveTextPos(680, 50);
                page.ShowText("Next=>");
                page.EndText();

                var rect = new HpdfRect
                {
                    Left = 680,
                    Right = 750,
                    Top = 70,
                    Bottom = 50
                };

                var dst = new HpdfDestination(next);
                dst.SetFit();
                var annot = page.CreateLinkAnnotation(rect, dst.DestArray);
                annot.SetBorderStyle(0, 0, 0);
                annot.SetHighlightMode(HpdfHighlightMode.InvertBox);
            }

            // Add "Prev" link if there's a previous page
            if (prev != null)
            {
                page.BeginText();
                page.MoveTextPos(50, 50);
                page.ShowText("<=Prev");
                page.EndText();

                var rect = new HpdfRect
                {
                    Left = 50,
                    Right = 110,
                    Top = 70,
                    Bottom = 50
                };

                var dst = new HpdfDestination(prev);
                dst.SetFit();
                var annot = page.CreateLinkAnnotation(rect, dst.DestArray);
                annot.SetBorderStyle(0, 0, 0);
                annot.SetHighlightMode(HpdfHighlightMode.InvertBox);
            }
        }

        public static void Run()
        {
            try
            {
                var pdf = new HpdfDocument();

                // Create default font
                var font = new HpdfFont(pdf.Xref, HpdfStandardFont.Courier, "F1");

                // Add 17 pages to the document
                var pages = new HpdfPage[17];
                for (int i = 0; i < 17; i++)
                {
                    pages[i] = pdf.AddPage();
                }

                // Print pages with different transition styles
                PrintPage(pages[0], font, "WIPE_RIGHT",
                    HpdfTransitionStyle.WipeRight, null, pages[1]);
                PrintPage(pages[1], font, "WIPE_UP",
                    HpdfTransitionStyle.WipeUp, pages[0], pages[2]);
                PrintPage(pages[2], font, "WIPE_LEFT",
                    HpdfTransitionStyle.WipeLeft, pages[1], pages[3]);
                PrintPage(pages[3], font, "WIPE_DOWN",
                    HpdfTransitionStyle.WipeDown, pages[2], pages[4]);
                PrintPage(pages[4], font, "BARN_DOORS_HORIZONTAL_OUT",
                    HpdfTransitionStyle.BarnDoorsHorizontalOut, pages[3], pages[5]);
                PrintPage(pages[5], font, "BARN_DOORS_HORIZONTAL_IN",
                    HpdfTransitionStyle.BarnDoorsHorizontalIn, pages[4], pages[6]);
                PrintPage(pages[6], font, "BARN_DOORS_VERTICAL_OUT",
                    HpdfTransitionStyle.BarnDoorsVerticalOut, pages[5], pages[7]);
                PrintPage(pages[7], font, "BARN_DOORS_VERTICAL_IN",
                    HpdfTransitionStyle.BarnDoorsVerticalIn, pages[6], pages[8]);
                PrintPage(pages[8], font, "BOX_OUT",
                    HpdfTransitionStyle.BoxOut, pages[7], pages[9]);
                PrintPage(pages[9], font, "BOX_IN",
                    HpdfTransitionStyle.BoxIn, pages[8], pages[10]);
                PrintPage(pages[10], font, "BLINDS_HORIZONTAL",
                    HpdfTransitionStyle.BlindsHorizontal, pages[9], pages[11]);
                PrintPage(pages[11], font, "BLINDS_VERTICAL",
                    HpdfTransitionStyle.BlindsVertical, pages[10], pages[12]);
                PrintPage(pages[12], font, "DISSOLVE",
                    HpdfTransitionStyle.Dissolve, pages[11], pages[13]);
                PrintPage(pages[13], font, "GLITTER_RIGHT",
                    HpdfTransitionStyle.GlitterRight, pages[12], pages[14]);
                PrintPage(pages[14], font, "GLITTER_DOWN",
                    HpdfTransitionStyle.GlitterDown, pages[13], pages[15]);
                PrintPage(pages[15], font, "GLITTER_TOP_LEFT_TO_BOTTOM_RIGHT",
                    HpdfTransitionStyle.GlitterTopLeftToBottomRight, pages[14], pages[16]);
                PrintPage(pages[16], font, "REPLACE",
                    HpdfTransitionStyle.Replace, pages[15], null);

                // Set full screen mode
                pdf.Catalog.SetPageMode(HpdfPageMode.FullScreen);

                // Save the document to a file
                pdf.SaveToFile("pdfs/SlideShowDemo.pdf");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Error in SlideShowDemo: {e.Message}");
                Console.Error.WriteLine($"Stack trace: {e.StackTrace}");
                throw;
            }
        }
    }
}
