namespace Haru.Png
{
    /// <summary>
    /// Factory for creating PNG reader instances.
    /// </summary>
    public static class PngReaderFactory
    {
        /// <summary>
        /// Creates a new PNG reader instance using the default implementation.
        /// </summary>
        /// <returns>A new IPngReader instance</returns>
        public static IPngReader Create()
        {
            return new StbImageReader();
        }

        /// <summary>
        /// Creates a new PNG reader instance of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of PNG reader to create</typeparam>
        /// <returns>A new IPngReader instance</returns>
        public static IPngReader Create<T>() where T : IPngReader, new()
        {
            return new T();
        }
    }
}
