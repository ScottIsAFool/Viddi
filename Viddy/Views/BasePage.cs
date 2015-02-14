using System.Linq;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Navigation;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Ioc;
using ScottIsAFool.WindowsPhone.Logging;
using ThemeManagerRt;
using Viddy.Common;
using Viddy.ViewModel;
using Viddy.ViewModel.Account;

namespace Viddy.Views
{
    public class BasePage : ThemeEnabledPage
    {
        protected readonly ILog Logger;
        private readonly INavigationService _navigationService;

        protected virtual ApplicationViewBoundsMode Mode
        {
            get { return ApplicationViewBoundsMode.UseVisible; }
        }

        public BasePage()
        {
            NavigationCacheMode = NavigationCacheMode.Required;
            Logger = new WinLogger(GetType().FullName);
            _navigationService = SimpleIoc.Default.GetInstance<INavigationService>();
        }

        protected virtual void OnBackKeyPressed(object sender, NavigationServiceBackKeyPressedEventArgs e)
        {
        }

        protected void SetFullScreen(ApplicationViewBoundsMode mode)
        {
            ApplicationView.GetForCurrentView().SetDesiredBoundsMode(mode);
        }

        protected virtual void InitialiseOnBack()
        {
            var backVm = DataContext as IBackSupportedViewModel;
            if (backVm != null)
            {
                backVm.ChangeContext(GetType());
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (_navigationService != null)
            {
                _navigationService.BackKeyPressed += OnBackKeyPressed;
            }

            Logger.Info("Navigated to {0}", GetType().FullName);

            SetFullScreen(Mode);

            if (e.NavigationMode == NavigationMode.Back)
            {
                InitialiseOnBack();
            }

            var parameters = e.Parameter as NavigationParameters;
            if (parameters != null)
            {
                if (parameters.ClearBackstack)
                {
                    Logger.Info("Clearing backstack");
                    Frame.BackStack.Clear();
                }
            }

            var vm = DataContext as ICanHasHomeButton;
            if (vm != null)
            {
                vm.ShowHomeButton = parameters != null && parameters.ShowHomeButton;
            }

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            //var vm = DataContext as IBackSupportedViewModel;
            //if (vm != null)
            //{
            //    if (e.NavigationMode == NavigationMode.Back)
            //    {
            //        vm.ChangeContext(GetType());
            //    }
            //}

            if (_navigationService != null)
            {
                _navigationService.BackKeyPressed -= OnBackKeyPressed;
            }

            if (e.NavigationMode != NavigationMode.Back)
            {
                var parameters = e.Parameter as NavigationParameters;
                if (parameters != null && parameters.RemoveCurrentPageFromBackstack)
                {
                    var page = Frame.BackStack.FirstOrDefault(x => x.SourcePageType == GetType());
                    if (page != null)
                    {
                        Frame.BackStack.Remove(page);
                    }
                }
            }

            //Logger.Info("Navigated from {0}", GetType().FullName);
            base.OnNavigatedFrom(e);
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
