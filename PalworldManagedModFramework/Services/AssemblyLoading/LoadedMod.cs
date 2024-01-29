
using System.Threading;

using PalworldManagedModFramework.Models;
using PalworldManagedModFramework.PalWorldSdk.Interfaces;
using PalworldManagedModFramework.Services.Logging;

namespace PalworldManagedModFramework.Services.AssemblyLoading {
    public class LoadedMod {
        public CancellationTokenSource CancelTokenSource { get; set; }


        private readonly Type _entryPoint;
        private object _entryPointInstance = null;

        private readonly ClrMod _clrMod;
        private readonly ILogger _logger;

        private readonly Task _runningTask;

        public LoadedMod(Type entryPoint, ClrMod clrMod, ILogger logger) {
            _entryPoint = entryPoint;
            _clrMod = clrMod;
            _logger = logger;

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

        private void Load() {
            _entryPointInstance = Activator.CreateInstance(_entryPoint);
            if (_entryPointInstance is null) {
                _logger.Error($"Could not load [{_clrMod.PalworldModAttribute.ModName}]. Failed to activate/construct mod's entrypoint!");
                return;
            }

            (_entryPointInstance as IPalworldMod).Load(CancelTokenSource.Token);

            while (!CancelTokenSource.Token.IsCancellationRequested) {
                Thread.Sleep(500);
            }
        }

        public void Unload() {
            _logger.Warning($"[{_clrMod.PalworldModAttribute.ModName}] Requested Unload.");
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
    }
}
