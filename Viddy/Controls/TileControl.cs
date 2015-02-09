using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Viddy.Controls
{
    public class TileControl : Control
    {
        private Image _image;

        public static readonly DependencyProperty ImageUrlProperty = DependencyProperty.Register(
            "ImageUrl", typeof (string), typeof (TileControl), new PropertyMetadata(default(string), OnImageUrlChanged));

        public string ImageUrl
        {
            get { return (string) GetValue(ImageUrlProperty); }
            set { SetValue(ImageUrlProperty, value); }
        }

        private static void OnImageUrlChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var tile = sender as TileControl;
            tile.SetImage();
        }

        public TileControl()
        {
            DefaultStyleKey = typeof (TileControl);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _image = GetTemplateChild("TileImage") as Image;
            
            SetImage();
        }

        private void SetImage()
        {
            if (_image != null)
            {
                var bitmap = new BitmapImage(new Uri(ImageUrl, UriKind.Absolute));
                _image.Source = bitmap;
            }
        }
    }
}
