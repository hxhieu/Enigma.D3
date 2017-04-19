using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace Mitmeo.D3.App.Core
{
    public static class Win32Interop
    {
        private const string User32 = "user32.dll";

        [DllImport(User32)]
        internal static extern bool GetClientRect(IntPtr windowHandle, out Int32Rect clientRect);

        [DllImport(User32)]
        internal static extern bool ClientToScreen(IntPtr windowHandle, ref Int32Rect point);

        [DllImport(User32)]
        internal static extern int GetWindowLong(IntPtr windowHandle, int index);

        [DllImport(User32)]
        internal static extern int SetWindowLong(IntPtr windowHandle, int index, int newStyle);

        [DllImport(User32)]
        public static extern IntPtr GetForegroundWindow();
    }
}
