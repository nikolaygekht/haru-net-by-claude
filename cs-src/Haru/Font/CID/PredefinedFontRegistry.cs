/*
 * << Haru Free PDF Library >> -- PredefinedFontRegistry.cs
 *
 * Copyright (c) 1999-2025 Haru Free PDF Library
 *
 * Permission to use, copy, modify, distribute and sell this software
 * and its documentation for any purpose is hereby granted without fee,
 */

using System;
using System.Collections.Generic;

namespace Haru.Font.CID
{
    /// <summary>
    /// Registry for predefined CJK fonts.
    /// Manages loading and caching of font definitions.
    /// </summary>
    public static class PredefinedFontRegistry
    {
        private static readonly Dictionary<string, PredefinedFontDefinition> _cache
            = new Dictionary<string, PredefinedFontDefinition>();

        private static readonly object _lock = new object();

        /// <summary>
        /// Gets a predefined font definition by name.
        /// Font definitions are loaded on demand and cached.
        /// </summary>
        /// <param name="fontName">The font name (e.g., "SimSun", "SimHei", "MingLiU").</param>
        /// <returns>The font definition.</returns>
        /// <exception cref="ArgumentException">If the font is not recognized.</exception>
        public static PredefinedFontDefinition GetDefinition(string fontName)
        {
            if (string.IsNullOrEmpty(fontName))
                throw new ArgumentNullException(nameof(fontName));

            lock (_lock)
            {
                // Check cache first
                if (_cache.TryGetValue(fontName, out var cached))
                    return cached;

                // Load from embedded resource
                try
                {
                    var fontDef = PredefinedFontDefinition.Load(fontName);
                    _cache[fontName] = fontDef;
                    return fontDef;
                }
                catch (Exception ex)
                {
                    throw new ArgumentException($"Predefined font '{fontName}' not found or could not be loaded.", nameof(fontName), ex);
                }
            }
        }

        /// <summary>
        /// Tries to get a predefined font definition by name.
        /// </summary>
        /// <param name="fontName">The font name.</param>
        /// <param name="fontDef">The font definition if found.</param>
        /// <returns>True if the font was found, false otherwise.</returns>
        public static bool TryGetDefinition(string fontName, out PredefinedFontDefinition fontDef)
        {
            try
            {
                fontDef = GetDefinition(fontName);
                return true;
            }
            catch
            {
                fontDef = null;
                return false;
            }
        }

        /// <summary>
        /// Registers a font definition in the cache (for testing/POC).
        /// </summary>
        /// <param name="fontName">The font name.</param>
        /// <param name="fontDef">The font definition.</param>
        public static void Register(string fontName, PredefinedFontDefinition fontDef)
        {
            if (string.IsNullOrEmpty(fontName))
                throw new ArgumentNullException(nameof(fontName));
            if (fontDef == null)
                throw new ArgumentNullException(nameof(fontDef));

            lock (_lock)
            {
                _cache[fontName] = fontDef;
            }
        }

        /// <summary>
        /// Clears the font definition cache (for testing).
        /// </summary>
        public static void ClearCache()
        {
            lock (_lock)
            {
                _cache.Clear();
            }
        }

        /// <summary>
        /// Checks if a font name is a recognized predefined font.
        /// </summary>
        /// <param name="fontName">The font name to check.</param>
        /// <returns>True if the font is recognized.</returns>
        public static bool IsPredefinedFont(string fontName)
        {
            if (string.IsNullOrEmpty(fontName))
                return false;

            // For now, check if we can load it
            return TryGetDefinition(fontName, out _);
        }
    }
}
