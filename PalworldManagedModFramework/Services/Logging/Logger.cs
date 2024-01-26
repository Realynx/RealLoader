using System.Runtime.CompilerServices;

using PalworldManagedModFramework.Models.Config;

namespace PalworldManagedModFramework.Services.Logging {
    [Flags]
    public enum LogLevel {
        Debugging = 1,
        Errors = 2,
        Warnings = 4,
        Info = 8
    }
    internal class Logger : ILogger {
        public LogLevel Level { get; set; }

        private readonly string _logLocation;
        private readonly Semaphore _semaphore;
        private readonly LoggerConfig _loggerConfig;

        private string TimeStamp {
            get {
                return DateTime.Now.ToLongTimeString();
            }
        }

        public Logger(LoggerConfig loggerConfig) {
            _loggerConfig = loggerConfig;

            _logLocation = loggerConfig.LogFile;

            _semaphore = new Semaphore(1, 1);
            Level = LogLevel.Info | LogLevel.Warnings | LogLevel.Errors;

            if (loggerConfig.DebugLogs) {
                Level |= LogLevel.Debugging;
                Debug("Enabled debug logs...");
            }
        }

        public void Info(string info,
            [CallerFilePath] string classFile = "",
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string callerName = "") {
            if (Level.HasFlag(LogLevel.Info)) {
                LogOutput(ConsoleColor.Green, string.Join("\n", info), classFile, lineNumber, callerName);
            }
        }

        public void Error(string error,
            [CallerFilePath] string classFile = "",
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string callerName = "") {
            if (Level.HasFlag(LogLevel.Errors)) {
                LogOutput(ConsoleColor.Red, string.Join("\n", error), classFile, lineNumber, callerName);
            }
        }

        public void Warning(string warning,
            [CallerFilePath] string classFile = "",
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string callerName = "") {
            if (Level.HasFlag(LogLevel.Warnings)) {
                LogOutput(ConsoleColor.Yellow, string.Join("\n", warning), classFile, lineNumber, callerName);
            }
        }

        public void Debug(string debug,
            [CallerFilePath] string classFile = "",
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string callerName = "") {
            if (Level.HasFlag(LogLevel.Debugging)) {
                LogOutput(ConsoleColor.Magenta, string.Join("\n", debug), classFile, lineNumber, callerName);
            }
        }

        private void LogOutput(ConsoleColor color, string log, string classFile, int lineNumber, string callerName) {
            _semaphore.WaitOne();
            var className = Path.GetFileNameWithoutExtension(classFile);
            var logPreamble = $"[{TimeStamp}][{className}::{callerName};{lineNumber}]: ";

            Console.ForegroundColor = color;
            Console.Write(logPreamble);

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(log);

            if (_loggerConfig.WriteFile) {
                File.WriteAllText(_logLocation, log);
            }

            _semaphore.Release();
        }
    }
}
