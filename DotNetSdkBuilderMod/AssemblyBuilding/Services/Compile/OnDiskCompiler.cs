using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using PalworldManagedModFramework.Sdk.Logging;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Compile {
    public class OnDiskCompiler : ICodeCompiler {
        private readonly ILogger _logger;
        private readonly string _sourceLocation;
        private readonly bool _displayCompilerOutput;

        public OnDiskCompiler(ILogger logger, string buildLocation, bool displayCompilerOutput) {
            _logger = logger;
            _sourceLocation = Path.Combine(buildLocation, "source");
            _displayCompilerOutput = displayCompilerOutput;
        }

        public void RegisterExistingAssembly(Assembly assembly) {
            // throw new NotImplementedException();
        }

        public void AppendSolutionFile(StringBuilder code) {
            var filePath = Path.Combine(_sourceLocation, $"{CodeGenConstants.CODE_SOLUTION_NAME}.sln");
            var directory = Path.GetDirectoryName(_sourceLocation)!;
            if (!Directory.Exists(directory)) {
                CreateDirectory(directory);
            }

            File.WriteAllText(filePath, code.ToString());
            _logger.Debug($"Wrote solution file {filePath}");
        }

        public void AppendProjectFile(StringBuilder code, string assemblyName) {
            var filePath = Path.Combine(_sourceLocation, assemblyName, $"{assemblyName}.csproj");
            var directory = Path.GetDirectoryName(_sourceLocation)!;
            if (!Directory.Exists(directory)) {
                CreateDirectory(directory);
            }

            File.WriteAllText(filePath, code.ToString());
            _logger.Debug($"Wrote project file {filePath}");
        }

        public void AppendFile(StringBuilder code, string assemblyName, string nameSpace) {
            var namespaceDirectories = Path.Combine(nameSpace.Split('.', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));
            var directory = Path.Combine(_sourceLocation, namespaceDirectories);
            if (Directory.Exists(directory)) {
                Directory.Delete(directory, true);
            }

            CreateDirectory(directory);

            var filePath = Path.Combine(directory, $"{nameSpace}.cs");
            if (File.Exists(filePath)) {
                throw new InvalidOperationException($"{filePath} already exists.");
            }

            using var sw = File.CreateText(filePath);
            sw.Write(code);

            // _logger.Debug($"Wrote code file {filePath}");
        }

        private void CreateDirectory(string directory) {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                Directory.CreateDirectory(directory);
            }
            else {
                Directory.CreateDirectory(directory, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
            }
        }

        public void Compile() {
            var solutionPath = Path.Combine(_sourceLocation, $"{CodeGenConstants.CODE_SOLUTION_NAME}.sln");
            var startInfo = new ProcessStartInfo {
                FileName = "dotnet",
                Arguments = $"build \"{solutionPath}\"", // -p:{CodeGenConstants.BUILD_OUTPUT_ENVIRONMENT_VARIABLE}=\"{_buildLocation}\"",
                UseShellExecute = false,
                CreateNoWindow = !_displayCompilerOutput,
            };

            var buildProcess = Process.Start(startInfo);
            if (buildProcess is null) {
                throw new Exception("Failed to run build process.");
            }

            buildProcess.WaitForExit();

            try {
                // For some reason...
                buildProcess.Kill();
            }
            catch {
                // ignored
            }

            if (buildProcess.ExitCode is not 0) {
                _logger.Error($".NET SDK exited with code {buildProcess.ExitCode}.");
                return;
            }

            var buildPath = Path.Combine(_sourceLocation, CodeGenConstants.CODE_NAMESPACE, "bin", "Debug", "net8.0");

            _logger.Info($"Successfully compiled proxy SDK to {buildPath}");
        }
    }
}