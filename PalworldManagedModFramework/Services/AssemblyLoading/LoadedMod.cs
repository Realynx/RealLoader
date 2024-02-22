using System.Reflection;

using PalworldManagedModFramework.Models;
using PalworldManagedModFramework.Sdk.Interfaces;
using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Services.SandboxDI.Interfaces;

namespace PalworldManagedModFramework.Services.AssemblyLoading {
    internal class LoadedMod {
        public CancellationTokenSource CancelTokenSource { get; set; }


        private readonly Type _entryPoint;
        private object _entryPointInstance = null;

        private readonly ClrMod _clrMod;
        private readonly ILogger _logger;
        private readonly ISandboxDIService _sandboxDIService;
        private readonly Task _runningTask;

        private bool IsSandboxedDI {
            get {
                return ModStartup != null;
            }
        }

        private ISbStartup _sbStartup = null;
        private ISbStartup ModStartup {
            get {
                if (_sbStartup is null) {
                    var startupType = _clrMod.Assembly.GetTypes()
                        .FirstOrDefault(type => typeof(ISbStartup).IsAssignableFrom(type) && !type.IsAbstract);

                    if (startupType != null) {
                        _sbStartup = Activator.CreateInstance(startupType) as ISbStartup;
                    }
                }

                return _sbStartup;
            }
        }

        public LoadedMod(Type entryPoint, ClrMod clrMod, ILogger logger, ISandboxDIService sandboxDIService) {
            _entryPoint = entryPoint;
            _clrMod = clrMod;
            _logger = logger;
            _sandboxDIService = sandboxDIService;
            CancelTokenSource = new CancellationTokenSource();

            _runningTask = Task.Factory.StartNew(Load, CancelTokenSource.Token)
                .ContinueWith(ModUnloaded);
        }

        public void ModUnloaded(Task modTask) {
            if (!modTask.IsFaulted) {
                _logger.Info($"[{_clrMod.PalworldModAttribute.ModName}] Mod was unloaded gracefully");
            }
            else {
                _logger.Warning($"[{_clrMod.PalworldModAttribute.ModName}] Mod was unloaded with errors...\n" +
                    $"{modTask.Exception}");
            }

            modTask.Dispose();
        }

        public void Unload() {
            _logger.Warning($"[{_clrMod.PalworldModAttribute.ModName}] Requested Unload.");

            if (IsSandboxedDI) {
                _sandboxDIService.DestroyProvider(ModStartup);
            }

            Task.Factory.StartNew(() => {
                (_entryPointInstance as IPalworldMod).Unload();
                CancelTokenSource.Cancel();

                Task.Delay(TimeSpan.FromSeconds(5));

                if (!_runningTask.IsCompleted) {
                    _logger.Error($"[{_clrMod.PalworldModAttribute.ModName}] Causing delinquent thread to hang!");
                }

                _runningTask.Dispose();
                CancelTokenSource.Dispose();
            });
        }

        private void Load() {

            if (!ConstructMod()) {
                return;
            }

            if (_entryPointInstance is null) {
                _logger.Error($"Could not load [{_clrMod.PalworldModAttribute.ModName}]. Failed to activate/construct mod's entrypoint!");
                return;
            }

            (_entryPointInstance as IPalworldMod).Load();
            _logger.Debug($"[{_clrMod.PalworldModAttribute.ModName}] has relinquished thread execution. Holding until unload event.");

            while (!CancelTokenSource.Token.IsCancellationRequested) {
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        }

        private bool ConstructMod() {
            var constructors = _entryPoint.GetConstructors();
            var ctor = constructors.FirstOrDefault();

            if (constructors.Length > 1) {
                _logger.Warning($"{_entryPoint.FullName} has too many constructors! Skipping mod.");
                return false;
            }

            if (ctor is null) {
                _logger.Warning($"{_entryPoint.FullName} does not contain a valid constructor! Skipping mod.");
                return false;
            }

            if (IsSandboxedDI) {
                _sandboxDIService.InitServiceProvider(ModStartup);
            }

            var dependencies = ResolveDependencies(ctor);
            if (dependencies.Any(i => i is null)) {
                _logger.Error($"Could not resolve dependencies for {ctor.GetType().Name}");
                return false;
            }

            _entryPointInstance = ctor.Invoke(dependencies);
            return true;
        }

        private object[] ResolveDependencies(ConstructorInfo ctor) {
            var parameters = ctor.GetParameters();
            var args = new object[parameters.Length];

            for (var x = 0; x < parameters.Length; x++) {
                var parameterType = parameters[x].ParameterType;

                if (parameterType == typeof(CancellationToken)) {
                    args[x] = CancelTokenSource.Token;
                    continue;
                }

                var service = _sandboxDIService.ResolveService(parameterType, ModStartup);
                if (service is null) {
                    _logger.Warning($"The required service of type {parameterType.FullName} cannot be resolved for {_entryPoint.FullName}. Skipping mod.");
                }

                args[x] = service;
            }

            return args;
        }
    }
}
