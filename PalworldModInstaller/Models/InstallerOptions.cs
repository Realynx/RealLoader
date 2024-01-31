using System.ComponentModel;
using System.Text;

using Spectre.Console;
using Spectre.Console.Cli;

namespace PalworldModInstaller.Models {
    internal class InstallerOptions : CommandSettings {
        [CommandOption("-l|--location")]
        [DefaultValue("C:\\Program Files (x86)\\Steam\\steamapps\\common\\Palworld")]
        [Description("This is the directory that your game is installed.")]
        public string? InstallLocation { get; set; }

        [CommandOption("-u|--update")]
        [DefaultValue(true)]
        [Description("This will allow you to update the mod loader in place.")]
        public bool CheckUpdates { get; set; }

        [CommandOption("-r|--uninstall")]
        [DefaultValue(false)]
        [Description("This flag will allow you to uninstall the mod loader and restore the state of your game (THIS WILL DELETE ALL OF YOUR MODS).")]
        public bool Uninstall { get; set; }

        [CommandOption("-b|--backup")]
        [Description("This will backup all of your mods to the specified directory.")]
        public string? Backup { get; set; }

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

            var aggregateErrorMessage = errorMessageBuilder.ToString();
            if (string.IsNullOrWhiteSpace(aggregateErrorMessage)) {
                return ValidationResult.Success();
            }

            return ValidationResult.Error(aggregateErrorMessage);
        }
    }
}
