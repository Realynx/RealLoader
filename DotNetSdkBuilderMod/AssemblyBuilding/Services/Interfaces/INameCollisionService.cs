namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces {
    public interface INameCollisionService {
        string GetNonCollidingName(string name, HashSet<string> existingNames);
    }
}