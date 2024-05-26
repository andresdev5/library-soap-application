using LibrarySoapService;
using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace LibraryClient.Models
{
    public class Book : INotifyPropertyChanged
    {
        private long _id;
        private string _title;
        private int _year;
        private string _isbn;
        private string _publisher;
        private double _price;
        private string _imageUrl;
        private BitmapImage _image;
        private Author _author;

        public long Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        public int Year
        {
            get { return _year; }
            set
            {
                _year = value;
                OnPropertyChanged(nameof(Year));
            }
        }

        public string Isbn
        {
            get { return _isbn; }
            set
            {
                _isbn = value;
                OnPropertyChanged(nameof(Isbn));
            }
        }

        public string Publisher
        {
            get { return _publisher; }
            set
            {
                _publisher = value;
                OnPropertyChanged(nameof(Publisher));
            }
        }

        public double Price
        {
            get { return _price; }
            set
            {
                _price = value;
                OnPropertyChanged(nameof(Price));
            }
        }

        public string ImageUrl
        {
            get { return _imageUrl; }
            set
            {
                _imageUrl = value;
                OnPropertyChanged(nameof(ImageUrl));
            }
        }

        public BitmapImage Image
        {
            get { return _image; }
            set
            {
                _image = value;
                OnPropertyChanged(nameof(Image));
            }
        }

        public Author Author
        {
            get { return _author; }
            set
            {
                _author = value;
                OnPropertyChanged(nameof(Author));
            }
        }

        public Book(BookModel model)
        {
            _id = model.id;
            _title = model.title;
            _year = model.year;
            _price = model.price;
            _isbn = model.isbn;
            _publisher = model.publisher;

            if (model.imageUrl != null && model.imageUrl != "")
            {
                _imageUrl = model.imageUrl;
                _image = new BitmapImage(new Uri(model.imageUrl));
            }
            else
            {
                _imageUrl = "https://i.imgur.com/XVthRcs.png";
                _image = new BitmapImage(new Uri(_imageUrl));
            }

            _author = new Author(model.author);
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
