using System.Diagnostics;

using PalworldManagedModFramework.Sdk.Services.Memory.Interfaces;

using static PalworldManagedModFramework.Sdk.Services.Memory.Windows.WindowsStructs;

namespace PalworldManagedModFramework.Sdk.Services.Memory.Windows {
    public class WindowsProcessSuspender : IProcessSuspender {
        public void PauseSelf() {
            var currentProcess = Process.GetCurrentProcess();
            var currentThread = WindowsNativeMethods.GetCurrentThreadId();

            foreach (ProcessThread thread in currentProcess.Threads) {
                if (thread.Id == currentThread) {
                    continue;
                }

                var pOpenThread = WindowsNativeMethods.OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)thread.Id);
                if (pOpenThread == IntPtr.Zero) {
                    continue;
                }

                WindowsNativeMethods.SuspendThread(pOpenThread);
                WindowsNativeMethods.CloseHandle(pOpenThread);
            }
        }

        public void ResumeSelf() {
            var currentProcess = Process.GetCurrentProcess();
            var currentThread = WindowsNativeMethods.GetCurrentThreadId();

            foreach (ProcessThread thread in currentProcess.Threads) {
                if (thread.Id == currentThread) {
                    continue;
                }


                var pOpenThread = WindowsNativeMethods.OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)thread.Id);
                if (pOpenThread == IntPtr.Zero) {
                    continue;
                }
                
                uint suspendCount = 1;
                uint trycount = 0;
                
                do {
                    suspendCount = WindowsNativeMethods.ResumeThread(pOpenThread);
                    trycount++;

                } while (suspendCount > 0 &&  trycount < 5);

                if (suspendCount == unchecked((uint)-1)) {
                    var err = WindowsNativeMethods.GetLastError();
                    throw new Exception($"Unable to resume thread error code: {err}");
                } else if(suspendCount > 0) {
                    throw new Exception($"Unable to resume thread suspendCount: {suspendCount}");
                }

                WindowsNativeMethods.CloseHandle(pOpenThread);
            }
        }
    }
}
