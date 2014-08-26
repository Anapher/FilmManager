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
using System.Windows.Shapes;

namespace Film_Manager.Views
{
    /// <summary>
    /// Interaktionslogik für ImportMoviesWindow.xaml
    /// </summary>
    public partial class ImportMoviesWindow : MahApps.Metro.Controls.MetroWindow
    {
        public ImportMoviesWindow()
        {
            InitializeComponent();
            ViewModels.ImportMoviesViewModel.Instance.BaseWindow = this;
        }
    }
}
