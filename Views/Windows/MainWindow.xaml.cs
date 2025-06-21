using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.ProcessBuilder;
using JasteeqCraft.Core;
using JasteeqCraft.Models;
using JasteeqCraft.Views.Pages;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using WpfApp.Core;

namespace JasteeqCraft
{
    public partial class MainWindow : Window
    {
        //
        private JsonController jsonController = new JsonController();
        private Json ObjectJson;

        private Updater updater = new Updater();

        public MainWindow()
        {
            ObjectJson = jsonController.JsonStart();
            LauncherControl.SetTheme(ObjectJson.Theme);

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://195.133.88.43");
                request.Timeout = 5000; // таймаут 5 секунд
                request.Method = "GET";

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 400)
                    {
                        // Сайт доступен
                        //MessageBox.Show("Сайт доступен.");
                    }
                    else
                    {
                        MessageBox.Show($"Сервер обновлений не доступен");
                        Environment.Exit(0);
                    }
                }
            }
            catch (WebException ex)
            {
                MessageBox.Show($"Сервер обновлений не доступен");
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Сервер обновлений не доступен");
                Environment.Exit(0);
            }

            InitializeComponent();

            NavArcCore.MainWindow = this;
            NavArcCore.MainFrame = MainFrame;
            //NavArcCore.MainFrame.NavigationService.Navigate(new Home_Page());
            //NavArcCore.ChangeFrame(new Home_Page());
            NavArcCore.ChangeFrame<Home_Page>();

            updater.start();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void Button_Cloase_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Button_Folding_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (MainFrame.Content is Page page)
            {
                //NavTitle.Content = page.Title;
                this.Title = page.Title;
            }
        }



        private void Button_PageHome_Click(object sender, RoutedEventArgs e)
        {
            //NavArcCore.ChangeFrame(new Home_Page());
            NavArcCore.ChangeFrame<Home_Page>();
        }

        private void Button_PageSettings_Click(object sender, RoutedEventArgs e)
        {
            //NavArcCore.ChangeFrame(new Settings_Page());
            NavArcCore.ChangeFrame<Settings_Page>();
        }

        private void Button_PageInfo_Click(object sender, RoutedEventArgs e)
        {
            //NavArcCore.ChangeFrame(new Settings_Page());
            NavArcCore.ChangeFrame<info_Page>();
        }
        private void Button_WebSite_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "http://195.133.88.43/1/",
                UseShellExecute = true
            });
        }
        /*
        private async void LaunchButton_Click(object sender, RoutedEventArgs e)
        {
            LaunchButton.IsEnabled = false;
            StatusText.Text = "Начинаем загрузку...";
            ProgressBarDownload.Value = 0;

            try
            {
                string verPatch = await LauncherControl.GetMinecraftVersion();
                MessageBox.Show($"Версия Minecraft на сервере: {verPatch}\nВерсия Minecraft на устройстве: {ObjectJson.minecraftVersionPatch}");

                if (verPatch != ObjectJson.minecraftVersionPatch)
                {
                    await LauncherControl.DownloadMinecraft(ProgressBarDownload);

                    StatusText.Text = "Распаковка завершена. Подготовка к запуску...";

                    ObjectJson.minecraftVersionPatch = verPatch;
                    jsonController.JsonSave(ObjectJson);
                }

                var path = new MinecraftPath(Path.Combine(Directory.GetCurrentDirectory(), "MinecraftSborks-main"));
                var launcher = new MinecraftLauncher(path);

                var versions = await launcher.GetAllVersionsAsync();

                if (versions.Count() == 0)
                {
                    StatusText.Text = "Версии Minecraft не найдены!";
                    return;
                }

                var launchOption = new MLaunchOption
                {
                    Session = MSession.CreateOfflineSession(Nick.Text),
                    MaximumRamMb = 2048,
                    GameLauncherName = "JasteeqCraft",
                    ServerIp = "hcplay.ru"
                };

                var process = await launcher.CreateProcessAsync(versions[0].Name, launchOption);
                process.Start();

                StatusText.Text = $"Запущен Minecraft {versions[0].Name}";
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
        */
    }
}