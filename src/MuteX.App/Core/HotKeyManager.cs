using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace MuteX.App.Core
{
    public class HotKeyManager : IDisposable
    {
        private const int WM_HOTKEY = 0x0312;
        private readonly HwndSource _source;
        private readonly int _hotKeyId = 9000;

        public event Action? HotKeyPressed;

        public HotKeyManager(IntPtr handle)
        {
            _source = HwndSource.FromHwnd(handle);
            _source.AddHook(HwndHook);
        }

        public bool RegisterHotKey(uint modifier, uint key)
        {
            return RegisterHotKey(_source.Handle, _hotKeyId, modifier, key);
        }

        public void UnregisterHotKey()
        {
            UnregisterHotKey(_source.Handle, _hotKeyId);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY)
            {
                int id = wParam.ToInt32();
                if (id == _hotKeyId)
                {
                    HotKeyPressed?.Invoke();
                    handled = true;
                }
            }
            return IntPtr.Zero;
        }

        public void Dispose()
        {
            UnregisterHotKey();
            _source.RemoveHook(HwndHook);
        }

        // WinAPI
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    }
}