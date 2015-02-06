using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using Viddy.Extensions;
using Viddy.Services;
using Viddy.ViewModel.Account;
using Viddy.ViewModel.Account.Manage;
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

            // This needs to be called this way because VidMeClient has multiple constructors
            SimpleIoc.Default.RegisterIf<IVidMeClient>(() => new VidMeClient());
            SimpleIoc.Default.RegisterIf<IApplicationSettingsService, ApplicationSettingsService>();
            SimpleIoc.Default.RegisterIf<AuthenticationService>(true);
            SimpleIoc.Default.RegisterIf<ReviewService>(true);
            SimpleIoc.Default.RegisterIf<INavigationService, NavigationService>();
            SimpleIoc.Default.RegisterIf<ICameraInfoService, CameraInfoService>();
            SimpleIoc.Default.RegisterIf<IMessageBoxService, MessageBoxService>();
            SimpleIoc.Default.RegisterIf<IToastService, ToastService>();
            SimpleIoc.Default.RegisterIf<ITileService, TileService>();

            SimpleIoc.Default.Register<AvatarViewModel>();
            SimpleIoc.Default.Register<VideoRecordViewModel>();
            SimpleIoc.Default.Register<AccountViewModel>();
            SimpleIoc.Default.Register<EditVideoViewModel>();
            SimpleIoc.Default.Register<UploadVideoViewModel>();
            SimpleIoc.Default.Register<ManageAccountViewModel>();
            SimpleIoc.Default.Register<ManageAppsAccessViewModel>();
            SimpleIoc.Default.Register<ManageMyAppsViewModel>();
            SimpleIoc.Default.Register<AddAppViewModel>();
            SimpleIoc.Default.Register<ManualLoginViewModel>();
            SimpleIoc.Default.Register<CreateAccountViewModel>();
            SimpleIoc.Default.Register<ProfileViewModel>(true);
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<VideoPlayerViewModel>(true);
            SimpleIoc.Default.Register<SearchViewModel>();
            SimpleIoc.Default.Register<ChannelViewModel>(true);
            SimpleIoc.Default.Register<SettingsViewModel>();
            SimpleIoc.Default.Register<BrowseChannelsViewModel>();
            SimpleIoc.Default.Register<EditProfileViewModel>();
        }

        public MainViewModel Main
        {
            get { return ServiceLocator.Current.GetInstance<MainViewModel>(); }
        }

        public VideoRecordViewModel VideoRecord
        {
            get { return ServiceLocator.Current.GetInstance<VideoRecordViewModel>(); }
        }

        public AccountViewModel Account
        {
            get { return ServiceLocator.Current.GetInstance<AccountViewModel>(); }
        }

        public EditVideoViewModel EditVideo
        {
            get { return ServiceLocator.Current.GetInstance<EditVideoViewModel>(); }
        }

        public UploadVideoViewModel Upload
        {
            get { return ServiceLocator.Current.GetInstance<UploadVideoViewModel>(); }
        }

        public ManageAccountViewModel ManageAccount
        {
            get { return ServiceLocator.Current.GetInstance<ManageAccountViewModel>(); }
        }

        public ManageAppsAccessViewModel ManageApps
        {
            get { return ServiceLocator.Current.GetInstance<ManageAppsAccessViewModel>(); }
        }

        public ManageMyAppsViewModel MyApps
        {
            get { return ServiceLocator.Current.GetInstance<ManageMyAppsViewModel>(); }
        }

        public ManualLoginViewModel ManualLogin
        {
            get { return ServiceLocator.Current.GetInstance<ManualLoginViewModel>(); }
        }

        public CreateAccountViewModel CreateAccount
        {
            get { return ServiceLocator.Current.GetInstance<CreateAccountViewModel>(); }
        }

        public ProfileViewModel Profile
        {
            get { return ServiceLocator.Current.GetInstance<ProfileViewModel>(); }
        }

        public AddAppViewModel AddApp
        {
            get { return ServiceLocator.Current.GetInstance<AddAppViewModel>(); }
        }

        public VideoPlayerViewModel VideoPlayer
        {
            get { return ServiceLocator.Current.GetInstance<VideoPlayerViewModel>(); }
        }

        public SearchViewModel Search
        {
            get { return ServiceLocator.Current.GetInstance<SearchViewModel>(); }
        }

        public ChannelViewModel Channel
        {
            get { return ServiceLocator.Current.GetInstance<ChannelViewModel>(); }
        }

        public SettingsViewModel Settings
        {
            get { return ServiceLocator.Current.GetInstance<SettingsViewModel>(); }
        }

        public BrowseChannelsViewModel BrowseChannels
        {
            get { return ServiceLocator.Current.GetInstance<BrowseChannelsViewModel>(); }
        }

        public EditProfileViewModel EditProfile
        {
            get { return ServiceLocator.Current.GetInstance<EditProfileViewModel>(); }
        }

        public AuthenticationService Auth
        {
            get { return ServiceLocator.Current.GetInstance<AuthenticationService>(); }
        }

        public ReviewService Review
        {
            get { return ServiceLocator.Current.GetInstance<ReviewService>(); }
        }
        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}