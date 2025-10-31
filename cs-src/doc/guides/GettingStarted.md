# Getting Started with Haru.NET

[← Back to Documentation Index](../INDEX.md)

---

## Introduction

This tutorial will walk you through creating your first PDF document with Haru.NET. By the end, you'll understand how to create pages, add text, draw shapes, and save PDF files.

### What You'll Build

You'll create a complete PDF document that includes:
- Custom page sizes
- Multiple fonts
- Formatted text
- Graphics (lines, rectangles, circles)
- Colors
- Multiple pages

---

## Prerequisites

### Installation

Create a new console application and add Haru.NET:

```bash
dotnet new console -n MyFirstPdf
cd MyFirstPdf
dotnet add package Haru
```

### Required Namespaces

Add these using statements to your Program.cs:

```csharp
using Haru.Doc;
using Haru.Font;
using Haru.Types;
```

---

## Step 1: Create Your First Document

Let's start with the absolute minimum to create a PDF:

```csharp
using Haru.Doc;
using Haru.Font;

// Create a new PDF document
var pdf = new HpdfDocument();

// Create a font (required for any text)
var font = new HpdfFont(pdf.Xref, HpdfStandardFont.Helvetica, "F1");

// Add a page (A4 size by default)
var page = pdf.AddPage();

// Write some text
page.SetFontAndSize(font, 24);
page.BeginText();
page.MoveTextPos(100, 700);
page.ShowText("My First PDF!");
page.EndText();

// Save the file
pdf.SaveToFile("first.pdf");

Console.WriteLine("PDF created: first.pdf");
```

**Run it:**
```bash
dotnet run
```

You should see `first.pdf` created in your project directory!

### Understanding the Code

1. **HpdfDocument** - The main document object that manages everything
2. **HpdfFont** - Font objects are created from the document's Xref (cross-reference table)
3. **AddPage()** - Creates a new blank page (A4 portrait by default)
4. **BeginText/EndText** - All text operations must be wrapped in these calls
5. **MoveTextPos** - Positions the text cursor (bottom-left origin: 0,0 is bottom-left)
6. **ShowText** - Renders the text at the current position
7. **SaveToFile** - Writes the PDF to disk

---

## Step 2: Understanding Coordinates

PDF uses a bottom-left origin coordinate system, which can be confusing at first.

```csharp
using Haru.Doc;
using Haru.Font;
using Haru.Types;

var pdf = new HpdfDocument();
var font = new HpdfFont(pdf.Xref, HpdfStandardFont.Helvetica, "F1");
var page = pdf.AddPage(HpdfPageSize.A4, HpdfPageDirection.Portrait);

// A4 page: 595 x 842 points (1 point = 1/72 inch)
Console.WriteLine($"Page size: {page.Width} x {page.Height}");

page.SetFontAndSize(font, 12);

// Top-left corner (50 from edges)
page.BeginText();
page.MoveTextPos(50, page.Height - 50);  // 50, 792
page.ShowText("Top-left corner");
page.EndText();

// Top-right corner
float textWidth = font.MeasureText("Top-right corner", 12);
page.BeginText();
page.MoveTextPos(page.Width - textWidth - 50, page.Height - 50);
page.ShowText("Top-right corner");
page.EndText();

// Bottom-left corner
page.BeginText();
page.MoveTextPos(50, 50);
page.ShowText("Bottom-left corner");
page.EndText();

// Bottom-right corner
textWidth = font.MeasureText("Bottom-right corner", 12);
page.BeginText();
page.MoveTextPos(page.Width - textWidth - 50, 50);
page.ShowText("Bottom-right corner");
page.EndText();

// Center of page
string centerText = "Center";
textWidth = font.MeasureText(centerText, 12);
page.BeginText();
page.MoveTextPos((page.Width - textWidth) / 2, page.Height / 2);
page.ShowText(centerText);
page.EndText();

pdf.SaveToFile("coordinates.pdf");
Console.WriteLine("Created: coordinates.pdf");
```

**Key Points:**
- **(0, 0)** is at the **bottom-left**
- **Y increases going up** (opposite of many graphics systems)
- Page dimensions are in **points** (72 points = 1 inch)
- Use `page.Height - y` to position from the top

---

## Step 3: Working with Different Fonts

PDF has 14 standard fonts that don't require embedding. You can also load custom TrueType fonts.

```csharp
using Haru.Doc;
using Haru.Font;

var pdf = new HpdfDocument();
var page = pdf.AddPage();

// Create different standard fonts
var helvetica = new HpdfFont(pdf.Xref, HpdfStandardFont.Helvetica, "F1");
var helveticaBold = new HpdfFont(pdf.Xref, HpdfStandardFont.HelveticaBold, "F2");
var times = new HpdfFont(pdf.Xref, HpdfStandardFont.TimesRoman, "F3");
var timesBold = new HpdfFont(pdf.Xref, HpdfStandardFont.TimesBold, "F4");
var courier = new HpdfFont(pdf.Xref, HpdfStandardFont.Courier, "F5");

float y = page.Height - 100;

// Helvetica family
page.SetFontAndSize(helvetica, 16);
page.BeginText();
page.MoveTextPos(50, y);
page.ShowText("Helvetica Regular");
page.EndText();

y -= 30;
page.SetFontAndSize(helveticaBold, 16);
page.BeginText();
page.MoveTextPos(50, y);
page.ShowText("Helvetica Bold");
page.EndText();

// Times family
y -= 50;
page.SetFontAndSize(times, 16);
page.BeginText();
page.MoveTextPos(50, y);
page.ShowText("Times Roman Regular");
page.EndText();

y -= 30;
page.SetFontAndSize(timesBold, 16);
page.BeginText();
page.MoveTextPos(50, y);
page.ShowText("Times Roman Bold");
page.EndText();

// Courier (monospace)
y -= 50;
page.SetFontAndSize(courier, 16);
page.BeginText();
page.MoveTextPos(50, y);
page.ShowText("Courier Monospace");
page.EndText();

pdf.SaveToFile("fonts.pdf");
Console.WriteLine("Created: fonts.pdf");
```

### Loading Custom TrueType Fonts

```csharp
// Load a .ttf file
var customFont = pdf.GetFont("path/to/your/font.ttf", "CP1252");

page.SetFontAndSize(customFont, 18);
page.BeginText();
page.MoveTextPos(50, 500);
page.ShowText("Text in custom font");
page.EndText();
```

---

## Step 4: Drawing Shapes

Now let's add some graphics to your PDF.

```csharp
using Haru.Doc;
using Haru.Font;
using Haru.Types;

var pdf = new HpdfDocument();
var font = new HpdfFont(pdf.Xref, HpdfStandardFont.Helvetica, "F1");
var page = pdf.AddPage();

// Title
page.SetFontAndSize(font, 20);
page.BeginText();
page.MoveTextPos(50, page.Height - 50);
page.ShowText("Shape Examples");
page.EndText();

// Draw a rectangle (outline only)
page.SetLineWidth(2);
page.SetRgbStroke(0, 0, 0);  // Black
page.Rectangle(50, 650, 200, 100);
page.Stroke();

// Label
page.SetFontAndSize(font, 10);
page.BeginText();
page.MoveTextPos(50, 760);
page.ShowText("Rectangle (stroked)");
page.EndText();

// Draw a filled rectangle
page.SetRgbFill(0.8f, 0.2f, 0.2f);  // Red
page.Rectangle(300, 650, 200, 100);
page.Fill();

// Label
page.BeginText();
page.MoveTextPos(300, 760);
page.ShowText("Rectangle (filled)");
page.EndText();

// Draw a rectangle with both fill and stroke
page.SetRgbFill(0.2f, 0.6f, 0.8f);  // Blue
page.SetRgbStroke(0, 0, 0);         // Black outline
page.SetLineWidth(3);
page.Rectangle(50, 500, 200, 100);
page.FillStroke();

// Label
page.BeginText();
page.MoveTextPos(50, 610);
page.ShowText("Rectangle (fill + stroke)");
page.EndText();

// Draw a circle
page.SetRgbStroke(0, 0.5f, 0);  // Green
page.SetLineWidth(2);
page.Circle(400, 550, 50);
page.Stroke();

// Label
page.BeginText();
page.MoveTextPos(360, 610);
page.ShowText("Circle");
page.EndText();

// Draw lines
page.SetRgbStroke(0.5f, 0, 0.5f);  // Purple
page.SetLineWidth(3);
page.MoveTo(50, 400);
page.LineTo(250, 450);
page.LineTo(450, 400);
page.Stroke();

// Label
page.BeginText();
page.MoveTextPos(50, 460);
page.ShowText("Connected lines");
page.EndText();

// Draw dashed line
page.SetRgbStroke(0, 0, 0);
page.SetLineWidth(1);
ushort[] dashPattern = new ushort[] { 5, 3 };  // 5 on, 3 off
page.SetDash(dashPattern, 0);
page.MoveTo(50, 320);
page.LineTo(500, 320);
page.Stroke();

// Reset dash to solid
page.SetDash(Array.Empty<ushort>(), 0);

// Label
page.BeginText();
page.MoveTextPos(50, 330);
page.ShowText("Dashed line");
page.EndText();

// Draw an ellipse
page.SetRgbFill(1.0f, 0.8f, 0.0f);  // Yellow
page.Ellipse(250, 200, 100, 50);
page.FillStroke();

// Label
page.BeginText();
page.MoveTextPos(200, 260);
page.ShowText("Ellipse");
page.EndText();

pdf.SaveToFile("shapes.pdf");
Console.WriteLine("Created: shapes.pdf");
```

### Key Drawing Methods

- **Rectangle(x, y, width, height)** - Draw a rectangle
- **Circle(x, y, radius)** - Draw a circle
- **Ellipse(x, y, xRadius, yRadius)** - Draw an ellipse
- **MoveTo(x, y)** - Move to position without drawing
- **LineTo(x, y)** - Draw line from current position to (x, y)
- **Stroke()** - Outline the path
- **Fill()** - Fill the path
- **FillStroke()** - Fill and outline in one operation

---

## Step 5: Working with Colors

```csharp
using Haru.Doc;
using Haru.Font;

var pdf = new HpdfDocument();
var font = new HpdfFont(pdf.Xref, HpdfStandardFont.Helvetica, "F1");
var page = pdf.AddPage();

// RGB colors (values from 0.0 to 1.0)
page.SetFontAndSize(font, 12);

// Red text
page.SetRgbFill(1.0f, 0.0f, 0.0f);
page.BeginText();
page.MoveTextPos(50, 700);
page.ShowText("Red text");
page.EndText();

// Green text
page.SetRgbFill(0.0f, 0.8f, 0.0f);
page.BeginText();
page.MoveTextPos(50, 670);
page.ShowText("Green text");
page.EndText();

// Blue text
page.SetRgbFill(0.0f, 0.0f, 1.0f);
page.BeginText();
page.MoveTextPos(50, 640);
page.ShowText("Blue text");
page.EndText();

// Draw colored rectangles
page.SetRgbFill(1.0f, 0.0f, 0.0f);  // Red
page.Rectangle(50, 550, 100, 50);
page.Fill();

page.SetRgbFill(0.0f, 1.0f, 0.0f);  // Green
page.Rectangle(170, 550, 100, 50);
page.Fill();

page.SetRgbFill(0.0f, 0.0f, 1.0f);  // Blue
page.Rectangle(290, 550, 100, 50);
page.Fill();

// CMYK colors (for printing)
page.SetCmykFill(1.0f, 0.0f, 0.0f, 0.0f);  // Cyan
page.Rectangle(50, 450, 100, 50);
page.Fill();

page.SetCmykFill(0.0f, 1.0f, 0.0f, 0.0f);  // Magenta
page.Rectangle(170, 450, 100, 50);
page.Fill();

page.SetCmykFill(0.0f, 0.0f, 1.0f, 0.0f);  // Yellow
page.Rectangle(290, 450, 100, 50);
page.Fill();

page.SetCmykFill(0.0f, 0.0f, 0.0f, 1.0f);  // Black
page.Rectangle(410, 450, 100, 50);
page.Fill();

// Label with black text
page.SetRgbFill(0, 0, 0);
page.BeginText();
page.MoveTextPos(50, 410);
page.ShowText("CMYK colors (Cyan, Magenta, Yellow, Black)");
page.EndText();

pdf.SaveToFile("colors.pdf");
Console.WriteLine("Created: colors.pdf");
```

**Color Methods:**
- **SetRgbFill(r, g, b)** - Set fill color (RGB)
- **SetRgbStroke(r, g, b)** - Set stroke/outline color (RGB)
- **SetCmykFill(c, m, y, k)** - Set fill color (CMYK)
- **SetCmykStroke(c, m, y, k)** - Set stroke color (CMYK)

---

## Step 6: Creating Multiple Pages

```csharp
using Haru.Doc;
using Haru.Font;
using Haru.Types;

var pdf = new HpdfDocument();
var font = new HpdfFont(pdf.Xref, HpdfStandardFont.Helvetica, "F1");

// Page 1: Portrait A4
var page1 = pdf.AddPage(HpdfPageSize.A4, HpdfPageDirection.Portrait);
page1.SetFontAndSize(font, 24);
page1.BeginText();
page1.MoveTextPos(100, 700);
page1.ShowText("Page 1 - A4 Portrait");
page1.EndText();

// Draw page border
page1.SetLineWidth(2);
page1.Rectangle(50, 50, page1.Width - 100, page1.Height - 100);
page1.Stroke();

// Page 2: Landscape A4
var page2 = pdf.AddPage(HpdfPageSize.A4, HpdfPageDirection.Landscape);
page2.SetFontAndSize(font, 24);
page2.BeginText();
page2.MoveTextPos(100, 500);
page2.ShowText("Page 2 - A4 Landscape");
page2.EndText();

// Draw page border
page2.SetLineWidth(2);
page2.Rectangle(50, 50, page2.Width - 100, page2.Height - 100);
page2.Stroke();

// Page 3: Letter size
var page3 = pdf.AddPage(HpdfPageSize.Letter, HpdfPageDirection.Portrait);
page3.SetFontAndSize(font, 24);
page3.BeginText();
page3.MoveTextPos(100, 650);
page3.ShowText("Page 3 - Letter Size");
page3.EndText();

// Draw page border
page3.SetLineWidth(2);
page3.Rectangle(50, 50, page3.Width - 100, page3.Height - 100);
page3.Stroke();

// Page 4: Custom size
var page4 = pdf.AddPage();
page4.SetSize(600, 600);  // Square page
page4.SetFontAndSize(font, 24);
page4.BeginText();
page4.MoveTextPos(100, 500);
page4.ShowText("Page 4 - Custom Size (600x600)");
page4.EndText();

// Draw page border
page4.SetLineWidth(2);
page4.Rectangle(50, 50, page4.Width - 100, page4.Height - 100);
page4.Stroke();

pdf.SaveToFile("multiple-pages.pdf");
Console.WriteLine("Created: multiple-pages.pdf with 4 pages");
```

---

## Step 7: Complete Example - Business Card

Let's put it all together to create a business card design:

```csharp
using Haru.Doc;
using Haru.Font;
using Haru.Types;

var pdf = new HpdfDocument();

// Business card size: 3.5" x 2" = 252 x 144 points
var page = pdf.AddPage();
page.SetSize(252, 144);

// Create fonts
var helveticaBold = new HpdfFont(pdf.Xref, HpdfStandardFont.HelveticaBold, "F1");
var helvetica = new HpdfFont(pdf.Xref, HpdfStandardFont.Helvetica, "F2");
var helveticaOblique = new HpdfFont(pdf.Xref, HpdfStandardFont.HelveticaOblique, "F3");

// Draw border
page.SetRgbStroke(0.2f, 0.2f, 0.6f);  // Dark blue
page.SetLineWidth(2);
page.Rectangle(10, 10, 232, 124);
page.Stroke();

// Draw accent line
page.SetRgbFill(0.2f, 0.2f, 0.6f);
page.Rectangle(10, 100, 232, 3);
page.Fill();

// Name
page.SetRgbFill(0, 0, 0);
page.SetFontAndSize(helveticaBold, 18);
page.BeginText();
page.MoveTextPos(20, 110);
page.ShowText("John Developer");
page.EndText();

// Title
page.SetRgbFill(0.3f, 0.3f, 0.3f);
page.SetFontAndSize(helveticaOblique, 10);
page.BeginText();
page.MoveTextPos(20, 85);
page.ShowText("Senior Software Engineer");
page.EndText();

// Contact info
page.SetRgbFill(0, 0, 0);
page.SetFontAndSize(helvetica, 9);

page.BeginText();
page.MoveTextPos(20, 60);
page.ShowText("Email: john@example.com");
page.EndText();

page.BeginText();
page.MoveTextPos(20, 48);
page.ShowText("Phone: +1 (555) 123-4567");
page.EndText();

page.BeginText();
page.MoveTextPos(20, 36);
page.ShowText("Web: www.example.com");
page.EndText();

page.BeginText();
page.MoveTextPos(20, 24);
page.ShowText("City, State, Country");
page.EndText();

// Logo area (simple circle as placeholder)
page.SetRgbFill(0.2f, 0.2f, 0.6f);
page.Circle(210, 70, 20);
page.Fill();

// Initials in logo
page.SetRgbFill(1, 1, 1);
page.SetFontAndSize(helveticaBold, 14);
string initials = "JD";
float initialsWidth = helveticaBold.MeasureText(initials, 14);
page.BeginText();
page.MoveTextPos(210 - initialsWidth / 2, 66);
page.ShowText(initials);
page.EndText();

pdf.SaveToFile("business-card.pdf");
Console.WriteLine("Created: business-card.pdf");
```

---

## Step 8: Document Metadata

Add metadata to your PDFs for better organization:

```csharp
using Haru.Doc;
using Haru.Font;

var pdf = new HpdfDocument();

// Set document metadata
pdf.Info.SetTitle("My Amazing PDF Document");
pdf.Info.SetAuthor("John Developer");
pdf.Info.SetSubject("Getting Started with Haru.NET");
pdf.Info.SetKeywords("PDF, Haru, .NET, Tutorial");
pdf.Info.SetCreator("Haru.NET Tutorial Application");

var font = new HpdfFont(pdf.Xref, HpdfStandardFont.Helvetica, "F1");
var page = pdf.AddPage();

page.SetFontAndSize(font, 20);
page.BeginText();
page.MoveTextPos(100, 700);
page.ShowText("Document with Metadata");
page.EndText();

pdf.SaveToFile("metadata.pdf");
Console.WriteLine("Created: metadata.pdf");
Console.WriteLine("Check document properties in your PDF viewer!");
```

---

## Step 9: Error Handling

Always handle errors gracefully:

```csharp
using Haru.Doc;
using Haru.Font;

try
{
    var pdf = new HpdfDocument();
    var font = new HpdfFont(pdf.Xref, HpdfStandardFont.Helvetica, "F1");
    var page = pdf.AddPage();

    page.SetFontAndSize(font, 24);
    page.BeginText();
    page.MoveTextPos(100, 700);
    page.ShowText("Success!");
    page.EndText();

    pdf.SaveToFile("output.pdf");
    Console.WriteLine("✓ PDF created successfully");
}
catch (Exception ex)
{
    Console.Error.WriteLine($"✗ Error creating PDF: {ex.Message}");
    Console.Error.WriteLine($"  Stack trace: {ex.StackTrace}");
}
```

---

## Next Steps

Congratulations! You now know the basics of creating PDFs with Haru.NET.

### Learn More

**Guides:**
- [Fonts Guide](FontsGuide.md) - Master TrueType, Type1, and CJK fonts
- [Graphics Guide](GraphicsGuide.md) - Advanced drawing techniques
- [Text Guide](TextGuide.md) - Text formatting and layout
- [Images Guide](ImagesGuide.md) - Working with PNG and JPEG images

**API Reference:**
- [HpdfDocument](../api/core/HpdfDocument.md) - Complete document API
- [HpdfPage](../api/core/HpdfPage.md) - Page operations reference
- [HpdfFont](../api/core/HpdfFont.md) - Font system reference

### Try These Challenges

1. **Create a certificate** - Use what you've learned to create an award certificate
2. **Build a report** - Create a multi-page report with headers and footers
3. **Design a flyer** - Combine text, shapes, and colors to create a promotional flyer
4. **Make an invoice** - Create a professional invoice template

---

## Common Mistakes to Avoid

### 1. Forgetting BeginText/EndText

```csharp
// ❌ WRONG - Missing BeginText/EndText
page.MoveTextPos(100, 700);
page.ShowText("Text");

// ✓ CORRECT
page.BeginText();
page.MoveTextPos(100, 700);
page.ShowText("Text");
page.EndText();
```

### 2. Wrong Coordinate System

```csharp
// ❌ WRONG - Thinking (0,0) is top-left
page.BeginText();
page.MoveTextPos(100, 100);  // This is near the BOTTOM
page.ShowText("Text");
page.EndText();

// ✓ CORRECT - Position from top
page.BeginText();
page.MoveTextPos(100, page.Height - 100);  // This is near the TOP
page.ShowText("Text");
page.EndText();
```

### 3. Not Setting Font Before Text

```csharp
// ❌ WRONG - No font set
page.BeginText();
page.MoveTextPos(100, 700);
page.ShowText("Text");  // Error or no text displayed
page.EndText();

// ✓ CORRECT
page.SetFontAndSize(font, 12);
page.BeginText();
page.MoveTextPos(100, 700);
page.ShowText("Text");
page.EndText();
```

### 4. Color Values Outside Range

```csharp
// ❌ WRONG - RGB values must be 0.0 to 1.0
page.SetRgbFill(255, 0, 0);  // This is wrong!

// ✓ CORRECT - Convert to 0.0-1.0 range
page.SetRgbFill(1.0f, 0.0f, 0.0f);  // Pure red
```

---

## Troubleshooting

**Problem:** Text doesn't appear in the PDF
- **Solution:** Make sure you called `SetFontAndSize()` before `BeginText()`
- **Solution:** Check that coordinates are within page bounds

**Problem:** Shapes are not visible
- **Solution:** Call `Stroke()`, `Fill()`, or `FillStroke()` after drawing the path
- **Solution:** Check that line width and colors are set

**Problem:** File size is too large
- **Solution:** Enable compression with `pdf.SetCompressionMode(HpdfCompressionMode.All)`

**Problem:** Text appears in wrong location
- **Solution:** Remember that (0,0) is bottom-left, not top-left
- **Solution:** Use `page.Height - y` to position from the top

---

[← Back to Documentation Index](../INDEX.md)

*Last updated: 2025-10-31*
