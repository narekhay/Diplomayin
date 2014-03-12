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
using System.Threading;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework.Media;
using System.Threading.Tasks;

namespace PianoPhone.Views
{
    public partial class MediaLibraryMultiSelector : PhoneApplicationPage
    {
        public MediaLibraryMultiSelector()
        {
            InitializeViewModels();
            InitializeComponent();
            BuildApplicationBar();
        }

        CancellationTokenSource cts = new CancellationTokenSource();
        public ObservableCollection<IViewModel> ViewModels { get; set; }
        private void InitializeViewModels()
        {
            ViewModels = new ObservableCollection<IViewModel>();
            var albumsModel = new AlbumsViewModel();
            albumsModel.IsSelectionEnabled = false;
            albumsModel.Initialize(cts.Token);
            var contactsModel = new ContactsViewModel();
            contactsModel.IsSelectionEnabled = true;
            contactsModel.Initialize(cts.Token);
            ViewModels.Add(albumsModel);
            ViewModels.Add(contactsModel);
        }

        private void BuildApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();
            CreateSelectionAppBar();
            // Create a new menu item with the localized string from AppResources.
            // ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
            // ApplicationBar.MenuItems.Add(appBarMenuItem);
        }

        private void CreateSelectionAppBar()
        {
            // Create a new button and set the text value to the localized string from AppResources.
            ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
            appBarButton.Text = "select";
            appBarButton.Click += appBarButton_Click;
            if (ApplicationBar.Buttons.Count == 0)
                ApplicationBar.Buttons.Add(appBarButton);
            else
                ApplicationBar.Buttons[0] = appBarButton;
        }

        private void CreateCompletedAppBar()
        {
            // Create a new button and set the text value to the localized string from AppResources.
            ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
            appBarButton.Text = "complete";
            appBarButton.Click +=appBarButton_Click;
            if (ApplicationBar.Buttons.Count == 0)
                ApplicationBar.Buttons.Add(appBarButton);
            else
                ApplicationBar.Buttons[0] = appBarButton;
        }

        void appBarButton_Click(object sender, EventArgs e)
        {
            if ((sender as ApplicationBarIconButton).Text == "select")
            {
                ViewModels[pivot.SelectedIndex].IsSelectionEnabled = true;
                pivot.IsEnabled = false;
                CreateCompletedAppBar();
            }
            else
            {
                FileManager.SaveItems(((pivot.SelectedItem as PivotItem).Content as CollectionControl).SelectedItems);
            }
         
        }

        int deep = 0;
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if(ViewModels[pivot.SelectedIndex].IsSelectionEnabled == true)
            {
                e.Cancel = true;
                ViewModels[pivot.SelectedIndex].IsSelectionEnabled = false;
                pivot.IsEnabled = true;
                CreateSelectionAppBar();
                return;
            }
            if (deep == 0)
                base.OnBackKeyPress(e);
            else
            {
                e.Cancel = true;
                deep--;
                if (pivot.SelectedIndex == 0)
                {
                    ViewModels[0] = new AlbumsViewModel();
                    ((pivot.SelectedItem as PivotItem).Content as CollectionControl).Initialize(ViewModels[0]);
                }
            }
        }

        private void pivot_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            return;
            if ((((pivot.SelectedItem as PivotItem).Content as CollectionControl).DataContext as IViewModel).IsSelectionEnabled)
                CreateCompletedAppBar();
            else
                CreateSelectionAppBar();
        }

        private void CollectionControl_CellSelected_1(object sender, CellSelectedEventArgs e)
        {
            if (pivot.SelectedIndex == 0)
            {
                if (e.Type == CellType.Album)
                {
                    var pModel = new PhotosViewModel();
                    pModel.Initialize(e.SelectedItem.Data as PictureAlbum, cts.Token);
                    ViewModels[0] = pModel;
                    ((pivot.SelectedItem as PivotItem).Content as CollectionControl).Initialize(ViewModels[0]);
                    CreateCompletedAppBar();
                }
            }
        }

        private void CollectionControl_CellSelected_2(object myObject, CellSelectedEventArgs myArgs)
        {

        }
    }
}
