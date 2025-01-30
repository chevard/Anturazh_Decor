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
    /// Логика взаимодействия для AddSuppliersWindow.xaml
    /// </summary>
    public partial class AddSuppliersWindow : Window
    {
        public event Action OnSupplierSaved;
        Suppliers Supplier;
        bool checknew;
        public AddSuppliersWindow(Suppliers sup)
        {
            InitializeComponent();
            if (sup == null)
            {
                Supplier = new Suppliers();
                checknew = true;
            }
            else
            {
                Supplier = sup;
                checknew = false;
            }

            DataContext = Supplier;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (checknew)
            {
                ConDB.context.Suppliers.Add(Supplier);
            }

            try
            {
                ConDB.context.SaveChanges();
                MessageBox.Show("Сохранено", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                OnSupplierSaved.Invoke();
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
