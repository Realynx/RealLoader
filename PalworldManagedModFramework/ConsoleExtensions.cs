using System.Diagnostics;
using System.Runtime.InteropServices;

using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Services.Memory.Windows;

using static PalworldManagedModFramework.Sdk.Services.Memory.Windows.WindowsStructs;

namespace PalworldManagedModFramework {
    public static class ConsoleExtensions {
        [Conditional("DEBUG")]
        public static void SetWindowAlwaysOnTop(ILogger logger) {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                return;
            }

            logger.Debug("Enabling console always on top...");

            var consoleHandle = Process.GetCurrentProcess().MainWindowHandle;
            var result = WindowsNativeMethods.SetWindowPos(consoleHandle, SET_WINDOW_POS_HWND_TOPMOST, 0, 0, 0, 0,
                SetWindowsPosFlags.SWP_NOMOVE | SetWindowsPosFlags.SWP_NOSIZE);

            if (!result) {
                var error = WindowsNativeMethods.GetLastError();
                logger.Debug($"Unable to enable console always on top, error code: 0x{error:X}");
            }
        }
    }
}