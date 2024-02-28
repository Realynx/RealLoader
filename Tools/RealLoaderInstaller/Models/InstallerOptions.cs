using System.ComponentModel;
using System.Text;

using Spectre.Console;
using Spectre.Console.Cli;

namespace RealLoaderInstaller.Models {
    public class InstallerOptions : CommandSettings {
        [CommandOption("-r|--remote")]
        [DefaultValue("https://github.com/PoofImaFox/RealLoader")]
        [Description("The remote repository to find update artifacts.")]
        public string RemoteSource { get; set; }

        [CommandOption("-l|--location")]
        [DefaultValue("C:\\Program Files (x86)\\Steam\\steamapps\\common\\Palworld")]
        [Description("This is the directory that your game is installed.")]
        public string InstallLocation { get; set; }

        [CommandOption("-u|--update")]
        [DefaultValue(true)]
        [Description("This will allow you to update the mod loader in place.")]
        public bool CheckUpdates { get; set; }

        [CommandOption("--uninstall")]
        [DefaultValue(false)]
        [Description("This flag will allow you to uninstall the mod loader and restore the state of your game (THIS WILL DELETE ALL OF YOUR MODS).")]
        public bool Uninstall { get; set; }

        [CommandOption("-p|--proxylib")]
        [DefaultValue("winhttp.dll")]
        [Description("The proxy dll variant you would like to use.")]
        public string ProxyDll { get; set; }

        [CommandOption("-b|--backup")]
        [Description("This will backup all of your mods to the specified directory.")]
        public string Backup { get; set; }

        [CommandOption("-m|--mods")]
        [DefaultValue(true)]
        [Description("This will allow you top prevent the installer from creating the mods folder by default. (Set to false to disable)")]
        public bool CreateModsFolder { get; set; }

        public override ValidationResult Validate() {
            var errorMessageBuilder = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(InstallLocation) && !Directory.Exists(InstallLocation)) {
                errorMessageBuilder.AppendLine("Specified install location does not exist!");
            }

            if (!string.IsNullOrWhiteSpace(Backup) && !Directory.Exists(Backup)) {
                errorMessageBuilder.AppendLine("Backup location does not exist!");
            }

            if (string.IsNullOrWhiteSpace(RemoteSource)) {
                errorMessageBuilder.AppendLine("Remote source cannot be empty!");
            }

            var aggregateErrorMessage = errorMessageBuilder.ToString();
            if (string.IsNullOrWhiteSpace(aggregateErrorMessage)) {
                return ValidationResult.Success();
            }

            return ValidationResult.Error(aggregateErrorMessage);
        }
    }
}
