using System;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Ioc;
using Viddy.Views.Account;

namespace Viddy.Controls
{
    public class PageHeaderControl : Control
    {
        private ProfilePictureControl _profilePicture;

        public static readonly DependencyProperty HeaderTextProperty = DependencyProperty.Register(
            "HeaderText", typeof (string), typeof (PageHeaderControl), new PropertyMetadata(default(string)));

        public string HeaderText
        {
            get { return (string) GetValue(HeaderTextProperty); }
            set { SetValue(HeaderTextProperty, value); }
        }

        public static readonly DependencyProperty ProfilePictureEnabledProperty = DependencyProperty.Register(
            "ProfilePictureEnabled", typeof (bool), typeof (PageHeaderControl), new PropertyMetadata(default(bool)));

        public bool ProfilePictureEnabled
        {
            get { return (bool) GetValue(ProfilePictureEnabledProperty); }
            set { SetValue(ProfilePictureEnabledProperty, value); }
        }

        public static readonly DependencyProperty ProfilePictureVisibilityProperty = DependencyProperty.Register(
            "ProfilePictureVisibility", typeof (Visibility), typeof (PageHeaderControl), new PropertyMetadata(default(Visibility)));

        public Visibility ProfilePictureVisibility
        {
            get { return (Visibility) GetValue(ProfilePictureVisibilityProperty); }
            set { SetValue(ProfilePictureVisibilityProperty, value); }
        }

        public static readonly DependencyProperty TapCommandProperty = DependencyProperty.Register(
            "TapCommand", typeof (ICommand), typeof (PageHeaderControl), new PropertyMetadata(default(ICommand)));

        public ICommand TapCommand
        {
            get { return (ICommand) GetValue(TapCommandProperty); }
            set { SetValue(TapCommandProperty, value); }
        }

        public static readonly DependencyProperty AvatarChangingProperty = DependencyProperty.Register(
            "AvatarChanging", typeof (bool), typeof (PageHeaderControl), new PropertyMetadata(default(bool)));

        public bool AvatarChanging
        {
            get { return (bool) GetValue(AvatarChangingProperty); }
            set { SetValue(AvatarChangingProperty, value); }
        }

        public PageHeaderControl()
        {
            DefaultStyleKey = typeof (PageHeaderControl);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _profilePicture = GetTemplateChild("ProfilePictureControl") as ProfilePictureControl;
            if (_profilePicture != null)
            {
                _profilePicture.Tapped += ProfilePictureOnTapped;
            }
        }

        private static void ProfilePictureOnTapped(object sender, TappedRoutedEventArgs tappedRoutedEventArgs)
        {
            SimpleIoc.Default.GetInstance<INavigationService>().Navigate<NotificationsView>();
        }
    }
}
