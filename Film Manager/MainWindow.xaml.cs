using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Film_Manager
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MahApps.Metro.Controls.MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Closing += ViewModels.MainViewModel.Instance.OnWindowClosing;
            ViewModels.MainViewModel.Instance.BaseWindow = this;
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModels.MainViewModel.Instance.Load();
            }
            catch (Exception)
            {
                
                throw;
            }
            
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Website.WebsiteGenerator generator = new Website.WebsiteGenerator();
            generator.Generate(ViewModels.MainViewModel.Instance.MovieData.Movies, new System.IO.DirectoryInfo(@"D:\Dokumente\Visual Studio 2013\Projects\Film Manager\test"));
        }
    }
}
