using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Viddy.Controls
{
    public class ProfilePictureControl : Control
    {
        public static readonly DependencyProperty TappedCommandProperty = DependencyProperty.Register(
            "TappedCommand", typeof (ICommand), typeof (ProfilePictureControl), new PropertyMetadata(default(ICommand)));

        public ICommand TappedCommand
        {
            get { return (ICommand) GetValue(TappedCommandProperty); }
            set { SetValue(TappedCommandProperty, value); }
        }

        public static readonly DependencyProperty IsChangingProperty = DependencyProperty.Register(
            "IsChanging", typeof (bool), typeof (ProfilePictureControl), new PropertyMetadata(default(bool)));

        public bool IsChanging
        {
            get { return (bool) GetValue(IsChangingProperty); }
            set { SetValue(IsChangingProperty, value); }
        }

        public static readonly DependencyProperty AvatarUrlProperty = DependencyProperty.Register(
            "AvatarUrl", typeof (string), typeof (ProfilePictureControl), new PropertyMetadata(default(string)));

        public string AvatarUrl
        {
            get { return (string) GetValue(AvatarUrlProperty); }
            set { SetValue(AvatarUrlProperty, value); }
        }

        public ProfilePictureControl()
        {
            DefaultStyleKey = typeof (ProfilePictureControl);
        }
    }
}
