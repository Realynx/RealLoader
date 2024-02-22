using System.Runtime.CompilerServices;

namespace PalworldManagedModFramework.Sdk.Logging {
    public interface ILogger {
        LogLevel Level { get; set; }

        void Info(string info, [CallerFilePath] string classFile = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string callerName = "");
        void Info(DefaultInterpolatedStringHandler info, [CallerFilePath] string classFile = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string callerName = "");
        void Error(string error, [CallerFilePath] string classFile = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string callerName = "");
        void Error(DefaultInterpolatedStringHandler error, [CallerFilePath] string classFile = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string callerName = "");
        void Warning(string warning, [CallerFilePath] string classFile = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string callerName = "");
        void Warning(DefaultInterpolatedStringHandler warning, [CallerFilePath] string classFile = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string callerName = "");
        void Debug(string debug, [CallerFilePath] string classFile = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string callerName = "");
        void Debug(DefaultInterpolatedStringHandler debug, [CallerFilePath] string classFile = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string callerName = "");
    }
}