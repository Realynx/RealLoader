using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

using PalworldManagedModFramework.Sdk.Attributes;
using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Services.Memory.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Memory.Linux;
using PalworldManagedModFramework.Sdk.Services.Memory.Models;

namespace PalworldManagedModFramework.Sdk.Services.Memory {
    public partial class PatternResolver : IPatternResolver {
        [GeneratedRegex(@"[0-9a-fA-F]{2}")]
        private static partial Regex HexRegex();

        private readonly ILogger _logger;
        private readonly IOperandResolver _operandResolver;
        private readonly IMemoryScanner _memoryScanner;

        public PatternResolver(ILogger logger, IOperandResolver operandResolver, IMemoryScanner memoryScanner) {
            _logger = logger;
            _operandResolver = operandResolver;
            _memoryScanner = memoryScanner;
        }

        public nint?[] ResolvePatterns(PropertyInfo[] members, object?[] instances) {
            var results = new nint?[members.Length];

            var patterns = new MachineCodePatternAttribute[members.Length];
            for (var x = 0; x < members.Length; x++) {
                var machineCodeAttribute = GetMachineCodeAttribute(members[x]);
                patterns[x] = machineCodeAttribute;
            }

            var stringPatterns = patterns.Select(i => i.Pattern).ToArray();
            var patternScanResults = _memoryScanner.SequenceScan(stringPatterns);
            if (patternScanResults is null) {
                return Array.Empty<nint?>();
            }

            for (var x = 0; x < patternScanResults.Length; x++) {
                var scanResult = patternScanResults[x];
                var firstMatchedAddress = scanResult.FirstOrDefault();
                if (firstMatchedAddress is 0) {
                    _logger.Error($"Could not resolve pattern for {members[x].Name}, Pattern: {patterns[x].Pattern}");
                    results[x] = null;
                }

                results[x] = UpdatePropertyValue(members[x], instances[x], patterns[x], firstMatchedAddress);
            }

            return results;
        }

        public nint? ResolvePattern(PropertyInfo member, object? instance = null) {
            var machineCodeAttribute = GetMachineCodeAttribute(member);

            var patternScanResult = _memoryScanner.SingleSequenceScan(machineCodeAttribute.Pattern);
            if (patternScanResult is not nint patternOffsetAddress) {
                _logger.Error($"Could not resolve pattern for {member.Name}, Pattern: {machineCodeAttribute.Pattern}");
                return null;
            }

            var resolvedAddress = UpdatePropertyValue(member, instance, machineCodeAttribute, patternOffsetAddress);
            return resolvedAddress;
        }

        private static MachineCodePatternAttribute GetMachineCodeAttribute(PropertyInfo member) {
            return member.GetCustomAttributes<MachineCodePatternAttribute>(true).FirstOrDefault()
                            ?? throw new InvalidOperationException($"Type must have {nameof(MachineCodePatternAttribute)}");
        }

        //TODO: Move into new service, 
        private nint UpdatePropertyValue(PropertyInfo member, object? instance, MachineCodePatternAttribute machineCodeAttribute, nint patternOffsetAddress) {
            _logger.Debug($"{member.Name} pattern found: 0x{patternOffsetAddress:X}");

            var patternMask = DeriveMask(machineCodeAttribute.Pattern);
            var resolvedAddress = _operandResolver.ResolveInstructionAddress(patternOffsetAddress, patternMask.OperandOffset, machineCodeAttribute);

            if (instance is not null) {
                if (machineCodeAttribute.PatternType == PatternType.Function) {
                    var delegateValue = Marshal
                        .GetDelegateForFunctionPointer(resolvedAddress, member.PropertyType);
                    member.SetValue(instance, delegateValue);
                }
                else {
                    member.SetValue(instance, resolvedAddress);
                }
            }

            return resolvedAddress;
        }

        public static ByteCodePattern DeriveMask(string signature) {
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
                    case var hex when HexRegex().IsMatch(hex):
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
            return new ByteCodePattern(mask.ToArray(), buffer.ToArray(), operandOffset);
        }
    }
}