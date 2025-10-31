# HpdfPage Class

[← Back to Documentation Index](../../INDEX.md)

**Namespace:** `Haru.Doc`

---

## Overview

`HpdfPage` represents a single page in a PDF document. It serves as the target for all graphics and text operations. All drawing,  text rendering, and annotations are added to specific pages.

### Purpose

- **Page dimensions** - Width, height, and various PDF boxes (MediaBox, CropBox, TrimBox, BleedBox, ArtBox)
- **Content stream management** - Stores all graphics and text commands
- **Graphics state tracking** - Current colors, line widths, fonts, transformations
- **Resource management** - Fonts, images, and extended graphics states used on the page
- **Annotations** - Links, notes, and form fields

### Basic Usage

```csharp
using Haru.Doc;
using Haru.Font;

var pdf = new HpdfDocument();
var font = new HpdfFont(pdf.Xref, HpdfStandardFont.Helvetica, "F1");

// Add a page
var page = pdf.AddPage();

// Page is ready for content
page.SetFontAndSize(font, 12);
page.BeginText();
page.MoveTextPos(100, 700);
page.ShowText("Content on the page");
page.EndText();

pdf.SaveToFile("output.pdf");
```

---

## Properties

### Width

Gets or sets the page width in points.

```csharp
public float Width { get; set; }
```

**Units:** Points (72 points = 1 inch)

**Example:**

```csharp
var page = pdf.AddPage();
Console.WriteLine($"Default width: {page.Width}");  // 595.276 (A4)

page.Width = 612;  // Letter width
Console.WriteLine($"New width: {page.Width}");      // 612
```

### Height

Gets or sets the page height in points.

```csharp
public float Height { get; set; }
```

**Units:** Points (72 points = 1 inch)

**Example:**

```csharp
var page = pdf.AddPage();
Console.WriteLine($"Default height: {page.Height}");  // 841.89 (A4)

page.Height = 792;  // Letter height
Console.WriteLine($"New height: {page.Height}");      // 792
```

### Dict

Gets the underlying PDF dictionary object for this page (advanced use).

```csharp
public HpdfDict Dict { get; }
```

**Usage:** For low-level PDF manipulation.

### Parent

Gets the parent pages object.

```csharp
public HpdfPages? Parent { get; }
```

### Contents

Gets the content stream for this page.

```csharp
public HpdfStreamObject Contents { get; }
```

**Usage:** The content stream contains all graphics and text operators.

### Stream

Gets the underlying memory stream for writing PDF operators.

```csharp
public HpdfMemoryStream Stream { get; }
```

**Usage:** For low-level content stream manipulation (advanced).

### GraphicsState

Gets the current graphics state.

```csharp
public HpdfGraphicsState GraphicsState { get; }
```

**Contains:**
- Current colors (stroke and fill)
- Line width, cap, join
- Current font and font size
- Text properties (character/word spacing, etc.)
- Transformation matrix
- Blend mode and transparency

**Example:**

```csharp
var page = pdf.AddPage();

page.SetRgbFill(1, 0, 0);  // Red
Console.WriteLine($"Fill color: {page.GraphicsState.RgbFill}");

page.SetLineWidth(3);
Console.WriteLine($"Line width: {page.GraphicsState.LineWidth}");
```

### CurrentPos

Gets or sets the current position in the path.

```csharp
public HpdfPoint CurrentPos { get; set; }
```

**Usage:** Tracks the current point for path operations (MoveTo, LineTo, etc.).

### TextMatrix

Gets the current text matrix (internal).

```csharp
public HpdfTransMatrix TextMatrix { get; }
```

### TextLineMatrix

Gets the current text line matrix (internal).

```csharp
public HpdfTransMatrix TextLineMatrix { get; }
```

### CurrentFont

Gets the current font set for this page.

```csharp
public HpdfFont? CurrentFont { get; }
```

**Example:**

```csharp
var page = pdf.AddPage();
var font = new HpdfFont(pdf.Xref, HpdfStandardFont.Helvetica, "F1");

page.SetFontAndSize(font, 16);
Console.WriteLine($"Current font: {page.CurrentFont.BaseFont}");  // "Helvetica"
```

---

## Constants

### DefaultWidth

Default page width in points (A4 width).

```csharp
public const float DefaultWidth = 595.276f;
```

### DefaultHeight

Default page height in points (A4 height).

```csharp
public const float DefaultHeight = 841.89f;
```

---

## Page Sizing Methods

### SetSize(HpdfPageSize, HpdfPageDirection)

Sets the page size using a predefined size and orientation.

```csharp
public void SetSize(HpdfPageSize size, HpdfPageDirection direction)
```

**Parameters:**
- `size` - Standard page size
- `direction` - Portrait or Landscape

**Example:**

```csharp
var page = pdf.AddPage();

// A4 portrait (default)
page.SetSize(HpdfPageSize.A4, HpdfPageDirection.Portrait);

// Letter landscape
page.SetSize(HpdfPageSize.Letter, HpdfPageDirection.Landscape);

// A3 portrait
page.SetSize(HpdfPageSize.A3, HpdfPageDirection.Portrait);
```

**Available Sizes:**
- Letter (612 × 792 pt)
- Legal (612 × 1008 pt)
- A3 (842 × 1191 pt)
- A4 (595 × 842 pt)
- A5 (420 × 595 pt)
- B4 (729 × 1032 pt)
- B5 (516 × 729 pt)

### SetSize(float, float)

Sets the page size using custom width and height.

```csharp
public void SetSize(float width, float height)
```

**Parameters:**
- `width` - Page width in points
- `height` - Page height in points

**Example:**

```csharp
// Square page
page.SetSize(600, 600);

// Business card (3.5" × 2" = 252 × 144 points)
page.SetSize(252, 144);

// Wide banner
page.SetSize(1200, 400);
```

### SetMediaBox(float, float, float, float)

Sets the MediaBox for this page (advanced).

```csharp
public void SetMediaBox(float llx, float lly, float urx, float ury)
```

**Parameters:**
- `llx` - Lower-left X coordinate
- `lly` - Lower-left Y coordinate
- `urx` - Upper-right X coordinate
- `ury` - Upper-right Y coordinate

**Usage:** The MediaBox defines the boundaries of the physical medium.

**Example:**

```csharp
// Standard usage: origin at (0,0)
page.SetMediaBox(0, 0, 612, 792);  // Letter size

// Advanced: offset MediaBox
page.SetMediaBox(10, 10, 605, 832);
```

---

## PDF Page Boxes

PDF supports multiple "boxes" that define different regions of a page. These are primarily used in professional printing workflows.

### SetCropBox

Sets the CropBox, which defines the region to which page contents are clipped when displayed or printed.

```csharp
public void SetCropBox(float left, float bottom, float right, float top)
public void SetCropBox(HpdfBox box)
```

**Example:**

```csharp
// Crop 1/2 inch from each edge
page.SetCropBox(36, 36, page.Width - 36, page.Height - 36);

// Using HpdfBox
var cropBox = new HpdfBox(36, 36, page.Width - 36, page.Height - 36);
page.SetCropBox(cropBox);
```

### GetCropBox()

Gets the CropBox if set.

```csharp
public HpdfBox? GetCropBox()
```

**Returns:** `HpdfBox` or null if not set.

### SetBleedBox

Sets the BleedBox, which defines the region for production environments (printing with bleed).

```csharp
public void SetBleedBox(float left, float bottom, float right, float top)
public void SetBleedBox(HpdfBox box)
```

**Usage:** The bleed box extends beyond the TrimBox to account for printing inaccuracies.

**Example:**

```csharp
// Extend 0.125 inch (9 points) beyond trim for bleed
float bleed = 9;
page.SetBleedBox(-bleed, -bleed, page.Width + bleed, page.Height + bleed);
```

### GetBleedBox()

Gets the BleedBox if set.

```csharp
public HpdfBox? GetBleedBox()
```

### SetTrimBox

Sets the TrimBox, which defines the intended dimensions after trimming.

```csharp
public void SetTrimBox(float left, float bottom, float right, float top)
public void SetTrimBox(HpdfBox box)
```

**Example:**

```csharp
// Final trimmed size
page.SetTrimBox(9, 9, page.Width - 9, page.Height - 9);
```

### GetTrimBox()

Gets the TrimBox if set.

```csharp
public HpdfBox? GetTrimBox()
```

### SetArtBox

Sets the ArtBox, which defines the extent of meaningful content.

```csharp
public void SetArtBox(float left, float bottom, float right, float top)
public void SetArtBox(HpdfBox box)
```

**Example:**

```csharp
// Define content area (with margins)
page.SetArtBox(50, 50, page.Width - 50, page.Height - 50);
```

### GetArtBox()

Gets the ArtBox if set.

```csharp
public HpdfBox? GetArtBox()
```

---

## Annotation Methods

### CreateTextAnnotation

Creates a text annotation (sticky note) on the page.

```csharp
public HpdfTextAnnotation CreateTextAnnotation(
    HpdfRect rect,
    string text,
    HpdfAnnotationIcon icon = HpdfAnnotationIcon.Note)
```

**Parameters:**
- `rect` - Position and size of the annotation
- `text` - Text content
- `icon` - Icon to display (Note, Comment, Help, etc.)

**Returns:** `HpdfTextAnnotation` object.

**Example:**

```csharp
var page = pdf.AddPage();

// Add a note at top-right
var note = page.CreateTextAnnotation(
    new HpdfRect(500, 750, 20, 20),
    "Remember to review this section!",
    HpdfAnnotationIcon.Comment
);
```

**Available icons:**
- `Note` (default)
- `Comment`
- `Help`
- `Key`
- `NewParagraph`
- `Paragraph`
- `Insert`

### CreateLinkAnnotation(HpdfRect, string)

Creates a URI link annotation (external link).

```csharp
public HpdfLinkAnnotation CreateLinkAnnotation(HpdfRect rect, string uri)
```

**Parameters:**
- `rect` - Clickable area
- `uri` - URL to link to

**Returns:** `HpdfLinkAnnotation` object.

**Example:**

```csharp
var page = pdf.AddPage();

// Create clickable link
var link = page.CreateLinkAnnotation(
    new HpdfRect(100, 700, 200, 20),
    "https://www.example.com"
);

// Add text to show what's clickable
page.SetFontAndSize(font, 12);
page.BeginText();
page.MoveTextPos(100, 702);
page.ShowText("Visit our website");
page.EndText();
```

### CreateLinkAnnotation(HpdfRect, HpdfArray)

Creates an internal link annotation (GoTo destination).

```csharp
public HpdfLinkAnnotation CreateLinkAnnotation(HpdfRect rect, HpdfArray destination)
```

**Parameters:**
- `rect` - Clickable area
- `destination` - PDF destination array

**Example:**

```csharp
var page1 = pdf.AddPage();
var page2 = pdf.AddPage();

// Create destination for page 2
var dest = page2.CreateDestination();
var destArray = dest.ToArray();

// Add link on page 1 to page 2
var link = page1.CreateLinkAnnotation(
    new HpdfRect(100, 700, 150, 20),
    destArray
);
```

### AddWidgetAnnotation

Adds a widget annotation (form field) to the page.

```csharp
public void AddWidgetAnnotation(HpdfWidgetAnnotation widget)
```

**Parameters:**
- `widget` - The widget annotation (form field)

**Example:**

```csharp
var acroForm = pdf.GetOrCreateAcroForm();
var page = pdf.AddPage();

// Create text field
var textField = acroForm.CreateTextField(
    page,
    "username",
    new HpdfRect(100, 700, 200, 20)
);

// Widget is automatically added to the page
```

---

## Graphics State Methods

### GSave()

Saves the current graphics state onto the stack.

```csharp
public void GSave()
```

**Usage:** Use before making temporary changes to graphics state.

**Example:**

```csharp
// Save current state
page.GSave();

// Make temporary changes
page.SetLineWidth(5);
page.SetRgbStroke(1, 0, 0);
page.Rectangle(100, 100, 200, 100);
page.Stroke();

// Restore previous state
page.GRestore();

// Back to previous line width and color
page.Rectangle(100, 250, 200, 100);
page.Stroke();
```

### GRestore()

Restores the graphics state from the stack.

```csharp
public void GRestore()
```

**Note:** Must be balanced with `GSave()` calls.

---

## Transition Effects (Slide Shows)

### SetSlideShow

Sets a transition effect for the page (for PDF presentations).

```csharp
public void SetSlideShow(
    HpdfTransitionStyle type,
    float displayTime,
    float transTime)
```

**Parameters:**
- `type` - Transition style
- `displayTime` - Display time in seconds before transitioning
- `transTime` - Transition duration in seconds

**Example:**

```csharp
var page1 = pdf.AddPage();
var page2 = pdf.AddPage();
var page3 = pdf.AddPage();

// Wipe right transition
page1.SetSlideShow(HpdfTransitionStyle.WipeRight, 3.0f, 1.0f);

// Dissolve transition
page2.SetSlideShow(HpdfTransitionStyle.Dissolve, 3.0f, 1.5f);

// Box out transition
page3.SetSlideShow(HpdfTransitionStyle.BoxOut, 3.0f, 1.0f);

// Set document to full screen mode
pdf.Catalog.SetPageMode(HpdfPageMode.FullScreen);
```

**Available transition styles:**
- `Replace` - Simple replacement (no effect)
- `WipeRight`, `WipeLeft`, `WipeUp`, `WipeDown`
- `BarnDoorsHorizontalOut`, `BarnDoorsHorizontalIn`
- `BarnDoorsVerticalOut`, `BarnDoorsVerticalIn`
- `BoxOut`, `BoxIn`
- `BlindsHorizontal`, `BlindsVertical`
- `Dissolve`
- `GlitterRight`, `GlitterDown`, `GlitterTopLeftToBottomRight`

---

## Extension Methods

`HpdfPage` is extended with many methods through extension method classes. These are organized by functionality:

### Graphics Operations

See [HpdfPageGraphics](../extensions/HpdfPageGraphics.md) for:
- **Path operations:** `MoveTo`, `LineTo`, `CurveTo`, `Rectangle`, `Circle`, `Arc`
- **Colors:** `SetRgbStroke`, `SetRgbFill`, `SetCmykStroke`, `SetCmykFill`
- **Line styles:** `SetLineWidth`, `SetLineCap`, `SetLineJoin`, `SetDash`
- **Path painting:** `Stroke`, `Fill`, `FillStroke`, `Clip`, `ClosePathStroke`, `ClosePathFillStroke`
- **Transformations:** `Concat`

**Quick example:**

```csharp
// Draw a colored rectangle
page.SetRgbFill(0.8f, 0.2f, 0.2f);
page.SetRgbStroke(0, 0, 0);
page.SetLineWidth(2);
page.Rectangle(100, 500, 200, 100);
page.FillStroke();
```

### Shape Drawing

See [HpdfPageShapes](../extensions/HpdfPageShapes.md) for:
- `Circle(x, y, radius)`
- `Ellipse(x, y, xRadius, yRadius)`
- `Arc(x, y, radius, startAngle, endAngle)`

**Quick example:**

```csharp
// Draw a circle
page.SetLineWidth(2);
page.Circle(300, 400, 50);
page.Stroke();

// Draw an ellipse
page.Ellipse(300, 250, 80, 40);
page.Stroke();
```

### Text Operations

See [HpdfPageText](../extensions/HpdfPageText.md) for:
- **Text blocks:** `BeginText`, `EndText`
- **Positioning:** `MoveTextPos`, `MoveToNextLine`, `SetTextMatrix`
- **Rendering:** `ShowText`, `ShowTextNextLine`
- **Formatting:** `SetFontAndSize`, `SetTextLeading`, `SetCharSpace`, `SetWordSpace`
- **Styling:** `SetTextRenderingMode`, `SetTextRise`, `SetHorizontalScaling`

**Quick example:**

```csharp
page.SetFontAndSize(font, 12);
page.BeginText();
page.MoveTextPos(100, 700);
page.ShowText("First line");
page.SetTextLeading(20);
page.ShowTextNextLine("Second line (20 points below)");
page.EndText();
```

### Page Extensions

See [HpdfPageExtensions](../extensions/HpdfPageExtensions.md) for:
- `DrawImage(image, x, y, width, height)` - Draw an image
- `TextOut(x, y, text)` - Quick text output (shortcut)
- `TextWidth(text)` - Measure text width
- `CreateDestination()` - Create a destination for links/bookmarks

**Quick example:**

```csharp
// Load and draw image
var image = pdf.LoadPngImageFromFile("logo.png");
page.DrawImage(image, 100, 600, 200, 100);

// Quick text output
page.TextOut(100, 500, "Quick text");

// Measure text
float width = page.TextWidth("Measure this");
```

---

## Complete Examples

### Basic Page with Text and Graphics

```csharp
using Haru.Doc;
using Haru.Font;
using Haru.Types;

var pdf = new HpdfDocument();
var font = new HpdfFont(pdf.Xref, HpdfStandardFont.Helvetica, "F1");

var page = pdf.AddPage(HpdfPageSize.Letter, HpdfPageDirection.Portrait);

// Title
page.SetFontAndSize(font, 24);
page.BeginText();
page.MoveTextPos(50, page.Height - 50);
page.ShowText("Graphics and Text Demo");
page.EndText();

// Draw a filled rectangle
page.SetRgbFill(0.2f, 0.4f, 0.8f);
page.SetRgbStroke(0, 0, 0);
page.SetLineWidth(2);
page.Rectangle(50, 600, 200, 100);
page.FillStroke();

// Draw a circle
page.SetRgbStroke(0.8f, 0.2f, 0.2f);
page.SetLineWidth(3);
page.Circle(400, 650, 50);
page.Stroke();

// Add some text
page.SetFontAndSize(font, 14);
page.BeginText();
page.MoveTextPos(50, 500);
page.ShowText("This page demonstrates basic graphics and text.");
page.EndText();

pdf.SaveToFile("page-demo.pdf");
```

### Multi-Size Pages

```csharp
var pdf = new HpdfDocument();
var font = new HpdfFont(pdf.Xref, HpdfStandardFont.Helvetica, "F1");

// A4 portrait page
var page1 = pdf.AddPage(HpdfPageSize.A4, HpdfPageDirection.Portrait);
page1.SetFontAndSize(font, 16);
page1.BeginText();
page1.MoveTextPos(50, page1.Height - 50);
page1.ShowText($"A4 Portrait: {page1.Width} x {page1.Height}");
page1.EndText();

// Letter landscape page
var page2 = pdf.AddPage(HpdfPageSize.Letter, HpdfPageDirection.Landscape);
page2.SetFontAndSize(font, 16);
page2.BeginText();
page2.MoveTextPos(50, page2.Height - 50);
page2.ShowText($"Letter Landscape: {page2.Width} x {page2.Height}");
page2.EndText();

// Custom size page (square)
var page3 = pdf.AddPage(600, 600);
page3.SetFontAndSize(font, 16);
page3.BeginText();
page3.MoveTextPos(50, page3.Height - 50);
page3.ShowText($"Custom Square: {page3.Width} x {page3.Height}");
page3.EndText();

pdf.SaveToFile("multi-size.pdf");
```

### Page with Annotations

```csharp
var pdf = new HpdfDocument();
var font = new HpdfFont(pdf.Xref, HpdfStandardFont.Helvetica, "F1");

var page = pdf.AddPage();

// Add text
page.SetFontAndSize(font, 14);
page.BeginText();
page.MoveTextPos(100, 700);
page.ShowText("Click the link below:");
page.EndText();

// Add clickable link
page.SetRgbFill(0, 0, 1);  // Blue text
page.BeginText();
page.MoveTextPos(100, 670);
page.ShowText("Visit Example.com");
page.EndText();

var link = page.CreateLinkAnnotation(
    new HpdfRect(100, 667, 170, 20),
    "https://www.example.com"
);

// Add a sticky note
var note = page.CreateTextAnnotation(
    new HpdfRect(500, 750, 20, 20),
    "This is an important note!",
    HpdfAnnotationIcon.Note
);

pdf.SaveToFile("annotations.pdf");
```

### Page with Graphics State Management

```csharp
var pdf = new HpdfDocument();
var page = pdf.AddPage();

// Default state
page.SetLineWidth(1);
page.SetRgbStroke(0, 0, 0);

// Draw with default state
page.Rectangle(50, 700, 100, 50);
page.Stroke();

// Save state and modify
page.GSave();
page.SetLineWidth(5);
page.SetRgbStroke(1, 0, 0);

// Draw with modified state
page.Rectangle(200, 700, 100, 50);
page.Stroke();

// Restore original state
page.GRestore();

// Draw with restored state (thin, black)
page.Rectangle(350, 700, 100, 50);
page.Stroke();

pdf.SaveToFile("state-management.pdf");
```

### Slide Show Presentation

```csharp
var pdf = new HpdfDocument();
var font = new HpdfFont(pdf.Xref, HpdfStandardFont.HelveticaBold, "F1");

// Create 5 slides with different transitions
string[] titles = { "Introduction", "Main Points", "Details", "Summary", "Thank You" };
var transitions = new[] {
    HpdfTransitionStyle.WipeRight,
    HpdfTransitionStyle.Dissolve,
    HpdfTransitionStyle.BoxOut,
    HpdfTransitionStyle.BlindsHorizontal,
    HpdfTransitionStyle.GlitterDown
};

for (int i = 0; i < 5; i++)
{
    var page = pdf.AddPage(HpdfPageSize.A4, HpdfPageDirection.Landscape);

    // Add title
    page.SetFontAndSize(font, 36);
    string title = titles[i];
    float titleWidth = font.MeasureText(title, 36);
    page.BeginText();
    page.MoveTextPos((page.Width - titleWidth) / 2, page.Height / 2);
    page.ShowText(title);
    page.EndText();

    // Set transition (display for 5 seconds, transition for 1 second)
    page.SetSlideShow(transitions[i], 5.0f, 1.0f);
}

// Set full screen mode
pdf.Catalog.SetPageMode(HpdfPageMode.FullScreen);

pdf.SaveToFile("presentation.pdf");
```

---

## Best Practices

### 1. Always Call BeginText/EndText

```csharp
// ❌ WRONG
page.MoveTextPos(100, 700);
page.ShowText("Text");

// ✓ CORRECT
page.BeginText();
page.MoveTextPos(100, 700);
page.ShowText("Text");
page.EndText();
```

### 2. Use GSave/GRestore for Temporary Changes

```csharp
// Save state before temporary changes
page.GSave();

// Temporary styling
page.SetLineWidth(10);
page.SetRgbStroke(1, 0, 0);
// ... draw something ...

// Restore previous state
page.GRestore();
```

### 3. Set Font Before Text Operations

```csharp
// Always set font and size before showing text
page.SetFontAndSize(font, 12);
page.BeginText();
page.MoveTextPos(100, 700);
page.ShowText("Text");
page.EndText();
```

### 4. Check Page Bounds

```csharp
float x = 100;
float y = 700;

// Ensure position is within page bounds
if (x >= 0 && x <= page.Width && y >= 0 && y <= page.Height)
{
    page.BeginText();
    page.MoveTextPos(x, y);
    page.ShowText("Text");
    page.EndText();
}
```

### 5. Use Appropriate Units

```csharp
// Remember: 72 points = 1 inch

// 1 inch margin
float marginPoints = 72;

// 0.5 inch margin
float halfInchMargin = 36;

// Calculate from mm (A4 is 210mm × 297mm)
float mm = 10;
float points = mm * 72 / 25.4f;
```

---

## Related Types

### HpdfGraphicsState Class

Contains current graphics state:
- Colors (RGB/CMYK fill and stroke)
- Line properties (width, cap, join, dash)
- Font and font size
- Text properties (spacing, leading, etc.)
- Transformation matrix

### HpdfBox Struct

Represents a PDF box (CropBox, BleedBox, TrimBox, ArtBox):

```csharp
public struct HpdfBox
{
    public float Left { get; set; }
    public float Bottom { get; set; }
    public float Right { get; set; }
    public float Top { get; set; }
}
```

### HpdfRect Struct

Represents a rectangle (for annotations, etc.):

```csharp
public struct HpdfRect
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
}
```

---

## See Also

### Core Classes
- [HpdfDocument](HpdfDocument.md) - Document management
- [HpdfFont](HpdfFont.md) - Font handling
- [HpdfImage](HpdfImage.md) - Image operations

### Extension Methods
- [HpdfPageGraphics](../extensions/HpdfPageGraphics.md) - Graphics operations
- [HpdfPageShapes](../extensions/HpdfPageShapes.md) - Shape drawing
- [HpdfPageText](../extensions/HpdfPageText.md) - Text operations
- [HpdfPageExtensions](../extensions/HpdfPageExtensions.md) - Convenience methods

### Guides
- [Getting Started](../../guides/GettingStarted.md) - Step-by-step tutorial
- [Graphics Guide](../../guides/GraphicsGuide.md) - Drawing techniques
- [Text Guide](../../guides/TextGuide.md) - Text rendering

---

[← Back to Documentation Index](../../INDEX.md)

*Last updated: 2025-10-31*
