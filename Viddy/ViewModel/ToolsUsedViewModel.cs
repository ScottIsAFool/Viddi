using System;
using System.Collections.Generic;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using Viddi.Model;

namespace Viddi.ViewModel
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
