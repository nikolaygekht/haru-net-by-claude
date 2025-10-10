/*
 * << Haru Free PDF Library >> -- LineDemo.cs
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
    public static class LineDemo
    {
        private static void DrawLine(HpdfPage page, float x, float y, string label)
        {
            page.BeginText();
            page.MoveTextPos(x, y - 10);
            page.ShowText(label);
            page.EndText();

            page.MoveTo(x, y - 15);
            page.LineTo(x + 220, y - 15);
            page.Stroke();
        }

        private static void DrawLine2(HpdfPage page, float x, float y, string label)
        {
            page.BeginText();
            page.MoveTextPos(x, y);
            page.ShowText(label);
            page.EndText();

            page.MoveTo(x + 30, y - 25);
            page.LineTo(x + 160, y - 25);
            page.Stroke();
        }

        private static void DrawRect(HpdfPage page, float x, float y, string label)
        {
            page.BeginText();
            page.MoveTextPos(x, y - 10);
            page.ShowText(label);
            page.EndText();

            page.Rectangle(x, y - 40, 220, 25);
        }

        public static void Run()
        {
            try
            {
                const string pageTitle = "LineDemo";

                var pdf = new HpdfDocument();

                // Create default-font
                var font = new HpdfFont(pdf.Xref, HpdfStandardFont.Helvetica, "F1");

                // Add a new page object
                var page = pdf.AddPage();

                // Print the lines of the page
                page.SetLineWidth(1);
                page.Rectangle(50, 50, page.Width - 100, page.Height - 110);
                page.Stroke();

                // Print the title of the page (with positioning center)
                page.SetFontAndSize(font, 24);
                float tw = font.MeasureText(pageTitle, 24);
                page.BeginText();
                page.MoveTextPos((page.Width - tw) / 2, page.Height - 50);
                page.ShowText(pageTitle);
                page.EndText();

                page.SetFontAndSize(font, 10);

                // Draw various widths of lines
                page.SetLineWidth(0);
                DrawLine(page, 60, 770, "line width = 0");

                page.SetLineWidth(1.0f);
                DrawLine(page, 60, 740, "line width = 1.0");

                page.SetLineWidth(2.0f);
                DrawLine(page, 60, 710, "line width = 2.0");

                // Line dash pattern
                ushort[] dashMode1 = new ushort[] { 3 };
                ushort[] dashMode2 = new ushort[] { 3, 7 };
                ushort[] dashMode3 = new ushort[] { 8, 7, 2, 7 };

                page.SetLineWidth(1.0f);

                page.SetDash(dashMode1, 1);
                DrawLine(page, 60, 680, "dash_ptn=[3], phase=1 -- 2 on, 3 off, 3 on...");

                page.SetDash(dashMode2, 2);
                DrawLine(page, 60, 650, "dash_ptn=[7, 3], phase=2 -- 5 on 3 off, 7 on,...");

                page.SetDash(dashMode3, 0);
                DrawLine(page, 60, 620, "dash_ptn=[8, 7, 2, 7], phase=0");

                page.SetDash(new ushort[0], 0);

                page.SetLineWidth(30);
                page.SetRgbStroke(0.0f, 0.5f, 0.0f);

                // Line Cap Style
                page.SetLineCap(HpdfLineCap.ButtEnd);
                DrawLine2(page, 60, 570, "HPDF_BUTT_END");

                page.SetLineCap(HpdfLineCap.RoundEnd);
                DrawLine2(page, 60, 505, "HPDF_ROUND_END");

                page.SetLineCap(HpdfLineCap.ProjectingSquareEnd);
                DrawLine2(page, 60, 440, "HPDF_PROJECTING_SQUARE_END");

                // Line Join Style
                page.SetLineWidth(30);
                page.SetRgbStroke(0.0f, 0.0f, 0.5f);

                page.SetLineJoin(HpdfLineJoin.MiterJoin);
                page.MoveTo(120, 300);
                page.LineTo(160, 340);
                page.LineTo(200, 300);
                page.Stroke();

                page.BeginText();
                page.MoveTextPos(60, 360);
                page.ShowText("HPDF_MITER_JOIN");
                page.EndText();

                page.SetLineJoin(HpdfLineJoin.RoundJoin);
                page.MoveTo(120, 195);
                page.LineTo(160, 235);
                page.LineTo(200, 195);
                page.Stroke();

                page.BeginText();
                page.MoveTextPos(60, 255);
                page.ShowText("HPDF_ROUND_JOIN");
                page.EndText();

                page.SetLineJoin(HpdfLineJoin.BevelJoin);
                page.MoveTo(120, 90);
                page.LineTo(160, 130);
                page.LineTo(200, 90);
                page.Stroke();

                page.BeginText();
                page.MoveTextPos(60, 150);
                page.ShowText("HPDF_BEVEL_JOIN");
                page.EndText();

                // Draw Rectangle
                page.SetLineWidth(2);
                page.SetRgbStroke(0, 0, 0);
                page.SetRgbFill(0.75f, 0.0f, 0.0f);

                DrawRect(page, 300, 770, "Stroke");
                page.Stroke();

                DrawRect(page, 300, 720, "Fill");
                page.Fill();

                DrawRect(page, 300, 670, "Fill then Stroke");
                page.FillStroke();

                // Clip Rect
                page.GSave();  // Save the current graphic state
                DrawRect(page, 300, 620, "Clip Rectangle");
                page.Clip();
                page.Stroke();
                page.SetFontAndSize(font, 13);

                page.BeginText();
                page.MoveTextPos(290, 600);
                page.SetTextLeading(12);
                page.ShowText("Clip Clip Clip Clip Clip Clipi Clip Clip Clip");
                page.ShowTextNextLine("Clip Clip Clip Clip Clip Clip Clip Clip Clip");
                page.ShowTextNextLine("Clip Clip Clip Clip Clip Clip Clip Clip Clip");
                page.EndText();
                page.GRestore();

                // Curve Example(CurveTo2)
                float x = 330;
                float y = 440;
                float x1 = 430;
                float y1 = 530;
                float x2 = 480;
                float y2 = 470;
                float x3 = 480;
                float y3 = 90;

                page.SetRgbFill(0, 0, 0);

                page.BeginText();
                page.MoveTextPos(300, 540);
                page.ShowText("CurveTo2(x1, y1, x2. y2)");
                page.EndText();

                page.BeginText();
                page.MoveTextPos(x + 5, y - 5);
                page.ShowText("Current point");
                page.MoveTextPos(x1 - x, y1 - y);
                page.ShowText("(x1, y1)");
                page.MoveTextPos(x2 - x1, y2 - y1);
                page.ShowText("(x2, y2)");
                page.EndText();

                page.SetDash(dashMode1, 0);

                page.SetLineWidth(0.5f);
                page.MoveTo(x1, y1);
                page.LineTo(x2, y2);
                page.Stroke();

                page.SetDash(new ushort[0], 0);

                page.SetLineWidth(1.5f);

                page.MoveTo(x, y);
                page.CurveTo2(x1, y1, x2, y2);
                page.Stroke();

                // Curve Example(CurveTo3)
                y -= 150;
                y1 -= 150;
                y2 -= 150;

                page.BeginText();
                page.MoveTextPos(300, 390);
                page.ShowText("CurveTo3(x1, y1, x2. y2)");
                page.EndText();

                page.BeginText();
                page.MoveTextPos(x + 5, y - 5);
                page.ShowText("Current point");
                page.MoveTextPos(x1 - x, y1 - y);
                page.ShowText("(x1, y1)");
                page.MoveTextPos(x2 - x1, y2 - y1);
                page.ShowText("(x2, y2)");
                page.EndText();

                page.SetDash(dashMode1, 0);

                page.SetLineWidth(0.5f);
                page.MoveTo(x, y);
                page.LineTo(x1, y1);
                page.Stroke();

                page.SetDash(new ushort[0], 0);

                page.SetLineWidth(1.5f);
                page.MoveTo(x, y);
                page.CurveTo3(x1, y1, x2, y2);
                page.Stroke();

                // Curve Example(CurveTo)
                y -= 150;
                y1 -= 160;
                y2 -= 130;
                x2 += 10;

                page.BeginText();
                page.MoveTextPos(300, 240);
                page.ShowText("CurveTo(x1, y1, x2. y2, x3, y3)");
                page.EndText();

                page.BeginText();
                page.MoveTextPos(x + 5, y - 5);
                page.ShowText("Current point");
                page.MoveTextPos(x1 - x, y1 - y);
                page.ShowText("(x1, y1)");
                page.MoveTextPos(x2 - x1, y2 - y1);
                page.ShowText("(x2, y2)");
                page.MoveTextPos(x3 - x2, y3 - y2);
                page.ShowText("(x3, y3)");
                page.EndText();

                page.SetDash(dashMode1, 0);

                page.SetLineWidth(0.5f);
                page.MoveTo(x, y);
                page.LineTo(x1, y1);
                page.Stroke();
                page.MoveTo(x2, y2);
                page.LineTo(x3, y3);
                page.Stroke();

                page.SetDash(new ushort[0], 0);

                page.SetLineWidth(1.5f);
                page.MoveTo(x, y);
                page.CurveTo(x1, y1, x2, y2, x3, y3);
                page.Stroke();

                pdf.SaveToFile("pdfs/LineDemo.pdf");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Error in LineDemo: {e.Message}");
                throw;
            }
        }
    }
}
