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

        public async void Initialize(PictureAlbum album)
        {
            await LoadAlbumsFromMediaLibrary(album);
        }

        public async Task LoadAlbumsFromMediaLibrary(PictureAlbum album)
        {
            foreach (var photo in album.Pictures)
            {
                Items.Add(new CollectionControlModel()
                {
                    FileName = Path.GetFileNameWithoutExtension(photo.Name),
                    Data = photo
                });
                using (Stream str = photo.GetThumbnail())
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

        public async void LoadPhotos()
        {
            StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            string folderName  ="PrivatePhotos";
            return;
            if (folder.CreateFolderAsync(folderName, CreationCollisionOption.FailIfExists).Status != Windows.Foundation.AsyncStatus.Error)
            {
                var resultFolder = await folder.GetFolderAsync(folderName);
                var resultFiles = await resultFolder.GetFilesAsync();
                foreach (var file in resultFiles)
                {
                    using (Stream fileStream = await file.OpenStreamForReadAsync())
                    {
                        if (fileStream != null)
                        {
                            Items.Add(new CollectionControlModel()
                            {
                               FileName = Path.GetFileNameWithoutExtension( file.Name)
                            });
                            Items.Last().Thumbnail.SetSource(fileStream);
                        }
                        fileStream.Close();
                        fileStream.Dispose();
                    }
                }
            }
            else
            {
            }
        }

        
        private async void AlbumPhotos(PictureAlbum album)
        {
        
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
