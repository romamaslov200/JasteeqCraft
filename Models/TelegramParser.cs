using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JasteeqCraft.Models
{
    public class TelegramPost
    {
        public string Id => $"{Date}-{Text?.GetHashCode()}";
        public string Date { get; set; }
        public string Text { get; set; }
        public string Views { get; set; }
        public string FirstImageUrl => ImageUrls?.FirstOrDefault();
        public List<string> ImageUrls { get; set; } = new List<string>();
        public string VideoUrl { get; set; }
    }

    public class TelegramParser
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private const string BaseUrl = "https://t.me/s/";



        public async Task<List<TelegramPost>> ParseLastPostsWithPhotos(string channelName, int count = 100)
        {
            var url = $"{BaseUrl}{channelName}";
            var postsList = new List<TelegramPost>();

            try
            {
                var html = await _httpClient.GetStringAsync(url);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                /*
                var allPosts = doc.DocumentNode.SelectNodes("//div[contains(@class, 'tgme_widget_message')]");
                MessageBox.Show($"Всего постов найдено: {allPosts?.Count ?? 0}");
                var photoPosts = allPosts?.Where(node => node.SelectSingleNode(".//a[contains(@class, 'tgme_widget_message_photo_wrap')]") != null).ToList();
                MessageBox.Show($"Постов с фото: {photoPosts?.Count ?? 0}");
                */

                var postsWithPhotos = doc.DocumentNode
                    .SelectNodes("//div[contains(@class, 'tgme_widget_message')][.//a[contains(@class, 'tgme_widget_message_photo_wrap')]]")?
                    .Reverse()
                    .Take(count);

                if (postsWithPhotos == null)
                    return postsList;

                foreach (var node in postsWithPhotos)
                {
                    var textNode = node.SelectSingleNode(".//div[contains(@class, 'tgme_widget_message_text')]");

                    var post = new TelegramPost
                    {
                        Date = node.SelectSingleNode(".//time[@datetime]")?.GetAttributeValue("datetime", ""),
                        Text = GetFullText(textNode)?.Trim(),
                        Views = node.SelectSingleNode(".//span[contains(@class, 'tgme_widget_message_views')]")?.InnerText
                    };

                    var firstPhotoNode = node.SelectSingleNode(".//a[contains(@class, 'tgme_widget_message_photo_wrap')]");
                    if (firstPhotoNode != null)
                    {
                        var style = firstPhotoNode.GetAttributeValue("style", "");
                        var urlStart = style.IndexOf("url('") + 5;
                        var urlEnd = style.IndexOf("')", urlStart);
                        if (urlStart > 0 && urlEnd > urlStart)
                        {
                            post.ImageUrls.Add(style.Substring(urlStart, urlEnd - urlStart));
                        }
                    }

                    postsList.Add(post);
                }

                return postsList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка парсинга: {ex.Message}");
                return postsList;
            }
        }

        private string GetFullText(HtmlNode node)
        {
            if (node == null)
                return string.Empty;

            var sb = new StringBuilder();

            foreach (var child in node.ChildNodes)
            {
                if (child.Name == "br")
                {
                    sb.AppendLine();
                }
                else if (child.NodeType == HtmlNodeType.Text)
                {
                    // Добавляем текст, декодируя HTML сущности (например, &amp; → &)
                    sb.Append(WebUtility.HtmlDecode(child.InnerText));
                }
                else
                {
                    // Рекурсивно обрабатываем вложенные элементы
                    sb.Append(GetFullText(child));
                }
            }

            return sb.ToString();
        }


        public async Task<TelegramPost> ParseLastPostWithPhoto(string channelName)
        {
            var url = $"{BaseUrl}{channelName}";

            try
            {
                var html = await _httpClient.GetStringAsync(url);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                // Ищем все посты с фотографиями (в обратном порядке)
                var postsWithPhotos = doc.DocumentNode
                    .SelectNodes("//div[contains(@class, 'tgme_widget_message')][.//a[contains(@class, 'tgme_widget_message_photo_wrap')]]")?
                    .Reverse(); // Сначала новые посты

                if (postsWithPhotos == null)
                    return null;

                // Берем первый пост с фото (самый новый)
                var lastMessageWithPhoto = postsWithPhotos.FirstOrDefault();
                if (lastMessageWithPhoto == null)
                    return null;

                var post = new TelegramPost
                {
                    Date = lastMessageWithPhoto.SelectSingleNode(".//time[@datetime]")?.GetAttributeValue("datetime", ""),
                    Text = lastMessageWithPhoto.SelectSingleNode(".//div[contains(@class, 'tgme_widget_message_text')]")?.InnerText.Trim(),
                    Views = lastMessageWithPhoto.SelectSingleNode(".//span[contains(@class, 'tgme_widget_message_views')]")?.InnerText
                };

                // Парсим только первую фотографию (если нужно все - оставьте как было)
                var firstPhotoNode = lastMessageWithPhoto.SelectSingleNode(".//a[contains(@class, 'tgme_widget_message_photo_wrap')]");
                if (firstPhotoNode != null)
                {
                    var style = firstPhotoNode.GetAttributeValue("style", "");
                    var urlStart = style.IndexOf("url('") + 5;
                    var urlEnd = style.IndexOf("')", urlStart);
                    if (urlStart > 0 && urlEnd > urlStart)
                    {
                        post.ImageUrls.Add(style.Substring(urlStart, urlEnd - urlStart));
                    }
                }

                return post;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка парсинга: {ex.Message}");
                return null;
            }
        }


        public async Task<List<TelegramPost>> ParseChannelWithMedia(string channelName, int maxPosts = 20)
        {
            var posts = new List<TelegramPost>();
            var url = $"{BaseUrl}{channelName}";

            try
            {
                var html = await _httpClient.GetStringAsync(url);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                var messageNodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'tgme_widget_message')]");

                if (messageNodes != null)
                {
                    foreach (var node in messageNodes.Take(maxPosts))
                    {
                        var post = new TelegramPost
                        {
                            Date = node.SelectSingleNode(".//time[@datetime]")?.GetAttributeValue("datetime", ""),
                            Text = node.SelectSingleNode(".//div[contains(@class, 'tgme_widget_message_text')]")?.InnerText.Trim(),
                            Views = node.SelectSingleNode(".//span[contains(@class, 'tgme_widget_message_views')]")?.InnerText
                        };

                        // Парсинг изображений
                        var photoNodes = node.SelectNodes(".//a[contains(@class, 'tgme_widget_message_photo_wrap')]");
                        if (photoNodes != null)
                        {
                            foreach (var photoNode in photoNodes)
                            {
                                var style = photoNode.GetAttributeValue("style", "");
                                var urlStart = style.IndexOf("url('") + 5;
                                var urlEnd = style.IndexOf("')", urlStart);
                                if (urlStart > 0 && urlEnd > urlStart)
                                {
                                    post.ImageUrls.Add(style.Substring(urlStart, urlEnd - urlStart));
                                }
                            }
                        }

                        // Парсинг видео
                        var videoNode = node.SelectSingleNode(".//video[contains(@class, 'tgme_widget_message_video')]");
                        if (videoNode != null)
                        {
                            post.VideoUrl = videoNode.GetAttributeValue("src", "");
                        }

                        posts.Add(post);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка парсинга: {ex.Message}");
            }

            return posts;
        }
    }
}
