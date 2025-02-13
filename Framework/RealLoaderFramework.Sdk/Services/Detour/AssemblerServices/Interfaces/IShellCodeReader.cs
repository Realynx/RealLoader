namespace RealLoaderFramework.Sdk.Services.Detour.AssemblerServices.Interfaces {
    public interface IShellCodeReader {
        int FindMinPatchSize(nint address, int patchLength, int bitness);
        byte[] FixRelativeOffsets(nint originalAddress, nint newAddress, byte[] codes, int bitness);
    }
}