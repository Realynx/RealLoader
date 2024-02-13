using DotNetSdkBuilderMod.AssemblyBuilding.Services;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using PalworldManagedModFramework.Sdk.Interfaces;
using PalworldManagedModFramework.Sdk.Logging;

namespace DotNetSdkBuilderMod.AssemblyBuilding.DI {
    public class Startup : ISbStartup {
        public IConfiguration Configuration { get; set; }

        public void Configure(IConfigurationBuilder configurationBuilder) {

        }

        public void ConfigureServices(IServiceCollection services) {
            services
                .AddSingleton<IReflectedGraphBuilder, ReflectedGraphBuilder>()
                .AddSingleton<SourceCodeGenerator>();
        }
    }
}
