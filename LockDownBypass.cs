using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;

namespace LockDownBypass
{
    public class Program : ApplicationContext
    {
        private NotifyIcon trayIcon;
        private KeyboardHook keyboardHook;
        private WindowManager windowManager;
        private Configuration config;
        private bool isEnabled = true;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Program());
        }

        public Program()
        {
            // Load configuration
            config = Configuration.Load();

            // Initialize components
            keyboardHook = new KeyboardHook();
            windowManager = new WindowManager();

            // Setup keyboard hook handlers
            keyboardHook.OnHotkeyPressed += HandleHotkey;

            // Register hotkeys
            RegisterHotkeys();

            // Create system tray icon
            InitializeTrayIcon();

            // Install keyboard hook
            keyboardHook.Install();

            Console.WriteLine("LockDown Browser Bypass Tool started");
            Console.WriteLine("Running in background. Check system tray for options.");
        }

        private void RegisterHotkeys()
        {
            // Next tab: Ctrl+Shift+T
            keyboardHook.RegisterHotkey("nextTab", Keys.T, KeyModifiers.Control | KeyModifiers.Shift);
            
            // Previous tab: Ctrl+Shift+W
            keyboardHook.RegisterHotkey("prevTab", Keys.W, KeyModifiers.Control | KeyModifiers.Shift);
            
            // Minimize window: Ctrl+Shift+M
            keyboardHook.RegisterHotkey("minimize", Keys.M, KeyModifiers.Control | KeyModifiers.Shift);
            
            // Switch windows: Ctrl+Shift+Tab
            keyboardHook.RegisterHotkey("switchWindow", Keys.Tab, KeyModifiers.Control | KeyModifiers.Shift);
            
            // Exit tool: Ctrl+Shift+Q
            keyboardHook.RegisterHotkey("exitTool", Keys.Q, KeyModifiers.Control | KeyModifiers.Shift);
        }

        private void HandleHotkey(string hotkeyName)
        {
            if (!isEnabled) return;

            switch (hotkeyName)
            {
                case "nextTab":
                    windowManager.SwitchToNextTab();
                    break;

                case "prevTab":
                    windowManager.SwitchToPreviousTab();
                    break;

                case "minimize":
                    windowManager.MinimizeCurrentWindow();
                    break;

                case "switchWindow":
                    windowManager.SwitchWindow();
                    break;

                case "exitTool":
                    ExitApplication();
                    break;
            }
        }

        private void InitializeTrayIcon()
        {
            trayIcon = new NotifyIcon();
            trayIcon.Icon = SystemIcons.Shield;
            trayIcon.Text = "LockDown Bypass (Active)";
            trayIcon.Visible = true;

            // Create context menu
            var contextMenu = new ContextMenuStrip();
            
            var enabledItem = new ToolStripMenuItem("Enabled", null, (s, e) => ToggleEnabled());
            enabledItem.Checked = isEnabled;
            contextMenu.Items.Add(enabledItem);
            
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add("Configure Hotkeys", null, (s, e) => ShowConfiguration());
            contextMenu.Items.Add("About", null, (s, e) => ShowAbout());
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add("Exit", null, (s, e) => ExitApplication());

            trayIcon.ContextMenuStrip = contextMenu;
            trayIcon.DoubleClick += (s, e) => ShowConfiguration();
        }

        private void ToggleEnabled()
        {
            isEnabled = !isEnabled;
            trayIcon.Text = isEnabled ? "LockDown Bypass (Active)" : "LockDown Bypass (Disabled)";
            
            var menu = trayIcon.ContextMenuStrip.Items[0] as ToolStripMenuItem;
            if (menu != null) menu.Checked = isEnabled;
        }

        private void ShowConfiguration()
        {
            MessageBox.Show(
                "Current Hotkeys:\n\n" +
                "Ctrl+Shift+T - Next Tab\n" +
                "Ctrl+Shift+W - Previous Tab\n" +
                "Ctrl+Shift+M - Minimize Window\n" +
                "Ctrl+Shift+Tab - Switch Windows\n" +
                "Ctrl+Shift+Q - Exit Tool\n\n" +
                "Edit config.json to customize hotkeys.",
                "Hotkey Configuration",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void ShowAbout()
        {
            MessageBox.Show(
                "LockDown Browser Bypass Tool v1.0\n\n" +
                "Enables window switching and tab navigation.\n\n" +
                "Use responsibly and in compliance with\n" +
                "your institution's policies.\n\n" +
                "GitHub: github.com/yourusername/lockdown-browser-bypass",
                "About",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void ExitApplication()
        {
            keyboardHook.Uninstall();
            trayIcon.Visible = false;
            Application.Exit();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                keyboardHook?.Dispose();
                trayIcon?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
