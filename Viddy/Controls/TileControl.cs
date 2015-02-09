using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Viddy.Controls
{
    public class TileControl : Control
    {
        public static readonly DependencyProperty ImageUrlProperty = DependencyProperty.Register(
            "ImageUrl", typeof (string), typeof (TileControl), new PropertyMetadata(default(string)));

        public string ImageUrl
        {
            get { return (string) GetValue(ImageUrlProperty); }
            set { SetValue(ImageUrlProperty, value); }
        }

        public TileControl()
        {
            DefaultStyleKey = typeof (TileControl);
        }
    }
}
