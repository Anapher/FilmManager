using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Film_Manager.ViewModelBase;
using System.IO;
using Ookii.Dialogs.Wpf;

namespace Film_Manager.ViewModels
{
    class ImportMoviesViewModel : PropertyChangedBase
    {
        #region Singleton & Constructor
        private static ImportMoviesViewModel _instance;
        public static ImportMoviesViewModel Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ImportMoviesViewModel();
                return _instance;
            }
        }


        private ImportMoviesViewModel()
        {
            MovieImport = new Data.MovieImport();
        }
        #endregion

        public MahApps.Metro.Controls.MetroWindow BaseWindow { get; set; }
        
        private Data.MovieImport movieimport;
        public Data.MovieImport MovieImport
        {
            get { return movieimport; }
            set
            {
                SetProperty(value, ref movieimport);
            }
        }

        
        private object selectedmovie;
        public object SelectedMovie
        {
            get { return selectedmovie; }
            set
            {
                SetProperty(value, ref selectedmovie);
            }
        }


        private RelayCommand importfolder;
        public RelayCommand ImportFolder
        {
            get
            {
                if (importfolder == null)

                    importfolder = new RelayCommand((object parameter) => {
                        VistaFolderBrowserDialog fbd = new VistaFolderBrowserDialog();
                        fbd.ShowNewFolderButton = false;
                        if (fbd.ShowDialog() == true)
                        {
                            movieimport.ImportFolder(new DirectoryInfo(fbd.SelectedPath));
                        }
                    });
                return importfolder;
            }
        }
        private RelayCommand removemovie;
        public RelayCommand RemoveMovie
        {
            get
            {
                if (removemovie == null)

                    removemovie = new RelayCommand((object parameter) => {
                        if (selectedmovie == null) return;
                        movieimport.RemoveEntry(selectedmovie);
                    });
                return removemovie;
            }
        }

        private RelayCommand removeallmovies;
        public RelayCommand RemoveAllMovies
        {
            get
            {
                if (removeallmovies == null)

                    removeallmovies = new RelayCommand((object parameter) => {
                        MovieImport.RemoveAllMovies();
                    });
                return removeallmovies;
            }
        }

        private RelayCommand importmovies;
        public RelayCommand ImportMovies
        {
            get
            {
                if (importmovies == null)
                    importmovies = new RelayCommand((object parameter) => { MovieImport.ImportMovies(BaseWindow, true); });
                return importmovies;
            }
        }
    }
}
