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
    public partial class ModifyWindow : Window
    {
        private DatabaseUserX database;
        public Product product { get; set; }
        public List<string> Categories { get; set; }

        public ModifyWindow(DatabaseUserX database, Product product)
        {
            InitializeComponent();
            this.database = database;
            this.product = product;

            Categories = database.Product.Select(p => p.Category).Distinct().ToList();

            DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (dialog.ShowDialog() == true)
            {
                product.Photo = dialog.SafeFileName;
                Photo.GetBindingExpression(Image.SourceProperty)?.UpdateTarget();
                database.SaveChanges();
            }
        }
    }
}
