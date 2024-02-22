using System.Runtime.CompilerServices;

namespace PalworldManagedModFramework.Sdk.Logging {
    [Flags]
    public enum LogLevel {
        Debugging = 1,
        Errors = 2,
        Warnings = 4,
        Info = 8
    }

    public class Logger : ILogger {
        public LogLevel Level { get; set; }

        private readonly string _logLocation;
        private readonly LoggerConfig _loggerConfig;

        private string TimeStamp {
            get {
                return DateTime.Now.ToLongTimeString();
            }
        }

        public Logger(LoggerConfig loggerConfig) {
            _loggerConfig = loggerConfig;
            _logLocation = loggerConfig.LogFile;

            Level = LogLevel.Info | LogLevel.Warnings | LogLevel.Errors;

            if (loggerConfig.WriteFile) {
                // TODO: Keep backup of last 5-10 log files instead of erasing the current file
                File.WriteAllText(_logLocation, null);
            }

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
                LogOutput(ConsoleColor.Green, info, classFile, lineNumber, callerName);
            }
        }

        public void Info(DefaultInterpolatedStringHandler info,
            [CallerFilePath] string classFile = "",
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string callerName = "") {
            if (Level.HasFlag(LogLevel.Info)) {
                var infoString = info.ToStringAndClear();
                LogOutput(ConsoleColor.Green, infoString, classFile, lineNumber, callerName);
            }
        }

        public void Error(string error,
            [CallerFilePath] string classFile = "",
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string callerName = "") {
            if (Level.HasFlag(LogLevel.Errors)) {
                LogOutput(ConsoleColor.Red, error, classFile, lineNumber, callerName);
            }
        }

        public void Error(DefaultInterpolatedStringHandler error,
            [CallerFilePath] string classFile = "",
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string callerName = "") {
            if (Level.HasFlag(LogLevel.Errors)) {
                var errorString = error.ToStringAndClear();
                LogOutput(ConsoleColor.Red, errorString, classFile, lineNumber, callerName);
            }
        }

        public void Warning(string warning,
            [CallerFilePath] string classFile = "",
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string callerName = "") {
            if (Level.HasFlag(LogLevel.Warnings)) {
                LogOutput(ConsoleColor.Yellow, warning, classFile, lineNumber, callerName);
            }
        }

        public void Warning(DefaultInterpolatedStringHandler warning,
            [CallerFilePath] string classFile = "",
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string callerName = "") {
            if (Level.HasFlag(LogLevel.Warnings)) {
                var warningString = warning.ToStringAndClear();
                LogOutput(ConsoleColor.Yellow, warningString, classFile, lineNumber, callerName);
            }
        }

        public void Debug(string debug,
            [CallerFilePath] string classFile = "",
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string callerName = "") {
            if (Level.HasFlag(LogLevel.Debugging)) {
                LogOutput(ConsoleColor.Magenta, debug, classFile, lineNumber, callerName);
            }
        }

        public void Debug(DefaultInterpolatedStringHandler debug,
            [CallerFilePath] string classFile = "",
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string callerName = "") {
            if (Level.HasFlag(LogLevel.Debugging)) {
                var debugString = debug.ToStringAndClear();
                LogOutput(ConsoleColor.Magenta, debugString, classFile, lineNumber, callerName);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void LogOutput(ConsoleColor color, string log, string classFile, int lineNumber, string callerName) {
            // If the binary was compiled on windows the constant class filenames will have windows path seperators, and vice versa for linux.
            // So we must check for this manually.
            var directorySeparatorChar = classFile.Contains('/') ? '/' : '\\';

            var className = classFile;
            if (className.Contains(directorySeparatorChar)) {
                className = className.Split(directorySeparatorChar)[^1];
            }
            className = Path.GetFileNameWithoutExtension(className);

            var timeStamp = $"[{TimeStamp}]";

            var logPreamble = $"{timeStamp}[{className}::{callerName};{lineNumber}]: ";

            Console.ForegroundColor = color;
            Console.Write(logPreamble);

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(log);

            if (_loggerConfig.WriteFile) {
                using var sw = File.AppendText(_logLocation);
                sw.Write(timeStamp);
                sw.Write(' ');
                sw.WriteLine(log);
            }
        }
    }
}
