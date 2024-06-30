using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Translation_Organizer
{
    public class ParagraphModel : INotifyPropertyChanged
    {
        private readonly ObservableCollection<string> jpSentences;
        private readonly ObservableCollection<string> rmjSentences;
        private readonly ObservableCollection<string> enSentences;

        public ObservableCollection<string> JpSentences { get { return jpSentences; } }
        public ObservableCollection<string> RmjSentences { get { return rmjSentences; } }
        public ObservableCollection<string> EnSentences { get { return enSentences; } }


        public ParagraphModel()
        {
            jpSentences = new ObservableCollection<string>();
            jpSentences.CollectionChanged += JpSentences_CollectionChanged;
            rmjSentences = new ObservableCollection<string>();
            rmjSentences.CollectionChanged += RmjSentences_CollectionChanged;
            enSentences = new ObservableCollection<string>();
            enSentences.CollectionChanged += EnSentences_CollectionChanged;
        }
        //Moved the list instatiation out of the constructor because of the deserializer
        public void Init()
        {
            jpSentences.Add("");
            rmjSentences.Add("");
            enSentences.Add("");
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            if(this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void JpSentences_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            this.NotifyPropertyChanged(nameof(JpSentences));
        }
        private void RmjSentences_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            this.NotifyPropertyChanged(nameof(RmjSentences));
        }
        private void EnSentences_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            this.NotifyPropertyChanged(nameof(EnSentences));
        }
    }
}
