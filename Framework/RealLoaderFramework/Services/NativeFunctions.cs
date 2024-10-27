using System.Runtime.InteropServices;

namespace RealLoaderFramework.Services {
    internal static partial class NativeFunctions {

        [LibraryImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool SetWindowPos(nint handle, nint handleRelativeTo, int x, int y, int with, int height,
            SetWindowsPosFlags flags);

        /// <summary> Places the window at the bottom of the Z order. If the hWnd parameter identifies a topmost window, the window loses its topmost status and is placed at the bottom of all other windows. </summary>
        public const int SET_WINDOW_POS_HWND_BOTTOM = 1;

        /// <summary> Places the window above all non-topmost windows (that is, behind all topmost windows). This flag has no effect if the window is already a non-topmost window. </summary>
        public const int SET_WINDOW_POS_HWND_NOTOPMOST = -2;

        /// <summary> Places the window at the top of the Z order. </summary>
        public const int SET_WINDOW_POS_HWND_TOP = 0;

        /// <summary> Places the window above all non-topmost windows. The window maintains its topmost position even when it is deactivated. </summary>
        public const int SET_WINDOW_POS_HWND_TOPMOST = -1;

        [Flags]
        public enum SetWindowsPosFlags : uint {
            SWP_ASYNCWINDOWPOS = 0x4000,
            SWP_DEFERERASE = 0x2000,
            SWP_DRAWFRAME = 0x0020,
            SWP_FRAMECHANGED = 0x0020,
            SWP_HIDEWINDOW = 0x0080,
            SWP_NOACTIVATE = 0x0010,
            SWP_NOCOPYBITS = 0x0100,
            SWP_NOMOVE = 0x0002,
            SWP_NOOWNERZORDER = 0x0200,
            SWP_NOREDRAW = 0x0008,
            SWP_NOREPOSITION = 0x0200,
            SWP_NOSENDCHANGING = 0x0400,
            SWP_NOSIZE = 0x0001,
            SWP_NOZORDER = 0x0004,
            SWP_SHOWWINDOW = 0x0040
        }
    }
}