using Wpf.Ui.Controls;

namespace LibraryClient.Views.Pages
{
    /// <summary>
    /// Lógica de interacción para BooksListPage.xaml
    /// </summary>
    public partial class BooksListPage : INavigableView<ViewModels.BooksListViewModel>
    {
        public ViewModels.BooksListViewModel ViewModel { get; }

        public BooksListPage(ViewModels.BooksListViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
        }
    }
}
