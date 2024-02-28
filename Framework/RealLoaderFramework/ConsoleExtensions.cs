using System.Diagnostics;
using System.Runtime.InteropServices;

using RealLoaderFramework.Sdk.Logging;
using RealLoaderFramework.Sdk.Services.Memory.Windows;

namespace RealLoaderFramework {
    public static class ConsoleExtensions {
        [Conditional("DEBUG")]
        public static void SetWindowAlwaysOnTop(ILogger logger) {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                return;
            }

            logger.Debug("Enabling console always on top...");

            var consoleHandle = Process.GetCurrentProcess().MainWindowHandle;
            var result = NativeFunctions.SetWindowPos(consoleHandle, NativeFunctions.SET_WINDOW_POS_HWND_TOPMOST, 0, 0, 0, 0,
                NativeFunctions.SetWindowsPosFlags.SWP_NOMOVE | NativeFunctions.SetWindowsPosFlags.SWP_NOSIZE);

            if (!result) {
                var error = NativeFunctions.GetLastError();
                logger.Debug($"Unable to enable console always on top, error code: 0x{error:X}");
            }
        }
    }
}