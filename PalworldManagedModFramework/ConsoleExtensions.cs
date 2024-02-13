﻿using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using PalworldManagedModFramework.Sdk.Logging;

namespace PalworldManagedModFramework;

public static class ConsoleExtensions {
    [Conditional("DEBUG")]
    public static void SetWindowAlwaysOnTop(ILogger logger) {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            return;
        }

        logger.Debug("Enabling console always on top...");

        var consoleHandle = Process.GetCurrentProcess().MainWindowHandle;
        var result = SetWindowPos(consoleHandle, HWND_TOPMOST, 0, 0, 0, 0,
            SetWindowsPosFlags.SWP_NOMOVE | SetWindowsPosFlags.SWP_NOSIZE);

        if (!result) {
            logger.Debug("Unable to enable console always on top.");
        }
    }

    [SupportedOSPlatform("windows")]
    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr handle, IntPtr relativeToOtherHandle, int x, int y, int with, int height, SetWindowsPosFlags flags);

#region RelativeHandleSpecialValues

    /// <summary>
    /// Places the window at the bottom of the Z order. If the hWnd parameter identifies a topmost window, the window loses its topmost status and is placed at the bottom of all other windows.
    /// </summary>
    const int HWND_BOTTOM = 1;

    /// <summary>
    /// Places the window above all non-topmost windows (that is, behind all topmost windows). This flag has no effect if the window is already a non-topmost window.
    /// </summary>
    const int HWND_NOTOPMOST = -2;

    /// <summary>
    /// Places the window at the top of the Z order.
    /// </summary>
    const int HWND_TOP = 0;

    /// <summary>
    /// Places the window above all non-topmost windows. The window maintains its topmost position even when it is deactivated.
    /// </summary>
    const int HWND_TOPMOST = -1;

#endregion

    [Flags]
    private enum SetWindowsPosFlags : uint {
        /// <summary>
        /// If the calling thread and the thread that owns the window are attached to different input queues, the system posts the request to the thread that owns the window. This prevents the calling thread from blocking its execution while other threads process the request.
        /// </summary>
        SWP_ASYNCWINDOWPOS = 0x4000,

        /// <summary>
        /// Prevents generation of the WM_SYNCPAINT message.
        /// </summary>
        SWP_DEFERERASE = 0x2000,

        /// <summary>
        /// Draws a frame (defined in the window's class description) around the window.
        /// </summary>
        SWP_DRAWFRAME = 0x0020,

        /// <summary>
        /// Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE is sent only when the window's size is being changed.
        /// </summary>
        SWP_FRAMECHANGED = 0x0020,

        /// <summary>
        /// Hides the window.
        /// </summary>
        SWP_HIDEWINDOW = 0x0080,

        /// <summary>
        /// Does not activate the window. If this flag is not set, the window is activated and moved to the top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter parameter).
        /// </summary>
        SWP_NOACTIVATE = 0x0010,

        /// <summary>
        /// Discards the entire contents of the client area. If this flag is not specified, the valid contents of the client area are saved and copied back into the client area after the window is sized or repositioned.
        /// </summary>
        SWP_NOCOPYBITS = 0x0100,

        /// <summary>
        /// Retains the current position (ignores X and Y parameters).
        /// </summary>
        SWP_NOMOVE = 0x0002,

        /// <summary>
        /// Does not change the owner window's position in the Z order.
        /// </summary>
        SWP_NOOWNERZORDER = 0x0200,

        /// <summary>
        /// Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent window uncovered as a result of the window being moved. When this flag is set, the application must explicitly invalidate or redraw any parts of the window and parent window that need redrawing.
        /// </summary>
        SWP_NOREDRAW = 0x0008,

        /// <summary>
        /// Same as the SWP_NOOWNERZORDER flag.
        /// </summary>
        SWP_NOREPOSITION = 0x0200,

        /// <summary>
        /// Prevents the window from receiving the WM_WINDOWPOSCHANGING message.
        /// </summary>
        SWP_NOSENDCHANGING = 0x0400,

        /// <summary>
        /// Retains the current size (ignores the cx and cy parameters).
        /// </summary>
        SWP_NOSIZE = 0x0001,

        /// <summary>
        /// Retains the current Z order (ignores the hWndInsertAfter parameter).
        /// </summary>
        SWP_NOZORDER = 0x0004,

        /// <summary>
        /// Displays the window.
        /// </summary>
        SWP_SHOWWINDOW = 0x0040
    }
}