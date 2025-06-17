using System;
using System.IO;
using System.Windows;

namespace JasteeqCraft // или другое имя пространства имён
{
    public partial class App : Application
    {
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
