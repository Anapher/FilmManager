using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;
using Film_Manager.ViewModelBase;
using TMDbLib.Client;

namespace Film_Manager.ViewModels
{
    class SelectMovieViewModel : ViewModelBase.PropertyChangedBase
    {
        #region Singleton & Constructor
        private static SelectMovieViewModel _instance;
        public static SelectMovieViewModel Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SelectMovieViewModel();
                return _instance;
            }
        }


        private SelectMovieViewModel()
        {
        }
        #endregion

        public System.Windows.Window BaseWindow { get; set; }

        private String searchtext;
        public String SearchText
        {
            get { return searchtext; }
            set
            {
                SetProperty(value, ref searchtext);
            }
        }


        private Data.SearchedMovie selectedmovie;
        public Data.SearchedMovie SelectedMovie
        {
            get { return selectedmovie; }
            set
            {
                SetProperty(value, ref selectedmovie);
            }
        }


        private List<Data.SearchedMovie> movies;
        public List<Data.SearchedMovie> Movies
        {
            get { return movies; }
            set
            {
                SetProperty(value, ref movies);
            }
        }

        public void UpdateList(SearchContainer<SearchMovie> list){
            var newlst = new List<Data.SearchedMovie>();

            foreach (SearchMovie item in list.Results)
            {
                newlst.Add(new Data.SearchedMovie { Title = item.Title, OriginalTitle = item.OriginalTitle, Image = new System.Windows.Media.Imaging.BitmapImage(new Uri(string.Format("http://image.tmdb.org/t/p/w500{0}", item.PosterPath), UriKind.Absolute)), Year = item.ReleaseDate.HasValue ? item.ReleaseDate.Value.Year : 0, SearchMovie = item });
            }
            Movies = newlst;
        }

        
        private bool allowskip =true;
        public bool AllowSkip
        {
            get { return allowskip =true; }
            set
            {
                SetProperty(value, ref allowskip);
            }
        }

        public void SetSelection(SearchMovie movie)
        {
            if (Movies != null && Movies.Count > 0)
            {
                foreach (var i in Movies)
                {
                    if (i.SearchMovie == movie)
                    {
                        SelectedMovie = i;
                        break;
                    }
                }
            }
        }

        private RelayCommand search;
        public RelayCommand Search
        {
            get
            {
                if (search == null)
                    search = new RelayCommand((object parameter) =>
                    {
                        TMDbClient client = new TMDbClient("c95eb0cc8533fa0f989fec4c8fc563ce");
                        client.DefaultLanguage = "de";
                        UpdateList(client.SearchMovie(SearchText));
                    });
                return search;
            }
        }

        private RelayCommand ok;
        public RelayCommand OK
        {
            get
            {
                if (ok == null)
                    ok = new RelayCommand((object parameter) => { this.Result = SelectMovieResult.OK; BaseWindow.Close(); });
                return ok;
            }
        }

        private RelayCommand skipmovie;
        public RelayCommand SkipMovie
        {
            get
            {
                if (skipmovie == null)
                    skipmovie = new RelayCommand((object parameter) => { this.Result = SelectMovieResult.Skip; BaseWindow.Close(); });
                return skipmovie;
            }
        }

        private RelayCommand justadd;
        public RelayCommand JustAdd
        {
            get
            {
                if (justadd == null)
                    justadd = new RelayCommand((object parameter) => { this.Result = SelectMovieResult.JustAdd; BaseWindow.Close(); });
                return justadd;
            }
        }

        public SelectMovieResult Result { get; set; }
    }

    public enum SelectMovieResult
    {
        OK,
        Skip,
        JustAdd
    }
}
