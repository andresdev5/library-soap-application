using LibrarySoapService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryClient.Models
{
    public partial class Genre : ObservableObject
    {
        [ObservableProperty]
        private long _id;

        [ObservableProperty]
        private string _name = "";

        public Genre() { }

        public Genre(long id, string name)
        {
            _id = id;
            _name = name;
        }

        public Genre(GenreModel model)
        {
            _id = model.id;
            _name = model.name;
        }
    }
}
