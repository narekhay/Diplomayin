using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Media;
using PianoPhone.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Windows.Storage;


namespace PianoPhone.ViewModels
{
    public class PhotosViewModel: IViewModel
    {
        public PhotosViewModel()
        {
            Items = new ObservableCollection<CollectionControlModel>();
        }

        public Task Initialize(PictureAlbum album, CancellationToken token, bool fromLibrary = true)
        {
            if (fromLibrary)
                return LoadAlbumsFromMediaLibraryAsync(album, token);
            else
                return LoadAlbumPhotosFromStoreAsync(album.Name, token);
        }

        public Task LoadAlbumsFromMediaLibraryAsync(PictureAlbum album, CancellationToken token)
        {
            return Task.Run(async () =>
                {
                    foreach (var photo in album.Pictures)
                    {
                        token.ThrowIfCancellationRequested();
                        Items.Add(new CollectionControlModel()
                        {
                            FileName = Path.GetFileNameWithoutExtension(photo.Name),
                            Data = photo
                        });
                        token.ThrowIfCancellationRequested();
                        using (Stream str = photo.GetThumbnail())
                        {
                            byte[] buffer = new byte[str.Length];
                            await str.ReadAsync(buffer, 0, buffer.Length);
                            token.ThrowIfCancellationRequested();
                            MemoryStream ms = new MemoryStream();
                            await ms.WriteAsync(buffer, 0, buffer.Length);
                            token.ThrowIfCancellationRequested();
                            Items.Last().Thumbnail = new BitmapImage();
                            Items.Last().Thumbnail.SetSource(ms);
                        }
                    }
                },token);
        }

        public Task LoadAlbumPhotosFromStoreAsync(string albumName, CancellationToken token)
        {
            return Task.Run(async () =>
            {
                token.ThrowIfCancellationRequested();
                var resultFiles = await FileManager.GetPhotosAsync(albumName, token);
                token.ThrowIfCancellationRequested();
                foreach (var photo in resultFiles)
                {
                    using (Stream fileStream = photo.Data)
                    {
                        if (fileStream != null)
                        {
                            Items.Add(new CollectionControlModel()
                            {
                                FileName = photo.Name
                            });
                            token.ThrowIfCancellationRequested();
                            Items.Last().Thumbnail.SetSource(fileStream);
                        }
                    }
                    token.ThrowIfCancellationRequested();
                }
            });
        }

        public ObservableCollection<CollectionControlModel> Items
        {
            get;
            set;
        }

        bool isSelectionEnabled;
        public bool IsSelectionEnabled
        {
            get
            {
                return isSelectionEnabled;
            }
            set
            {
                isSelectionEnabled = value;
                OnPropertyChanged("IsSelectionEnabled");
            }
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
