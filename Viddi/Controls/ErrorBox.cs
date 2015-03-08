using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Viddi.Controls
{
    public class ErrorBox : Control
    {
        public static readonly DependencyProperty ErrorMessageProperty = DependencyProperty.Register(
            "ErrorMessage", typeof (string), typeof (ErrorBox), new PropertyMetadata(default(string)));

        public string ErrorMessage
        {
            get { return (string) GetValue(ErrorMessageProperty); }
            set { SetValue(ErrorMessageProperty, value); }
        }

        public ErrorBox()
        {
            DefaultStyleKey = typeof (ErrorBox);
        }
    }
}
