using System;
using Windows.ApplicationModel.DataTransfer;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
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
        private DataTransferManager _manager;

        private Video _video;
        private bool _isUploading;

        public EditVideoViewModel(INavigationService navigationService, IVidMeClient vidMeClient, IMessageBoxService messageBoxService)
        {
            _navigationService = navigationService;
            _vidMeClient = vidMeClient;
            _messageBoxService = messageBoxService;
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
            _isUploading = isUploading;
        }

        public bool CanEdit { get; set; }
        public bool IsNsfw { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public bool CanSetNsfw
        {
            get { return !_video.Nsfw; }
        }

        public bool IsChanged { get; set; }

        public bool CanSave
        {
            get { return !ProgressIsVisible && IsChanged; }
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
                        }
                    }
                    catch (Exception ex)
                    {

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
                    if (_isUploading)
                    {
                        // TODO: show "do not exit message"
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
