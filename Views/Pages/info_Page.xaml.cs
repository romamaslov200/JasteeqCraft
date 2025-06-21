using JasteeqCraft.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

namespace JasteeqCraft.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для info_Page.xaml
    /// </summary>
    public partial class info_Page : Page
    {

        public ObservableCollection<TelegramPost> TelegramPosts { get; set; } = new();
        public info_Page()
        {
            InitializeComponent();
            PostsListBox.ItemsSource = TelegramPosts;
            Load_post();
        }

        private async void Load_post()
        {
            TelegramPosts.Clear();

            try
            {
                var parser = new TelegramParser();
                var posts = await parser.ParseLastPostsWithPhotos("testanall");

                if (posts.Count == 0)
                {
                    TelegramPosts.Add(new TelegramPost { Text = "Посты не найдены." });
                }
                else
                {
                    foreach (var post in posts)
                        TelegramPosts.Add(post);
                }
            }
            catch (Exception ex)
            {
                TelegramPosts.Add(new TelegramPost { Text = "Ошибка: " + ex.Message });
            }

        }

    }
}
