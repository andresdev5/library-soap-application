using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace LibraryClient.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private bool _isInitialized = false;

        [ObservableProperty]
        private string _applicationTitle = string.Empty;

        [ObservableProperty]
        private ObservableCollection<object> _navigationItems = [];

        [ObservableProperty]
        private ObservableCollection<object> _navigationFooter = [];

        [ObservableProperty]
        private ObservableCollection<Wpf.Ui.Controls.MenuItem> _trayMenuItems = [];

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Style",
            "IDE0060:Remove unused parameter"
        )]
        public MainWindowViewModel(INavigationService navigationService)
        {
            if (!_isInitialized)
            {
                InitializeViewModel();
            }
        }

        private void InitializeViewModel()
        {
            ApplicationTitle = "Library Desktop Client";

            NavigationItems =
            [
                new NavigationViewItem()
            {
                Content = "Libros",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Book24 },
                MenuItemsSource = new object[]
                {
                    new NavigationViewItem()
                    {
                        Content = "Últimos libros",
                        Icon = new SymbolIcon { Symbol = SymbolRegular.List24 },
                        TargetPageType = typeof(Views.Pages.BooksListPage),
                    },
                    new NavigationViewItem()
                    {
                        Content = "Agregar libro",
                        Icon = new SymbolIcon { Symbol = SymbolRegular.Add24 },
                        TargetPageType = typeof(Views.Pages.AddBookPage),
                    }
                }
            },
        ];

            NavigationFooter =
            [
                new NavigationViewItem()
            {
                Content = "Configuracion",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
            },
        ];

            TrayMenuItems = [];
            _isInitialized = true;
        }
    }
}
