using System;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Messaging;
using VidMePortable;

namespace Viddy.ViewModel
{
    public class EditVideoViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IVidMeClient _vidMeClient;

        private StorageFile _SelectedVideoFile;

        public EditVideoViewModel(INavigationService navigationService, IVidMeClient vidMeClient)
        {
            _navigationService = navigationService;
            _vidMeClient = vidMeClient;
        }

        public StorageItemThumbnail Thumbnail { get; set; }

        
    }
}
