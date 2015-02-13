using System.Collections.Generic;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using Viddy.Model;
using Viddy.Views;

namespace Viddy.ViewModel
{
    public class FoursqureViewModel : ViewModelBase
    {
        private readonly ISettingsService _settingsService;
        private readonly INavigationService _navigationService;
        

        public FoursqureViewModel(ISettingsService settingsService, INavigationService navigationService)
        {
            _settingsService = settingsService;
            _navigationService = navigationService;
        }

        public async Task GetLocations()
        {
            if (!_settingsService.LocationIsOn)
            {
                LocationText = "Turn location on";
            }

            LocationText = "Finding you...";

            double longitude = 40.7, latitude = -74;

            var options = new Dictionary<string, string> {{"ll", string.Format("{0},{1}", longitude, latitude)}, {"limit", "10"}};

        }

        public List<string> Locations { get; set; }

        public string LocationText { get; set; }

        public RelayCommand TurnLocationOn
        {
            get { return new RelayCommand(() => _navigationService.Navigate<SettingsView>());}
        }

        private string GetSearchUrl(double longitude, double latitude)
        {
            return string.Format(Constants.FourSquareSearchUrl, Constants.FourSquareClientId, Constants.FourSquareClientSecret, longitude, latitude);
        }
    }
}
