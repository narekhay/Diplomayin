using Microsoft.Phone.Controls;
using PianoPhone.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PianoPhone.ViewModels
{
    public class ViewModel
    {
       ObservableCollection<CollectionControlModel> Items { get; set; }

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
               OnViewModelLayoutChanged(this);
           }
       }

       bool isSelectionEnabled;
       public bool IsSelectionEnabled
       {
           get { return isSelectionEnabled; }
           set
           {
               isSelectionEnabled = value;
               OnViewModelLayoutChanged(this);
           }
       }

        public delegate void ViewModelLayoutChangedEventHandler(object sender);
        public event ViewModelLayoutChangedEventHandler ViewModelLayoutChanged;
        void OnViewModelLayoutChanged(object sender)
        {
            if (ViewModelLayoutChanged != null)
            {
                ViewModelLayoutChanged(sender);
            }
        }


        //#region INotifyPropertyChanged
        //public event PropertyChangedEventHandler PropertyChanged;
        //protected void OnPropertyChanged(string p)
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged(this, new PropertyChangedEventArgs(p));
        //    }
        //}
        //#endregion

    }


}
