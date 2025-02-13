using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices;

using Avalonia.Controls;

using DynamicData;

using ReactiveUI;

using RealLoaderGuiInstaller.Services;

namespace RealLoaderGuiInstaller {
    public partial class MainWindow : Window, IReactiveObject {
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        private string _selectedGamePath;

        public event PropertyChangingEventHandler? PropertyChanging;

        public ObservableCollection<string> AvailableGamePaths { get; } = new();

        public string SelectedGamePath {
            get => _selectedGamePath;
            set => this.RaiseAndSetIfChanged(ref _selectedGamePath, value);
        }

        public MainWindow() {
            InitializeComponent();

            var installedGames = UEGameSearch.FindUEGames();
            AvailableGamePaths.AddRange(installedGames);

            DataContext = this;
        }

        public void Install(string gamePath) {
            AllocConsole();

            Console.WriteLine($"Installing RealLoader to '{gamePath}'");
            RealLoaderInstaller.Program.Main(["-l", $"{gamePath}"]);
        }

        public void InstallButton() {
            Install(SelectedGamePath);
        }

        public void RaisePropertyChanging(PropertyChangingEventArgs args) {
            throw new System.NotImplementedException();
        }

        public void RaisePropertyChanged(PropertyChangedEventArgs args) {
            throw new System.NotImplementedException();
        }
    }
}