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
using Wpf.Ui.Controls;

namespace LibraryClient.Views.Pages
{
    /// <summary>
    /// Lógica de interacción para AddBookPage.xaml
    /// </summary>
    public partial class AddBookPage : INavigableView<ViewModels.AddBookViewModel>
    {
        public ViewModels.AddBookViewModel ViewModel { get; }

        public AddBookPage(ViewModels.AddBookViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
        }

        private void authorSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            ViewModel.AuthorSuggestionChosenCommand.Execute(args);
        }
    }
}
