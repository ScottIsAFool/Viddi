using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Viddy.Controls
{
    public class FollowingControl : CheckBox
    {
        public static readonly DependencyProperty FollowingTextProperty = DependencyProperty.Register(
            "FollowingText", typeof (string), typeof (FollowingControl), new PropertyMetadata(default(string)));

        public string FollowingText
        {
            get { return (string) GetValue(FollowingTextProperty); }
            set { SetValue(FollowingTextProperty, value); }
        }

        public FollowingControl()
        {
            DefaultStyleKey = typeof (FollowingControl);
        }
    }
}
