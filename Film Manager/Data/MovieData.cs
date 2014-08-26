using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;
using Newtonsoft.Json;
using System.Windows;

namespace Film_Manager.Data
{
    [Serializable]
    class MovieData : ViewModelBase.PropertyChangedBase
    {
        public event Action MoviesChanged;

        public MovieData()
        {

        }

        private string Path;

        public MovieData(string path)
        {
            this.Path = path;
            this.movies = new ObservableCollection<Movie>();
        }

        private ObservableCollection<Movie> movies;
        public ObservableCollection<Movie> Movies
        {
            get { return movies; }
            set
            {
                SetProperty(value, ref movies);
            }
        }

        public static MovieData LoadMovies(string path)
        {
            MovieData result = JsonConvert.DeserializeObject<MovieData>(File.ReadAllText(path));
            result.Path = path;
            return result;
        }

        public void CheckMovies(MahApps.Metro.Controls.MetroWindow win, string folder)
        {
            List<DirectoryInfo> directorystoimport = new List<DirectoryInfo>();
            foreach (DirectoryInfo d in new DirectoryInfo(folder).GetDirectories("*", SearchOption.TopDirectoryOnly))
            {
                if (!ContainsMovie(d))
                {
                    bool IsSkipped = false;
                    if (SkippedMovies != null)
                    {
                        foreach (string str in SkippedMovies)
                        {
                            if (str.ToUpper() == d.FullName.ToUpper())
                            {
                                IsSkipped = true;
                                break;
                            }
                        }
                    }
                    if (!IsSkipped && MovieImport.IsValidMovie(d))
                    {
                        directorystoimport.Add(d);
                    }
                    
                }
            }
            if (directorystoimport.Count == 0) return;

            MovieImport import = new MovieImport();
            import.FoundMovies = new ObservableCollection<MovieInfo>();
            foreach (DirectoryInfo di in directorystoimport)
            {
                import.ImportMovie(di);
            }
            import.ImportMovies(win);
        }

        private bool ContainsMovie(DirectoryInfo folder)
        {
            foreach (Data.Movie m in this.Movies)
            {
                if (folder.FullName == m.BaseDirectory) { return true; }
            }
            return false;
        }

        public List<string> SkippedMovies { get; set; }

        public void Save()
        {
            JsonSerializer serializer = new JsonSerializer();

            using (StreamWriter sw = new StreamWriter(Path))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, this);
            }
        }

        public void RemoveMovie(Movie movie)
        {
            Movies.Remove(movie);
            MoviesChanged();
        }

        private bool DontRaiseImportedEvent = false;
        public void MoviesImported()
        {
            if (!DontRaiseImportedEvent) { MoviesChanged(); }
        }
    }
}
