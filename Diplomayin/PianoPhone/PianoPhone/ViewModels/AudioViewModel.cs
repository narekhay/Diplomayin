using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using PianoPhone.Models;
using System.ComponentModel;
using Microsoft.Xna.Framework.Media;
using System.IO;

namespace PianoPhone.ViewModels
{
    //class AudioViewModel:IViewModel
    //{
    //    public ObservableCollection<CollectionControlModel> Items { get; set; }
    //    public bool IsSelectionEnabled { get; set; }

    //    public void LoadSongsMetaDataFromMediaLibrary()
    //    {
    //        using (var mediaLibrary = new MediaLibrary())
    //        {
    //            SongCollection allSongs = mediaLibrary.Songs;
    //            foreach (var song in allSongs)
    //            {
    //                Items.Add(new CollectionControlModel()
    //                {
    //                    FileName = Path.GetFileNameWithoutExtension(song.Name),
    //                    Data = song
    //                });
    //            }
    //        }
    //    }

    //    #region INotifyPropertyChanged
    //    public event PropertyChangedEventHandler PropertyChanged;
    //    private void OnPropertyChanged(string p)
    //    {
    //        if (PropertyChanged != null)
    //        {
    //            PropertyChanged(this, new PropertyChangedEventArgs(p));
    //        }
    //    }
    //    #endregion
    //}
}
