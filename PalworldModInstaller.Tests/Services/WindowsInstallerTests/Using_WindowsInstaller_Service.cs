using PalworldModInstaller.Models;
using PalworldModInstaller.Services;

using Shared.SystemUnderTest;

namespace PalworldModInstaller.Tests.Services.WindowsInstallerTests {
    public class Using_WindowsInstaller_Service : SpecAutoMocker<IInstaller, WindowsInstaller>{
        protected InstallerOptions _installerOptions;

        protected string _palWorldPE;
        protected string _palWin64Folder;
        protected string _palGamePE;

        public Using_WindowsInstaller_Service() {
            var testInstallPath = Path.Combine(Environment.CurrentDirectory, $"TestInstallWindows-{Guid.NewGuid()}");

            _palWorldPE = Path.Combine(testInstallPath, "Palworld.exe");
            _palWin64Folder = Path.Combine(testInstallPath, "Pal", "Binaries", "Win64");
            _palGamePE = Path.Combine(_palWin64Folder, "Palworld-Win64-Shipping.exe");

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
