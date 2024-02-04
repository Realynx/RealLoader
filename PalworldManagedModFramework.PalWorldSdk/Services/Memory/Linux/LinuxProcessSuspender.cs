using System.Diagnostics;

namespace PalworldManagedModFramework.PalWorldSdk.Services.Memory.Linux {
    public class LinuxProcessSuspender : IProcessSuspender {
        public void PauseSelf() {
            var currentThreadId = GetCurrentThreadId();
            var processPath = $"/proc/self/task";
            var threadDirs = Directory.GetDirectories(processPath);

            foreach (var threadDir in threadDirs) {
                var threadId = Path.GetFileName(threadDir);
                if (threadId != currentThreadId) {
                    var startInfo = new ProcessStartInfo {
                        FileName = "kill",
                        Arguments = $"-STOP {threadId}",
                        UseShellExecute = false
                    };
                    Process.Start(startInfo);
                }
            }
        }

        public void ResumeSelf() {
            var currentThreadId = GetCurrentThreadId();
            var processPath = $"/proc/self/task";
            var threadDirs = Directory.GetDirectories(processPath);

            foreach (var threadDir in threadDirs) {
                var threadId = Path.GetFileName(threadDir);
                if (threadId != currentThreadId) {
                    var startInfo = new ProcessStartInfo {
                        FileName = "kill",
                        Arguments = $"-CONT {threadId}",
                        UseShellExecute = false
                    };
                    Process.Start(startInfo);
                }
            }
        }

        private string GetCurrentThreadId() {
            return File.ReadAllText("/proc/self/task/self/tid").Trim();
        }
    }
}
