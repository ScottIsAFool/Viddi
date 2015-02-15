using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using Viddy.Core.Extensions;
using Viddy.Extensions;
using Viddy.Foursquare;
using Viddy.Model;
using Viddy.Views;

namespace Viddy.ViewModel
{
    public class FoursqureViewModel : ViewModelBase
    {
        private readonly ISettingsService _settingsService;
        private readonly INavigationService _navigationService;
        private readonly FoursquareClient _foursquareClient;

        private Geopoint _curentLocation;

        public FoursqureViewModel(ISettingsService settingsService, INavigationService navigationService)
        {
            _settingsService = settingsService;
            _navigationService = navigationService;
            _foursquareClient = new FoursquareClient();
        }

        public async Task GetLocations()
        {
            if (!_settingsService.LocationIsOn)
            {
                LocationText = "Turn location on";
                return;
            }

            LocationText = "Finding you...";

            var position = await GetCurrentLocation();
            if (position == null)
            {
                LocationText = "Failed to find you";
                return;
            }

            _curentLocation = position.Point;

            var venues = await _foursquareClient.GetVenuesAsync(_curentLocation.Position.Longitude, _curentLocation.Position.Latitude);
            if (venues.IsNullOrEmpty())
            {
                LocationText = "Nothing nearby";
                return;
            }

            Locations = venues;
            SelectedVenue = Locations.FirstOrDefault();
            LocationText = SelectedVenue != null ? SelectedVenue.Name : "Add location?";
            ShowVenues = true;
        }

        public double? Longitude
        {
            get
            {
                if (_curentLocation != null)
                {
                    return _curentLocation.Position.Longitude;
                }

                return null;
            }
        }

        public double? Latitude
        {
            get
            {
                if (_curentLocation != null)
                {
                    return _curentLocation.Position.Latitude;
                }

                return null;
            }
        }

        public string VenueId
        {
            get { return SelectedVenue != null ? SelectedVenue.Id : null; }
        }

        public string VenueName
        {
            get { return SelectedVenue != null ? SelectedVenue.Name : null; }
        }

        private async Task<Geocoordinate> GetCurrentLocation()
        {
            var locator = new Geolocator
            {
                DesiredAccuracyInMeters = 30,
            };

            try
            {
                var position = await locator.GetGeopositionAsync(TimeSpan.FromMinutes(5), TimeSpan.FromSeconds(10));

                if (position != null)
                {
                    return position.Coordinate;
                }
            }
            catch (Exception ex)
            {
            }

            return null;
        }

        public List<Venue> Locations { get; set; }
        public Venue SelectedVenue { get; set; }

        public bool ShowVenues { get; set; }

        public bool HasVenue
        {
            get { return SelectedVenue != null; }
        }

        public string LocationText { get; set; }

        public RelayCommand<Venue> VenueTappedCommand
        {
            get
            {
                return new RelayCommand<Venue>(venue =>
                {
                    LocationText = venue.Name;
                    SelectedVenue = venue;
                    ShowVenues = false;
                });
            }
        }

        public RelayCommand ClearLocationCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    SelectedVenue = null;
                    LocationText = "Add location?";
                    ShowVenues = false;
                }, () => _settingsService.LocationIsOn);
            }
        }

        public RelayCommand VenueTextTappedCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (!_settingsService.LocationIsOn)
                    {
                        _navigationService.Navigate<SettingsView>();
                        return;
                    }

                    if (Locations.IsNullOrEmpty())
                    {
                        GetLocations();
                        return;
                    }

                    ShowVenues = !ShowVenues;
                });
            }
        }
    }
}
