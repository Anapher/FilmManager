using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Film_Manager.Data;
using System.Collections.ObjectModel;

namespace Film_Manager.Website
{
    class WebsiteGenerator
    {
        public void Generate(ObservableCollection<Movie> Movies, DirectoryInfo location)
        {
            DirectoryInfo diImages = new DirectoryInfo(Path.Combine(location.FullName, "covers"));
            diImages.Create();
            using (StreamWriter sw = new StreamWriter(Path.Combine(location.FullName, "index.html"), false, Encoding.UTF8))
            {
                sw.Write(string.Format(Properties.Resources.WebsiteHeader, Movies.Count));
                string newfilepath;
                foreach (Movie m in Movies)
                {
                    if (!string.IsNullOrEmpty(m.ImagePath))
                    {
                        FileInfo fiImage = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", m.ImagePath.Remove(0, 1)));
                        newfilepath = Path.Combine(diImages.Name, m.ImagePath.Remove(0, 1));
                        fiImage.CopyTo(Path.Combine(location.FullName, newfilepath), true);
                    }
                    else
                    {
                        newfilepath = Path.Combine("files", "nocover.png");
                    }

                    sw.WriteLine(string.Format(Properties.Resources.Table, newfilepath, "www.website.com", m.Title, m.Year.ToString("yyyy"), FSKToString(m.AgeRating), TimeSpan.Parse(m.Playtime).TotalMinutes, GenresToString(m.Genres), m.Description, m.ProductCountries == null ? string.Empty : string.Join(", ", m.ProductCompanies), m.Director, m.ProductCountries == null ? string.Empty : string.Join(", ", m.ProductCountries), m.OriginalTitle, m.Trailer));
                }
                sw.Write(Properties.Resources.WebsiteBottom);
            }
            DirectoryInfo difiles = new DirectoryInfo(Path.Combine(location.FullName, "files"));
            if (!difiles.Exists) { difiles.Create(); }
            File.WriteAllText(Path.Combine(difiles.FullName, "moviecat.js"), Properties.Resources.MovieCat);
            File.WriteAllText(Path.Combine(difiles.FullName, "black.css"), Properties.Resources.Black);
            File.WriteAllText(Path.Combine(difiles.FullName, "white.css"), Properties.Resources.White);
            File.WriteAllText(Path.Combine(difiles.FullName, "old.css"), Properties.Resources.Old);
            Properties.Resources.nocover.Save(Path.Combine(difiles.FullName, "nocover.png"), System.Drawing.Imaging.ImageFormat.Png);
        }

        private string FSKToString(FSK fsk)
        {
            switch (fsk)
            {
                case FSK.FSK0:
                    return "0";
                case FSK.FSK6:
                    return "6";
                case FSK.FSK12:
                    return "12";
                case FSK.FSK16:
                    return "16";
                case FSK.FSK18:
                    return "18";
            }
            return "Keine Ahnung";
        }

        private string GenresToString(List<string> lst)
        {
            if (lst == null || lst.Count == 0)
            {
                return string.Empty;
            }
            var newlst = new List<string>();
            foreach (string s in lst)
            {
                switch (s)
                {
                    case "Action":
                        newlst.Add("Action");
                        break;
                    case "Abenteuer":
                        newlst.Add("Adventure");
                        break;
                    case "Animation":
                        newlst.Add("Animation");
                        break;
                    case "Komödie":
                        newlst.Add("Comedy");
                        break;
                    case "Fantasy":
                        newlst.Add("Fantasy");
                        break;
                    case "Mystery":
                        newlst.Add("Mystery");
                        break;
                    case "Science Fiction":
                        newlst.Add("Sci-Fi");
                        break;
                    case "Short":
                        newlst.Add("Short");
                        break;
                    case "Thriller":
                        newlst.Add("Thriller");
                        break;
                }
            }
            return string.Join(" / ", newlst);
        }
    }
}
