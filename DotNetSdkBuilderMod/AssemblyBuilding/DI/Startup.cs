using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using PalworldManagedModFramework.Sdk.Interfaces;

namespace DotNetSdkBuilderMod.AssemblyBuilding.DI {
    public class Startup : ISbStartup {
        public IConfiguration Configuration { get; set; }

        public void Configure(IConfigurationBuilder configurationBuilder) {

        }

        public void ConfigureServices(IServiceCollection services) {
            services
                .AddSingleton<SourceCodeGenerator>();
        }
    }
}
