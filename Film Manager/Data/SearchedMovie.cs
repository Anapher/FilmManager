using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Film_Manager.Data
{
    class SearchedMovie
    {
        public string Title { get; set; }
        public string OriginalTitle { get; set; }
        public BitmapImage Image { get; set; }
        public int Year { get; set; }
        public TMDbLib.Objects.Search.SearchMovie SearchMovie { get; set; }  
    }
}
