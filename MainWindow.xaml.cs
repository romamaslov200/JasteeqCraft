using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Installer.Forge;
using CmlLib.Core.Installers;
using CmlLib.Core.ProcessBuilder;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace JasteeqCraft
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void LaunchButton_Click(object sender, RoutedEventArgs e)
        {
            string mcpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".minecraft");
            Directory.CreateDirectory(mcpath);

            var path = new MinecraftPath(mcpath);
            var launcher = new MinecraftLauncher(path);

            launcher.FileProgressChanged += UpdateProgress;

            // Установка базовой версии Minecraft
            await launcher.InstallAsync("1.12.2");

            // Установка Forge
            var forgeInstaller = new ForgeInstaller(launcher);

            await forgeInstaller.Install("1.12.2", "14.23.5.2860");

            // Запуск Minecraft с установленным Forge
            var minecraftProcess = await launcher.CreateProcessAsync("1.12.2-forge-14.23.5.2860", new MLaunchOption
            {
                Session = MSession.CreateOfflineSession(Nick.Text),
                MaximumRamMb = 2048,
                GameLauncherName = "JasteeqCraft",
                ServerIp = "hcplay.ru"
            });

            MessageBox.Show("START");
            minecraftProcess.Start();
        }

        private void UpdateProgress(object sender, InstallerProgressChangedEventArgs e)
        {
            int percent = e.ProgressedTasks;
            if (percent < 0) percent = 0;
            if (percent > 100) percent = 100;
            StatusText.Text = $"Downloading {e.Name} ({percent}%)";
        }
    }
}
