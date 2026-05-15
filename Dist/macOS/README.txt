============================================
  LockDown Browser Bypass - macOS
  Version 2.0
============================================

REQUIREMENTS
  - macOS 11 (Big Sur) or later
  - .NET 8 SDK (for building)
  - Administrator (root) access for process patching
  - LockDown Browser for Mac

HOW TO BUILD
  1. Install .NET 8 SDK:
     brew install --cask dotnet-sdk
     OR download from: https://dotnet.microsoft.com/download/dotnet/8.0

  2. Open Terminal in this folder
  3. Run: chmod +x build.sh && ./build.sh
  4. The executable will be in:
     bin/Release/net8.0/osx-x64/publish/LockDownBypass

HOW TO USE
  1. Open Terminal
  2. Navigate to the publish folder
  3. Run: sudo ./LockDownBypass
     (sudo is required for full functionality)

  4. Open LockDown Browser
  5. Hotkeys (macOS uses Cmd instead of Ctrl):

  Cmd+Shift+T       - Next tab
  Cmd+Shift+W       - Previous tab
  Cmd+Shift+Tab     - Switch to another window
  Cmd+Shift+M       - Minimize current window
  Cmd+Shift+Option+C - Take screenshot
  Cmd+Shift+X       - Copy selected text
  Cmd+Shift+Q       - Exit the tool

NOTE: Hotkey configuration can be changed in config.json

MAC-SPECIFIC NOTES
  - The process patching (blocking LockDown's focus detection)
    is more complex on macOS. The tool may need version-specific
    updates for full LockDown bypass.
  - Window switching uses AppleScript, which works reliably.
  - Screenshots use the screencapture command-line tool.
  - Copy uses Cmd+C via AppleScript System Events automation.
  - Apple Events must be allowed in:
    System Preferences > Security & Privacy > Privacy > Automation

KNOWN LIMITATIONS
  - Full process patching (like Windows WriteProcessMemory)
    requires macOS dyld shared cache analysis which varies by
    LockDown Browser version.
  - Screenshot capture may return black/grey when LockDown
    uses macOS display protection (CGSSessionCopyAllWindows).
  - For the best experience, use a second device/camera to
    capture LockDown content on macOS.

============================================
