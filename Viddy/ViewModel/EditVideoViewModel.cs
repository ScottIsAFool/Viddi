using System;
using System.Collections.Generic;
using Windows.ApplicationModel.DataTransfer;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using Viddy.Common;
using Viddy.Core.Services;
using Viddy.Views.Account;
using VidMePortable;
using VidMePortable.Model;
using VidMePortable.Model.Requests;

namespace Viddy.ViewModel
{
    public class EditVideoViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IVidMeClient _vidMeClient;
        private readonly IMessageBoxService _messageBoxService;
        private readonly ILocalisationLoader _localisationLoader;
        private DataTransferManager _manager;

        private Video _video;

        public EditVideoViewModel(INavigationService navigationService, IVidMeClient vidMeClient, IMessageBoxService messageBoxService, ILocalisationLoader localisationLoader)
        {
            _navigationService = navigationService;
            _vidMeClient = vidMeClient;
            _messageBoxService = messageBoxService;
            _localisationLoader = localisationLoader;

            if (IsInDesignMode)
            {
                CanEdit = true;
            }
        }

        public void SetVideo(Video video)
        {
            _video = video;
            Title = _video.Title;
            Description = _video.Description;
            IsNsfw = _video.Nsfw;
            CanEdit = true;
        }

        public string ThumbnailImage { get; set; }

        public void SetIsUploading(bool isUploading)
        {
            IsUploading = isUploading;
        }

        public bool CanEdit { get; set; }
        public bool IsNsfw { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public bool CanSetNsfw
        {
            get { return _video != null && !_video.Nsfw; }
        }

        public bool IsChanged { get; set; }
        public string ErrorMessage { get; set; }

        public bool IsUploading { get; private set; }

        public bool CanSave
        {
            get { return !ProgressIsVisible && IsChanged; }
        }

        public void TryGoingBack()
        {
            if (!IsUploading)
            {
                if (_navigationService.CanGoBack)
                {
                    _navigationService.GoBack();
                }

                return;
            }

            _messageBoxService.ShowAsync(_localisationLoader.GetString("ErrorStillUploadingBack"), _localisationLoader.GetString("ErrorTitle"), new List<string> { _localisationLoader.GetString("Ok") });
        }

        public RelayCommand SaveChangesCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    try
                    {
                        var request = new VideoRequest();

                        if (!string.IsNullOrEmpty(Title) && !Title.Equals(_video.Title))
                        {
                            request.Title = Title;
                        }

                        if (!string.IsNullOrEmpty(Description) && !Description.Equals(_video.Description))
                        {
                            request.Description = Description;
                        }

                        if (IsNsfw && !_video.Nsfw)
                        {
                            request.IsNsfw = true;
                        }

                        var response = await _vidMeClient.EditVideoAsync(_video.VideoId, request);

                        if (response != null)
                        {
                            IsChanged = false;
                            RaisePropertyChanged(() => CanSetNsfw);

                            if (_navigationService.CanGoBack)
                            {
                                _navigationService.GoBack();
                            }
                            else
                            {
                                _navigationService.Navigate<AccountView>(new NavigationParameters { RemoveCurrentPageFromBackstack = true });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.ErrorException("SaveChanges", ex);
                        ErrorMessage = "There was an error updating your video";
                    }
                }, () => CanSave);
            }
        }

        public RelayCommand ShareCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (_video == null)
                    {
                        // TODO: display error
                        return;
                    }
                    if (IsUploading)
                    {
                        _messageBoxService.ShowAsync(_localisationLoader.GetString("ErrorStillUploadingShare"), _localisationLoader.GetString("ErrorTitle"), new List<string> { _localisationLoader.GetString("Ok") });
                        return;
                    }

                    _manager = DataTransferManager.GetForCurrentView();
                    _manager.DataRequested += ManagerOnDataRequested;
                    DataTransferManager.ShowShareUI();
                });
            }
        }

        private void ManagerOnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            _manager.DataRequested -= ManagerOnDataRequested;
            var request = args.Request;
            request.Data.Properties.Title = !string.IsNullOrEmpty(_video.Title) ? _video.Title : "Check out my video";
            var message = "Check out my video";
            request.Data.Properties.Description = message;
            request.Data.SetUri(new Uri(_video.FullUrl));
        }

        public override void UpdateProperties()
        {
            RaisePropertyChanged(() => CanSave);
        }
    }
}
