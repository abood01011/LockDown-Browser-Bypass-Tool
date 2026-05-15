using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading;

namespace LockDownBypass
{
    // ===== Main Entry Point =====
    class Program
    {
        private static Logger? _logger;
        private static Configuration? _config;
        private static bool _exiting;

        static void Main(string[] args)
        {
            Console.WriteLine("LockDown Bypass for macOS v2.0");
            Console.WriteLine("=================================");

            // Check for admin/root (required for process patching)
            if (!IsRoot())
            {
                Console.WriteLine("WARNING: Not running as root. Process patching will not work.");
                Console.WriteLine("Please run: sudo ./LockDownBypass");
                Console.WriteLine("Continuing in limited mode...");
            }
            else
            {
                Console.WriteLine("Running as root - full functionality available.");
            }

            _logger = new Logger("LockDownBypass");
            _logger.Info("=== LockDown Bypass for macOS v2.0 ===");

            _config = Configuration.Load();
            Thread.Sleep(500);

            // Start hotkey polling thread
            var poller = new Thread(HotkeyPollLoop)
            {
                IsBackground = true,
                Name = "HotkeyPoller"
            };
            poller.Start();

            Console.WriteLine("Hotkey polling started. Press Ctrl+C to exit.");
            Console.WriteLine("");
            Console.WriteLine("Hotkeys:");
            Console.WriteLine("  Ctrl+Shift+T     - Next LockDown tab");
            Console.WriteLine("  Ctrl+Shift+W     - Previous LockDown tab");
            Console.WriteLine("  Ctrl+Shift+Tab   - Switch window");
            Console.WriteLine("  Ctrl+Shift+Alt+C - Screenshot capture");
            Console.WriteLine("  Ctrl+Shift+X     - Copy selected text");
            Console.WriteLine("  Ctrl+Shift+Q     - Exit tool");
            Console.WriteLine("");

            // Keep running until Ctrl+C
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                _exiting = true;
                _logger?.Info("Shutting down...");
                _logger?.Dispose();
                Environment.Exit(0);
            };

            while (!_exiting)
                Thread.Sleep(1000);
        }

        private static bool IsRoot()
        {
            try
            {
                var psi = new ProcessStartInfo("id", "-u")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                };
                var proc = Process.Start(psi);
                if (proc == null) return false;
                string output = proc.StandardOutput.ReadToEnd().Trim();
                return output == "0";
            }
            catch { return false; }
        }

        // ===== Hotkey Polling (macOS equivalent of GetAsyncKeyState) =====
        private static void HotkeyPollLoop()
        {
            var hotkeys = new Dictionary<string, (ulong keyCode, ulong modifiers)>
            {
                ["nextTab"]     = (0x11, 0x08),  // VK_TAB, Cmd+Shift  (Note: Mac uses Cmd not Ctrl)
                ["prevTab"]     = (0x0D, 0x08),  // VK_W, Cmd+Shift
                ["switchWindow"] = (0x30, 0x08),  // VK_Tab, Cmd+Shift
                ["capture"]     = (0x08, 0x08 | 0x100000),  // VK_C, Cmd+Shift+Option
                ["copy"]        = (0x07, 0x08),   // VK_X, Cmd+Shift
                ["exitTool"]    = (0x0C, 0x08),   // VK_Q, Cmd+Shift
                // Windows uses Ctrl, but on Mac Chrome/LockDown uses Cmd
                // We'll map Ctrl+... to Cmd+... on Mac
            };

            var prevState = new Dictionary<string, bool>();
            foreach (var k in hotkeys.Keys)
                prevState[k] = false;

            while (!_exiting)
            {
                foreach (var kvp in hotkeys)
                {
                    bool pressed = IsKeyPressed(kvp.Value.keyCode, kvp.Value.modifiers);
                    if (pressed && !prevState[kvp.Key])
                    {
                        prevState[kvp.Key] = true;
                        HandleHotkey(kvp.Key);
                    }
                    else if (!pressed)
                    {
                        prevState[kvp.Key] = false;
                    }
                }
                Thread.Sleep(50);
            }
        }

        private static bool IsKeyPressed(ulong keyCode, ulong requiredModifiers)
        {
            try
            {
                // CoreGraphics key state check
                // Check if key is pressed
                bool keyDown = CGEventSourceKeyState(0, keyCode);

                // Check modifiers - CGEventSourceFlagsState returns current modifier flags
                ulong modFlags = OSXAPI.CGEventSourceFlagsState(0);
                ulong pressedMods = 0;

                // Map CGEvent modifier flags:
                // kCGEventFlagMaskCommand   = 0x100000
                // kCGEventFlagMaskShift     = 0x20000
                // kCGEventFlagMaskAlternate = 0x80000
                // kCGEventFlagMaskControl   = 0x40000
                if ((modFlags & 0x100000) != 0) pressedMods |= 0x08;   // Cmd
                if ((modFlags & 0x20000) != 0) pressedMods |= 0x01;   // Shift
                if ((modFlags & 0x80000) != 0) pressedMods |= 0x100000; // Option
                if ((modFlags & 0x40000) != 0) pressedMods |= 0x04;   // Ctrl

                return keyDown && (pressedMods & requiredModifiers) == requiredModifiers;
            }
            catch { return false; }
        }

        [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
        private static extern bool CGEventSourceKeyState(int source, ulong keyCode);

        // ===== Hotkey Actions =====
        private static void HandleHotkey(string name)
        {
            _logger?.Info($"Hotkey: {name}");
            try
            {
                switch (name)
                {
                    case "nextTab":      MacWindowManager.SwitchToNextTab();      break;
                    case "prevTab":      MacWindowManager.SwitchToPreviousTab();  break;
                    case "switchWindow": MacWindowManager.SwitchWindow();         break;
                    case "capture":      MacWindowManager.CaptureScreenshot();    break;
                    case "copy":         MacWindowManager.CopySelectedText();     break;
                    case "exitTool":     _exiting = true;                         break;
                }
            }
            catch (Exception ex)
            {
                _logger?.Error($"Hotkey error", ex);
            }
        }
    }

    // ===== Configuration (same as Windows version) =====
    public class HotkeyConfig
    {
        public string? NextTab { get; set; }
        public string? PrevTab { get; set; }
        public string? Minimize { get; set; }
        public string? SwitchWindow { get; set; }
        public string? Capture { get; set; }
        public string? Copy { get; set; }
        public string? ExitTool { get; set; }
    }

    public class Configuration
    {
        public HotkeyConfig Hotkeys { get; set; } = new();
        public bool StealthMode { get; set; } = true;
        public bool AutoStart { get; set; } = false;

        public static Configuration Load()
        {
            string configPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, "config.json");
            if (!File.Exists(configPath))
                return CreateDefault();

            try
            {
                var json = File.ReadAllText(configPath);
                return JsonSerializer.Deserialize<Configuration>(json) ?? CreateDefault();
            }
            catch { return CreateDefault(); }
        }

        private static string GetConfigPath() =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

        private static Configuration CreateDefault()
        {
            return new Configuration
            {
                Hotkeys = new HotkeyConfig
                {
                    NextTab = "Cmd+Shift+T",
                    PrevTab = "Cmd+Shift+W",
                    Minimize = "Cmd+Shift+M",
                    SwitchWindow = "Cmd+Shift+Tab",
                    Capture = "Cmd+Shift+Option+C",
                    Copy = "Cmd+Shift+X",
                    ExitTool = "Cmd+Shift+Q"
                }
            };
        }
    }

    // ===== Logger (same as Windows version) =====
    public class Logger : IDisposable
    {
        private readonly string _logPath;
        private readonly object _lock = new();

        public Logger(string appName)
        {
            string dir = Environment.GetEnvironmentVariable("HOME") ?? "/tmp";
            dir = Path.Combine(dir, "Library", "Logs", appName);
            Directory.CreateDirectory(dir);
            _logPath = Path.Combine(dir, "log.txt");
        }

        public void Info(string msg) => Write("INFO", msg);
        public void Warn(string msg) => Write("WARN", msg);
        public void Error(string msg, Exception? ex = null) =>
            Write("ERROR", msg + (ex != null ? " | " + ex : ""));

        private void Write(string level, string msg)
        {
            lock (_lock)
            {
                try
                {
                    File.AppendAllText(_logPath,
                        $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {msg}{Environment.NewLine}");
                }
                catch { }
            }
        }

        public void Dispose() { }
    }

    // ===== Mac-Specific API Declarations =====
    public static class OSXAPI
    {
        [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
        public static extern ulong CGEventSourceFlagsState(int source);

        [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
        public static extern int CGDisplayPixelsWide(int displayId);

        [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
        public static extern int CGDisplayPixelsHigh(int displayId);

        [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
        public static extern int CGMainDisplayID();

        // For memory patching (requires root)
        [DllImport("/usr/lib/system/libsystem_kernel.dylib")]
        public static extern int task_for_pid(int targetTask, int pid, out int task);

        [DllImport("/usr/lib/system/libsystem_kernel.dylib")]
        public static extern int mach_vm_protect(int task, IntPtr address,
            IntPtr size, int maxProt, int newProt);

        [DllImport("/usr/lib/system/libsystem_kernel.dylib")]
        public static extern int mach_vm_write(int task, IntPtr address,
            byte[] data, IntPtr size);

        [DllImport("/usr/lib/system/libsystem_kernel.dylib")]
        public static extern int mach_task_self();

        public const int VM_PROT_READ = 1;
        public const int VM_PROT_WRITE = 2;
        public const int VM_PROT_EXECUTE = 4;
        public const int VM_PROT_DEFAULT = VM_PROT_READ | VM_PROT_WRITE;
        public const int VM_PROT_ALL = VM_PROT_READ | VM_PROT_WRITE | VM_PROT_EXECUTE;
    }

    // ===== Mac Window Manager =====
    public static class MacWindowManager
    {
        private static readonly Logger _logger = new("LockDownBypass");

        public static void SwitchToNextTab()
        {
            string[] apps = FindLockDownApps();
            foreach (var app in apps)
                RunAppleScript($"tell application \"{app}\" to activate\n" +
                    $"tell application \"System Events\" to keystroke tab using command down");
            _logger.Info("nextTab");
        }

        public static void SwitchToPreviousTab()
        {
            string[] apps = FindLockDownApps();
            foreach (var app in apps)
                RunAppleScript($"tell application \"{app}\" to activate\n" +
                    $"tell application \"System Events\" to keystroke tab using {{command down, shift down}}");
            _logger.Info("prevTab");
        }

        public static void SwitchWindow()
        {
            // Switch to next non-LockDown app
            RunAppleScript(
                "tell application \"System Events\"\n" +
                "  set frontApp to name of first application process whose frontmost is true\n" +
                "  if frontApp contains \"LockDown\" then\n" +
                "    -- Activate Finder to force switch away\n" +
                "    tell application \"Finder\" to activate\n" +
                "  end if\n" +
                "end tell");
            _logger.Info("switchWindow");
        }

        public static void CaptureScreenshot()
        {
            try
            {
                string desktopDir = Environment.GetFolderPath(
                    Environment.SpecialFolder.Desktop);
                string path = Path.Combine(desktopDir,
                    $"LockDown_Capture_{DateTime.Now:yyyyMMdd_HHmmss}.png");

                // Use screencapture CLI to capture the LockDown window
                string[] apps = FindLockDownApps();
                if (apps.Length > 0)
                {
                    // Try to capture specific LockDown window
                    RunAppleScript(
                        $"tell application \"{apps[0]}\" to activate\n" +
                        "delay 0.3\n" +
                        "tell application \"System Events\" to tell process \"" +
                        apps[0] + "\" to get position of window 1");

                    // Use screencapture with mouse click option to capture selected area
                    // Or use full screen capture (limited on Mac without SIP bypass)
                    Process.Start("screencapture", $"-T0 -x \"{path}\"");
                    Thread.Sleep(500);

                    if (File.Exists(path) && new FileInfo(path).Length > 0)
                        _logger.Info($"Screenshot saved: {path}");
                }
                else
                {
                    // Full screen capture
                    Process.Start("screencapture", $"-T0 -x \"{path}\"");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("CaptureScreenshot failed", ex);
            }
        }

        public static void CopySelectedText()
        {
            // Send Cmd+C via AppleScript keystroke
            string[] apps = FindLockDownApps();
            foreach (var app in apps)
            {
                RunAppleScript(
                    $"tell application \"{app}\" to activate\n" +
                    "tell application \"System Events\" to keystroke \"c\" using command down");
            }

            // Read clipboard after copy
            try
            {
                var psi = new ProcessStartInfo("pbpaste")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                };
                var proc = Process.Start(psi);
                if (proc != null)
                {
                    string text = proc.StandardOutput.ReadToEnd();
                    if (!string.IsNullOrEmpty(text))
                        _logger.Info("Text copied successfully");
                }
            }
            catch { }
        }

        // ===== AppleScript Helper =====
        private static void RunAppleScript(string script)
        {
            try
            {
                // Write script to temp file and execute
                string tmpPath = Path.GetTempFileName() + ".scpt";
                File.WriteAllText(tmpPath, script);

                var psi = new ProcessStartInfo("osascript", $"\"{tmpPath}\"")
                {
                    RedirectStandardError = true,
                    UseShellExecute = false
                };
                var proc = Process.Start(psi);
                proc?.WaitForExit(5000);

                try { File.Delete(tmpPath); }
                catch { }
            }
            catch (Exception ex)
            {
                _logger.Error($"AppleScript failed: {ex.Message}");
            }
        }

        private static string[] FindLockDownApps()
        {
            var apps = new List<string>();
            try
            {
                // Find LockDown Browser by checking running apps
                string script =
                    "tell application \"System Events\"\n" +
                    "  set lockApps to {}\n" +
                    "  set allProcs to name of every process whose background only is false\n" +
                    "  repeat with p in allProcs\n" +
                    "    if p contains \"LockDown\" then\n" +
                    "      set end of lockApps to p\n" +
                    "    end if\n" +
                    "  end repeat\n" +
                    "  return lockApps as string\n" +
                    "end tell";

                string tmpPath = Path.GetTempFileName() + ".scpt";
                File.WriteAllText(tmpPath, script);

                var psi = new ProcessStartInfo("osascript", $"\"{tmpPath}\"")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                };
                var proc = Process.Start(psi);
                if (proc != null)
                {
                    string output = proc.StandardOutput.ReadToEnd().Trim();
                    if (!string.IsNullOrEmpty(output) && output != "{}")
                    {
                        // Parse comma-separated app names
                        foreach (var name in output.Split(','))
                        {
                            string cleanName = name.Trim();
                            if (!string.IsNullOrEmpty(cleanName))
                                apps.Add(cleanName);
                        }
                    }
                }

                try { File.Delete(tmpPath); }
                catch { }

                // Fallback: common LockDown Browser names
                if (apps.Count == 0)
                {
                    foreach (var name in new[] { "LockDown Browser", "LockDownBrowser" })
                        apps.Add(name);
                }
            }
            catch { }
            return apps.ToArray();
        }

        // ===== Process Patcher (requires root) =====
        public static bool PatchLockDown(int pid)
        {
            _logger.Info($"Attempting to patch LockDown Browser PID {pid}");
            try
            {
                // Get task port for the LockDown Browser process
                int selfTask = OSXAPI.mach_task_self();
                int targetTask;
                int kr = OSXAPI.task_for_pid(selfTask, pid, out targetTask);
                if (kr != 0)
                {
                    _logger.Warn($"task_for_pid failed: {kr}");
                    return false;
                }

                // On macOS, LockDown Browser uses NSApplication.sharedApplication
                // to check if it's active. We need to find the objc_msgSend
                // address in the LockDown process and patch it.

                // The actual patching would involve:
                // 1. Find the LockDown process's dyld shared cache
                // 2. Find the NSApp method implementations
                // 3. Patch them to always return the LockDown app

                // This is complex and version-dependent.
                // For now, log that root access was obtained but patching
                // requires version-specific analysis.

                _logger.Info($"Got task port: {targetTask}");
                _logger.Warn("Process patching on macOS requires version-specific analysis.");
                _logger.Warn("Window switching via AppleScript should still work.");

                return false; // Placeholder - actual patching needs dyld analysis
            }
            catch (Exception ex)
            {
                _logger.Error("PatchLockDown failed", ex);
                return false;
            }
        }
    }
}
