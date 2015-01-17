using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Viddy.Controls
{
    public class CustomButton : Button
    {
        public static readonly DependencyProperty TapCommandProperty = DependencyProperty.Register(
            "TapCommand", typeof (ICommand), typeof (CustomButton), new PropertyMetadata(default(ICommand)));

        public ICommand TapCommand
        {
            get { return (ICommand) GetValue(TapCommandProperty); }
            set { SetValue(TapCommandProperty, value); }
        }

        public CustomButton()
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