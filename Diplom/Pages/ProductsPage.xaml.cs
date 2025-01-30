using Diplom.AppData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;


namespace Diplom.Pages
{
    public partial class ProductsPage : Page
    {

        Products Product;
        bool checknew;

        public ProductsPage (Products p = null)
        {
            InitializeComponent();
            UpdateDB();
        }

        private void DG_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateDB();
        }

        void UpdateDB()
        {
            var vivod = ConDB.context.Products.ToList();
            vivod = vivod.Where(x => x.ProductName.ToLower().Contains(txtSearch.Text.ToLower())).ToList();
            DG.ItemsSource = vivod;
        }

        private void DeteleBtn_Click(object sender, RoutedEventArgs e)
        {
            var delProducts = DG.SelectedItems.Cast<Products>().ToList();

            foreach (var delProduct in delProducts)
            {
                if (ConDB.context.OrderDetails.Any(x => x.ProductID == delProduct.ProductID))
                {
                    MessageBox.Show("Данный товар используется в заказе. Его невозможно удалить", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            ConDB.context.Products.RemoveRange(delProducts);

            try
            {
                    ConDB.context.SaveChanges();
                    MessageBox.Show("Данные удалены!");
                    UpdateDB();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);  
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            AddProductWindow addProductWindow = new AddProductWindow(null);
            addProductWindow.OnProductSaved += () => UpdateDB();
            addProductWindow.Show();
        }


        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            AddProductWindow addProductWindow = new AddProductWindow(DG.SelectedItem as Products);
            addProductWindow.OnProductSaved += () => UpdateDB();
            addProductWindow.Show();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateDB();
        }
    }
}
