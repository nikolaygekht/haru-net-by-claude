# PDF/A Compliance Implementation Guide

This document provides a comprehensive guide for implementing PDF/A support in the Haru C# port.

## What is PDF/A?

PDF/A is an ISO-standardized version of PDF specialized for digital preservation of electronic documents. Key characteristics:

- **100% self-contained** - All information necessary for displaying the document is embedded
- **Long-term archival** - Ensures documents can be reproduced exactly the same way in years to come
- **No external dependencies** - Fonts, images, color profiles all embedded
- **Widely required** - US Supreme Court, federal government (508 compliance), many enterprises

## PDF/A Standards Overview

| Standard | Based On | Year | Key Features |
|----------|----------|------|--------------|
| PDF/A-1a | PDF 1.4 | 2005 | Level A: Full accessibility + structure tags |
| PDF/A-1b | PDF 1.4 | 2005 | Level B: Visual reproduction only (easier) |
| PDF/A-2  | PDF 1.7 | 2011 | Adds JPEG2000, transparency, layers |
| PDF/A-3  | PDF 1.7 | 2012 | Allows embedded files (e.g., source data) |
| PDF/A-4  | PDF 2.0 | 2020 | Based on latest PDF spec |

**Recommendation:** Start with **PDF/A-1b** (Level B conformance) - it's the most widely supported and doesn't require complex accessibility tagging.

---

## Current Implementation Status

### What We Already Have ✅

1. **Document Information Dictionary** (`HpdfInfo.cs`) - Fully implemented
   - Title, Author, Subject, Keywords, Creator, Producer
   - Creation and modification dates with timezone support
   - Custom metadata support
   - ✅ 16 tests passing

2. **Basic PDF Structure** - Complete
   - Catalog, Pages, Page tree
   - Cross-reference system with trailer
   - Object management

3. **Compression** - Implemented
   - FlateDecode (zlib) support via `System.IO.Compression`

4. **PDF Version Control** - Supported
   - Versions 1.2 through 1.7
   - Can set to 1.4 for PDF/A-1 compliance

5. **Stream Objects** - Working
   - `HpdfStreamObject` for content and metadata streams

---

## What We Need for PDF/A-1b Compliance

### 1. XMP Metadata Stream (CRITICAL - Required)

**Status:** ❌ Not yet implemented
**Effort:** ~1 day
**Priority:** HIGH

#### What is XMP?

XMP (Extensible Metadata Platform) is an XML-based metadata format that PDF/A requires for all document metadata. Unlike the traditional Info dictionary, XMP is:
- Structured as RDF/XML
- Extensible with custom schemas
- Machine-readable
- Required to be embedded as a stream in the Catalog

#### Requirements

The XMP metadata must:
1. Be stored as a Metadata stream in the document Catalog
2. Have `Type` = "Metadata" and `Subtype` = "XML"
3. Contain RDF/XML formatted metadata
4. Mirror all Info dictionary fields in appropriate XMP namespaces
5. Include PDF/A identification namespace

#### XMP Namespace Mappings

| Info Dictionary Field | XMP Namespace | XMP Property |
|-----------------------|---------------|--------------|
| Title | Dublin Core (dc) | `dc:title` |
| Author | Dublin Core (dc) | `dc:creator` |
| Subject | Dublin Core (dc) | `dc:description` |
| Keywords | Adobe PDF (pdf) | `pdf:Keywords` |
| Producer | Adobe PDF (pdf) | `pdf:Producer` |
| Creator | Adobe XMP (xmp) | `xmp:CreatorTool` |
| CreationDate | Adobe XMP (xmp) | `xmp:CreateDate` |
| ModDate | Adobe XMP (xmp) | `xmp:ModifyDate` |

#### XMP Structure Template

```xml
<?xpacket begin='' id='W5M0MpCehiHzreSzNTczkc9d'?>
<x:xmpmeta xmlns:x='adobe:ns:meta/' x:xmptk='XMP toolkit 2.9.1-13, framework 1.6'>
  <rdf:RDF xmlns:rdf='http://www.w3.org/1999/02/22-rdf-syntax-ns#'
           xmlns:iX='http://ns.adobe.com/iX/1.0/'>

    <!-- Dublin Core metadata -->
    <rdf:Description xmlns:dc='http://purl.org/dc/elements/1.1/' rdf:about=''>
      <dc:title>
        <rdf:Alt>
          <rdf:li xml:lang="x-default">Document Title</rdf:li>
        </rdf:Alt>
      </dc:title>
      <dc:creator>
        <rdf:Seq>
          <rdf:li>Author Name</rdf:li>
        </rdf:Seq>
      </dc:creator>
      <dc:description>
        <rdf:Alt>
          <rdf:li xml:lang="x-default">Subject/Description</rdf:li>
        </rdf:Alt>
      </dc:description>
    </rdf:Description>

    <!-- XMP metadata -->
    <rdf:Description xmlns:xmp='http://ns.adobe.com/xap/1.0/' rdf:about=''>
      <xmp:CreatorTool>Application Name</xmp:CreatorTool>
      <xmp:CreateDate>2025-10-08T12:30:45+00:00</xmp:CreateDate>
      <xmp:ModifyDate>2025-10-08T14:15:30+00:00</xmp:ModifyDate>
    </rdf:Description>

    <!-- PDF metadata -->
    <rdf:Description xmlns:pdf='http://ns.adobe.com/pdf/1.3/' rdf:about=''>
      <pdf:Keywords>keyword1, keyword2, keyword3</pdf:Keywords>
      <pdf:Producer>Haru Free PDF Library</pdf:Producer>
    </rdf:Description>

    <!-- PDF/A identification -->
    <rdf:Description rdf:about=''
                     xmlns:pdfaid='http://www.aiim.org/pdfa/ns/id/'
                     pdfaid:part='1'
                     pdfaid:conformance='B'/>
  </rdf:RDF>
</x:xmpmeta>
<?xpacket end='w'?>
```

#### Date Format Conversion

PDF dates: `D:YYYYMMDDHHmmSSOHH'mm'`
XMP dates: `YYYY-MM-DDTHH:mm:SS+HH:mm` (ISO 8601)

Example conversion:
- PDF: `D:20231215143045+05'30'`
- XMP: `2023-12-15T14:30:45+05:30`

#### Implementation Classes Needed

```csharp
namespace Haru.PdfA
{
    /// <summary>
    /// Generates XMP metadata for PDF/A compliance.
    /// </summary>
    public class HpdfXmpMetadata
    {
        public static HpdfStreamObject CreateXmpStream(HpdfXref xref, HpdfInfo info, HpdfPdfAType pdfaType);
        private static string ConvertPdfDateToXmp(string pdfDate);
        private static string EscapeXml(string text);
    }

    public enum HpdfPdfAType
    {
        PdfA1A,  // Level A - Full accessibility
        PdfA1B   // Level B - Visual reproduction only
    }
}
```

#### Integration Points

1. Add `Metadata` entry to Catalog dictionary
2. Create XMP stream from Info dictionary during `Save()`
3. Set PDF version to 1.4

---

### 2. Output Intent with ICC Color Profile (CRITICAL - Required)

**Status:** ❌ Not yet implemented
**Effort:** ~0.5 day
**Priority:** HIGH

#### What is an Output Intent?

An Output Intent specifies the intended output condition for the PDF, ensuring consistent color reproduction across different devices and platforms. It includes an embedded ICC (International Color Consortium) profile.

#### Requirements

For PDF/A-1b, we need:
1. An `OutputIntents` array in the Catalog
2. At least one Output Intent dictionary with:
   - `Type` = "OutputIntent"
   - `S` = "GTS_PDFA1" (identifies PDF/A conformance)
   - `OutputConditionIdentifier` - Human-readable name (e.g., "sRGB IEC61966-2.1")
   - `OutputCondition` - Description of output device
   - `Info` - Additional information
   - `DestOutputProfile` - Stream containing ICC profile data

#### Default ICC Profile: sRGB

For most use cases, the sRGB color space is appropriate:
- **Profile name:** sRGB IEC61966-2.1
- **Number of components:** 3 (RGB)
- **Size:** ~3KB embedded binary data

The sRGB ICC profile can be:
- Downloaded from ICC.org or embedded as a resource
- Embedded in the PDF as a compressed stream

#### Output Intent Structure

```
Catalog
└── OutputIntents [Array]
    └── [0] Dictionary
        ├── Type: /OutputIntent
        ├── S: /GTS_PDFA1
        ├── OutputConditionIdentifier: (sRGB IEC61966-2.1)
        ├── OutputCondition: (sRGB IEC61966-2.1)
        ├── Info: (sRGB IEC61966-2.1)
        └── DestOutputProfile: Stream
            ├── N: 3
            └── Stream data: [ICC profile binary]
```

#### Implementation Classes Needed

```csharp
namespace Haru.PdfA
{
    /// <summary>
    /// Manages Output Intents for PDF/A compliance.
    /// </summary>
    public class HpdfOutputIntent
    {
        public static void AddSrgbOutputIntent(HpdfDocument doc);
        private static byte[] GetEmbeddedSrgbProfile();
    }
}
```

#### ICC Profile Resource

The sRGB ICC profile should be embedded as a resource in the assembly:
- Add `sRGB-IEC61966-2.1.icc` to project as embedded resource
- Load using `Assembly.GetManifestResourceStream()`

---

### 3. Document ID in Trailer (CRITICAL - Required)

**Status:** ❌ Not yet implemented
**Effort:** ~0.5 day
**Priority:** HIGH

#### What is the Document ID?

The document ID is a unique identifier for the PDF file, stored in the trailer dictionary. PDF/A requires it for:
- Document version tracking
- Detecting modifications
- Digital signatures

#### Requirements

The ID must be:
1. An array of two binary strings
2. Each string is a 16-byte (128-bit) identifier
3. Both strings are initially identical
4. Generated using MD5 hash of document metadata + timestamp
5. Added to the trailer dictionary with key "ID"

#### ID Generation Algorithm

```
Input: Document metadata + current timestamp
Process:
  1. Concatenate: "libHaru" + current_timestamp + metadata
  2. Calculate MD5 hash → 16 bytes
  3. Create array with two copies of the hash
Output: /ID [<hash> <hash>]
```

#### Example

```
Trailer
  /Size 42
  /Root 1 0 R
  /Info 2 0 R
  /ID [<5F3C2A1B9E4D7C8A6F1E3B2D4C5A6789> <5F3C2A1B9E4D7C8A6F1E3B2D4C5A6789>]
```

#### Implementation

```csharp
namespace Haru.Doc
{
    public partial class HpdfDocument
    {
        /// <summary>
        /// Generates a unique document ID for PDF/A compliance.
        /// </summary>
        private void GenerateDocumentId()
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                var data = System.Text.Encoding.UTF8.GetBytes(
                    "libHaru" +
                    DateTime.Now.ToString("o") +
                    Info.Title +
                    Info.Author +
                    Info.Producer
                );

                byte[] hash = md5.ComputeHash(data);

                var idArray = new HpdfArray();
                idArray.Add(new HpdfBinary(hash));
                idArray.Add(new HpdfBinary(hash));

                _xref.Trailer["ID"] = idArray;
            }
        }
    }
}
```

---

### 4. Font Embedding (CRITICAL - Required)

**Status:** ⚠️ Partial infrastructure exists
**Effort:** ~3-4 days
**Priority:** HIGH (but can be deferred initially)

#### Why Font Embedding is Required

PDF/A mandates that **all fonts must be embedded** to ensure the document looks identical on any system, regardless of which fonts are installed.

#### Current Status

We have:
- ✅ TrueType table structures (`TrueTypeStructures.cs`)
- ✅ TrueType parser (`TrueTypeParser.cs`)
- ✅ Basic TrueType font class (`HpdfTrueTypeFont.cs`)
- ✅ Character-to-glyph mapping (cmap format 4)
- ✅ Font metrics (widths, ascent, descent, bbox)

We still need:
- ❌ Font subsetting (include only used glyphs)
- ❌ Font program embedding (TrueType binary data)
- ❌ ToUnicode CMap generation (for text extraction)
- ❌ Font descriptor with embedded stream

#### Font Subsetting Algorithm

From C source (`c-src/hpdf_fontdef_tt.c`):

1. **Track used glyphs** - Mark which glyphs are actually used in the document
2. **Recreate glyf table** - Copy only used glyph data
3. **Rebuild loca table** - Update glyph location offsets
4. **Modify name table** - Add unique suffix to font name
5. **Recalculate checksums** - Update all table checksums
6. **Write required tables** - Output 13 required tables:
   - OS/2, cmap, cvt, fpgm, glyf, head, hhea, hmtx, loca, maxp, name, post, prep

#### ToUnicode CMap

A ToUnicode CMap is required for:
- Text extraction
- Searching
- Accessibility (screen readers)

Format:
```
/CIDInit /ProcSet findresource begin
12 dict begin
begincmap
/CIDSystemInfo
<< /Registry (Adobe)
   /Ordering (UCS)
   /Supplement 0
>> def
/CMapName /Adobe-Identity-UCS def
/CMapType 2 def
1 begincodespacerange
<0000> <FFFF>
endcodespacerange
100 beginbfchar
<0003> <0020>  % Space
<0004> <0041>  % A
<0005> <0042>  % B
...
endbfchar
endcmap
```

#### Workaround: Reference Fonts (Non-compliant but functional)

For initial testing without full embedding:
- Use non-embedded TrueType references
- Document will work but won't be PDF/A compliant
- Useful for development and testing

#### Implementation Priority

**Phase 2** (after basic PDF/A infrastructure):
1. Font subsetting algorithm (2 days)
2. ToUnicode CMap generation (1 day)
3. Font program embedding (0.5 day)
4. Testing with various fonts (0.5 day)

---

### 5. PDF/A Validation and Mode Control

**Status:** ❌ Not yet implemented
**Effort:** ~0.5 day
**Priority:** MEDIUM

#### PDF/A Mode Flag

Add a flag to `HpdfDocument` to enable PDF/A mode:

```csharp
public class HpdfDocument
{
    private HpdfPdfAType? _pdfaMode;

    public HpdfPdfAType? PdfAMode
    {
        get => _pdfaMode;
        set
        {
            _pdfaMode = value;
            if (value.HasValue)
            {
                _version = HpdfVersion.Version14; // PDF/A-1 requires PDF 1.4
            }
        }
    }
}
```

#### Prohibited Features Validation

When PDF/A mode is enabled, validate that:

❌ **Encryption** is not used
- We haven't implemented encryption yet, so this is automatically compliant

❌ **JavaScript** actions are not added
- Need to check for JS actions in annotations and catalog

❌ **Audio/Video** content is not embedded
- We haven't implemented this, so compliant by default

❌ **Transparency** is not used (PDF/A-1 only)
- Check ExtGState for transparency settings
- Validate alpha channels in images

❌ **External references** (except annotation links)
- Check for external file references
- Allow only URI links and GoTo destinations

❌ **Non-embedded fonts**
- Validate all fonts have embedded streams

#### Validation Method

```csharp
public class HpdfDocument
{
    private void ValidatePdfACompliance()
    {
        if (!_pdfaMode.HasValue) return;

        // Check for prohibited features
        if (HasEncryption())
            throw new HpdfException(HpdfErrorCode.PdfAEncryptionNotAllowed,
                "Encryption is not allowed in PDF/A");

        if (HasTransparency() && _pdfaMode == HpdfPdfAType.PdfA1B)
            throw new HpdfException(HpdfErrorCode.PdfATransparencyNotAllowed,
                "Transparency is not allowed in PDF/A-1");

        if (HasNonEmbeddedFonts())
            throw new HpdfException(HpdfErrorCode.PdfAFontsNotEmbedded,
                "All fonts must be embedded in PDF/A");
    }
}
```

Call `ValidatePdfACompliance()` during `Save()`.

---

### 6. PDF Version Enforcement

**Status:** ✅ Already supported
**Effort:** Minimal
**Priority:** LOW

#### Requirements

- PDF/A-1 is based on **PDF 1.4**
- Must set version header to `%PDF-1.4`
- Already implemented in `HpdfDocument.Version` property

#### Implementation

```csharp
public void SetPdfAConformance(HpdfPdfAType pdfaType)
{
    PdfAMode = pdfaType;
    Version = HpdfVersion.Version14; // Automatically set to 1.4
}
```

---

## Implementation Roadmap

### Phase 1: Core PDF/A-1b Infrastructure (2-3 days)

**Goal:** Basic PDF/A-1b support without font embedding

#### Day 1: XMP Metadata
- [ ] Create `HpdfXmpMetadata` class
- [ ] Implement XMP/RDF XML generation
- [ ] Add PDF date → ISO 8601 converter
- [ ] Integrate with `HpdfDocument.Save()`
- [ ] Unit tests for XMP generation

#### Day 2: Output Intent + Document ID
- [ ] Embed sRGB ICC profile as resource
- [ ] Create `HpdfOutputIntent` class
- [ ] Implement document ID generation with MD5
- [ ] Add to Catalog and Trailer
- [ ] Integration tests

#### Day 3: Validation + Testing
- [ ] Add PDF/A mode flag to `HpdfDocument`
- [ ] Implement compliance validation
- [ ] Create `SetPdfAConformance()` method
- [ ] Test with PDF/A validators (e.g., VeraPDF)
- [ ] Document limitations (no font embedding yet)

**Deliverable:** PDF/A-1b compliant PDFs (with non-embedded fonts as limitation)

---

### Phase 2: Font Embedding (3-4 days)

**Goal:** Full PDF/A-1b compliance with embedded fonts

#### Day 1-2: Font Subsetting
- [ ] Implement glyph usage tracking
- [ ] Create font subsetting algorithm
- [ ] Rebuild glyf and loca tables
- [ ] Update font checksums
- [ ] Test with sample fonts

#### Day 3: ToUnicode CMap
- [ ] Implement CMap generation
- [ ] Add to font dictionary
- [ ] Test text extraction

#### Day 4: Integration + Testing
- [ ] Embed font program in PDF
- [ ] Update font descriptor
- [ ] End-to-end testing
- [ ] Validation with VeraPDF

**Deliverable:** Fully compliant PDF/A-1b documents

---

### Phase 3: PDF/A-1a Support (Optional - 2-3 days)

**Goal:** Accessibility compliance (Level A)

- [ ] Implement structure tree (tagged PDF)
- [ ] Add role maps
- [ ] Mark content with tags
- [ ] Test with accessibility validators

---

## Testing Strategy

### Validation Tools

1. **VeraPDF** (https://verapdf.org/)
   - Open-source PDF/A validator
   - Industry standard
   - Detailed compliance reports

2. **Adobe Acrobat Pro**
   - Preflight tool for PDF/A validation
   - Commercial but widely available

3. **PDFTron** (https://www.pdftron.com/pdf-tools/pdfa-validator/)
   - Online validator
   - Free for testing

### Test Cases

Create test PDFs with:
- [ ] Minimal content (text only)
- [ ] Multiple fonts
- [ ] Images (PNG, JPEG)
- [ ] Links and annotations
- [ ] Bookmarks/outlines
- [ ] Complete metadata
- [ ] Multiple pages

Validate each with VeraPDF and fix any issues.

---

## Error Codes

Add new error codes for PDF/A:

```csharp
public enum HpdfErrorCode
{
    // ... existing codes ...

    // PDF/A errors
    PdfAEncryptionNotAllowed = 0x1100,
    PdfATransparencyNotAllowed = 0x1101,
    PdfAFontsNotEmbedded = 0x1102,
    PdfAJavascriptNotAllowed = 0x1103,
    PdfAExternalContentNotAllowed = 0x1104,
    PdfAInvalidXmpMetadata = 0x1105,
    PdfAMissingOutputIntent = 0x1106,
    PdfAMissingDocumentId = 0x1107,
}
```

---

## API Usage Example

```csharp
using Haru.Doc;
using Haru.PdfA;

// Create document
var doc = new HpdfDocument();

// Set PDF/A-1b conformance (enables all validations)
doc.SetPdfAConformance(HpdfPdfAType.PdfA1B);

// Set metadata (required for PDF/A)
doc.Info.Title = "My PDF/A Document";
doc.Info.Author = "John Doe";
doc.Info.Subject = "Testing PDF/A compliance";
doc.Info.Keywords = "PDF/A, archival, test";
doc.Info.Creator = "My Application v1.0";
doc.Info.SetCreationDate(DateTime.Now);

// Add content
var page = doc.AddPage();
var font = doc.GetFont("Helvetica", null); // Use standard font
page.BeginText();
page.SetFontAndSize(font, 12);
page.TextOut(100, 700, "Hello PDF/A World!");
page.EndText();

// Save - automatically generates XMP metadata, output intent, and document ID
doc.SaveToFile("output.pdf");

// The PDF is now PDF/A-1b compliant (validated with VeraPDF)
```

---

## Resources

### Specifications
- ISO 19005-1:2005 (PDF/A-1)
- PDF Reference 1.4 (Adobe)

### Tools
- VeraPDF: https://verapdf.org/
- sRGB ICC Profile: https://www.color.org/srgbprofiles.xalter

### C Source Reference
- `c-src/hpdf_pdfa.c` - XMP metadata generation
- `c-src/hpdf_pdfa.h` - PDF/A API
- `c-src/hpdf_fontdef_tt.c` - TrueType font subsetting

### Online Resources
- PDF/A Competence Center: https://www.pdfa.org/
- Adobe XMP Specification: https://www.adobe.com/devnet/xmp.html

---

## Summary Checklist

### Minimum PDF/A-1b Requirements

- [ ] **XMP Metadata** - Embedded in Catalog as Metadata stream
- [ ] **Output Intent** - sRGB ICC profile embedded
- [ ] **Document ID** - MD5-based unique identifier in trailer
- [ ] **PDF Version** - Set to 1.4
- [ ] **Font Embedding** - All fonts embedded (or use Standard 14 initially)
- [ ] **Validation** - No encryption, JavaScript, transparency, etc.

### Nice-to-Have

- [ ] **Font Subsetting** - Optimize file size by including only used glyphs
- [ ] **ToUnicode CMap** - Enable text extraction and search
- [ ] **PDF/A-1a Support** - Add structure tags for accessibility

---

## Estimated Timeline

| Phase | Description | Effort | Dependencies |
|-------|-------------|--------|--------------|
| Phase 1 | Core infrastructure | 2-3 days | HpdfInfo (✅ complete) |
| Phase 2 | Font embedding | 3-4 days | Phase 1 |
| Phase 3 | PDF/A-1a (optional) | 2-3 days | Phase 2 |

**Total: 7-10 days for full PDF/A-1b compliance with embedded fonts**
**Quick start: 2-3 days for basic PDF/A-1b (without embedded fonts)**
