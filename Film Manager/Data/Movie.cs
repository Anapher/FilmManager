using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Film_Manager.ViewModelBase;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.General;
using System.IO;

namespace Film_Manager.Data
{
    [Serializable]
    class Movie : ViewModelBase.PropertyChangedBase
    {

        public Movie()
        {

        }

        #region Default

        private String title;
        public String Title
        {
            get { return title; }
            set
            {
                SetProperty(value, ref title);
            }
        }


        private string originaltitle;
        public string OriginalTitle
        {
            get { return originaltitle; }
            set
            {
                SetProperty(value, ref originaltitle);
            }
        }

        private FSK agerating;
        public FSK AgeRating
        {
            get { return agerating; }
            set
            {
                SetProperty(value, ref agerating);
            }
        }


        private String description;
        public String Description
        {
            get { return description; }
            set
            {
                SetProperty(value, ref description);
            }
        }

        public List<MoviePath> Paths { get; set; }

        #endregion

        #region Details

        private DateTime year;
        public DateTime Year
        {
            get { return year; }
            set
            {
                SetProperty(value, ref year);
            }
        }


        private String playtime;
        public String Playtime
        {
            get { return playtime; }
            set
            {
                SetProperty(value, ref playtime);
            }
        }


        private List<string> genres;
        public List<string> Genres
        {
            get { return genres; }
            set
            {
                SetProperty(value, ref genres);
            }
        }


        private List<string> productcompanies;
        public List<string> ProductCompanies
        {
            get { return productcompanies; }
            set
            {
                SetProperty(value, ref productcompanies);
            }
        }


        private List<Actor> actors;
        public List<Actor> Actors
        {
            get { return actors; }
            set
            {
                SetProperty(value, ref actors);
            }
        }


        private String imagepath;
        public String ImagePath
        {
            get { return imagepath; }
            set
            {
                SetProperty(value, ref imagepath);
            }
        }

        #endregion

        #region Media


        private string resolution;
        public string Resolution
        {
            get { return resolution; }
            set
            {
                SetProperty(value, ref resolution);
            }
        }


        private int videobitrate;
        public int VideoBitrate
        {
            get { return videobitrate; }
            set
            {
                SetProperty(value, ref videobitrate);
            }
        }


        private String videocodec;
        public String VideoCodec
        {
            get { return videocodec; }
            set
            {
                SetProperty(value, ref videocodec);
            }
        }


        private double framerate;
        public double FrameRate
        {
            get { return framerate; }
            set
            {
                SetProperty(value, ref framerate);
            }
        }


        private int audiobitrate;
        public int AudioBitrate
        {
            get { return audiobitrate; }
            set
            {
                SetProperty(value, ref audiobitrate);
            }
        }


        private string audiocodec;
        public string AudioCodec
        {
            get { return audiocodec; }
            set
            {
                SetProperty(value, ref audiocodec);
            }
        }


        private int channels;
        public int Channels
        {
            get { return channels; }
            set
            {
                SetProperty(value, ref channels);
            }
        }


        private String formatid;
        public String FormatID
        {
            get { return formatid; }
            set
            {
                SetProperty(value, ref formatid);
            }
        }


        private String director;
        public String Director
        {
            get { return director; }
            set
            {
                SetProperty(value, ref director);
            }
        }


        private String trailer;
        public String Trailer
        {
            get { return trailer; }
            set
            {
                SetProperty(value, ref trailer);
            }
        }


        private String tagline;
        public String Tagline
        {
            get { return tagline; }
            set
            {
                SetProperty(value, ref tagline);
            }
        }


        private List<string> productCountries;
        public List<string> ProductCountries
        {
            get { return productCountries; }
            set
            {
                SetProperty(value, ref productCountries);
            }
        }

        public string BaseDirectory { get; set; }

        #endregion

        private bool seen;
        public bool Seen
        {
            get { return seen; }
            set
            {
                SetProperty(value, ref seen);
            }
        }

        [Newtonsoft.Json.JsonIgnore]
        private System.Windows.Media.Imaging.BitmapImage image;
        [Newtonsoft.Json.JsonIgnore]
        public System.Windows.Media.Imaging.BitmapImage Image
        {
            get
            {
                if (image == null && ImagePath != null)
                {
                    image = new System.Windows.Media.Imaging.BitmapImage(new Uri(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", ImagePath.Remove(0, 1))));
                }
                return image;
            }
        }

        private RelayCommand opentrailer;
        [Newtonsoft.Json.JsonIgnore]
        public RelayCommand OpenTrailer
        {
            get
            {
                if (opentrailer == null)

                    opentrailer = new RelayCommand((object parameter) =>
                    {
                        if (string.IsNullOrEmpty(this.Trailer))
                        {
                            System.Diagnostics.Process.Start(string.Format("https://www.youtube.com/results?search_query={0}+trailer+deutsch", this.Title.Replace(" ", "+")));
                        }
                        else
                        {
                            System.Diagnostics.Process.Start(Trailer);
                        }
                    });
                return opentrailer;
            }
        }

        #region Load
        public void LoadInfos(TMDbLib.Objects.Movies.Movie tmovie, string ImageFolder)
        {
            if (tmovie.Trailers.Youtube.Count > 0)
            {
                this.Trailer = string.Format("https://www.youtube.com/watch?v={0}", tmovie.Trailers.Youtube[0].Source);
            }
            var countries = new List<string>();
            foreach (ProductionCountry country in tmovie.ProductionCountries)
            {
                countries.Add(country.Name);
            }
            this.ProductCountries = countries;
            foreach (Crew p in tmovie.Credits.Crew)
            {
                if (p.Job == "Director")
                {
                    this.Director = p.Name;
                    break;
                }
            }
            this.Tagline = tmovie.Tagline;
            this.Title = tmovie.Title;
            this.OriginalTitle = tmovie.OriginalTitle;
            var lst = new List<string>();

            foreach (Genre genre in tmovie.Genres)
            {
                lst.Add(genre.Name);
            }
            this.Genres = lst;
            this.Year = tmovie.ReleaseDate.Value;
            this.Description = tmovie.Overview;
            var lstactors = new List<Actor>();
            foreach (Cast cast in tmovie.Credits.Cast)
            {
                lstactors.Add(new Actor { Character = cast.Character, Name = cast.Name });
            }
            this.Actors = lstactors;
            var productcomp = new List<string>();
            foreach (ProductionCompany c in tmovie.ProductionCompanies)
            {
                productcomp.Add(c.Name);
            }
            this.ProductCompanies = productcomp;
            using (System.Net.WebClient wc = new System.Net.WebClient())
            {
                wc.DownloadFile(string.Format("http://image.tmdb.org/t/p/w500{0}", tmovie.PosterPath), Path.Combine(ImageFolder, tmovie.PosterPath.Remove(0, 1)));
            }
            this.ImagePath = tmovie.PosterPath;
            Data.FSK result = FSK.Nothing;
            if (!string.IsNullOrEmpty(tmovie.ImdbId))
            {
                using (System.Net.WebClient wc = new System.Net.WebClient())
                {
                    string s = wc.DownloadString(string.Format("http://www.imdb.com/title/{0}", tmovie.ImdbId));
                    System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"<span itemprop=""contentRating"">(?<rating>(.*?))<\/span>");
                    System.Text.RegularExpressions.MatchCollection matches = regex.Matches(s);
                    if (matches.Count > 0)
                    {
                        switch (matches[0].Groups["rating"].Value)
                        {
                            case "o.Al.":
                                result = FSK.FSK0;
                                break;
                            case "6":
                                result = FSK.FSK6;
                                break;
                            case "12":
                                result = FSK.FSK12;
                                break;
                            case "16":
                                result = FSK.FSK16;
                                break;
                            case "18":
                                result = FSK.FSK18;
                                break;
                        }
                    }
                }
            }
            this.AgeRating = result;
            image = null;
            OnPropertyChanged("Image");
        }
        #endregion
    }

    public enum FSK { FSK0, FSK6, FSK12, FSK16, FSK18, Nothing }

    public class MoviePath
    {
        public string path { get; set; }
        public string title { get; set; }

        private RelayCommand playmovie;
        public RelayCommand PlayMovie
        {
            get
            {
                if (playmovie == null)

                    playmovie = new RelayCommand((object parameter) =>
                    {
                        System.Diagnostics.Process.Start(path);
                    });
                return playmovie;
            }
        }
    }
}