using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace PalworldManagedModFramework.Sdk.Services {
    public static class DebugUtilities {
        public static void WaitForDebuggerAttach(CancellationToken cancellationToken = default,
            [CallerFilePath] string classFile = "",
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string callerName = "") {
            // If the binary was compiled on windows the constant class filenames will have windows path seperators, and vice versa for linux.
            // So we must check for this manually.
            var directorySeparatorChar = classFile.Contains('/') ? '/' : '\\';

            var className = classFile;
            if (className.Contains(directorySeparatorChar)) {
                className = className.Split(directorySeparatorChar)[^1];
            }
            className = Path.GetFileNameWithoutExtension(className);

            Console.WriteLine($"*** Waiting for Debugger Attach at {className}::{callerName};{lineNumber} ***");
            while (!Debugger.IsAttached) {
                if (cancellationToken.IsCancellationRequested) {
                    break;
                }

                Thread.Sleep(100);
            }
        }
    }
}