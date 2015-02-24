using System;
using System.Collections.Generic;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using Viddy.Core;
using Viddy.Core.Services;
using VidMePortable;
using VidMePortable.Model;

namespace Viddy.ViewModel.Account.Manage
{
    public class RevokeAppViewModel : ViewModelBase
    {
        private readonly IVidMeClient _vidMeClient;
        private readonly ManageAppsAccessViewModel _manageAppsAccessViewModel;
        private readonly IMessageBoxService _messageBoxService;
        private readonly ILocalisationLoader _localisationLoader;
        public Application Application { get; set; }

        public RevokeAppViewModel(Application application, IVidMeClient vidMeClient, ManageAppsAccessViewModel manageAppsAccessViewModel, IMessageBoxService messageBoxService, ILocalisationLoader localisationLoader)
        {
            _vidMeClient = vidMeClient;
            _manageAppsAccessViewModel = manageAppsAccessViewModel;
            _messageBoxService = messageBoxService;
            _localisationLoader = localisationLoader;
            Application = application;
        }

        public RelayCommand<RevokeAppViewModel> RevokeTokenCommand
        {
            get
            {
                return new RelayCommand<RevokeAppViewModel>(async appVm =>
                {
                    try
                    {
                        var app = appVm.Application;
                        if (app.ClientId == Constants.ClientId)
                        {
                            var result = await _messageBoxService.ShowAsync(
                                _localisationLoader.GetString("RevokeViddyTokenBody"), 
                                _localisationLoader.GetString("MessageAreYouSureTitle"),
                                new List<string> { _localisationLoader.GetString("Yes"), _localisationLoader.GetString("No") });
                            if (result == 1)
                            {
                                return;
                            }
                        }

                        SetProgressBar("Revoking token...");

                        if (await _vidMeClient.RevokeAppTokenAsync(app.ClientId))
                        {
                            if (app.ClientId == Constants.ClientId)
                            {
                                _manageAppsAccessViewModel.ResetApp();
                            }
                            else
                            {
                                _manageAppsAccessViewModel.Items.Remove(appVm);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }

                    SetProgressBar();
                });
            }
        }
    }
}
