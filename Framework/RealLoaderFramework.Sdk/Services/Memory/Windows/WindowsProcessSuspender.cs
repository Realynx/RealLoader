using System.Diagnostics;
using System.Runtime.Versioning;

using RealLoaderFramework.Sdk.Logging;
using RealLoaderFramework.Sdk.Services.Memory.Interfaces;

namespace RealLoaderFramework.Sdk.Services.Memory.Windows {
    [SupportedOSPlatform("windows")]
    public class WindowsProcessSuspender : IProcessSuspender {
        private readonly ILogger _logger;

        public WindowsProcessSuspender(ILogger logger) {
            _logger = logger;
        }

        public void PauseSelf() {
            var currentProcess = Process.GetCurrentProcess();
            var currentThread = NativeFunctions.GetCurrentThreadId();

            foreach (ProcessThread thread in currentProcess.Threads) {
                if (thread.Id == currentThread) {
                    continue;
                }

                suspendThread(thread);
            }

            static void suspendThread(ProcessThread thread, int retry = 0) {
                if (retry > 5) {
                    throw new Exception($"Unable to suspend thread: {thread.Id}");
                }

                var pOpenThread = NativeFunctions.OpenThread(NativeFunctions.ThreadAccess.SUSPEND_RESUME, false, thread.Id);
                if (pOpenThread.IsNil) {
                    return;
                }

                var suspendCount = NativeFunctions.SuspendThread(pOpenThread);
                if (suspendCount != -1) {
                    NativeFunctions.CloseHandle(pOpenThread);
                    return;
                }

                suspendThread(thread, retry++);
            }
        }

        public void ResumeSelf() {
            var currentProcess = Process.GetCurrentProcess();
            var currentThread = NativeFunctions.GetCurrentThreadId();

            foreach (ProcessThread thread in currentProcess.Threads) {
                if (thread.Id == currentThread) {
                    continue;
                }

                resumeThread(thread);
            }

            static void resumeThread(ProcessThread thread, int retry = 0) {
                if (retry > 5) {
                    throw new Exception($"Unable to resume thread: {thread.Id}");
                }

                var pOpenThread = NativeFunctions.OpenThread(NativeFunctions.ThreadAccess.SUSPEND_RESUME, false, thread.Id);
                if (pOpenThread.IsNil) {
                    return;
                }

                var suspendCount = NativeFunctions.ResumeThread(pOpenThread);
                if (suspendCount != -1) {
                    NativeFunctions.CloseHandle(pOpenThread);
                    return;
                }

                resumeThread(thread, retry++);
            }
        }
    }
}
