using Xunit;

namespace RealLoaderInstaller.Tests.Services.LinuxInstallerTests {
    public class When_Installing_Fresh_Install : Using_LinuxInstaller_Service {

        protected override void Setup() {
            Directory.CreateDirectory(_palLinuxFolder);
            File.WriteAllText(_palGamePE, string.Empty);
        }

        protected override void Act() {
            TestableImplementation.InstallFiles(_installerOptions);
        }

        [Fact]
        public void Was_Mods_Folder_Created() {
            try {
                var modsFolder = Path.Combine(_installerOptions.InstallLocation!, "ClrMods");
                var exists = Path.Exists(modsFolder);
                Assert.True(exists);
            }
            finally {
                Dispose();
            }
        }

        [Fact]
        public void Was_Framework_Install_Dir_Created() {
            try {
                var managedModFrameworkFolder = Path.Combine(_palLinuxFolder, "ManagedModFramework");
                Assert.True(Path.Exists(managedModFrameworkFolder));
            }
            finally {
                Dispose();
            }
        }
    }
}
