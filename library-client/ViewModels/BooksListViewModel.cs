using CommunityToolkit.Mvvm.ComponentModel;
using LibraryClient.Models;
using LibrarySoapService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Wpf.Ui.Controls;

namespace LibraryClient.ViewModels
{
    public partial class BooksListViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;

        [ObservableProperty]
        private List<DataColor> _colors = [];

        [ObservableProperty]
        private ObservableCollection<Book> _books = [];

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
            {
                InitializeViewModel();
            }
        }

        public void OnNavigatedFrom() { }

        private void InitializeViewModel()
        {
            try
            {
                Task.Run(() => LoadBooks());
            }
            catch (Exception ex)
            {
                // Log error
                Console.Write(ex.ToString());
            }
            
            _isInitialized = true;
        }

        private async void LoadBooks()
        {
            LibraryPortClient client = new();
            var response = await client.getBooksAsync(new getBooksRequest());
            var books = response.getBooksResponse1;

            Application.Current.Dispatcher.Invoke(() =>
            {
                books.ToList<BookModel>().ForEach(model =>
                {
                    Books.Add(new Book(model));
                });
            });
        }
    }
}
