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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Diplom.Pages
{
    /// <summary>
    /// Логика взаимодействия для EmployeesPage.xaml
    /// </summary>
    public partial class EmployeesPage : Page
    {
        Users Employee;
        bool checknew;
        public EmployeesPage(Users p = null)
        {

            InitializeComponent();
           
            if (p == null)
            {
                Employee = new Users();
                checknew = true;
            }
            else
            {
                Employee = ConDB.context.Users.SingleOrDefault(prod => prod.UserID == p.UserID);
                checknew = false;
            }

            DataContext = Employee;
        }
        void UpdateDB()
        {
            var vivod = ConDB.context.Users.ToList();
            vivod = vivod.Where(x => x.FullName.ToLower().Contains(txtSearch.Text.ToLower())).ToList();
            DG.ItemsSource = vivod;
        }
        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            AddUserWindow addUserWindow = new AddUserWindow(null);
            addUserWindow.OnEmployeeSaved += () => UpdateDB();
            addUserWindow.Show();
        }
        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            AddUserWindow addUserWindow = new AddUserWindow(DG.SelectedItem as Users);
            addUserWindow.OnEmployeeSaved += () => UpdateDB();
            addUserWindow.Show();
        }


        private void DeteleBtn_Click(object sender, RoutedEventArgs e)
        {
            var delCategories = DG.SelectedItems.Cast<Users>().ToList();

            foreach (var delCategory in delCategories)
            {
                if (ConDB.context.Users.Any(x => x.UserID == delCategory.UserID))
                {
                    MessageBox.Show("Удаление невозможно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            ConDB.context.Users.RemoveRange(delCategories);

            try
            {
                if (delCategories.Any())
                {
                    ConDB.context.SaveChanges();
                    MessageBox.Show("Данные удалены!");
                    UpdateDB();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void DG_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateDB();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateDB();
        }

    }
}
