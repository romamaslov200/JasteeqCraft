using System;
using Microsoft.Win32;

namespace WpfApp.Core
{
    class JsonController
    {
        private const string RegistryRoot = @"SOFTWARE\JasteeqCraft";

        public Json JsonStart()
        {
            var json = new Json();

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryRoot))
            {
                if (key != null)
                {
                    json.minecraftPath = key.GetValue("minecraftPath", json.minecraftPath)?.ToString();

                    json.minecraftVersionPatch = key.GetValue("MinecraftVersionPatch", json.minecraftVersionPatch)?.ToString();
                    json.launcherVersionPatch = key.GetValue("LauncherVersionPatch", json.launcherVersionPatch)?.ToString();
                    json.vRam = Convert.ToInt32(key.GetValue("VRam", json.vRam));
                    json.Nickname = key.GetValue("Nickname", json.Nickname)?.ToString();
                }
            }

            return json;
        }

        public void JsonSave(Json json)
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(RegistryRoot))
            {
                key.SetValue("minecraftPath", json.minecraftPath);


                key.SetValue("MinecraftVersionPatch", json.minecraftVersionPatch);
                key.SetValue("LauncherVersionPatch", json.launcherVersionPatch);
                key.SetValue("VRam", json.vRam);
                key.SetValue("Nickname", json.Nickname);
            }
        }
    }
}

/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Text.Json;

namespace WpfApp.Core
{
    class JsonController
    {
        private const string nameJsonFile = "json.json";
        
        public Json JsonStart()
        {
            //var Json = new Json();
            var Json = File.Exists(nameJsonFile) ? JsonConvert.DeserializeObject<Json>(File.ReadAllText(nameJsonFile)) : new Json();

            string objectSerialized = System.Text.Json.JsonSerializer.Serialize(Json);
            File.WriteAllText(nameJsonFile, objectSerialized);
            return Json;
        }

        public void JsonSave(Json json)
        {
            File.WriteAllText(nameJsonFile, JsonConvert.SerializeObject(json));
        }
    }
}
*/