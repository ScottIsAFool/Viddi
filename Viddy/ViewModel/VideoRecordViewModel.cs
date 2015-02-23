using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Viddy.Core;
using Viddy.Core.Model;
using Viddy.Messaging;
using Viddy.Services;
using Viddy.ViewModel.Account;
using Viddy.ViewModel.Item;
using Viddy.Views;
using Viddy.Views.Account;
using VidMePortable.Model;

namespace Viddy.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class VideoRecordViewModel : ViewModelBase, ICanHasHomeButton
    {
        private readonly INavigationService _navigationService;
        private readonly ICameraInfoService _cameraInfo;
        private readonly ITileService _tileService;
        private readonly IMessageBoxService _messageBoxService;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public VideoRecordViewModel(
            INavigationService navigationService, 
            ICameraInfoService cameraInfo, 
            AvatarViewModel avatar, 
            ITileService tileService, 
            FoursqureViewModel foursquare,
            IMessageBoxService messageBoxService)
        {
            Avatar = avatar;
            _navigationService = navigationService;
            _cameraInfo = cameraInfo;
            _tileService = tileService;
            Foursquare = foursquare;
            _messageBoxService = messageBoxService;
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
                CanTurnOnFlash = true;
                HasFrontFacingCamera = true;
            }

            _cameraInfo.IsInitialisedChanged += CameraInfoOnIsInitialisedChanged;            
        }

        private async void CameraInfoOnIsInitialisedChanged(object sender, EventArgs eventArgs)
        {
            if (_cameraInfo.IsInitialised)
            {
                _cameraInfo.IsInitialisedChanged -= CameraInfoOnIsInitialisedChanged;
                CanTurnOnFlash = await _cameraInfo.HasFlash();
                HasFrontFacingCamera = await _cameraInfo.HasFrontFacingCamera();
            }
        }

        public FoursqureViewModel Foursquare { get; set; }
        public AvatarViewModel Avatar { get; set; }
        public bool CanTurnOnFlash { get; set; }
        public bool HasFrontFacingCamera { get; set; }

        public RelayCommand MainPageLoadedCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await Foursquare.GetLocations();
                });
            }
        }

        public RelayCommand PinToStartCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    
                });
            }
        }

        public override bool IsPinned
        {
            get { return _tileService.IsVideoRecordPinned; }
        }

        public override async Task PinUnpin()
        {
            if (IsPinned)
            {
                await _tileService.UnpinVideoRecord();
            }
            else
            {
                await _tileService.PinVideoRecord();
            }

            RaisePropertyChanged(() => IsPinned);
        }

        public override string GetPinFileName(bool isWideTile = false)
        {
            var filename = _tileService.GetTileFileName(TileType.VideoRecord);
            return filename;
        }

        public RelayCommand NavigateToAccountCommand
        {
            get
            {
                return new RelayCommand(() => _navigationService.Navigate<AccountView>());
            }
        }

        private ChannelItemViewModel _channel;

        public void FinishedRecording(IStorageFile file)
        {
            if (App.Locator.Upload != null)
            {
                Messenger.Default.Send(new NotificationMessage(file, _channel, Constants.Messages.VideoFileMsg));
                SimpleIoc.Default.GetInstance<INavigationService>().Navigate<UploadVideoView>();
            }
        }

        public RelayCommand SelectVideoCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    var filePicker = new FileOpenPicker { ViewMode = PickerViewMode.Thumbnail, SuggestedStartLocation = PickerLocationId.VideosLibrary };
                    filePicker.FileTypeFilter.Add(".avi");
                    filePicker.FileTypeFilter.Add(".mov");
                    filePicker.FileTypeFilter.Add(".mp4");
                    filePicker.FileTypeFilter.Add(".wmv");

                    filePicker.PickSingleFileAndContinue();
                });
            }
        }

        #region ICanHasHomeButton implementation
        public bool ShowHomeButton { get; set; }

        public RelayCommand NavigateHomeCommand
        {
            get
            {
                return new RelayCommand(() => _navigationService.Navigate<MainView>());
            }
        }
        #endregion

        protected override void WireMessages()
        {
            base.WireMessages();
            Messenger.Default.Register<ChannelMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.Messages.AddVideoToChannelMsg))
                {
                    _channel = m.Channel;
                }
            });
        }
    }
}