using System;

namespace Haru.Objects
{
    /// <summary>
    /// Object ownership and lifecycle flags
    /// </summary>
    [Flags]
    public enum HpdfObjectType : uint
    {
        /// <summary>
        /// No type specified
        /// </summary>
        None = 0x00000000,

        /// <summary>
        /// Object is owned by a container object (embedded)
        /// </summary>
        Direct = 0x80000000,

        /// <summary>
        /// Object is managed by xref table (indirect reference)
        /// </summary>
        Indirect = 0x40000000,

        /// <summary>
        /// Object can be either direct or indirect
        /// </summary>
        Any = Direct | Indirect,

        /// <summary>
        /// Object is hidden from output
        /// </summary>
        Hidden = 0x10000000
    }

    /// <summary>
    /// PDF object class types
    /// </summary>
    public enum HpdfObjectClass : ushort
    {
        /// <summary>
        /// Unknown object type
        /// </summary>
        Unknown = 0x0001,

        /// <summary>
        /// Null object
        /// </summary>
        Null = 0x0002,

        /// <summary>
        /// Boolean object (true/false)
        /// </summary>
        Boolean = 0x0003,

        /// <summary>
        /// Integer number object
        /// </summary>
        Number = 0x0004,

        /// <summary>
        /// Real (floating-point) number object
        /// </summary>
        Real = 0x0005,

        /// <summary>
        /// Name object (/Name)
        /// </summary>
        Name = 0x0006,

        /// <summary>
        /// String object (text)
        /// </summary>
        String = 0x0007,

        /// <summary>
        /// Binary data object
        /// </summary>
        Binary = 0x0008,

        /// <summary>
        /// Array object [...]
        /// </summary>
        Array = 0x0010,

        /// <summary>
        /// Dictionary object &lt;&lt;...&gt;&gt;
        /// </summary>
        Dict = 0x0011,

        /// <summary>
        /// Proxy object (reference to another object)
        /// </summary>
        Proxy = 0x0012,

        /// <summary>
        /// Any object class
        /// </summary>
        Any = 0x00FF
    }

    /// <summary>
    /// PDF object subclass types for specialized dictionaries
    /// </summary>
    public enum HpdfObjectSubclass : ushort
    {
        /// <summary>
        /// No subclass
        /// </summary>
        None = 0x0000,

        /// <summary>
        /// Font dictionary
        /// </summary>
        Font = 0x0100,

        /// <summary>
        /// Catalog dictionary
        /// </summary>
        Catalog = 0x0200,

        /// <summary>
        /// Pages dictionary
        /// </summary>
        Pages = 0x0300,

        /// <summary>
        /// Page dictionary
        /// </summary>
        Page = 0x0400,

        /// <summary>
        /// XObject dictionary
        /// </summary>
        XObject = 0x0500,

        /// <summary>
        /// Outline dictionary
        /// </summary>
        Outline = 0x0600,

        /// <summary>
        /// Destination dictionary
        /// </summary>
        Destination = 0x0700,

        /// <summary>
        /// Annotation dictionary
        /// </summary>
        Annotation = 0x0800,

        /// <summary>
        /// Encryption dictionary
        /// </summary>
        Encrypt = 0x0900,

        /// <summary>
        /// Extended graphics state dictionary
        /// </summary>
        ExtGState = 0x0A00,

        /// <summary>
        /// Extended graphics state dictionary (read-only)
        /// </summary>
        ExtGStateReadOnly = 0x0B00,

        /// <summary>
        /// Name dictionary
        /// </summary>
        NameDict = 0x0C00,

        /// <summary>
        /// Name tree
        /// </summary>
        NameTree = 0x0D00
    }
}
