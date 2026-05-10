using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;

namespace LockDownBypass
{
    [Flags]
    public enum KeyModifiers
    {
        None = 0,
        Alt = 1,
        Control = 2,
        Shift = 4,
        Win = 8
    }

    public class KeyboardHook : IDisposable
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private LowLevelKeyboardProc hookProc;
        private IntPtr hookId = IntPtr.Zero;

        private Dictionary<string, HotkeyDefinition> hotkeys = new Dictionary<string, HotkeyDefinition>();
        
        public event Action<string> OnHotkeyPressed;

        public KeyboardHook()
        {
            hookProc = HookCallback;
        }

        public void Install()
        {
            hookId = SetHook(hookProc);
        }

        public void Uninstall()
        {
            if (hookId != IntPtr.Zero)
            {
                UnhookWindowsHookEx(hookId);
                hookId = IntPtr.Zero;
            }
        }

        public void RegisterHotkey(string name, Keys key, KeyModifiers modifiers)
        {
            hotkeys[name] = new HotkeyDefinition
            {
                Key = key,
                Modifiers = modifiers
            };
        }

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, 
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Keys key = (Keys)vkCode;

                // Get current modifier state
                KeyModifiers currentModifiers = KeyModifiers.None;
                if ((GetKeyState(0x10) & 0x8000) != 0) currentModifiers |= KeyModifiers.Shift;
                if ((GetKeyState(0x11) & 0x8000) != 0) currentModifiers |= KeyModifiers.Control;
                if ((GetKeyState(0x12) & 0x8000) != 0) currentModifiers |= KeyModifiers.Alt;

                // Check if any registered hotkey matches
                foreach (var hotkey in hotkeys)
                {
                    if (hotkey.Value.Key == key && hotkey.Value.Modifiers == currentModifiers)
                    {
                        OnHotkeyPressed?.Invoke(hotkey.Key);
                        return (IntPtr)1; // Block the key from being processed further
                    }
                }
            }

            return CallNextHookEx(hookId, nCode, wParam, lParam);
        }

        public void Dispose()
        {
            Uninstall();
        }

        // Windows API imports
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, 
            IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        private static extern short GetKeyState(int nVirtKey);
    }

    internal class HotkeyDefinition
    {
        public Keys Key { get; set; }
        public KeyModifiers Modifiers { get; set; }
    }
}
