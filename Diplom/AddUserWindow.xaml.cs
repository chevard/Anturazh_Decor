using Diplom.AppData;
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

namespace Diplom
{
    /// <summary>
    /// Логика взаимодействия для AddUserWindow.xaml
    /// </summary>
    public partial class AddUserWindow : Window
    {
        public event Action OnEmployeeSaved;
        bool checknew;
        Users Employee;

        public AddUserWindow(Users employee)
        {
            InitializeComponent();

            if (employee == null)
            {
                Employee = new Users();
                checknew = true;
            }
            else
            {
                Employee = employee;
                checknew = false;
            }

            DataContext = Employee;
            RoleCB.ItemsSource = ConDB.context.Roles.ToList();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (checknew)
            {
                ConDB.context.Users.Add(Employee);
            }

            try
            {
                ConDB.context.SaveChanges();
                MessageBox.Show("Сохранено", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                OnEmployeeSaved?.Invoke();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
