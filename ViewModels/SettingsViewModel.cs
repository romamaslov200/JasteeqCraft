using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Management;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WpfApp.Core;

namespace JasteeqCraft.ViewModels
{
    public class SettingsViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly JsonController jsonController = new JsonController();
        public Json ObjectJson { get; set; }

        private List<string> _nickNameList;
        public List<string> NickNameList
        {
            get => _nickNameList;
            set { _nickNameList = value; OnPropertyChanged(); }
        }

        private string _patchLabelText;
        public string PatchLabelText
        {
            get => _patchLabelText;
            set { _patchLabelText = value; OnPropertyChanged(); }
        }

        private string _vramLabelText;
        public string VramLabelText
        {
            get => _vramLabelText;
            set { _vramLabelText = value; OnPropertyChanged(); }
        }

        private double _vramMax;
        public double VramMax
        {
            get => _vramMax;
            set { _vramMax = value; OnPropertyChanged(); }
        }

        private double _vramValue;
        public double VramValue
        {
            get => _vramValue;
            set { _vramValue = value; OnPropertyChanged(); }
        }

        public SettingsViewModel()
        {
            ObjectJson = jsonController.JsonStart();

            PatchLabelText = $"{ObjectJson.minecraftPath}\\JasteeqCraftMincraft";
            VramLabelText = $"Оперативная память: {ObjectJson.vRam} MB";

            ulong totalMemory = 0;
            var searcher = new ManagementObjectSearcher("SELECT Capacity FROM Win32_PhysicalMemory");
            foreach (ManagementObject obj in searcher.Get())
                totalMemory += (ulong)obj["Capacity"];

            VramMax = totalMemory / (1024 * 1024);
            VramValue = ObjectJson.vRam;

            NickNameList = new List<string>
            {
                "Вариант 1", "Вариант 2", "Вариант 3", "Вариант 4", "Вариант 5"
            };
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
