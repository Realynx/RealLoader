namespace PalworldManagedModFramework.Sdk.Services.Detour.AssemblerServices.Interfaces {
    public interface IShellCodeReader {
        int FindMinPatchSize(nint address, int patchLength, int bitness);
    }
}