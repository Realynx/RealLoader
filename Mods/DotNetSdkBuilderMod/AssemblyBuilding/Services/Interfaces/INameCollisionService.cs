namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces {
    public interface INameCollisionService {
        string GetNonCollidingName(string name, ISet<string> existingNames);
        string GetNonCollidingName(string name, string existingName);
        string GetNonCollidingName(string name);
    }
}