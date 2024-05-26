using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryClient.Models
{
    public partial class GenreListViewItem : ObservableObject
    {
        [ObservableProperty]
        public Genre _genre = new Genre();

        [ObservableProperty]
        public bool _isSelected = false;
    }
}
