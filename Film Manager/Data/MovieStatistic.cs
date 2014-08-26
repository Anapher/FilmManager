using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Film_Manager.Data
{
  public class MovieStatistic : ViewModelBase.PropertyChangedBase
    {
        public MovieStatistic(IEnumerable<Movie> movielist)
        {
            MovieCount = movielist.Count();

            TimeSpan TotalPlaytime = TimeSpan.FromSeconds(0);
            double TotalPlaytimeSecounds = 0;
            Int32 TotalVideoBitrate = 0;
            Int32 TotalAudioBitrate = 0;

            foreach (Movie m in movielist)
            {
                filescount += m.Paths.Count;
                foreach (MoviePath path in m.Paths)
                {
                    filessize += new System.IO.FileInfo(path.path).Length;
                }
                TimeSpan timespan = TimeSpan.Parse(m.Playtime);
                TotalPlaytime = TotalPlaytime.Add(timespan);
                TotalPlaytimeSecounds += timespan.TotalSeconds;
                TotalVideoBitrate += m.VideoBitrate;
                TotalAudioBitrate += m.AudioBitrate;
            }

            Playtime = string.Format("{0} Minuten", (int)(TotalPlaytime.TotalMinutes));
            OnPropertyChanged("FilesCount");
            OnPropertyChanged("FilesSize");
            AverageFilesize = FilesSize / MovieCount;
            AveragePlaytime = TimeSpan.FromSeconds(TotalPlaytimeSecounds / MovieCount).ToString(@"hh\:mm\:ss");
            AverageVideoBitrate = TotalVideoBitrate / MovieCount;
            AverageAudioBitrate = TotalAudioBitrate / MovieCount;
        }

        
        private int moviecount;
        public int MovieCount
        {
            get { return moviecount; }
            set
            {
                SetProperty(value, ref moviecount);
            }
        }

        
        private int filescount;
        public int FilesCount
        {
            get { return filescount; }
            set
            {
                SetProperty(value, ref filescount);
            }
        }

        
        private Int64 filessize;
        public Int64 FilesSize
        {
            get { return filessize; }
            set
            {
                SetProperty(value, ref filessize);
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

        
        private Int64 averagefilesize;
        public Int64 AverageFilesize
        {
            get { return averagefilesize; }
            set
            {
                SetProperty(value, ref averagefilesize);
            }
        }

        
        private int averagevideobitrate;
        public int AverageVideoBitrate
        {
            get { return averagevideobitrate; }
            set
            {
                SetProperty(value, ref averagevideobitrate);
            }
        }

        
        private int averageaudiobitrate;
        public int AverageAudioBitrate
        {
            get { return averageaudiobitrate; }
            set
            {
                SetProperty(value, ref averageaudiobitrate);
            }
        }

        
        private String averageplaytime;
        public String AveragePlaytime
        {
            get { return averageplaytime; }
            set
            {
                SetProperty(value, ref averageplaytime);
            }
        }
    }
}
