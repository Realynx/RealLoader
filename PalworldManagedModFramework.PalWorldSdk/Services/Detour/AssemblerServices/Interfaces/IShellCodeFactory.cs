namespace PalworldManagedModFramework.Sdk.Services.Detour.AssemblerServices.Interfaces {
    public interface IShellCodeFactory {
        byte[] BuildStackDetour64(nint redirect);
        byte[] BuildTrampoline64(byte[] overwrittenCodes, nint offsetStackAddr);
    }
}