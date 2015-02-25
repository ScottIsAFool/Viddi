using System;
using System.Collections.Generic;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using Viddy.Core;
using Viddy.Localisation;
using VidMePortable;
using VidMePortable.Model;

namespace Viddy.ViewModel.Account.Manage
{
    public class RevokeAppViewModel : ViewModelBase
    {
        private readonly IVidMeClient _vidMeClient;
        private readonly ManageAppsAccessViewModel _manageAppsAccessViewModel;
        private readonly IMessageBoxService _messageBoxService;
        public Application Application { get; set; }

        public RevokeAppViewModel(Application application, IVidMeClient vidMeClient, ManageAppsAccessViewModel manageAppsAccessViewModel, IMessageBoxService messageBoxService)
        {
            _vidMeClient = vidMeClient;
            _manageAppsAccessViewModel = manageAppsAccessViewModel;
            _messageBoxService = messageBoxService;
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
                                Resources.RevokeViddyTokenBody, 
                                Resources.MessageAreYouSureTitle,
                                new List<string> { Resources.YesText, Resources.NoText });
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
