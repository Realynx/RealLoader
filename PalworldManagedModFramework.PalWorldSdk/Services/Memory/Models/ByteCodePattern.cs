namespace PalworldManagedModFramework.Sdk.Services.Memory.Models {
    public record ByteCodePattern {
        public ByteCodePattern(char[] mask, byte[] pattern, int operandOffset) {
            Mask = mask;
            Pattern = pattern;
            OperandOffset = operandOffset;
        }

        public char[] Mask { get; }
        public byte[] Pattern { get; }
        public int OperandOffset { get; }
    }
}
