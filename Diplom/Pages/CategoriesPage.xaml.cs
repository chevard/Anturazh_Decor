using Diplom.AppData;
using System;
using System.Collections.Generic;
using System.Data;
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
    public partial class CategoriesPage : Page
    {
        public CategoriesPage()
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
            var vivod = ConDB.context.ProductCategories.ToList();
            vivod = vivod.Where(x => x.CategoryName.ToLower().Contains(txtSearch.Text.ToLower())).ToList();
            DG.ItemsSource = vivod;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            AddCategoryWindow addCategoryWindow = new AddCategoryWindow(null);
            addCategoryWindow.OnCategorySaved += () => UpdateDB();
            addCategoryWindow.Show();
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            AddCategoryWindow addCategoryWindow = new AddCategoryWindow(DG.SelectedItem as ProductCategories);
            addCategoryWindow.OnCategorySaved += () => UpdateDB();
            addCategoryWindow.Show();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateDB();
        }

        private void DeteleBtn_Click(object sender, RoutedEventArgs e)
        {
            var delCategories = DG.SelectedItems.Cast<ProductCategories>().ToList();

            foreach (var delCategory in delCategories)
            {
                if (ConDB.context.Products.Any(x => x.CategoryID == delCategory.CategoryID))
                {
                    MessageBox.Show("Эта категория используется продуктами. Ее невозможно удалить", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            ConDB.context.ProductCategories.RemoveRange(delCategories);

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
    }
}