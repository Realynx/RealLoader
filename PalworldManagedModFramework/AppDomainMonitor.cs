using System.Runtime.ExceptionServices;

using PalworldManagedModFramework.Sdk.Services;

namespace PalworldManagedModFramework {
    public static class AppDomainMonitor {
        public static void MonitorDomain() {
            var currentDomain = AppDomain.CurrentDomain;
            Console.WriteLine(@$"
AppDomain Details:
{nameof(currentDomain.FriendlyName)}: {currentDomain.FriendlyName}
{nameof(currentDomain.Id)}: {currentDomain.Id}
{nameof(currentDomain.SetupInformation.TargetFrameworkName)}: {currentDomain.SetupInformation.TargetFrameworkName}
Image base: {currentDomain.SetupInformation.ApplicationBase ?? "N/A"}
Environment: {Enum.GetName(typeof(PlatformID), Environment.OSVersion.Platform)} :: {Environment.OSVersion.VersionString}
{nameof(currentDomain.ShadowCopyFiles)}: {currentDomain.ShadowCopyFiles}
{nameof(currentDomain.IsFullyTrusted)}: {currentDomain.IsFullyTrusted}
{nameof(currentDomain.IsHomogenous)}: {currentDomain.IsHomogenous}
Loaded Assemblies: {AppDomain.CurrentDomain.GetAssemblies().Length}
Available Memory: 0x{GC.GetGCMemoryInfo().TotalAvailableMemoryBytes:x2}
Heap Size: 0x{GC.GetGCMemoryInfo().HeapSizeBytes:x2}
");

            CatchErrors();
        }

        private static void CatchErrors() {
            Console.WriteLine("Monitoring for internal CLR exceptions...");
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // Log.Info("Monitoring for loaded assemblies...");
            // AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
        }

        private static void CurrentDomain_FirstChanceException(object? sender, FirstChanceExceptionEventArgs e) {
            var exception = e.Exception;
            if (!ShouldLogException(exception)) {
                return;
            }

            Console.WriteLine(@$"
[0x{exception.HResult:x2}] An exception was thrown in managed code
Error Message: {exception.Message}
Error Site: {exception.TargetSite?.DeclaringType?.ToString() ?? "Private type"}::{exception.TargetSite?.Name}
Stack Trace:
{exception.StackTrace}");

            if (exception.HelpLink != null) {
                Console.WriteLine($"Help Link: {exception.HelpLink}");
            }
        }
        private static void CurrentDomain_UnhandledException(object? sender, UnhandledExceptionEventArgs e) {
            var exception = e.ExceptionObject as Exception;
            if (!ShouldLogException(exception)) {
                return;
            }

            Console.WriteLine(@$"
[0x{exception!.HResult:x2}] An exception was thrown in managed code
Error Message: {exception.Message}
Error Site: {exception.TargetSite?.DeclaringType?.ToString() ?? "Private type"}::{exception.TargetSite?.Name}
Stack Trace:
{exception.StackTrace}");

            if (exception.HelpLink != null) {
                Console.WriteLine($"Help Link: {exception.HelpLink}");
            }
        }

        private static void CurrentDomain_AssemblyLoad(object? sender, AssemblyLoadEventArgs args) {
            Console.WriteLine(@$"
Loaded Assembly: 
{nameof(args.LoadedAssembly.FullName)}: {args.LoadedAssembly.FullName}
{nameof(args.LoadedAssembly.Location)}: {args.LoadedAssembly.Location}
{nameof(args.LoadedAssembly.EntryPoint)}: {args.LoadedAssembly.EntryPoint?.Name ?? "No entry Point"}
{nameof(args.LoadedAssembly.ImageRuntimeVersion)}: {args.LoadedAssembly.ImageRuntimeVersion}
{nameof(args.LoadedAssembly.IsDynamic)}: {args.LoadedAssembly.IsDynamic}
");
        }

        private static bool ShouldLogException(Exception e) {
            if (e is null) {
                return false;
            }

            if (e.TargetSite?.DeclaringType?.Name is "MethodBaseInvoker" && e.TargetSite?.Name is "InvokeDirectByRefWithFewArgs") {
                return false;
            }

            return true;
        }
    }
}
