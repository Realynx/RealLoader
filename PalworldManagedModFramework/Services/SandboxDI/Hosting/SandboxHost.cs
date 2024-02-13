using Microsoft.Extensions.Hosting;

namespace PalworldManagedModFramework.Services.SandboxDI.Hosting {
    public class SandboxHost : IHost {
        public SandboxHost(IServiceProvider serviceProvider) {
            Services = serviceProvider;
        }

        public IServiceProvider Services { get; }

        public Task StartAsync(System.Threading.CancellationToken cancellationToken = default) {
            return Task.CompletedTask;
        }

        public Task StopAsync(System.Threading.CancellationToken cancellationToken = default) {
            return Task.CompletedTask;
        }

        public void Dispose() {

        }
    }
}
