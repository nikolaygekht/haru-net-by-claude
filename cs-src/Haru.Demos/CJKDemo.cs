/*
 * << Haru Free PDF Library >> -- CJKDemo.cs
 *
 * CJK (Chinese, Japanese, Korean) Font Demo
 *
 * This demo demonstrates CID (Character Identifier) font support for
 * multi-byte character sets used in CJK languages.
 *
 * Copyright (c) 1999-2025 Haru Free PDF Library
 *
 * Permission to use, copy, modify, distribute and sell this software
 * and its documentation for any purpose is hereby granted without fee,
 */

using System;
using Haru.Doc;
using Haru.Font;
using Haru.Font.CID;
using Haru.Types;

namespace BasicDemos
{
    /// <summary>
    /// Demonstrates CID font support for CJK languages
    ///
    /// Requirements:
    /// - CID Font implementation (HpdfCIDFont class)
    /// - Multi-byte text encoding support
    /// - Identity-H encoding for Unicode mapping
    /// - ToUnicode CMap for text extraction
    ///
    /// Critical: Adobe Acrobat compatibility
    /// - Must work in Adobe Acrobat Reader (not just Chrome/Edge)
    /// - Previous attempt had "Cannot find or create font" errors in Adobe
    /// - Focus on correct PDF structure and object ordering
    /// </summary>
    public static class CJKDemo
    {
        public static void Run()
        {
            try
            {
                Console.WriteLine("Creating CJK Font Demo...");

                var pdf = new HpdfDocument();
                var page = pdf.AddPage();

                float height = page.Height;
                float width = page.Width;

                // Print the border
                page.SetLineWidth(1);
                page.Rectangle(50, 50, width - 100, height - 110);
                page.Stroke();

                // Print the title
                var defFont = new HpdfFont(pdf.Xref, HpdfStandardFont.Helvetica, "F0");
                page.SetFontAndSize(defFont, 24);

                string pageTitle = "CJK Font Demo";
                float tw = defFont.MeasureText(pageTitle, 24);
                page.BeginText();
                page.MoveTextPos((width - tw) / 2, height - 50);
                page.ShowText(pageTitle);
                page.EndText();

                // Output subtitle
                page.BeginText();
                page.SetFontAndSize(defFont, 14);
                page.MoveTextPos(60, height - 80);
                page.ShowText("Chinese, Japanese, Korean Language Support");
                page.EndText();

                // Load CID fonts for each language

                // Traditional Chinese (Taiwan)
                var chineseTraditionalFont = HpdfCIDFont.LoadFromTrueTypeFile(
                    pdf.Xref,
                    "CHT",
                    "demo/ttfont/noto-cht.ttf",
                    950);  // CP950 (Big5)

                // Simplified Chinese (China)
                var chineseSimplifiedFont = HpdfCIDFont.LoadFromTrueTypeFile(
                    pdf.Xref,
                    "CHS",
                    "demo/ttfont/noto-chs.ttf",
                    936);  // CP936 (GBK)

                // Japanese
                var japaneseFont = HpdfCIDFont.LoadFromTrueTypeFile(
                    pdf.Xref,
                    "JP",
                    "demo/ttfont/noto-jp.ttf",
                    932);  // CP932 (Shift-JIS)

                // Korean
                var koreanFont = HpdfCIDFont.LoadFromTrueTypeFile(
                    pdf.Xref,
                    "KR",
                    "demo/ttfont/noto-kr.ttf",
                    949);  // CP949 (EUC-KR)

                // Render sample text in each language
                page.BeginText();
                page.SetFontAndSize(defFont, 12);
                page.MoveTextPos(60, height - 120);

                // Traditional Chinese section
                page.SetFontAndSize(defFont, 10);
                page.ShowText("Traditional Chinese (Big5, CP950):");
                page.MoveTextPos(0, -20);

                page.SetFontAndSize(chineseTraditionalFont.AsFont(), 16);
                page.ShowText("你好世界");  // Hello World
                page.MoveTextPos(0, -25);
                page.ShowText("繁體中文測試");  // Traditional Chinese Test
                page.MoveTextPos(0, -30);

                // Simplified Chinese section
                page.SetFontAndSize(defFont, 10);
                page.ShowText("Simplified Chinese (GBK, CP936):");
                page.MoveTextPos(0, -20);

                page.SetFontAndSize(chineseSimplifiedFont.AsFont(), 16);
                page.ShowText("你好世界");  // Hello World
                page.MoveTextPos(0, -25);
                page.ShowText("简体中文测试");  // Simplified Chinese Test
                page.MoveTextPos(0, -30);

                // Japanese section
                page.SetFontAndSize(defFont, 10);
                page.ShowText("Japanese (Shift-JIS, CP932):");
                page.MoveTextPos(0, -20);

                page.SetFontAndSize(japaneseFont.AsFont(), 16);
                page.ShowText("こんにちは世界");  // Hello World (Hiragana)
                page.MoveTextPos(0, -25);
                page.ShowText("日本語テスト");  // Japanese Test (Kanji + Katakana)
                page.MoveTextPos(0, -30);

                // Korean section
                page.SetFontAndSize(defFont, 10);
                page.ShowText("Korean (EUC-KR, CP949):");
                page.MoveTextPos(0, -20);

                page.SetFontAndSize(koreanFont.AsFont(), 16);
                page.ShowText("안녕하세요");  // Hello (Hangul)
                page.MoveTextPos(0, -25);
                page.ShowText("한국어 테스트");  // Korean Test
                page.MoveTextPos(0, -30);

                page.EndText();

                // Add implementation notes
                page.BeginText();
                page.SetFontAndSize(defFont, 9);
                page.MoveTextPos(60, height - 480);
                page.ShowText("Implementation Details:");
                page.MoveTextPos(0, -15);
                page.ShowText("- CID Font Type: CIDFontType2 (TrueType-based)");
                page.MoveTextPos(0, -15);
                page.ShowText("- Encoding: Identity-H (horizontal writing)");
                page.MoveTextPos(0, -15);
                page.ShowText("- ToUnicode CMap: Enabled for text extraction");
                page.MoveTextPos(0, -15);
                page.ShowText("- Font Structure: Type 0 (Composite) -> CIDFontType2 -> FontFile2");
                page.MoveTextPos(0, -20);
                page.ShowText("Adobe Acrobat Compatibility:");
                page.MoveTextPos(0, -15);
                page.ShowText("- Correct PDF object build order (FontFile2 -> Descriptor -> CIDFont -> Type0)");
                page.MoveTextPos(0, -15);
                page.ShowText("- Simple font naming to avoid conflicts");
                page.MoveTextPos(0, -15);
                page.ShowText("- Complete object dictionaries before cross-referencing");
                page.EndText();

                pdf.SaveToFile("pdfs/CJKDemo.pdf");
                Console.WriteLine("CJK Demo PDF created: pdfs/CJKDemo.pdf");
                Console.WriteLine("CJK text rendered successfully in Traditional Chinese, Simplified Chinese, Japanese, and Korean.");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Error in CJKDemo: {e.Message}");
                Console.Error.WriteLine($"Stack trace: {e.StackTrace}");
                throw;
            }
        }
    }
}
