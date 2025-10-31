# Quick Start Guide

[← Back to Documentation Index](INDEX.md)

---

## Installation

### Requirements
- **.NET 8.0 or later**
- No additional prerequisites

### NuGet Installation

Install Haru.NET via NuGet Package Manager:

```bash
dotnet add package Haru
```

Or add to your `.csproj` file:

```xml
<PackageReference Include="Haru" Version="1.0.0" />
```

### Dependencies

Haru.NET automatically installs two dependencies:
- **StbImageSharp** - Image parsing (permissive license)
- **System.Text.Encoding.CodePages** - International character encoding support

---

## Basic Concepts

### The Document Object Model

Haru.NET follows a hierarchical structure:

```
HpdfDocument (the document)
  └─ HpdfPage (pages in the document)
      ├─ Content (graphics and text operations)
      ├─ HpdfFont (fonts for text)
      └─ HpdfImage (images to display)
```

### Core Classes

- **HpdfDocument** - Main entry point, manages document lifecycle
- **HpdfPage** - Individual page, target for all graphics/text operations
- **HpdfFont** - Font object for rendering text
- **HpdfImage** - Image object for embedding pictures

### Extension Methods

Haru.NET organizes functionality using extension methods:

- **HpdfPageGraphics** - Lines, curves, colors, transformations
- **HpdfPageShapes** - Circles, ellipses, arcs
- **HpdfPageText** - Text rendering operations
- **HpdfDocumentExtensions** - Font and image loading

---

## Your First PDF

### Minimal Example

Create a simple PDF with text:

```csharp
using Haru.Doc;
using Haru.Font;

// Create document
var pdf = new HpdfDocument();

// Create font
var font = new HpdfFont(pdf.Xref, HpdfStandardFont.Helvetica, "F1");

// Add page (A4 portrait by default)
var page = pdf.AddPage();

// Write text
page.SetFontAndSize(font, 24);
page.BeginText();
page.MoveTextPos(100, 700);
page.ShowText("Hello, PDF World!");
page.EndText();

// Save
pdf.SaveToFile("hello.pdf");
```

**Key Points:**
1. Create `HpdfDocument` first
2. Create fonts using `HpdfFont` constructor with the document's Xref
3. Add pages with `AddPage()`
4. Text requires `BeginText()` / `EndText()` blocks
5. Position text with `MoveTextPos(x, y)` (bottom-left origin)
6. Save with `SaveToFile(filename)`

---

## Common Tasks

### Creating Pages with Custom Sizes

```csharp
using Haru.Types;

var pdf = new HpdfDocument();

// Letter size, portrait
var page1 = pdf.AddPage(HpdfPageSize.Letter, HpdfPageDirection.Portrait);

// A4, landscape
var page2 = pdf.AddPage(HpdfPageSize.A4, HpdfPageDirection.Landscape);

// Custom size (width, height in points; 1 point = 1/72 inch)
var page3 = pdf.AddPage();
page3.SetSize(600, 800);

pdf.SaveToFile("pages.pdf");
```

### Drawing Shapes

```csharp
var pdf = new HpdfDocument();
var page = pdf.AddPage();

// Rectangle (x, y, width, height)
page.SetLineWidth(2);
page.Rectangle(100, 600, 200, 100);
page.Stroke();

// Filled rectangle with color
page.SetRgbFill(0.8f, 0.2f, 0.2f);  // Red
page.Rectangle(100, 450, 200, 100);
page.Fill();

// Circle (x, y, radius)
page.Circle(200, 300, 50);
page.Stroke();

// Line
page.MoveTo(50, 200);
page.LineTo(300, 200);
page.Stroke();

pdf.SaveToFile("shapes.pdf");
```

### Working with Colors

```csharp
var page = pdf.AddPage();

// RGB colors (values from 0.0 to 1.0)
page.SetRgbStroke(1.0f, 0.0f, 0.0f);  // Red stroke
page.SetRgbFill(0.0f, 0.0f, 1.0f);     // Blue fill

// CMYK colors
page.SetCmykStroke(0.0f, 1.0f, 1.0f, 0.0f);  // Cyan stroke
page.SetCmykFill(1.0f, 0.0f, 0.0f, 0.0f);     // Magenta fill

// Draw with current colors
page.Rectangle(100, 500, 200, 100);
page.FillStroke();  // Fill and stroke in one operation
```

### Using Different Fonts

```csharp
var pdf = new HpdfDocument();
var page = pdf.AddPage();

// Standard PDF fonts (no embedding required)
var helvetica = new HpdfFont(pdf.Xref, HpdfStandardFont.Helvetica, "F1");
var times = new HpdfFont(pdf.Xref, HpdfStandardFont.TimesRoman, "F2");
var courier = new HpdfFont(pdf.Xref, HpdfStandardFont.Courier, "F3");

// Use fonts
page.SetFontAndSize(helvetica, 16);
page.BeginText();
page.MoveTextPos(50, 700);
page.ShowText("Helvetica font");
page.EndText();

page.SetFontAndSize(times, 16);
page.BeginText();
page.MoveTextPos(50, 650);
page.ShowText("Times Roman font");
page.EndText();

page.SetFontAndSize(courier, 16);
page.BeginText();
page.MoveTextPos(50, 600);
page.ShowText("Courier font");
page.EndText();

pdf.SaveToFile("fonts.pdf");
```

### Loading TrueType Fonts

```csharp
using Haru.Font;

var pdf = new HpdfDocument();
var page = pdf.AddPage();

// Load TrueType font from file
var customFont = pdf.GetFont("path/to/font.ttf", "CP1252");

page.SetFontAndSize(customFont, 20);
page.BeginText();
page.MoveTextPos(50, 700);
page.ShowText("Text in custom font");
page.EndText();

pdf.SaveToFile("custom-font.pdf");
```

### Embedding Images

```csharp
var pdf = new HpdfDocument();
var page = pdf.AddPage();

// Load PNG image
var pngImage = pdf.LoadPngImageFromFile("path/to/image.png");

// Load JPEG image
var jpegImage = pdf.LoadJpegImageFromFile("path/to/photo.jpg");

// Draw image at position with size
page.DrawImage(pngImage, 100, 600, 200, 150);

// Draw image maintaining aspect ratio
float aspectRatio = (float)jpegImage.Height / jpegImage.Width;
float width = 300;
float height = width * aspectRatio;
page.DrawImage(jpegImage, 100, 400, width, height);

pdf.SaveToFile("images.pdf");
```

### Setting Document Metadata

```csharp
var pdf = new HpdfDocument();

// Set metadata
pdf.Info.SetTitle("My Document");
pdf.Info.SetAuthor("John Doe");
pdf.Info.SetSubject("Document Subject");
pdf.Info.SetKeywords("PDF, Haru, Example");
pdf.Info.SetCreator("My Application");

var page = pdf.AddPage();
// ... add content ...

pdf.SaveToFile("metadata.pdf");
```

### Saving to Memory

```csharp
var pdf = new HpdfDocument();
var page = pdf.AddPage();
// ... add content ...

// Save to memory stream
byte[] pdfBytes = pdf.SaveToMemory();

// Use bytes (e.g., send over network, return from web API)
// await File.WriteAllBytesAsync("output.pdf", pdfBytes);
```

---

## Coordinate System

### Understanding PDF Coordinates

PDF uses a **bottom-left origin** coordinate system:

- **(0, 0)** is at the **bottom-left** corner
- **X increases** moving **right**
- **Y increases** moving **up**

### Page Dimensions

Common page sizes in points (1 point = 1/72 inch):

| Size | Width | Height | Portrait | Landscape |
|------|-------|--------|----------|-----------|
| **A4** | 595 | 842 | 595×842 | 842×595 |
| **Letter** | 612 | 792 | 612×792 | 792×612 |
| **Legal** | 612 | 1008 | 612×1008 | 1008×612 |

### Positioning Examples

```csharp
var page = pdf.AddPage(HpdfPageSize.A4, HpdfPageDirection.Portrait);
// Page size: 595 x 842

// Top-left corner
float x = 50;
float y = page.Height - 50;  // 842 - 50 = 792

// Bottom-right corner
x = page.Width - 50;   // 595 - 50 = 545
y = 50;

// Center of page
x = page.Width / 2;    // 297.5
y = page.Height / 2;   // 421

// Position text from top
page.BeginText();
page.MoveTextPos(50, page.Height - 100);  // 50 from left, 100 from top
page.ShowText("Text positioned from top");
page.EndText();
```

---

## Error Handling

### Basic Error Handling

```csharp
try
{
    var pdf = new HpdfDocument();
    var page = pdf.AddPage();

    // ... PDF operations ...

    pdf.SaveToFile("output.pdf");
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Error creating PDF: {ex.Message}");
    // Handle error appropriately
}
```

### Common Issues

**Font not found:**
```csharp
// Ensure file path is correct and file exists
if (!File.Exists(fontPath))
{
    throw new FileNotFoundException($"Font file not found: {fontPath}");
}
var font = pdf.GetFont(fontPath, "CP1252");
```

**Image loading failure:**
```csharp
// Validate image file before loading
if (!File.Exists(imagePath))
{
    throw new FileNotFoundException($"Image not found: {imagePath}");
}
var image = pdf.LoadPngImageFromFile(imagePath);
```

**Text outside page bounds:**
```csharp
// Ensure coordinates are within page boundaries
if (x < 0 || x > page.Width || y < 0 || y > page.Height)
{
    Console.WriteLine($"Warning: Position ({x}, {y}) outside page bounds");
}
```

---

## Best Practices

### 1. Use `using` Statements (Future)
Currently, `HpdfDocument` manages its own resources. Future versions may implement `IDisposable`.

### 2. Reuse Font Objects
Create font objects once and reuse them across pages:

```csharp
var pdf = new HpdfDocument();
var font = new HpdfFont(pdf.Xref, HpdfStandardFont.Helvetica, "F1");

var page1 = pdf.AddPage();
page1.SetFontAndSize(font, 12);
// ... use font ...

var page2 = pdf.AddPage();
page2.SetFontAndSize(font, 12);  // Reuse same font
// ... use font ...
```

### 3. Graphics State Management
Save and restore graphics state when making temporary changes:

```csharp
page.GSave();  // Save current state

// Make temporary changes
page.SetLineWidth(5);
page.SetRgbStroke(1, 0, 0);
// ... draw something ...

page.GRestore();  // Restore previous state
```

### 4. Text Measurement
Measure text before positioning to ensure it fits:

```csharp
var text = "Sample text";
var fontSize = 24f;
float textWidth = font.MeasureText(text, fontSize);

// Center text on page
float x = (page.Width - textWidth) / 2;
float y = page.Height / 2;

page.SetFontAndSize(font, fontSize);
page.BeginText();
page.MoveTextPos(x, y);
page.ShowText(text);
page.EndText();
```

### 5. Compression
Enable compression for smaller file sizes:

```csharp
var pdf = new HpdfDocument();
pdf.SetCompressionMode(HpdfCompressionMode.All);
// ... create content ...
pdf.SaveToFile("compressed.pdf");
```

---

## Next Steps

### Learn More
- [Getting Started Tutorial](guides/GettingStarted.md) - Comprehensive step-by-step guide
- [Fonts Guide](guides/FontsGuide.md) - TrueType, Type1, and CJK fonts
- [Graphics Guide](guides/GraphicsGuide.md) - Advanced drawing techniques
- [Text Guide](guides/TextGuide.md) - Text formatting and layout
- [Images Guide](guides/ImagesGuide.md) - Image handling in detail

### API Reference
- [HpdfDocument](api/core/HpdfDocument.md) - Complete document API
- [HpdfPage](api/core/HpdfPage.md) - Page manipulation reference
- [HpdfFont](api/core/HpdfFont.md) - Font system reference

### Examples
- [Basic Document Examples](examples/BasicDocument.md)
- [Working with Fonts](examples/WorkingWithFonts.md)
- [Drawing Graphics](examples/DrawingGraphics.md)

---

[← Back to Documentation Index](INDEX.md)

*Last updated: 2025-10-31*
