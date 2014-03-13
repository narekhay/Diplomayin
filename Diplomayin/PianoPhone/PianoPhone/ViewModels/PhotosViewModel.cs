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
    public class PhotosViewModel: ViewModel
    {
        public PhotosViewModel()
        {
            LayoutMode = LongListSelectorLayoutMode.Grid;
            
            Items = new ObservableCollection<CollectionControlModel>();

        }

        public async void Initialize(PictureAlbum album, CancellationToken token, bool fromLibrary = true)
        {
            if (fromLibrary)
            {
                IsSelectionEnabled = true;
                await LoadAlbumsFromMediaLibraryAsync(album, token);
            }
            else
            {
                IsSelectionEnabled = false;
                await LoadAlbumPhotosFromStoreAsync(album.Name, token);
            }
        }

        public async Task LoadAlbumsFromMediaLibraryAsync(PictureAlbum album, CancellationToken token)
        {
            ObservableCollection<CollectionControlModel> newItems = new ObservableCollection<CollectionControlModel>();
            foreach (var photo in album.Pictures)
            {
                Log(photo.Name);
               // await Task.Delay(new TimeSpan(0,0,0,0,10));
                token.ThrowIfCancellationRequested();
                Log(photo.Name + "50");
                newItems.Add(new CollectionControlModel()
                {
                    FileName = Path.GetFileNameWithoutExtension(photo.Name),
                    Data = photo
                });
                Log(photo.Name);
                token.ThrowIfCancellationRequested();
                using (Stream str = photo.GetThumbnail())
                {
                   
                    Log(photo.Name + "60");
                    byte[] buffer = new byte[str.Length];
                    await str.ReadAsync(buffer, 0, buffer.Length);
                    Log(photo.Name + "63");
                    token.ThrowIfCancellationRequested();
                    using (MemoryStream ms = new MemoryStream())
                    {
                        Log(photo.Name + "67");
                        await ms.WriteAsync(buffer, 0, buffer.Length);
                        token.ThrowIfCancellationRequested();
                        newItems.Last().Thumbnail = new BitmapImage();
                        newItems.Last().Thumbnail.SetSource(ms);
                        Log(photo.Name + "72");
                    }
                    //await Task.Delay(new TimeSpan(0, 0, 0, 0, 10));
                    //Thread.Sleep(1000);
                }
            }
            foreach (var item in newItems)
            {
                Items.Add(item);
            }
        }

        void Log(params string[] a )
    {
        foreach (var item in a)
        {
            System.Diagnostics.Debug.WriteLine(item.ToString());
        }
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

       
    }
}
