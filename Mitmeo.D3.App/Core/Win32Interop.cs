using System;
using System.Runtime.InteropServices;

namespace Mitmeo.D3.App.Core
{
    public static class Win32Interop
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
    }
}
