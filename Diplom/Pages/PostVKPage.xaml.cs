using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;

namespace Diplom.Pages
{
    /// <summary>
    /// Логика взаимодействия для PostVKPage.xaml
    /// </summary>
    public partial class PostVKPage : Page
    {

        private string selectedPhotoPath;
        public PostVKPage()
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
                await PublishPostToVK(TB.Text, selectedPhotoPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private async Task PublishPostToVK(string postText, string photoPath)
        {
            string accessToken = "vk1.a.liv3JTrkmaEbcImluiUTN-lThtgskwfFsjBZCZKH2oJHN8uQwztV47DdE_BjQLXEyVgQpVnCu6pBy9j2bUYz-4DYLuE9I9nnOnImhnesyMYibazdrj4Gn040qvb6yK3BXf2eyfozc88aURU2xr-hzkZJIJzJ8fFEoLI0m2GA6UR-xcUOSQoe_cgHSKilXNfiimS8IX1tzVUS3HX9pvH-wg"; //https://oauth.vk.com/authorize?client_id=YOUR_APP_ID&display=page&redirect_uri=https://oauth.vk.com/blank.html&scope=wall,photos,groups,offline&response_type=token&v=5.131
            string groupId = "227902276";

            var client = new HttpClient();

            var uploadServerResponse = await client.GetStringAsync($"https://api.vk.com/method/photos.getWallUploadServer?group_id={groupId}&access_token={accessToken}&v=5.131");
            string uploadUrl = ParseUploadUrl(uploadServerResponse);  //получаем url для загрузки фото


            var uploadContent = new MultipartFormDataContent();  //загрузка контента на сервер
            uploadContent.Add(new StreamContent(File.OpenRead(photoPath)), "photo", System.IO.Path.GetFileName(photoPath));
            var uploadResponse = await client.PostAsync(uploadUrl, uploadContent); // запрос на загрузку фото на сервер

            var uploadResult = await uploadResponse.Content.ReadAsStringAsync();  //результаты загрузки фотки
            var uploadResultData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(uploadResult);

            var savePhotoResponse = await client.GetStringAsync(
                $"https://api.vk.com/method/photos.saveWallPhoto?group_id={groupId}&photo={uploadResultData.photo}&server={uploadResultData.server}&hash={uploadResultData.hash}&access_token={accessToken}&v=5.131"
            ); //сохранение на сервер после загрузки

            var savePhotoResult = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(savePhotoResponse);
            if (savePhotoResult.response == null || savePhotoResult.response.Count == 0)
            {
                MessageBox.Show("Ошибка при сохранении фото.");
                return;
            }

            //получаем id фото для публикации
            string photoId = savePhotoResult.response[0].id;
            string ownerId = savePhotoResult.response[0].owner_id;

            //публикуем пост с текстом и фоткой
            var publishResponse = await client.GetStringAsync($"https://api.vk.com/method/wall.post?owner_id=-{groupId}&message={postText}&attachments=photo{ownerId}_{photoId}&access_token={accessToken}&v=5.131");

            //проверка, опубликовалось ли
            dynamic jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject(publishResponse);
            if (jsonResponse.error != null)
            {
                MessageBox.Show($"Ошибка публикации поста: {jsonResponse.error.message}");
            }
            else
            {
                MessageBox.Show("Пост с фото опубликован!");
            }
        }



        //загрузка url для загрузки фото
        private string ParseUploadUrl(string response)
        {
            dynamic jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject(response);
            if (jsonResponse.response == null || jsonResponse.response.upload_url == null)
            {
                throw new Exception("Не удалось получить upload_url из ответа");
            }
            return jsonResponse.response.upload_url;
        }
        
        //данные о фото
        private string ParsePhoto(string response)
        {
            dynamic jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject(response);
            if (jsonResponse.photo == null)
            {
                throw new Exception("Не удалось получить фото");
            }
            return jsonResponse.photo;
        }



    }
}

