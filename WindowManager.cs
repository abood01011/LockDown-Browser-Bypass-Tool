using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

namespace LockDownBypass
{
    public class WindowManager
    {
        private const int SW_MINIMIZE = 6;
        private const int SW_RESTORE = 9;

        public void SwitchToNextTab()
        {
            // Simulate Ctrl+Tab
            SimulateKeyPress(Keys.Tab, KeyModifiers.Control);
        }

        public void SwitchToPreviousTab()
        {
            // Simulate Ctrl+Shift+Tab
            SimulateKeyPress(Keys.Tab, KeyModifiers.Control | KeyModifiers.Shift);
        }

        public void MinimizeCurrentWindow()
        {
            IntPtr hwnd = GetForegroundWindow();
            if (hwnd != IntPtr.Zero)
            {
                ShowWindow(hwnd, SW_MINIMIZE);
            }
        }

        public void SwitchWindow()
        {
            // Simulate Alt+Tab
            SimulateKeyPress(Keys.Tab, KeyModifiers.Alt);
        }

        private void SimulateKeyPress(Keys key, KeyModifiers modifiers)
        {
            var inputs = new System.Collections.Generic.List<INPUT>();

            // Press modifiers
            if ((modifiers & KeyModifiers.Control) != 0)
                inputs.Add(CreateKeyInput(0x11, false)); // VK_CONTROL

            if ((modifiers & KeyModifiers.Shift) != 0)
                inputs.Add(CreateKeyInput(0x10, false)); // VK_SHIFT

            if ((modifiers & KeyModifiers.Alt) != 0)
                inputs.Add(CreateKeyInput(0x12, false)); // VK_MENU

            // Press and release the key
            inputs.Add(CreateKeyInput((byte)key, false));
            inputs.Add(CreateKeyInput((byte)key, true));

            // Release modifiers in reverse order
            if ((modifiers & KeyModifiers.Alt) != 0)
                inputs.Add(CreateKeyInput(0x12, true));

            if ((modifiers & KeyModifiers.Shift) != 0)
                inputs.Add(CreateKeyInput(0x10, true));

            if ((modifiers & KeyModifiers.Control) != 0)
                inputs.Add(CreateKeyInput(0x11, true));

            // Send all inputs
            SendInput((uint)inputs.Count, inputs.ToArray(), Marshal.SizeOf(typeof(INPUT)));
        }

        private INPUT CreateKeyInput(byte vk, bool keyUp)
        {
            return new INPUT
            {
                type = 1, // INPUT_KEYBOARD
                U = new InputUnion
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = vk,
                        wScan = 0,
                        dwFlags = (uint)(keyUp ? 2 : 0), // KEYEVENTF_KEYUP = 2
                        time = 0,
                        dwExtraInfo = IntPtr.Zero
                    }
                }
            };
        }

        public bool IsLockDownBrowserRunning()
        {
            Process[] processes = Process.GetProcessesByName("LockDownBrowser");
            return processes.Length > 0;
        }

        // Windows API structures and imports
        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT
        {
            public int type;
            public InputUnion U;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct InputUnion
        {
            [FieldOffset(0)]
            public MOUSEINPUT mi;
            [FieldOffset(0)]
            public KEYBDINPUT ki;
            [FieldOffset(0)]
            public HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBDINPUT
        {
            public byte wVk;
            public byte wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
    }
}
