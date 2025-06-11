using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp.Core
{
    public static class NavArcCore
    {
        public static Window MainWindow { get; set; }

        public static Frame MainFrame { get; set; }
        public static Page MainPage { get; set; }
        public static Page LastFrame { get; set; }
        public static Page NextFrame { get; set; }

        public static void ChageFrame(Page Page)
        {
            if(LastFrame != MainFrame.Content as Page)
            {
                LastFrame = MainFrame.Content as Page;
            }
            MainFrame.Navigate(Page);
        }

        public static void GoBack() 
        {
            if(LastFrame != null && LastFrame != MainFrame.Content as Page)
            {
                NextFrame = MainFrame.Content as Page;
                MainFrame.Navigate(LastFrame);
            }
        }

        public static void GoNext()
        {
            if(NextFrame != null && NextFrame != MainFrame.Content as Page)
            {
                LastFrame = MainFrame.Content as Page;
                MainFrame.Navigate(NextFrame);
            }
        }
    }
}
