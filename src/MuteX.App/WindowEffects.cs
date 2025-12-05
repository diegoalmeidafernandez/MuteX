using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace MuteX.App
{
    public static class WindowEffects
    {
        private const int DWMWA_SYSTEMBACKDROP_TYPE = 38;
        private const int DWMWA_MICA_EFFECT = 1029;

        private enum BackdropType
        {
            Auto = 0,
            None = 1,
            Mica = 2,
            Acrylic = 3,
            Tabbed = 4
        }

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(
            IntPtr hwnd,
            int attribute,
            ref int value,
            int size);

        public static void ApplyMica(Window window)
        {
            var windowHandle = new WindowInteropHelper(window).Handle;

            int micaValue = (int)BackdropType.Mica;

            DwmSetWindowAttribute(
                windowHandle,
                DWMWA_SYSTEMBACKDROP_TYPE,
                ref micaValue,
                sizeof(int));

            int trueValue = 1;
            DwmSetWindowAttribute(
                windowHandle,
                1029,
                ref trueValue,
                sizeof(int));
        }
    }
}
