using LibraryClient.Views.Pages;
using System.Windows;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace LibraryClient.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INavigationWindow
    {
        public ViewModels.MainWindowViewModel ViewModel { get; }

        public MainWindow(
            ViewModels.MainWindowViewModel viewModel,
            IPageService pageService,
            INavigationService navigationService,
            IServiceProvider serviceProvider,
            ISnackbarService snackbarService
        )
        {
            ViewModel = viewModel;
            DataContext = this;
            Wpf.Ui.Appearance.SystemThemeWatcher.Watch(this);
            InitializeComponent();
            SetPageService(pageService);

            snackbarService.SetSnackbarPresenter(SnackbarPresenter);
            navigationService.SetNavigationControl(RootNavigation);
            ApplicationThemeManager.Apply(
                ApplicationTheme.Light,
                WindowBackdropType.None,
                false
            );

            RootNavigation.SetServiceProvider(serviceProvider);
        }

        public INavigationView GetNavigation() => RootNavigation;

        public bool Navigate(Type pageType) => RootNavigation.Navigate(pageType);

        public void SetPageService(IPageService pageService) => RootNavigation.SetPageService(pageService);

        public void ShowWindow() => Show();

        public void CloseWindow() => Close();

        /// <summary>
        /// Raises the closed event.
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Make sure that closing this window will begin the process of closing the application.
            Application.Current.Shutdown();
        }

        private void OnNavigationSelectionChanged(object sender, RoutedEventArgs e)
        {
            if (sender is not Wpf.Ui.Controls.NavigationView navigationView)
            {
                return;
            }

            RootNavigation.SetCurrentValue(
                NavigationView.HeaderVisibilityProperty,
                Visibility.Visible
            );
        }

        INavigationView INavigationWindow.GetNavigation()
        {
            throw new NotImplementedException();
        }

        public void SetServiceProvider(IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
        }
    }
}