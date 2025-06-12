using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Net;
using System.Net.Http.Headers;

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

        public static async Task DownloadMinecraft(ProgressBar bar, TextBlock statusText = null)
        {
            string user = "romamaslov200";
            string repo = "MinecraftSborks";
            string branch = "main";

            //string zipUrl = $"https://github.com/{user}/{repo}/archive/refs/heads/{branch}.zip";
            //string zipUrl = $"https://github.com/{user}/{repo}/archive/{branch}.zip";
            string zipUrl = $"http://194.87.239.214/MinecraftSborks.zip";


            string zipFileName = $"{repo}.zip";
            string extractPath = Directory.GetCurrentDirectory();

            using (var httpClient = new HttpClient())
            using (var response = await httpClient.GetAsync(zipUrl, HttpCompletionOption.ResponseHeadersRead))
            {
                var rs = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, zipUrl));
                if (rs.Content.Headers.ContentLength.HasValue)
                    MessageBox.Show("Размер файла: " + rs.Content.Headers.ContentLength.Value + " байт");
                else
                    MessageBox.Show("Content-Length отсутствует!");



                response.EnsureSuccessStatusCode();

                var contentLength = response.Content.Headers.ContentLength;
                bool canReportProgress = contentLength.HasValue;
                long totalBytes = contentLength ?? -1;

                using (var contentStream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream(zipFileName, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                {
                    var buffer = new byte[8192];
                    long totalRead = 0;
                    int bytesRead;

                    bar.Dispatcher.Invoke(() =>
                    {
                        bar.IsIndeterminate = !canReportProgress;
                        bar.Minimum = 0;
                        bar.Maximum = 100;
                        bar.Value = 0;
                    });

                    while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, bytesRead);
                        totalRead += bytesRead;

                        if (canReportProgress)
                        {
                            double progressPercent = (double)totalRead / totalBytes * 100;
                            double mbRead = totalRead / 1024.0 / 1024.0;
                            double mbTotal = totalBytes / 1024.0 / 1024.0;

                            bar.Dispatcher.Invoke(() =>
                            {
                                bar.Value = progressPercent;
                            });

                            if (statusText != null)
                            {
                                statusText.Dispatcher.Invoke(() =>
                                {
                                    //statusText.Text = $"Загружено: {mbRead:F1} МБ / {mbTotal:F1} МБ ({progressPercent:F1}%)";
                                    statusText.Text = $"{mbRead:F1} МБ / {mbTotal:F1} МБ ({progressPercent:F1}%)";
                                });
                            }
                        }
                    }
                }
            }

            // Распаковка
            if (File.Exists(zipFileName))
            {
                ZipFile.ExtractToDirectory(zipFileName, extractPath, true);
                File.Delete(zipFileName);
            }

            // Завершение
            bar.Dispatcher.Invoke(() =>
            {
                bar.Value = 100;
            });

            if (statusText != null)
            {
                statusText.Dispatcher.Invoke(() =>
                {
                    statusText.Text = $"Загрузка завершена (100%)";
                });
            }
        }




    }
}
