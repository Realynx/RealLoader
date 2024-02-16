using System.Diagnostics;

using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services {
    public class FunctionTimingService : IFunctionTimingService {
        public TimeSpan Execute(Action predicate) {
            var timer = new Stopwatch();
            timer.Start();
            predicate();
            timer.Stop();

            return timer.Elapsed;
        }

        public TimeSpan Execute<T>(Func<T> predicate, out T result) {
            var timer = new Stopwatch();
            timer.Start();
            result = predicate();
            timer.Stop();

            return timer.Elapsed;
        }
    }
}