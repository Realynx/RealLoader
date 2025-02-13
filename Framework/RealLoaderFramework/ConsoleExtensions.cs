using System.Diagnostics;
using System.Runtime.InteropServices;

using RealLoaderFramework.Sdk.Logging;

using static RealLoaderFramework.Services.NativeFunctions;

namespace RealLoaderFramework {
    public static class ConsoleExtensions {
        [Conditional("DEBUG")]
        public static void SetWindowAlwaysOnTop(ILogger logger) {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                return;
            }

            logger.Debug("Enabling console always on top...");

            var consoleHandle = Process.GetCurrentProcess().MainWindowHandle;
            const SetWindowsPosFlags FLAGS = SetWindowsPosFlags.SWP_NOMOVE | SetWindowsPosFlags.SWP_NOSIZE;
            var success = SetWindowPos(consoleHandle, SET_WINDOW_POS_HWND_TOPMOST, 0, 0, 0, 0, FLAGS);

            if (!success) {
                var error = Marshal.GetLastPInvokeError();
                logger.Debug($"Unable to enable console always on top, error code: 0x{error:X}");
            }
        }
    }
}