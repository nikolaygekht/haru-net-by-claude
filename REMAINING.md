# Remaining Features to Implement

**Last Updated**: 2025-01-15
**Overall Completion**: ~97%
**Estimated Remaining Effort**: 0.5-1 day

## Priority Features Still To Do

### 1. AcroForms (PDF Forms) - HIGH PRIORITY
**Estimated Effort**: 0.5-1 day (compatibility fixes)
**Complexity**: High
**Status**: 90% complete - Core implemented, viewer compatibility issues remain

PDF forms with interactive fields for data entry.

**Completed Components**:
- ✓ Form field base class with all field types
- ✓ Text fields (single-line, multiline, password)
- ✓ Checkboxes
- ✓ Radio button groups
- ✓ Choice fields (combo boxes, list boxes with multi-select)
- ✓ Signature fields (unsigned placeholders)
- ✓ Widget annotations with appearance streams
- ✓ Field flags and attributes (required, read-only)
- ✓ Default values and formatting
- ✓ Field hierarchy (parent-child relationships via Kids array)
- ✓ Calculation order support
- ✓ Default appearance strings
- ✓ NeedAppearances flag for viewer rendering

**Known Issues** (requires investigation):
- ⚠️ **Checkboxes/Radio Buttons**: Work in Chrome, not in Edge, partial in Adobe Acrobat (updates only on blur)
- ⚠️ **Signature Fields**: Work in Adobe Acrobat, not in Chrome/Edge
- Need to compare with PDF specification examples and working PDFs

**Files Created**:
- `cs-src/Haru/Forms/HpdfAcroForm.cs` - Main form container
- `cs-src/Haru/Forms/HpdfField.cs` - Base field class
- `cs-src/Haru/Forms/HpdfTextField.cs` - Text input fields
- `cs-src/Haru/Forms/HpdfCheckbox.cs` - Checkbox fields
- `cs-src/Haru/Forms/HpdfRadioButton.cs` - Radio button groups
- `cs-src/Haru/Forms/HpdfChoice.cs` - Dropdown/listbox fields
- `cs-src/Haru/Forms/HpdfSignature.cs` - Signature field placeholders
- `cs-src/Haru/Forms/HpdfWidgetAnnotation.cs` - Widget annotations

**Next Steps**:
1. Read PDF 1.4 specification sections on AcroForms
2. Find example PDFs with working forms
3. Compare implementation with spec and examples
4. Fix viewer compatibility issues

---

## Completed Features

### ✓ CIDFontType0 (Predefined CJK Fonts) - COMPLETED
**Status**: Fully implemented and tested
**Completion Date**: 2025-01-15

Predefined CJK fonts that reference system fonts without embedding.

**Implemented Components**:
- ✓ 11 predefined fonts with JSON-based metrics
  - Chinese Simplified: SimSun, SimHei (GBK-EUC-H, CP936)
  - Chinese Traditional: MingLiU (ETen-B5-H, CP950)
  - Japanese: MS-Gothic, MS-Mincho, MS-PGothic, MS-PMincho (90ms-RKSJ-H/EUC-H, CP932/CP20932)
  - Korean: DotumChe, BatangChe, Dotum, Batang (KSCms-UHC-H, CP949)
- ✓ 5 CID encoders inheriting from CIDEncoder base class
- ✓ Fixed-width fonts (7 fonts) and proportional fonts (4 fonts)
- ✓ Binary search for width lookup (O(log n))
- ✓ Adobe CMap references (built into PDF viewers)
- ✓ Lazy loading with caching

**Files Created**:
- `cs-src/Haru/Font/CID/HpdfCIDFontType0.cs` - Main implementation
- `cs-src/Haru/Font/CID/CIDEncoder.cs` - Base encoder class
- `cs-src/Haru/Font/CID/GBKEucHEncoder.cs` - Chinese Simplified (CP936)
- `cs-src/Haru/Font/CID/ETenB5HEncoder.cs` - Chinese Traditional (CP950)
- `cs-src/Haru/Font/CID/Ms90RKSJHEncoder.cs` - Japanese Shift-JIS (CP932)
- `cs-src/Haru/Font/CID/EucHEncoder.cs` - Japanese EUC (CP20932)
- `cs-src/Haru/Font/CID/KSCmsUHCHEncoder.cs` - Korean (CP949)
- `cs-src/Haru/Font/CID/Data/*.json` - 11 font definition files (embedded resources)
- `cs-src/Haru.Test/Font/HpdfCIDFontType0Tests.cs` - 31 comprehensive tests

**Demo**: CJKFontsDemo (shows all 11 fonts on 2 pages)

---

### ✓ Page Labels (COMPLETED)
**Status**: Implemented and tested
**Completion Date**: 2025-10-12

Custom page numbering styles with prefixes and number ranges.

**Implemented Components**:
- ✓ Page numbering styles (Decimal, Roman upper/lower, Letters)
- ✓ Page label prefixes (e.g., "A-1", "Appendix ")
- ✓ PageLabels dictionary in catalog
- ✓ Multiple number ranges per document

**Files Created**:
- `cs-src/Haru/Doc/HpdfPageLabel.cs`
- `cs-src/Haru.Demos/PageLabelAndBoundaryDemo.cs`

**Demo**: PageLabelAndBoundaryDemo

---

### ✓ Page Boundaries (COMPLETED)
**Status**: Implemented and tested
**Completion Date**: 2025-10-12

Advanced page boundary boxes for print production.

**Implemented Components**:
- ✓ MediaBox (full page size - already existed)
- ✓ CropBox (visible region when displayed)
- ✓ BleedBox (clipping for print production)
- ✓ TrimBox (final page dimensions after trimming)
- ✓ ArtBox (meaningful content area)

**Files Updated**:
- `cs-src/Haru/Doc/HpdfPage.cs` (added boundary box methods)

**Demo**: PageLabelAndBoundaryDemo

**Note**: User units, thumbnail images, and per-page metadata streams remain unimplemented (very low priority).

---

## Optional/Lower Priority Features

### 2. (Renumbered) Additional Page Features
**Estimated Effort**: 0.5 day
**Complexity**: Low
**Priority**: Very Low

Remaining advanced page features.

**Required Components**:
- [ ] User units (custom unit scaling)
- [ ] Thumbnail images for pages
- [ ] Per-page metadata streams

**Note**: Main page boundaries already implemented (✓)

---

### 4. Additional Image Formats
**Estimated Effort**: 0.5-1 day
**Complexity**: Medium
**Priority**: Low

Support for specialized image formats.

**Completed Components**:
- ✓ Raw image data loading (Grayscale, RGB, CMYK) - **DONE (2025-01-14)**

**Required Components**:
- [ ] CCITT Group 3/4 fax images (1-bit monochrome compression)
- [ ] 1-bit monochrome images
- [ ] Color mask support (transparency via color key)
- [ ] Image interpolation flag

**Files Updated**:
- ✓ `cs-src/Haru/Doc/HpdfImage.cs` (LoadRawImageFromMem, LoadPngImageFromMem added)
- ✓ `cs-src/Haru/Doc/HpdfPageExtensions.cs` (SetHeight, SetWidth added)

**Files to Create**:
- `cs-src/Haru/Images/` (CCITT image type classes)

**C Source References**:
- `c-src/hpdf_image_ccitt.c` (400+ lines)

---

### 5. Name Dictionary
**Estimated Effort**: 0.5 day
**Complexity**: Low
**Priority**: Low

Named resources and destinations.

**Required Components**:
- [ ] Named destinations (for cross-references)
- [ ] Embedded files registry
- [ ] JavaScript name tree
- [ ] Named pages

**Files to Create**:
- `cs-src/Haru/Doc/HpdfNameDict.cs`

**C Source References**:
- `c-src/hpdf_namedict.c` (200+ lines)

---

### 6. External Data Support
**Estimated Effort**: 0.5 day
**Complexity**: Low
**Priority**: Very Low

External file attachments and streams.

**Required Components**:
- [ ] External stream data
- [ ] File specification objects
- [ ] Embedded file streams
- [ ] External file references

**Files to Create**:
- `cs-src/Haru/Doc/HpdfExData.cs`
- `cs-src/Haru/Doc/HpdfFileSpec.cs`

**C Source References**:
- `c-src/hpdf_exdata.c` (200+ lines)

---

### 7. 3D Annotations (U3D)
**Estimated Effort**: 3-4 days
**Complexity**: High
**Priority**: Very Low

3D model embedding (rarely used feature).

**Required Components**:
- [ ] U3D file format support
- [ ] 3D annotation objects
- [ ] 3D views and projections
- [ ] 3D measurement annotations
- [ ] 3D activation and deactivation
- [ ] 3D cross sections

**Files to Create**:
- `cs-src/Haru/Doc/Hpdf3DAnnotation.cs`
- `cs-src/Haru/Doc/HpdfU3D.cs`
- `cs-src/Haru/Doc/Hpdf3DMeasure.cs`
- `cs-src/Haru/Doc/Hpdf3DView.cs`

**C Source References**:
- `c-src/hpdf_u3d.c` (300+ lines)
- `c-src/hpdf_3dmeasure.c` (200+ lines)

**Note**: This is a very specialized feature rarely needed in practice.

---

### 8. PDF/A Phase 2 (Extended Compliance)
**Estimated Effort**: 2-3 days
**Complexity**: Medium
**Priority**: Low

Enhanced PDF/A compliance (Phase 1 already complete).

**Required Components**:
- [ ] Font embedding enforcement (ensure all fonts embedded)
- [ ] Stricter validation rules
- [ ] Extended metadata requirements
- [ ] Color space restrictions
- [ ] Transparency flattening (if needed)

**Note**: Builds on existing PDF/A-1b implementation. Phase 1 (basic compliance with XMP metadata, output intent, document ID) is complete.

---

## Summary

### Recently Completed
- ✓ **CIDFontType0** (2 days) - All predefined CJK fonts (11 fonts, 5 encoders) - DONE (2025-01-15)
- ✓ **Page Labels** (0.5 day) - Custom page numbering - DONE (2025-10-12)
- ✓ **Page Boundaries** (0.5 day) - Advanced page boxes - DONE (2025-10-12)
- ✓ **AcroForms Core** (3 days) - All field types implemented - DONE (2025-10-13)
- ✓ **Image Loading from Memory** (0.25 day) - PNG and Raw image loading - DONE (2025-01-14)
- ✓ **Page Size Convenience Methods** (0.1 day) - SetHeight/SetWidth - DONE (2025-01-14)
- ✓ **PdfPig Integration Tests** (0.25 day) - Third-party PDF validation - DONE (2025-01-14)
- ✓ **Empty Page Bug Fix** (0.1 day) - PDF spec compliance for stream objects - DONE (2025-01-14)

### Immediate Priority
1. **AcroForms Compatibility** (0.5-1 day) - Fix viewer compatibility issues

### Optional Enhancements
2. **Additional Page Features** (0.5 day) - User units, thumbnails, per-page metadata
3. **Additional Images** (0.5-1 day) - CCITT fax, 1-bit monochrome, color masks
4. **Name Dictionary** (0.5 day) - Named resources
5. **External Data** (0.5 day) - File attachments
6. **PDF/A Phase 2** (2-3 days) - Extended compliance
7. **3D Support** (3-4 days) - Very specialized, rarely needed

### Total Remaining Effort
- **High Priority (AcroForms Compatibility)**: 0.5-1 day
- **All Optional Features**: 0.5-3 days
- **Complete to 100%**: 1-4 days total

## What Works Now

The library is already **production-ready** for:
- ✓ International documents (CJK languages: Chinese, Japanese, Korean)
  - CIDFontType2 (embedded TrueType fonts)
  - CIDFontType0 (predefined fonts: 11 fonts, 7 fixed-width + 4 proportional)
- ✓ Multi-language content (Western, Cyrillic, Greek, Turkish)
- ✓ Secure PDFs (password protection, permissions)
- ✓ Interactive documents (links, bookmarks, annotations)
- ✓ **Interactive forms (AcroForms)** - Text fields, checkboxes, radio buttons, dropdowns, signatures
- ✓ Archival documents (PDF/A-1b compliance)
- ✓ Custom typography (TrueType, Type 1, CID fonts)
- ✓ Rich graphics (shapes, images, transparency)
- ✓ Professional reports and certificates

**AcroForms compatibility** with Edge and full signature support are the main issues to resolve. All other remaining features are specialized or optional.
