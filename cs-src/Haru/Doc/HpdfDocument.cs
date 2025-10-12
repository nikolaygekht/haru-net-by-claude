/*
 * << Haru Free PDF Library >> -- HpdfDocument.cs
 *
 * C# port of Haru Free PDF Library
 *
 * Copyright (c) 1999-2025 Haru Free PDF Library
 *
 * Permission to use, copy, modify, distribute and sell this software
 * and its documentation for any purpose is hereby granted without fee,
 * provided that the above copyright notice appear in all copies and
 * that both that copyright notice and this permission notice appear
 * in supporting documentation.
 * It is provided "as is" without express or implied warranty.
 *
 */

using System.Collections.Generic;
using System.IO;
using System.Text;
using Haru.Objects;
using Haru.Streams;
using Haru.Types;
using Haru.Xref;

namespace Haru.Doc
{
    /// <summary>
    /// PDF version specification.
    /// </summary>
    public enum HpdfVersion
    {
        /// <summary>
        /// PDF version 1.2
        /// </summary>
        Version12 = 0,

        /// <summary>
        /// PDF version 1.3
        /// </summary>
        Version13,

        /// <summary>
        /// PDF version 1.4
        /// </summary>
        Version14,

        /// <summary>
        /// PDF version 1.5
        /// </summary>
        Version15,

        /// <summary>
        /// PDF version 1.6
        /// </summary>
        Version16,

        /// <summary>
        /// PDF version 1.7
        /// </summary>
        Version17
    }

    /// <summary>
    /// Main entry point for creating PDF documents.
    /// This class manages the document catalog, page tree, and all document-level structures.
    /// </summary>
    public class HpdfDocument
    {
        private static readonly string[] VersionHeaders = new[]
        {
            "%PDF-1.2\n%\u00b7\u00be\u00ad\u00aa\n", // Version 1.2
            "%PDF-1.3\n%\u00b7\u00be\u00ad\u00aa\n", // Version 1.3
            "%PDF-1.4\n%\u00b7\u00be\u00ad\u00aa\n", // Version 1.4
            "%PDF-1.5\n%\u00b7\u00be\u00ad\u00aa\n", // Version 1.5
            "%PDF-1.6\n%\u00b7\u00be\u00ad\u00aa\n", // Version 1.6
            "%PDF-1.7\n%\u00b7\u00be\u00ad\u00aa\n"  // Version 1.7
        };

        private readonly HpdfXref _xref;
        private readonly HpdfCatalog _catalog;
        private readonly HpdfPages _rootPages;
        private readonly HpdfInfo _info;
        private readonly List<HpdfPage> _pageList;
        private HpdfPages _currentPages;
        private HpdfPage _currentPage;
        private HpdfVersion _version;
        private HpdfOutline _outlineRoot;
        private bool _pdfACompliant;
        private string _pdfAConformance;
        private HpdfEncryptDict _encryptDict;
        private readonly Dictionary<int, HpdfPageLabel> _pageLabels;

        /// <summary>
        /// Gets the PDF version for this document.
        /// </summary>
        public HpdfVersion Version
        {
            get => _version;
            set => _version = value;
        }

        /// <summary>
        /// Gets the document catalog.
        /// </summary>
        public HpdfCatalog Catalog => _catalog;

        /// <summary>
        /// Gets the root pages object.
        /// </summary>
        public HpdfPages RootPages => _rootPages;

        /// <summary>
        /// Gets the current pages object (for adding new pages).
        /// </summary>
        public HpdfPages CurrentPages
        {
            get => _currentPages;
            set => _currentPages = value ?? _rootPages;
        }

        /// <summary>
        /// Gets the current page.
        /// </summary>
        public HpdfPage CurrentPage => _currentPage;

        /// <summary>
        /// Gets the list of all pages in the document.
        /// </summary>
        public IReadOnlyList<HpdfPage> Pages => _pageList.AsReadOnly();

        /// <summary>
        /// Gets the document information dictionary.
        /// </summary>
        public HpdfInfo Info => _info;

        /// <summary>
        /// Gets the cross-reference table.
        /// </summary>
        public HpdfXref Xref => _xref;

        /// <summary>
        /// Gets the root outline (bookmarks) for the document.
        /// Creates it if it doesn't exist yet.
        /// </summary>
        public HpdfOutline GetOutlineRoot()
        {
            if (_outlineRoot == null)
            {
                _outlineRoot = new HpdfOutline(_xref);
                _catalog.Dict.Add("Outlines", _outlineRoot.Dict);
            }
            return _outlineRoot;
        }

        /// <summary>
        /// Creates a top-level outline entry.
        /// </summary>
        /// <param name="title">The title of the outline entry.</param>
        /// <returns>The newly created outline.</returns>
        public HpdfOutline CreateOutline(string title)
        {
            var root = GetOutlineRoot();
            return root.CreateChild(title);
        }

        /// <summary>
        /// Enables PDF/A compliance for this document.
        /// This adds XMP metadata, Output Intent, and Document ID.
        /// </summary>
        /// <param name="conformance">PDF/A conformance level (e.g., "1B", "2B", "3B"). Default is "1B".</param>
        public void SetPdfACompliance(string conformance = "1B")
        {
            _pdfACompliant = true;
            _pdfAConformance = conformance ?? "1B";

            // PDF/A-1 requires PDF version 1.4
            if (_pdfAConformance.StartsWith("1"))
                _version = HpdfVersion.Version14;
        }

        /// <summary>
        /// Gets whether PDF/A compliance is enabled.
        /// </summary>
        public bool IsPdfACompliant => _pdfACompliant;

        /// <summary>
        /// Creates a new PDF document.
        /// </summary>
        public HpdfDocument()
        {
            _version = HpdfVersion.Version12; // Default to PDF 1.2
            _xref = new HpdfXref(0);
            _pageList = new List<HpdfPage>();
            _pageLabels = new Dictionary<int, HpdfPageLabel>();

            // Create root pages
            _rootPages = new HpdfPages(_xref);
            _currentPages = _rootPages;

            // Create catalog
            _catalog = new HpdfCatalog(_xref, _rootPages);

            // Create info dictionary
            _info = new HpdfInfo(_xref);

            // Set up trailer
            _xref.Trailer.Add("Root", _catalog.Dict);
            _xref.Trailer.Add("Info", _info.Dict);
        }

        /// <summary>
        /// Adds a new page to the document with default size.
        /// </summary>
        /// <returns>The newly created page.</returns>
        public HpdfPage AddPage()
        {
            var page = new HpdfPage(_xref);
            _currentPages.AddKid(page);
            _pageList.Add(page);
            _currentPage = page;
            return page;
        }

        /// <summary>
        /// Adds a new page with a specific size and orientation.
        /// </summary>
        /// <param name="size">The page size.</param>
        /// <param name="direction">The page orientation.</param>
        /// <returns>The newly created page.</returns>
        public HpdfPage AddPage(HpdfPageSize size, HpdfPageDirection direction)
        {
            var page = AddPage();
            page.SetSize(size, direction);
            return page;
        }

        /// <summary>
        /// Adds a new page with custom dimensions.
        /// </summary>
        /// <param name="width">The page width in points.</param>
        /// <param name="height">The page height in points.</param>
        /// <returns>The newly created page.</returns>
        public HpdfPage AddPage(float width, float height)
        {
            var page = AddPage();
            page.SetSize(width, height);
            return page;
        }

        /// <summary>
        /// Inserts a new intermediate pages node for hierarchical page organization.
        /// </summary>
        /// <param name="parent">The parent pages node, or null to use the current pages.</param>
        /// <returns>The newly created pages node.</returns>
        public HpdfPages InsertPagesNode(HpdfPages parent = null)
        {
            parent = parent ?? _currentPages;
            var pages = new HpdfPages(_xref, parent);
            _currentPages = pages;
            return pages;
        }

        /// <summary>
        /// Sets encryption on the document with user and owner passwords.
        /// </summary>
        /// <param name="userPassword">The user password (opens with restricted permissions).</param>
        /// <param name="ownerPassword">The owner password (opens with full permissions).</param>
        /// <param name="permission">The permission flags for the user password.</param>
        /// <param name="mode">The encryption mode (R2=40-bit RC4, R3=128-bit RC4, R4=128-bit AES).</param>
        public void SetEncryption(string userPassword, string ownerPassword,
            HpdfPermission permission = HpdfPermission.Print,
            HpdfEncryptMode mode = HpdfEncryptMode.R3)
        {
            // Create encryption dictionary if it doesn't exist
            if (_encryptDict == null)
            {
                _encryptDict = new HpdfEncryptDict(_xref);
            }

            // Set encryption parameters
            int keyLength = mode == HpdfEncryptMode.R2 ? 5 : 16; // 40-bit or 128-bit
            _encryptDict.SetEncryptionMode(mode, keyLength);
            _encryptDict.SetUserPassword(userPassword);
            _encryptDict.SetOwnerPassword(ownerPassword);
            _encryptDict.SetPermission(permission);

            // Update PDF version based on encryption mode
            if (mode == HpdfEncryptMode.R3 && _version < HpdfVersion.Version14)
            {
                _version = HpdfVersion.Version14; // R3 requires PDF 1.4
            }
            else if (mode == HpdfEncryptMode.R4 && _version < HpdfVersion.Version16)
            {
                _version = HpdfVersion.Version16; // R4 (AES) requires PDF 1.6
            }
        }

        /// <summary>
        /// Gets whether encryption is enabled for this document.
        /// </summary>
        public bool IsEncrypted => _encryptDict != null;

        /// <summary>
        /// Saves the document to a stream.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        public void Save(Stream stream)
        {
            if (stream == null)
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "Stream cannot be null");

            // Add PDF/A components if enabled
            if (_pdfACompliant)
                ApplyPdfACompliance();

            // Prepare encryption if enabled
            if (_encryptDict != null)
                ApplyEncryption();

            // Prepare outlines before writing
            if (_outlineRoot != null)
                PrepareOutlines(_outlineRoot);

            // Apply page labels if any have been added
            ApplyPageLabels();

            var hpdfStream = new HpdfMemoryStream();

            // Write PDF header
            WriteHeader(hpdfStream);

            // Write all objects and xref table
            _xref.WriteToStream(hpdfStream);

            // Copy to output stream
            var data = hpdfStream.ToArray();
            stream.Write(data, 0, data.Length);
        }

        /// <summary>
        /// Applies PDF/A compliance components to the document.
        /// </summary>
        private void ApplyPdfACompliance()
        {
            // 1. Add XMP Metadata
            var xmpGenerator = new HpdfXmpMetadata(_info, _pdfAConformance);
            var xmpStream = xmpGenerator.GenerateXmpStream(_xref);
            _catalog.Dict.Add("Metadata", xmpStream);

            // 2. Add Output Intent (sRGB)
            var outputIntent = HpdfOutputIntent.CreateSRgbIntent(_xref);
            var outputIntents = new HpdfArray();
            outputIntents.Add(outputIntent.Dict);
            _catalog.Dict.Add("OutputIntents", outputIntents);

            // 3. Add Document ID
            var documentId = HpdfDocumentId.GenerateId(_info);
            _xref.Trailer.Add("ID", documentId);
        }

        /// <summary>
        /// Applies encryption to the document.
        /// </summary>
        private void ApplyEncryption()
        {
            // Check if document ID already exists (e.g., from PDF/A compliance)
            HpdfArray documentId;
            if (_xref.Trailer.TryGetValue("ID", out var existingId) && existingId is HpdfArray existingIdArray)
            {
                documentId = existingIdArray;
            }
            else
            {
                // Generate new document ID (required for encryption)
                documentId = HpdfDocumentId.GenerateId(_info);
                _xref.Trailer.Add("ID", documentId);
            }

            // Extract the encryption ID bytes from the ID array
            byte[] idBytes = null;
            if (documentId.Count > 0 && documentId[0] is HpdfBinary firstId)
            {
                idBytes = firstId.Value;
            }

            // Prepare encryption dictionary with the ID
            _encryptDict.Prepare(idBytes, _info);

            // Add encryption dictionary to trailer
            _xref.Trailer.Add("Encrypt", _encryptDict.Dict);

            // Store encryption handler in xref for use during object writing
            _xref.SetEncryption(_encryptDict.Encrypt);
        }

        /// <summary>
        /// Prepares outlines for writing by calling BeforeWrite recursively.
        /// </summary>
        /// <param name="outline">The outline to prepare.</param>
        private void PrepareOutlines(HpdfOutline outline)
        {
            outline.BeforeWrite();

            // Process all children recursively
            if (outline.Dict.TryGetValue("First", out var firstObj) && firstObj is HpdfDict firstChild)
            {
                var current = firstChild;
                while (current != null)
                {
                    var childOutline = new HpdfOutline(current, _xref);
                    PrepareOutlines(childOutline);

                    if (current.TryGetValue("Next", out var nextObj) && nextObj is HpdfDict next)
                        current = next;
                    else
                        break;
                }
            }
        }

        /// <summary>
        /// Saves the document to a file.
        /// </summary>
        /// <param name="filename">The file path to save to.</param>
        public void SaveToFile(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "Filename cannot be null or empty");

            using (var fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                Save(fileStream);
            }
        }

        /// <summary>
        /// Saves the document to a byte array.
        /// </summary>
        /// <returns>The PDF document as a byte array.</returns>
        public byte[] SaveToMemory()
        {
            using (var memoryStream = new MemoryStream())
            {
                Save(memoryStream);
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Writes the PDF header to the stream.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        private void WriteHeader(HpdfStream stream)
        {
            var header = VersionHeaders[(int)_version];
            var headerBytes = Encoding.ASCII.GetBytes(header);
            stream.Write(headerBytes, 0, headerBytes.Length);
        }

        /// <summary>
        /// Adds a page label to define custom page numbering for a page range.
        /// </summary>
        /// <param name="pageNum">The starting page number (0-based index).</param>
        /// <param name="style">The numbering style.</param>
        /// <param name="firstPage">The value of the numeric portion for the first page (default: 1).</param>
        /// <param name="prefix">Optional prefix string (default: null).</param>
        public void AddPageLabel(int pageNum, HpdfPageNumStyle style, int firstPage = 1, string prefix = null)
        {
            if (pageNum < 0)
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "Page number must be >= 0");

            var label = new HpdfPageLabel(style, firstPage, prefix);
            _pageLabels[pageNum] = label;
        }

        /// <summary>
        /// Applies page labels to the document catalog.
        /// This method is called automatically before saving.
        /// </summary>
        private void ApplyPageLabels()
        {
            if (_pageLabels.Count == 0)
                return;

            // Create PageLabels dictionary with Nums array
            var pageLabelsDict = new HpdfDict();
            var numsArray = new HpdfArray();

            // Sort page labels by page number and add to Nums array
            var sortedLabels = new List<int>(_pageLabels.Keys);
            sortedLabels.Sort();

            foreach (var pageNum in sortedLabels)
            {
                var label = _pageLabels[pageNum];
                numsArray.Add(new HpdfNumber(pageNum));
                numsArray.Add(label.ToDict());
            }

            pageLabelsDict.Add("Nums", numsArray);

            // Add PageLabels to catalog
            _catalog.SetPageLabels(pageLabelsDict);
        }

        /// <summary>
        /// Gets the total number of pages in the document.
        /// </summary>
        public int PageCount => _pageList.Count;
    }
}
