using System.Diagnostics;

namespace PalworldManagedModFramework.PalWorldSdk.Services {
    public static class DebugUtilities {
        public static void WaitForDebuggerAttach(CancellationToken cancellationToken = default) {
            while (!Debugger.IsAttached) {
                if (cancellationToken.IsCancellationRequested) {
                    break;
                }

                Thread.Sleep(100);
            }
        }
    }
}
