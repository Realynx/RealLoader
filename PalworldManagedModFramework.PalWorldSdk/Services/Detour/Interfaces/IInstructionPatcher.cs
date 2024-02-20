namespace PalworldManagedModFramework.Sdk.Services.Detour.Interfaces {
    public interface IInstructionPatcher {
        byte[] PatchInstructions(nint address, ReadOnlySpan<byte> instructionBytes);
    }
}