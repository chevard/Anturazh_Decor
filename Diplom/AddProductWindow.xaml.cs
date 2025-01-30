using Diplom.AppData;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Diplom
{
    public partial class AddProductWindow : Window
    {
        public event Action OnProductSaved;
        bool checknew; 
        Products Product; 

        public AddProductWindow(Products product)
        {
            InitializeComponent();
            if (product == null)
            {
                Product = new Products();
                checknew = true;
            }
            else
            {
                Product = product;
                checknew = false; 
            }
            DataContext = Product;

            CategoryCB.ItemsSource = ConDB.context.ProductCategories.ToList();
            SupplierCB.ItemsSource = ConDB.context.Suppliers.ToList();
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (checknew)
            {
                ConDB.context.Products.Add(Product); 
            }

            try
            {
                ConDB.context.SaveChanges(); 
                MessageBox.Show("Сохранено", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                OnProductSaved.Invoke();
                this.Close(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
