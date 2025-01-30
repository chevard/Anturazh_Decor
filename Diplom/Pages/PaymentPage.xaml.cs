using Diplom.AppData;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
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
using static iTextSharp.text.pdf.AcroFields;

namespace Diplom.Pages
{
    public partial class PaymentPage : Page
    {
        private int _userId;
        private Users curProdavets;
        private List<OrderDetails> selectedProducts = new List<OrderDetails>();

        public PaymentPage(Users user)
        {
            InitializeComponent();
            curProdavets = user;
            DG2.ItemsSource = ConDB.context.Products.ToList();
            TBProdName.Text = curProdavets.FullName;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = DG2.SelectedItem as Products;

            if (selectedProduct != null)
            {
                if (selectedProduct.StockQuantity < 1)
                {
                    MessageBox.Show("Товар закончился!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                var existingOrderDetail = selectedProducts.FirstOrDefault(p => p.ProductID == selectedProduct.ProductID);

                if (existingOrderDetail != null)
                {
                    existingOrderDetail.Quantity++;
                    existingOrderDetail.Total = existingOrderDetail.Quantity * selectedProduct.Price;
                }
                else
                {
                    var orderDetail = new OrderDetails
                    {
                        ProductID = selectedProduct.ProductID,
                        Products = selectedProduct,
                        Quantity = 1,
                        CategoryID = (int)selectedProduct.CategoryID,
                        Total = selectedProduct.Price,
                        PaymentDate = DateTime.Now
                    };

                    selectedProducts.Add(orderDetail);
                }

                selectedProduct.StockQuantity--;

                DG2.ItemsSource = null;
                DG2.ItemsSource = ConDB.context.Products.ToList();

                DG.ItemsSource = null;
                DG.ItemsSource = selectedProducts;

                decimal total = selectedProducts.Sum(item => item.Total ?? 0);
                totaltxt.Text = total.ToString("F2");
            }
        }


        private void DeteleBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedOrderDetail = DG.SelectedItem as OrderDetails;

            if (selectedOrderDetail != null)
            {
                var productToUpdate = ConDB.context.Products.FirstOrDefault(p => p.ProductID == selectedOrderDetail.ProductID);

                if (productToUpdate != null)
                {
                    productToUpdate.StockQuantity++;

                    if (productToUpdate.StockQuantity <= 0)
                    {
                        ConDB.context.Products.Remove(productToUpdate);
                        MessageBox.Show("Товар полностью распродан и был удален.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }

                selectedOrderDetail.Quantity--;

                if (selectedOrderDetail.Quantity == 0)
                {
                    selectedProducts.Remove(selectedOrderDetail);
                }

                DG.ItemsSource = null;
                DG.ItemsSource = selectedProducts;

                DG2.ItemsSource = null;
                DG2.ItemsSource = ConDB.context.Products.ToList();

                decimal total = selectedProducts.Sum(item => item.Total ?? 0);
                totaltxt.Text = total.ToString("F2");

                ConDB.context.SaveChanges();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!selectedProducts.Any())
                {
                    MessageBox.Show("Добавьте товары для оплаты.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                foreach (var orderDetail in selectedProducts)
                {
                    var existingOrderDetail = ConDB.context.OrderDetails.FirstOrDefault(o => o.ProductID == orderDetail.ProductID && o.UserID == _userId);

                    if (existingOrderDetail == null)
                    {
                        ConDB.context.OrderDetails.Add(orderDetail);
                    }
                    else
                    {
                        existingOrderDetail.Quantity += orderDetail.Quantity;
                        existingOrderDetail.Total += orderDetail.Total;
                    }
                }

                ConDB.context.SaveChanges(); 

                var result = MessageBox.Show("Распечатать чек?", "Чек", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    GeneratePdfReceipt(selectedProducts);
                    MessageBox.Show("Чек распечатан", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Оплата прошла успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                selectedProducts.Clear();
                DG.ItemsSource = null;
                DG.ItemsSource = selectedProducts;
                totaltxt.Text = "0"; 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GeneratePdfReceipt(List<OrderDetails> orderDetails)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    var document = new Document(new iTextSharp.text.Rectangle(227, 800));
                    PdfWriter.GetInstance(document, stream);

                    string fontPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
                    var baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    var titleFont = new Font(baseFont, 16, Font.BOLD);
                    var headerFont = new Font(baseFont, 12, Font.BOLD);
                    var textFont = new Font(baseFont, 10, Font.NORMAL);
                    var smallFont = new Font(baseFont, 8, Font.ITALIC);

                    document.Open();

                    var paragraph = new iTextSharp.text.Paragraph();
                    paragraph.Alignment = Element.ALIGN_CENTER;
                    paragraph.Add(new Chunk("МАГАЗИН\n", titleFont));
                    paragraph.Add(new Chunk("\"Антураж\"\n\n", smallFont));
                    document.Add(paragraph);

                    var infoParagraph = new iTextSharp.text.Paragraph();
                    infoParagraph.Add(new Chunk("КАССОВЫЙ ЧЕК\n", headerFont));
                    infoParagraph.Add(new Chunk($"Дата: {DateTime.Now:dd.MM.yyyy HH:mm}\n", textFont));
                    infoParagraph.Add(new Chunk($"Кассир: {curProdavets.FullName}\n\n", textFont));
                    infoParagraph.Alignment = Element.ALIGN_CENTER;
                    document.Add(infoParagraph);

                    var table = new PdfPTable(3);
                    table.WidthPercentage = 100;
                    table.SetWidths(new float[] { 45f, 25f, 30f });

                    PdfPCell cell = new PdfPCell(new Phrase("ТОВАР", headerFont));
                    cell.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER;
                    cell.BorderWidthBottom = 0.5f;
                    cell.PaddingBottom = 5f;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase("КОЛ-ВО", headerFont));
                    cell.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER;
                    cell.BorderWidthBottom = 0.5f;
                    cell.PaddingBottom = 5f;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase("ЦЕНА", headerFont));
                    cell.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER;
                    cell.BorderWidthBottom = 0.5f;
                    cell.PaddingBottom = 5f;
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    table.AddCell(cell);

                    foreach (var item in orderDetails)
                    {
                        cell = new PdfPCell(new Phrase(item.Products.ProductName, textFont));
                        cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase(item.Quantity.ToString(), textFont));
                        cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.PaddingRight = 5f;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase($"{item.Products.Price:F2}", textFont));
                        cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        table.AddCell(cell);
                    }

                    cell = new PdfPCell(new Phrase("ИТОГО К ОПЛАТЕ:", headerFont));
                    cell.Colspan = 2;
                    cell.Border = iTextSharp.text.Rectangle.TOP_BORDER;
                    cell.BorderWidthTop = 0.5f;
                    cell.PaddingTop = 5f;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase($"{orderDetails.Sum(x => x.Total):0.00}", headerFont));
                    cell.Border = iTextSharp.text.Rectangle.TOP_BORDER;
                    cell.BorderWidthTop = 0.5f;
                    cell.PaddingTop = 5f;
                    cell.PaddingRight = 0.5f;
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    table.AddCell(cell);

                    document.Add(table);

                    var footerParagraph = new iTextSharp.text.Paragraph("\n", smallFont);
                    footerParagraph.Add(new Chunk("СПАСИБО ЗА ПОКУПКУ!\n", headerFont));
                    footerParagraph.Add(new Chunk("Надеемся увидеть Вас снова!\n\n", smallFont));
                    footerParagraph.Add(new Chunk("* * * * * * * * * * * * * * *\n", textFont));
                    footerParagraph.Add(new Chunk("Телефон: +7 (123) 456-78-90\n", smallFont));
                    footerParagraph.Add(new Chunk("Email: anrturazh@yandex.ru\n", smallFont));
                    footerParagraph.Alignment = Element.ALIGN_CENTER;
                    document.Add(footerParagraph);

                    document.Close();

                    string tempFile = System.IO.Path.GetTempFileName() + ".pdf";
                    File.WriteAllBytes(tempFile, stream.ToArray());

                    var process = new System.Diagnostics.Process();
                    process.StartInfo.FileName = tempFile;
                    process.StartInfo.UseShellExecute = true;
                    process.Start();

                    System.Threading.Thread.Sleep(2000);

                    File.Delete(tempFile);
                }

                MessageBox.Show("Чек успешно создан", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при генерации чека: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    }
}
