namespace Haru.Streams
{
    /// <summary>
    /// Types of HPDF streams
    /// </summary>
    public enum HpdfStreamType
    {
        /// <summary>
        /// Unknown stream type
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Callback-based stream
        /// </summary>
        Callback,

        /// <summary>
        /// File-based stream
        /// </summary>
        File,

        /// <summary>
        /// Memory-based stream
        /// </summary>
        Memory
    }
}
