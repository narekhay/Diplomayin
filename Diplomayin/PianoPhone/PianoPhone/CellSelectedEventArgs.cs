using Microsoft.Xna.Framework.Media;
using PianoPhone.Models;
using PianoPhone.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PianoPhone
{
 public   class CellSelectedEventArgs:EventArgs
    {
        public CollectionControlModel SelectedItem { get; private set; }
        public CellType Type { get; set; }
        public CellSelectedEventArgs(CollectionControlModel selectedItem, CellType type)
        {
            SelectedItem = selectedItem;
            Type = type;
        }
        
    }
}
