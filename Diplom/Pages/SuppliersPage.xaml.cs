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
    /// Логика взаимодействия для SuppliersPage.xaml
    /// </summary>
    public partial class SuppliersPage : Page
    {
        Suppliers Supplier;
        bool checknew;
        public SuppliersPage()
        {

            InitializeComponent();
         
        }
        void UpdateDB()
        {
            var vivod = ConDB.context.Suppliers.ToList();
            vivod = vivod.Where(x => x.SupplierName.ToLower().Contains(txtSearch.Text.ToLower())).ToList();
            DG.ItemsSource = vivod;
        }
        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            AddSuppliersWindow addSuppliersWindow = new AddSuppliersWindow(null);
            addSuppliersWindow.OnSupplierSaved += () => UpdateDB();
            addSuppliersWindow.Show();
        }
        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            AddSuppliersWindow AddSuppliersWindow = new AddSuppliersWindow(DG.SelectedItem as Suppliers);
            AddSuppliersWindow.OnSupplierSaved += () => UpdateDB();
            AddSuppliersWindow.Show();
        }


        private void DeteleBtn_Click(object sender, RoutedEventArgs e)
        {
            var delCategories = DG.SelectedItems.Cast<Suppliers>().ToList();

            foreach (var delCategory in delCategories)
            {
                if (ConDB.context.Products.Any(x => x.SupplierID == delCategory.SupplierID))
                {
                    MessageBox.Show("Данный поставщик используется в товарах. Его невозможно удалить", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            ConDB.context.Suppliers.RemoveRange(delCategories);

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
