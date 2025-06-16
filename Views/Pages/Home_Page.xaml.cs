using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Installer.Forge;
using CmlLib.Core.ProcessBuilder;
using JasteeqCraft.Core;
using JasteeqCraft.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    
    public class NullToVisibilityConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return string.IsNullOrEmpty(value as string) ? Visibility.Collapsed : Visibility.Visible;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

    public partial class Home_Page : Page
    {
        private JsonController jsonController = new JsonController();
        private Json ObjectJson;

        private readonly TelegramParser _parser = new TelegramParser();

        public Home_Page()
        {
            InitializeComponent();
            ObjectJson = jsonController.JsonStart();
            //DownloadForge();
            this.KeepAlive = true; // 

            Nick.Text = ObjectJson.Nickname;
            PostLoad();
            UpdateServerStatus();
            TotalMinutesLable_Loaded();
        }

        public void TotalMinutesLable_Loaded()
        {
            double totalMinutes = Math.Round(ObjectJson.TotalMinutesPlayed, 1);
            
            if (totalMinutes > 60)
            {
                TotalMinutesLable.Content = $"{Math.Round(ObjectJson.TotalMinutesPlayed/60, 1)} ч.";
            }
            else
            {
                TotalMinutesLable.Content = $"{Math.Round(ObjectJson.TotalMinutesPlayed, 1)} м.";
            }
        }

        private async void UpdateServerStatus()
        {
            string serverIp = "54.37.238.70:25587";
            string status = await LauncherControl.CheckStatusAsync(serverIp);
            OnlineLable.Content = status;
            
            double width = this.ActualWidth;
            double height = this.ActualHeight;
            //MessageBox.Show($"width={width};\nheight={height}");
        }

        private async void PostLoad()
        {
            var channelName = "testanall";
            if (string.IsNullOrEmpty(channelName)) return;

            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                var lastPostWithPhoto = await _parser.ParseLastPostWithPhoto(channelName);
                PostsList.ItemsSource = lastPostWithPhoto != null ? new List<TelegramPost> { lastPostWithPhoto } : null;

                if (lastPostWithPhoto == null)
                {
                    //MessageBox.Show("Не найдено постов с фотографиями", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }


        public static async Task DownloadForge()
        {
            string mcpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MinecraftSborks");
            Directory.CreateDirectory(mcpath);

            var path = new MinecraftPath(mcpath);
            var launcher = new MinecraftLauncher(path);

            //await launcher.InstallAsync("1.12.2");

            // Установка Forge
            var forgeInstaller = new ForgeInstaller(launcher);

            await forgeInstaller.Install("1.20.1", "47.3.0");

            // Запуск Minecraft с установленным Forge
            var minecraftProcess = await launcher.CreateProcessAsync("1.20.1-forge-47.3.0", new MLaunchOption
            {
                Session = MSession.CreateOfflineSession("Nick"),
                MaximumRamMb = 2048,
                GameLauncherName = "JasteeqCraft",
                ServerIp = "hcplay.ru"
            });

            MessageBox.Show("START");
            minecraftProcess.Start();
        }
        
        private async void LaunchButton_Click(object sender, RoutedEventArgs e)
        {
            ObjectJson = jsonController.JsonStart();

            LaunchButton.IsEnabled = false;
            StatusText.Text = "Подготовка";
            ProgressBarDownload.Value = 0;

            try
            {
                string verPatch = await LauncherControl.GetMinecraftVersion();
                ObjectJson = jsonController.JsonStart();
                //MessageBox.Show(verPatch);
                //MessageBox.Show($"Версия Minecraft на сервере: {verPatch}\nВерсия Minecraft на устройстве: {ObjectJson.minecraftVersionPatch}");
                //MessageBox.Show($"{ObjectJson.minecraftPath}\\JasteeqCraftMincraft");
                //MessageBox.Show(Directory.Exists($"{ObjectJson.minecraftPath}\\JasteeqCraftMincraft").ToString());
                if (verPatch != ObjectJson.minecraftVersionPatch || !Directory.Exists($"{ObjectJson.minecraftPath}\\JasteeqCraftMincraft"))
                {
                    await LauncherControl.DownloadMinecraft(ProgressBarDownload, StatusText);


                    StatusText.Text = "Распаковка завершена. Подготовка к запуску...";

                    ObjectJson.minecraftVersionPatch = verPatch;
                    jsonController.JsonSave(ObjectJson);
                }
                var path = new MinecraftPath(Path.Combine(ObjectJson.minecraftPath, "JasteeqCraftMincraft"));

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
                var forgeVersion = versions.FirstOrDefault(v => v.Name.Contains("forge-47.3.0") && v.Name.Contains("1.20.1"));
                if (forgeVersion == null)
                {
                    StatusText.Text = "Forge 1.20.1 не найдён!";
                    return;
                }



                var launchOption = new MLaunchOption
                {
                    Session = MSession.CreateOfflineSession(Nick.Text),
                    MaximumRamMb = ObjectJson.vRam,
                    GameLauncherName = "JasteeqCraft",
                    ServerIp = "54.37.238.70:25587"
                };

                var process = await launcher.CreateProcessAsync(forgeVersion.Name, launchOption);
                process.Start();

                //StatusText.Text = $"Запущен Minecraft {forgeVersion.Name}";
                StatusText.Text = $"Запуск";

                DateTime startTime = DateTime.Now;
                await process.WaitForExitAsync(); // Exit Jdom
                DateTime endTime = DateTime.Now;

                TimeSpan playedTime = endTime - startTime;

                SavePlayedTimeToFile(Nick.Text, playedTime);
            }
            catch (Exception ex)
            {
                StatusText.Text = "Ошибка при запуске!";
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Logs.AddLog($"{ex.Message}");
            }
            finally
            {
                LaunchButton.IsEnabled = true;
            }
        }

        private void Nick_TextChanged(object sender, TextChangedEventArgs e)
        {
            ObjectJson.Nickname = Nick.Text;
            jsonController.JsonSave(ObjectJson);
        }

        void SavePlayedTimeToFile(string nickname, TimeSpan sessionTime)
        {
            double totalMinutes = ObjectJson.TotalMinutesPlayed;

            totalMinutes += sessionTime.TotalMinutes;

            ObjectJson.TotalMinutesPlayed = totalMinutes;
            jsonController.JsonSave(ObjectJson);
            TotalMinutesLable_Loaded();
        }

    }
}
