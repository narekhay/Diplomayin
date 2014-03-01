using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Media;
using PianoPhone.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PianoPhone.ViewModels
{
    public class AlbumsViewModel : IViewModel
    {
        public AlbumsViewModel()
        {
            Items = new ObservableCollection<CollectionControlModel>();
            LayoutMode = LongListSelectorLayoutMode.Grid;
        }

        LongListSelectorLayoutMode layoutMode;
        public LongListSelectorLayoutMode LayoutMode
        {
            get
            {
                return layoutMode;
            }
            set
            {
                layoutMode = value;
                OnPropertyChanged("LayoutMode");
            }
        }

        bool isSelectionEnabled;
        public bool IsSelectionEnabled
        {
            get { return isSelectionEnabled; }
            set
            {
                isSelectionEnabled = value;
                OnPropertyChanged("IsSelectionEnabled");
            }
        }



        public async void LoadAlbumsFromMediaLibrary()
        {
            using (var mediaLibrary = new MediaLibrary())
            {
                PictureAlbumCollection allAlbums = mediaLibrary.RootPictureAlbum.Albums;
                foreach (var album in allAlbums)
                {
                    Items.Add(new CollectionControlModel()
                    {
                        FileName = Path.GetFileNameWithoutExtension(album.Name),
                        Data = album
                    });

                    if (album.Pictures.Count == 0)
                    {
                        //set default thumbnail
                        continue;
                    }
                    using (Stream str = album.Pictures.First().GetThumbnail())
                    {
                        byte[] buffer = new byte[str.Length];
                        await str.ReadAsync(buffer, 0, buffer.Length);
                        MemoryStream ms = new MemoryStream();
                        await ms.WriteAsync(buffer, 0, buffer.Length);
                        Items.Last().Thumbnail = new BitmapImage();
                        Items.Last().Thumbnail.SetSource(ms);

                    }
                }
            }
        }

        public ObservableCollection<CollectionControlModel> Items
        {
            get;
            set;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string p)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(p));
            }
        }
        #endregion
    }
}
