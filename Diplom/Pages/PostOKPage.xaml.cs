using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Diplom.Pages
{
    public partial class PostOKPage : Page
    {
        private string selectedPhotoPath;

        public PostOKPage()
        {
            InitializeComponent();
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                selectedPhotoPath = openFileDialog.FileName;
                SelectedImage.Source = new BitmapImage(new Uri(selectedPhotoPath));
            }
        }

        private async void PostBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await PublishPostToOK(TB.Text, selectedPhotoPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private async Task PublishPostToOK(string postText, string photoPath)
        {
            string accessToken = "n-i9J5YyFEokB7q2X3upw1ku7T8kLXh7NPXpzFn12woOPopxj6FakAaFGQHw3LRvrQMOlaT8QRGEpxHYxC0";  // Вставьте свой access_token
            string groupId = "70000033279116";  // ID вашей группы в Одноклассниках

            var client = new HttpClient();

            try
            {
                // Шаг 1: Получение uploadUrl для загрузки фото
                string uploadUrlRequest = $"https://api.ok.ru/fb.do?access_token={accessToken}&method=photos.getUploadServer&gid={groupId}";
                var uploadUrlResponse = await client.GetStringAsync(uploadUrlRequest);

                dynamic uploadResponse = JsonConvert.DeserializeObject(uploadUrlResponse);
                string uploadUrl = uploadResponse.upload_url;

                // Шаг 2: Загрузка фото
                var uploadContent = new MultipartFormDataContent();
                uploadContent.Add(new StreamContent(File.OpenRead(photoPath)), "file", Path.GetFileName(photoPath));
                var uploadResultResponse = await client.PostAsync(uploadUrl, uploadContent);
                var uploadResult = await uploadResultResponse.Content.ReadAsStringAsync();

                dynamic uploadResultData = JsonConvert.DeserializeObject(uploadResult);
                string server = uploadResultData.server;
                string photo = uploadResultData.photo;
                string hash = uploadResultData.hash;

                // Шаг 3: Сохранение фото
                string savePhotoRequest = $"https://api.ok.ru/fb.do?access_token={accessToken}&method=photos.save&server={server}&photo={photo}&hash={hash}";
                var savePhotoResponse = await client.GetStringAsync(savePhotoRequest);

                dynamic savePhotoResult = JsonConvert.DeserializeObject(savePhotoResponse);
                if (savePhotoResult.error != null)
                {
                    MessageBox.Show("Ошибка при сохранении фото.");
                    return;
                }

                // Шаг 4: Публикация поста с фото
                string photoId = savePhotoResult[0].id;
                string attach = $"photo{photoId}";
                string postRequest = $"https://api.ok.ru/fb.do?access_token={accessToken}&method=wall.post&gid={groupId}&message={Uri.EscapeDataString(postText)}&attachment={attach}";
                var postResponse = await client.GetStringAsync(postRequest);

                dynamic postResult = JsonConvert.DeserializeObject(postResponse);
                if (postResult.error != null)
                {
                    MessageBox.Show($"Ошибка публикации поста: {postResult.error.message}");
                }
                else
                {
                    MessageBox.Show("Пост с фото опубликован!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
    }
}
