using System;

namespace Haru.Types
{
    /// <summary>
    /// Document permission flags for encrypted PDFs.
    /// These flags control what operations users can perform on the encrypted document.
    /// </summary>
    [Flags]
    public enum HpdfPermission : uint
    {
        /// <summary>
        /// No permissions granted.
        /// </summary>
        None = 0,

        /// <summary>
        /// Allow printing the document.
        /// Revision 2: Print the document.
        /// Revision 3+: Print the document (possibly not at full quality).
        /// </summary>
        Print = 0x00000004,

        /// <summary>
        /// Allow modifying the document contents.
        /// </summary>
        Edit = 0x00000008,

        /// <summary>
        /// Allow copying or extracting text and graphics.
        /// </summary>
        Copy = 0x00000010,

        /// <summary>
        /// Allow adding or modifying text annotations and interactive form fields.
        /// </summary>
        EditAnnotations = 0x00000020,

        /// <summary>
        /// (Revision 3+) Allow filling in form fields and signing.
        /// </summary>
        FillForm = 0x00000100,

        /// <summary>
        /// (Revision 3+) Allow extracting text and graphics for accessibility purposes.
        /// </summary>
        ExtractAccess = 0x00000200,

        /// <summary>
        /// (Revision 3+) Allow assembling the document (insert, rotate, or delete pages
        /// and create bookmarks or thumbnail images).
        /// </summary>
        Assemble = 0x00000400,

        /// <summary>
        /// (Revision 3+) Allow printing the document at high quality.
        /// </summary>
        PrintHighQuality = 0x00000800,

        /// <summary>
        /// All permissions granted (default for unencrypted documents).
        /// Note: Some bits must always be set for proper PDF encryption.
        /// </summary>
        All = Print | Edit | Copy | EditAnnotations | FillForm |
              ExtractAccess | Assemble | PrintHighQuality,

        /// <summary>
        /// Permission padding bits that must always be set (bits 7, 8, 9-31).
        /// This is used internally for PDF encryption compliance.
        /// </summary>
        PaddingBits = 0xFFFFF0C0
    }
}
