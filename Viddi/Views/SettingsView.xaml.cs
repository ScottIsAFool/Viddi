using Windows.UI.Xaml.Navigation;

namespace Viddi.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsView
    {
        protected override NavigationCacheMode NavCacheMode
        {
            get { return NavigationCacheMode.Required; }
        }

        public SettingsView()
        {
            InitializeComponent();
        }
    }
}
