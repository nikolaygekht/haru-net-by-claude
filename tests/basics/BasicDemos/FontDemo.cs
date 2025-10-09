/*
 * << Haru Free PDF Library >> -- FontDemo.cs
 *
 * Adapted from original C demo to use the new C# Haru library
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
    public static class FontDemo
    {
        public static void Run()
        {
            string[] fontList = new string[] {
                "Courier",
                "Courier-Bold",
                "Courier-Oblique",
                "Courier-BoldOblique",
                "Helvetica",
                "Helvetica-Bold",
                "Helvetica-Oblique",
                "Helvetica-BoldOblique",
                "Times-Roman",
                "Times-Bold",
                "Times-Italic",
                "Times-BoldItalic",
                "Symbol",
                "ZapfDingbats"
            };

            HpdfStandardFont[] stdFonts = new HpdfStandardFont[] {
                HpdfStandardFont.Courier,
                HpdfStandardFont.CourierBold,
                HpdfStandardFont.CourierOblique,
                HpdfStandardFont.CourierBoldOblique,
                HpdfStandardFont.Helvetica,
                HpdfStandardFont.HelveticaBold,
                HpdfStandardFont.HelveticaOblique,
                HpdfStandardFont.HelveticaBoldOblique,
                HpdfStandardFont.TimesRoman,
                HpdfStandardFont.TimesBold,
                HpdfStandardFont.TimesItalic,
                HpdfStandardFont.TimesBoldItalic,
                HpdfStandardFont.Symbol,
                HpdfStandardFont.ZapfDingbats
            };

            try
            {
                const string pageTitle = "FontDemo";

                var pdf = new HpdfDocument();
                var page = pdf.AddPage();

                float height = page.Height;
                float width = page.Width;

                // Print the lines of the page
                page.SetLineWidth(1);
                page.Rectangle(50, 50, width - 100, height - 110);
                page.Stroke();

                // Print the title of the page (with positioning center)
                var defFont = new HpdfFont(pdf.Xref, HpdfStandardFont.Helvetica, "F0");
                page.SetFontAndSize(defFont, 24);

                float tw = defFont.MeasureText(pageTitle, 24);
                page.BeginText();
                page.MoveTextPos((width - tw) / 2, height - 50);
                page.ShowText(pageTitle);
                page.EndText();

                // Output subtitle
                page.BeginText();
                page.SetFontAndSize(defFont, 16);
                page.MoveTextPos(60, height - 80);
                page.ShowText("<Standard Type1 fonts samples>");
                page.EndText();

                page.BeginText();
                page.MoveTextPos(60, height - 105);

                for (int i = 0; i < fontList.Length; i++)
                {
                    const string sampText = "abcdefgABCDEFG12345!#$%&+-@?";
                    var font = new HpdfFont(pdf.Xref, stdFonts[i], $"F{i + 1}");

                    // Print a label of text
                    page.SetFontAndSize(defFont, 9);
                    page.ShowText(fontList[i]);
                    page.MoveTextPos(0, -18);

                    // Print a sample text
                    page.SetFontAndSize(font, 20);
                    page.ShowText(sampText);
                    page.MoveTextPos(0, -20);
                }

                page.EndText();

                pdf.SaveToFile("FontDemo.pdf");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Error in FontDemo: {e.Message}");
                throw;
            }
        }
    }
}
