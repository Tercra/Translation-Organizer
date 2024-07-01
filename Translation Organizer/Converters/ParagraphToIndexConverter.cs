using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Translation_Organizer
{
    internal class ParagraphToIndexConverter:IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var paragraphList = (ObservableCollection<ParagraphModel>)values[0];
            var paragraph = (ParagraphModel)values[1];
            if(paragraphList == null || paragraph == null)
            {
                return "";
            }

            int index = paragraphList.IndexOf(paragraph) + 1;
            return index.ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
