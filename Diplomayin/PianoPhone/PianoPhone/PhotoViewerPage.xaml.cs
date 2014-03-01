using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PianoPhone.Views;
using System.IO;

namespace PianoPhone
{
    public partial class PhotoViewerPage : PhoneApplicationPage
    {
        public PhotoViewerPage()
        {
            InitializeComponent();
            this.Loaded += PhotoViewerPage_Loaded;
            
        }
        float scale = 1.5f;
        void PhotoViewerPage_Loaded(object sender, RoutedEventArgs e)
        {
            BitmapImage source = new BitmapImage();
            using (Stream s = (UserDataPage.SelectedImage.GetImage()))
            {
                source.SetSource(s);
                image.Source = source;
                viewPort.Width = source.PixelWidth*scale;
                viewPort.Height = source.PixelHeight * scale;
            }
        }
        bool scaled = false;
        private void Image_DoubleTap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Image source = (sender as Image);
            if (scaled)
            {
                (source.RenderTransform as CompositeTransform).ScaleX =1;
                (source.RenderTransform as CompositeTransform).ScaleY =1;
                scaled = false;
            }
            else
            {
                (source.RenderTransform as CompositeTransform).ScaleX *= scale;
                (source.RenderTransform as CompositeTransform).ScaleY *= scale;
                scaled = true;
            }
            
        }

        private void image_ManipulationDelta_1(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            if (e.PinchManipulation != null)
            {
                Image source = sender as Image;
                var transform = (source.RenderTransform as CompositeTransform);
                transform.CenterX = e.PinchManipulation.Original.Center.X;
                transform.CenterY = e.PinchManipulation.Original.Center.Y;

                transform.ScaleX = scaleX * e.PinchManipulation.CumulativeScale;
                transform.ScaleY = scaleY * e.PinchManipulation.CumulativeScale;
            }
        }

        double scaleX = 1, scaleY = 1;
        private void image_ManipulationStarted_1(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            var transform = (CompositeTransform)image.RenderTransform;
            scaleX = transform.ScaleX;
           scaleY = transform.ScaleY;
        }
    }
}