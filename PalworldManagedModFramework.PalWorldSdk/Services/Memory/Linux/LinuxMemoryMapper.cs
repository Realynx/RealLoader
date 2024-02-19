using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;

using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Services.Memory.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Memory.Models;

namespace PalworldManagedModFramework.Sdk.Services.Memory.Linux {
    public partial class LinuxMemoryMapper : IMemoryMapper {
        private const string LINUX_MAPS_PATH = "/proc/self/maps";
        
        [GeneratedRegex(@"([0-9a-fA-F]+)-([0-9a-fA-F]+) ([r-])([w-])([x-])([p-]) ([0-9a-fA-F]+) .+")]
        private static partial Regex LinuxMapRegex();

        private readonly ILogger _logger;

        public LinuxMemoryMapper(ILogger logger) {
            _logger = logger;
        }

        public MemoryRegion[] FindMemoryRegions() {
            return EnumerateMemoryRegions().ToArray();
        }

        public nint GetBaseAddress() {
            var startAddress = Process.GetCurrentProcess().MainModule!.BaseAddress;
            _logger.Debug($"Linux base address: 0x{startAddress:X}");
            return startAddress;
        }

        private IEnumerable<MemoryRegion> EnumerateMemoryRegions() {
            _logger.Debug("Enumerating memory regions");

            var linuxMapsFile = File.ReadAllText(LINUX_MAPS_PATH);
            var memoryRegions = LinuxMapRegex().Matches(linuxMapsFile);

            for (var i = 0; i < memoryRegions.Count; i++) {
                var regionMatch = memoryRegions[i];

                var safePreviousIndex = i > 0 ? i - 1 : 0;
                if (memoryRegions[safePreviousIndex].Value.Contains("[heap]")) {
                    break;
                }

                if (!ulong.TryParse(regionMatch.Groups[1].Value, NumberStyles.AllowHexSpecifier,
                    CultureInfo.InvariantCulture, out var startAddress)) {
                    continue;
                }

                if (!ulong.TryParse(regionMatch.Groups[2].Value, NumberStyles.AllowHexSpecifier,
                    CultureInfo.InvariantCulture, out var endAddress)) {
                    continue;
                }

                var memoryRegion = ConstructMemoryRegion(regionMatch, startAddress, endAddress);
                yield return memoryRegion;
            }
        }

        private static MemoryRegion ConstructMemoryRegion(Match regionMatch, ulong startAddress, ulong endAddress) {
            var readFlag = regionMatch.Groups[3].Value == "r";
            var writeFlag = regionMatch.Groups[4].Value == "w";
            var executeFlag = regionMatch.Groups[5].Value == "x";

            var memoryRegion = new MemoryRegion {
                StartAddress = startAddress,
                EndAddress = endAddress,
                MemorySize = endAddress - startAddress,
                ExecuteFlag = executeFlag,
                WriteFlag = writeFlag,
                ReadFlag = readFlag,
            };

            return memoryRegion;
        }
    }
}
