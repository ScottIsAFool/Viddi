using System.Linq;
using Windows.UI.Xaml.Navigation;

namespace Viddi.Views.Account
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ManualLoginView
    {
        public ManualLoginView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode != NavigationMode.Back)
            {
                var page = Frame.BackStack.FirstOrDefault(x => x.SourcePageType == GetType());
                if (page != null)
                {
                    Frame.BackStack.Remove(page);
                }
            }
        }
    }
}
