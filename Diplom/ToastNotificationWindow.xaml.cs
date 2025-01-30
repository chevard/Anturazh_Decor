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
    /// Логика взаимодействия для ToastNotificationWindow.xaml
    /// </summary>
    public partial class ToastNotificationWindow : Window
    {
        public ToastNotificationWindow(string message)
        {
            InitializeComponent();
            NotificationText.Text = message;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += (sender, args) =>
            {
                timer.Stop();
                this.Close(); 
            };
            timer.Start();
        }

        public static void ShowToast(string message)
        {
            var toastWindow = new ToastNotificationWindow(message)
            {
                Left = SystemParameters.WorkArea.Right - 350,
                Top = SystemParameters.WorkArea.Bottom - 150  
            };
            toastWindow.Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

