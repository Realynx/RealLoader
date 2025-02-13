namespace RealLoaderFramework.Sdk.Services.Detour.Interfaces {
    public interface IInstructionPatcher {
        byte[] PatchLiveInstructions(nint address, ReadOnlySpan<byte> instructionBytes);
    }
}