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
                WindowsNativeMethods.ResumeThread(pOpenThread);
                WindowsNativeMethods.CloseHandle(pOpenThread);
            }
        }
    }
}
