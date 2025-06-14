using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management;
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
using System.Windows.Shapes;
using WpfApp.Core;

using Microsoft.WindowsAPICodePack.Dialogs;


namespace JasteeqCraft.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для Settings_Page.xaml
    /// </summary>
    public partial class Settings_Page : Page
    {
        private JsonController jsonController = new JsonController();
        private Json ObjectJson;

        public Settings_Page()
        {
            InitializeComponent();
            ObjectJson = jsonController.JsonStart();
            this.KeepAlive = true; // Типо для сохранения состояния страници но работает и без этого

            PatchLable.Text = $"{ObjectJson.minecraftPath}\\JasteeqCraftMincraft";
            Vram_Lable.Content = $"Оперативная память: {ObjectJson.vRam} MB";

            ulong totalMemory = 0;
            var searcher = new ManagementObjectSearcher("SELECT Capacity FROM Win32_PhysicalMemory");
            foreach (ManagementObject obj in searcher.Get())
            {
                totalMemory += (ulong)obj["Capacity"];
            }

            VramSlider.Maximum = totalMemory / (1024 * 1024);
            VramSlider.Value = ObjectJson.vRam;
        }

        private void VramSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double newValue = e.NewValue;
            Console.WriteLine($"Новое значение VRAM: {newValue}");
            Vram_Lable.Content = $"Оперативная память: {newValue} MB";

            if ((int)VramSlider.Value < 500)
            {
                VramSlider.Value = 500;
            }
            ObjectJson.vRam = (int)VramSlider.Value;
            jsonController.JsonSave(ObjectJson);
        }

        private async void MinecraftPatch(object sender, RoutedEventArgs e)
        {
            var dlg = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Title = "Выберите папку",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
            };

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                ObjectJson.minecraftPath = dlg.FileName;
                jsonController.JsonSave(ObjectJson);
                PatchLable.Text = $"{ObjectJson.minecraftPath}\\JasteeqCraftMincraft";
            }
        }
    }
}
