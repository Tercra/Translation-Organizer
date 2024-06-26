using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Translation_Organizer
{
    internal class IndicesToSentenceConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int pIndex = (int)values[0];
            int sIndex = (int)values[1];
            ObservableCollection<ParagraphModel> paragraphs = (ObservableCollection<ParagraphModel>)values[2];
            string sentenceType = (string)parameter;
            if(paragraphs == null)
            {
                return null;
            }
            switch (sentenceType)
            {
                case "jp":
                    return paragraphs[pIndex].JpSentences[sIndex];
                case "rmj":
                    return paragraphs[pIndex].RmjSentences[sIndex];
                case "en":
                    return paragraphs[pIndex].EnSentences[sIndex];
                default:
                    return paragraphs[pIndex].JpSentences[sIndex];
            }
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new object[] { Binding.DoNothing, Binding.DoNothing, Binding.DoNothing };
        }
    }
}
