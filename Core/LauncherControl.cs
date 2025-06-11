using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Net;

namespace JasteeqCraft.Core
{
    public static class LauncherControl
    {
        public static WebClient WClient = new WebClient();

        public static async Task<string> GetMinecraftVersion()
        {
            string url = "https://raw.githubusercontent.com/romamaslov200/MinecraftSborks/refs/heads/main/minecraftVersionPatch.text";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string content = await client.GetStringAsync(url);
                    return content;
                }
                catch (Exception ex)
                {
                    return $"Ошибка: {ex.Message}";
                }
            }
        }

        public static Task DownloadMinecraft(ProgressBar bar)
        {
            var tcs = new TaskCompletionSource<object>();

            string user = "romamaslov200";
            string repo = "MinecraftSborks";
            string branch = "main";

            string zipUrl = $"https://github.com/{user}/{repo}/archive/refs/heads/{branch}.zip";
            string zipFileName = $"{repo}.zip";
            string extractPath = Directory.GetCurrentDirectory();

            WebClient client = new WebClient();

            // Инициализация прогресс-бара
            bar.Dispatcher.Invoke(() =>
            {
                bar.IsIndeterminate = false;
                bar.Minimum = 0;
                bar.Maximum = 100;
                bar.Value = 0;
            });

            client.DownloadProgressChanged += (s, e) =>
            {
                // Обновляем прогресс через Dispatcher
                bar.Dispatcher.BeginInvoke(() =>
                {
                    bar.Value = e.ProgressPercentage;
                });
            };

            client.DownloadFileCompleted += async (s, e) =>
            {
                if (e.Error != null)
                {
                    tcs.TrySetException(e.Error);
                    return;
                }

                if (e.Cancelled)
                {
                    tcs.TrySetCanceled();
                    return;
                }

                try
                {
                    // Распаковка
                    await Task.Run(() =>
                    {
                        if (File.Exists(zipFileName))
                        {
                            ZipFile.ExtractToDirectory(zipFileName, extractPath, true);
                            File.Delete(zipFileName); // Удаляем архив
                        }
                    });

                    // Установка финального значения
                    bar.Dispatcher.Invoke(() =>
                    {
                        bar.Value = 100;
                    });

                    tcs.TrySetResult(null);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            };

            try
            {
                client.DownloadFileAsync(new Uri(zipUrl), zipFileName);
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }

            return tcs.Task;
        }


    }
}
