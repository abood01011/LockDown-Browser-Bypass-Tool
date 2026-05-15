#!/bin/bash
# LockDown Bypass for macOS - Build Script
# 
# Prerequisites:
#   1. Install .NET 8 SDK for macOS:
#      brew install --cask dotnet-sdk
#      OR download from: https://dotnet.microsoft.com/download/dotnet/8.0
#
#   2. Run this script:
#      chmod +x build.sh
#      ./build.sh

echo "============================================"
echo "  LockDown Bypass for macOS"
echo "  Build Script"
echo "============================================"
echo ""

# Check for .NET SDK
if ! command -v dotnet &> /dev/null; then
    echo "ERROR: .NET SDK not found!"
    echo "Install it from: https://dotnet.microsoft.com/download/dotnet/8.0"
    echo "Or using Homebrew: brew install --cask dotnet-sdk"
    exit 1
fi

echo "Building LockDown Bypass for macOS..."
echo ""

# Restore and build
dotnet restore
if [ $? -ne 0 ]; then
    echo "ERROR: dotnet restore failed"
    exit 1
fi

# Publish as self-contained app
dotnet publish -c Release -r osx-x64 --self-contained true -p:PublishSingleFile=true

if [ $? -ne 0 ]; then
    echo "ERROR: Build failed"
    exit 1
fi

echo ""
echo "============================================"
echo "  Build Successful!"
echo "============================================"
echo ""
echo "The executable is at:"
echo "  bin/Release/net8.0/osx-x64/publish/LockDownBypass"
echo ""
echo "To run:"
echo "  cd bin/Release/net8.0/osx-x64/publish/"
echo "  sudo ./LockDownBypass"
echo ""
echo "NOTE: sudo is required for process patching."
echo "Without sudo, window switching via AppleScript"
echo "will still work."
echo ""

# Copy config.json if available
if [ -f "../../config.json" ]; then
    cp "../../config.json" "bin/Release/net8.0/osx-x64/publish/"
    echo "config.json copied to output directory."
elif [ -f "../config.json" ]; then
    cp "../config.json" "bin/Release/net8.0/osx-x64/publish/"
    echo "config.json copied to output directory."
fi

echo ""
echo "To create a distributable package:"
echo "  zip -r LockDownBypass-Mac.zip bin/Release/net8.0/osx-x64/publish/"
echo ""
