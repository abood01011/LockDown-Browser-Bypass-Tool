# Build Instructions

## Prerequisites

1. **Install .NET 8.0 SDK**
   - Download from: https://dotnet.microsoft.com/download/dotnet/8.0
   - Verify installation: `dotnet --version`

2. **Install Visual Studio 2022** (Optional but recommended)
   - Community Edition (free): https://visualstudio.microsoft.com/
   - Workload: ".NET desktop development"

## Building from Command Line

### Debug Build
```bash
cd lockdown-browser-bypass
dotnet build
```

The output will be in `bin/Debug/net8.0-windows/`

### Release Build
```bash
dotnet build -c Release
```

The output will be in `bin/Release/net8.0-windows/`

### Single-File Executable
```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

The single-file executable will be in `bin/Release/net8.0-windows/win-x64/publish/`

## Building with Visual Studio

1. Open `LockDownBypass.csproj` in Visual Studio 2022
2. Select **Release** configuration from the toolbar
3. Right-click the project → **Publish**
4. Choose **Folder** as publish target
5. Click **Publish**

## Testing the Build

1. Navigate to the output directory
2. Right-click `LockDownBypass.exe`
3. Select **Run as Administrator**
4. Look for the system tray icon (shield icon)
5. Test hotkeys:
   - Press `Ctrl+Shift+T` to verify next tab functionality
   - Press `Ctrl+Shift+M` to minimize the current window

## Troubleshooting

**Error: SDK not found**
- Ensure .NET 8.0 SDK is installed
- Restart your terminal/IDE after installation

**Error: Missing Windows Forms reference**
- The project targets `net8.0-windows` specifically
- Ensure you're building on Windows

**Warning: Missing icon.ico**
- This is optional; the build will succeed without it
- Create or download an icon file and place it in the project root

**Antivirus blocking the executable**
- This is common with keyboard hook applications
- Add an exception in Windows Defender for the output folder
- Sign the executable with a code signing certificate for production use

## Distribution

For distributing to end users:

1. Build the single-file executable (see above)
2. Include `config.json` in the same directory
3. Create a ZIP file with:
   - `LockDownBypass.exe`
   - `config.json`
   - `README.md`
   - `LICENSE`

## Code Signing (Optional but Recommended)

To avoid Windows SmartScreen warnings:

```bash
signtool sign /f certificate.pfx /p password /t http://timestamp.digicert.com LockDownBypass.exe
```

You'll need to obtain a code signing certificate from a trusted CA.
