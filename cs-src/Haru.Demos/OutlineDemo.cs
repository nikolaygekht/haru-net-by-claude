/*
 * << Haru Free PDF Library >> -- OutlineDemo.cs
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
    public static class OutlineDemo
    {
        private static void PrintPage(HpdfPage page, int pageNum)
        {
            page.Width = 800;
            page.Height = 800;

            page.BeginText();
            page.MoveTextPos(30, 740);
            string buf = $"Page:{pageNum}";
            page.ShowText(buf);
            page.EndText();
        }

        public static void Run()
        {
            try
            {
                var pdf = new HpdfDocument();

                // Create default-font
                var font = new HpdfFont(pdf.Xref, HpdfStandardFont.Helvetica, "F1");

                pdf.Catalog.SetPageMode(HpdfPageMode.UseOutlines);

                // Add 3 pages to the document
                var page0 = pdf.AddPage();
                page0.SetFontAndSize(font, 30);
                PrintPage(page0, 1);

                var page1 = pdf.AddPage();
                page1.SetFontAndSize(font, 30);
                PrintPage(page1, 2);

                var page2 = pdf.AddPage();
                page2.SetFontAndSize(font, 30);
                PrintPage(page2, 3);

                // Create outline root
                var root = pdf.GetOutlineRoot();
                root.SetOpened(true);

                var outline0 = root.CreateChild("page1");
                var outline1 = root.CreateChild("page2");
                var outline2 = root.CreateChild("page3");

                // Create destination objects on each pages and link it to outline items
                var dst = new HpdfDestination(page0);
                dst.SetXYZ(0, page0.Height, 1);
                outline0.SetDestination(dst);

                dst = new HpdfDestination(page1);
                dst.SetXYZ(0, page1.Height, 1);
                outline1.SetDestination(dst);

                dst = new HpdfDestination(page2);
                dst.SetXYZ(0, page2.Height, 1);
                outline2.SetDestination(dst);

                // Save the document to a file
                pdf.SaveToFile("pdfs/OutlineDemo.pdf");

            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Error in OutlineDemo: {e.Message}");
                Console.Error.WriteLine($"Stack trace: {e.StackTrace}");
                throw;
            }
        }
    }
}
