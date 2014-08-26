using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Film_Manager.Converter
{
    class FSKConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string name ="FSK";

            switch ((Data.FSK)value)
            {
                case Data.FSK.FSK0:
                    name = "FSK_0";
                    break;
                case Data.FSK.FSK6:
                    name = "FSK_6";
                    break;
                case Data.FSK.FSK12:
                    name = "FSK_12";
                    break;
                case Data.FSK.FSK16:
                    name = "FSK_16";
                    break;
                case Data.FSK.FSK18:
                    name = "FSK_18";
                    break;
            }

            return new BitmapImage(new Uri(string.Format("/Resources/fsk/{0}.png", name), UriKind.Relative));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
