using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Viddy.Controls
{
    public class CustomHyperlinkButton : HyperlinkButton
    {
        public static readonly DependencyProperty TapCommandProperty = DependencyProperty.Register(
            "TapCommand", typeof (ICommand), typeof (CustomHyperlinkButton), new PropertyMetadata(default(ICommand)));

        public ICommand TapCommand
        {
            get { return (ICommand) GetValue(TapCommandProperty); }
            set { SetValue(TapCommandProperty, value); }
        }

        public CustomHyperlinkButton()
        {
            Tapped += OnTap;
        }

        private void OnTap(object sender, TappedRoutedEventArgs e)
        {
            if (TapCommand != null)
            {
                TapCommand.Execute(CommandParameter);
            }
        }
    }
}
