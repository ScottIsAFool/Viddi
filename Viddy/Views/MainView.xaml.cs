using Windows.UI.Xaml.Documents;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Ioc;
using Viddi.Views.Account;

namespace Viddi.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainView
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void SignInHyperlink_OnClick(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            Navigate<ManualLoginView>();
        }

        private static void Navigate<T>()
        {
            SimpleIoc.Default.GetInstance<INavigationService>().Navigate<T>();
        }

        private void CreateAccountHyperlink_OnClick(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            Navigate<CreateAccountView>();
        }
    }
}
