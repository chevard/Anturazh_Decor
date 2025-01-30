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
    public partial class AddCategoryWindow : Window
    {
        public event Action OnCategorySaved;
        bool checknew;
        ProductCategories Category;

        public AddCategoryWindow(ProductCategories category)
        {
            InitializeComponent();
            if (category == null)
            {
                Category = new ProductCategories();
                checknew = true;
            }
            else
            {
                Category = category;
                checknew = false;
            }
            DataContext = Category;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveButton_Click_1(object sender, RoutedEventArgs e)
        {
            if (checknew)
            {
                ConDB.context.ProductCategories.Add(Category);
            }
            try
            {
                ConDB.context.SaveChanges();
                MessageBox.Show("Категория добавлена/изменена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                OnCategorySaved?.Invoke();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
