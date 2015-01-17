using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Viddy.Controls
{
    public class ProfilePictureControl : Control
    {
        public static readonly DependencyProperty AvatarProperty = DependencyProperty.Register(
            "Avatar", typeof (ImageBrush), typeof (ProfilePictureControl), new PropertyMetadata(default(ImageBrush)));

        public ImageBrush Avatar
        {
            get { return (ImageBrush) GetValue(AvatarProperty); }
            set { SetValue(AvatarProperty, value); }
        }

        public static readonly DependencyProperty TappedCommandProperty = DependencyProperty.Register(
            "TappedCommand", typeof (ICommand), typeof (ProfilePictureControl), new PropertyMetadata(default(ICommand)));

        public ICommand TappedCommand
        {
            get { return (ICommand) GetValue(TappedCommandProperty); }
            set { SetValue(TappedCommandProperty, value); }
        }

        public ProfilePictureControl()
        {
            DefaultStyleKey = typeof (ProfilePictureControl);
        }
    }
}
