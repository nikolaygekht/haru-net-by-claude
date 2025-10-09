using System;
using Haru.Doc;
using Haru.Types;

class EncryptionTest
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Haru PDF Encryption Test ===\n");

        // Test 1: RC4 40-bit encryption (R2)
        Console.WriteLine("1. Creating RC4 40-bit encrypted PDF...");
        CreateEncryptedPdf(
            "test_rc4_40bit.pdf",
            HpdfEncryptMode.R2,
            "RC4 40-bit (Revision 2) Encryption"
        );

        // Test 2: RC4 128-bit encryption (R3)
        Console.WriteLine("2. Creating RC4 128-bit encrypted PDF...");
        CreateEncryptedPdf(
            "test_rc4_128bit.pdf",
            HpdfEncryptMode.R3,
            "RC4 128-bit (Revision 3) Encryption"
        );

        // Test 3: AES-128 encryption (R4)
        Console.WriteLine("3. Creating AES-128 encrypted PDF...");
        CreateEncryptedPdf(
            "test_aes_128.pdf",
            HpdfEncryptMode.R4,
            "AES-128 (Revision 4) Encryption"
        );

        // Test 4: Different permissions
        Console.WriteLine("4. Creating PDF with limited permissions...");
        CreatePdfWithPermissions();

        Console.WriteLine("\n=== All tests completed! ===");
        Console.WriteLine("\nGenerated files:");
        Console.WriteLine("  - test_rc4_40bit.pdf     (user: user123, owner: owner456)");
        Console.WriteLine("  - test_rc4_128bit.pdf    (user: user123, owner: owner456)");
        Console.WriteLine("  - test_aes_128.pdf       (user: user123, owner: owner456)");
        Console.WriteLine("  - test_permissions.pdf   (user: user123, owner: owner456, print only)");
        Console.WriteLine("\nTry opening these PDFs to test encryption!");
    }

    static void CreateEncryptedPdf(string filename, HpdfEncryptMode mode, string title)
    {
        var doc = new HpdfDocument();

        // Set document metadata
        doc.Info.Title = title;
        doc.Info.Author = "Haru Encryption Test";
        doc.Info.Creator = "EncryptionTest.cs";
        doc.Info.SetCreationDate(DateTime.Now);

        // Add a page
        var page = doc.AddPage(HpdfPageSize.A4, HpdfPageDirection.Portrait);

        // Get font
        var font = doc.GetFont("Helvetica");

        // Draw title
        page.SetFont(font, 24);
        page.BeginText();
        page.TextOut(100, 750, "Encrypted PDF Test");
        page.EndText();

        // Draw encryption info
        page.SetFont(font, 14);
        page.BeginText();
        page.MoveTextPos(100, 700);
        page.ShowText($"Encryption Mode: {mode}");
        page.MoveTextPos(0, -20);
        page.ShowText($"Title: {title}");
        page.MoveTextPos(0, -20);
        page.ShowText("User Password: user123");
        page.MoveTextPos(0, -20);
        page.ShowText("Owner Password: owner456");
        page.MoveTextPos(0, -40);
        page.ShowText("This document is encrypted!");
        page.MoveTextPos(0, -20);
        page.ShowText("You need a password to open it.");
        page.EndText();

        // Draw a rectangle
        page.SetRgbStroke(0.5f, 0.5f, 0.8f);
        page.SetLineWidth(2);
        page.Rectangle(80, 500, 450, 120);
        page.Stroke();

        // Add more content
        page.SetFont(font, 12);
        page.BeginText();
        page.MoveTextPos(100, 580);
        page.ShowText("This is a test of the Haru PDF encryption implementation.");
        page.MoveTextPos(0, -18);
        page.ShowText("The document should require a password when opened.");
        page.MoveTextPos(0, -18);
        page.ShowText("Once opened with the correct password, you should be");
        page.MoveTextPos(0, -18);
        page.ShowText("able to view and interact with the content.");
        page.EndText();

        // Set encryption
        doc.SetEncryption(
            userPassword: "user123",
            ownerPassword: "owner456",
            permission: HpdfPermission.Print | HpdfPermission.Copy,
            mode: mode
        );

        // Save
        doc.SaveToFile(filename);
        Console.WriteLine($"   Created: {filename}");
    }

    static void CreatePdfWithPermissions()
    {
        var doc = new HpdfDocument();

        doc.Info.Title = "Limited Permissions Test";
        doc.Info.Author = "Haru Encryption Test";

        var page = doc.AddPage(HpdfPageSize.A4, HpdfPageDirection.Portrait);
        var font = doc.GetFont("Helvetica");

        page.SetFont(font, 20);
        page.BeginText();
        page.TextOut(100, 750, "Limited Permissions PDF");
        page.EndText();

        page.SetFont(font, 12);
        page.BeginText();
        page.MoveTextPos(100, 700);
        page.ShowText("This PDF has LIMITED permissions:");
        page.MoveTextPos(0, -25);
        page.ShowText("✓ Printing is ALLOWED");
        page.MoveTextPos(0, -20);
        page.ShowText("✗ Copying text is NOT ALLOWED");
        page.MoveTextPos(0, -20);
        page.ShowText("✗ Editing is NOT ALLOWED");
        page.MoveTextPos(0, -20);
        page.ShowText("✗ Annotations are NOT ALLOWED");
        page.MoveTextPos(0, -40);
        page.ShowText("Try copying this text - it should be restricted!");
        page.EndText();

        // Set encryption with ONLY print permission
        doc.SetEncryption(
            userPassword: "user123",
            ownerPassword: "owner456",
            permission: HpdfPermission.Print,  // Only print allowed
            mode: HpdfEncryptMode.R3
        );

        doc.SaveToFile("test_permissions.pdf");
        Console.WriteLine("   Created: test_permissions.pdf");
    }
}
