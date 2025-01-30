using Diplom.AppData;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Diplom.Pages
{
    public partial class ProfilePage : Page
    {
        private Users userr;
        private MainP mainWindow;
        private bool isEditing = false;

        public ProfilePage(Users user, MainP mainWindow)
        {
            InitializeComponent();
            userr = user;
            this.mainWindow = mainWindow;

            // Инициализация полей
            FullNameTB.Text = userr.FullName;
            LoginTB.Text = userr.UserName;
            RoleTB.Text = userr.Roles.RoleName;
            PasswordTB.Text = userr.PasswordHash;
            pic.ImageSource = new BitmapImage(new Uri(userr.PhotoPath, UriKind.RelativeOrAbsolute));

            // Установка начального состояния кнопки смены фото
            var changePhotoButton = this.FindName("ChangePhotoButton") as Button;
            if (changePhotoButton != null)
                changePhotoButton.IsEnabled = false;
        }

     
        private void ChangePhoto_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, находимся ли мы в режиме редактирования
            if (!isEditing)
                return;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";
            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;
                pic.ImageSource = new BitmapImage(new Uri(selectedFilePath, UriKind.RelativeOrAbsolute));
                userr.PhotoPath = selectedFilePath;
            }
        }

        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            isEditing = !isEditing;

            // Управление доступностью полей
            FullNameTB.IsReadOnly = !isEditing;
            LoginTB.IsReadOnly = !isEditing;
            PasswordTB.IsReadOnly = !isEditing;

            // Особая логика для поля роли
            if (userr.Roles.RoleName == "Администратор")
            {
                RoleTB.IsReadOnly = !isEditing;
            }
            else
            {
                RoleTB.IsReadOnly = true;
            }

            // Управление видимостью кнопки сохранения
            SaveButton.Visibility = isEditing ? Visibility.Visible : Visibility.Collapsed;

            // Управление доступностью кнопки смены фото
            var changePhotoButton = this.FindName("ChangePhotoButton") as Button;
            if (changePhotoButton != null)
                changePhotoButton.IsEnabled = isEditing;

            // Изменение текста кнопки редактирования
            EditProfileButton.Content = isEditing ? "Отмена" : "Редактировать";
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var user = ConDB.context.Users.FirstOrDefault(u => u.UserID == userr.UserID);
                if (user != null)
                {
                    user.FullName = FullNameTB.Text;
                    user.UserName = LoginTB.Text;
                    user.Roles.RoleName = RoleTB.Text;
                    user.PasswordHash = PasswordTB.Text;
                    user.PhotoPath = userr.PhotoPath;

                    ConDB.context.SaveChanges();
                    MessageBox.Show("Изменения сохранены успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    mainWindow.UpdateUserInfo(user);
                    Nav.MainFrame.GoBack();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении изменений: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}