using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Film_Manager.ViewModelBase;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows;
using MahApps.Metro.Controls.Dialogs;

using TMDbLib.Client;
using TMDbLib;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using MediaInfoNET;

namespace Film_Manager.Data
{
    class MovieImport : PropertyChangedBase
    {

        private static List<string> movieextensions;
        private static List<string> MovieExtensions
        {
            get
            {
                if (movieextensions == null)
                {
                    movieextensions = new List<string> { ".MP4", ".AVI", ".MKV", ".WMV", ".MOV", ".MPEG", ".FLV" };
                }
                return movieextensions;
            }
        }


        public MovieImport()
        {

            FoundMovies = new ObservableCollection<MovieInfo>();
        }

        #region settings
        private bool ignoresamples = true;
        public bool IgnoreSamples
        {
            get { return ignoresamples = true; }
            set
            {
                SetProperty(value, ref ignoresamples);
            }
        }


        private bool usefoldername = true;
        public bool UseFolderName
        {
            get { return usefoldername; }
            set
            {
                SetProperty(value, ref usefoldername);
            }
        }


        private bool usefilename;
        public bool UseFileName
        {
            get { return usefilename; }
            set
            {
                SetProperty(value, ref usefilename);
            }
        }


        private bool usefilter = true;
        public bool UseFilter
        {
            get { return usefilter; }
            set
            {
                SetProperty(value, ref usefilter);
            }
        }


        private bool replaceumlauts = true;
        public bool ReplaceUmlauts
        {
            get { return replaceumlauts = true; }
            set
            {
                SetProperty(value, ref replaceumlauts);
            }
        }


        private bool replacecodec = true;
        public bool ReplaceCodec
        {
            get { return replacecodec; }
            set
            {
                SetProperty(value, ref replacecodec);
            }
        }


        private bool replaceyear = true;
        public bool ReplaceYear
        {
            get { return replaceyear; }
            set
            {
                SetProperty(value, ref replaceyear);
            }
        }


        private bool replacequalitysetting = true;
        public bool ReplaceQualitySetting
        {
            get { return replacequalitysetting; }
            set
            {
                SetProperty(value, ref replacequalitysetting);
            }
        }


        private bool replacegroup = true;
        public bool ReplaceGroup
        {
            get { return replacegroup; }
            set
            {
                SetProperty(value, ref replacegroup);
            }
        }


        private bool replacelanguage = true;
        public bool ReplaceLanguage
        {
            get { return replacelanguage; }
            set
            {
                SetProperty(value, ref replacelanguage);
            }
        }


        private bool usefirstmoviefirst;
        public bool UseFirstMovieFirst
        {
            get { return usefirstmoviefirst; }
            set
            {
                SetProperty(value, ref usefirstmoviefirst);
            }
        }
        #endregion

        private ObservableCollection<MovieInfo> foundmovies;
        public ObservableCollection<MovieInfo> FoundMovies
        {
            get { return foundmovies; }
            set
            {
                SetProperty(value, ref foundmovies);
            }
        }

        public void ImportFolder(DirectoryInfo path)
        {
            foreach (DirectoryInfo d in path.GetDirectories("*.*", SearchOption.TopDirectoryOnly))
            {
                ImportMovie(d);
            }
        }

        public void ImportMovie(DirectoryInfo path)
        {
            MovieInfo info = new MovieInfo();
            info.Movies = GetVideosFromFolder(path, ignoresamples);
            if (info.Movies.Count == 0) { return; }
            info.BaseDirectory = path;
            foundmovies.Add(info);
        }

        public static bool IsValidMovie(DirectoryInfo folder)
        {
            return GetVideosFromFolder(folder, true).Count > 0;
        }

        private static ObservableCollection<FileInfo> GetVideosFromFolder(DirectoryInfo folder, bool ignoresamples = true)
        {
            ObservableCollection<FileInfo> result = new ObservableCollection<FileInfo>();
            foreach (FileInfo fi in folder.GetFiles("*.*", SearchOption.AllDirectories))
            {
                if (MovieExtensions.Contains(fi.Extension.ToUpper()) && (ignoresamples && !fi.Name.ToUpper().Contains("SAMPLE")))
                {
                    result.Add(fi);
                }
            }
            return result;
        }

        public void RemoveEntry(object entry)
        {
            if (entry is Data.MovieInfo)
            {
                FoundMovies.Remove((MovieInfo)entry);
            }
            else
            {
                FileInfo fi = (FileInfo)entry;
                foreach (MovieInfo i in FoundMovies)
                {
                    if (i.Movies.Contains(fi))
                    {
                        i.Movies.Remove(fi);
                        break;
                    }
                }
            }
        }

        public void RemoveAllMovies()
        {
            FoundMovies.Clear();
        }

        public async void ImportMovies(MahApps.Metro.Controls.MetroWindow win, bool CloseAfterFinished = false)
        {
            ProgressDialogController controller = await win.ShowProgressAsync("Importiere", "", false);
            System.Threading.Thread t = new System.Threading.Thread(async () =>
            {
                TMDbClient client = new TMDbClient("c95eb0cc8533fa0f989fec4c8fc563ce");
                client.DefaultLanguage = "de";
                DirectoryInfo di = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images"));
                if (!di.Exists) di.Create();

                for (int i = 0; i < FoundMovies.Count; i++)
                {
                    MovieInfo m = foundmovies[i];
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        controller.SetMessage(string.Format("{0} ({1} von {2})", m.BaseDirectory.Name, i, FoundMovies.Count));
                        double progress = (double)i / (double)foundmovies.Count;
                        controller.SetProgress(progress);
                    });

                    MovieHelper helper = new MovieHelper
                    {
                        ReplaceUmlauts = this.ReplaceUmlauts,
                        ReplaceCodec = this.ReplaceCodec,
                        ReplaceGroup = this.ReplaceGroup,
                        ReplaceLanguage = this.ReplaceLanguage,
                        ReplaceQualitySetting = this.ReplaceQualitySetting,
                        ReplaceYear = this.ReplaceYear,
                    };
                    string name = helper.GetName(UseFolderName ? m.BaseDirectory.Name : m.Movies[0].Name);
                    int movieid;

                    SearchContainer<SearchMovie> results = client.SearchMovie(name);

                    if (results.Results == null || results.Results.Count == 0 || UseFirstMovieFirst || !(results.Results[0].Title.ToUpper() == name.ToUpper() || !(results.Results[0].OriginalTitle.ToUpper() == name.ToUpper()) || (results.Results.Count > 1 && results.Results[1].Title.ToUpper() == name.ToUpper() || results.Results[1].OriginalTitle.ToUpper() == name.ToUpper())))
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Views.SelectMovieWindow selectmoviewindow = new Views.SelectMovieWindow();
                            selectmoviewindow.Owner = win;
                            ViewModels.SelectMovieViewModel.Instance.UpdateList(results);
                            ViewModels.SelectMovieViewModel.Instance.SearchText = name;
                            ViewModels.SelectMovieViewModel.Instance.AllowSkip = true;
                            if (results.Results.Count > 0){ ViewModels.SelectMovieViewModel.Instance.SetSelection(results.Results[0]);}
                            selectmoviewindow.ShowDialog();
                        });
                        if (ViewModels.SelectMovieViewModel.Instance.Result == ViewModels.SelectMovieResult.OK)
                        {
                            movieid = ViewModels.SelectMovieViewModel.Instance.SelectedMovie.SearchMovie.Id;
                        }
                        else if (ViewModels.SelectMovieViewModel.Instance.Result == ViewModels.SelectMovieResult.JustAdd)
                        {
                            movieid = -1;
                        }
                        else
                        {
                            if (ViewModels.MainViewModel.Instance.MovieData.SkippedMovies == null) ViewModels.MainViewModel.Instance.MovieData.SkippedMovies = new List<string>();
                            ViewModels.MainViewModel.Instance.MovieData.SkippedMovies.Add(m.BaseDirectory.FullName);
                            continue;
                        }
                    }
                    else
                    {
                        movieid = results.Results[0].Id;
                    }
                    Movie movie = new Movie();
                    long duration = 0;

                    MediaFile mediafile = new MediaFile(m.Movies[0].FullName);
                    if (mediafile.Audio.Count > 0)
                    {
                        movie.AudioBitrate = mediafile.Audio[0].Bitrate;
                        movie.AudioCodec = mediafile.Audio[0].CodecID;
                        movie.Channels = mediafile.Audio[0].Channels;
                    }
                    if (mediafile.Video.Count > 0)
                    {
                        movie.VideoBitrate = mediafile.Video[0].Bitrate;
                        movie.VideoCodec = mediafile.Video[0].CodecID;
                        movie.FrameRate = mediafile.Video[0].FrameRate;
                        movie.Resolution = mediafile.Video[0].FrameSize;
                        movie.FormatID = mediafile.Video[0].FormatID;
                        duration = mediafile.Video[0].DurationMillis;
                    }
                    if (m.Movies.Count > 1)
                    {
                        for (int index = 1; index < m.Movies.Count; index++)
                        {
                            MediaFile newmediafile = new MediaFile(m.Movies[index].FullName);
                            if (newmediafile.Video.Count > 0)
                            {
                                duration += newmediafile.Video[0].DurationMillis;
                            }
                        }
                    }
                    movie.Playtime = TimeSpan.FromMilliseconds(duration).ToString();
                    movie.BaseDirectory = m.BaseDirectory.FullName;
                    movie.Paths = new List<MoviePath>();
                    if (m.Movies.Count == 1)
                    {
                        movie.Paths.Add(new MoviePath { path = m.Movies[0].FullName, title = "Abspielen" });
                    }
                    else
                    {
                        for (int index = 0; index < m.Movies.Count; index++)
                        {
                            movie.Paths.Add(new MoviePath { path = m.Movies[index].FullName, title = string.Format("Teil {0}", index + 1) });
                        }
                    }
                    if (movieid == -1)
                    {
                        movie.Title = name;
                        Application.Current.Dispatcher.Invoke(() => ViewModels.MainViewModel.Instance.MovieData.Movies.Add(movie));
                        continue;
                    }

                    TMDbLib.Objects.Movies.Movie tmovie = client.GetMovie(movieid, TMDbLib.Objects.Movies.MovieMethods.Credits | MovieMethods.Trailers);
                    movie.LoadInfos(tmovie, di.FullName);
                    Application.Current.Dispatcher.Invoke(new Action(() => ViewModels.MainViewModel.Instance.MovieData.Movies.Add(movie)));
                }
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ViewModels.MainViewModel.Instance.MovieData.Movies = new ObservableCollection<Movie>(ViewModels.MainViewModel.Instance.MovieData.Movies.OrderBy(i => i.Title));

                });
                ViewModels.MainViewModel.Instance.MovieData.MoviesImported();
                await controller.CloseAsync();
                await Application.Current.Dispatcher.Invoke(async () => await win.ShowMessageAsync("Erfolgreich", string.Format("{0} Filme wurde erfolgreich importiert", FoundMovies.Count), MessageDialogStyle.Affirmative));
                if (CloseAfterFinished)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        win.Close();
                    });
                }
            });
            t.IsBackground = true;
            t.Start();
        }
    }

    class MovieInfo
    {
        public DirectoryInfo BaseDirectory { get; set; }

        public ObservableCollection<FileInfo> Movies { get; set; }

        public override string ToString()
        {
            return BaseDirectory.Name;
        }
    }
}
