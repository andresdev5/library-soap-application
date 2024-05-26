using LibraryClient.Models;
using LibrarySoapService;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace LibraryClient.ViewModels
{
    public partial class AddBookViewModel(ISnackbarService snackbarService) : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;
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

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
            {
                InitializeViewModel();
            }
        }

        public void OnNavigatedFrom() {}

        private void InitializeViewModel()
        {
            Task.Run(() =>
            {
                try
                {
                    LoadAuthors();
                    LoadGenres();
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        snackbarService.Show(
                            "Error",
                            ex.Message,
                            ControlAppearance.Danger,
                            new SymbolIcon(SymbolRegular.ErrorCircle24),
                            TimeSpan.FromSeconds(15)
                        );
                    });
                }
            });
            _isInitialized = true;
        }

        private async void LoadAuthors()
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

        private async void LoadGenres()
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
        }

        [RelayCommand]
        public void SubmitForm()
        {
            if (Title.Trim() == string.Empty)
            {
                snackbarService.Show(
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
                snackbarService.Show(
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
                snackbarService.Show(
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
                snackbarService.Show(
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
                snackbarService.Show(
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
                Task.Run(() => AddBook());
            }
            catch (Exception ex)
            {
                snackbarService.Show(
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

        private async void AddBook()
        {
            LibraryPortClient client = new();
            double priceValue = Double.TryParse(Price, out double price) ? price : 0;
            int yearValue = Int32.TryParse(Year, out int year) ? year : 0;
            var request = new addBookRequest();
            var bookModel = new BookModel();
            bookModel.title = Title;
            bookModel.year = yearValue;
            bookModel.yearSpecified = true;
            bookModel.price = priceValue;

            if (Isbn.Trim() != string.Empty)
            {
                bookModel.isbn = Isbn;
            }

            if (Publisher.Trim() != string.Empty)
            {
                bookModel.publisher = Publisher;
            }
            

            if (OpenedPicturePath.Trim() != string.Empty)
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
                await client.addBookAsync(request);
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

                Application.Current.Dispatcher.Invoke(() =>
                {
                    snackbarService.Show(
                        "Éxito",
                        "Libro agregado correctamente!",
                        ControlAppearance.Success,
                        new SymbolIcon(SymbolRegular.Checkmark24),
                        TimeSpan.FromSeconds(2)
                    );
                });
            }
            catch(Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    snackbarService.Show(
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
