# Haru.NET Documentation Plan

Based on the documentation standards in [.claude/DOCUMENTATION.md](.claude/DOCUMENTATION.md) and comprehensive analysis of the codebase, here's the complete plan for creating user-focused "how to use" API documentation.

---

## Documentation Structure Overview

The documentation will be organized in **./cs-src/doc/** with the following hierarchy:

```
cs-src/doc/
├── INDEX.md                          # Main entry point
├── LICENSE.md                        # License information
├── USAGE.md                          # Quick start guide
├── STRUCTURE.md                      # Library architecture with UML
├── guides/                           # User guides
│   ├── GettingStarted.md
│   ├── FontsGuide.md
│   ├── GraphicsGuide.md
│   ├── TextGuide.md
│   ├── ImagesGuide.md
│   ├── FormsGuide.md
│   └── EncryptionGuide.md
├── api/                              # API reference for all classes
│   ├── core/
│   │   ├── HpdfDocument.md
│   │   ├── HpdfPage.md
│   │   ├── HpdfFont.md
│   │   └── HpdfImage.md
│   ├── extensions/
│   │   ├── HpdfPageGraphics.md
│   │   ├── HpdfPageShapes.md
│   │   ├── HpdfPageText.md
│   │   ├── HpdfPageExtensions.md
│   │   └── HpdfDocumentExtensions.md
│   ├── types/
│   │   ├── HpdfPageSize.md
│   │   ├── HpdfRgbColor.md
│   │   ├── HpdfRect.md
│   │   └── [enums and types]
│   ├── forms/
│   │   ├── HpdfAcroForm.md
│   │   ├── HpdfTextField.md
│   │   └── [form field classes]
│   └── annotations/
│       └── [annotation classes]
└── examples/                         # Code examples
    ├── BasicDocument.md
    ├── WorkingWithFonts.md
    ├── DrawingGraphics.md
    └── [example scenarios]
```

---

## Phase 1: Core Documentation (High Priority)

### Mandatory Files (Must Create First)

1. **INDEX.md** - Main entry point with library overview and navigation
2. **LICENSE.md** - License information (need from you)
3. **USAGE.md** - Quick start: installation, first PDF, basic concepts
4. **STRUCTURE.md** - Architecture overview with Mermaid UML diagrams

### Essential User Guides

5. **GettingStarted.md** - Step-by-step tutorial for creating first PDF
6. **FontsGuide.md** - Complete font guide (Standard, TrueType, Type1, CID/CJK)
7. **GraphicsGuide.md** - Drawing paths, shapes, colors, transformations
8. **TextGuide.md** - Text rendering, positioning, formatting
9. **ImagesGuide.md** - Loading and placing PNG/JPEG images

### Core API Documentation

10. **HpdfDocument.md** - Main entry point class
11. **HpdfPage.md** - Page manipulation
12. **HpdfFont.md** - Font wrapper class
13. **HpdfPageGraphics.md** - Graphics extension methods
14. **HpdfPageShapes.md** - Shape drawing extensions
15. **HpdfPageText.md** - Text extension methods

---

## Phase 2: Extended Documentation (Medium Priority)

### Advanced User Guides

16. **FormsGuide.md** - Creating interactive PDF forms (AcroForm)
17. **EncryptionGuide.md** - Document security and permissions
18. **AnnotationsGuide.md** - Adding annotations and links
19. **OutlinesGuide.md** - Creating bookmarks/navigation

### Additional API Documentation

20. **HpdfDocumentExtensions.md**
21. **HpdfPageExtensions.md**
22. **HpdfImage.md**
23. Form field classes (HpdfTextField, HpdfCheckbox, etc.)
24. Type documentation (HpdfPageSize, HpdfRgbColor, enums)

---

## Phase 3: Comprehensive Reference (Lower Priority)

25. All remaining public classes and interfaces
26. Complete enum documentation
27. Advanced examples
28. Migration guide (if migrating from C Haru library)

---

## Information Required From User

To create effective documentation, I need the following information:

### 1. License Information ✅ CRITICAL
MIT license:
  Copyright (c) 1999-2006 Takeshi Kanno <takeshi_kanno@est.hi-ho.ne.jp>
  Copyright (c) 2007-2009 Antony Dovgal <tony@daylessday.org>
  Copyright (c) 2010-2025 Gehtsoft USA LLC <contact@gehtsoftusa.com>

Permission to use, copy, modify, distribute and sell this software
and its documentation for any purpose is hereby granted without fee,
provided that the above copyright notice appear in all copies and
that both that copyright notice and this permission notice appear
in supporting documentation. It is provided "as is" without express or 
implied warranty.


### 2. Project Goals & Audience 🎯 IMPORTANT

The target audence is the .NET software development who need cross-platform, powerful implementation 
of PDF creation software without crazy fees or license limitations. 

The library provides basic primitives to create professionally looking PDF. For more DTP/document-level
formatting the developers can use PDF.Flow library that adds a layout layer to this library. 

### 3. Installation & Distribution 📦 CRITICAL
- Get Haru.version.nupkg
- .NET 8.0+
- No prequistites
- Package dependencies: StbImageSharp (also permissive license), System.Text.Encoding.CodePages

### 4. Version & Stability 📋 IMPORTANT
- 1.0
- It is beta

### 5. Font Resources 🔤 IMPORTANT
For the fonts guide, I need to know:

Font is copyrighted material. The user should consider allowed use of the font before embedding them into PDF. 

Use either reader-supported fonts (like Helvetica) or use free/open source fonts catalogues Google Fonts for https://fonts.google.com/

### 8. Support & Community 💬 HELPFUL
report issues to github https://github.com/nikolaygekht/haru-net-by-claude/issues

How to contribute?
clone repository, make change and then create PR. 

---

## Documentation Strategy

### Focus on "How To Use"

Each document will prioritize:

1. **Purpose** - What this is for and when to use it
2. **Quick Example** - Minimal working code first
3. **Detailed Reference** - Properties/methods with examples
4. **Common Scenarios** - Real-world usage patterns
5. **Best Practices** - Tips from demo code patterns

### Code Examples Philosophy

- All examples will be **self-contained and runnable**
- Use realistic scenarios from the demo projects
- Show both simple and advanced usage
- Include error handling where appropriate
- Follow .NET conventions and best practices

### Navigation & Cross-Linking

- Every page links back to INDEX.md
- Related classes cross-reference each other
- Guides reference relevant API documentation
- Examples link to related guides

---

## Library Architecture Summary

### Main Entry Points

**HpdfDocument** - Main class for creating PDFs
- Document lifecycle management
- Page creation (AddPage methods)
- Font loading (GetFont extension method)
- Image loading (LoadPngImageFromFile, LoadJpegImageFromFile)
- Encryption (SetEncryption)
- Saving (SaveToFile, SaveToMemory)

**HpdfPage** - Individual page manipulation
- Page size management
- Content stream management
- Graphics operations (via extension methods)
- Text operations (via extension methods)
- Annotations and form fields

### Extension Methods Pattern

The library uses extension methods to organize functionality:

- **HpdfPageGraphics** - Low-level graphics (paths, colors, transformations)
- **HpdfPageShapes** - High-level shapes (circles, ellipses, arcs)
- **HpdfPageText** - Text rendering and positioning
- **HpdfPageExtensions** - Convenience methods
- **HpdfDocumentExtensions** - Document-level operations

### Font System Architecture

Polymorphic design with IHpdfFontImplementation interface:

1. **HpdfStandardFontImpl** - Base14 fonts (Times, Helvetica, Courier)
2. **HpdfTrueTypeFont** - TrueType fonts (.ttf) with code page support
3. **HpdfType1Font** - PostScript Type1 fonts with AFM/PFB parsing
4. **HpdfCIDFont** - CJK fonts using Type 0 composite fonts (CIDFontType2)
5. **HpdfCIDFontType0** - CJK predefined fonts (no embedding)

All wrapped in **HpdfFont** for unified interface.

### Key Type Categories

**Geometric Types:**
- HpdfPoint - 2D coordinates
- HpdfRect / HpdfBox - Rectangles
- HpdfTransMatrix - 2D transformations

**Color Types:**
- HpdfRgbColor - RGB color space
- HpdfCmykColor - CMYK color space

**Page Types:**
- HpdfPageSize - Standard page sizes (A4, Letter, etc.)
- HpdfPageDirection - Portrait/Landscape
- HpdfPageLayout - Document layout modes
- HpdfPageMode - Initial display mode

**Graphics Enums:**
- HpdfLineCap - Line cap styles
- HpdfLineJoin - Line join styles
- HpdfTextRenderingMode - Text rendering modes
- HpdfBlendMode - Blend modes

**Document Enums:**
- HpdfVersion - PDF version (1.2-1.7)
- HpdfCompressionMode - Compression flags
- HpdfEncryptMode - Encryption modes (R2, R3, R4)
- HpdfPermission - Document permissions

---

## Implementation Approach Options

### Option A: Start with Core Documentation (Recommended)
1. User provides critical information
2. Create Phase 1 files (INDEX, LICENSE, USAGE, STRUCTURE, core guides)
3. Review and iterate
4. Continue to Phase 2 and 3

### Option B: Start with Specific Area
If urgent need, prioritize:
- Font documentation (most complex area)
- Getting started guide (for users)
- API reference for specific classes

### Option C: Generate Draft Structure First
Create skeleton files with TODO markers for needed information, then fill in content as details are provided.

---

## Estimated Deliverables

This plan will create **comprehensive, user-focused documentation** covering:

- ✅ 25+ documentation files
- ✅ Complete API reference for all public classes
- ✅ 9+ user guides for different scenarios
- ✅ Numerous runnable code examples
- ✅ UML diagrams for architecture
- ✅ Full cross-linking and navigation

**The documentation will follow standards exactly**: English, Markdown, Mermaid diagrams, consistent structure, and developer-focused "how to use" approach.

---

## Next Steps

1. Provide the required information listed above (especially license and installation)
2. Choose implementation approach (A, B, or C)
3. Specify any priority areas or special requirements
4. Begin documentation generation

---

*Plan created: 2025-10-30*
*Based on: .claude/DOCUMENTATION.md standards and comprehensive codebase analysis*
