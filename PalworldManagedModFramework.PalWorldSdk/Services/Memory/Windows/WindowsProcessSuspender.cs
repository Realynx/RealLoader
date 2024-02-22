using System.Diagnostics;
using System.Runtime.Versioning;

using PalworldManagedModFramework.Sdk.Services.Memory.Interfaces;

namespace PalworldManagedModFramework.Sdk.Services.Memory.Windows {
    [SupportedOSPlatform("windows")]
    public class WindowsProcessSuspender : IProcessSuspender {
        public void PauseSelf() {
            var currentProcess = Process.GetCurrentProcess();
            var currentThread = NativeFunctions.GetCurrentThreadId();

            foreach (ProcessThread thread in currentProcess.Threads) {
                if (thread.Id == currentThread) {
                    continue;
                }

                var pOpenThread = NativeFunctions.OpenThread(NativeFunctions.ThreadAccess.SUSPEND_RESUME, false, (uint)thread.Id);
                if (pOpenThread == IntPtr.Zero) {
                    continue;
                }

                NativeFunctions.SuspendThread(pOpenThread);
                NativeFunctions.CloseHandle(pOpenThread);
            }
        }

        public void ResumeSelf() {
            var currentProcess = Process.GetCurrentProcess();
            var currentThread = NativeFunctions.GetCurrentThreadId();

            foreach (ProcessThread thread in currentProcess.Threads) {
                if (thread.Id == currentThread) {
                    continue;
                }


                var pOpenThread = NativeFunctions.OpenThread(NativeFunctions.ThreadAccess.SUSPEND_RESUME, false, (uint)thread.Id);
                if (pOpenThread == IntPtr.Zero) {
                    continue;
                }

                uint suspendCount = 1;
                uint tryCount = 0;

                do {
                    suspendCount = NativeFunctions.ResumeThread(pOpenThread);
                    tryCount++;
                } while (suspendCount > 0 && tryCount < 5);

                if (suspendCount == unchecked((uint)-1)) {
                    var err = NativeFunctions.GetLastError();
                    throw new Exception($"Unable to resume thread error code: 0x{err:x}");
                }
                if (suspendCount > 0) {
                    throw new Exception($"Unable to resume thread suspendCount: {suspendCount}");
                }

                NativeFunctions.CloseHandle(pOpenThread);
            }
        }
    }
}
