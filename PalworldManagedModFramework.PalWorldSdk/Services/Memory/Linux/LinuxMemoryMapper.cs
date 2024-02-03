using System.Diagnostics;
using System.Text.RegularExpressions;

namespace PalworldManagedModFramework.PalWorldSdk.Services.Memory.Linux {
    public class LinuxMemoryMapper : IMemoryMapper {
        private readonly Regex _linuxMapRegex = new Regex(@"([0-9a-fA-F]+)-([0-9a-fA-F]+) ([r-])([w-])([x-])([p-]) ([0-9a-fA-F]+) ");

        public MemoryRegion[] FindMemoryRegions(ProcessModule processModule) {
            return EnumerateMemoryRegions().ToArray();
        }

        private IEnumerable<MemoryRegion> EnumerateMemoryRegions() {
            var linuxMapsFile = File.ReadAllText("/proc/self/maps");
            var memoryRegions = _linuxMapRegex.Matches(linuxMapsFile);

            foreach (Match regionMatch in memoryRegions!) {
                if (!ulong.TryParse(regionMatch.Groups[1].Value, out var startAddress)) {
                    continue;
                }

                if (!ulong.TryParse(regionMatch.Groups[2].Value, out var endAddress)) {
                    continue;
                }

                var readFlag = regionMatch.Groups[3].Value == "r";
                var writeFlag = regionMatch.Groups[4].Value == "w";
                var executeFlag = regionMatch.Groups[5].Value == "x";


                yield return new MemoryRegion {
                    StartAddress = startAddress,
                    EndAddress = endAddress,
                    MemorySize = endAddress - startAddress,
                    ExecuteFlag = executeFlag,
                    WriteFlag = writeFlag,
                    ReadFlag = readFlag,
                };
            }
        }
    }
}
