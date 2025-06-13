using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shell;
using WpfApp.Core;

namespace JasteeqCraft.Models
{
    public class Updater
    {
        private JsonController jsonController = new JsonController();
        private Json ObjectJson;


        public async void start()
        {
            string url = "http://194.87.239.214/JasteeqCraft/launcher/launcherVersionPatch.txt";

            string version = "";

            ObjectJson = jsonController.JsonStart();

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    version = await client.GetStringAsync(url);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }

            if(version != ObjectJson.launcherVersionPatch)
            {
                string downloadUrl = "http://194.87.239.214/JasteeqCraft/launcher/updater.exe";
                
                MessageBox.Show($"Сейчас вы обновитесь с версии {ObjectJson.launcherVersionPatch} на {version}", "Обновлени лаунчера", MessageBoxButton.OK, MessageBoxImage.Information);

                using (HttpClient client = new HttpClient())
                {
                    var data = await client.GetByteArrayAsync(downloadUrl);
                    File.WriteAllBytes("updater.exe", data);
                }

                // Запуск скачанного файла
                Process.Start(new ProcessStartInfo
                {
                    FileName = "updater.exe",
                    UseShellExecute = true
                });

                ObjectJson.launcherVersionPatch = version;
                jsonController.JsonSave(ObjectJson);
            }
        }
    }
}
