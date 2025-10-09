namespace Haru.Types
{
    /// <summary>
    /// Encryption mode for PDF documents.
    /// </summary>
    public enum HpdfEncryptMode
    {
        /// <summary>
        /// Revision 2 - 40-bit RC4 encryption (PDF 1.3).
        /// Weakest encryption, use only for compatibility with old readers.
        /// </summary>
        R2 = 2,

        /// <summary>
        /// Revision 3 - 128-bit RC4 encryption (PDF 1.4).
        /// Standard encryption for most use cases.
        /// </summary>
        R3 = 3,

        /// <summary>
        /// Revision 4 - 128-bit AES encryption (PDF 1.6).
        /// Modern encryption with AES algorithm. Recommended for new documents.
        /// </summary>
        R4 = 4
    }
}
