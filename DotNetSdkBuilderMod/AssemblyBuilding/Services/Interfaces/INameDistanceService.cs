namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces
{
    public unsafe interface INameDistanceService
    {
        string FindLargestCommonSubstring(ReadOnlySpan<char> stringA, ReadOnlySpan<char> stringB);
        int FindDistance(ReadOnlySpan<char> stringA, ReadOnlySpan<char> stringB);
    }
}