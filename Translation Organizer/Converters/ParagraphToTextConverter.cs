using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Translation_Organizer
{
    internal class ParagraphToTextConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ObservableCollection<string> paragraph = (ObservableCollection<string>)value;
            StringBuilder s = new StringBuilder();
            foreach(string sentence in  paragraph)
            {
                s.Append(sentence + " ");
            }

            return s.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
