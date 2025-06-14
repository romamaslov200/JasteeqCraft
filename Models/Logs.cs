using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JasteeqCraft.Models
{
    public static class Logs
    {
        public static void AddLog(string text)
        {
            if (!File.Exists("Logs.log")) 
            {
                File.Create("Logs.log");
            }

            try
            {
                File.AppendAllText("Logs.log", $"\n{text}");
            }
            catch { }
        }
    }
}
