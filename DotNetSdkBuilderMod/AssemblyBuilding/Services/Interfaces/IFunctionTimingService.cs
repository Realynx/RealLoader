namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces {
    public interface IFunctionTimingService {
        TimeSpan Execute(Action predicate);
        TimeSpan Execute<T>(Func<T> predicate, out T result);
    }
}