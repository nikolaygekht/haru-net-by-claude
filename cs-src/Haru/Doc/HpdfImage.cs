/*
 * << Haru Free PDF Library >> -- HpdfImage.cs
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

using System;
using System.IO;
using Haru.Objects;
using Haru.Xref;
using Haru.Types;
using Haru.Streams;
using Haru.Png;

namespace Haru.Doc
{
    /// <summary>
    /// Represents a PDF image XObject.
    /// Images are external objects that can be drawn on pages using the Do operator.
    /// </summary>
    public class HpdfImage
    {
        private readonly HpdfStreamObject _streamObject;
        private readonly HpdfXref _xref;
        private readonly string _localName;

        /// <summary>
        /// Gets the underlying stream object.
        /// </summary>
        public HpdfStreamObject StreamObject => _streamObject;

        /// <summary>
        /// Gets the image dictionary (the stream object itself is a dictionary).
        /// </summary>
        public HpdfDict Dict => _streamObject;

        /// <summary>
        /// Gets the local resource name (e.g., "Im1").
        /// </summary>
        public string LocalName => _localName;

        /// <summary>
        /// Gets the image width in pixels.
        /// </summary>
        public uint Width
        {
            get
            {
                if (Dict.TryGetValue("Width", out var widthObj) && widthObj is HpdfNumber number)
                    return (uint)number.Value;
                return 0;
            }
        }

        /// <summary>
        /// Gets the image height in pixels.
        /// </summary>
        public uint Height
        {
            get
            {
                if (Dict.TryGetValue("Height", out var heightObj) && heightObj is HpdfNumber number)
                    return (uint)number.Value;
                return 0;
            }
        }

        private HpdfImage(HpdfXref xref, string localName)
        {
            if (xref == null)
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "Xref cannot be null");
            if (string.IsNullOrEmpty(localName))
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "Local name cannot be null or empty");

            _xref = xref;
            _localName = localName;
            _streamObject = new HpdfStreamObject();

            // Set up basic image XObject dictionary
            Dict.Add("Type", new HpdfName("XObject"));
            Dict.Add("Subtype", new HpdfName("Image"));

            // Add to xref
            xref.Add(_streamObject);
        }

        /// <summary>
        /// Loads a PNG image from a file.
        /// </summary>
        public static HpdfImage LoadPngImage(HpdfXref xref, string localName, string filePath)
        {
            if (!File.Exists(filePath))
                throw new HpdfException(HpdfErrorCode.InvalidParameter, $"File not found: {filePath}");

            using (var fileStream = File.OpenRead(filePath))
            {
                return LoadPngImage(xref, localName, fileStream);
            }
        }

        /// <summary>
        /// Loads a PNG image from a file (alias for LoadPngImage for API compatibility).
        /// </summary>
        public static HpdfImage LoadPngImageFromFile(HpdfXref xref, string localName, string filePath)
        {
            return LoadPngImage(xref, localName, filePath);
        }

        /// <summary>
        /// Loads a PNG image from a stream.
        /// </summary>
        public static HpdfImage LoadPngImage(HpdfXref xref, string localName, Stream stream)
        {
            var image = new HpdfImage(xref, localName);

            using (var pngReader = PngReaderFactory.Create())
            {
                if (!pngReader.ValidateSignature(stream))
                    throw new HpdfException(HpdfErrorCode.InvalidPngImage, "Invalid PNG signature");

                stream.Position = 0;
                var info = pngReader.ReadImageInfo(stream);

                // Set image dimensions
                image.Dict.Add("Width", new HpdfNumber((int)info.Width));
                image.Dict.Add("Height", new HpdfNumber((int)info.Height));
                image.Dict.Add("BitsPerComponent", new HpdfNumber(info.BitDepth));

                // Read pixel data
                stream.Position = 0;
                var pixelData = pngReader.ReadPixelData(stream, info);

                // Set color space and process transparency
                image.SetPngColorSpace(info, pixelData);

                // Process pixel data based on color type
                byte[] processedData = pixelData;
                if (info.ColorType == PngColorType.RgbAlpha || info.ColorType == PngColorType.GrayscaleAlpha)
                {
                    // Strip alpha channel - will be handled separately in SMask
                    int bytesPerPixel = info.ColorType == PngColorType.RgbAlpha ? 4 : 2;
                    int colorComponents = info.ColorType == PngColorType.RgbAlpha ? 3 : 1;
                    processedData = StripAlphaChannel(pixelData, (int)info.Width, (int)info.Height, bytesPerPixel, colorComponents);
                }

                // Write processed pixel data to stream and set FlateDecode filter
                image._streamObject.Stream.Write(processedData, 0, processedData.Length);
                image._streamObject.Filter = HpdfStreamFilter.FlateDecode;
            }

            return image;
        }

        private void SetPngColorSpace(PngImageInfo info, byte[] pixelData)
        {
            switch (info.ColorType)
            {
                case PngColorType.Grayscale:
                    Dict.Add("ColorSpace", new HpdfName("DeviceGray"));
                    break;

                case PngColorType.Rgb:
                    Dict.Add("ColorSpace", new HpdfName("DeviceRGB"));
                    break;

                case PngColorType.Palette:
                    SetIndexedColorSpace(info);
                    break;

                case PngColorType.GrayscaleAlpha:
                    // Grayscale with alpha - use DeviceGray and create SMask
                    Dict.Add("ColorSpace", new HpdfName("DeviceGray"));
                    CreateSMaskForAlpha(info, pixelData, 2); // 2 bytes per pixel (gray + alpha)
                    break;

                case PngColorType.RgbAlpha:
                    // RGB with alpha - use DeviceRGB and create SMask
                    Dict.Add("ColorSpace", new HpdfName("DeviceRGB"));
                    CreateSMaskForAlpha(info, pixelData, 4); // 4 bytes per pixel (R + G + B + alpha)
                    break;

                default:
                    throw new HpdfException(HpdfErrorCode.InvalidPngImage, $"Unsupported PNG color type: {info.ColorType}");
            }
        }

        private void SetIndexedColorSpace(PngImageInfo info)
        {
            if (info.Palette == null || info.Palette.Colors.Length == 0)
                throw new HpdfException(HpdfErrorCode.InvalidPngImage, "Palette PNG image has no palette data");

            // Create indexed color space array: [/Indexed /DeviceRGB maxIndex paletteData]
            var colorSpaceArray = new HpdfArray();
            colorSpaceArray.Add(new HpdfName("Indexed"));
            colorSpaceArray.Add(new HpdfName("DeviceRGB"));
            colorSpaceArray.Add(new HpdfNumber(info.Palette.ColorCount - 1));

            // Create palette data as binary string
            var paletteData = new byte[info.Palette.ColorCount * 3];
            for (int i = 0; i < info.Palette.ColorCount; i++)
            {
                var color = info.Palette.GetColor(i);
                paletteData[i * 3] = color.R;
                paletteData[i * 3 + 1] = color.G;
                paletteData[i * 3 + 2] = color.B;
            }
            colorSpaceArray.Add(new HpdfBinary(paletteData));

            Dict.Add("ColorSpace", colorSpaceArray);
        }

        /// <summary>
        /// Loads a JPEG image from a file.
        /// </summary>
        public static HpdfImage LoadJpegImage(HpdfXref xref, string localName, string filePath)
        {
            if (!File.Exists(filePath))
                throw new HpdfException(HpdfErrorCode.InvalidParameter, $"File not found: {filePath}");

            using (var fileStream = File.OpenRead(filePath))
            {
                return LoadJpegImage(xref, localName, fileStream);
            }
        }

        /// <summary>
        /// Loads a JPEG image from a stream.
        /// </summary>
        public static HpdfImage LoadJpegImage(HpdfXref xref, string localName, Stream stream)
        {
            var image = new HpdfImage(xref, localName);

            // Parse JPEG header to get dimensions and color space
            var jpegInfo = ParseJpegHeader(stream);

            // Set image properties
            image.Dict.Add("Width", new HpdfNumber((int)jpegInfo.Width));
            image.Dict.Add("Height", new HpdfNumber((int)jpegInfo.Height));
            image.Dict.Add("BitsPerComponent", new HpdfNumber(jpegInfo.BitsPerComponent));
            image.Dict.Add("ColorSpace", new HpdfName(jpegInfo.ColorSpace));

            // For CMYK JPEG, add Decode array
            if (jpegInfo.Components == 4)
            {
                var decodeArray = new HpdfArray();
                for (int i = 0; i < 4; i++)
                {
                    decodeArray.Add(new HpdfNumber(1));
                    decodeArray.Add(new HpdfNumber(0));
                }
                image.Dict.Add("Decode", decodeArray);
            }

            // Copy JPEG data directly to stream (already DCT-compressed)
            stream.Position = 0;
            byte[] buffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                image._streamObject.Stream.Write(buffer, 0, bytesRead);
            }

            // Set DCTDecode filter - PrepareStreamData will pass the data through unchanged
            // since JPEG files are already DCT-compressed
            image._streamObject.Filter = HpdfStreamFilter.DctDecode;

            return image;
        }

        private static JpegInfo ParseJpegHeader(Stream stream)
        {
            stream.Position = 0;
            using (var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true))
            {
                // Check JPEG signature (0xFF 0xD8)
                ushort marker = ReadUInt16BE(reader);
                if (marker != 0xFFD8)
                    throw new HpdfException(HpdfErrorCode.InvalidJpegData, "Invalid JPEG signature");

                // Find SOF (Start of Frame) marker
                while (true)
                {
                    marker = ReadUInt16BE(reader);
                    ushort length = ReadUInt16BE(reader);

                    // Check for SOF markers (0xFFC0-0xFFC3, 0xFFC5-0xFFC7, 0xFFC9-0xFFCB, 0xFFCD-0xFFCF)
                    if (marker == 0xFFC0 || marker == 0xFFC1 || marker == 0xFFC2 || marker == 0xFFC9)
                    {
                        byte precision = reader.ReadByte();
                        ushort height = ReadUInt16BE(reader);
                        ushort width = ReadUInt16BE(reader);
                        byte components = reader.ReadByte();

                        string colorSpace;
                        switch (components)
                        {
                            case 1:
                                colorSpace = "DeviceGray";
                                break;
                            case 3:
                                colorSpace = "DeviceRGB";
                                break;
                            case 4:
                                colorSpace = "DeviceCMYK";
                                break;
                            default:
                                throw new HpdfException(HpdfErrorCode.UnsupportedJpegFormat, $"Unsupported JPEG component count: {components}");
                        }

                        return new JpegInfo
                        {
                            Width = width,
                            Height = height,
                            BitsPerComponent = precision,
                            Components = components,
                            ColorSpace = colorSpace
                        };
                    }

                    // Skip to next marker
                    if ((marker & 0xFF00) != 0xFF00)
                        throw new HpdfException(HpdfErrorCode.UnsupportedJpegFormat, "Lost JPEG marker");

                    stream.Seek(length - 2, SeekOrigin.Current);
                }
            }
        }

        private static ushort ReadUInt16BE(BinaryReader reader)
        {
            byte b1 = reader.ReadByte();
            byte b2 = reader.ReadByte();
            return (ushort)((b1 << 8) | b2);
        }

        /// <summary>
        /// Strips the alpha channel from pixel data, leaving only the color components.
        /// </summary>
        private static byte[] StripAlphaChannel(byte[] pixelData, int width, int height, int bytesPerPixel, int colorComponents)
        {
            byte[] result = new byte[width * height * colorComponents];
            int destIndex = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int srcIndex = (y * width + x) * bytesPerPixel;

                    // Copy color components (skip alpha which is the last component)
                    for (int c = 0; c < colorComponents; c++)
                    {
                        result[destIndex++] = pixelData[srcIndex + c];
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Creates a soft mask (SMask) image for alpha channel transparency.
        /// </summary>
        private void CreateSMaskForAlpha(PngImageInfo info, byte[] pixelData, int bytesPerPixel)
        {
            // Extract alpha channel (last component of each pixel)
            int width = (int)info.Width;
            int height = (int)info.Height;
            byte[] alphaData = new byte[width * height];
            int alphaIndex = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int pixelIndex = (y * width + x) * bytesPerPixel;
                    alphaData[alphaIndex++] = pixelData[pixelIndex + bytesPerPixel - 1]; // Last component is alpha
                }
            }

            // Create SMask image object
            var smask = new HpdfStreamObject();
            smask.Add("Type", new HpdfName("XObject"));
            smask.Add("Subtype", new HpdfName("Image"));
            smask.Add("Width", new HpdfNumber(width));
            smask.Add("Height", new HpdfNumber(height));
            smask.Add("ColorSpace", new HpdfName("DeviceGray"));
            smask.Add("BitsPerComponent", new HpdfNumber(info.BitDepth));

            // Write alpha data to SMask stream
            smask.Stream.Write(alphaData, 0, alphaData.Length);
            smask.Filter = HpdfStreamFilter.FlateDecode;

            // Add SMask to xref
            _xref.Add(smask);

            // Add SMask reference to main image
            Dict.Add("SMask", smask);
        }

        /// <summary>
        /// Sets a color mask for the image, making pixels within the specified RGB ranges transparent.
        /// This is useful for simple transparency effects on RGB images.
        /// </summary>
        /// <param name="rmin">Minimum red value (0-255).</param>
        /// <param name="rmax">Maximum red value (0-255).</param>
        /// <param name="gmin">Minimum green value (0-255).</param>
        /// <param name="gmax">Maximum green value (0-255).</param>
        /// <param name="bmin">Minimum blue value (0-255).</param>
        /// <param name="bmax">Maximum blue value (0-255).</param>
        public void SetColorMask(uint rmin, uint rmax, uint gmin, uint gmax, uint bmin, uint bmax)
        {
            // PDF color mask array: [rmin rmax gmin gmax bmin bmax]
            // Pixels with color components within these ranges will be transparent
            var maskArray = new HpdfArray();
            maskArray.Add(new HpdfNumber((int)rmin));
            maskArray.Add(new HpdfNumber((int)rmax));
            maskArray.Add(new HpdfNumber((int)gmin));
            maskArray.Add(new HpdfNumber((int)gmax));
            maskArray.Add(new HpdfNumber((int)bmin));
            maskArray.Add(new HpdfNumber((int)bmax));

            Dict.Add("Mask", maskArray);
        }

        /// <summary>
        /// Sets a mask image for this image, providing alpha channel transparency.
        /// The mask image must be a grayscale image with the same dimensions.
        /// </summary>
        /// <param name="maskImage">The mask image (must be grayscale).</param>
        public void SetMaskImage(HpdfImage maskImage)
        {
            if (maskImage == null)
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "Mask image cannot be null");

            // Verify mask image is grayscale
            if (maskImage.Dict.TryGetValue("ColorSpace", out var colorSpace))
            {
                var colorSpaceName = colorSpace as HpdfName;
                if (colorSpaceName == null || colorSpaceName.Value != "DeviceGray")
                {
                    throw new HpdfException(HpdfErrorCode.InvalidParameter,
                        "Mask image must have DeviceGray color space");
                }
            }

            // Verify dimensions match
            if (maskImage.Width != Width || maskImage.Height != Height)
            {
                throw new HpdfException(HpdfErrorCode.InvalidParameter,
                    "Mask image dimensions must match the main image");
            }

            // Convert the grayscale image data to 1-bit packed format
            // The mask image is currently stored as 8-bit grayscale (1 byte per pixel)
            // We need to convert it to 1-bit (8 pixels per byte, packed)
            var originalData = maskImage._streamObject.Stream.ToArray();
            int width = (int)maskImage.Width;
            int height = (int)maskImage.Height;
            int expectedSize = width * height; // 1 byte per pixel for 8-bit grayscale

            if (originalData.Length < expectedSize)
            {
                throw new HpdfException(HpdfErrorCode.InvalidImage,
                    $"Mask image data size mismatch: expected {expectedSize}, got {originalData.Length}");
            }

            // Convert 8-bit grayscale to 1-bit packed format
            // Each row must be padded to byte boundary
            int bytesPerRow = (width + 7) / 8; // Round up to nearest byte
            byte[] packedData = new byte[bytesPerRow * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int srcIndex = y * width + x;
                    byte pixelValue = originalData[srcIndex];

                    // Threshold: values >= 128 are considered opaque (1), < 128 are transparent (0)
                    if (pixelValue >= 128)
                    {
                        int dstIndex = y * bytesPerRow + (x / 8);
                        int bitPosition = 7 - (x % 8); // MSB first
                        packedData[dstIndex] |= (byte)(1 << bitPosition);
                    }
                }
            }

            // Clear the old stream data and write the new packed data
            maskImage._streamObject.ClearStream();
            maskImage._streamObject.Stream.Write(packedData, 0, packedData.Length);

            // Convert the mask image to an ImageMask (1-bit stencil mask)
            // Remove ColorSpace from the mask and add ImageMask flag
            maskImage.Dict.Remove("ColorSpace");
            maskImage.Dict["ImageMask"] = new HpdfBoolean(true);

            // For ImageMask, only 1 bit per component is allowed
            maskImage.Dict["BitsPerComponent"] = new HpdfNumber(1);

            // Decode array for ImageMask: [0 1] means 0=transparent, 1=opaque
            var decode = new HpdfArray();
            decode.Add(new HpdfNumber(0));
            decode.Add(new HpdfNumber(1));
            maskImage.Dict["Decode"] = decode;

            // Add Mask entry as indirect reference to the mask image stream
            Dict.Add("Mask", maskImage.StreamObject);
        }

        private class JpegInfo
        {
            public ushort Width { get; set; }
            public ushort Height { get; set; }
            public byte BitsPerComponent { get; set; }
            public byte Components { get; set; }
            public string ColorSpace { get; set; }
        }
    }
}
