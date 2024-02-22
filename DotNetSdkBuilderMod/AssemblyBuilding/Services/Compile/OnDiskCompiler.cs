using System.Runtime.InteropServices;
using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using PalworldManagedModFramework.Sdk.Logging;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Compile {
    public class OnDiskCompiler : ICodeCompiler {
        private readonly ILogger _logger;
        private readonly string _buildLocation;
        private List<string> _writtenFiles = new();

        public OnDiskCompiler(ILogger logger, string buildLocation) {
            _logger = logger;
            _buildLocation = buildLocation;
        }

        public void AppendFile(StringBuilder code, string nameSpace) {
            var directory = Path.Combine(_buildLocation, Path.Combine(nameSpace.Split('.', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)));
            if (Directory.Exists(directory)) {
                Directory.Delete(directory, true);
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                Directory.CreateDirectory(directory);
            }
            else {
                Directory.CreateDirectory(directory, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
            }

            var filePath = Path.Combine(directory, $"{nameSpace}.cs");
            if (File.Exists(filePath)) {
                throw new Exception($"{filePath} already exists.");
            }

            using var sw = File.CreateText(filePath);
            sw.Write(code);

            _writtenFiles.Add(filePath);
            _logger.Debug($"Wrote code file {filePath}");
        }

        public void Compile() {

        }
    }
}