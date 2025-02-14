using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.Versioning;

using Microsoft.Win32;

namespace RealLoaderGuiInstaller.Services {
    internal static class UEGameSearch {
        private static readonly EnumerationOptions _recursiveEnumerationOptions = new() { RecurseSubdirectories = true };

        public static string[] FindUEGames() {
            // TODO: make this cross plat compatible, this will prbly require DI services and interfaces.
            var potentialUePaths = GetPotentialUEPaths();

            var ueGames = new List<string>();
            foreach (var softwareDir in potentialUePaths) {
                var foundExecutables = Directory.EnumerateFiles(softwareDir, "*.exe").ToArray();

                // TODO: Some UE games have multiple alias executables/launchers
                // TODO: Some UE games don't have alias executables e.g. Killing Floor 2
                if (foundExecutables.Length != 1) {
                    continue;
                }

                var aliasExecutable = foundExecutables.Single();

                // TODO: Not all UE games end with "Win64-Shipping.exe" e.g. Rocket League
                var win64Path = Directory.EnumerateFiles(softwareDir, "*.exe", _recursiveEnumerationOptions)
                    .SingleOrDefault(i => i.EndsWith("Win64-Shipping.exe"));

                if (win64Path is null || !File.Exists(win64Path)) {
                    continue;
                }

                using var aliasExecutableStream = File.OpenRead(aliasExecutable);
                using var win64PathStream = File.OpenRead(win64Path);

                var foundInAlias = ContainsFingerprint(aliasExecutableStream, ".rsrc");
                var foundInWin64 = ContainsFingerprint(win64PathStream, ".rdata");

                if (!foundInAlias || !foundInWin64) {
                    continue;
                }

                ueGames.Add(softwareDir);
            }

            return [.. ueGames];
        }

        private static bool ContainsFingerprint(FileStream fileStream, string dataSearchSection) {
            var fingerPrint = "U\0n\0r\0e\0a\0l\0E\0n\0g\0i\0n\0e\0"u8;

            var peReader = new PEReader(fileStream);
            var rdataSection = peReader.GetSectionData(dataSearchSection);

            var reader = rdataSection.GetReader();
            var fingerPrintIndex = 0;

            while (reader.RemainingBytes > 0) {
                var currentByte = reader.ReadByte();
                if (fingerPrint[fingerPrintIndex] == currentByte) {
                    fingerPrintIndex++;
                    if (fingerPrintIndex == fingerPrint.Length) {
                        return true;
                    }
                }
                else {
                    fingerPrintIndex = 0;
                }
            }

            return false;
        }

        private static string[] GetPotentialUEPaths() {
            var installPaths = new HashSet<string>();

            if (OperatingSystem.IsWindows()) {
                GetWindowsUninstallerGames(installPaths);
                GetWindowsGameBarGames(installPaths);
            }

            return installPaths.ToArray();
        }

        [SupportedOSPlatform("windows")]
        private static void GetWindowsUninstallerGames(HashSet<string> installPaths)
        {
            string[] registryKeys = [
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
                @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall",
            ];

            foreach (var keyPath in registryKeys) {
                using var key = Registry.LocalMachine.OpenSubKey(keyPath);
                if (key == null) {
                    continue;
                }

                foreach (var subKeyName in key.GetSubKeyNames()) {
                    using var subKey = key.OpenSubKey(subKeyName);
                    if (subKey == null) {
                        continue;
                    }

                    var installLocation = subKey.GetValue("InstallLocation") as string;
                    var displayName = subKey.GetValue("DisplayName") as string;

                    if (!string.IsNullOrEmpty(installLocation) && Directory.Exists(Path.Combine(installLocation, "Engine"))) {
                        installPaths.Add(installLocation);
                    }
                }
            }
        }

        [SupportedOSPlatform("windows")]
        private static void GetWindowsGameBarGames(HashSet<string> installPaths)
        {
            string[] registryKeys = [
                @"System\GameConfigStore\Children"
            ];

            foreach (var keyPath in registryKeys) {
                using var key = Registry.CurrentUser.OpenSubKey(keyPath);
                if (key == null) {
                    continue;
                }

                foreach (var subKeyName in key.GetSubKeyNames()) {
                    using var subKey = key.OpenSubKey(subKeyName);
                    if (subKey == null) {
                        continue;
                    }

                    var matchedExe = subKey.GetValue("MatchedExeFullPath") as string;

                    if (string.IsNullOrEmpty(matchedExe) || !File.Exists(matchedExe)) {
                        continue;
                    }

                    var fullDirectoryPath = Path.GetDirectoryName(matchedExe);
                    if (fullDirectoryPath is null || !fullDirectoryPath.EndsWith(Path.Combine("Binaries", "Win64"))) {
                        continue;
                    }

                    var parent = new DirectoryInfo(fullDirectoryPath).Parent?.Parent;
                    if (parent is null) {
                        continue;
                    }

                    if (Directory.Exists(Path.Combine(parent.FullName, "Engine"))) {
                        installPaths.Add(parent.FullName);
                    }

                    parent = parent.Parent;
                    if (parent is not null && Directory.Exists(Path.Combine(parent.FullName, "Engine"))) {
                        installPaths.Add(parent.FullName);
                    }
                }
            }
        }
    }
}