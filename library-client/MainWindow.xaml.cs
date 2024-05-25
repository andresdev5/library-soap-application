using LibraryClient.Pages;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LibraryClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void SidebarOnSelectedItem(object? sender, SelectionChangedEventArgs e)
        {
            if (sender == null)
            {
                return;
            }

            var parentItem = (sender as ListView);
            var selectedItem = parentItem?.SelectedItem as ListViewItem;

            if (selectedItem == null)
            {
                return;
            }

            foreach (ListView listView in FindVisualChildren<ListView>(this))
            {

                if (listView != null && listView != parentItem)
                {
                    listView.SelectedIndex = -1;
                }
            }

            string name = selectedItem.Name;

            // clear NavigationFrame
            NavigationFrame.Navigate(null);

            switch (name)
            {
                case "Books_Add":
                    NavigationFrame.Navigate(new BooksAddPage());
                    break;
                case "Books_List":
                    NavigationFrame.Navigate(new BooksListPage());
                    break;
                default: break;
            }
        }

        private IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield return (T)Enumerable.Empty<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject ithChild = VisualTreeHelper.GetChild(depObj, i);
                if (ithChild == null) continue;
                if (ithChild is T t) yield return t;
                foreach (T childOfChild in FindVisualChildren<T>(ithChild)) yield return childOfChild;
            }
        }
    }
}