# Documentation Standards and Structure

## General Requirements

- All documentation must be written in **English** and use **Markdown** format.  
- Use **`mermaid`** syntax for UML diagrams.  
- Use **Markdown syntax highlighting** for code samples and method declarations.  
- All internal links must be **relative** and verified to work.  
- The documentation is intended for **developers** who will use or extend the library.  
  Write clearly and avoid unnecessary implementation jargon.

---

## Documentation Folder Structure

All documentation files must be placed in the project folder:  
```
./cs-src/doc
```

### Mandatory Files

| File | Description |
|------|--------------|
| **INDEX.md** | Main entry point. Includes the name and purpose of the library, and links to all related articles and class documentation. |
| **LICENSE.md** | Contains the license information. |
| **USAGE.md** | Short reference describing how to include and start using the library. |
| **STRUCTURE.md** | Describes the structure of the library and includes UML diagrams. |
| **CLASSNAME.md** | Documentation file for each class, interface, structure, or enum. File names must match their exact source identifiers. |

---

## Markdown Style Rules

- Use fenced code blocks with language tags (e.g., ```csharp).  
- Use consistent heading levels:  
  - `#` for file title  
  - `##` for top sections  
  - `###` for subsections  
- Prefer **tables** for listing parameters, return values, and properties.  
- Place UML diagrams at the top of `STRUCTURE.md` or within each class file if relevant.  
- At the bottom of each file, include a navigation link:  
  ```markdown
  [Back to Index](../INDEX.md)
  ```

---

## UML Diagram Guidelines

Use `mermaid` diagrams to illustrate:
- **Class relationships** (class diagrams)  
- **Component structures**  
- **Sequence diagrams** for complex method flows  

Keep diagrams simple, readable, and focused on relationships or data flow rather than layout aesthetics.

---

## Structure of Class / Interface / Structure / Enum (CISE) Documentation

Each file (`CLASSNAME.md`) must contain the following sections in this order:

### 1. Purpose
A short, plain-English description (1–3 sentences) explaining what the CISE represents and when it should be used.

### 2. Getting an Instance
Explain how to obtain or create an instance of the class, including any initialization or configuration requirements.

### 3. Example
Provide a minimal, self-contained code sample that demonstrates a basic use case.  
Include necessary `using` statements if applicable.

```csharp
var obj = new MyClass(config);
obj.Execute();
```

### 4. Properties
Provide a summary table of all public properties.

| Name | Type | Description |
|------|------|--------------|
| Name | string | The display name of the object |

Each name in this list must link to an anchor in the detailed property descriptions section.

### 5. Methods
Provide a summary table of all public methods.

| Name | Description |
|------|--------------|
| Execute() | Runs the main process |

Each method name must link to its detailed description.

### 6. Detailed Descriptions

Each property and method must have its own section using this format:

#### Property: *PropertyName*
**Declaration:**  
```csharp
public string Name { get; set; }
```

**Description:**  
Short, clear explanation of what this property represents.

**Example:**  
```csharp
Console.WriteLine(obj.Name);
```

---

#### Method: *MethodName()*
**Declaration:**  
```csharp
public void Execute()
```

**Parameters:**

| Name | Type | Description |
|------|------|-------------|
| input | string | User-provided data |

**Return Value:**  
`void`

**Description:**  
What the method does and when to use it.

**Example:**  
```csharp
obj.Execute();
```

---

## Cross-Linking and Navigation

- Use relative links between all documentation files.  
- `INDEX.md` must include a **hierarchical list** of all classes, grouped by module or namespace.  
- Each documentation file must include a `[Back to Index]` link at the end.

---

## Example Consistency Rules

- Examples must compile and demonstrate one concept per snippet.  
- Use realistic but minimal data.  
- Avoid pseudocode unless the real code is unavailable.  
- Prefer small, runnable examples over long listings.

---

## Optional Sections

For larger or public projects, consider including:
- **Version History** – to track changes and API updates.  
- **Known Issues / Limitations** – for transparency.  
- **Related Classes** – links to related or dependent types.

---

## Validation Checklist

Before finalizing any documentation file, verify:

- [ ] File naming matches the source type.  
- [ ] All public properties and methods are documented.  
- [ ] All internal and external links work.  
- [ ] Examples are valid and formatted correctly.  
- [ ] UML diagrams render properly in Markdown.  
- [ ] The file follows heading and code style rules.  

---

## Notes for AI-Generated Documentation

When documentation is generated by AI:
- Extract method signatures and XML comments directly from the source code.  
- If a property, method, or parameter is undocumented, insert a clear `TODO:` marker.  
- Avoid guessing functionality; use neutral placeholders until verified by a developer.  
- Maintain formatting, link structure, and file hierarchy as described above.
