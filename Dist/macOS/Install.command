#!/bin/bash
# LockDown Bypass for macOS - Installer
# Double-click this file, enter your password when prompted

INSTALL_DIR="/Applications/LockDownBypass"
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"

echo "============================================"
echo "  LockDown Browser Bypass Tool - macOS"
echo "  Installer"
echo "============================================"
echo ""

# Check if already installed
if [ -d "$INSTALL_DIR" ]; then
    echo "Updating existing installation..."
fi

# Get admin privileges
echo "Administrator access required for installation."
echo "Enter your Mac password when prompted."
echo ""

osascript -e "
do shell script \"
mkdir -p '$INSTALL_DIR' && 
cp '$SCRIPT_DIR/LockDownBypass' '$INSTALL_DIR/' && 
cp '$SCRIPT_DIR/config.json' '$INSTALL_DIR/' && 
cp '$SCRIPT_DIR/build.sh' '$INSTALL_DIR/' &&
cp '$SCRIPT_DIR/README.txt' '$INSTALL_DIR/' &&
chmod +x '$INSTALL_DIR/LockDownBypass' &&
chmod +x '$INSTALL_DIR/build.sh'
\" with administrator privileges
"

if [ $? -ne 0 ]; then
    echo ""
    echo "Installation cancelled or failed."
    echo "Try: sudo cp -R \"$SCRIPT_DIR\" /Applications/LockDownBypass/"
    read -p "Press Enter to exit..."
    exit 1
fi

echo "Done! Installed to: $INSTALL_DIR"
echo ""

# Add to startup/login items
osascript -e "tell application \"System Events\" to make login item at end with properties {path:\"$INSTALL_DIR/LockDownBypass\", hidden:true}" 2>/dev/null
echo "Added to startup (Login Items)."

# Launch
echo ""
read -p "Run LockDown Bypass now? (Y/n): " run_answer
if [ "$run_answer" != "n" ] && [ "$run_answer" != "N" ]; then
    echo ""
    echo "Starting LockDown Bypass..."
    echo "Enter your Mac password again if prompted."
    osascript -e "do shell script \"'$INSTALL_DIR/LockDownBypass'\" with administrator privileges" &
    sleep 2
    echo ""
    echo "LockDown Bypass is running in the background."
fi

echo ""
echo "============================================"
echo "  Done!"
echo "============================================"
echo ""
echo "Hotkeys:"
echo "  Cmd+Shift+T       - Next tab"
echo "  Cmd+Shift+W       - Previous tab"
echo "  Cmd+Shift+Tab     - Switch window"
echo "  Cmd+Shift+Option+C - Screenshot"
echo "  Cmd+Shift+X       - Copy text"
echo "  Cmd+Shift+Q       - Exit"
echo ""
echo "To run manually: sudo /Applications/LockDownBypass/LockDownBypass"
echo ""
read -p "Press Enter to close..."
