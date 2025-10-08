namespace Haru.Xref
{
    /// <summary>
    /// PDF cross-reference entry type
    /// </summary>
    public enum HpdfXrefEntryType
    {
        /// <summary>
        /// Free entry (not in use) - marked with 'f' in PDF
        /// </summary>
        Free = 'f',

        /// <summary>
        /// In-use entry - marked with 'n' in PDF
        /// </summary>
        InUse = 'n'
    }
}
