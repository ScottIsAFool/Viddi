using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using Viddy.Extensions;
using Viddy.Services;
using VidMePortable;

namespace Viddy.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.RegisterIf<IVidMeClient>(() => new VidMeClient("1A101EA7-7848-4EC2-A140-03E5416B280E", "ios"));
            SimpleIoc.Default.RegisterIf<IApplicationSettingsService, ApplicationSettingsService>();
            SimpleIoc.Default.RegisterIf<AuthenticationService>(true);
            SimpleIoc.Default.RegisterIf<INavigationService, NavigationService>();

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<AccountViewModel>();
        }

        public MainViewModel Main
        {
            get { return ServiceLocator.Current.GetInstance<MainViewModel>(); }
        }

        public AccountViewModel Account
        {
            get { return ServiceLocator.Current.GetInstance<AccountViewModel>(); }
        }

        public AuthenticationService Auth
        {
            get { return ServiceLocator.Current.GetInstance<AuthenticationService>(); }
        }
        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}