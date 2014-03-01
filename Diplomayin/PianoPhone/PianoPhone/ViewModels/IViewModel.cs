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
    public interface IViewModel:INotifyPropertyChanged
    {
       ObservableCollection<CollectionControlModel> Items { get; set; }

       bool IsSelectionEnabled { get; set; }
       LongListSelectorLayoutMode LayoutMode { get; set; }
    }


}
