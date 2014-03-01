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
using Microsoft.Xna.Framework.Media;
using System.Windows.Media.Imaging;

namespace PianoPhone.Views
{
    public partial class UserDataPage : PhoneApplicationPage
    {
        public UserDataPage()
        { 
            InitializeComponent();
            photosCollectionControl.CellSelected += photosCollectionControl_CellSelected;
            AlbumsCollectionViewModel = new AlbumsViewModel();
            photosCollectionControl.Initialize(AlbumsCollectionViewModel);
            AlbumsCollectionViewModel.LoadAlbumsFromMediaLibrary();
        }

        public int deep = 0;
        public static Picture SelectedImage;
        
        void photosCollectionControl_CellSelected(object myObject, CellSelectedEventArgs myArgs)
        {
            if (selecting)
            {
                switch (myArgs.Type)
                {
                    case CellType.Album:
                        
                        break;
                    case CellType.Photo:
                        break;
                    default:
                        break;
                }
                ListBox l = new ListBox();
 
            }
            switch (myArgs.Type)
            {
                case CellType.Album:
                    PhotosCollectionViewModel = new PhotosViewModel();
                    PhotosCollectionViewModel.Initialize(myArgs.SelectedItem.Data as PictureAlbum);
                    photosCollectionControl.Initialize(PhotosCollectionViewModel);
                    deep++;
                    break;

                case CellType.Photo:
                    SelectedImage = (myArgs.SelectedItem.Data as Picture);
                   NavigationService.Navigate(new Uri("/PhotoViewerPage.xaml", UriKind.Relative));
                   break;

                default:
                    break;
            }
        }

        public void BackKeyPressed()
        {
            
        }
        PhotosViewModel PhotosCollectionViewModel { get; set; }
        public AlbumsViewModel AlbumsCollectionViewModel { get; set; }
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (deep == 0)
                base.OnBackKeyPress(e);
            else
            {
                e.Cancel = true;
                deep--;
                photosCollectionControl.Initialize(AlbumsCollectionViewModel);
            }
        }

        bool selecting;
        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            if (selecting)
            {
                selecting = false;
            }
            else
            {
                selecting = true;
            }
        }
    }
}