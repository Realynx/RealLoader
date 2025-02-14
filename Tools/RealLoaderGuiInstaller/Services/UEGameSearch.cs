using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;

using Microsoft.Win32;

namespace RealLoaderGuiInstaller.Services {
    internal static class UEGameSearch {
        public static unsafe string[] FindUEGames() {
            // TODO: make this cross plat compatible, this will prbly require DI services and interfaces.
            var installedWindowsSoftware = GetPotentialUEPaths();

            var ueGames = new List<string>();
            foreach (var softwareDir in installedWindowsSoftware) {
                var foundExecutables = Directory.EnumerateFiles(softwareDir, "*.exe");
                if (foundExecutables.Count() != 1) {
                    continue;
                }

                var aliasExecutable = foundExecutables.Single();
                var UEProjectName = aliasExecutable.Split(".")[0];

                var win64Path = Directory.EnumerateFiles(softwareDir, "*", SearchOption.AllDirectories)
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

        private static unsafe bool ContainsFingerprint(FileStream fileStream, string dataSearchSection) {
            ReadOnlySpan<byte> fingerPrint = [
                0x00, 0x55, 0x00, 0x6E, 0x00, 0x72, 0x00, 0x65,
                0x00, 0x61, 0x00, 0x6C, 0x00, 0x45, 0x00, 0x6E,
                0x00, 0x67, 0x00, 0x69, 0x00, 0x6E, 0x00, 0x65, 0x00
            ];

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

        static string[] GetPotentialUEPaths() {
            var installPaths = new List<string>();

            string[] registryKeys =
            {
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
                @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
            };

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

            return installPaths.Distinct().ToArray();
        }
    }
}
