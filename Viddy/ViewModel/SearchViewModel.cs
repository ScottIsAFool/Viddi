using System.Threading.Tasks;
using System.Windows.Input;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ScottIsAFool.Windows.Core.ViewModel;
using Viddi.Core;
using Viddi.Messaging;
using Viddi.Views;
using VidMePortable;
using VidMePortable.Model.Responses;

namespace Viddi.ViewModel
{
    public class SearchViewModel : VideoLoadingViewModel, ICanHasHomeButton
    {
        private readonly INavigationService _navigationService;
        private readonly IVidMeClient _vidMeClient;

        public SearchViewModel(INavigationService navigationService, IVidMeClient vidMeClient)
        {
            _navigationService = navigationService;
            _vidMeClient = vidMeClient;
        }

        public string SearchText { get; set; }
        public bool IncludeNsfw { get; set; }

        public bool CanSearch
        {
            get { return !ProgressIsVisible && !string.IsNullOrEmpty(SearchText); }
        }

        public RelayCommand SearchCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await LoadData(true);
                }, () => CanSearch);
            }
        }

        public override Task<VideosResponse> GetVideos(int offset)
        {
            return _vidMeClient.SearchVideosAsync(SearchText, offset, includeNsfw: IncludeNsfw);
        }

        public override void UpdateProperties()
        {
            RaisePropertyChanged(() => CanSearch);
        }

        protected override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.Messages.ClearSearchMsg))
                {
                    Reset();
                }
            });

            Messenger.Default.Register<ProtocolMessage>(this, m =>
            {
                if (m.Type != ProtocolMessage.ProtocolType.Search)
                {
                    return;
                }

                SearchText = m.Content;
                IncludeNsfw = m.SecondaryContent;
                SearchCommand.Execute(null);
            });

            base.WireMessages();
        }

        protected override void Reset()
        {
            SearchText = string.Empty;
            IncludeNsfw = false;
            base.Reset();
        }

        #region ICanHasHomeButton implementation
        public bool ShowHomeButton { get; set; }

        public ICommand NavigateHomeCommand
        {
            get
            {
                return new RelayCommand(() => _navigationService.Navigate<MainView>());
            }
        }
        #endregion
    }
}
