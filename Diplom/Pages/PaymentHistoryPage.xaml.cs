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
    /// Логика взаимодействия для PaymentHistoryPage.xaml
    /// </summary>
    public partial class PaymentHistoryPage : Page
    {
        public PaymentHistoryPage()
        {
            InitializeComponent();
            var paymentHistory = ConDB.context.OrderDetails.ToList();
            LVPaymentHistory.ItemsSource = paymentHistory;

            decimal totalSum = paymentHistory.Sum(x => x.Total ?? 0);
            TotalSumTB.Text = totalSum.ToString("N0") + " ₽";

            TransactionsCountTB.Text = paymentHistory.Count.ToString();

            decimal averageCheck = totalSum / paymentHistory.Count;
            AverageCheckTB.Text = averageCheck.ToString("N0") + " ₽";
        }



    }
}

