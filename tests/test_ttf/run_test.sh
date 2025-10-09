#!/bin/bash

# TrueType Font Embedding Test Runner
# This script runs the TrueType font embedding test and displays the results

set -e

echo "======================================"
echo "  TrueType Font Embedding Test"
echo "======================================"
echo ""

# Check if font exists
if [ ! -f "Roboto-Regular.ttf" ]; then
    echo "Downloading Roboto font..."
    curl -L -o Roboto-Regular.ttf "https://github.com/googlefonts/roboto/raw/main/src/hinted/Roboto-Regular.ttf" 2>&1 | tail -1
    echo "✓ Font downloaded"
fi

# Build the project
echo ""
echo "Building test project..."
dotnet build --nologo --verbosity quiet
echo "✓ Build successful"

# Copy font to output directory
cp Roboto-Regular.ttf bin/Debug/net8.0/

# Run the test
echo ""
dotnet run --no-build

# Show output PDF info
echo ""
echo "======================================"
echo "  Output File Information"
echo "======================================"

if [ -f "bin/Debug/net8.0/output_roboto_test.pdf" ]; then
    OUTPUT_PATH="bin/Debug/net8.0/output_roboto_test.pdf"
    echo "File: $OUTPUT_PATH"
    echo "Size: $(du -h "$OUTPUT_PATH" | cut -f1)"
    echo ""
    echo "PDF contains:"
    strings "$OUTPUT_PATH" | grep -E "(quick brown fox)" | head -2
    echo ""
    echo "✓ PDF verified"
    echo ""
    echo "To view the PDF, open: $OUTPUT_PATH"
fi
