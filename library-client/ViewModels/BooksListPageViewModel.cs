using LibraryClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryClient.ViewModels
{
    class BooksListPageViewModel
    {
        private IList<Book> _books;

        public IList<Book> Books
        {
            get { return _books; }
            set { _books = value; }
        }

        public BooksListPageViewModel()
        {
            LibrarySoapService.LibraryPortClient client = new();
            var task = client.getBooksAsync(new LibrarySoapService.getBooksRequest());
            task.Wait();
            var models = task.Result.getBooksResponse1.OfType<LibrarySoapService.BookModel>().ToArray();
            _books = models.Select(m => new Book(m)).ToList();
        }
    }
}
