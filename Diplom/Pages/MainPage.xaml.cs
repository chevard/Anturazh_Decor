using Diplom.AppData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using System.Data.Entity;
using LiveCharts.Wpf;
using LiveCharts;
using System.Windows.Threading;

namespace Diplom.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        private Users user;
        private DispatcherTimer timer;
        public MainPage(Users users)
        {
            InitializeComponent();
            DaysChart();
            InitializeTimeUpdater();
            ProductOfToday();
            UpdateUserInfo(users);
            MonthsChart();
        }


        public void UpdateUserInfo(Users updatedUser)
        {
            user = updatedUser;
            GoodDayTB.Text = $"Добро пожаловать, {updatedUser.FullName}!";
        }

        private void DaysChart()
        {
            DateTime today = DateTime.Today;
            DateTime monday = today;
            while (monday.DayOfWeek != DayOfWeek.Monday)
            {
                monday = monday.AddDays(-1);
            }
            DateTime sunday = monday.AddDays(7);
            var sales = ConDB.context.OrderDetails.Where(x => x.PaymentDate >= monday && x.PaymentDate <= sunday).Select(x => new
            {
                x.PaymentDate,
                x.Quantity,
                x.Total
            }).ToList();

            var salesByDay = sales.GroupBy(y => ((DateTime)y.PaymentDate).DayOfWeek).Select(g => new
            {
                day = g.Key,
                salesCount = g.Sum(o => o.Quantity),
                salesTotal = g.Sum(o => o.Total)
            }).ToList();

            var salesCountByDay = new int[7];
            var salesTotalByDay = new decimal[7];

            for (int i = 0; i < 7; i++)
            {
                salesCountByDay[i] = 0;
                salesTotalByDay[i] = 0;
            }

            foreach (var sale in salesByDay)
            {
                int dayIndex = ((int)sale.day - 1);
                if (sale.day == DayOfWeek.Sunday)
                {
                    dayIndex = 6;
                }
                salesCountByDay[dayIndex] = sale.salesCount.GetValueOrDefault();
                salesTotalByDay[dayIndex] = (decimal)sale.salesTotal;
            }
            Chartt.Series = new SeriesCollection
            {

                new ColumnSeries
                {
                    Title = "Количество товаров продано",
                    Values = new ChartValues<int>(salesCountByDay),
                    Fill = new SolidColorBrush(Colors.IndianRed)
                },
                new ColumnSeries
        
                {
                    Title = "Сумма продаж",
                    Values = new ChartValues<decimal>(salesTotalByDay),
                    Fill = new SolidColorBrush(Colors.DarkRed)
                }
            };
        }
        private void MonthsChart()
        {
            int currentYear = DateTime.Today.Year;
            var sales = ConDB.context.OrderDetails.Where(x => ((DateTime)x.PaymentDate).Year == currentYear).Select(x => new
            {
                PaymentDate = x.PaymentDate,
                Quantity = x.Quantity,
                Total = x.Total
            }).ToList();

            var salesByMonth = sales.GroupBy(y => ((DateTime)y.PaymentDate).Month).Select(g => new
            {

                Month = g.Key,
                SalesCount = g.Sum(o => o.Quantity),
                SalesTotal = g.Sum(o => o.Total)
            }).ToList();

            var salesCountByMonth = new int[12];
            var salesTotalByMonth = new decimal[12];
            for (int i = 0; i < 12; i++)
            {
                salesCountByMonth[i] = 0;
                salesTotalByMonth[i] = 0;
            }

            foreach (var sale in salesByMonth)
            {
                salesCountByMonth[sale.Month - 1] = sale.SalesCount.GetValueOrDefault();
                salesTotalByMonth[sale.Month - 1] = (decimal)sale.SalesTotal;
            }

            SalesChart.Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Количество товаров продано",
                    Values = new ChartValues<int>(salesCountByMonth),
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD73636"))
                },
                new ColumnSeries
                {
                    Title = "Сумма продаж",
                    Values = new ChartValues<decimal>(salesTotalByMonth),
                   Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF920C0C"))
                }
            };
        }

        private void ProductOfToday()
        {
            var sales = ConDB.context.OrderDetails.Select(x => new
            {
                x.PaymentDate,
                x.Quantity,
                ProductName = x.Products.ProductName 
            }).ToList();

            var mostSold = sales.GroupBy(s => s.ProductName).OrderByDescending(g => g.Sum(x => x.Quantity)).Select(g => new
            {
                ProductName = g.Key
            }).FirstOrDefault();

            if (mostSold != null)
            {
                TodayProductTB.Text = mostSold.ProductName;
            }
            else
            {
                TodayProductTB.Text = "Пока еще нет данных";
            }
        }

        private void InitializeTimeUpdater()
        {
            timer = new DispatcherTimer{Interval = TimeSpan.FromSeconds(1)};
            timer.Tick += (sender, e) => UpdateDateTimeInfo();
            timer.Start();
        }

        private void UpdateDateTimeInfo()
        {
            DayOfWeekTB.Text = DateTime.Now.ToString("dddd");

            DateTB.Text = DateTime.Now.ToString("d MMMM yyyy", new System.Globalization.CultureInfo("ru-RU"));

            TimeNowTB.Text = DateTime.Now.ToString("HH:mm:ss");
            var endOfDay = DateTime.Today.AddHours(19);
            var timeLeft = endOfDay - DateTime.Now;
            if(DateTime.Now > endOfDay)
            {
                TimeTillEndOfDayTB.Text = "Рабочий день окончен!";
            }
            else
            TimeTillEndOfDayTB.Text = $"{timeLeft.Hours}ч {timeLeft.Minutes}м {timeLeft.Seconds}с";
        }


       

    }
}
