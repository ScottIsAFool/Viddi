using System;
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

        private Video _video;

        public EditVideoViewModel(INavigationService navigationService, IVidMeClient vidMeClient)
        {
            _navigationService = navigationService;
            _vidMeClient = vidMeClient;
        }

        public void SetVideo(Video video)
        {
            _video = video;
            CanEdit = true;
        }

        public bool CanEdit { get; set; }
        public bool IsNsfw { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public bool IsChanged { get; set; }

        public RelayCommand SaveChangesCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    try
                    {
                        var request = new VideoRequest();

                        if (!Title.Equals(_video.Title))
                        {
                            request.Title = Title;
                        }

                        if (!Description.Equals(_video.Description))
                        {
                            request.Description = Description;
                        }

                        if (IsNsfw)
                        {
                            request.Title += " NSFW";
                        }

                        var response = await _vidMeClient.EditVideoAsync(_video.VideoId, request);
                        
                        if (response != null && IsNsfw && response.Nsfw && response.Title.EndsWith(" NSFW"))
                        {
                            request = new VideoRequest
                            {
                                Title = response.Title.Substring(0, response.Title.Length - 4).Trim()
                            };

                            response = await _vidMeClient.EditVideoAsync(_video.VideoId, request);
                        }

                        if (response != null)
                        {
                            
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }, () => IsChanged);
            }
        }
    }
}
