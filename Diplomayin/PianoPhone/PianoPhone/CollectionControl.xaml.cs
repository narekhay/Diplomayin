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
using System.Windows.Data;

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
            longListSelector.LayoutMode = viewModel.LayoutMode;
            longListSelector.IsSelectionEnabled = viewModel.IsSelectionEnabled;
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

        public static readonly DependencyProperty LayoutModeProperty = DependencyProperty.Register("LayoutMode", typeof(double), typeof(LongListSelectorLayoutMode), new PropertyMetadata(SetLayoutMode));
        public static readonly DependencyProperty IsSelectionEnabledProperty = DependencyProperty.Register("IsSelectionEnabled", typeof(double), typeof(bool), new PropertyMetadata(SetIsSelectionEnabled));
        
        private static void SetLayoutMode(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((d as CollectionControl).DataContext as IViewModel).LayoutMode = (LongListSelectorLayoutMode)e.NewValue;
        }

        private static void SetIsSelectionEnabled(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((d as CollectionControl).DataContext as IViewModel).IsSelectionEnabled = (bool)e.NewValue;
        }

    }

    public enum CellType
    {
        Album,
        Photo
    }
}
