#!/usr/bin/env python3
"""
Create test PNG files for the Haru PNG reader tests.
Creates small 2x2 pixel PNG files of different types.
"""

from PIL import Image
import os

output_dir = "cs-src/Haru.Test/Resources"
os.makedirs(output_dir, exist_ok=True)

# 1. Grayscale 2x2 PNG
# Pixels: (0,0)=Black, (1,0)=DarkGray, (0,1)=LightGray, (1,1)=White
grayscale = Image.new('L', (2, 2))
grayscale.putpixel((0, 0), 0)      # Black
grayscale.putpixel((1, 0), 85)     # Dark gray
grayscale.putpixel((0, 1), 170)    # Light gray
grayscale.putpixel((1, 1), 255)    # White
grayscale.save(os.path.join(output_dir, "test_grayscale_2x2.png"))
print("Created: test_grayscale_2x2.png - Grayscale 2x2")

# 2. Palette (256 colors) 2x2 PNG
# Create an image with palette
palette_img = Image.new('P', (2, 2))
# Set up a simple palette (256 colors)
palette = []
for i in range(256):
    palette.extend([i, i, i])  # Grayscale palette
palette_img.putpalette(palette)

# Set pixel values as palette indices
palette_img.putpixel((0, 0), 0)    # Index 0 -> RGB(0,0,0) Black
palette_img.putpixel((1, 0), 85)   # Index 85 -> RGB(85,85,85) Dark gray
palette_img.putpixel((0, 1), 170)  # Index 170 -> RGB(170,170,170) Light gray
palette_img.putpixel((1, 1), 255)  # Index 255 -> RGB(255,255,255) White
palette_img.save(os.path.join(output_dir, "test_palette_2x2.png"))
print("Created: test_palette_2x2.png - 256-color palette 2x2")

# 3. TrueColor RGB 2x2 PNG
# Pixels: Red, Green, Blue, White
rgb = Image.new('RGB', (2, 2))
rgb.putpixel((0, 0), (255, 0, 0))    # Red
rgb.putpixel((1, 0), (0, 255, 0))    # Green
rgb.putpixel((0, 1), (0, 0, 255))    # Blue
rgb.putpixel((1, 1), (255, 255, 255)) # White
rgb.save(os.path.join(output_dir, "test_rgb_2x2.png"))
print("Created: test_rgb_2x2.png - RGB TrueColor 2x2")

# 4. TrueColor with Alpha (RGBA) 2x2 PNG
# Pixels with varying transparency
rgba = Image.new('RGBA', (2, 2))
rgba.putpixel((0, 0), (255, 0, 0, 255))   # Red, fully opaque
rgba.putpixel((1, 0), (0, 255, 0, 170))   # Green, semi-transparent
rgba.putpixel((0, 1), (0, 0, 255, 85))    # Blue, more transparent
rgba.putpixel((1, 1), (255, 255, 255, 0)) # White, fully transparent
rgba.save(os.path.join(output_dir, "test_rgba_2x2.png"))
print("Created: test_rgba_2x2.png - RGBA with transparency 2x2")

# 5. Grayscale with Alpha 2x2 PNG
gray_alpha = Image.new('LA', (2, 2))
gray_alpha.putpixel((0, 0), (0, 255))     # Black, opaque
gray_alpha.putpixel((1, 0), (85, 170))    # Dark gray, semi-transparent
gray_alpha.putpixel((0, 1), (170, 85))    # Light gray, more transparent
gray_alpha.putpixel((1, 1), (255, 0))     # White, fully transparent
gray_alpha.save(os.path.join(output_dir, "test_grayscale_alpha_2x2.png"))
print("Created: test_grayscale_alpha_2x2.png - Grayscale with alpha 2x2")

# 6. Palette with transparency (using tRNS chunk)
palette_trans = Image.new('P', (2, 2))
# Create a palette with specific colors
palette_colors = [
    (255, 0, 0),    # Index 0: Red
    (0, 255, 0),    # Index 1: Green
    (0, 0, 255),    # Index 2: Blue
    (255, 255, 255) # Index 3: White
]
# Extend to 256 colors
for i in range(len(palette_colors), 256):
    palette_colors.append((128, 128, 128))

flat_palette = []
for color in palette_colors:
    flat_palette.extend(color)
palette_trans.putpalette(flat_palette)

palette_trans.putpixel((0, 0), 0)  # Red
palette_trans.putpixel((1, 0), 1)  # Green
palette_trans.putpixel((0, 1), 2)  # Blue
palette_trans.putpixel((1, 1), 3)  # White

# Add transparency to palette entries
transparency_values = [255, 170, 85, 0]  # Alpha for indices 0-3
palette_trans.info['transparency'] = bytes(transparency_values)
palette_trans.save(os.path.join(output_dir, "test_palette_trans_2x2.png"))
print("Created: test_palette_trans_2x2.png - Palette with transparency 2x2")

print("\nAll test PNG files created successfully!")
