using System;
using System.IO;
using System.Windows;

namespace JasteeqCraft
{
    public partial class App : Application
    {
        public static string IpServer = "195.133.88.43";
        protected override void OnStartup(StartupEventArgs e)
        {
            // Обработчик необработанных исключений
            AppDomain.CurrentDomain.UnhandledException += (s, ex) =>
            {
                File.WriteAllText("crash.log", ex.ExceptionObject.ToString());
            };

            DispatcherUnhandledException += (s, ex) =>
            {
                File.WriteAllText("crash_ui.log", ex.Exception.ToString());
                ex.Handled = true; // чтобы не падало сразу
            };

            base.OnStartup(e);
        }
    }
}
