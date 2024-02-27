using System.Net.Http.Headers;
using System.Reflection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Polly;
using Polly.Extensions.Http;

using Serilog;

using UnrealCoreObjectApiSourceGen.Models;
using UnrealCoreObjectApiSourceGen.Services;
using UnrealCoreObjectApiSourceGen.Services.Interfaces;
using UnrealCoreObjectApiSourceGen.Services.SourceGen;

namespace UnrealCoreObjectApiSourceGen.DI {
    internal static class Startup {
        private const string UNREAL_ENGINE_SOURCE = "https://raw.githubusercontent.com/EpicGames/UnrealEngine/";


        internal static IConfigurationRoot Configuration { get; set; }

        internal static void Configure(HostBuilderContext context, IConfigurationBuilder configurationBuilder) {
            configurationBuilder
               .AddJsonFile("AppSettings.json", false)
               .AddUserSecrets(Assembly.GetExecutingAssembly());


        }

        internal static void ConfigureServices(HostBuilderContext context, IServiceCollection services) {
            services
                .AddHttpClient<PrivateGithubReader>(client => {
                    client.BaseAddress = new Uri(UNREAL_ENGINE_SOURCE);

                    var githubConfig = new GithubConfig(Configuration);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", githubConfig.AccessToken);
                    client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");

                    client.DefaultRequestHeaders.UserAgent.Clear();
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("ManagedModWorks_SourceGen/1.0 (Windows; Linux; https://github.com/PoofImaFox/PalworldManagedModFramework)");
                })
                .AddPolicyHandler(GetRetryPolicy());


            services.AddLogging(loggingBuilder => {
                loggingBuilder.ClearProviders()
                .SetMinimumLevel(LogLevel.Trace)
                .AddSerilog()
                .AddConsole();
            });

            services
                .AddSingleton(Log.Logger)
                .AddSingleton<IPrivateGithubReader, PrivateGithubReader>()
                .AddSingleton<NativeCodeParser>();
        }


        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy() {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }
    }
}
