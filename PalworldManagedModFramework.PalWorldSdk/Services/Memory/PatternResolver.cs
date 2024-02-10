using PalworldManagedModFramework.Sdk.Attributes;
using System.Reflection;

using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Services.Memory.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Memory.Linux;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PalworldManagedModFramework.Sdk.Services.Memory {
    public class PatternResolver : IPatternResolver {
        private static readonly Regex _hexRegex = new(@"[0-9a-fA-F]{2}");

        private readonly ILogger _logger;
        private readonly IOperandResolver _operandResolver;
        private readonly IMemoryScanner _memoryScanner;

        public PatternResolver(ILogger logger, IOperandResolver operandResolver, IMemoryScanner memoryScanner) {
            _logger = logger;
            _operandResolver = operandResolver;
            _memoryScanner = memoryScanner;
        }

        public nint? ResolvePattern(PropertyInfo member, object? instance = null) {
            var machineCodeAttribute = member.GetCustomAttributes<MachineCodePatternAttribute>(true).FirstOrDefault()
                ?? throw new InvalidOperationException($"Type must have {nameof(MachineCodePatternAttribute)}");

            var patternScanResult = _memoryScanner.SingleSequenceScan(machineCodeAttribute.Pattern);
            if (patternScanResult is not nint patternOffsetAddress) {
                _logger.Error($"Could not resolve pattern for {member.Name}, Pattern: {machineCodeAttribute.Pattern}");
                return null;
            }

            _logger.Debug($"{member.Name} pattern found: 0x{patternOffsetAddress:X}");
            var patternMask = DeriveMask(machineCodeAttribute.Pattern);
            var resolvedAddress = _operandResolver.ResolveInstructionAddress(patternOffsetAddress, patternMask.operandOffset, machineCodeAttribute);

            if (instance is not null) {
                member.SetValue(instance, resolvedAddress);
            }

            return resolvedAddress;
        }

        public static (char[] mask, byte[] pattern, int operandOffset) DeriveMask(string signature) {
            var hexValues = signature.Split(' ');

            if (hexValues.Length < 2) {
                throw new Exception($"Invalid pattern, your {nameof(signature)} is not long enough.");
            }

            var mask = new List<char>();
            var buffer = new List<byte>();
            var operandOffset = -1;

            var bytePosition = 0;

            for (var x = 0; x < hexValues.Length; x++) {
                switch (hexValues[x]) {
                    case var hex when _hexRegex.IsMatch(hex):
                        buffer.Add(byte.Parse(hex, NumberStyles.HexNumber));
                        mask.Add('x');
                        bytePosition++;
                        break;

                    case "?":
                        buffer.Add(0x00);
                        mask.Add('?');
                        bytePosition++;
                        break;

                    case "|":
                        if (operandOffset != -1) {
                            throw new Exception("Multiple offset indicators '|' found in the pattern.");
                        }
                        operandOffset = bytePosition;
                        break;
                }
            }

            if (buffer.Count != mask.Count) {
                throw new Exception("Invalid Signature");
            }

            operandOffset = operandOffset == -1 ? 0 : operandOffset;
            return (mask.ToArray(), buffer.ToArray(), operandOffset);
        }
    }
}
