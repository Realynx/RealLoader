using System.Globalization;
using System.Text.RegularExpressions;

using PalworldManagedModFramework.Sdk.Attributes;

namespace PalworldManagedModFramework.Sdk.Services.Memory.Models {
    public partial record ByteCodePattern {
        public ByteCodePattern(char[] mask, byte[] pattern, int operandOffset, PatternType patternType) {
            Mask = mask;
            Pattern = pattern;
            OperandOffset = operandOffset;
            PatternType = patternType;
        }

        public char[] Mask { get; }
        public byte[] Pattern { get; }
        public int OperandOffset { get; }
        public PatternType PatternType { get; }

        [GeneratedRegex(@"[0-9a-fA-F]{2}")]
        private static partial Regex HexRegex();

        public static ByteCodePattern DeriveMask(string signature, PatternType patternType) {
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
            return new ByteCodePattern(mask.ToArray(), buffer.ToArray(), operandOffset, patternType);
        }

        public virtual bool Equals(ByteCodePattern? other) {
            if ((object)this == (object)other) {
                return true;
            }

            if (other is null) {
                return false;
            }

            return Mask.AsSpan().SequenceEqual(other.Mask) && Pattern.AsSpan().SequenceEqual(other.Pattern)
                && OperandOffset == other.OperandOffset && PatternType == other.PatternType;
        }
    }
}
