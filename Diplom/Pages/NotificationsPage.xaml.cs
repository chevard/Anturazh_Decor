using Diplom.AppData;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Diplom.Pages
{
    public partial class NotificationsPage : Page
    {
        private List<Notification> notifications;

        public NotificationsPage(List<Notification> notifications)
        {
            InitializeComponent();
            this.notifications = notifications;
            NotificationsListView.ItemsSource = notifications; // Привязка списка уведомлений
            NotificationsListView.Items.Refresh();  // Обновление отображения
        }
    }
}
