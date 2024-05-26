using LibraryClient.Models;
using LibraryClient.Services;
using LibrarySoapService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace LibraryClient.ViewModels
{
    public partial class EditBookViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;
        private INavigationParams _navigationParams;
        private IServiceProvider _serviceProvider;
        private ISnackbarService _snackbarService;
        private Book _book;

        private LibraryPortClient _client = new();

        [ObservableProperty]
        private Visibility _openedFolderPathVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private Visibility _openedPicturePathVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private string _openedPicturePath = string.Empty;

        [ObservableProperty]
        private string _title = string.Empty;

        [ObservableProperty]
        private string _isbn = string.Empty;

        [ObservableProperty]
        private string _year = string.Empty;

        [ObservableProperty]
        private string _publisher = string.Empty;

        [ObservableProperty]
        private string _price = "0,0";

        [ObservableProperty]
        private ObservableCollection<Author> _authors = new ObservableCollection<Author>();

        [ObservableProperty]
        private bool _isAuthorSuggestionSelected = false;

        [ObservableProperty]
        private Author _selectedAuthor = new Author();

        [ObservableProperty]
        private string _authorSuggestEnteredText = string.Empty;

        [ObservableProperty]
        private ObservableCollection<GenreListViewItem> _genres = new ObservableCollection<GenreListViewItem>();

        [ObservableProperty]
        private BitmapImage _previewImage = new BitmapImage();

        public EditBookViewModel(
            IServiceProvider serviceProvider, 
            INavigationParams navigationParams,
            ISnackbarService snackbarService)
        {
            _serviceProvider = serviceProvider;
            _navigationParams = navigationParams;
            _snackbarService = snackbarService;
        }

        public void OnNavigatedFrom() {}

        public void OnNavigatedTo() 
        {
            if (!_isInitialized)
            {
                InitializeViewModel();
            }
        }

        private void InitializeViewModel()
        {
            Task.Run(() => LoadData());
            _isInitialized = true;
        }

        private async Task LoadData()
        {
            try
            {
                await LoadAuthors();
                await LoadGenres();
                LoadBookContext();
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _snackbarService.Show(
                        "Error",
                        ex.Message,
                        ControlAppearance.Danger,
                        new SymbolIcon(SymbolRegular.ErrorCircle24),
                        TimeSpan.FromSeconds(15)
                    );
                });
            }
        }

        private async Task LoadAuthors()
        {
            var response = await _client.getAuthorsAsync(new getAuthorsRequest());
            var authors = response.getAuthorsResponse1;

            Application.Current.Dispatcher.Invoke(() =>
            {
                Authors.Clear();
                authors.ToList<AuthorModel>().ForEach(model =>
                {
                    Authors.Add(new Author(model));
                });
            });
        }

        private async Task LoadGenres()
        {
            var response = await _client.getGenresAsync(new getGenresRequest());
            var genres = response.getGenresResponse1;

            Application.Current.Dispatcher.Invoke(() =>
            {
                Genres.Clear();
                genres.ToList<GenreModel>().ForEach(model =>
                {
                    GenreListViewItem item = new();
                    item.Genre = new Genre(model);
                    item.IsSelected = false;

                    Genres.Add(item);
                });
            });
        }

        private void LoadBookContext()
        {
            _book = _navigationParams.Get<Book>("BookToUpdate");

            Application.Current.Dispatcher.Invoke(() =>
            {
                Title = _book.Title;
                Isbn = _book.Isbn;
                Year = _book.Year.ToString();
                Publisher = _book.Publisher;
                Price = _book.Price.ToString();

                if (_book.Author != null && _book.Author.Id != 0)
                {
                    SelectedAuthor = Authors.First(author => author.Id == _book.Author.Id);
                    IsAuthorSuggestionSelected = true;
                    AuthorSuggestEnteredText = SelectedAuthor.CompleteDisplayName;
                }

                if (_book.Image != null)
                {
                    Uri uri = new(_book.ImageUrl);
                    string filename = uri.Segments.Last();

                    OpenedPicturePath = "server://" + filename;
                    OpenedPicturePathVisibility = Visibility.Visible;
                    PreviewImage = new BitmapImage(uri);
                }

                Genres.ToList().ForEach(genre =>
                {
                    genre.IsSelected = _book.Genres.Any(bookGenre => bookGenre.Id == genre.Genre.Id);
                });
            });
        }

        [RelayCommand]
        public void OnOpenPicture()
        {
            OpenedPicturePathVisibility = Visibility.Collapsed;

            OpenFileDialog openFileDialog =
                new()
                {
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                    Filter = "Image files (*.bmp;*.jpg;*.jpeg;*.png)|*.bmp;*.jpg;*.jpeg;*.png|All files (*.*)|*.*"
                };

            if (openFileDialog.ShowDialog() != true)
            {
                return;
            }

            if (!File.Exists(openFileDialog.FileName))
            {
                return;
            }

            OpenedPicturePath = openFileDialog.FileName;
            OpenedPicturePathVisibility = Visibility.Visible;
            PreviewImage = new BitmapImage(new Uri(openFileDialog.FileName));
        }

        [RelayCommand]
        public void SubmitForm()
        {
            if (Title.Trim() == string.Empty)
            {
                _snackbarService.Show(
                    "Error",
                    "El título del libro es requerido!",
                    ControlAppearance.Danger,
                    new SymbolIcon(SymbolRegular.Book24),
                    TimeSpan.FromSeconds(2)
                );

                return;
            }

            if (Year.Trim() == string.Empty)
            {
                _snackbarService.Show(
                    "Error",
                    "El año del libro es requerido!",
                    ControlAppearance.Danger,
                    new SymbolIcon(SymbolRegular.Book24),
                    TimeSpan.FromSeconds(2)
                );

                return;
            }

            if (SelectedAuthor == null || SelectedAuthor.Id == 0)
            {
                _snackbarService.Show(
                    "Error",
                    "El autor del libro es requerido!",
                    ControlAppearance.Danger,
                    new SymbolIcon(SymbolRegular.Book24),
                    TimeSpan.FromSeconds(2)
                );

                return;
            }

            var selectedGenres = Genres.Where(item => item.IsSelected).ToList();

            if (selectedGenres.Count == 0)
            {
                _snackbarService.Show(
                    "Error",
                    "Al menos un género es requerido",
                    ControlAppearance.Danger,
                    new SymbolIcon(SymbolRegular.Book24),
                    TimeSpan.FromSeconds(2)
                );

                return;
            }

            double priceValue = Double.TryParse(Price, out double price) ? price : 0;

            if (priceValue <= 0)
            {
                _snackbarService.Show(
                    "Error",
                    "El precio del libro es requerido!",
                    ControlAppearance.Danger,
                    new SymbolIcon(SymbolRegular.Book24),
                    TimeSpan.FromSeconds(2)
                );

                return;
            }

            try
            {
                Task.Run(() => SaveBook());
            }
            catch (Exception ex)
            {
                _snackbarService.Show(
                    "Error",
                    "Ha ocurrido un error al agregar el libro!",
                    ControlAppearance.Caution,
                    new SymbolIcon(SymbolRegular.Book24),
                    TimeSpan.FromSeconds(2)
                );
            }
        }

        [RelayCommand]
        public void AuthorSuggestionChosen(AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            var author = args.SelectedItem as Author;

            if (author != null)
            {
                SelectedAuthor = author;
                IsAuthorSuggestionSelected = true;
            }
        }

        [RelayCommand]
        public void ClearAuthorSelected()
        {
            SelectedAuthor = new Author();
            IsAuthorSuggestionSelected = false;
            AuthorSuggestEnteredText = string.Empty;
        }

        private async void SaveBook()
        {
            double priceValue = Double.TryParse(Price, out double price) ? price : 0;
            int yearValue = Int32.TryParse(Year, out int year) ? year : 0;
            var request = new updateBookRequest();
            var bookModel = new BookModel();
            bookModel.id = _book.Id;
            bookModel.title = Title;
            bookModel.year = yearValue;
            bookModel.yearSpecified = true;
            bookModel.price = priceValue;

            if (Isbn != null && Isbn.Trim() != string.Empty)
            {
                bookModel.isbn = Isbn;
            }

            if (Publisher != null && Publisher.Trim() != string.Empty)
            {
                bookModel.publisher = Publisher;
            }


            if (OpenedPicturePath.Trim() != string.Empty && !OpenedPicturePath.StartsWith("server://"))
            {
                byte[] imageBytes = File.ReadAllBytes(OpenedPicturePath);
                RequestImage requestImage = new()
                {
                    data = imageBytes,
                    name = Path.GetFileName(OpenedPicturePath),
                    type = Path.GetExtension(OpenedPicturePath)
                };
                bookModel.image = requestImage;
            }

            bookModel.author = new AuthorModel()
            {
                id = SelectedAuthor.Id
            };

            var selectedGenres = Genres.Where(item => item.IsSelected).ToList();
            bookModel.genres = selectedGenres.Select(genre => new GenreModel() { id = genre.Genre.Id }).ToArray();
            request.book = bookModel;

            try
            {
                await _client.updateBookAsync(request);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Title = string.Empty;
                    Isbn = string.Empty;
                    Publisher = string.Empty;
                    Year = string.Empty;
                    Price = string.Empty;
                    OpenedPicturePath = string.Empty;
                    OpenedPicturePathVisibility = Visibility.Collapsed;
                    SelectedAuthor = new Author();
                    IsAuthorSuggestionSelected = false;
                    AuthorSuggestEnteredText = string.Empty;
                    Genres.ToList().ForEach(genre => genre.IsSelected = true);
                    PreviewImage = new BitmapImage();

                    var navigationWindow = (_serviceProvider.GetService(typeof(INavigationWindow)) as INavigationWindow)!;
                    _ = navigationWindow.Navigate(typeof(LibraryClient.Views.Pages.BooksListPage));

                    _snackbarService.Show(
                        "Éxito",
                        "Libro editado correctamente!",
                        ControlAppearance.Success,
                        new SymbolIcon(SymbolRegular.Checkmark24),
                        TimeSpan.FromSeconds(2)
                    );
                });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _snackbarService.Show(
                        "Error",
                        ex.Message,
                        ControlAppearance.Danger,
                        new SymbolIcon(SymbolRegular.ErrorCircle24),
                        TimeSpan.FromSeconds(15)
                    );
                });
            }
        }
    }
}
