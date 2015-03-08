using System;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Viddi.Controls
{
    public class AboutPerson : Control
    {
        public static readonly DependencyProperty PersonNameProperty = DependencyProperty.Register(
            "PersonName", typeof (string), typeof (AboutPerson), new PropertyMetadata(default(string)));

        public string PersonName
        {
            get { return (string) GetValue(PersonNameProperty); }
            set { SetValue(PersonNameProperty, value); }
        }

        public static readonly DependencyProperty TwitterProperty = DependencyProperty.Register(
            "Twitter", typeof (string), typeof (AboutPerson), new PropertyMetadata(default(string)));

        public string Twitter
        {
            get { return (string) GetValue(TwitterProperty); }
            set { SetValue(TwitterProperty, value); }
        }

        public static readonly DependencyProperty AvatarProperty = DependencyProperty.Register(
            "Avatar", typeof(Uri), typeof(AboutPerson), new PropertyMetadata(default(Uri)));

        public Uri Avatar
        {
            get { return (Uri)GetValue(AvatarProperty); }
            set { SetValue(AvatarProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command", typeof (ICommand), typeof (AboutPerson), new PropertyMetadata(default(ICommand)));

        public ICommand Command
        {
            get { return (ICommand) GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public AboutPerson()
        {
            DefaultStyleKey = typeof (AboutPerson);
        }
    }
}
