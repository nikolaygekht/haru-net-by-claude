# Remaining Features to Implement

**Last Updated**: 2025-10-12
**Overall Completion**: ~92%
**Estimated Remaining Effort**: 3-6 days

## Priority Features Still To Do

### 1. AcroForms (PDF Forms) - HIGH PRIORITY
**Estimated Effort**: 3-5 days
**Complexity**: High
**Status**: Not started (0%)

PDF forms with interactive fields for data entry.

**Required Components**:
- [ ] Form field base class (text fields, checkboxes, radio buttons, buttons, choice fields)
- [ ] Field appearance generation
- [ ] Field validation and formatting
- [ ] Form submission actions
- [ ] Interactive JavaScript support
- [ ] Field hierarchy (parent-child relationships)
- [ ] Field flags and attributes
- [ ] Default values and formatting

**Files to Create**:
- `cs-src/Haru/Doc/Forms/HpdfAcroForm.cs` - Main form container
- `cs-src/Haru/Doc/Forms/HpdfFormField.cs` - Base field class
- `cs-src/Haru/Doc/Forms/HpdfTextField.cs` - Text input fields
- `cs-src/Haru/Doc/Forms/HpdfCheckBox.cs` - Checkbox fields
- `cs-src/Haru/Doc/Forms/HpdfRadioButton.cs` - Radio button groups
- `cs-src/Haru/Doc/Forms/HpdfPushButton.cs` - Button fields
- `cs-src/Haru/Doc/Forms/HpdfChoiceField.cs` - Dropdown/listbox fields

**C Source References**:
- `c-src/hpdf_annotation.c` (contains widget annotations for forms)
- Form-related structures in PDF specification

**Key Features**:
- Widget annotations for field placement
- Appearance streams for visual rendering
- Form actions and triggers
- Field calculation and validation
- Tab order and field navigation

---

## Completed Features

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
**Estimated Effort**: 1-2 days
**Complexity**: Medium
**Priority**: Low

Support for specialized image formats.

**Required Components**:
- [ ] CCITT Group 3/4 fax images (1-bit monochrome compression)
- [ ] Raw image data loading
- [ ] 1-bit monochrome images
- [ ] Color mask support (transparency via color key)
- [ ] Image interpolation flag

**Files to Update**:
- `cs-src/Haru/Doc/HpdfImage.cs` (add methods)
- `cs-src/Haru/Images/` (new image type classes)

**C Source References**:
- `c-src/hpdf_image_ccitt.c` (400+ lines)
- `c-src/hpdf_image.c` (1500+ lines)

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
- ✓ **Page Labels** (0.5 day) - Custom page numbering - DONE
- ✓ **Page Boundaries** (0.5 day) - Advanced page boxes - DONE

### Immediate Priority
1. **AcroForms** (3-5 days) - Interactive PDF forms with fields

### Optional Enhancements
2. **Additional Page Features** (0.5 day) - User units, thumbnails, per-page metadata
3. **Additional Images** (1-2 days) - CCITT fax, raw images
4. **Name Dictionary** (0.5 day) - Named resources
5. **External Data** (0.5 day) - File attachments
6. **PDF/A Phase 2** (2-3 days) - Extended compliance
7. **3D Support** (3-4 days) - Very specialized, rarely needed

### Total Remaining Effort
- **High Priority (AcroForms)**: 3-5 days
- **All Optional Features**: 0.5-3.5 days
- **Complete to 100%**: 3.5-8.5 days total

## What Works Now

The library is already **production-ready** for:
- ✓ International documents (CJK languages: Chinese, Japanese, Korean)
- ✓ Multi-language content (Western, Cyrillic, Greek, Turkish)
- ✓ Secure PDFs (password protection, permissions)
- ✓ Interactive documents (links, bookmarks, annotations)
- ✓ Archival documents (PDF/A-1b compliance)
- ✓ Custom typography (TrueType, Type 1, CID fonts)
- ✓ Rich graphics (shapes, images, transparency)
- ✓ Professional reports and certificates

**AcroForms is the main missing feature** for truly interactive data-entry forms. All other remaining features are specialized or optional.
