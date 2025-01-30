using MimeKit;
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
using MailKit.Net.Smtp;

namespace Diplom.Pages
{
    /// <summary>
    /// Логика взаимодействия для EmailPage.xaml
    /// </summary>
    public partial class EmailPage : Page
    {
        public EmailPage()
        {
            InitializeComponent();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string subject = SubjectTB.Text;
            string message = MessageTB.Text;

            if (string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show("Пожалуйста, заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress("Продавец", "tatyana.tchevardova@yandex.ru"));
                email.To.Add(new MailboxAddress("Администратор", "ttaverna1973@yandex.ru"));
                email.Subject = subject;

                email.Body = new TextPart("plain")
                {
                    Text = message
                };

                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.yandex.ru", 465, true);
                    client.Authenticate("tatyana.tchevardova@yandex.ru", "phmpxjpdgawfukrz");
                    client.Send(email);
                    client.Disconnect(true);
                }
                MessageBox.Show("Сообщение успешно отправлено!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                SubjectTB.Text = string.Empty;
                MessageTB.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отправке письма: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
