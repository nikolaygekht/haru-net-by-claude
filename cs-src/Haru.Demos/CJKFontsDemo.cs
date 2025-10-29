/*
 * << Haru Free PDF Library >> -- CJKFontsDemo.cs
 *
 * Comprehensive demo for all CJK CIDFontType0 fonts
 */

using Haru.Doc;
using Haru.Font;
using Haru.Font.CID;

namespace Haru.Demos
{
    /// <summary>
    /// Demonstrates all CIDFontType0 predefined CJK fonts (11 fonts total):
    /// Fixed-Width: SimSun, SimHei, MingLiU, MS-Gothic, MS-Mincho, DotumChe, BatangChe
    /// Proportional: MS-PGothic, MS-PMincho, Dotum, Batang
    /// </summary>
    public static class CJKFontsDemo
    {
        public static void Run(string outputPath)
        {
            Console.WriteLine("=== CJK Fonts Demo (CIDFontType0) ===");
            Console.WriteLine("Testing all predefined CJK fonts with encoders");
            Console.WriteLine();

            try
            {
                // Create PDF document
                var doc = new HpdfDocument();

                // Add a page
                var page = doc.AddPage();
                page.SetSize(HpdfPageSize.A4, HpdfPageDirection.Portrait);

                float pageWidth = page.Width;
                float pageHeight = page.Height;
                float yPos = pageHeight - 50;
                float lineHeight = 25;

                // Title
                var titleFont = new HpdfFont(doc.Xref, HpdfStandardFont.HelveticaBold, "Title");
                page.BeginText();
                page.SetFontAndSize(titleFont, 20);
                page.MoveTextPos(50, yPos);
                page.ShowText("CJK Fonts Demo - CIDFontType0");
                page.EndText();
                yPos -= lineHeight * 2;

                // Chinese Simplified (SimSun - GBK-EUC-H)
                Console.WriteLine("Loading SimSun (Chinese Simplified)...");
                var gbkEncoder = new GBKEucHEncoder();
                var simSunFont = HpdfCIDFontType0.Create(doc, "SimSun", "SimSun", 936, "GBK-EUC-H", gbkEncoder).AsFont();

                page.BeginText();
                page.SetFontAndSize(titleFont, 12);
                page.MoveTextPos(50, yPos);
                page.ShowText("Chinese Simplified (SimSun + GBK-EUC-H):");
                page.EndText();
                yPos -= lineHeight;

                page.BeginText();
                page.SetFontAndSize(simSunFont, 16);
                page.MoveTextPos(50, yPos);
                page.ShowText("你好世界 - Hello World");
                page.EndText();
                yPos -= lineHeight * 1.5f;
                Console.WriteLine("  SimSun: OK");

                // Chinese Simplified (SimHei - GBK-EUC-H)
                Console.WriteLine("Loading SimHei (Chinese Simplified - Sans)...");
                var simHeiFont = HpdfCIDFontType0.Create(doc, "SimHei", "SimHei", 936, "GBK-EUC-H", gbkEncoder).AsFont();

                page.BeginText();
                page.SetFontAndSize(titleFont, 12);
                page.MoveTextPos(50, yPos);
                page.ShowText("Chinese Simplified (SimHei + GBK-EUC-H):");
                page.EndText();
                yPos -= lineHeight;

                page.BeginText();
                page.SetFontAndSize(simHeiFont, 16);
                page.MoveTextPos(50, yPos);
                page.ShowText("你好世界 - Hello World");
                page.EndText();
                yPos -= lineHeight * 1.5f;
                Console.WriteLine("  SimHei: OK");

                // Chinese Traditional (MingLiU - ETen-B5-H)
                Console.WriteLine("Loading MingLiU (Chinese Traditional)...");
                var etenEncoder = new ETenB5HEncoder();
                var mingLiUFont = HpdfCIDFontType0.Create(doc, "MingLiU", "MingLiU", 950, "ETen-B5-H", etenEncoder).AsFont();

                page.BeginText();
                page.SetFontAndSize(titleFont, 12);
                page.MoveTextPos(50, yPos);
                page.ShowText("Chinese Traditional (MingLiU + ETen-B5-H):");
                page.EndText();
                yPos -= lineHeight;

                page.BeginText();
                page.SetFontAndSize(mingLiUFont, 16);
                page.MoveTextPos(50, yPos);
                page.ShowText("你好世界 - Hello World");
                page.EndText();
                yPos -= lineHeight * 1.5f;
                Console.WriteLine("  MingLiU: OK");

                // Japanese (MS-Gothic - 90ms-RKSJ-H)
                Console.WriteLine("Loading MS-Gothic (Japanese)...");
                var sjisEncoder = new Ms90RKSJHEncoder();
                var msGothicFont = HpdfCIDFontType0.Create(doc, "MS-Gothic", "MSGothic", 932, "90ms-RKSJ-H", sjisEncoder).AsFont();

                page.BeginText();
                page.SetFontAndSize(titleFont, 12);
                page.MoveTextPos(50, yPos);
                page.ShowText("Japanese (MS-Gothic + 90ms-RKSJ-H):");
                page.EndText();
                yPos -= lineHeight;

                page.BeginText();
                page.SetFontAndSize(msGothicFont, 16);
                page.MoveTextPos(50, yPos);
                page.ShowText("こんにちは世界 - Hello World");
                page.EndText();
                yPos -= lineHeight * 1.5f;
                Console.WriteLine("  MS-Gothic: OK");

                // Japanese (MS-Mincho - 90ms-RKSJ-H)
                Console.WriteLine("Loading MS-Mincho (Japanese - Serif)...");
                var msMinchoFont = HpdfCIDFontType0.Create(doc, "MS-Mincho", "MSMincho", 932, "90ms-RKSJ-H", sjisEncoder).AsFont();

                page.BeginText();
                page.SetFontAndSize(titleFont, 12);
                page.MoveTextPos(50, yPos);
                page.ShowText("Japanese (MS-Mincho + 90ms-RKSJ-H):");
                page.EndText();
                yPos -= lineHeight;

                page.BeginText();
                page.SetFontAndSize(msMinchoFont, 16);
                page.MoveTextPos(50, yPos);
                page.ShowText("こんにちは世界 - Hello World");
                page.EndText();
                yPos -= lineHeight * 1.5f;
                Console.WriteLine("  MS-Mincho: OK");

                // Korean (DotumChe - KSCms-UHC-H)
                Console.WriteLine("Loading DotumChe (Korean)...");
                var uhcEncoder = new KSCmsUHCHEncoder();
                var dotumCheFont = HpdfCIDFontType0.Create(doc, "DotumChe", "DotumChe", 949, "KSCms-UHC-H", uhcEncoder).AsFont();

                page.BeginText();
                page.SetFontAndSize(titleFont, 12);
                page.MoveTextPos(50, yPos);
                page.ShowText("Korean (DotumChe + KSCms-UHC-H):");
                page.EndText();
                yPos -= lineHeight;

                page.BeginText();
                page.SetFontAndSize(dotumCheFont, 16);
                page.MoveTextPos(50, yPos);
                page.ShowText("안녕하세요 세계 - Hello World");
                page.EndText();
                yPos -= lineHeight * 1.5f;
                Console.WriteLine("  DotumChe: OK");

                // Korean (BatangChe - KSCms-UHC-H)
                Console.WriteLine("Loading BatangChe (Korean - Serif)...");
                var batangCheFont = HpdfCIDFontType0.Create(doc, "BatangChe", "BatangChe", 949, "KSCms-UHC-H", uhcEncoder).AsFont();

                page.BeginText();
                page.SetFontAndSize(titleFont, 12);
                page.MoveTextPos(50, yPos);
                page.ShowText("Korean (BatangChe + KSCms-UHC-H):");
                page.EndText();
                yPos -= lineHeight;

                page.BeginText();
                page.SetFontAndSize(batangCheFont, 16);
                page.MoveTextPos(50, yPos);
                page.ShowText("안녕하세요 세계 - Hello World");
                page.EndText();
                yPos -= lineHeight * 1.5f;
                Console.WriteLine("  BatangChe: OK");

                // Add page 2 for proportional fonts
                var page2 = doc.AddPage();
                page2.SetSize(HpdfPageSize.A4, HpdfPageDirection.Portrait);
                yPos = pageHeight - 50;

                // Page 2 Title
                page2.BeginText();
                page2.SetFontAndSize(titleFont, 20);
                page2.MoveTextPos(50, yPos);
                page2.ShowText("CJK Proportional Fonts");
                page2.EndText();
                yPos -= lineHeight * 2;

                // Japanese Proportional (MS-PGothic - 90ms-RKSJ-H)
                Console.WriteLine("Loading MS-PGothic (Japanese Proportional)...");
                var msPGothicFont = HpdfCIDFontType0.Create(doc, "MS-PGothic", "MSPGothic", 932, "90ms-RKSJ-H", sjisEncoder).AsFont();

                page2.BeginText();
                page2.SetFontAndSize(titleFont, 12);
                page2.MoveTextPos(50, yPos);
                page2.ShowText("Japanese Proportional (MS-PGothic + 90ms-RKSJ-H):");
                page2.EndText();
                yPos -= lineHeight;

                page2.BeginText();
                page2.SetFontAndSize(msPGothicFont, 16);
                page2.MoveTextPos(50, yPos);
                page2.ShowText("こんにちは世界 - Hello World");
                page2.EndText();
                yPos -= lineHeight * 1.5f;
                Console.WriteLine("  MS-PGothic: OK");

                // Japanese Proportional (MS-PMincho - 90ms-RKSJ-H)
                Console.WriteLine("Loading MS-PMincho (Japanese Proportional - Serif)...");
                var msPMinchoFont = HpdfCIDFontType0.Create(doc, "MS-PMincho", "MSPMincho", 932, "90ms-RKSJ-H", sjisEncoder).AsFont();

                page2.BeginText();
                page2.SetFontAndSize(titleFont, 12);
                page2.MoveTextPos(50, yPos);
                page2.ShowText("Japanese Proportional (MS-PMincho + 90ms-RKSJ-H):");
                page2.EndText();
                yPos -= lineHeight;

                page2.BeginText();
                page2.SetFontAndSize(msPMinchoFont, 16);
                page2.MoveTextPos(50, yPos);
                page2.ShowText("こんにちは世界 - Hello World");
                page2.EndText();
                yPos -= lineHeight * 1.5f;
                Console.WriteLine("  MS-PMincho: OK");

                // Korean Proportional (Dotum - KSCms-UHC-H)
                Console.WriteLine("Loading Dotum (Korean Proportional)...");
                var dotumFont = HpdfCIDFontType0.Create(doc, "Dotum", "Dotum", 949, "KSCms-UHC-H", uhcEncoder).AsFont();

                page2.BeginText();
                page2.SetFontAndSize(titleFont, 12);
                page2.MoveTextPos(50, yPos);
                page2.ShowText("Korean Proportional (Dotum + KSCms-UHC-H):");
                page2.EndText();
                yPos -= lineHeight;

                page2.BeginText();
                page2.SetFontAndSize(dotumFont, 16);
                page2.MoveTextPos(50, yPos);
                page2.ShowText("안녕하세요 세계 - Hello World");
                page2.EndText();
                yPos -= lineHeight * 1.5f;
                Console.WriteLine("  Dotum: OK");

                // Korean Proportional (Batang - KSCms-UHC-H)
                Console.WriteLine("Loading Batang (Korean Proportional - Serif)...");
                var batangFont = HpdfCIDFontType0.Create(doc, "Batang", "Batang", 949, "KSCms-UHC-H", uhcEncoder).AsFont();

                page2.BeginText();
                page2.SetFontAndSize(titleFont, 12);
                page2.MoveTextPos(50, yPos);
                page2.ShowText("Korean Proportional (Batang + KSCms-UHC-H):");
                page2.EndText();
                yPos -= lineHeight;

                page2.BeginText();
                page2.SetFontAndSize(batangFont, 16);
                page2.MoveTextPos(50, yPos);
                page2.ShowText("안녕하세요 세계 - Hello World");
                page2.EndText();
                yPos -= lineHeight * 1.5f;
                Console.WriteLine("  Batang: OK");

                // Add footer note on page 2
                yPos -= lineHeight;
                page2.BeginText();
                page2.SetFontAndSize(titleFont, 8);
                page2.MoveTextPos(50, yPos);
                page2.ShowText("NOTE: These fonts must be installed on the viewer's system.");
                page2.EndText();
                yPos -= lineHeight * 0.7f;

                page2.BeginText();
                page2.SetFontAndSize(titleFont, 8);
                page2.MoveTextPos(50, yPos);
                page2.ShowText("If unavailable, substitute fonts will be used.");
                page2.EndText();

                // Save PDF
                var directory = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                using (var stream = File.Create(outputPath))
                {
                    doc.Save(stream);
                }

                Console.WriteLine($"\nPDF saved to: {outputPath}");
                Console.WriteLine("\nSummary:");
                Console.WriteLine("  Fixed-Width Fonts:");
                Console.WriteLine("    - Chinese Simplified: SimSun, SimHei (GBK-EUC-H, CP936)");
                Console.WriteLine("    - Chinese Traditional: MingLiU (ETen-B5-H, CP950)");
                Console.WriteLine("    - Japanese: MS-Gothic, MS-Mincho (90ms-RKSJ-H, CP932)");
                Console.WriteLine("    - Korean: DotumChe, BatangChe (KSCms-UHC-H, CP949)");
                Console.WriteLine("  Proportional Fonts:");
                Console.WriteLine("    - Japanese: MS-PGothic, MS-PMincho (90ms-RKSJ-H, CP932)");
                Console.WriteLine("    - Korean: Dotum, Batang (KSCms-UHC-H, CP949)");
                Console.WriteLine($"  PDF Version: {doc.Version}");
                Console.WriteLine($"  Pages: 2");
                Console.WriteLine($"  File Size: {new FileInfo(outputPath).Length} bytes");
                Console.WriteLine("\nAll 11 CJK fonts loaded successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nERROR: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }
    }
}
