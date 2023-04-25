using System.Windows.Controls;

namespace Variant10.Pages
{
    public partial class ProductList : Page
    {
        private DatabaseUserX database;

        public ProductList(DatabaseUserX database)
        {
            InitializeComponent();

            this.database = database;
        }
    }
}
