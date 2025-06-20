﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace WpfApp.Core
{
    public class Json
    {
        public string minecraftPath { get; set; } = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}";

        public string minecraftVersionPatch { get; set; } = "";
        public string launcherVersionPatch { get; set; } = "0.2";

        public int vRam { get; set; } = 2000;

        public string Nickname { get; set; }
        public string[] NicknameList { get; set; }
        //public ObservableCollection<string> NickNameList { get; set; }

        public double TotalMinutesPlayed { get; set; } = 0;

        public string Theme { get; set; } = "Dark";
    }
}