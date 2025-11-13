using System;
using System.Collections.Concurrent;
using System.IO;
using Haru.Font.TrueType;
using Haru.Font.Type1;

namespace Haru.Font
{
    /// <summary>
    /// Thread-safe global pool for sharing parsed font data across all HpdfDocument instances.
    /// This eliminates redundant font file parsing and reduces memory usage when multiple documents
    /// use the same fonts.
    ///
    /// Design principles:
    /// - Thread-safe: Uses ConcurrentDictionary for lock-free reads
    /// - Key by absolute path: Ensures same file = same cached data
    /// - Explicit cleanup: Call Clear() to free memory (e.g., between test runs or after batch processing)
    /// - Immutable data: ParsedFontData instances are read-only after creation
    /// </summary>
    public static class FontDataPool
    {
        // Cache TrueType fonts by absolute file path
        private static readonly ConcurrentDictionary<string, ParsedTrueTypeData> _trueTypeFonts =
            new ConcurrentDictionary<string, ParsedTrueTypeData>(StringComparer.OrdinalIgnoreCase);

        // Cache Type1 fonts by AFM path (AFM+PFB are treated as a pair)
        private static readonly ConcurrentDictionary<string, ParsedType1Data> _type1Fonts =
            new ConcurrentDictionary<string, ParsedType1Data>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Gets or loads TrueType font data from the pool.
        /// If the font is already cached, returns the cached data.
        /// Otherwise, loads and parses the font file, caches it, and returns the data.
        /// </summary>
        /// <param name="filePath">Path to the TrueType font file (.ttf or .otf).</param>
        /// <param name="codePage">Code page for text encoding (default: 437 for standard ASCII).</param>
        /// <returns>Parsed TrueType font data that can be shared across documents.</returns>
        /// <exception cref="ArgumentNullException">If filePath is null.</exception>
        /// <exception cref="FileNotFoundException">If the font file does not exist.</exception>
        /// <exception cref="InvalidOperationException">If the font file is invalid or cannot be parsed.</exception>
        public static ParsedTrueTypeData GetOrLoadTrueType(string filePath, int codePage = 437)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath));

            // Normalize to absolute path for consistent cache key
            string absolutePath = Path.GetFullPath(filePath);

            if (!File.Exists(absolutePath))
                throw new FileNotFoundException($"TrueType font file not found: {absolutePath}", absolutePath);

            // Try to get from cache first (lock-free read)
            if (_trueTypeFonts.TryGetValue(absolutePath, out var cachedData))
            {
                return cachedData;
            }

            // Not in cache - load and parse the font
            // GetOrAdd ensures only one thread parses the font even if multiple threads request it simultaneously
            var parsedData = _trueTypeFonts.GetOrAdd(absolutePath, path => LoadTrueTypeFont(path, codePage));

            return parsedData;
        }

        /// <summary>
        /// Gets or loads Type1 font data from the pool.
        /// Type1 fonts consist of two files: AFM (metrics) and PFB (font program).
        /// </summary>
        /// <param name="afmPath">Path to the AFM (Adobe Font Metrics) file.</param>
        /// <param name="pfbPath">Path to the PFB (Printer Font Binary) file.</param>
        /// <returns>Parsed Type1 font data that can be shared across documents.</returns>
        /// <exception cref="ArgumentNullException">If afmPath or pfbPath is null.</exception>
        /// <exception cref="FileNotFoundException">If either file does not exist.</exception>
        /// <exception cref="InvalidOperationException">If the font files are invalid or cannot be parsed.</exception>
        public static ParsedType1Data GetOrLoadType1(string afmPath, string pfbPath)
        {
            if (string.IsNullOrWhiteSpace(afmPath))
                throw new ArgumentNullException(nameof(afmPath));
            if (string.IsNullOrWhiteSpace(pfbPath))
                throw new ArgumentNullException(nameof(pfbPath));

            // Normalize to absolute paths
            string absoluteAfmPath = Path.GetFullPath(afmPath);
            string absolutePfbPath = Path.GetFullPath(pfbPath);

            if (!File.Exists(absoluteAfmPath))
                throw new FileNotFoundException($"AFM file not found: {absoluteAfmPath}", absoluteAfmPath);
            if (!File.Exists(absolutePfbPath))
                throw new FileNotFoundException($"PFB file not found: {absolutePfbPath}", absolutePfbPath);

            // Cache key is the AFM path (AFM+PFB are treated as a pair)
            if (_type1Fonts.TryGetValue(absoluteAfmPath, out var cachedData))
            {
                return cachedData;
            }

            // Not in cache - load and parse the font
            var parsedData = _type1Fonts.GetOrAdd(absoluteAfmPath, path => LoadType1Font(absoluteAfmPath, absolutePfbPath));

            return parsedData;
        }

        /// <summary>
        /// Clears all cached font data. Call this to free memory when fonts are no longer needed
        /// (e.g., after batch PDF generation, between test runs, or on application shutdown).
        /// </summary>
        public static void Clear()
        {
            _trueTypeFonts.Clear();
            _type1Fonts.Clear();
        }

        /// <summary>
        /// Gets the current cache statistics for monitoring and diagnostics.
        /// </summary>
        public static FontDataPoolStats GetStats()
        {
            return new FontDataPoolStats
            {
                TrueTypeFontsCount = _trueTypeFonts.Count,
                Type1FontsCount = _type1Fonts.Count
            };
        }

        // Private helper: Loads and parses a TrueType font file
        private static ParsedTrueTypeData LoadTrueTypeFont(string absolutePath, int codePage)
        {
            try
            {
                // Read entire font file into memory (this is the only disk I/O)
                byte[] fontData = File.ReadAllBytes(absolutePath);

                // Extract font name for caching purposes
                string extractedFontName = ExtractFontNameFromBytes(fontData);

                return new ParsedTrueTypeData(
                    filePath: absolutePath,
                    fontData: fontData,
                    extractedFontName: extractedFontName,
                    codePage: codePage
                );
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to load TrueType font from {absolutePath}: {ex.Message}", ex);
            }
        }

        // Private helper: Loads and parses a Type1 font
        private static ParsedType1Data LoadType1Font(string absoluteAfmPath, string absolutePfbPath)
        {
            try
            {
                // Parse AFM file
                AfmData afmData = AfmParser.ParseFile(absoluteAfmPath);

                // Read PFB data
                byte[] pfbData = File.ReadAllBytes(absolutePfbPath);

                // Extract font name from AFM
                string extractedFontName = afmData.FontName ?? Path.GetFileNameWithoutExtension(absoluteAfmPath);

                return new ParsedType1Data(
                    afmPath: absoluteAfmPath,
                    pfbPath: absolutePfbPath,
                    afmData: afmData,
                    pfbData: pfbData,
                    extractedFontName: extractedFontName
                );
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to load Type1 font from {absoluteAfmPath}: {ex.Message}", ex);
            }
        }

        // Private helper: Extracts font name from TrueType font bytes
        private static string ExtractFontNameFromBytes(byte[] fontData)
        {
            using (var stream = new MemoryStream(fontData))
            using (var parser = new TrueTypeParser(stream))
            {
                var offsetTable = parser.ParseOffsetTable();
                var nameTableRef = parser.FindTable(offsetTable, "name");

                if (nameTableRef is null)
                    return $"CustomTTFont-{Guid.NewGuid():N}";

                var nameTable = parser.ParseName(nameTableRef);

                if (nameTable is null || nameTable.NameRecords is null)
                    return $"CustomTTFont-{Guid.NewGuid():N}";

                // Priority 1: PostScript name (ID 6) - Platform 3, Encoding 1, Language 0x0409
                TrueTypeNameRecord? postScriptName = FindNameRecord(nameTable.NameRecords, 6, 3, 1, 0x0409);

                // Priority 2: PostScript name - Platform 3, any encoding/language
                if (postScriptName is null)
                    postScriptName = FindNameRecord(nameTable.NameRecords, 6, 3);

                // Priority 3: PostScript name - Platform 1 (Macintosh)
                if (postScriptName is null)
                    postScriptName = FindNameRecord(nameTable.NameRecords, 6, 1);

                // Priority 4: PostScript name - Any platform
                if (postScriptName is null)
                    postScriptName = FindNameRecord(nameTable.NameRecords, 6);

                if (postScriptName != null)
                {
                    string? name = parser.ReadNameString(nameTable, nameTableRef, postScriptName);
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        return CleanFontName(name);
                    }
                }

                // Fallback: Family name (ID 1) + Subfamily name (ID 2)
                TrueTypeNameRecord? familyName = FindNameRecord(nameTable.NameRecords, 1, 3, 1, 0x0409);
                if (familyName is null)
                    familyName = FindNameRecord(nameTable.NameRecords, 1, 3);
                if (familyName is null)
                    familyName = FindNameRecord(nameTable.NameRecords, 1);

                TrueTypeNameRecord? subfamilyName = FindNameRecord(nameTable.NameRecords, 2, 3, 1, 0x0409);
                if (subfamilyName is null)
                    subfamilyName = FindNameRecord(nameTable.NameRecords, 2, 3);
                if (subfamilyName is null)
                    subfamilyName = FindNameRecord(nameTable.NameRecords, 2);

                if (familyName != null)
                {
                    string? family = parser.ReadNameString(nameTable, nameTableRef, familyName);
                    if (!string.IsNullOrWhiteSpace(family))
                    {
                        string? subfamily = null;
                        if (subfamilyName != null)
                        {
                            subfamily = parser.ReadNameString(nameTable, nameTableRef, subfamilyName);
                        }

                        if (!string.IsNullOrWhiteSpace(subfamily) && subfamily != "Regular")
                        {
                            return CleanFontName($"{family}-{subfamily.Replace(" ", "")}");
                        }
                        else
                        {
                            return CleanFontName(family);
                        }
                    }
                }

                // Last resort
                return $"CustomTTFont-{Guid.NewGuid():N}";
            }
        }

        private static TrueTypeNameRecord? FindNameRecord(TrueTypeNameRecord[] records, ushort nameId,
            ushort? platformId = null, ushort? encodingId = null, ushort? languageId = null)
        {
            foreach (var record in records)
            {
                if (record.NameId == nameId &&
                    (!platformId.HasValue || record.PlatformId == platformId.Value) &&
                    (!encodingId.HasValue || record.EncodingId == encodingId.Value) &&
                    (!languageId.HasValue || record.LanguageId == languageId.Value))
                {
                    return record;
                }
            }
            return null;
        }

        private static string CleanFontName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return $"CustomTTFont-{Guid.NewGuid():N}";

            var cleaned = new System.Text.StringBuilder();
            foreach (char c in name)
            {
                if (c > 32 && c < 127 &&
                    c != '#' && c != '/' && c != '(' && c != ')' &&
                    c != '<' && c != '>' && c != '[' && c != ']' &&
                    c != '{' && c != '}' && c != '%' && c != ' ')
                {
                    cleaned.Append(c);
                }
                else if (c == ' ')
                {
                    cleaned.Append('-');
                }
            }

            string result = cleaned.ToString();
            return string.IsNullOrWhiteSpace(result) ? $"CustomTTFont-{Guid.NewGuid():N}" : result;
        }
    }

    /// <summary>
    /// Statistics about the font data pool for monitoring and diagnostics.
    /// </summary>
    public class FontDataPoolStats
    {
        /// <summary>
        /// Number of TrueType fonts currently cached.
        /// </summary>
        public int TrueTypeFontsCount { get; init; }

        /// <summary>
        /// Number of Type1 fonts currently cached.
        /// </summary>
        public int Type1FontsCount { get; init; }

        /// <summary>
        /// Total number of fonts cached.
        /// </summary>
        public int TotalFontsCount => TrueTypeFontsCount + Type1FontsCount;
    }
}
