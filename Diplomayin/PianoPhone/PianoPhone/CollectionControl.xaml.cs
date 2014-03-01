using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PianoPhone.ViewModels;
using PianoPhone.Models;
using Microsoft.Xna.Framework.Media;
using System.Collections;

namespace PianoPhone
{
    public partial class CollectionControl : UserControl
    {
        public CollectionControl()
        {
            InitializeComponent();
        }

        public void Initialize(IViewModel viewModel)
        {
            this.DataContext = viewModel;            
        }


        private void Grid_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (e.Handled)
                return;
            e.Handled = true;
            var model = (sender as Grid).DataContext as CollectionControlModel;
            CellType type = CellType.Photo;
            if(model.Data is Picture)
            {
                type = CellType.Photo;
            }
            else
            {
                if(model.Data is PictureAlbum)
                {
                   type = CellType.Album;
                }
            }

             OnCellSelected(this, new CellSelectedEventArgs(model, type ));
        }
        public delegate void CellSelectedEventHandler(object myObject,CellSelectedEventArgs myArgs);
        public event CellSelectedEventHandler CellSelected;
        void OnCellSelected(object sender, CellSelectedEventArgs e)
        {
            if (CellSelected != null)
                CellSelected(sender, e);
        }

        public List<CollectionControlModel> SelectedItems = new List<CollectionControlModel>();
        private void longListSelector_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            foreach (CollectionControlModel item in e.AddedItems)
            {
                SelectedItems.Add(item);
            }
        }

        public void ResetSelection()
        {
            SelectedItems.Clear();
        }

    }

    public enum CellType
    {
        Album,
        Photo
    }
}
