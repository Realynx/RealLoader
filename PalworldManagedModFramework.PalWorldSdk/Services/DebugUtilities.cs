using System.Diagnostics;

namespace PalworldManagedModFramework.PalWorldSdk.Services {
    public static class DebugUtilities {
        public static void WaitForDebuggerAttach(CancellationToken cancellationToken = default) {
            Console.WriteLine("*** Waiting for Debugger Attach ***");
            while (!Debugger.IsAttached) {
                if (cancellationToken.IsCancellationRequested) {
                    break;
                }

                Thread.Sleep(100);
            }
        }
    }
}
