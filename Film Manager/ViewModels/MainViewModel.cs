using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Film_Manager.ViewModelBase;
using System.Windows;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.General;
using TMDbLib.Client;

namespace Film_Manager.ViewModels
{
    class MainViewModel : ViewModelBase.PropertyChangedBase
    {

        #region Singleton & Constructor

        private static MainViewModel _instance;
        public static MainViewModel Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new MainViewModel();
                return _instance;
            }
        }

        public MahApps.Metro.Controls.MetroWindow BaseWindow { get; set; }


        private MainViewModel()
        {

        }

        public void Load()
        {
            FileInfo fiSettings = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.xml"));
            if (fiSettings.Exists)
            {
                Settings = Film_Manager.Settings.Settings.Load(fiSettings.FullName);
            }
            else
            {
                Settings = new Film_Manager.Settings.Settings(fiSettings.FullName);
            }
            FileInfo fi = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.xml"));
            if (fi.Exists)
            {
                MovieData = Data.MovieData.LoadMovies(fi.FullName);
                //MovieData.CheckMovies(BaseWindow, @"D:\Filme");
            }
            else
            {
                MovieData = new Data.MovieData(fi.FullName);
            }
            MovieData.MoviesChanged += () =>
            {
                if (Settings.AutoSyncWebsite)
                {
                    SynchronizeWebsite();
                }
            };
            if (settings.Synchronize && settings.FolderToSynchronize != null)
            {
                SynchronizeMovies();
            }
        }

        public void OnWindowClosing(object sender, EventArgs e)
        {
            if (MovieData.Movies != null && MovieData.Movies.Count > 0)
            {
                MovieData.Save();
            }
            if (Settings != null) { settings.Save(); }
        }

        #endregion

        private RelayCommand addmovies;
        public RelayCommand AddMovies
        {
            get
            {
                if (addmovies == null)
                    addmovies = new RelayCommand((object parameter) => { Views.ImportMoviesWindow frm = new Views.ImportMoviesWindow(); frm.Owner = Application.Current.MainWindow; frm.ShowDialog(); });
                return addmovies;
            }
        }

        private Data.MovieData moviedata;
        public Data.MovieData MovieData
        {
            get { return moviedata; }
            set
            {
                SetProperty(value, ref moviedata);
            }
        }


        private Data.Movie selectedmovie;
        public Data.Movie SelectedMovie
        {
            get { return selectedmovie; }
            set
            {
                SetProperty(value, ref selectedmovie);
            }
        }

        private RelayCommand removeselectedmovie;
        public RelayCommand RemoveSelectedMovie
        {
            get
            {
                if (removeselectedmovie == null)

                    removeselectedmovie = new RelayCommand((object parameter) =>
                    {
                        if (SelectedMovie != null)
                        {
                            MovieData.RemoveMovie(SelectedMovie);
                        }
                    });
                return removeselectedmovie;
            }
        }

        private RelayCommand loadfilminfos;
        public RelayCommand LoadFilmInfos
        {
            get
            {
                if (loadfilminfos == null)

                    loadfilminfos = new RelayCommand((object parameter) => {
                        if (SelectedMovie == null) { return; }
                        TMDbClient client = new TMDbClient("c95eb0cc8533fa0f989fec4c8fc563ce");
                        client.DefaultLanguage = "de";
                        string name = new DirectoryInfo(SelectedMovie.BaseDirectory).Name;
                        SearchContainer<SearchMovie> results = client.SearchMovie(name);
                        Views.SelectMovieWindow selectmoviewindow = new Views.SelectMovieWindow();
                        selectmoviewindow.Owner = BaseWindow;
                        ViewModels.SelectMovieViewModel.Instance.UpdateList(results);
                        ViewModels.SelectMovieViewModel.Instance.SearchText = name;
                        if (results.Results.Count > 0) { ViewModels.SelectMovieViewModel.Instance.SetSelection(results.Results[0]); }
                        ViewModels.SelectMovieViewModel.Instance.AllowSkip = false;
                        selectmoviewindow.ShowDialog();
                        if (ViewModels.SelectMovieViewModel.Instance.Result == SelectMovieResult.OK)
                        {
                            TMDbLib.Objects.Movies.Movie tmovie = client.GetMovie(ViewModels.SelectMovieViewModel.Instance.SelectedMovie.SearchMovie.Id, TMDbLib.Objects.Movies.MovieMethods.Credits | TMDbLib.Objects.Movies.MovieMethods.Trailers);
                            DirectoryInfo di = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images"));
                            if (!di.Exists) di.Create();
                            SelectedMovie.LoadInfos(tmovie, di.FullName);
                        }
                    });
                return loadfilminfos;
            }
        }

        #region settings
        private Settings.Settings settings;
        public Settings.Settings Settings
        {
            get { return settings; }
            set
            {
                SetProperty(value, ref settings);
            }
        }

        private RelayCommand synchronize;
        public RelayCommand Synchronize
        {
            get
            {
                if (synchronize == null)
                    synchronize = new RelayCommand(SynchronizeMovies);
                return synchronize;
            }
        }

        public void SynchronizeMovies(object parameter = null)
        {
            foreach (string path in settings.FolderToSynchronize)
            {
                MovieData.CheckMovies(BaseWindow, path);
            }
        }

        private RelayCommand opensettings;
        public RelayCommand OpenSettings
        {
            get
            {
                if (opensettings == null)
                    opensettings = new RelayCommand((object parameter) => { Views.SettingsWindow win = new Views.SettingsWindow { Owner = BaseWindow }; win.ShowDialog(); });
                return opensettings;
            }
        }

        private RelayCommand synchronizewebsitecommand;
        public RelayCommand SynchronizeWebsiteCommand
        {
            get
            {
                if (synchronizewebsitecommand == null)
                    synchronizewebsitecommand = new RelayCommand(SynchronizeWebsite);
                return synchronizewebsitecommand;
            }
        }

        public void SynchronizeWebsite(object data = null)
        {
            if (!string.IsNullOrWhiteSpace(Settings.WebsitePath))
            {
                Website.WebsiteGenerator generator = new Website.WebsiteGenerator();
                generator.Generate(ViewModels.MainViewModel.Instance.MovieData.Movies, new System.IO.DirectoryInfo(Settings.WebsitePath));
            }
        }
        #endregion
    }
}
