using Microsoft.Win32;
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

namespace Variant10
{

    public partial class AddProductWindow : Window
    {
        public List<string> Categories { get; set; }
        public List<string> Manufacturers { get; set; }
        public List<string> Providers { get; set; }
        public List<string> Units { get; set; }

        public Product product { get; set; }
        private DatabaseUserX database;

        public AddProductWindow(DatabaseUserX database)
        {
            InitializeComponent();
            this.database = database;

            product = new Product();

            Categories = database.Product.Select(x => x.Category).Distinct().ToList();
            Manufacturers = database.Product.Select(x => x.Manufacturer).Distinct().ToList();
            Providers = database.Product.Select(x => x.Provider).Distinct().ToList();
            Units = database.Product.Select(x => x.Unit).Distinct().ToList();

            DataContext = this;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            ((MainWindow)Owner).CloseAddWindow();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (dialog.ShowDialog() == true)
            { 
                product.Photo = dialog.SafeFileName;
                Photo.GetBindingExpression(Image.SourceProperty)?.UpdateTarget();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            database.Product.Add(product);
            database.SaveChanges();
        }
    }
}
