/*
 * << Haru Free PDF Library >> -- TextWrappingDemo.cs
 *
 * Demonstrates text wrapping functionality with HpdfFont.MeasureText
 *
 * Copyright (c) 1999-2025 Haru Free PDF Library
 */

using Haru.Doc;
using Haru.Font;

namespace BasicDemos
{
    public static class TextWrappingDemo
    {
        public static void Run()
        {
            Console.WriteLine("Running TextWrappingDemo...");

            var document = new HpdfDocument();
            var page = document.AddPage();
            page.Width = 595;  // A4 width
            page.Height = 842; // A4 height

            var font = document.GetFont("Helvetica");
            float fontSize = 12f;
            float maxWidth = 400f;

            // Sample text that's too long to fit in maxWidth
            string longText = "This is a demonstration of the text wrapping functionality. " +
                            "The MeasureText method can now calculate how many characters fit " +
                            "within a specified width, with support for word wrapping to break " +
                            "at word boundaries rather than mid-word.";

            // Position for drawing
            float x = 50;
            float y = page.Height - 50;
            float lineHeight = fontSize * 1.5f;

            // Draw a box showing the maximum width
            page.SetRgbStroke(0.8f, 0.8f, 0.8f);
            page.SetLineWidth(1);
            page.Rectangle(x, y - 200, maxWidth, 200);
            page.Stroke();

            // Process text with word wrapping
            page.BeginText();
            page.SetFontAndSize(font, fontSize);
            page.MoveTextPos(x, y);

            int offset = 0;
            int lineCount = 0;
            string remainingText = longText;

            while (remainingText.Length > 0 && lineCount < 15)
            {
                // Measure how much text fits with word wrap
                int charCount = font.MeasureText(
                    remainingText,
                    fontSize,
                    maxWidth,
                    charSpace: 0f,
                    wordSpace: 0f,
                    wordWrap: true,
                    out float actualWidth);

                if (charCount == 0)
                    break;

                // Extract the line to display
                string line = remainingText.Substring(0, charCount).TrimEnd();

                // Display the line
                page.ShowText(line);
                page.MoveTextPos(0, -lineHeight);

                // Move to next line
                remainingText = remainingText.Substring(charCount).TrimStart();
                offset += charCount;
                lineCount++;
            }

            page.EndText();

            // Add title and info
            page.BeginText();
            var titleFont = document.GetFont("Helvetica-Bold");
            page.SetFontAndSize(titleFont, 18);
            page.MoveTextPos(50, page.Height - 30);
            page.ShowText("Text Wrapping Demo");
            page.EndText();

            page.BeginText();
            page.SetFontAndSize(font, 10);
            page.MoveTextPos(50, y - 220);
            page.ShowText($"Maximum width: {maxWidth} points");
            page.MoveTextPos(0, -15);
            page.ShowText($"Font: Helvetica, Size: {fontSize}pt");
            page.MoveTextPos(0, -15);
            page.ShowText($"Lines rendered: {lineCount}");
            page.EndText();

            // Compare with non-word-wrap mode
            y = y - 300;

            page.BeginText();
            page.SetFontAndSize(titleFont, 14);
            page.MoveTextPos(50, y);
            page.ShowText("Without Word Wrap (breaks anywhere):");
            page.EndText();

            y -= 30;
            page.SetRgbStroke(0.8f, 0.8f, 0.8f);
            page.Rectangle(x, y - 100, maxWidth, 100);
            page.Stroke();

            page.BeginText();
            page.SetFontAndSize(font, fontSize);
            page.MoveTextPos(x, y);

            remainingText = "Thisisaverylongwordthatcannotbebrokenatnormalboundaries";
            lineCount = 0;

            while (remainingText.Length > 0 && lineCount < 5)
            {
                int charCount = font.MeasureText(
                    remainingText,
                    fontSize,
                    maxWidth,
                    charSpace: 0f,
                    wordSpace: 0f,
                    wordWrap: false,  // No word wrap - break anywhere
                    out float actualWidth);

                if (charCount == 0)
                    break;

                string line = remainingText.Substring(0, charCount);
                page.ShowText(line);
                page.MoveTextPos(0, -lineHeight);

                remainingText = remainingText.Substring(charCount);
                lineCount++;
            }

            page.EndText();

            // Save the PDF
            string outputPath = "pdfs/TextWrappingDemo.pdf";
            document.SaveToFile(outputPath);

            Console.WriteLine($"TextWrappingDemo created: {outputPath}");
            Console.WriteLine("Demonstrates word wrapping and character-by-character breaking.");
            Console.WriteLine("TextWrappingDemo completed.");
        }
    }
}
