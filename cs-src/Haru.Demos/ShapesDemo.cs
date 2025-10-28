/*
 * << Haru Free PDF Library >> -- ShapesDemo.cs
 *
 * Demonstrates high-level shape drawing: circles, ellipses, rectangles, and arcs
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
    public static class ShapesDemo
    {
        public static void Run()
        {
            try
            {
                var pdf = new HpdfDocument();

                // Enable compression
                pdf.SetCompressionMode(HpdfCompressionMode.All);

                // Create default font
                var font = new HpdfFont(pdf.Xref, HpdfStandardFont.Helvetica, "F1");
                var boldFont = new HpdfFont(pdf.Xref, HpdfStandardFont.HelveticaBold, "F2");

                // ===== PAGE 1: Basic Shapes =====
                CreateBasicShapesPage(pdf, font, boldFont);

                // ===== PAGE 2: Arcs =====
                CreateArcsPage(pdf, font, boldFont);

                // ===== PAGE 3: Combined Shapes =====
                CreateCombinedShapesPage(pdf, font, boldFont);

                pdf.SaveToFile("pdfs/ShapesDemo.pdf");
                Console.WriteLine("ShapesDemo completed successfully!");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Error in ShapesDemo: {e.Message}");
                Console.Error.WriteLine($"Stack trace: {e.StackTrace}");
                throw;
            }
        }

        private static void CreateBasicShapesPage(HpdfDocument pdf, HpdfFont font, HpdfFont boldFont)
        {
            var page = pdf.AddPage();
            float height = page.Height;
            float width = page.Width;

            // Title
            page.SetFontAndSize(boldFont, 24);
            page.BeginText();
            page.MoveTextPos(50, height - 50);
            page.ShowText("Basic Shapes");
            page.EndText();

            // Subtitle
            page.SetFontAndSize(font, 12);
            page.BeginText();
            page.MoveTextPos(50, height - 75);
            page.ShowText("Circles, Ellipses, and Rectangles with different styles");
            page.EndText();

            float y = height - 120;
            float x = 100;

            // ===== CIRCLES =====
            DrawLabel(page, font, 50, y, "Circles:");

            // Fill
            page.SetRgbFill(1.0f, 0.3f, 0.3f);
            page.Circle(x, y - 50, 25);
            page.Fill();
            DrawSmallLabel(page, font, x, y - 86, "Fill");

            // Stroke
            page.SetRgbStroke(0.2f, 0.6f, 1.0f);
            page.SetLineWidth(3);
            page.Circle(x + 100, y - 50, 25);
            page.Stroke();
            DrawSmallLabel(page, font, x + 100, y - 86, "Stroke");

            // Fill + Stroke
            page.SetRgbFill(1.0f, 0.9f, 0.3f);
            page.SetRgbStroke(1.0f, 0.5f, 0.0f);
            page.SetLineWidth(2);
            page.Circle(x + 200, y - 50, 25);
            page.FillStroke();
            DrawSmallLabel(page, font, x + 200, y - 86, "Fill+Stroke");

            y -= 130;

            // ===== ELLIPSES =====
            DrawLabel(page, font, 50, y, "Ellipses:");

            // Fill
            page.SetRgbFill(0.6f, 0.3f, 0.9f);
            page.Ellipse(x, y - 50, 40, 25);
            page.Fill();
            DrawSmallLabel(page, font, x, y - 86, "Fill");

            // Stroke
            page.SetRgbStroke(0.0f, 0.7f, 0.5f);
            page.SetLineWidth(3);
            page.Ellipse(x + 100, y - 50, 40, 25);
            page.Stroke();
            DrawSmallLabel(page, font, x + 100, y - 86, "Stroke");

            // Fill + Stroke
            page.SetRgbFill(1.0f, 0.7f, 0.8f);
            page.SetRgbStroke(0.9f, 0.2f, 0.5f);
            page.SetLineWidth(2);
            page.Ellipse(x + 200, y - 50, 40, 25);
            page.FillStroke();
            DrawSmallLabel(page, font, x + 200, y - 86, "Fill+Stroke");

            y -= 130;

            // ===== RECTANGLES =====
            DrawLabel(page, font, 50, y, "Rectangles:");

            // Fill
            page.SetRgbFill(0.3f, 0.8f, 0.3f);
            page.Rectangle(x - 25, y - 65, 50, 40);
            page.Fill();
            DrawSmallLabel(page, font, x, y - 76, "Fill");

            // Stroke
            page.SetRgbStroke(0.8f, 0.3f, 0.8f);
            page.SetLineWidth(3);
            page.Rectangle(x + 75, y - 65, 50, 40);
            page.Stroke();
            DrawSmallLabel(page, font, x + 100, y - 76, "Stroke");

            // Fill + Stroke
            page.SetRgbFill(0.5f, 0.7f, 1.0f);
            page.SetRgbStroke(0.1f, 0.3f, 0.8f);
            page.SetLineWidth(2);
            page.Rectangle(x + 175, y - 65, 50, 40);
            page.FillStroke();
            DrawSmallLabel(page, font, x + 200, y - 76, "Fill+Stroke");

            y -= 140;

            // ===== ASPECT RATIOS =====
            DrawLabel(page, font, 50, y, "Different Aspect Ratios:");

            // Circle (1:1)
            page.SetRgbFill(0.9f, 0.9f, 1.0f);
            page.SetRgbStroke(0.3f, 0.3f, 0.8f);
            page.SetLineWidth(2);
            page.Ellipse(x - 10, y - 50, 30, 30);
            page.FillStroke();
            DrawSmallLabel(page, font, x - 10, y - 91, "Circle 1:1");

            // Wide ellipse (3:1)
            page.SetRgbFill(1.0f, 0.9f, 0.9f);
            page.SetRgbStroke(0.8f, 0.3f, 0.3f);
            page.Ellipse(x + 90, y - 50, 50, 17);
            page.FillStroke();
            DrawSmallLabel(page, font, x + 90, y - 76, "Ellipse 3:1");

            // Tall ellipse (1:2)
            page.SetRgbFill(0.9f, 1.0f, 0.9f);
            page.SetRgbStroke(0.3f, 0.8f, 0.3f);
            page.Ellipse(x + 210, y - 60, 20, 40);
            page.FillStroke();
            DrawSmallLabel(page, font, x + 210, y - 111, "Ellipse 1:2");

            y -= 140;

            // ===== RECTANGLES WITH DIFFERENT PROPORTIONS =====
            DrawLabel(page, font, 50, y, "Rectangle Proportions:");

            // Square
            page.SetRgbFill(1.0f, 1.0f, 0.8f);
            page.SetRgbStroke(0.8f, 0.6f, 0.0f);
            page.SetLineWidth(2);
            page.Rectangle(x - 20, y - 65, 40, 40);
            page.FillStroke();
            DrawSmallLabel(page, font, x, y - 76, "Square");

            // Wide rectangle
            page.SetRgbFill(0.8f, 1.0f, 1.0f);
            page.SetRgbStroke(0.0f, 0.6f, 0.8f);
            page.Rectangle(x + 60, y - 50, 70, 25);
            page.FillStroke();
            DrawSmallLabel(page, font, x + 95, y - 61, "Wide");

            // Tall rectangle
            page.SetRgbFill(1.0f, 0.9f, 1.0f);
            page.SetRgbStroke(0.7f, 0.3f, 0.8f);
            page.Rectangle(x + 180, y - 70, 25, 55);
            page.FillStroke();
            DrawSmallLabel(page, font, x + 192, y - 81, "Tall");
        }

        private static void CreateArcsPage(HpdfDocument pdf, HpdfFont font, HpdfFont boldFont)
        {
            var page = pdf.AddPage();
            float height = page.Height;
            float width = page.Width;

            // Title
            page.SetFontAndSize(boldFont, 24);
            page.BeginText();
            page.MoveTextPos(50, height - 50);
            page.ShowText("Arcs Demo");
            page.EndText();

            // Subtitle
            page.SetFontAndSize(font, 12);
            page.BeginText();
            page.MoveTextPos(50, height - 75);
            page.ShowText("High-level arc drawing using Arc() method");
            page.EndText();

            float y = height - 130;
            float x = 100;
            float radius = 40;

            // Section 1: Different arc angles
            DrawLabel(page, font, 50, y, "Different Arc Angles:");

            page.SetRgbStroke(0.8f, 0.0f, 0.0f);
            page.SetLineWidth(2);

            // 90 degree arc
            page.Arc(x, y - 60, radius, 0, 90);
            page.Stroke();
            DrawSmallLabel(page, font, x, y - 116, "0° to 90°");

            // 180 degree arc
            page.Arc(x + 120, y - 60, radius, 0, 180);
            page.Stroke();
            DrawSmallLabel(page, font, x + 120, y - 116, "0° to 180°");

            // 270 degree arc
            page.Arc(x + 240, y - 60, radius, 0, 270);
            page.Stroke();
            DrawSmallLabel(page, font, x + 240, y - 116, "0° to 270°");

            // Full circle (almost)
            page.Arc(x + 360, y - 60, radius, 0, 350);
            page.Stroke();
            DrawSmallLabel(page, font, x + 360, y - 116, "0° to 350°");

            y -= 170;

            // Section 2: Different starting angles
            DrawLabel(page, font, 50, y, "Different Starting Angles:");

            page.SetRgbStroke(0.0f, 0.6f, 0.0f);

            page.Arc(x, y - 60, radius, 0, 120);
            page.Stroke();
            DrawSmallLabel(page, font, x, y - 116, "Start: 0°");

            page.Arc(x + 120, y - 60, radius, 45, 165);
            page.Stroke();
            DrawSmallLabel(page, font, x + 120, y - 116, "Start: 45°");

            page.Arc(x + 240, y - 60, radius, 90, 210);
            page.Stroke();
            DrawSmallLabel(page, font, x + 240, y - 116, "Start: 90°");

            page.Arc(x + 360, y - 60, radius, 180, 300);
            page.Stroke();
            DrawSmallLabel(page, font, x + 360, y - 116, "Start: 180°");

            y -= 170;

            // Section 3: Pie charts (filled arcs)
            DrawLabel(page, font, 50, y, "Pie Chart Segments (Filled Arcs):");

            float pieX = 150;
            float pieY = y - 100;
            float pieRadius = 60;

            // Draw pie segments
            page.SetRgbFill(1.0f, 0.8f, 0.8f);
            page.SetRgbStroke(0.5f, 0.0f, 0.0f);
            page.SetLineWidth(1);
            page.MoveTo(pieX, pieY);
            page.Arc(pieX, pieY, pieRadius, 0, 90);
            page.LineTo(pieX, pieY);
            page.FillStroke();

            page.SetRgbFill(0.8f, 1.0f, 0.8f);
            page.SetRgbStroke(0.0f, 0.5f, 0.0f);
            page.MoveTo(pieX, pieY);
            page.Arc(pieX, pieY, pieRadius, 90, 180);
            page.LineTo(pieX, pieY);
            page.FillStroke();

            page.SetRgbFill(0.8f, 0.8f, 1.0f);
            page.SetRgbStroke(0.0f, 0.0f, 0.5f);
            page.MoveTo(pieX, pieY);
            page.Arc(pieX, pieY, pieRadius, 180, 270);
            page.LineTo(pieX, pieY);
            page.FillStroke();

            page.SetRgbFill(1.0f, 1.0f, 0.8f);
            page.SetRgbStroke(0.5f, 0.5f, 0.0f);
            page.MoveTo(pieX, pieY);
            page.Arc(pieX, pieY, pieRadius, 270, 360);
            page.LineTo(pieX, pieY);
            page.FillStroke();

            DrawSmallLabel(page, font, pieX, y - 186, "Pie Chart");

            // Another pie chart with different segments
            pieX = 400;
            page.SetRgbFill(1.0f, 0.7f, 0.7f);
            page.SetRgbStroke(0.6f, 0.0f, 0.0f);
            page.MoveTo(pieX, pieY);
            page.Arc(pieX, pieY, pieRadius, 0, 120);
            page.LineTo(pieX, pieY);
            page.FillStroke();

            page.SetRgbFill(0.7f, 1.0f, 0.7f);
            page.SetRgbStroke(0.0f, 0.6f, 0.0f);
            page.MoveTo(pieX, pieY);
            page.Arc(pieX, pieY, pieRadius, 120, 240);
            page.LineTo(pieX, pieY);
            page.FillStroke();

            page.SetRgbFill(0.7f, 0.7f, 1.0f);
            page.SetRgbStroke(0.0f, 0.0f, 0.6f);
            page.MoveTo(pieX, pieY);
            page.Arc(pieX, pieY, pieRadius, 240, 360);
            page.LineTo(pieX, pieY);
            page.FillStroke();

            DrawSmallLabel(page, font, pieX, y - 186, "3-segment Pie");
        }

        private static void CreateCombinedShapesPage(HpdfDocument pdf, HpdfFont font, HpdfFont boldFont)
        {
            var page = pdf.AddPage();
            float height = page.Height;
            float width = page.Width;

            // Title
            page.SetFontAndSize(boldFont, 24);
            page.BeginText();
            page.MoveTextPos(50, height - 50);
            page.ShowText("Combined Shapes");
            page.EndText();

            // Subtitle
            page.SetFontAndSize(font, 12);
            page.BeginText();
            page.MoveTextPos(50, height - 75);
            page.ShowText("Complex figures using multiple shapes");
            page.EndText();

            float y = height - 130;

            // Section 1: Target
            DrawLabel(page, font, 50, y, "Target (Concentric Circles):");

            float targetX = 150;
            float targetY = y - 80;

            page.SetRgbFill(1.0f, 0.0f, 0.0f);
            page.Circle(targetX, targetY, 60);
            page.Fill();

            page.SetRgbFill(1.0f, 1.0f, 1.0f);
            page.Circle(targetX, targetY, 45);
            page.Fill();

            page.SetRgbFill(1.0f, 0.0f, 0.0f);
            page.Circle(targetX, targetY, 30);
            page.Fill();

            page.SetRgbFill(1.0f, 1.0f, 1.0f);
            page.Circle(targetX, targetY, 15);
            page.Fill();

            DrawSmallLabel(page, font, targetX, y - 156, "Bullseye Target");

            // Section 2: Flower
            DrawLabel(page, font, 300, y, "Flower (Overlapping Circles):");

            float flowerX = 420;
            float flowerY = y - 80;
            float petalRadius = 25;

            page.SetRgbFill(1.0f, 0.6f, 0.8f);
            page.SetRgbStroke(1.0f, 0.4f, 0.6f);
            page.SetLineWidth(1);

            // 6 petals
            for (int i = 0; i < 6; i++)
            {
                float angle = i * 60 * (float)Math.PI / 180;
                float petalX = flowerX + (float)Math.Cos(angle) * petalRadius;
                float petalY = flowerY + (float)Math.Sin(angle) * petalRadius;
                page.Circle(petalX, petalY, petalRadius);
                page.FillStroke();
            }

            // Center
            page.SetRgbFill(1.0f, 1.0f, 0.0f);
            page.SetRgbStroke(0.8f, 0.6f, 0.0f);
            page.Circle(flowerX, flowerY, 15);
            page.FillStroke();

            DrawSmallLabel(page, font, flowerX, y - 156, "Flower Pattern");

            y -= 200;

            // Section 3: Olympic Rings
            DrawLabel(page, font, 50, y, "Olympic Rings:");

            float ringX = 150;
            float ringY = y - 80;
            float ringRadius = 30;
            float ringSpacing = 70;

            page.SetLineWidth(5);

            // Top row: Blue, Black, Red
            page.SetRgbStroke(0.0f, 0.5f, 1.0f);
            page.Circle(ringX, ringY + 20, ringRadius);
            page.Stroke();

            page.SetRgbStroke(0.0f, 0.0f, 0.0f);
            page.Circle(ringX + ringSpacing, ringY + 20, ringRadius);
            page.Stroke();

            page.SetRgbStroke(1.0f, 0.0f, 0.0f);
            page.Circle(ringX + ringSpacing * 2, ringY + 20, ringRadius);
            page.Stroke();

            // Bottom row: Yellow, Green
            page.SetRgbStroke(1.0f, 0.8f, 0.0f);
            page.Circle(ringX + ringSpacing / 2, ringY - 15, ringRadius);
            page.Stroke();

            page.SetRgbStroke(0.0f, 0.7f, 0.0f);
            page.Circle(ringX + ringSpacing * 1.5f, ringY - 15, ringRadius);
            page.Stroke();

            DrawSmallLabel(page, font, ringX + ringSpacing, y - 156, "Olympic Rings");
        }

        private static void DrawLabel(HpdfPage page, HpdfFont font, float x, float y, string text)
        {
            page.SetFontAndSize(font, 11);
            page.SetRgbFill(0, 0, 0);
            page.BeginText();
            page.MoveTextPos(x, y);
            page.ShowText(text);
            page.EndText();
        }

        private static void DrawSmallLabel(HpdfPage page, HpdfFont font, float x, float y, string text)
        {
            page.SetFontAndSize(font, 8);
            page.SetRgbFill(0, 0, 0);
            page.BeginText();
            float textWidth = font.MeasureText(text, 8);
            page.MoveTextPos(x - textWidth / 2, y);
            page.ShowText(text);
            page.EndText();
        }
    }
}
