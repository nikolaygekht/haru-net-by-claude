# Haru.Net Code Quality Issues Tracking

**Project**: Haru.Net C# Port
**Location**: `/home/gleb/work/gehtsoft/haru-net-by-claude/cs-src/`
**Status**: Analysis completed, fixes in progress
**Last Updated**: 2025-10-30

---

## Summary

| Category | Total | Fixed | Remaining |
|----------|-------|-------|-----------|
| API Compatibility (null parameters) | 1 | 0 | 1 |
| Disposable Issues (CA1001, CA1816) | 3 | 0 | 3 |
| Null Safety Errors (CS8618, CS8603, etc.) | 352 | 0 | 352 |
| Null Validation (CA1062) | 164 | 0 | 164 |
| Other Warnings | 27 | 0 | 27 |
| **Total** | **547** | **0** | **547** |

### Build Status
- **Errors**: 261 (due to WarningsAsErrors configuration)
- **Warnings**: 27
- **Build Result**: ❌ FAILED (expected until fixes are applied)

---

## Priority 1: API Compatibility Issues (null parameters)

> **Goal**: Ensure Haru C# port accepts null parameters where old P/Invoke version did, eliminating workarounds in consumer code.

### Known Issues

- [ ] **SetDash(null, 0)** - Currently requires Array.Empty<ushort>() workaround
  - **File**: `Haru/Doc/HpdfPageGraphics.cs` (found via grep)
  - **Current signature**: `public static void SetDash(this IDrawable drawable, ushort[] pattern, uint phase)`
  - **Consumer workaround**: `LibharuGraphics.cs:273` in PDF.Flow uses `Array.Empty<ushort>()`
  - **Fix**: Change signature to `SetDash(ushort[]? pattern, uint phase)` and handle null properly
  - **Status**: Not started

### Issues from Analyzer

None - this is an API design issue, not caught by analyzers

---

## Priority 2: Disposable Issues

> **Goal**: Ensure all IDisposable objects are properly disposed, preventing resource leaks.

### Critical Issues (CA2000, CA1001, CA1816, CA2213)

#### CA1001: Types that own disposable fields should be disposable (2 issues)

- [ ] **HpdfStreamObject** owns disposable field `_stream` but is not disposable
  - **File**: `Haru/Objects/HpdfStreamObject.cs:10`
  - **Fix**: Implement IDisposable interface and dispose `_stream` field
  - **Status**: Not started

- [ ] **TrueTypeParser** owns disposable field `_reader` but is not disposable
  - **File**: `Haru/Font/TrueType/TrueTypeParser.cs:26`
  - **Fix**: Implement IDisposable interface and dispose `_reader` field
  - **Status**: Not started

#### CA1816: Dispose methods should call SuppressFinalize (1 issue)

- [ ] **StbImageReader.Dispose()** should call GC.SuppressFinalize(this)
  - **File**: `Haru/Png/StbImageReader.cs:290`
  - **Fix**: Add `GC.SuppressFinalize(this);` at end of Dispose() method
  - **Status**: Not started

### Files to Manually Review

- [ ] `Haru/Doc/HpdfDocument.cs` - Document lifecycle management
- [ ] `Haru/Doc/HpdfImage.cs` - Image resource disposal
- [ ] `Haru/Font/HpdfTrueTypeFont.cs` - TTF file loading and disposal
- [ ] `Haru/Font/HpdfType1Font.cs` - Type1 file loading and disposal
- [ ] `Haru/Font/HpdfFont.cs` - Font object lifecycle
- [ ] All stream usage in font/image loading

---

## Priority 3: Null Safety Issues

> **Goal**: Eliminate null reference exceptions through proper validation and nullable annotations.

### Critical Issues (CS8600-CS8625) - 352 total errors

#### CS8618: Non-nullable field must be initialized (194 errors)
Non-nullable fields not assigned in constructor. Affects:
- Font classes: `HpdfTrueTypeFont`, `HpdfType1Font`, `HpdfCIDFont`
- TrueType structures: `TrueTypeOffsetTable`, `TrueTypeHeadTable`, `TrueTypeCmapFormat4`, etc.
- Other: `HpdfImage`, `HpdfName`, `HpdfBinary`, `HpdfStream`

**Resolution**: Add nullable annotations (`?`) or initialize fields in constructor

#### CS8603: Possible null reference return (58 errors)
Methods returning non-nullable type but may return null. Affects:
- `TrueTypeParser`: Various Get methods
- `HpdfCIDFont`: Property getters

**Resolution**: Change return type to nullable (`T?`) or ensure never-null return

#### CS8625: Cannot convert null literal to non-nullable reference (44 errors)
Attempting to assign null to non-nullable fields/properties.

**Resolution**: Change field/property type to nullable or use non-null default

#### CS8600: Converting null literal/possible null to non-nullable (36 errors)
Assignment of possibly-null value to non-nullable type.

**Resolution**: Add null checks or change target type to nullable

#### CS8604: Possible null reference argument (10 errors)
Passing possibly-null value to parameter expecting non-null.

**Resolution**: Add null checks before passing or change parameter to nullable

#### CS8602: Dereference of possibly null reference (8 errors)
Using possibly-null value without null check.

**Resolution**: Add null checks before dereferencing

#### CS8601: Possible null reference assignment (2 errors)
Assigning possibly-null to non-nullable variable.

**Resolution**: Add null guards or change variable type to nullable

### Parameter Validation Issues (CA1062) - 164 errors

Public methods missing null validation on parameters. Affects:
- `HpdfCompatExtensions.cs`: Most extension methods (29 methods)
- `HpdfPageText.cs`: All text operation methods
- Other extension classes

**Resolution**: Add `ArgumentNullException.ThrowIfNull(param);` or equivalent at method start

### Files to Manually Review

- [ ] All public method parameters in `Haru/Doc/HpdfDocument.cs`
- [ ] All public method parameters in `Haru/Doc/HpdfPage.cs`
- [ ] All public method parameters in `Haru/Doc/HpdfPageExtensions.cs`
- [ ] All public method parameters in `Haru/Doc/HpdfDocumentExtensions.cs`
- [ ] Constructor validation in all classes

---

## Priority 4: Memory Leak Risks

> **Goal**: Identify and fix potential memory leaks from unclosed resources or growing collections.

### Critical Issues (CA1806, CA1821)

> Issues will be added after Phase 2 analysis

### Areas to Review

- [ ] Static collections (font registry, image cache)
- [ ] Event handler subscriptions/unsubscriptions
- [ ] Large buffer allocations (image data, font data)
- [ ] Circular references in object graphs
- [ ] Stream handling in parsers

---

## Priority 5: Other Issues

> **Goal**: Fix performance, style, and best practice issues.

### Performance Issues (CA1822, CA1825, etc.)

> Issues will be added after Phase 2 analysis

### Code Style Issues (IDE rules)

> Issues will be added after Phase 2 analysis

---

## Analysis Output

### Build Command Used
```bash
cd /home/gleb/work/gehtsoft/haru-net-by-claude/cs-src
dotnet build Haru.sln /p:TreatWarningsAsErrors=false > haru_analyzer_output.txt 2>&1
```

### Raw Analyzer Output Summary

**File**: `haru_analyzer_output.txt` (585 lines)
**Build Time**: 6.40 seconds
**Result**: FAILED (261 errors, 27 warnings)

**Error Distribution**:
```
CS8618: 194 errors (Non-nullable field not initialized)
CA1062: 164 errors (Missing parameter validation)
CS8603:  58 errors (Possible null reference return)
CS8625:  44 errors (Cannot convert null literal)
CS8600:  36 errors (Converting null to non-nullable)
CS8604:  10 errors (Possible null reference argument)
CS8602:   8 errors (Dereference of possibly null)
CA1001:   4 errors (Type owns disposable fields)
CS8601:   2 errors (Possible null reference assignment)
CA1816:   2 errors (Dispose should call SuppressFinalize)
```

Note: Duplicate occurrences in output reduce unique count from 522 to 261 errors.

---

## Fix Log

### API Compatibility Fixes

> Fixes will be logged here as they are completed

### Disposable Fixes

> Fixes will be logged here as they are completed

### Null Safety Fixes

> Fixes will be logged here as they are completed

### Memory Leak Fixes

> Fixes will be logged here as they are completed

### Other Fixes

> Fixes will be logged here as they are completed

---

## Notes

- Nullable reference types enabled via Directory.Build.props
- Using balanced analyzer configuration (critical/high priority rules)
- Test files have relaxed analyzer rules (see .editorconfig)
- All fixes should maintain backward compatibility with existing tests

---

## Next Steps

1. ✅ Phase 1: Setup analyzers, Directory.Build.props, .editorconfig
2. ✅ Phase 2: Build with analyzers and analyze output
3. ⏳ Phase 3: Fix issues by category (API compat → Disposables → Null safety → Memory → Other)
   - Start with API compatibility (1 issue)
   - Then disposables (3 issues)
   - Then null safety (516 issues - largest effort)
4. ⏳ Phase 4: Manual review of all critical files
5. ⏳ Phase 5: Validate with full test suite (738 tests)
6. ⏳ Phase 6-9: Repeat for PDF.Flow project
