using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.ProcessBuilder;
using JasteeqCraft.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using WpfApp.Core;

namespace JasteeqCraft.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для Home_Page.xaml
    /// </summary>
    public partial class Home_Page : Page
    {
        private JsonController jsonController = new JsonController();
        private Json ObjectJson;

        public Home_Page()
        {
            InitializeComponent();
            ObjectJson = jsonController.JsonStart();
        }

        private async void LaunchButton_Click(object sender, RoutedEventArgs e)
        {
            LaunchButton.IsEnabled = false;
            StatusText.Text = "Начинаем загрузку...";
            ProgressBarDownload.Value = 0;

            try
            {
                string verPatch = await LauncherControl.GetMinecraftVersion();
                MessageBox.Show(verPatch);
                MessageBox.Show($"Версия Minecraft на сервере: {verPatch}\nВерсия Minecraft на устройстве: {ObjectJson.minecraftVersionPatch}");

                if (verPatch != ObjectJson.minecraftVersionPatch)
                {
                    await LauncherControl.DownloadMinecraft(ProgressBarDownload, StatusText);


                    StatusText.Text = "Распаковка завершена. Подготовка к запуску...";

                    ObjectJson.minecraftVersionPatch = verPatch;
                    jsonController.JsonSave(ObjectJson);
                }
                var path = new MinecraftPath(Path.Combine(Directory.GetCurrentDirectory(), "MinecraftSborks"));

                //var path = new MinecraftPath(Path.Combine(Directory.GetCurrentDirectory(), "MinecraftSborks-main"));
                //MessageBox.Show(path.Versions);
                var launcher = new MinecraftLauncher(path);

                var versions = await launcher.GetAllVersionsAsync();
                /*
                if (versions.Count() == 0)
                {
                    StatusText.Text = "Версии Minecraft не найдены!";
                    return;
                }
                */
                
                //var forgeVersion = versions.FirstOrDefault(v => v.Name.Contains("forge") && v.Name.Contains("1.20.1"));
                var forgeVersion = versions.FirstOrDefault(v => v.Name.Contains("Forge 1.20.1"));
                if (forgeVersion == null)
                {
                    StatusText.Text = "Forge 1.20.1 не найдён!";
                    return;
                }



                var launchOption = new MLaunchOption
                {
                    Session = MSession.CreateOfflineSession(Nick.Text),
                    MaximumRamMb = 12288,
                    GameLauncherName = "JasteeqCraft",
                    ServerIp = "54.37.238.70:25587"
                };

                var process = await launcher.CreateProcessAsync(forgeVersion.Name, launchOption);
                process.Start();

                StatusText.Text = $"Запущен Minecraft {forgeVersion.Name}";
            }
            catch (Exception ex)
            {
                StatusText.Text = "Ошибка при запуске!";
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                LaunchButton.IsEnabled = true;
            }
        }
    }
}
