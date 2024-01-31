using PalworldModInstaller.Models;
using PalworldModInstaller.Services;

using Spectre.Console;
using Spectre.Console.Cli;

namespace PalworldModInstaller.Commands {
    internal class InstallCommand : Command<InstallerOptions> {
        private readonly LinuxInstaller _linuxInstaller;
        private readonly WindowsInstaller _windowsInstaller;

        public InstallCommand(LinuxInstaller linuxInstaller, WindowsInstaller windowsInstaller) {
            _linuxInstaller = linuxInstaller;
            _windowsInstaller = windowsInstaller;
        }

        public override int Execute(CommandContext context, InstallerOptions settings) {
            var currentPlatform = Environment.OSVersion.Platform;

            Installer modloaderInstaller = null;
            switch (currentPlatform) {

                // Windows 3.1 that provided partial Win32 API support
                case PlatformID.Win32S:
                    break;

                // Windows 95 and Windows 98 platforms
                case PlatformID.Win32Windows:
                    break;

                // Windows NT and all modern Windows versions based on the NT architecture, Windows 2000, XP, Vista, 7, 8, 10, and 11
                case PlatformID.Win32NT:
                    modloaderInstaller = _windowsInstaller;
                    break;

                // Windows CE (Compact Edition)
                case PlatformID.WinCE:
                    break;

                case PlatformID.Unix:
                    modloaderInstaller = _linuxInstaller;
                    break;

                case PlatformID.Xbox:
                    break;

                case PlatformID.MacOSX:
                    break;

                case PlatformID.Other:
                    break;

                default:
                    break;
            }

            if (modloaderInstaller is null) {
                var exception = new Exception($"[{currentPlatform}] System is not supported!");
                AnsiConsole.WriteException(exception);

                throw exception;
            }

            if (settings.Uninstall) {
                modloaderInstaller.UninstallFiles(settings);
            }
            else {
                modloaderInstaller.InstallFiles(settings);
            }

            return 0;
        }
    }
}
