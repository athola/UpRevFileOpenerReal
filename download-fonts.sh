#!/bin/bash
# Script to download OpenSans fonts for the MAUI project

FONTS_DIR="./UpRevFileOpener.Maui/Resources/Fonts"

echo "Downloading OpenSans fonts..."

# OpenSans Regular
curl -L "https://github.com/google/fonts/raw/main/apache/opensans/OpenSans-Regular.ttf" \
  -o "$FONTS_DIR/OpenSans-Regular.ttf"

# OpenSans SemiBold
curl -L "https://github.com/google/fonts/raw/main/apache/opensans/OpenSans-SemiBold.ttf" \
  -o "$FONTS_DIR/OpenSans-Semibold.ttf"

echo "Fonts downloaded successfully!"
echo "Location: $FONTS_DIR"
ls -lh "$FONTS_DIR"/*.ttf 2>/dev/null || echo "Note: Fonts may not have downloaded correctly. Please check manually."
