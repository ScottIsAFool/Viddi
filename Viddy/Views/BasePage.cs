using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Navigation;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Ioc;
using ThemeManagerRt;
using Viddi.ViewModel;

namespace Viddi.Views
{
    public class BasePage : ScottIsAFool.Windows.Controls.BasePage
    {
        public override INavigationService NavigationService
        {
            get { return SimpleIoc.Default.GetInstance<INavigationService>(); }
        }

        public BasePage()
        {
            this.ThemeEnableThisPage();
        }

        protected void SetFullScreen(ApplicationViewBoundsMode mode)
        {
            ApplicationView.GetForCurrentView().SetDesiredBoundsMode(mode);
        }

        protected override void InitialiseOnBack()
        {
            var backVm = DataContext as IBackSupportedViewModel;
            if (backVm != null)
            {
                backVm.ChangeContext(GetType());
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            var vm = DataContext as IBackSupportedViewModel;
            if (vm != null)
            {
                if (e.NavigationMode != NavigationMode.Back)
                {
                    vm.SaveContext();
                }
            }

            base.OnNavigatingFrom(e);
        }
    }
}
