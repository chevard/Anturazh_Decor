using Diplom.AppData;
using Diplom.Pages;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Diplom
{
    /// <summary>
    /// Логика взаимодействия для MainP.xaml
    /// </summary>
    public partial class MainP : Window
    {

        private Users user;
        private DispatcherTimer _notificationTimer;
        private List<Notification> notifications = new List<Notification>();
        public MainP(Users users)
        {
            InitializeComponent();
            Nav.MainFrame = MFrame;
            Nav.MainFrame.Navigate(new MainPage(users));
            if (users != null)
            {
                UpdateUserInfo(users);
            }
            _notificationTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2) 
            };
            _notificationTimer.Tick += NotificationTimer_Tick;
            _notificationTimer.Start();
        }

        private void NotificationTimer_Tick(object sender, EventArgs e)
        {
            CheckLowStock();
        }

        public void UpdateUserInfo(Users updatedUser)
        {
            user = updatedUser;
            Usernamee.Text = updatedUser.FullName;
            pic.ImageSource = new BitmapImage(new Uri(user.PhotoPath, UriKind.RelativeOrAbsolute));
        }
        private void UpdateBellButtonStyle()
        {
            // Если есть непрочитанные уведомления, меняем фон кнопки
            if (notifications.Any(n => !n.IsRead))
            {
                BellIcon.Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.BellAlert;
            }
            else
                 BellIcon.Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.Bell;
        }
        private void AddNotification(string message)
        {
            var newNotification = new Notification(message);
            notifications.Add(newNotification);
            UpdateBellButtonStyle();
        }

        private void CheckLowStock()
        {
            var lowStockProducts = ConDB.context.Products.Where(p => p.StockQuantity < 4).ToList();
            foreach (var product in lowStockProducts)
            {
                string message = $"Количество товара '{product.ProductName}' меньше 4";

                // Проверяем, нет ли такого уведомления
                if (!notifications.Any(n => n.Message == message))
                {
                    AddNotification(message);
                    ToastNotificationWindow.ShowToast(message);
                }
            }

            UpdateBellButtonStyle(); // Обновляем стиль кнопки колокольчика
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void ProductsPageBtn_Click(object sender, RoutedEventArgs e)
        {
            Nav.MainFrame.Navigate(new ProductsPage());
        }

        private void CategoriesBtn_Click(object sender, RoutedEventArgs e)
        {
            Nav.MainFrame.Navigate(new CategoriesPage());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BellBtn_Click(object sender, RoutedEventArgs e)
        {
            Nav.MainFrame.Navigate(new NotificationsPage(notifications));

            // Помечаем все уведомления как прочитанные
            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

            UpdateBellButtonStyle();
        }

        private void PaymentPageBtn_Click(object sender, RoutedEventArgs e)
        {
            Nav.MainFrame.Navigate(new PaymentPage(user));
        }

        private void MailPageBtn_Click(object sender, RoutedEventArgs e)
        {
            Nav.MainFrame.Navigate(new EmailPage());
        }

        private void PostVKBtn_Click(object sender, RoutedEventArgs e)
        {
            Nav.MainFrame.Navigate(new PostVKPage());
        }

        private void ProfilePageBtn_Click(object sender, RoutedEventArgs e)
        {
            Nav.MainFrame.Navigate(new ProfilePage(user, this));
        }

        private void HistoryPageBtn_Click(object sender, RoutedEventArgs e)
        {
            Nav.MainFrame.Navigate(new PaymentHistoryPage());
        }

        private void EmployeesPageBtn_Click(object sender, RoutedEventArgs e)
        {
            Nav.MainFrame.Navigate(new EmployeesPage());
        }

        private void CaterogoriesgeBtn_Click(object sender, RoutedEventArgs e)
        {
            Nav.MainFrame.Navigate(new CategoriesPage());
        }

        private void SuppliersPageBtn_Click(object sender, RoutedEventArgs e)
        {
            Nav.MainFrame.Navigate(new SuppliersPage());    
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void HomeBtn_Click(object sender, RoutedEventArgs e)
        {
            Nav.MainFrame.Navigate(new MainPage(user));
        }

        private void PostOK_Click(object sender, RoutedEventArgs e)
        {
            Nav.MainFrame.Navigate(new PostOKPage());
        }
    }
}
