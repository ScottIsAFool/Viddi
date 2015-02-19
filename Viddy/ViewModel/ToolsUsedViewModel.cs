using System;
using System.Collections.Generic;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using Viddy.Model;

namespace Viddy.ViewModel
{
    public class ToolsUsedViewModel : ViewModelBase
    {
        private readonly ILauncherService _launcherService;

        public ToolsUsedViewModel(ILauncherService launcherService)
        {
            _launcherService = launcherService;

            Tools = Tool.GetTools();
        }

        public List<Tool> Tools { get; set; }

        public RelayCommand<Tool> OpenToolCommand
        {
            get
            {
                return new RelayCommand<Tool>(tool => _launcherService.LaunchUriAsync(new Uri(tool.Url, UriKind.Absolute)));
            }
        }
    }
}
