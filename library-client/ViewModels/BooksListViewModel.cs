using CommunityToolkit.Mvvm.ComponentModel;
using LibraryClient.Models;
using LibraryClient.Services;
using LibraryClient.Views;
using LibrarySoapService;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Navigation;
using Wpf.Ui;
using Wpf.Ui.Controls;
using static System.Reflection.Metadata.BlobBuilder;

namespace LibraryClient.ViewModels
{
    public partial class BooksListViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;
        private LibraryPortClient _client = new();
        private IServiceProvider serviceProvider;
        private INavigationParams navigationParams;

        [ObservableProperty]
        private List<DataColor> _colors = [];

        [ObservableProperty]
        private ObservableCollection<Book> _books = [];

        public BooksListViewModel(IServiceProvider serviceProvider, INavigationParams navigationParams)
        {
            this.serviceProvider = serviceProvider;
            this.navigationParams = navigationParams;
        }

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
            {
                InitializeViewModel();
            }

            try
            {
                Task.Run(() => LoadBooks());
            }
            catch (Exception ex)
            {
                // Log error
                Console.Write(ex.ToString());
            }
        }

        public void OnNavigatedFrom() { }

        private void InitializeViewModel()
        {
            _isInitialized = true;
        }

        private async void LoadBooks()
        {
            try
            {
                var response = await _client.getBooksAsync(new getBooksRequest());
                var books = response.getBooksResponse1;

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Books.Clear();

                    books.ToList<BookModel>().ForEach(model =>
                    {
                        Books.Add(new Book(model));
                    });
                });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    OnOpenErrorMessageBox().Wait();
                    Application.Current.Shutdown();
                });
            }
        }

        [RelayCommand]
        public void DeleteBook(object sender)
        {
            if (sender is Book book)
            {
                deleteBookRequest request = new deleteBookRequest
                {
                    id = book.Id
                };

                Task.Run(async () =>
                {
                    await _client.deleteBookAsync(request);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var uiMessageBox = new Wpf.Ui.Controls.MessageBox
                        {
                            Title = "Atención",
                            Content = "Libro eliminado correctamente",
                        };

                        _ = uiMessageBox.ShowDialogAsync();
                    });

                    LoadBooks();
                });
            }
        }

        [RelayCommand]
        public void EditBook(object sender)
        {
            if (sender is Book book)
            {
                navigationParams.Set("BookToUpdate", book);
                var navigationWindow = (serviceProvider.GetService(typeof(INavigationWindow)) as INavigationWindow)!;
                _ = navigationWindow.Navigate(typeof(LibraryClient.Views.Pages.EditBookPage));
            }
        }


        private async Task OnOpenErrorMessageBox()
        {
            var uiMessageBox = new Wpf.Ui.Controls.MessageBox
            {
                Title = "Error",
                Content = "Ocurrió un error al intentar conectarse con el servidor",
            };

            _ = await uiMessageBox.ShowDialogAsync();
        }
    }
}
