using System.Diagnostics;

using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services {
    public class FunctionTimingService : IFunctionTimingService {
        public TimeSpan Execute(Action predicate) {
            var timer = Stopwatch.StartNew();
            predicate();
            timer.Stop();

            return timer.Elapsed;
        }

        public TimeSpan Execute<T>(Func<T> predicate, out T result) {
            var timer = Stopwatch.StartNew();
            result = predicate();
            timer.Stop();

            return timer.Elapsed;
        }
    }
}