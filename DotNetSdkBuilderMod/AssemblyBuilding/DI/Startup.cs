using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.GraphBuilders;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using PalworldManagedModFramework.Sdk.Interfaces;

namespace DotNetSdkBuilderMod.AssemblyBuilding.DI {
    public class Startup : ISbStartup {
        public IConfiguration Configuration { get; set; }

        public void Configure(string assemblyFolder, IConfigurationBuilder configurationBuilder) {
            configurationBuilder
                .AddJsonFile(Path.Combine(assemblyFolder, "SdkBuilderConfig.json"));

            Configuration = configurationBuilder.Build();
        }

        public void ConfigureServices(IServiceCollection services) {
            services
                .AddSingleton<SdkBuilderConfig>()
                .AddSingleton<IReflectedGraphBuilder, ReflectedGraphBuilder>();

            services
                .AddSingleton<IFunctionTimingService, FunctionTimingService>();

            services
                .AddSingleton<IPackageNameGenerator, PackageNameGenerator>()
                .AddSingleton<INameDistanceService, NameDistanceService>()
                .AddSingleton<INameSpaceService, NameSpaceService>()
                .AddSingleton<IImportResolver, ImportResolver>()

                .AddSingleton<ICodeGenGraphBuilder, CodeGenGraphBuilder>()
                .AddSingleton<ISourceCodeGenerator, SourceCodeGenerator>()

                .AddSingleton<IFileGenerator, FileGenerator>()
                .AddSingleton<IClassGenerator, ClassGenerator>()
                .AddSingleton<IPropertyGenerator, PropertyGenerator>()
                .AddSingleton<IMethodGenerator, MethodGenerator>()
                .AddSingleton<IOperatorGenerator, OperatorGenerator>();
        }
    }
}
