using PalworldModInstaller.Models;
using PalworldModInstaller.Services;

using Shared.SystemUnderTest;

namespace PalworldModInstaller.Tests.Services.LinuxInstallerTests {
    public class Using_LinuxInstaller_Service : SpecAutoMocker<IInstaller, LinuxInstaller> {
        protected InstallerOptions _installerOptions;

        protected string _palWorldPE;
        protected string _palLinuxFolder;
        protected string _palGamePE;

        public Using_LinuxInstaller_Service() {
            var testInstallPath = Path.Combine(Environment.CurrentDirectory, $"TestInstallLinux-{Guid.NewGuid()}");

            _palWorldPE = Path.Combine(testInstallPath, "PalServer.sh");
            _palLinuxFolder = Path.Combine(testInstallPath, "Pal", "Binaries", "Linux");
            _palGamePE = Path.Combine(_palLinuxFolder, "PalServer-Linux-Test");

            _installerOptions = new InstallerOptions() {
                CheckUpdates = false,
                CreateModsFolder = true,
                Uninstall = false,
                Backup = null,
                InstallLocation = testInstallPath
            };

            Init();
        }

        public void Dispose() {
            Directory.Delete(_installerOptions.InstallLocation!, true);
        }
    }
}
