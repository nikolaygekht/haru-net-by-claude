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
        var page = doc.AddPage(HpdfPageSize.A4, Haru.Doc.HpdfPageDirection.Portrait);

        // Draw a large rectangle to show the page has content
        page.SetRgbStroke(0.2f, 0.4f, 0.8f);
        page.SetRgbFill(0.8f, 0.9f, 1.0f);
        page.SetLineWidth(3);
        page.Rectangle(50, 50, 500, 700);
        page.FillStroke();

        // Draw smaller rectangle for title
        page.SetRgbFill(0.2f, 0.4f, 0.8f);
        page.Rectangle(75, 675, 450, 50);
        page.Fill();

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

        var page = doc.AddPage(HpdfPageSize.A4, Haru.Doc.HpdfPageDirection.Portrait);

        // Draw content
        page.SetRgbStroke(0.8f, 0.2f, 0.2f);
        page.SetRgbFill(1.0f, 0.9f, 0.9f);
        page.SetLineWidth(3);
        page.Rectangle(50, 50, 500, 700);
        page.FillStroke();

        // Draw warning sign (red X)
        page.SetRgbStroke(0.8f, 0, 0);
        page.SetLineWidth(10);
        page.MoveTo(100, 100);
        page.LineTo(250, 250);
        page.MoveTo(250, 100);
        page.LineTo(100, 250);
        page.Stroke();

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
