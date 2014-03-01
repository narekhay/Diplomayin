using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PianoPhone.Models
{
    public class CollectionControlModel:INotifyPropertyChanged
    {
        string fileName;
        public string FileName
        {
            get { return fileName; }
            set { fileName= value;
            OnPropertyChanged("FileName");
            }
        }

        BitmapSource thumbnail;
        public BitmapSource Thumbnail
        {
            get { return thumbnail; }
            set
            {
                thumbnail = value;
            OnPropertyChanged("Thumbnail");
            }
        }

        public object Data { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string p)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(p));
            }
        }
    }
}
