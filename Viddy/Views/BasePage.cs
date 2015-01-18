using System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using ScottIsAFool.WindowsPhone.Logging;
using ThemeManagerRt;
using Viddy.Common;
using Viddy.Helpers;

namespace Viddy.Views
{
    public class BasePage : ThemeEnabledPage
    {
        private readonly NavigationHelper _navigationHelper;
        protected readonly ILog Logger;

        public BasePage()
        {
            NavigationCacheMode = NavigationCacheMode.Required;
            _navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += NavigationHelperLoadState;
            _navigationHelper.SaveState += NavigationHelperSaveState;
            Logger = new WinLogger(GetType().FullName);
            ApplicationView.GetForCurrentView().SetDesiredBoundsMode(ApplicationViewBoundsMode.UseVisible);
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return _navigationHelper; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        protected virtual void NavigationHelperLoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        protected virtual void NavigationHelperSaveState(object sender, SaveStateEventArgs e)
        {
        }

        protected virtual void InitialiseOnBack()
        {
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Logger.Info("Navigated to {0}", GetType().FullName);

            if (e.NavigationMode == NavigationMode.Back)
            {
                InitialiseOnBack();
            }
            else
            {
                var parameters = e.Parameter as NavigationParameters;
                if (parameters != null && parameters.ClearBackstack)
                {
                    Logger.Info("Clearing backstack");
                    Frame.SetNavigationState("1,0");
                }
            }

            _navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            //Logger.Info("Navigated from {0}", GetType().FullName);
            _navigationHelper.OnNavigatedFrom(e);
        }
    }
}
