/*
 * << Haru Free PDF Library >> -- InternationalDemo.cs
 *
 * Demonstrates international text rendering using embedded Noto fonts with various code pages
 *
 * Note: This demo uses simple TrueType fonts with single-byte encodings.
 * CJK languages (Chinese, Japanese, Korean) require multi-byte encodings and CID fonts,
 * which are not yet implemented. For now, we demonstrate Western European, Cyrillic, and Greek scripts.
 *
 * Copyright (c) 1999-2025 Haru Free PDF Library
 *
 * Permission to use, copy, modify, distribute and sell this software
 * and its documentation for any purpose is hereby granted without fee,
 */

using System;
using System.Text;
using Haru.Doc;
using Haru.Font;
using Haru.Types;

namespace BasicDemos
{
    public static class InternationalDemo
    {
        public static void Run()
        {
            try
            {
                var pdf = new HpdfDocument();

                // Load Noto fonts with different code pages
                // Noto Sans supports Latin, Cyrillic, Greek and many other scripts
                // Note: We use single-byte encodings (CP1252, CP1251, CP1253)
                var notoLatin = HpdfTrueTypeFont.LoadFromFile(
                    pdf.Xref,
                    "NotoLatin",
                    "demo/ttfont/noto.ttf",
                    true,
                    1252  // CP1252 - Western European (Latin)
                );

                var notoCyrillic = HpdfTrueTypeFont.LoadFromFile(
                    pdf.Xref,
                    "NotoCyrillic",
                    "demo/ttfont/noto.ttf",
                    true,
                    1251  // CP1251 - Cyrillic
                );

                var notoGreek = HpdfTrueTypeFont.LoadFromFile(
                    pdf.Xref,
                    "NotoGreek",
                    "demo/ttfont/noto.ttf",
                    true,
                    1253  // CP1253 - Greek
                );

                // Turkish uses Latin script with additional characters
                var notoTurkish = HpdfTrueTypeFont.LoadFromFile(
                    pdf.Xref,
                    "NotoTurkish",
                    "demo/ttfont/noto.ttf",
                    true,
                    1254  // CP1254 - Turkish
                );

                // Create Page 1: "Hello" in multiple languages
                CreatePage1_HelloWorld(pdf, notoLatin, notoCyrillic, notoGreek, notoTurkish);

                // Create Page 2: Cyrillic alphabet test
                CreatePage2_CyrillicAlphabet(pdf, notoLatin, notoCyrillic);

                pdf.SaveToFile("InternationalDemo.pdf");

                Console.WriteLine("InternationalDemo.pdf created successfully!");
                Console.WriteLine("Page 1: Greetings in 7 languages with special characters");
                Console.WriteLine("  - English, French (é, à, ç), German (ü, ö, ä, ß), Portuguese (á, ã, õ)");
                Console.WriteLine("  - Russian (Cyrillic), Greek, Turkish (ğ, ı, ş)");
                Console.WriteLine("Page 2: Cyrillic alphabet demonstration");
                Console.WriteLine();
                Console.WriteLine("Note: CJK (Chinese, Japanese, Korean) require CID fonts (future feature)");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Error in InternationalDemo: {e.Message}");
                Console.Error.WriteLine($"Stack trace: {e.StackTrace}");
                throw;
            }
        }

        private static void CreatePage1_HelloWorld(
            HpdfDocument pdf,
            HpdfTrueTypeFont notoLatin,
            HpdfTrueTypeFont notoCyrillic,
            HpdfTrueTypeFont notoGreek,
            HpdfTrueTypeFont notoTurkish)
        {
            var page = pdf.AddPage();
            float height = page.Height;
            float width = page.Width;

            // Print the border of the page
            page.SetLineWidth(1);
            page.Rectangle(50, 50, width - 100, height - 110);
            page.Stroke();

            // Title with standard font
            var defFont = new HpdfFont(pdf.Xref, HpdfStandardFont.Helvetica, "F0");
            page.SetFontAndSize(defFont, 24);

            const string pageTitle = "International Demo - \"Hello\" Around the World";
            float tw = defFont.MeasureText(pageTitle, 24);
            page.BeginText();
            page.MoveTextPos((width - tw) / 2, height - 50);
            page.ShowText(pageTitle);
            page.EndText();

            // Subtitle
            page.BeginText();
            page.SetFontAndSize(defFont, 12);
            page.MoveTextPos(60, height - 75);
            page.ShowText("Demonstrating single-byte encodings (CP1252, CP1251, CP1253, CP1254)");
            page.EndText();

            // Define greetings
            float yPos = height - 110;
            float lineHeight = 50;

            // English (Latin)
            DrawGreeting(page, defFont, notoLatin.AsFont(), "English:", "Hello", ref yPos, lineHeight);

            // French (Latin with accents: é, à)
            DrawGreeting(page, defFont, notoLatin.AsFont(), "French:", "Salut! Ça va?", ref yPos, lineHeight);

            // German (Latin with umlauts: ü, ö, ä, ß)
            DrawGreeting(page, defFont, notoLatin.AsFont(), "German:", "Grüße! Schön!", ref yPos, lineHeight);

            // Portuguese (Latin with accents: á, ã, õ)
            DrawGreeting(page, defFont, notoLatin.AsFont(), "Portuguese:", "Olá! Saudações!", ref yPos, lineHeight);

            // Russian (Cyrillic)
            DrawGreeting(page, defFont, notoCyrillic.AsFont(), "Russian:", "Привет мир!", ref yPos, lineHeight);

            // Greek
            DrawGreeting(page, defFont, notoGreek.AsFont(), "Greek:", "Γειά σου κόσμε!", ref yPos, lineHeight);

            // Turkish (Latin with special characters: ğ, ı, ş, ç, ö, ü)
            DrawGreeting(page, defFont, notoTurkish.AsFont(), "Turkish:", "Merhaba dünya!", ref yPos, lineHeight);

            // Add note about CJK
            page.BeginText();
            page.SetFontAndSize(defFont, 10);
            page.MoveTextPos(60, 80);
            page.ShowText("Note: Chinese, Japanese, and Korean require multi-byte encodings (CID fonts),");
            page.MoveTextPos(0, -15);
            page.ShowText("which are planned for future implementation.");
            page.EndText();
        }

        private static void DrawGreeting(
            HpdfPage page,
            HpdfFont labelFont,
            HpdfFont textFont,
            string label,
            string greeting,
            ref float yPos,
            float lineHeight)
        {
            page.BeginText();
            page.MoveTextPos(70, yPos);

            // Draw label
            page.SetFontAndSize(labelFont, 14);
            page.ShowText(label);

            // Draw greeting text
            page.MoveTextPos(100, 0);
            page.SetFontAndSize(textFont, 20);
            page.ShowText(greeting);

            page.EndText();

            yPos -= lineHeight;
        }

        private static void CreatePage2_CyrillicAlphabet(
            HpdfDocument pdf,
            HpdfTrueTypeFont notoLatin,
            HpdfTrueTypeFont notoCyrillic)
        {
            var page = pdf.AddPage();
            float height = page.Height;
            float width = page.Width;

            // Print the border of the page
            page.SetLineWidth(1);
            page.Rectangle(50, 50, width - 100, height - 110);
            page.Stroke();

            // Title with standard font
            var defFont = new HpdfFont(pdf.Xref, HpdfStandardFont.Helvetica, "F0");
            page.SetFontAndSize(defFont, 24);

            const string pageTitle = "Cyrillic Alphabet Test (CP1251)";
            float tw = defFont.MeasureText(pageTitle, 24);
            page.BeginText();
            page.MoveTextPos((width - tw) / 2, height - 50);
            page.ShowText(pageTitle);
            page.EndText();

            // Russian pangram
            string cyrillicText = "съешь же ещё этих мягких французских булок, да выпей чаю";

            // Display the Russian pangram
            page.BeginText();
            page.MoveTextPos(60, height - 120);

            // Label
            page.SetFontAndSize(defFont, 12);
            page.ShowText("Russian pangram (CP1251):");
            page.MoveTextPos(0, -25);

            // Cyrillic text with embedded font
            page.SetFontAndSize(notoCyrillic.AsFont(), 20);
            page.ShowText(cyrillicText);
            page.MoveTextPos(0, -30);

            // Show the text again in different size
            page.SetFontAndSize(defFont, 12);
            page.ShowText("Same text in larger size:");
            page.MoveTextPos(0, -25);

            page.SetFontAndSize(notoCyrillic.AsFont(), 28);
            page.ShowText(cyrillicText);

            page.EndText();

            // Add Cyrillic alphabet examples
            page.BeginText();
            page.MoveTextPos(60, height - 250);

            page.SetFontAndSize(defFont, 12);
            page.ShowText("Cyrillic alphabet (uppercase):");
            page.MoveTextPos(0, -20);

            string uppercaseAlphabet = "АБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
            page.SetFontAndSize(notoCyrillic.AsFont(), 16);
            page.ShowText(uppercaseAlphabet);
            page.MoveTextPos(0, -25);

            page.SetFontAndSize(defFont, 12);
            page.ShowText("Cyrillic alphabet (lowercase):");
            page.MoveTextPos(0, -20);

            string lowercaseAlphabet = "абвгдежзийклмнопрстуфхцчшщъыьэюя";
            page.SetFontAndSize(notoCyrillic.AsFont(), 16);
            page.ShowText(lowercaseAlphabet);

            page.EndText();
        }
    }
}
