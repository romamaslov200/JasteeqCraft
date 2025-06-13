using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp.Core
{
    public static class NavArcCore
    {
        public static Window MainWindow { get; set; }
        public static Frame MainFrame { get; set; }

        private static Stack<Page> backStack = new Stack<Page>();
        private static Stack<Page> forwardStack = new Stack<Page>();

        private static Dictionary<Type, Page> _pageCache = new();

        public static void ChangeFrame<T>() where T : Page, new()
        {
            if (!_pageCache.ContainsKey(typeof(T)))
                _pageCache[typeof(T)] = new T();

            var page = _pageCache[typeof(T)];
            MainFrame.Navigate(page);
        }

        public static void GoBack()
        {
            if (backStack.Count > 0)
            {
                var current = MainFrame.Content as Page;
                if (current != null)
                {
                    forwardStack.Push(current);
                }

                var previousPage = backStack.Pop();
                MainFrame.Navigate(previousPage);
            }
        }

        public static void GoForward()
        {
            if (forwardStack.Count > 0)
            {
                var current = MainFrame.Content as Page;
                if (current != null)
                {
                    backStack.Push(current);
                }

                var nextPage = forwardStack.Pop();
                MainFrame.Navigate(nextPage);
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
*/