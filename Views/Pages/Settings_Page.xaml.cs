using CmlLib.Core.Auth.Microsoft;
using CmlLib.Core.Auth.Microsoft.Sessions;
using JasteeqCraft.Core;
using JasteeqCraft.Models;
using Microsoft.VisualBasic.Logging;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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

namespace JasteeqCraft.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для Settings_Page.xaml
    /// </summary>
    public partial class Settings_Page : Page
    {
        private JsonController jsonController = new JsonController();
        private Json ObjectJson;

        //public List<string> NickNameList { get; set; }
        public ObservableCollection<string> NickNameList { get; set; }

        private string TypeNickName;
        private string SelectedNickname;

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


            // Установка начальной темы

            switch (ObjectJson.Theme)
            {
                case "Dark":
                    Dark.IsChecked = true;
                    break;
                case "Pink":
                    Pink.IsChecked = true;
                    break;
            }

            // Загруска ников
            NickNameList = new ObservableCollection<string>(ObjectJson.NicknameList ?? new string[0]);
            this.DataContext = this;

            // выбор сохранённого ника после загрузки
            this.Loaded += (s, e) =>
            {
                foreach (var item in NickNameList)
                {
                    if (item == ObjectJson.Nickname)
                    {
                        // Найти и отметить RadioButton с нужным ником
                        SelectRadioButton(item);
                        break;
                    }
                }
            };
        }

        private void SelectRadioButton(string nick)
        {
            // Найти RadioButton по Content и отметить его
            foreach (var child in FindVisualChildren<RadioButton>(this))
            {
                if (child.Content?.ToString() == nick)
                {
                    child.IsChecked = true;
                    break;
                }
            }
        }
        // Вспомогательный метод для поиска всех RadioButton на странице
        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T t)
                        yield return t;

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                        yield return childOfChild;
                }
            }
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
            ObjectJson = jsonController.JsonStart();

            var dlg = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Title = "Выберите папку",
                InitialDirectory = ObjectJson.minecraftPath
            };

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                try
                {
                    if (Directory.Exists($"{ObjectJson.minecraftPath}\\JasteeqCraftMincraft") && $"{ObjectJson.minecraftPath}\\JasteeqCraftMincraft" != $"{dlg.FileName}\\JasteeqCraftMincraft")
                    {
                        Directory.Delete($"{ObjectJson.minecraftPath}\\JasteeqCraftMincraft", recursive: true);
                    }
                }
                catch(Exception ex)
                {
                    Logs.AddLog($"Error: {ex.Message}");
                }
                ObjectJson.minecraftPath = dlg.FileName;
                jsonController.JsonSave(ObjectJson);
                PatchLable.Text = $"{ObjectJson.minecraftPath}\\JasteeqCraftMincraft";
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var selectedRadio = sender as RadioButton;
            if (selectedRadio != null)
            {
                string selectedTheme = selectedRadio.Name;

                switch (selectedTheme)
                {
                    case "Dark":
                        LauncherControl.SetTheme("Dark");
                        break;
                    case "Pink":
                        LauncherControl.SetTheme("Pink");
                        break;
                }
            }
        }

        private void NickType_RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var selectedRadio = sender as RadioButton;
            if (selectedRadio != null)
            {
                string selectedType = selectedRadio.Name;

                switch (selectedType)
                {
                    case "Microsoft":
                        TypeNickName = "Microsoft";
                        break;
                    case "Обычный":
                        TypeNickName = "Обычный";
                        break;
                }
            }
        }

        private void AccaundAddButton_Click(object sender, RoutedEventArgs e)
        {
            if (TypeNickName == "Обычный" && !string.IsNullOrEmpty(TextBox_AddNick.Text))
            {
                NickNameList.Add(TextBox_AddNick.Text);
                ObjectJson.NicknameList = NickNameList.ToArray();
                jsonController.JsonSave(ObjectJson);
            }
            else if (TypeNickName == "Microsoft")
            {
                var loginHandler = JELoginHandlerBuilder.BuildDefault();
                var session = loginHandler.AuthenticateInteractively();
                var accaunts = loginHandler.AccountManager.GetAccounts();
                foreach (var account in accaunts)
                {
                    if (account is not JEGameAccount jEGameAccount)
                        continue;

                    var username = jEGameAccount.Profile?.Username;
                    if (!string.IsNullOrEmpty(username))
                    {
                        NickNameList.Add(username);
                    }
                }
                ObjectJson.NicknameList = NickNameList.ToArray();
                jsonController.JsonSave(ObjectJson);
            }
        }

        private void NickRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb && rb.Content is string nickname)
            {
                SelectedNickname = nickname;
                ObjectJson.Nickname = nickname;
                jsonController.JsonSave(ObjectJson);
            }
        }

        private void AccaundRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(SelectedNickname) && NickNameList.Contains(SelectedNickname))
            {
                NickNameList.Remove(SelectedNickname);
                ObjectJson.NicknameList = NickNameList.ToArray();
                jsonController.JsonSave(ObjectJson);

                // Обновить ItemsControl
                this.DataContext = null;      // "Сбросить" привязку
                this.DataContext = this;      // "Перезагрузить" привязку
            }
        }

    }
}
