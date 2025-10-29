/*
 * << Haru Free PDF Library >> -- TransparencyDemo.cs
 *
 * Demonstrates alpha transparency and blend modes using Extended Graphics State
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
    public static class TransparencyDemo
    {
        // Global opaque state for text rendering
        private static HpdfExtGState? _opaqueState;

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

                // Create single opaque state for all text
                _opaqueState = new HpdfExtGState(pdf.Xref, "GS_Opaque_Text");
                _opaqueState.SetAlphaFill(1.0f);
                _opaqueState.SetAlphaStroke(1.0f);

                // ===== PAGE 1: Alpha Transparency =====
                CreateAlphaTransparencyPage(pdf, font, boldFont);

                // ===== PAGE 2: Blend Modes =====
                CreateBlendModesPage(pdf, font, boldFont);

                // ===== PAGE 3: Practical Examples =====
                CreatePracticalExamplesPage(pdf, font, boldFont);

                pdf.SaveToFile("pdfs/TransparencyDemo.pdf");
                Console.WriteLine("TransparencyDemo completed successfully!");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Error in TransparencyDemo: {e.Message}");
                Console.Error.WriteLine($"Stack trace: {e.StackTrace}");
                throw;
            }
        }

        private static void CreateAlphaTransparencyPage(HpdfDocument pdf, HpdfFont font, HpdfFont boldFont)
        {
            var page = pdf.AddPage();
            float height = page.Height;
            float width = page.Width;

            // Title
            DrawLabel(page, boldFont, 50, height - 50, "Alpha Transparency", 24);

            // Subtitle
            DrawLabel(page, font, 50, height - 75, "Controlling opacity with Extended Graphics State", 12);

            float y = height - 120;

            // Section 1: Fill Alpha
            DrawLabel(page, font, 50, y, "Fill Alpha (Shape Opacity):");

            float x = 80;
            float[] alphas = { 1.0f, 0.75f, 0.5f, 0.25f, 0.1f };
            string[] labels = { "100%", "75%", "50%", "25%", "10%" };

            for (int i = 0; i < alphas.Length; i++)
            {
                // Create ExtGState with alpha
                var gstate = new HpdfExtGState(pdf.Xref, $"GS_Fill_{i}");
                gstate.SetAlphaFill(alphas[i]);
                page.SetExtGState(gstate);

                // Draw circle
                page.SetRgbFill(1.0f, 0.0f, 0.0f);
                page.Circle(x + i * 90, y - 60, 30);
                page.Fill();

                // Label (will reset to opaque inside)
                DrawSmallLabel(page, font, x + i * 90, y - 106, labels[i]);
            }

            y -= 160;

            // Section 2: Stroke Alpha
            DrawLabel(page, font, 50, y, "Stroke Alpha (Outline Opacity):");

            for (int i = 0; i < alphas.Length; i++)
            {
                var gstate = new HpdfExtGState(pdf.Xref, $"GS_Stroke_{i}");
                gstate.SetAlphaStroke(alphas[i]);
                page.SetExtGState(gstate);

                page.SetRgbStroke(0.0f, 0.0f, 1.0f);
                page.SetLineWidth(6);
                page.Circle(x + i * 90, y - 60, 30);
                page.Stroke();

                DrawSmallLabel(page, font, x + i * 90, y - 106, labels[i]);
            }

            y -= 160;

            // Section 3: Overlapping Shapes
            DrawLabel(page, font, 50, y, "Overlapping Shapes:");

            // Without transparency
            DrawLabel(page, font, 60, y - 20, "Without transparency:", 10);

            var opaqueState = new HpdfExtGState(pdf.Xref, "GS_Opaque_1");
            opaqueState.SetAlphaFill(1.0f);
            page.SetExtGState(opaqueState);

            page.SetRgbFill(1.0f, 0.0f, 0.0f);
            page.Circle(100, y - 80, 35);
            page.Fill();

            page.SetRgbFill(0.0f, 0.0f, 1.0f);
            page.Circle(130, y - 80, 35);
            page.Fill();

            // With transparency
            DrawLabel(page, font, 240, y - 20, "With 50% transparency:", 10);

            var halfAlpha = new HpdfExtGState(pdf.Xref, "GS_Half");
            halfAlpha.SetAlphaFill(0.5f);
            page.SetExtGState(halfAlpha);

            page.SetRgbFill(1.0f, 0.0f, 0.0f);
            page.Circle(280, y - 80, 35);
            page.Fill();

            page.SetRgbFill(0.0f, 0.0f, 1.0f);
            page.Circle(310, y - 80, 35);
            page.Fill();

            DrawSmallLabel(page, font, 115, y - 136, "Opaque (second hides first)");
            DrawSmallLabel(page, font, 295, y - 136, "Colors blend");

            y -= 180;

            // Section 4: Transparent Text
            DrawLabel(page, font, 50, y, "Transparent Text:");

            // Opaque text
            DrawLabel(page, font, 60, y - 30, "Normal opaque text", 14);

            // 50% transparent text
            var textAlpha50 = new HpdfExtGState(pdf.Xref, "GS_Text_50");
            textAlpha50.SetAlphaFill(0.5f);
            page.SetExtGState(textAlpha50);

            page.SetRgbFill(0.0f, 0.0f, 0.0f);
            page.BeginText();
            page.SetFontAndSize(font, 14);
            page.MoveTextPos(60, y - 50);
            page.ShowText("50% transparent text");
            page.EndText();

            // 25% transparent text
            var textAlpha25 = new HpdfExtGState(pdf.Xref, "GS_Text_25");
            textAlpha25.SetAlphaFill(0.25f);
            page.SetExtGState(textAlpha25);

            page.BeginText();
            page.MoveTextPos(60, y - 70);
            page.ShowText("25% transparent text");
            page.EndText();

            y -= 120;

            // Section 5: Gradual transparency
            DrawLabel(page, font, 50, y, "Transparency Gradient:");

            float barX = 80;
            float barWidth = 400;
            float barHeight = 40;
            int steps = 20;

            for (int i = 0; i < steps; i++)
            {
                float alpha = 1.0f - (i / (float)steps);
                var gradState = new HpdfExtGState(pdf.Xref, $"GS_Grad_{i}");
                gradState.SetAlphaFill(alpha);
                page.SetExtGState(gradState);

                page.SetRgbFill(0.2f, 0.6f, 0.9f);
                page.Rectangle(barX + i * (barWidth / steps), y - 60, barWidth / steps + 1, barHeight);
                page.Fill();
            }

            DrawLabel(page, font, barX, y - 80, "100% opaque", 9);
            DrawLabel(page, font, barX + barWidth - 60, y - 80, "0% opaque", 9);
        }

        private static void CreateBlendModesPage(HpdfDocument pdf, HpdfFont font, HpdfFont boldFont)
        {
            var page = pdf.AddPage();
            float height = page.Height;
            float width = page.Width;

            // Title
            DrawLabel(page, boldFont, 50, height - 50, "Blend Modes", 24);

            // Subtitle
            DrawLabel(page, font, 50, height - 75, "Different ways colors combine when overlapping", 12);

            float y = height - 130;
            float x = 60;

            // Create base circles (background)
            void DrawBlendExample(string modeName, HpdfBlendMode mode, float posX, float posY)
            {
                // Background circles (opaque)
                var opaqueState = new HpdfExtGState(pdf.Xref, $"GS_BG_{modeName}");
                opaqueState.SetAlphaFill(1.0f);
                page.SetExtGState(opaqueState);

                // Red circle (left)
                page.SetRgbFill(1.0f, 0.0f, 0.0f);
                page.Circle(posX, posY, 25);
                page.Fill();

                // Blue circle (right)
                page.SetRgbFill(0.0f, 0.0f, 1.0f);
                page.Circle(posX + 30, posY, 25);
                page.Fill();

                // Overlapping circle with blend mode (green)
                var blendState = new HpdfExtGState(pdf.Xref, $"GS_Blend_{modeName}");
                blendState.SetAlphaFill(0.75f);
                blendState.SetBlendMode(mode);
                page.SetExtGState(blendState);

                page.SetRgbFill(0.0f, 1.0f, 0.0f);
                page.Circle(posX + 15, posY + 15, 25);
                page.Fill();

                // Label
                DrawSmallLabel(page, font, posX + 15, posY - 36, modeName);
            }

            // Row 1
            DrawBlendExample("Normal", HpdfBlendMode.Normal, x, y);
            DrawBlendExample("Multiply", HpdfBlendMode.Multiply, x + 120, y);
            DrawBlendExample("Screen", HpdfBlendMode.Screen, x + 240, y);
            DrawBlendExample("Overlay", HpdfBlendMode.Overlay, x + 360, y);

            y -= 110;

            // Row 2
            DrawBlendExample("Darken", HpdfBlendMode.Darken, x, y);
            DrawBlendExample("Lighten", HpdfBlendMode.Lighten, x + 120, y);
            DrawBlendExample("ColorDodge", HpdfBlendMode.ColorDodge, x + 240, y);
            DrawBlendExample("ColorBurn", HpdfBlendMode.ColorBurn, x + 360, y);

            y -= 110;

            // Row 3
            DrawBlendExample("HardLight", HpdfBlendMode.HardLight, x, y);
            DrawBlendExample("SoftLight", HpdfBlendMode.SoftLight, x + 120, y);
            DrawBlendExample("Difference", HpdfBlendMode.Difference, x + 240, y);
            DrawBlendExample("Exclusion", HpdfBlendMode.Exclusion, x + 360, y);

            // Information box
            y -= 120;

            // Reset to opaque for drawing box
            var boxState = new HpdfExtGState(pdf.Xref, "GS_Box");
            boxState.SetAlphaStroke(1.0f);
            page.SetExtGState(boxState);

            page.SetRgbStroke(0.5f, 0.5f, 0.5f);
            page.SetLineWidth(1);
            page.Rectangle(50, y - 80, width - 100, 70);
            page.Stroke();

            DrawLabel(page, font, 60, y - 30, "Blend modes control how overlapping colors interact:", 10);
            DrawLabel(page, font, 60, y - 45, "• Multiply/Darken: Make colors darker  • Screen/Lighten: Make colors lighter", 10);
            DrawLabel(page, font, 60, y - 60, "• Overlay/HardLight: Increase contrast  • Difference/Exclusion: Create artistic effects", 10);
        }

        private static void CreatePracticalExamplesPage(HpdfDocument pdf, HpdfFont font, HpdfFont boldFont)
        {
            var page = pdf.AddPage();
            float height = page.Height;
            float width = page.Width;

            // Title
            DrawLabel(page, boldFont, 50, height - 50, "Practical Examples", 24);

            // Subtitle
            DrawLabel(page, font, 50, height - 75, "Real-world uses of transparency", 12);

            float y = height - 120;

            // Example 1: Watermark
            DrawLabel(page, font, 50, y, "Watermark:");

            // Document content (simulated)
            var opaqueState = new HpdfExtGState(pdf.Xref, "GS_Opaque_Content");
            opaqueState.SetAlphaFill(1.0f);
            page.SetExtGState(opaqueState);

            page.SetRgbFill(0.95f, 0.95f, 0.95f);
            page.Rectangle(70, y - 120, 200, 100);
            page.Fill();

            DrawLabel(page, font, 80, y - 35, "This is a sample document", 10);
            DrawLabel(page, font, 80, y - 50, "with important content that", 10);
            DrawLabel(page, font, 80, y - 65, "needs to be protected with", 10);
            DrawLabel(page, font, 80, y - 80, "a watermark overlay.", 10);

            // Watermark with transparency
            var watermarkState = new HpdfExtGState(pdf.Xref, "GS_Watermark");
            watermarkState.SetAlphaFill(0.3f);
            watermarkState.SetAlphaStroke(0.3f);
            page.SetExtGState(watermarkState);

            page.GSave();
            page.Concat(0.707f, 0.707f, -0.707f, 0.707f, 170, y - 100);
            page.SetRgbFill(1.0f, 0.0f, 0.0f);
            page.BeginText();
            page.SetFontAndSize(boldFont, 36);
            page.ShowText("DRAFT");
            page.EndText();
            page.GRestore();

            DrawLabel(page, font, 75, y - 130, "30% alpha allows reading text through watermark", 9);

            // Example 2: Highlight overlay
            DrawLabel(page, font, 310, y, "Highlight Overlay:");

            // Text
            DrawLabel(page, font, 330, y - 35, "Important paragraph that", 11);
            DrawLabel(page, font, 330, y - 50, "needs to be highlighted", 11);
            DrawLabel(page, font, 330, y - 65, "with a semi-transparent", 11);
            DrawLabel(page, font, 330, y - 80, "yellow background.", 11);

            // Yellow highlight
            var highlightState = new HpdfExtGState(pdf.Xref, "GS_Highlight");
            highlightState.SetAlphaFill(0.4f);
            page.SetExtGState(highlightState);

            page.SetRgbFill(1.0f, 1.0f, 0.0f);
            page.Rectangle(325, y - 85, 165, 65);
            page.Fill();

            y -= 160;

            // Example 3: Shadow effect
            DrawLabel(page, font, 50, y, "Shadow Effect:");
            DrawLabel(page, font, 55, y - 15, "(Black rectangle at 50% alpha creates depth)", 9);

            // Shadow (transparent black) - drawn first, appears behind
            var shadowState = new HpdfExtGState(pdf.Xref, "GS_Shadow");
            shadowState.SetAlphaFill(0.5f);
            page.SetExtGState(shadowState);

            page.SetRgbFill(0.0f, 0.0f, 0.0f);
            page.Rectangle(135, y - 100, 80, 50);
            page.Fill();

            // Main rectangle (opaque) - drawn second, appears on top
            page.SetExtGState(opaqueState);
            page.SetRgbFill(0.2f, 0.6f, 1.0f);
            page.SetRgbStroke(0.1f, 0.3f, 0.6f);
            page.SetLineWidth(2);
            page.Rectangle(130, y - 95, 80, 50);
            page.FillStroke();

            // Button text
            DrawLabel(page, boldFont, 145, y - 78, "Button", 14, 1.0f, 1.0f, 1.0f);

            DrawLabel(page, font, 125, y - 120, "Shadow offset", 8);

            // Example 4: Layered transparency
            DrawLabel(page, font, 310, y, "Layered Transparency:");
            DrawLabel(page, font, 315, y - 15, "(White circles at different alpha on blue)", 9);

            float glassX = 390;
            float glassY = y - 100;
            float glassSize = 60;

            // Base layer (opaque blue)
            page.SetExtGState(opaqueState);
            page.SetRgbFill(0.1f, 0.3f, 0.7f);
            page.Rectangle(glassX, glassY, glassSize, glassSize);
            page.Fill();

            // Highlight layer (top, 60% transparent white)
            var shineState = new HpdfExtGState(pdf.Xref, "GS_Shine");
            shineState.SetAlphaFill(0.6f);
            page.SetExtGState(shineState);

            page.SetRgbFill(1.0f, 1.0f, 1.0f);
            page.Circle(glassX + 18, glassY + glassSize - 18, 18);
            page.Fill();

            // Medium layer (30% transparent white)
            var midState = new HpdfExtGState(pdf.Xref, "GS_Mid");
            midState.SetAlphaFill(0.3f);
            page.SetExtGState(midState);

            page.Circle(glassX + glassSize - 15, glassY + 15, 12);
            page.Fill();

            // Reflection bar (top, 20% transparent white)
            var reflectState = new HpdfExtGState(pdf.Xref, "GS_Reflect");
            reflectState.SetAlphaFill(0.2f);
            page.SetExtGState(reflectState);

            page.Rectangle(glassX + 5, glassY + glassSize - 18, glassSize - 10, 10);
            page.Fill();

            DrawLabel(page, font, glassX + 13, glassY - 10, "Layers blend", 8);

            y -= 140;

            // Information box
            page.SetExtGState(opaqueState);
            page.SetRgbStroke(0.5f, 0.5f, 0.5f);
            page.SetLineWidth(1);
            page.Rectangle(50, y - 80, width - 100, 70);
            page.Stroke();

            DrawLabel(page, font, 60, y - 30, "Common transparency applications:", 10);
            DrawLabel(page, font, 60, y - 45, "• Watermarks: Identify document status without obscuring content", 10);
            DrawLabel(page, font, 60, y - 60, "• Highlights: Emphasize important text while keeping it readable", 10);
            DrawLabel(page, font, 60, y - 75, "• Shadows & Glass: Create depth and modern UI effects", 10);
        }

        private static void DrawLabel(HpdfPage page, HpdfFont font, float x, float y, string text,
            float size = 11, float r = 0, float g = 0, float b = 0)
        {
            // Always reset to opaque for text
            if (_opaqueState != null)
                page.SetExtGState(_opaqueState);

            page.SetFontAndSize(font, size);
            page.SetRgbFill(r, g, b);
            page.BeginText();
            page.MoveTextPos(x, y);
            page.ShowText(text);
            page.EndText();
        }

        private static void DrawSmallLabel(HpdfPage page, HpdfFont font, float x, float y, string text)
        {
            // Always reset to opaque for text
            if (_opaqueState != null)
                page.SetExtGState(_opaqueState);

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
