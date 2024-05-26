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

namespace LibraryClient.Controls
{
    /// <summary>
    /// Lógica de interacción para BookCard.xaml
    /// </summary>
    public partial class BookCard : UserControl
    {
        public event EventHandler? OnDelete;
        public event EventHandler? OnEdit;

        public BookCard()
        {
            InitializeComponent();
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            if (OnEdit != null)
            {
                OnEdit(DataContext, e);
            }
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (OnDelete != null)
            {
                OnDelete(DataContext, e);
            }
        }
    }
}
