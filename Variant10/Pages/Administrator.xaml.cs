using System.Windows.Controls;

namespace Variant10.Pages
{
    public partial class Administrator : Page
    {
        private DatabaseUserX database;
        private User user;
        public Administrator(DatabaseUserX database, User user)
        {
            InitializeComponent();

            this.database = database;
            this.user = user;

            lUserName.Content = string.Format("{0} {1} {2}", user.Surname, user.Name, user.Patronymic);

            ProductListFrame.Navigate(new ProductList(database));
        }
    }
}
