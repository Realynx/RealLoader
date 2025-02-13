using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RealLoaderFramework.Sdk.Interfaces {
    public interface ISbStartup {

        /// <summary>
        /// This is filled before <see cref="ConfigureServices"/> is called.
        /// </summary>
        IConfiguration Configuration { get; set; }

        /// <summary>
        /// This gets called before your class is constructed. You may use services setup here in your class constructor.
        /// </summary>
        /// <param name="services"></param>
        void ConfigureServices(IServiceCollection services);

        void Configure(string assemblyFolder, IConfigurationBuilder configurationBuilder);
    }
}
