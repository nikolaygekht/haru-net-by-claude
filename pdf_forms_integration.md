# Implementing PDF Forms (AcroForms) in an Existing PDF Writer (Haru Integration)

This refactored guide expands the technical detail of AcroForm implementation and maps it directly to the **Haru PDF library architecture**. It adds bit-level flag enumerations, syntax examples, and integration notes for each affected subsystem.

---

## 1. Scope and Overview

- **Target:** Full PDF 1.4-compatible AcroForm implementation.
- **Excludes:** XFA (XML-based forms).
- **Field Types Supported:**
  - `Btn` – Pushbutton, Checkbox, Radio Button
  - `Tx` – Text Field
  - `Ch` – Choice (List/Combo)
  - `Sig` – Signature Field

---

## 2. Integration with Haru Library

### Existing Infrastructure
```
├── HpdfDocument.cs              → Add AcroForm property
├── HpdfCatalog.cs               → Add /AcroForm reference
├── HpdfPage.cs                  → Link widgets to /Annots array
├── HpdfAnnotation.cs            → Base for HpdfWidgetAnnotation
└── HpdfFont.cs                  → Used for /DA (default appearance)
```

### New Components
```
├── Haru/Forms/
│   ├── HpdfAcroForm.cs          → Document-level form container
│   ├── HpdfField.cs             → Base field class
│   ├── HpdfTextField.cs         → Text input fields
│   ├── HpdfCheckbox.cs          → Checkbox fields
│   ├── HpdfRadioButton.cs       → Radio button fields
│   ├── HpdfChoice.cs            → List/combo boxes
│   ├── HpdfSignature.cs         → Signature fields
│   ├── HpdfWidgetAnnotation.cs  → Widget implementation
│   ├── HpdfFieldFlags.cs        → Flag enumerations
│   ├── HpdfAppearance.cs        → Appearance stream generator
│   └── HpdfFormAction.cs        → SubmitForm/ResetForm actions
```

---

## 3. Object Model

### Structure
```
Document
 └── Catalog (/Root)
      └── /AcroForm
           ├── /Fields [field references]
           ├── /DA (/Helv 12 Tf 0 0 0 rg)
           ├── /DR (default resources)
           ├── /NeedAppearances true/false
           └── /CO [calculation order array]
```

### Field Dictionary
| Key | Description |
|-----|--------------|
| `/FT` | Field type (`Btn`, `Tx`, `Ch`, `Sig`) |
| `/T` | Partial field name |
| `/V` | Value |
| `/DV` | Default value |
| `/Ff` | Bit flags |
| `/Kids` | Child widgets |
| `/Parent` | Parent field |
| `/AA` | Additional actions (optional) |

### Widget Annotation
| Key | Description |
|-----|--------------|
| `/Subtype /Widget` | Marks annotation as form control |
| `/Rect` | Control rectangle |
| `/P` | Page reference |
| `/AP` | Appearance dictionary (`/N`, `/R`, `/D`) |
| `/AS` | Current appearance state |
| `/MK` | Appearance characteristics |
| `/A` | Action (optional) |
| `/F` | Annotation flags |

### Widget Linking Example (C#)
```csharp
page.Annots.Add(widget);
widget.Dict["P"] = page.IndirectReference;
field.Dict["Kids"] = new HpdfArray { widget };
```

---

## 4. Field Types and Mechanisms

### Text Field (Tx)
- `/V` – text string
- `/Q` – quadding: 0 = left, 1 = center, 2 = right
- `/MaxLen` – character limit
- `/DA` – default appearance (see below)
- `/Ff` – flags: `Multiline`, `Password`, `ReadOnly`, etc.

### Choice Field (Ch)
- `/Opt` – array of options
- `/V` – selected value
- `/Ff` – `Combo`, `Edit` for editable combo box

### Checkbox / Radio (Btn)
- `/V` and `/AS` – state names (e.g., `/Yes`, `/Off`)
- `/AP/N` – appearance dictionary with state streams
- **Radio Group Example:**
```pdf
Parent Field:
  /FT /Btn
  /T (Gender)
  /Kids [button1 button2]
  /V /Male

Child Widget 1:
  /Parent parent_ref
  /AP /N << /Male <stream> /Off <stream> >>
  /AS /Male

Child Widget 2:
  /Parent parent_ref
  /AP /N << /Female <stream> /Off <stream> >>
  /AS /Off
```

### Signature Field (Sig)
Contains placeholder dictionary:
```
/FT /Sig
/Type /Annot
/Subtype /Widget
/Rect [...]
/V << /Type /Sig /Filter /Adobe.PPKLite /SubFilter /adbe.pkcs7.detached /ByteRange [0 0 0 0] /Contents <> >>
```

---

## 5. Field Flags Enumeration

```csharp
[Flags]
public enum HpdfFieldFlags
{
    ReadOnly = 1 << 0,
    Required = 1 << 1,
    NoExport = 1 << 2,
    Multiline = 1 << 12,
    Password = 1 << 13,
    Radio = 1 << 15,
    Pushbutton = 1 << 16,
    Combo = 1 << 17,
    Edit = 1 << 18,
    Sort = 1 << 19,
    MultiSelect = 1 << 21
}
```
Source: PDF 1.4 Spec, Tables 8.70–8.77.

---

## 6. Default Appearance (DA) Syntax

Default appearance defines the base text style:
```
/DA (/Helv 12 Tf 0 0 0 rg)
```
| Element | Meaning |
|----------|----------|
| `/Helv` | Font name |
| `12` | Font size |
| `Tf` | Set font |
| `0 0 0 rg` | Set text color (RGB) |

---

## 7. Appearance Stream Examples

### Text Field
```pdf
1 w 0 G
0 0 100 20 re S
0.95 0.95 0.95 rg
1 1 98 18 re f
BT
/Helv 12 Tf
0 0 0 rg
2 4 Td
(Sample Text) Tj
ET
```

### Checkbox (Checked)
```pdf
1 w 0 G
0 0 12 12 re S
BT
/ZaDb 10 Tf
1 1 Td
(4) Tj
ET
```

---

## 8. Appearance State Naming

- `/Yes` and `/Off` are standard.
- Custom names allowed if `/AS` matches key in `/AP/N` dictionary.

---

## 9. Widget-to-Page and Form Linking

Ensure bidirectional linking:
- Page `/Annots` → Widget reference.
- Widget `/P` → Page reference.
- Field `/Kids` → Widget array.
- Document `/AcroForm/Fields` → Field references.

---

## 10. Calculation Order

`/CO` is an array of **field references** that defines computation sequence.
```pdf
/CO [ 12 15 22 ]
```

---

## 11. Widget Border Styles

`/BS` dictionary example:
```
/BS << /W 1 /S /S >>     % Solid
/BS << /W 1 /S /D >>     % Dashed
/BS << /W 1 /S /B >>     % Beveled
```

---

## 12. API Surface

```csharp
HpdfAcroForm form = doc.GetOrCreateAcroForm();
var field = form.AddTextField("Name", new HpdfRect(50, 50, 250, 70));
field.SetValue("John Doe");
field.Flags |= HpdfFieldFlags.Required;
form.SetNeedAppearances(false);
```

---

## 13. Testing Checklist

1. Text field with /DA rendering.
2. Checkbox / Radio with /AP switching.
3. Combo box editability.
4. Signature placeholder.
5. Widget-page linkage.
6. Cross-viewer validation (Acrobat, Foxit, PDF.js).

---

## Appendix A – Field Flags (Bit Positions)
| Flag | Bit | Applies To |
|-------|-----|-------------|
| ReadOnly | 1 | All |
| Required | 2 | All |
| NoExport | 3 | All |
| Multiline | 13 | Tx |
| Password | 14 | Tx |
| Radio | 16 | Btn |
| Pushbutton | 17 | Btn |
| Combo | 18 | Ch |
| Edit | 19 | Ch |
| Sort | 20 | Ch |
| MultiSelect | 22 | Ch |

---

## Appendix B – Appearance Stream Templates
See section 7 for examples. Add `/AP/R` and `/AP/D` for rollover/down states if needed.

---

## Appendix C – Minimal Form Example (Simplified)
```pdf
1 0 obj << /Type /Catalog /AcroForm 2 0 R >> endobj
2 0 obj << /Fields [3 0 R] /DA (/Helv 12 Tf 0 0 0 rg) >> endobj
3 0 obj << /FT /Tx /T (Name) /V (John Doe) /Rect [50 50 200 70] /Subtype /Widget /P 4 0 R >> endobj
4 0 obj << /Type /Page /Annots [3 0 R] >> endobj
```

---

## Appendix D – Widget Annotation Dictionary
| Key | Type | Description |
|-----|------|--------------|
| `/Type` | Name | Always `/Annot` |
| `/Subtype` | Name | `/Widget` |
| `/Rect` | Array | Position |
| `/P` | Ref | Parent page |
| `/AP` | Dict | Appearance streams |
| `/AS` | Name | Current state |
| `/MK` | Dict | Appearance characteristics |
| `/BS` | Dict | Border style |

---

## References
- PDF 1.4 Reference, §8.6–8.7
- [HexaPDF Forms Overview](https://hexapdf.gettalong.org/documentation/forms.html)
- [PDFBox Forms Tutorial](https://pdfbox.apache.org/1.8/cookbook/interactiveform.html)
- [iText PdfAcroForm Docs](https://api.itextpdf.com)
- [Foxit Blog: AcroForms vs XFA](https://www.foxit.com/blog/acroforms-vs-xfa-forms/)

---

