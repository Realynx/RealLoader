﻿using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Compile;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.GraphBuilders;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using RealLoaderFramework.Sdk.Interfaces;

namespace DotNetSdkBuilderMod.AssemblyBuilding.DI {
    public class Startup : ISbStartup {
        public IConfiguration Configuration { get; set; }

        public void Configure(string assemblyFolder, IConfigurationBuilder configurationBuilder) {
            configurationBuilder
                .AddJsonFile(Path.Combine(assemblyFolder, "SdkBuilderConfig.json"));
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
                .AddSingleton<INameCollisionService, NameCollisionService>()
                .AddSingleton<INameSpaceService, NameSpaceService>()
                .AddSingleton<IImportResolver, ImportResolver>()

                .AddSingleton<ICodeGenAttributeNodeFactory, CodeGenAttributeNodeFactory>()
                .AddSingleton<ICodeGenMethodNodeFactory, CodeGenMethodNodeFactory>()
                .AddSingleton<ICodeGenPropertyNodeFactory, CodeGenPropertyNodeFactory>()
                .AddSingleton<ICodeGenConstructorNodeFactory, CodeGenConstructorNodeFactory>()
                .AddSingleton<ICodeGenOperatorNodeFactory, CodeGenOperatorNodeFactory>()
                .AddSingleton<ICodeGenInterfaceNodeFactory, CodeGenInterfaceNodeFactory>()
                .AddSingleton<ICodeGenClassNodeFactory, CodeGenClassNodeFactory>()
                .AddSingleton<ICodeGenGraphBuilder, CodeGenGraphBuilder>()
                .AddSingleton<IUObjectInteropExtensionsBuilder, UObjectInteropExtensionsBuilder>()
                .AddSingleton<ICodeCompilerFactory, CodeCompilerFactory>()
                .AddSingleton<ISourceCodeGenerator, SourceCodeGenerator>()

                .AddSingleton<IArgumentGenerator, ArgumentGenerator>()
                .AddSingleton<IAttributeGenerator, AttributeGenerator>()
                .AddSingleton<IFileGenerator, FileGenerator>()
                .AddSingleton<IClassGenerator, ClassGenerator>()
                .AddSingleton<IConstructorGenerator, ConstructorGenerator>()
                .AddSingleton<IPropertyGenerator, PropertyGenerator>()
                .AddSingleton<IMethodGenerator, MethodGenerator>()
                .AddSingleton<IOperatorGenerator, OperatorGenerator>()
                .AddSingleton<IInterfaceGenerator, InterfaceGenerator>()
                .AddSingleton<IGenericGenerator, GenericGenerator>()
                .AddSingleton<IXmlTagGenerator, XmlTagGenerator>()
                .AddSingleton<IProjectGenerator, ProjectGenerator>()
                .AddSingleton<ISolutionGenerator, SolutionGenerator>();
        }
    }
}
