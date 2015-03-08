using System;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Viddi.Controls
{
    public class ProfilePictureControl : Control
    {
        private Grid _placeholderGrid;
        private Ellipse _imageEllipse;

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
            "AvatarUrl", typeof (string), typeof (ProfilePictureControl), new PropertyMetadata(default(string), OnAvatarUrlChanged));

        public string AvatarUrl
        {
            get { return (string) GetValue(AvatarUrlProperty); }
            set { SetValue(AvatarUrlProperty, value); }
        }

        public static readonly DependencyProperty DisplayDefaultAvatarProperty = DependencyProperty.Register(
            "DisplayDefaultAvatar", typeof (bool), typeof (ProfilePictureControl), new PropertyMetadata(default(bool)));

        public bool DisplayDefaultAvatar
        {
            get { return (bool) GetValue(DisplayDefaultAvatarProperty); }
            set { SetValue(DisplayDefaultAvatarProperty, value); }
        }

        public static readonly DependencyProperty DisplayNotificationsProperty = DependencyProperty.Register(
            "DisplayNotifications", typeof (bool), typeof (ProfilePictureControl), new PropertyMetadata(default(bool)));

        public bool DisplayNotifications
        {
            get { return (bool) GetValue(DisplayNotificationsProperty); }
            set { SetValue(DisplayNotificationsProperty, value); }
        }

        private static void OnAvatarUrlChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var profile = sender as ProfilePictureControl;
            if (profile != null)
            {
                if (profile._placeholderGrid != null)
                {
                    profile._placeholderGrid.Visibility = Visibility.Visible;
                }
            }
        }

        public ProfilePictureControl()
        {
            DefaultStyleKey = typeof (ProfilePictureControl);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _imageEllipse = GetTemplateChild("ImageEllipse") as Ellipse;
            _placeholderGrid = GetTemplateChild("PlaceHolderGrid") as Grid;

            if (_imageEllipse != null)
            {
                var fill = _imageEllipse.Fill as ImageBrush;
                if (fill != null)
                {
                    fill.ImageOpened += FillOnImageOpened;
                }
            }
        }

        private void FillOnImageOpened(object sender, RoutedEventArgs routedEventArgs)
        {
            if (_placeholderGrid != null)
            {
                _placeholderGrid.Visibility = Visibility.Collapsed;
            }
        }
    }
}
