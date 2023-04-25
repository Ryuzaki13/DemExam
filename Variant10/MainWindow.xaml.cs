using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using Variant10.Pages;

namespace Variant10
{
    public class ProductSort
    {
        public string DisplayName { get; set; }
        public ListSortDirection SortDirection { get; set; }
    }

    public partial class MainWindow : Window
    {
        private DatabaseUserX database;
        private AddProductWindow addProductWindow;

        public ObservableCollection<Product> Products { get; set; }
        public ObservableCollection<ProductSort> ProductSorts { get; set; }
        public ObservableCollection<string> ProductFilter { get; set; }

        int timeout = 10;
        DispatcherTimer timer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();

            database = new DatabaseUserX();

            Products = new ObservableCollection<Product>(database.Product.ToList());
            ProductSorts = new ObservableCollection<ProductSort>()
            {
                new ProductSort() {DisplayName = "По возрастанию", SortDirection = ListSortDirection.Ascending},
                new ProductSort() {DisplayName = "По убыванию", SortDirection = ListSortDirection.Descending}
            };
            ProductFilter = new ObservableCollection<string>(
                database.Product
                .Select(p => p.Manufacturer)
                .Distinct()
                .ToList()
            );
            cbFilter.SetBinding(ItemsControl.ItemsSourceProperty, new Binding() { Source = ProductFilter });

            DataContext = this;

            timeout = 10;
            lTimeout.Content = timeout;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();

            Console.WriteLine(WindowState);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timeout--;
            lTimeout.Content = timeout;

            if (timeout < 1)
            {
                timer.Stop();
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            Product product = listView.SelectedItem as Product;
            if (product != null)
            {
                
            }

        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateFilter();
        }

        private void cbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateFilter();
        }

        private void cbFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateFilter();
        }

        private void UpdateFilter()
        {
            var view = CollectionViewSource.GetDefaultView(ProductList.ItemsSource);
            if (view != null)
            {
                view.Filter = new Predicate<object>(product =>
                {
                    Product p = product as Product;
                    bool result = true;
                    /** SEARCH */
                    if (tbSearch.Text.Trim().Length > 0)
                    {
                        string searchValue = tbSearch.Text.Trim().ToLower();
                        result = p.Manufacturer.ToLower().Contains(searchValue) ||
                            p.Name.ToLower().Contains(searchValue) ||
                            p.Category.ToLower().Contains(searchValue) ||
                            p.Description.ToLower().Contains(searchValue);
                    }
                    /** FILTER BY MANUFACTURER */
                    if (cbFilter.SelectedIndex > -1)
                    {
                        string manufacturer = cbFilter.SelectedItem as string;
                        result = result && p.Manufacturer == manufacturer;
                    }
                    return result;
                });

                /** SORT BY PRODUCT COST */
                view.SortDescriptions.Clear();
                if (cbSort.SelectedIndex > -1)
                {
                    ProductSort productSort = cbSort.SelectedItem as ProductSort;
                    view.SortDescriptions.Add(new SortDescription("Cost", productSort.SortDirection));
                }
            }
        }

        public void CloseAddWindow()
        {
            addProductWindow = null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (addProductWindow == null)
            {
                
                addProductWindow = new AddProductWindow(database);
                addProductWindow.Show();
                addProductWindow.Owner = this;
               
            }
        }

        private void ProductList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Product product = ProductList.SelectedItem as Product;
            ModifyWindow w = new ModifyWindow(database, product);
            w.Show();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            Console.WriteLine(WindowState);
            Console.WriteLine("{0} {1}", Width, Height);
        }
    }
}
