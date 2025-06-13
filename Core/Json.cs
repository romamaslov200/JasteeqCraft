using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace WpfApp.Core
{
    class Json
    {
        public string minecraftVersionPatch { get; set; } = "";
        public string launcherVersionPatch { get; set; } = "0.3";

        public int vRam { get; set; } = 2000;

        public string Nickname { get; set; } = "Nickname";
    }
}