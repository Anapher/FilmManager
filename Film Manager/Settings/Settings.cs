using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Film_Manager.ViewModelBase;
using Ookii.Dialogs.Wpf;
using Newtonsoft.Json;
using System.IO;

namespace Film_Manager.Settings
{
    class Settings : PropertyChangedBase
    {
        public Settings(string path)
        {
            this.Path = path;
        }

        public Settings()
        {

        }

        protected string Path { get; set; }

        public static Settings Load(string path)
        {
            Settings result = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(path));
            result.Path = path;
            return result;
        }

        public void Save()
        {
            JsonSerializer serializer = new JsonSerializer();

            using (StreamWriter sw = new StreamWriter(Path))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, this);
            }
        }

        private bool synchronize;
        public bool Synchronize
        {
            get { return synchronize; }
            set
            {
                SetProperty(value, ref synchronize);
            }
        }


        private ObservableCollection<string> foldertosynchronize;
        public ObservableCollection<string> FolderToSynchronize
        {
            get { return foldertosynchronize; }
            set
            {
                SetProperty(value, ref foldertosynchronize);
            }
        }

        private RelayCommand addFolder;
        [JsonIgnore]
        public RelayCommand AddFolder
        {
            get
            {
                if (addFolder == null)
                    addFolder = new RelayCommand((object parameter) =>
                    {
                        VistaFolderBrowserDialog fbd = new VistaFolderBrowserDialog();
                        fbd.ShowNewFolderButton = false;
                        if (fbd.ShowDialog() == true)
                        {
                            if (FolderToSynchronize == null) { FolderToSynchronize = new ObservableCollection<string>(); }
                            FolderToSynchronize.Add(fbd.SelectedPath);
                        }
                    });
                return addFolder;
            }
        }

        private RelayCommand removefolder;
        [JsonIgnore]
        public RelayCommand RemoveFolder
        {
            get
            {
                if (removefolder == null)
                    removefolder = new RelayCommand((object parameter) => { if (parameter != null && FolderToSynchronize != null) { FolderToSynchronize.Remove(parameter.ToString()); } });
                return removefolder;
            }
        }


        private String websitepath;
        public String WebsitePath
        {
            get { return websitepath; }
            set
            {
                SetProperty(value, ref websitepath);
            }
        }

        private RelayCommand selectwebsitepath;
        [JsonIgnore]
        public RelayCommand SelectWebsitePath
        {
            get
            {
                if (selectwebsitepath == null)
                    selectwebsitepath = new RelayCommand((object parameter) =>
                    {
                        VistaFolderBrowserDialog fbd = new VistaFolderBrowserDialog();
                        fbd.ShowNewFolderButton = true;
                        if (fbd.ShowDialog() == true)
                        {
                            WebsitePath = fbd.SelectedPath;
                        }
                    });
                return selectwebsitepath;
            }
        }


        private bool autosyncwebsite;
        public bool AutoSyncWebsite
        {
            get { return autosyncwebsite; }
            set
            {
                SetProperty(value, ref autosyncwebsite);
            }
        }
    }
}
