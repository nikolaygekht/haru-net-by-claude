using System;
using System.IO;
using System.Reflection;

namespace Haru.Test
{
    /// <summary>
    /// Helper class for accessing test resources embedded in the assembly.
    /// </summary>
    public static class TestHelper
    {
        /// <summary>
        /// Gets a stream for an embedded resource file.
        /// </summary>
        /// <param name="resourceName">The name of the resource file (e.g., "test_grayscale_2x2.png")</param>
        /// <returns>A stream containing the resource data</returns>
        public static Stream GetResourceStream(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fullResourceName = $"Haru.Test.Resources.{resourceName}";

            var stream = assembly.GetManifestResourceStream(fullResourceName);

            if (stream == null)
            {
                throw new FileNotFoundException(
                    $"Embedded resource '{fullResourceName}' not found. " +
                    $"Available resources: {string.Join(", ", assembly.GetManifestResourceNames())}");
            }

            return stream;
        }

        /// <summary>
        /// Gets the bytes of an embedded resource file.
        /// </summary>
        /// <param name="resourceName">The name of the resource file</param>
        /// <returns>The resource data as a byte array</returns>
        public static byte[] GetResourceBytes(string resourceName)
        {
            using (var stream = GetResourceStream(resourceName))
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
