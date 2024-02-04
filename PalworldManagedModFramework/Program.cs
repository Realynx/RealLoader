﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using PalworldManagedModFramework.DI;
using PalworldManagedModFramework.PalWorldSdk.Logging;
using PalworldManagedModFramework.Services.AssemblyLoading.Interfaces;
using PalworldManagedModFramework.Services.MemoryScanning;

namespace PalworldManagedModFramework {
    internal static class Program {

        internal delegate void VoidDelegateSignature();
        public static void EntryPoint() {
            try {
                AppDomainMonitor.MonitorDomain();

                Console.WriteLine($"Loading .NET DI Service Container...");

                var hostBuilder = new HostBuilder()
                    .UseConsoleLifetime();

                // Call startup functions to configure DI Container.
                hostBuilder.ConfigureAppConfiguration(Startup.Configure);
                hostBuilder.ConfigureAppConfiguration((_, config) => Startup.Configuration = config.Build());
                hostBuilder.ConfigureServices(Startup.ConfigureServices);

                var host = hostBuilder
                    .Build();

                var loggerInstance = host.
                    Services.GetRequiredService<ILogger>();

                loggerInstance.Info("DI Container Setup!");


                // TODO: Move this into it's own function for setting up all the runtime scanning.
                var reflectionScanner = host.Services.GetRequiredService<UReflectionPointerScanner>();
                reflectionScanner.ScanMemoryForUnrealReflectionPointers();

                var modLoader = host.Services.GetRequiredService<IModLoader>();
                modLoader.LoadMods();

                for (; ; )
                    Console.ReadLine();
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
        }
    }
}
