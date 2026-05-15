#!/bin/bash
# LockDown Bypass - macOS Installer
# Double-click this file to install

INSTALL_DIR="/Applications/LockDownBypass"
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"

# Get sudo (required)
if [ "$EUID" -ne 0 ]; then
    osascript -e "do shell script \"chmod +x '$SCRIPT_DIR/LockDownBypass' && mkdir -p '$INSTALL_DIR' && cp -R '$SCRIPT_DIR/'* '$INSTALL_DIR/' && chmod +x '$INSTALL_DIR/LockDownBypass'\" with administrator privileges"
    if [ $? -ne 0 ]; then
        echo "Installation cancelled or failed."
        read -p "Press Enter to exit..."
        exit 1
    fi
fi

echo "============================================"
echo "  LockDown Browser Bypass Tool - macOS"
echo "  Installation Complete!"
echo "============================================"
echo ""
echo "Installed to: $INSTALL_DIR"
echo ""

# Add to Login Items
osascript -e "tell application \"System Events\" to make login item at end with properties {path:\"$INSTALL_DIR/LockDownBypass\", hidden:false}" 2>/dev/null
echo "Added to Startup (Login Items)"

# Ask to run
read -p "Run LockDown Bypass now? (Y/n): " answer
if [ "$answer" != "n" ] && [ "$answer" != "N" ]; then
    echo ""
    echo "Starting LockDown Bypass (requires sudo password)..."
    osascript -e "do shell script \"'$INSTALL_DIR/LockDownBypass'\"" with administrator privileges &
    echo ""
    echo "LockDown Bypass is running in the background."
    echo "Open LockDown Browser and use the hotkeys."
fi

echo ""
echo "Hotkeys:"
echo "  Cmd+Shift+T       - Next tab"
echo "  Cmd+Shift+W       - Previous tab"
echo "  Cmd+Shift+Tab     - Switch window"
echo "  Cmd+Shift+Option+C - Screenshot"
echo "  Cmd+Shift+X       - Copy text"
echo "  Cmd+Shift+Q       - Exit"
echo ""
read -p "Press Enter to close..."
