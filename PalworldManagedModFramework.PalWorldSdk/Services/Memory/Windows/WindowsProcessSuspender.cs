using System.Diagnostics;

using static PalworldManagedModFramework.PalWorldSdk.Services.Memory.Windows.WindowsStructs;

namespace PalworldManagedModFramework.PalWorldSdk.Services.Memory.Windows {
    public class WindowsProcessSuspender : IProcessSuspender {
        public void PauseSelf() {
            var currentProcess = Process.GetCurrentProcess();
            foreach (ProcessThread thread in currentProcess.Threads) {
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
            foreach (ProcessThread thread in currentProcess.Threads) {
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
